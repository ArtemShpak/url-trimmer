namespace UrlShortener.Core.Entity;

public class AboutPageContent
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;
    public int LastModifiedByUserId { get; set; }

    public DateTime LastModifiedDate { get; set; }
}
