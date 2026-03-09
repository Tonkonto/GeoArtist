using GeoComponent.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoComponent.ViewComponents;

public class GeoMapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GeoMapComponentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var vm = new GeoMapVm
        {
            Geo = model.Geo,
            GeoBatch = model.GeoBatch,
            Options = model.Options ?? new GeoMapOptions()
        };

        return View("~/Views/Shared/Components/GeoMap/Default.cshtml", vm);
    }
}