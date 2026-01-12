using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelInsuranceManagementSystem.Application.Data;
using TravelInsuranceManagementSystem.Application.Models;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    // Restrict access to users with the 'Agent' role only
    [Authorize(Roles = "Agent")]
    // Prevent the browser from caching these private pages
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AgentController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor injecting the database context
        public AgentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // READ OPERATION: Fetch all policies including family members
        public async Task<IActionResult> Policies()
        {
            var policies = await _context.Policies
                .Include(p => p.Members) // Eager load the family members
                .OrderByDescending(p => p.PolicyId)
                .ToListAsync();

            return View(policies);
        }

        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }

        public IActionResult SupportTickets()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Clear the authentication cookie explicitly
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session data if any exists
            HttpContext.Session.Clear();

            // Redirect to the Home/SignIn page after logout
            return RedirectToAction("SignIn", "Home");
        }
    }
}