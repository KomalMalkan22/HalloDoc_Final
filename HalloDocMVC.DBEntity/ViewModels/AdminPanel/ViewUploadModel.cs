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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<Documents> documents { get; set; }
        public class Documents
        {         
            public string? Uploader { get; set; }
            public int? RequestwiseFilesId { get; set; }
            public string isDeleted { get; set; }
        }
    }
}
