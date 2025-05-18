using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarSalesSite.Models;

using Microsoft.EntityFrameworkCore;

namespace CarSalesSite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
    {
        _logger = logger;
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        var cars = _context.Cars.Include(c => c.Brand).ToList();
        ViewBag.Brands = _context.Brands.OrderBy(b => b.Name).ToList(); // Список брендов
        return View(cars);
    }

    [HttpPost]
    public IActionResult SaveCallback(string name, string contactMethod, string phone,
    string comment, string promocode)
    {
        try
        {
            var data = $"{DateTime.Now}: {name}, {contactMethod}, {phone}, {comment}, {promocode}\n";
            var path = Path.Combine(_env.WebRootPath, "callbacks.txt");
            System.IO.File.AppendAllText(path, data);

            TempData["Message"] = "Заявка принята! Мы вам перезвоним";
        }
        catch (Exception ex)
        {
            TempData["Message"] = "Ошибка при сохранении заявки";
        }

        return RedirectToAction("Index");
    }
    public IActionResult About()
    {
        return View();
    }

    public IActionResult ImportAbout()
    {
        return View();
    }
    public IActionResult ClearanceOfCars()
    {
        return View();
    }

    public IActionResult HeadPay()
    {
        return View();
    }

    public IActionResult Delivery()
    {
        return View();
    }
    public IActionResult Choose()
    {
        return View();
    }
    public IActionResult Peculiarities()
    {
        return View();
    }

    public IActionResult Faq()
    {
        return View();
    }
    public IActionResult Docs()
    {
        return View();
    }
    public IActionResult ImportRules()
    {
        return View();
    }
    public IActionResult Calculator()
    {
        return View();
    }
    public IActionResult Details()
    {
        return View();
    }
    public IActionResult InstallCharger()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}