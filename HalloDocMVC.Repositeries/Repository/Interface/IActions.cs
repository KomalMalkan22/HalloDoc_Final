﻿using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IActions
    {
        public ViewCaseModel GetRequestForViewCase(int id);
        public bool EditCase(ViewCaseModel model);
        public ViewNotesModel GetNotes(int id);
        public bool EditNotes(string? AdminNotes, string? PhysicianNotes, int RequestId);
        Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
        public bool BlockCase(int RequestID, string Note);
        public Task<bool> TransferPhysician(int RequestId, int ProviderId, string Note);
        public bool ClearCase(int RequestID);
        Task<ViewUploadModel> GetDocuments(int? id);
        public bool UploadDocuments(int Requestid, IFormFile file);
        Task<bool> DeleteDocuments(string ids);
        public Healthprofessional SelectProfessionalById(int VendorId);
        public bool SendOrders(SendOrderModel sendOrder);
        public bool SendAgreement(int requestid);
        public bool SendAgreement_Accept(int RequestId);
        public bool SendAgreement_Reject(int RequestId, string Notes);
        CloseCaseModel GetCloseCase(int RequestId);
        bool EditCloseCase(CloseCaseModel closeCase);
        bool CaseClosed(int RequestId);
        EncounterModel GetEncounterData(int RequestId);
        bool EditEncounterData(EncounterModel Data, string id);
    }
}
