using Microsoft.AspNetCore.Mvc;
using CarSalesSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace CarSalesSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Вход
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            _logger.LogInformation($"Попытка входа: {email}");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogError("Пользователь не найден.");
                ModelState.AddModelError("", "Неверный email или пароль");
                return View();
            }

            _logger.LogInformation($"Найден пользователь: {user.Email}, роль: {user.Role}");

            bool isPasswordValid = user.VerifyPassword(password);
            _logger.LogInformation($"Результат проверки пароля: {isPasswordValid}");

            if (!isPasswordValid)
            {
                _logger.LogError("Неверный пароль.");
                ModelState.AddModelError("", "Неверный email или пароль");
                return View();
            }

            _logger.LogInformation($"Пароль для {email} верифицирован.");

            var claims = new List<Claim>
             {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
             };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            _logger.LogInformation($"Роль пользователя: {user.Role}. Перенаправление...");

            return user.Role == "admin"
                ? RedirectToAction("Cars", "Admin")
                : RedirectToAction("Profile");
        }

        // Регистрация
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email уже занят");
                return View(user);
            }

            user.SetPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // Личный кабинет
        [Authorize]
        public IActionResult Profile()
        {
            var email = User.Identity.Name;
            var user = _context.Users
                .Include(u => u.Favourites)
                    .ThenInclude(f => f.Car)
                        .ThenInclude(c => c.Brand) // Добавьте, если нужно
                .FirstOrDefault(u => u.Email == email);

            if (user == null) return NotFound();

            // Гарантируем, что коллекция не null
            user.Favourites ??= new List<Favourites>();

            return View(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToFavourites(int carId)
        {
            var user = await _context.Users
                .Include(u => u.Favourites)
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if (user == null) return Unauthorized();

            // Проверка на существование автомобиля
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();

            // Проверка на дубликат
            if (!user.Favourites.Any(f => f.CarId == carId))
            {
                user.Favourites.Add(new Favourites { CarId = carId });
                await _context.SaveChangesAsync();
                TempData["Message"] = "Автомобиль добавлен в избранное";
            }

            return RedirectToAction("Details", "Cars", new { id = carId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavourites(int favouriteId)
        {
            var favourite = await _context.Favourites.FindAsync(favouriteId);
            if (favourite != null)
            {
                _context.Favourites.Remove(favourite);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Автомобиль удален из избранного";
            }
            return RedirectToAction("Profile");
        }
        // Выход
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}