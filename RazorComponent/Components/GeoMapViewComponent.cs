using Core.Models;
using Microsoft.AspNetCore.Mvc;
using RazorComponent.Models;

namespace RazorComponent.Components;

public class GeoMapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        GeoResult? geo = null,
        IEnumerable<GeoResult>? geoBatch = null,
        string mapId = "geoMap",
        string height = "500px",
        string polygonColor = "#3388ff",
        double polygonOpacity = 0.5)
    {
        var vm = new GeoMapVm
        {
            Geo = geo,
            GeoBatch = geoBatch,
            MapId = mapId,
            Height = height,
            PolygonColor = polygonColor,
            PolygonOpacity = polygonOpacity
        };

        return View("~/Views/Shared/Components/GeoMap/Default.cshtml", vm);
    }
}