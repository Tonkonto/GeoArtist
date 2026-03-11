using GeoComponent.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoComponent.ViewComponents;

public class GeoMapViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GeoMap model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Options ??= new GeoMapOptions();

        return View("~/Views/Shared/Components/GeoMap/Default.cshtml", model);
    }
}