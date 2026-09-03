namespace UrlShortener.Core.Entity;

public class AboutPageContent
{
    public int Id { get; set; }

    public string Content { get; set; }

    public int LastModifiedByUserId { get; set; }

    public DateTime LastModifiedDate { get; set; }
}
