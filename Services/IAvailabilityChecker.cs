using System.Threading.Tasks;

namespace ServiceMonitor.Web.Services
{
    public interface IAvailabilityChecker
    {
        Task CheckAllAsync();
    }
}
