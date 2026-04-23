using Empen.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Empen.Pages.Notice
{
    public class IndexModel : PageModel
    {
        private readonly MasterDbContext _context;

        public IndexModel(MasterDbContext context)
        {
            _context = context;
        }

        public IList<master_notice> NoticeList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DateTime now = DateTime.Now;

            NoticeList = await _context.master_notice
                .AsNoTracking()
                .Where(n => n.start_date <= now && (n.end_date == null || n.end_date >= now))
                .OrderBy(n => n.show_order)
                .ThenByDescending(n => n.start_date)
                .ToListAsync();
        }
    }
}