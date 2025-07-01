using HuynhNgocTien_SE18B01_A02.Models;
using HuynhNgocTien_SE18B01_A02.DTOs;

namespace HuynhNgocTien_SE18B01_A02.Extensions
{
    public static class ModelExtensions
    {
        public static NewsArticleDto ToDto(this NewsArticle article)
        {
            return new NewsArticleDto
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.CategoryName,
                NewsStatus = article.NewsStatus,
                CreatedById = article.CreatedById,
                CreatedByName = article.CreatedBy?.AccountName,
                ModifiedDate = article.ModifiedDate,
                TagNames = article.Tags?.Select(t => t.TagName ?? "").ToList() ?? new List<string>()
            };
        }

        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.CategoryName,
                IsActive = category.IsActive,
                NewsArticleCount = category.NewsArticles?.Count ?? 0
            };
        }

        public static SystemAccountDto ToDto(this SystemAccount account)
        {
            var roleNames = new Dictionary<int, string>
            {
                { 1, "Member" },
                { 2, "Staff" },
                { 3, "Admin" }
            };

            return new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountRoleName = account.AccountRole.HasValue && roleNames.ContainsKey(account.AccountRole.Value) 
                    ? roleNames[account.AccountRole.Value] 
                    : "Unknown",
                NewsArticleCount = account.NewsArticles?.Count ?? 0
            };
        }
    }
}
