using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceMonitor.Web.Data;
using ServiceMonitor.Web.Models.Entities;

namespace ServiceMonitor.Web.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private readonly MonitoringContext _context;

        public ServicesController(MonitoringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.AsNoTracking().ToListAsync();
            return View(services);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (service == null) return NotFound();

            return View(service);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,TargetUrl")] Service service)
        {
            if (ModelState.IsValid)
            {
                service.IsOnline = false;
                service.LastChecked = null;
                service.ResponseTimeMs = null;

                _context.Add(service);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Index", "Dashboard");
            }
            return View(service);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TargetUrl")] Service serviceForm)
        {
            if (id != serviceForm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceToUpdate = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
                    if (serviceToUpdate == null) return NotFound();

                    serviceToUpdate.Name = serviceForm.Name;
                    serviceToUpdate.TargetUrl = serviceForm.TargetUrl;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(serviceForm.Id)) return NotFound();
                    else throw;
                }
                
                return RedirectToAction("Index", "Dashboard");
            }
            return View(serviceForm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
