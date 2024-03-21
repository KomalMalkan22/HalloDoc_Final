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
        [Required(ErrorMessage = "Business is required")]
        public int VendorId { get; set; }
        [Required(ErrorMessage = "BusinessContact is required")]
        public string BusinessContact { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "FaxNumber is required")]
        public string? FaxNumber { get; set; }
        [Required(ErrorMessage = "Prescription is required")]
        public string Prescription { get; set; }
        [Range(0, 15)]
        [Required(ErrorMessage = "Number of refill is required")]
        public int? NoOfRefill { get; set; }
    }
}
