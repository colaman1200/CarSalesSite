using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CarSalesSite.Models;

namespace CarSalesSite.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string letter)
        {
            IQueryable<Brand> query = _context.Brands;

            if (!string.IsNullOrEmpty(letter))
            {
                query = query.Where(b => b.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase));
            }

            var brands = query.OrderBy(b => b.Name).ToList();
            ViewBag.CurrentLetter = letter; 

            return View(brands);
        }
    }
}