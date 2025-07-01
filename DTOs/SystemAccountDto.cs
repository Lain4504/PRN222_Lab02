namespace HuynhNgocTien_SE18B01_A02.DTOs
{
    public class SystemAccountDto
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string? AccountRoleName { get; set; }
        public int NewsArticleCount { get; set; }
    }
}
