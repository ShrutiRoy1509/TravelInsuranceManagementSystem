using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelInsuranceManagementSystem.Application.Data;
using TravelInsuranceManagementSystem.Application.Models;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // FIXED: Only one constructor allowed for Dependency Injection
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";
            return View();
        }

        // READ OPERATION: Fetching real data from the database
        public async Task<IActionResult> Policies()
        {
            ViewData["Title"] = "Policy Management";

            // Get all policies and include family members to show "Customer Name"
            var policies = await _context.Policies
                .Include(p => p.Members)
                .OrderByDescending(p => p.PolicyId)
                .ToListAsync();

            return View(policies);
        }

        public IActionResult Claims()
        {
            ViewData["Title"] = "Claims Overview";
            return View();
        }

        public IActionResult Payments()
        {
            ViewData["Title"] = "Payment History";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn", "Home");
        }
    }
}