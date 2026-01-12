using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelInsuranceManagementSystem.Application.Data;
using TravelInsuranceManagementSystem.Application.Models;

namespace TravelInsuranceManagementSystem.Application.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsuranceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: Show the Form
        [HttpGet]
        public IActionResult FamilyInsurance()
        {
            return View();
        }

        // 2. POST: Create (Save the data)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFamily([FromBody] FamilyInsuranceDto data)
        {
            // Changed data.Policy to data.PolicyDetails to resolve the Ambiguity error
            if (data == null || data.PolicyDetails == null)
                return BadRequest("No data received.");

            try
            {
                // MAP DTO TO YOUR DATABASE ENTITY (Policy + Members)
                var newPolicy = new Policy
                {
                    DestinationCountry = data.PolicyDetails.Destination,
                    TravelStartDate = data.PolicyDetails.TripStart,
                    TravelEndDate = data.PolicyDetails.TripEnd,
                    CoverageType = data.PolicyDetails.PlanType,
                    PolicyStatus = PolicyStatus.ACTIVE,

                    // Logic to set coverage amount based on plan
                    CoverageAmount = data.PolicyDetails.PlanType == "Premium" ? 50000 : 10000,

                    // Map the list of members from the DTO to the Database Entity (PolicyMember)
                    Members = data.Members.Select(m => new PolicyMember
                    {
                        Title = m.Title,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Relation = m.Relation,
                        DOB = m.DOB,
                        Mobile = m.Mobile
                    }).ToList()
                };

                _context.Policies.Add(newPolicy);
                await _context.SaveChangesAsync();

                // Return the generated database ID so JS can redirect to Success page
                return Ok(new { message = "Policy generated successfully!", id = newPolicy.PolicyId });
            }
            catch (Exception ex)
            {
                // Log the error (optional) and return failure
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // 3. GET: Success Page (Redirect destination)
        [HttpGet]
        public IActionResult Success(int id)
        {
            // Formats the ID as P-00001, P-00002, etc.
            ViewBag.PolicyDisplayId = "P-" + id.ToString("D5");
            return View();
        }
    }
}