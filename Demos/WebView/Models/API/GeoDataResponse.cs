namespace WebView.Models.API;

public sealed class GeoDataResponse
{
    public string GeometryType { get; set; } = "None";
    public int CoordinateCount { get; set; }
    public string GeoJson { get; set; } = "";
    public bool IsValid { get; set; }
}

