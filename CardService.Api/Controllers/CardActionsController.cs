using Microsoft.AspNetCore.Mvc;

namespace CardService.Api.Controllers
{
    public class CardActionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
