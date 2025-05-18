using Microsoft.AspNetCore.Mvc;
using CarSalesSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace CarSalesSite.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Список автомобилей для администрирования
        public IActionResult Cars()
        {
            var cars = _context.Cars
                .Include(c => c.Brand)
                .ToList();
            return View(cars);
        }

        // Редактирование автомобиля
        public IActionResult AddCar()
        {
            ViewBag.Brands = _context.Brands.ToList();
            return View(new Car());
        }

        // AddCar (POST)
        [HttpPost]
        public async Task<IActionResult> AddCar(Car car)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = _context.Brands.ToList();
                return View(car);
            }

            car.Photos = SanitizePhotoUrls(car.Photos);
            car.CreatedAt = DateTime.Now;

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Cars");
        }

        // EditCar (GET)
        public IActionResult EditCar(int id)
        {
            var car = _context.Cars
                .Include(c => c.Brand)
                .FirstOrDefault(c => c.CarId == id);

            if (car == null) return NotFound();

            ViewBag.Brands = _context.Brands.ToList();
            return View(car);
        }

        // EditCar (POST)
        [HttpPost]
        public async Task<IActionResult> EditCar(Car car)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = _context.Brands.ToList();
                return View(car);
            }

            car.Photos = SanitizePhotoUrls(car.Photos);
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("Cars");
        }

        private string SanitizePhotoUrls(string urls)
        {
            return string.Join(",",
                urls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(url => url.Trim())
                    .Where(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            );
        }


        // Удаление автомобиля
        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cars");
        }
    }
}
