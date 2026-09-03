namespace UrlShortener.Core.Entity;

using System;

public class ShortUrl
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; }

    public string ShortCode { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int ClickCount { get; set; }
}
