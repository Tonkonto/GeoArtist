using GeoComponent.Core.Models;
using GeoComponent.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoComponent.ViewComponents;

public class GeoMapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        GeoResult? geo = null,
        IEnumerable<GeoResult>? geoBatch = null,
        GeoMapOptions? options = null )
    {
        var vm = new GeoMapVm
        {
            Geo = geo,
            GeoBatch = geoBatch,
            Options = options ?? new GeoMapOptions()
        };

        return View("~/Views/Shared/Components/GeoMap/Default.cshtml", vm);
    }
}