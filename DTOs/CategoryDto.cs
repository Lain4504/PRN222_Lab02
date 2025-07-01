namespace HuynhNgocTien_SE18B01_A02.DTOs
{
    public class CategoryDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int NewsArticleCount { get; set; }
    }
}
