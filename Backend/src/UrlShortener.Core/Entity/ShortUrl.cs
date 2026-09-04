namespace UrlShortener.Core.Entity;

public class ShortUrl
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; } = string.Empty;

    public string ShortCode { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int ClickCount { get; set; }
}