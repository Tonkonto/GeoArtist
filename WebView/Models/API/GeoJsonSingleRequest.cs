using System.ComponentModel.DataAnnotations;

namespace WebView.Models.API
{
    public class GeoJsonSingleRequest
    {
        [Required]
        public string GeoJson { get; set; } = default!;
    }
}
