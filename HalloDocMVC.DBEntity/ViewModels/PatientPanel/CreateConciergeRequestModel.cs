using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.PatientPanel
{
    public class CreateConciergeRequestModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string C_FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string C_LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string C_Email { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a contact number")]
        public string C_PhoneNumber { get; set; }
        public string C_Property { get; set; }
        [Required(ErrorMessage = "Street is required")]
        public string C_Street { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string C_City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string C_State { get; set; }
        [Required(ErrorMessage = "Zip Code is required")]
        public string C_ZipCode { get; set; }
        public string ID { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"([0-9]{10})", ErrorMessage = "Please enter 10 digits for a contact number")]
        public string PhoneNumber { get; set; }
        public string? RoomSuite { get; set; }
    }
}
