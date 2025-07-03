using System.ComponentModel.DataAnnotations;
using HuynhNgocTien_SE18B01_A02.Models;

namespace HuynhNgocTien_SE18B01_A02.ViewModels
{
    public class CategoryViewModel
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string CategoryDesciption { get; set; } = string.Empty;

        [Display(Name = "Parent Category")]
        public short? ParentCategoryId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }

    public class CategoryStatsViewModel
    {
        public Category Category { get; set; } = null!;
        public int ArticleCount { get; set; }
    }
}
