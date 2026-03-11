using System.ComponentModel.DataAnnotations;

namespace WebView.Models.API
{
    public class WktSingleRequest
    {
        [Required]
        public string Wkt { get; set; } = default!;

        [Range(1, int.MaxValue)]
        public int Srid { get; set; }
    }
}
