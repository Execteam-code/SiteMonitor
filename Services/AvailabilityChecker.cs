using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceMonitor.Web.Data;

namespace ServiceMonitor.Web.Services
{
    public class AvailabilityChecker : IAvailabilityChecker
    {
        private readonly MonitoringContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AvailabilityChecker(MonitoringContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task CheckAllAsync()
        {
            var services = await _context.Services.ToListAsync();
            var client = _httpClientFactory.CreateClient();

            foreach (var service in services)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var response = await client.GetAsync(service.TargetUrl, cts.Token);
                    
                    int statusCode = (int)response.StatusCode;
                    service.IsOnline = statusCode >= 200 && statusCode <= 399;
                }
                catch
                {
                    // Перехват любых сетевых ошибок (DNS, Timeout, отсутствие сети)
                    service.IsOnline = false;
                }
                finally
                {
                    // Гарантированная фиксация времени независимо от исхода запроса
                    stopwatch.Stop();
                    service.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
                    service.LastChecked = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
~
