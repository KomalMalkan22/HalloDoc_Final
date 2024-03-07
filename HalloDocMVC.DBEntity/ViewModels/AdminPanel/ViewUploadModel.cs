using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels.AdminPanel
{
    public class ViewUploadModel
    {
        public int RequestId { get; set; }
        public int RequestClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Documents> documents { get; set; }
        public class Documents
        {            
            public int? Status { get; set; }
            public string? Uploader { get; set; }
            public string Filename { get; set; }
            public DateTime CreatedDate { get; set; }
            public int? RequestwiseFilesId { get; set; }
            public string isDeleted { get; set; }
        }
    }
}
