using System.ComponentModel.DataAnnotations;

namespace WebView.Models.API;

public class GeoJsonBatchRequest
{
    [Required, MinLength(1)]
    public List<string> GeoJsonList { get; set; } = [];
}
