using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceMonitor.Web.Data;
using ServiceMonitor.Web.Services;

namespace ServiceMonitor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly MonitoringContext _context;
        private readonly IAvailabilityChecker _checker;

        public DashboardController(MonitoringContext context, IAvailabilityChecker checker)
        {
            _context = context;
            _checker = checker;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToListAsync();
                
            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAll()
        {
            await _checker.CheckAllAsync();
            TempData["SuccessMessage"] = "Проверка завершена";
            return RedirectToAction(nameof(Index));
        }
    }
}
