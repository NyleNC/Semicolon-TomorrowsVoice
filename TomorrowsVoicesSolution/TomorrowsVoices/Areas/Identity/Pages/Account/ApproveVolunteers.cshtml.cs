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
        public List<Volunteer> ApprovedVolunteers { get; set; }
        public List<Volunteer> RejectedVolunteers { get; set; }

        [BindProperty]
        public List<int> SelectedVolunteers { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {
            PendingVolunteers = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Where(v => v.Status == ApprovalStatus.Pending)
                .ToListAsync();

            ApprovedVolunteers = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Where(v => v.Status == ApprovalStatus.Approved)
                .ToListAsync();

            RejectedVolunteers = await _context.Volunteers
                .Include(v => v.VolLocation)
                .Where(v => v.Status == ApprovalStatus.Rejected)
                .ToListAsync();
        }

        // Single-action methods
        public async Task<IActionResult> OnPostApproveAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Approved);
           
            TempData["ApprovedSuccess"] = true;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Rejected);
            TempData["Rejected"] = true;
            return RedirectToPage();
        }

        // Bulk-action methods
        public async Task<IActionResult> OnPostApproveSelectedAsync()
        {
            if (SelectedVolunteers != null && SelectedVolunteers.Any())
            {
                foreach (var id in SelectedVolunteers)
                {
                    await UpdateVolunteerStatus(id, ApprovalStatus.Approved);
                }
            }
            TempData["ApprovedBulk"] = true;
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
            TempData["RejectedBulk"] = true;
            return RedirectToPage();
        }

        // Methods for approved/rejected tabs
        public async Task<IActionResult> OnPostRevokeApprovalAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Pending);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReconsiderAsync(int volunteerId)
        {
            await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Pending);
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