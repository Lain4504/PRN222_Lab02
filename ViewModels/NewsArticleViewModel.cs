using System.ComponentModel.DataAnnotations;

namespace HuynhNgocTien_SE18B01_A02.ViewModels
{
    public class NewsArticleViewModel
    {
        public short NewsArticleId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "News Title")]
        public string NewsTitle { get; set; } = string.Empty;

        [Display(Name = "Headline")]
        public string? Headline { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "News Content")]
        public string NewsContent { get; set; } = string.Empty;

        [Display(Name = "News Source")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public short CategoryId { get; set; }

        [Display(Name = "Tags")]
        public string? Tags { get; set; }

        [Display(Name = "News Status")]
        public bool NewsStatus { get; set; } = true;

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public short? CreatedById { get; set; }
    }
}
