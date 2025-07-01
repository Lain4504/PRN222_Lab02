using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HuynhNgocTien_SE18B01_A02.Services;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.Pages.NewsArticle
{
    public class PublicDetailsModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public PublicDetailsModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public Models.NewsArticle Article { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var article = await _newsService.GetByIdAsync(id);
            
            if (article == null || article.NewsStatus != true)
            {
                return NotFound();
            }

            Article = article;
            return Page();
        }
    }
}
