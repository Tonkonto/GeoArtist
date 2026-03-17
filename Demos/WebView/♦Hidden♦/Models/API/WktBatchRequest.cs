using System.ComponentModel.DataAnnotations;

namespace WebView.Models.API;

public class WktBatchRequest
{
    [Required, MinLength(1)]
    public List<string> WktList { get; set; } = [];

    [Range(1, int.MaxValue)]
    public int Srid { get; set; }
}
