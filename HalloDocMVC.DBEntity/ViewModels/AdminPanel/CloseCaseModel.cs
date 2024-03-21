using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class CloseCaseModel
    {
        public int RequestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public int RequestwiseFileId { get; set; }
        public int RequestClientId { get; set; }
        public string Client_FirstName { get; set; }
        public string Client_LastName { get; set;}
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Client_Email { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a contact number")]
        public string Client_PhoneNumber { get; set; }
        public DateTime Client_DateOfBirth { get; set; }
        public List<Documents> documents { get; set; } = null;
    }
}
