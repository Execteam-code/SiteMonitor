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
                var stopwatch = new Stopwatch();
                service.LastChecked = DateTime.Now;

                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    
                    stopwatch.Start();
                    var response = await client.GetAsync(service.TargetUrl, cts.Token);
                    stopwatch.Stop();

                    service.ResponseTimeMs = stopwatch.ElapsedMilliseconds;

                    int statusCode = (int)response.StatusCode;
                    
                    if (statusCode >= 200 && statusCode <= 399)
                    {
                        service.IsOnline = true;
                    }
                    else
                    {
                        service.IsOnline = false;
                    }
                }
                catch (Exception)
                {
                    stopwatch.Stop();
                    service.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
                    service.IsOnline = false;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
