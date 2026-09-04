namespace UrlShortener.Application.Dto;

public record ShortUrlDetailsResponse(
    int Id,
    string ShortCode,
    string OriginalUrl,
    string CreatedByUserName,
    DateTime CreatedDate,
    int ClickCount,
    bool CanDelete);
