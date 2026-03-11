namespace WebView.Models.API;

public class ApiErrorResponse
{
    public string Error { get; set; } = default!;
    public string? Details { get; set; }
}
