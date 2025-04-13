using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using TomorrowsVoices.Data;
using TomorrowsVoices.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task OnGetAsync(string tab = "pending")
    {
        ViewData["ActiveTab"] = tab;

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

    public async Task<IActionResult> OnPostApproveAsync(int volunteerId, string currentTab)
    {
        await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Approved);
        TempData["ApprovedSuccess"] = true;
        return RedirectToPage(new { tab = currentTab });
    }

    public async Task<IActionResult> OnPostRejectAsync(int volunteerId, string currentTab)
    {
        await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Rejected);
        TempData["Rejected"] = true;
        return RedirectToPage(new { tab = currentTab });
    }

    public async Task<IActionResult> OnPostApproveSelectedAsync(string currentTab)
    {
        if (SelectedVolunteers != null && SelectedVolunteers.Any())
        {
            foreach (var id in SelectedVolunteers)
            {
                await UpdateVolunteerStatus(id, ApprovalStatus.Approved);
            }
        }
        TempData["ApprovedBulk"] = true;
        return RedirectToPage(new { tab = currentTab });
    }

    public async Task<IActionResult> OnPostRejectSelectedAsync(string currentTab)
    {
        if (SelectedVolunteers != null && SelectedVolunteers.Any())
        {
            foreach (var id in SelectedVolunteers)
            {
                await UpdateVolunteerStatus(id, ApprovalStatus.Rejected);
            }
        }
        TempData["RejectedBulk"] = true;
        return RedirectToPage(new { tab = currentTab });
    }

    public async Task<IActionResult> OnPostRevokeApprovalAsync(int volunteerId, string currentTab)
    {
        await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Pending);
        return RedirectToPage(new { tab = currentTab });
    }

    public async Task<IActionResult> OnPostReconsiderAsync(int volunteerId, string currentTab)
    {
        await UpdateVolunteerStatus(volunteerId, ApprovalStatus.Pending);
        return RedirectToPage(new { tab = currentTab });
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