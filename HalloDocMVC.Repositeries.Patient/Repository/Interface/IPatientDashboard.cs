using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositeries.Patient.Repository.Interface
{
    public interface IPatientDashboard
    {
        Task<bool> UploadDocuments(IFormFile? UploadFile, int RequestId);
        public CreatePatientRequestModel RequestForMe();
        Task<bool> CreateRequestForMe(CreatePatientRequestModel patientRequest);
        Task<bool> CreateRequestForSomeoneElse(CreatePatientRequestModel patientRequest);
    }
}
