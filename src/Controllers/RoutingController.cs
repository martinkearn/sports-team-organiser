using Microsoft.AspNetCore.Mvc;

[Route("controllers/[controller]/[action]")]
public class RoutingController : Controller
{
    public IActionResult LatestGame(int id)
    {
        return Redirect("https://bing.com");
    }
}