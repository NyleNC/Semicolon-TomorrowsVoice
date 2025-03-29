using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;

namespace TomorrowsVoices.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class ApproveVolunteersModel : PageModel
    {
        private readonly TomorrowsVoicesContext _context;

        public ApproveVolunteersModel(TomorrowsVoicesContext context)
        {
            _context = context;
        }

        public List<Volunteer> PendingVolunteers { get; set; }

        public async Task OnGetAsync()
        {
            PendingVolunteers = await _context.Volunteers
                 .Include(v => v.VolLocation)
                .Where(v => v.Status == ApprovalStatus.Pending)
           
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int volunteerId)
        {
            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            if (volunteer != null)
            {
                volunteer.Status = ApprovalStatus.Approved;
            
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int volunteerId)
        {
            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            if (volunteer != null)
            {
                volunteer.Status =ApprovalStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
