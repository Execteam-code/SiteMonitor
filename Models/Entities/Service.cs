using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceMonitor.Web.Models.Enums;

namespace ServiceMonitor.Web.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Укажите название")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Укажите URL")]
        [Url(ErrorMessage = "Неверный формат (начните с http:// или https://)")]
        public string TargetUrl { get; set; } = string.Empty;
        
        public bool IsOnline { get; set; }
        
        public DateTime? LastChecked { get; set; }
        
        public long? ResponseTimeMs { get; set; }

        [NotMapped]
        public Status CurrentStatus
        {
            get
            {
                if (!IsOnline)
                    return Status.Offline;

                if (ResponseTimeMs > 300)
                    return Status.Slow;

                return Status.Online;
            }
        }
    }
}
