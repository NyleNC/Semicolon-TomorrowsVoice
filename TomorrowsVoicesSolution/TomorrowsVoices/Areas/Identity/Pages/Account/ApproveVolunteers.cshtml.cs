using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [BindProperty]
        public List<int> SelectedVolunteers { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            PendingVolunteers = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Where(v => v.Status == ApprovalStatus.Pending)
                .ToListAsync();
        }

        // Existing single-action methods
        public async Task<IActionResult> OnPostApproveAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Approved);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Rejected);
            return RedirectToPage();
        }

        // New bulk-action methods
        public async Task<IActionResult> OnPostApproveSelectedAsync()
        {
            if (SelectedVolunteers != null && SelectedVolunteers.Any())
            {
                foreach (var id in SelectedVolunteers)
                {
                    await UpdateVolunteerStatus(id, ApprovalStatus.Approved);
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectSelectedAsync()
        {
            if (SelectedVolunteers != null && SelectedVolunteers.Any())
            {
                foreach (var id in SelectedVolunteers)
                {
                    await UpdateVolunteerStatus(id, ApprovalStatus.Rejected);
                }
            }
            return RedirectToPage();
        }

        private async Task UpdateVolunteerStatus(int volunteerId, ApprovalStatus status)
        {
            var volunteer = await _context.Volunteers.FindAsync(volunteerId);
            if (volunteer != null)
            {
                volunteer.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}