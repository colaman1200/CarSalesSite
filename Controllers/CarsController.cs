using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSalesSite.Models;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;


namespace CarSalesSite.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;

        public CarsController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            
        }

        public IActionResult Index(string filterType, string filterValue)
        {
            IQueryable<Car> query = _context.Cars;

            
            var cars = query.OrderBy(c => c.CarId).ToList();
            

            return View(cars);
            
        }

        

        [HttpPost]
        public async Task<IActionResult> UploadPhotos(int carId, List<IFormFile> photos)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();

            var urls = new List<string>();
            if (!string.IsNullOrEmpty(car.Photos))
                urls.AddRange(car.PhotoUrls);

            foreach (var photo in photos)
            {
                if (photo.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    var uploadPath = Path.Combine(_env.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(uploadPath);

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    urls.Add($"/images/cars/{fileName}");
                }
            }

            car.Photos = string.Join(',', urls);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = carId });
        }
        public IActionResult ByBrand(int brandId)
        {
            var cars = _context.Cars
                .Include(c => c.Brand)
                .Where(c => c.BrandId == brandId)
                .ToList();

            var brand = _context.Brands.FirstOrDefault(b => b.BrandId == brandId);
            ViewBag.FilterTitle = brand != null ? $"Автомобили {brand.Name}" : "Автомобили марки";

            // Передаем бренды, чтобы секция не пропадала
            ViewBag.Brands = _context.Brands.OrderBy(b => b.Name).ToList();

            return View("~/Views/Home/Index.cshtml", cars);
        }

        [HttpGet]
        public IActionResult Filter(
         int? minPrice,
         int? maxPrice,
         int? minYear,
         int? maxYear,
         string country)
        {
            IQueryable<Car> query = _context.Cars.Include(c => c.Brand);

            
            if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice);
            if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice);
            if (minYear.HasValue) query = query.Where(c => c.Year >= minYear);
            if (maxYear.HasValue) query = query.Where(c => c.Year <= maxYear);

            
            if (!string.IsNullOrEmpty(country))
            {
                string russianCountry = country switch
                {
                    "Europe" => "Европа",
                    "USA" => "США",
                    "Korea" => "Корея",
                    _ => country
                };
                query = query.Where(c => c.CountryOrigin == russianCountry);
            }

            ViewBag.FilterTitle = "Результаты фильтрации";
            return View("~/Views/Home/Index.cshtml", query.ToList());
        }

        [HttpGet]
        public IActionResult FilterByCategory(string category, string value)
        {
            IQueryable<Car> query = _context.Cars.Include(c => c.Brand);

            switch (category?.ToLower())
            {
                case "origin":
                    string russianCountry = value switch
                    {
                        "Europe" => "Европа",
                        "USA" => "США",
                        "Korea" => "Корея",
                        _ => value
                    };
                    query = query.Where(c => c.CountryOrigin == russianCountry);
                    ViewBag.FilterTitle = $"Автомобили из {russianCountry}";
                    break;

                case "brand":
                    query = query.Where(c => c.Brand.Name.ToLower() == value.ToLower());
                    ViewBag.FilterTitle = $"Автомобили {value}";
                    break;

                case "price":
                    ApplyPriceFilter(ref query, value);
                    break;

                case "electric":
                    string electricCountry = value switch
                    {
                        "Europe" => "Европа",
                        "USA" => "США",
                        "Korea" => "Корея",
                        _ => value
                    };
                    query = query.Where(c => c.FuelType == "Электрический" && c.CountryOrigin == electricCountry);
                    ViewBag.FilterTitle = $"Электрокары из {electricCountry}";
                    break;

                default:
                    return RedirectToAction("Index");
            }
            Console.WriteLine($"SQL: {query.ToQueryString()}");
            
            return View("~/Views/Home/Index.cshtml", query.ToList());
        }

        
        private void ApplyPriceFilter(ref IQueryable<Car> query, string priceRange)
        {
            switch (priceRange.ToLower())
            {
                case "under-10000":
                    query = query.Where(c => c.Price < 10000);
                    ViewBag.FilterTitle = "Автомобили до 10 000$";
                    break;

                case "10000-20000":
                    query = query.Where(c => c.Price >= 10000 && c.Price < 20000);
                    ViewBag.FilterTitle = "Автомобили 10 000-20 000$";
                    break;

                case "20000-30000":
                    query = query.Where(c => c.Price >= 20000 && c.Price < 30000);
                    ViewBag.FilterTitle = "Автомобили 20 000-30 000$";
                    break;

                case "over-30000":
                    query = query.Where(c => c.Price >= 30000);
                    ViewBag.FilterTitle = "Автомобили от 30 000$";
                    break;
            }
        }

        public IActionResult Details(int id)
        {
            var car = _context.Cars
                .Include(c => c.Brand)
                .FirstOrDefault(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            return View("~/Views/Home/Details.cshtml", car); 
        }
        
    }
}