using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class SendOrderModel
    {
        public int RequestId { get; set; }
        public int VendorId { get; set; }
        public string BusinessContact { get; set; }
        public string Email { get; set; }   
        public string? FaxNumber { get; set; }
        public string Prescription { get; set; }
        [Range(0, 15)]
        public int? NoOfRefill { get; set; }
    }
}
