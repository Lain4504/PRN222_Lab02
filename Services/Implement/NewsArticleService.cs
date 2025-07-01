using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.Repositories;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Hubs;
using HuynhNgocTien_SE18B01_A02.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace HuynhNgocTien_SE18B01_A02.Services.Implement;

public class NewsArticleService : INewsArticleService
{
    private readonly INewsArticleRepository _newsRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IHubContext<NewsArticleHub> _hubContext;

    public NewsArticleService(INewsArticleRepository newsRepository, ITagRepository tagRepository, IHubContext<NewsArticleHub> hubContext)
    {
        _newsRepository = newsRepository;
        _tagRepository = tagRepository;
        _hubContext = hubContext;
    }

    public async Task<NewsArticle?> GetByIdAsync(string id)
    {
        return await _newsRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<NewsArticle>> GetAllAsync()
    {
        return await _newsRepository.GetAllAsync();
    }

    public async Task<IEnumerable<NewsArticle>> GetActiveAsync()
    {
        var allNews = await _newsRepository.GetAllAsync();
        return allNews.Where(n => n.NewsStatus == true);
    }

    public async Task<IEnumerable<NewsArticle>> GetByCategoryAsync(short categoryId)
    {
        return await _newsRepository.GetByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<NewsArticle>> GetByAuthorAsync(short authorId)
    {
        return await _newsRepository.GetByAuthorAsync(authorId);
    }

    public async Task<IEnumerable<NewsArticle>> GetByTagAsync(int tagId)
    {
        return await _newsRepository.GetByTagAsync(tagId);
    }

    public async Task<NewsArticle> CreateAsync(NewsArticle article, IEnumerable<int> tagIds)
    {
        // Validate tags
        foreach (var tagId in tagIds)
        {
            if (!await _tagRepository.ExistsAsync(tagId))
            {
                throw new InvalidOperationException($"Tag with ID {tagId} does not exist");
            }
        }

        // Set creation date
        article.CreatedDate = DateTime.Now;

        // Create article
        var createdArticle = await _newsRepository.AddAsync(article);

        // Add tags
        var tags = await _tagRepository.GetAllAsync();
        createdArticle.Tags = tags.Where(t => tagIds.Contains(t.TagId)).ToList();
        await _newsRepository.UpdateAsync(createdArticle);

        // Send SignalR notification
        try
        {
            var articleDto = createdArticle.ToDto();
            await _hubContext.Clients.Group("NewsArticles").SendAsync("ArticleCreated", articleDto);
            if (articleDto.CategoryId.HasValue)
            {
                await _hubContext.Clients.Group($"Category_{articleDto.CategoryId}").SendAsync("ArticleCreated", articleDto);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return createdArticle;
    }

    public async Task<NewsArticle> UpdateAsync(NewsArticle article, IEnumerable<int> tagIds)
    {
        var existingArticle = await _newsRepository.GetByIdAsync(article.NewsArticleId);
        if (existingArticle == null)
        {
            throw new InvalidOperationException("News article not found");
        }

        // Validate tags
        foreach (var tagId in tagIds)
        {
            if (!await _tagRepository.ExistsAsync(tagId))
            {
                throw new InvalidOperationException($"Tag with ID {tagId} does not exist");
            }
        }

        // Update article
        article.ModifiedDate = DateTime.Now;
        var updatedArticle = await _newsRepository.UpdateAsync(article);

        // Update tags
        var tags = await _tagRepository.GetAllAsync();
        updatedArticle.Tags = tags.Where(t => tagIds.Contains(t.TagId)).ToList();
        var finalUpdatedArticle = await _newsRepository.UpdateAsync(updatedArticle);

        // Send SignalR notification
        try
        {
            var articleDto = finalUpdatedArticle.ToDto();
            await _hubContext.Clients.Group("NewsArticles").SendAsync("ArticleUpdated", articleDto);
            if (articleDto.CategoryId.HasValue)
            {
                await _hubContext.Clients.Group($"Category_{articleDto.CategoryId}").SendAsync("ArticleUpdated", articleDto);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }

        return finalUpdatedArticle;
    }

    public async Task DeleteAsync(string id)
    {
        var article = await _newsRepository.GetByIdAsync(id);
        if (article == null)
        {
            throw new InvalidOperationException("News article not found");
        }

        var categoryId = article.CategoryId; // Store for SignalR notification

        // Clear the tags collection first
        article.Tags.Clear();
        await _newsRepository.UpdateAsync(article);

        // Now delete the article
        await _newsRepository.DeleteAsync(id);

        // Send SignalR notification
        try
        {
            await _hubContext.Clients.Group("NewsArticles").SendAsync("ArticleDeleted", id);
            if (categoryId.HasValue)
            {
                await _hubContext.Clients.Group($"Category_{categoryId}").SendAsync("ArticleDeleted", id);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the operation
            Console.WriteLine($"SignalR notification failed: {ex.Message}");
        }
    }

    public async Task<IEnumerable<NewsArticle>> SearchAsync(string searchTerm)
    {
        var allNews = await _newsRepository.GetAllAsync();
        return allNews.Where(n =>
            n.NewsTitle?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
            n.Headline.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            n.NewsContent?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var allNews = await _newsRepository.GetAllAsync();
        return allNews
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _newsRepository.ExistsAsync(id);
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        return await _newsRepository.GetAllTagsAsync();
    }
} 