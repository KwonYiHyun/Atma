using Empen.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Empen.Pages.Notice
{
    public class ReadModel : PageModel
    {
        private readonly MasterDbContext _context;

        public ReadModel(MasterDbContext context)
        {
            _context = context;
        }

        public master_notice Notice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return RedirectToPage("./Index");

            Notice = await _context.master_notice.AsNoTracking()
                .FirstOrDefaultAsync(m => m.notice_id == id);

            if (Notice == null) return RedirectToPage("./Index");

            return Page();
        }
    }
}