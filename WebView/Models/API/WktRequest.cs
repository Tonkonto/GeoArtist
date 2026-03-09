namespace WebView.Models.API
{
    public class WktRequest
    {
        public string? Wkt { get; set; }
        public List<string>? WktList { get; set; }
        public int? Srid { get; set; }
    }
}
