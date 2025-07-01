namespace HuynhNgocTien_SE18B01_A02.DTOs
{
    public class NewsArticleDto
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();
    }
}
