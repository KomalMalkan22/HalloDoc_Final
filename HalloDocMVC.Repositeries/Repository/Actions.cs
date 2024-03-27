using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class Actions : IActions
    {
        #region Configuration
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HalloDocContext _context;
        private readonly EmailConfiguration _emailConfiguration;
        public Actions(HalloDocContext context, EmailConfiguration emailConfiguration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailConfiguration = emailConfiguration;
            this.httpContextAccessor = httpContextAccessor;
        }
        #endregion Configuration

        #region GetRequestForViewCase
        public ViewCaseModel GetRequestForViewCase(int id)
        {
            var n = _context.Requests.FirstOrDefault(E => E.Requestid == id);

            var l = _context.Requestclients.FirstOrDefault(E => E.Requestid == id);

            var region = _context.Regions.FirstOrDefault(E => E.Regionid == l.Regionid);

            ViewCaseModel requestforviewcase = new()
            {
                RequestId = id,
                Region = region.Name,
                FirstName = l.Firstname,
                LastName = l.Lastname,
                PhoneNumber = l.Phonenumber,
                Notes = l.Notes,
                Email = l.Email,
                RequestTypeId = n.Requesttypeid,
                Address = l.Street + "," + l.City + "," + l.State,
                Room = l.Address,
                ConfirmationNumber = n.Confirmationnumber,
                DateOfBirth = new DateTime((int)l.Intyear, DateTime.ParseExact(l.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)l.Intdate)
            };
            return requestforviewcase;
        }
        #endregion GetRequestForViewCase

        #region EditCase
        public bool EditCase(ViewCaseModel model)
        {
            try
            {
                int monthnum = model.DateOfBirth.Month;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthnum);
                int date = model.DateOfBirth.Day;
                int year = model.DateOfBirth.Year;
                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == model.RequestId);
                if (client != null)
                {
                    client.Firstname = model.FirstName;
                    client.Lastname = model.LastName;
                    client.Phonenumber = model.PhoneNumber;
                    client.Intdate = date;
                    client.Intyear = year;
                    client.Strmonth = monthName;
                    client.Notes = model.Notes;
                    client.Phonenumber = model.PhoneNumber;
                    client.Email = model.Email;
                    List<string> location = model.Address.Split(',').ToList();
                    client.Street = location[0];
                    client.City = location[1];
                    client.State = location[2];
                    client.Address = model.Room;
                    _context.Requestclients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion EditCase

        #region GetNotes
        public ViewNotesModel GetNotes(int id)
        {
            var request = _context.Requests.FirstOrDefault(req => req.Requestid == id);
            var symptoms = _context.Requestclients.FirstOrDefault(req => req.Requestid == id);

            var transfer =
                (
                    from rsl in _context.Requeststatuslogs
                    join phy in _context.Physicians
                    on rsl.Physicianid equals phy.Physicianid into phyGroup
                    from phy in phyGroup.DefaultIfEmpty()
                    join p in _context.Physicians
                    on rsl.Transtophysicianid equals p.Physicianid into pGroup
                    from p in pGroup.DefaultIfEmpty()
                    join adm in _context.Admins
                    on rsl.Adminid equals adm.Adminid into admGroup
                    from adm in admGroup.DefaultIfEmpty()
                    where rsl.Requestid == id && rsl.Status == 2

                    select new TransferNotesModel
                    {
                        TransToPhysician = p.Firstname,
                        Admin = adm.Firstname + " " + adm.Lastname,
                        Physician = phy.Firstname,
                        RequestId = rsl.Requestid,
                        Notes = rsl.Notes,
                        Status = rsl.Status,
                        PhysicianId = rsl.Physicianid,
                        CreatedDate = rsl.Createddate,
                        RequestStatusLogId = rsl.Requeststatuslogid,
                        TransToAdmin = rsl.Transtoadmin,
                        TransToPhysicianId = rsl.Transtophysicianid
                    }
                ).ToList();

            // cbp : Cancel By Provider
            var cbp = _context.Requeststatuslogs.Where(rsl => rsl.Requestid == id && (rsl.Transtoadmin != null));
            // cap : Cancel By admin or Patient
            var cap = _context.Requeststatuslogs.Where(rsl => rsl.Requestid == id && (rsl.Status == 3 || rsl.Status == 7));
            // notes : Admin and Provider Notes
            var notes = _context.Requestnotes.FirstOrDefault(rsl => rsl.Requestid == id);

            ViewNotesModel vdm = new()
            {
                RequestId = id,
                PatientNotes = symptoms.Notes
            };

            if (notes == null)
            {
                vdm.AdminNotes = "-";
                vdm.PhysicianNotes = "-";
            }
            else
            {
                vdm.Status = request.Status;
                vdm.RequestNotesId = notes.Requestnotesid;
                vdm.PhysicianNotes = notes.Physiciannotes ?? "-";
                vdm.AdminNotes = notes.Adminnotes ?? "-";
            }

            List<TransferNotesModel> trans = new();
            foreach(var item in transfer)
            {
                trans.Add(new TransferNotesModel
                {
                    TransToPhysician = item.TransToPhysician,
                    Admin = item.Admin,
                    Physician = item.Physician,
                    RequestId = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.PhysicianId,
                    CreatedDate = item.CreatedDate,
                    RequestStatusLogId = item.RequestStatusLogId,
                    TransToAdmin = item.TransToAdmin,
                    TransToPhysicianId = item.TransToPhysicianId
                });
            }
            vdm.transfernotes = trans;

            List<TransferNotesModel> cancelProvider = new();
            foreach(var item in cbp)
            {
                cancelProvider.Add(new TransferNotesModel
                {
                    RequestId = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.Physicianid,
                    CreatedDate = item.Createddate,
                    RequestStatusLogId = item.Requeststatuslogid,
                    TransToAdmin = item.Transtoadmin,
                    TransToPhysicianId = item.Transtophysicianid
                });
            }
            vdm.cancelbyphysician = cancelProvider;

            List<TransferNotesModel> cancelRequest = new();
            foreach(var item in cap)
            {
                cancelRequest.Add(new TransferNotesModel
                {
                    RequestId = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    PhysicianId = item.Physicianid,
                    CreatedDate = item.Createddate,
                    RequestStatusLogId = item.Requeststatuslogid,
                    TransToAdmin = item.Transtoadmin,
                    TransToPhysicianId = item.Transtophysicianid,
                });
            }
            vdm.cancel = cancelRequest;

            return vdm;
        }
        #endregion GetNotes

        #region EditNotes
        public bool EditNotes(string? AdminNotes, string? PhysicianNotes, int RequestId)
        {
            try
            {
                Requestnote note = _context.Requestnotes.FirstOrDefault(req => req.Requestid == RequestId);
                if(note != null)
                {
                    if(PhysicianNotes != null)
                    {
                        if(note != null)
                        {
                            note.Physiciannotes = PhysicianNotes;
                            note.Modifieddate = DateTime.Now;
                            _context.Requestnotes.Update(note);
                            _context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(AdminNotes != null)
                    {
                        if(note != null)
                        {
                            note.Adminnotes = AdminNotes;
                            note.Modifieddate = DateTime.Now;
                            _context.Requestnotes.Update(note);
                            _context.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Requestnote requestnote = new()
                    {
                        Requestid = RequestId,
                        Adminnotes = AdminNotes,
                        Physiciannotes = PhysicianNotes,
                        Createddate = DateTime.Now,
                        Createdby = "02ae2720-3e7c-4fff-b83f-038f29013420"
                    };
                    _context.Requestnotes.Add(requestnote);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion EditNotes

        #region AssignProvider
        public async Task<bool> AssignProvider(int RequestId, int ProviderId, string notes)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            request.Physicianid = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            Requeststatuslog rsl = new()
            {
                Requestid = RequestId,
                Physicianid = ProviderId,
                Notes = notes,

                Createddate = DateTime.Now,
                Status = 2
            };
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();

            return true;
        }
        #endregion AssignProvider

        #region CancelCase
        public bool CancelCase(int RequestId, string Note, string CaseTag)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestId);
                if (requestData != null)
                {
                    requestData.Casetag = CaseTag;
                    requestData.Status = 3;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Requeststatuslog rsl = new()
                    {
                        Requestid = RequestId,
                        Notes = Note,
                        Status = 3,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion CancelCase

        #region BlockCase
        public bool BlockCase(int RequestID, string Note)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.Requestid == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Blockrequest blc = new()
                    {
                        Requestid = requestData.Requestid.ToString(),
                        Phonenumber = requestData.Phonenumber,
                        Email = requestData.Email,
                        Reason = Note,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now
                    };
                    _context.Blockrequests.Add(blc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion BlockCase

        #region TransferPhysician
        public async Task<bool> TransferPhysician(int RequestId, int ProviderId, string Note)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.Requestid == RequestId);
            
            Requeststatuslog rsl = new()
            {
                Requestid = RequestId,
                Status = 2,
                Physicianid = request.Physicianid,
                Transtophysicianid = ProviderId,
                Notes = Note,
                Createddate = DateTime.Now
            };
            _context.Requeststatuslogs.Update(rsl);
            _context.SaveChanges();

            request.Physicianid = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return true;
        }
        #endregion TransferPhysician

        #region ClearCase
        public bool ClearCase(int RequestID)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(req => req.Requestid == RequestID);
                if (request != null)
                {
                    request.Status = 10;
                    _context.Requests.Update(request);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new()
                    {
                        Requestid = RequestID,
                        Status = 10,
                        Createddate= DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion ClearCase

        #region GetDocuments
        public async Task<ViewUploadModel> GetDocuments(int? id)
        {
            var req = _context.Requests.FirstOrDefault(r => r.Requestid == id);
            var reqClient = _context.Requestclients.FirstOrDefault(rc => rc.Requestid == id);
            ViewUploadModel upload = new()
            {
                ConfirmationNumber = req.Confirmationnumber,
                RequestId = req.Requestid,
                FirstName = reqClient.Firstname,
                LastName = reqClient.Lastname
            };

            var result = from requestWiseFile in _context.Requestwisefiles
                         join request in _context.Requests on requestWiseFile.Requestid equals request.Requestid
                         join physician in _context.Physicians on request.Physicianid equals physician.Physicianid into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.Adminid equals admin.Adminid into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.Requestid == id && requestWiseFile.Isdeleted == new BitArray(1)
                         select new
                         {
                             Uploader = requestWiseFile.Physicianid != null ? phys.Firstname : (requestWiseFile.Adminid != null ? adm.Firstname : request.Firstname),
                             isDeleted = requestWiseFile.Isdeleted.ToString(),
                             RequestwisefilesId = requestWiseFile.Requestwisefileid,
                             Status = requestWiseFile.Doctype,
                             CreatedDate = requestWiseFile.Createddate,
                             filename = requestWiseFile.Filename,
                         };
                        List<Documents> doclist = new();
                        foreach (var item in result)
                        {
                            doclist.Add(new Documents
                            {
                                Uploader = item.Uploader,
                                isDeleted = item.isDeleted,
                                RequestwiseFilesId = item.RequestwisefilesId,
                                Status = item.Status,
                                CreatedDate = item.CreatedDate,
                                FileName = item.filename
                            });
                        }
            upload.documents = doclist;
            return upload;
        }
        #endregion GetDocuments

        #region UploadDocuments
        public bool UploadDocuments(int Requestid, IFormFile file)
        {
            string upload = SaveFileModel.UploadDocument(file, Requestid);
            var requestwisefile = new Requestwisefile
            {
                Requestid = Requestid,
                Filename = upload,
                Isdeleted = new BitArray(1),
                Adminid = 1,
                Createddate = DateTime.Now
            };
            _context.Requestwisefiles.Add(requestwisefile);
            _context.SaveChanges();
            return true;
        }
        #endregion UploadDocuments

        #region DeleteDocuments
        public async Task<bool> DeleteDocuments(string ids)
        {
            List<int> delete = ids.Split(',').Select(int.Parse).ToList();
            foreach (int item in delete)
            {
                if (item > 0)
                {
                    var data = await _context.Requestwisefiles.Where(e => e.Requestwisefileid == item).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.Isdeleted[0] = true;
                        _context.Requestwisefiles.Update(data);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion DeleteDocuments

        #region SendOrder
        public Healthprofessional SelectProfessionalById(int VendorId)
        {
            return _context.Healthprofessionals.FirstOrDefault(p => p.Vendorid == VendorId);
        }
        public bool SendOrders(SendOrderModel sendOrder)
        {
            try
            {
                Orderdetail od = new()
                {
                    Requestid = sendOrder.RequestId,
                    Vendorid = sendOrder.VendorId,
                    Faxnumber = sendOrder.FaxNumber,
                    Email = sendOrder.Email,
                    Businesscontact = sendOrder.BusinessContact,
                    Prescription = sendOrder.Prescription,
                    Noofrefill = sendOrder.NoOfRefill,
                    Createddate = DateTime.Now,
                    Createdby = "02ae2720-3e7c-4fff-b83f-038f29013420"
                };
                _context.Orderdetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.Requestid == sendOrder.RequestId);
                Task<bool> task = _emailConfiguration.SendMail(od.Email, "New Order arrived", "Prescription : " + od.Prescription + " Requestor name : " + req.Firstname);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region SendAgreement
        public bool SendAgreement(int requestid)
        {
            var res = _context.Requestclients.FirstOrDefault(e => e.Requestid == requestid);
            var agreementUrl = "https://localhost:44362/SendAgreement?RequestID=" + requestid;
            _emailConfiguration.SendMail(res.Email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree / Disagree</a>");
            return true;
        }
        #endregion SendAgreement

        #region SendAgreement_Accept
        public bool SendAgreement_Accept(int RequestId)
        {
            var request = _context.Requests.Find(RequestId);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new()
                {
                    Requestid = RequestId,
                    Status = 4,
                    Createddate = DateTime.Now
                };
                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion SendAgreement_Accept

        #region SendAgreement_Reject
        public bool SendAgreement_Reject(int RequestId, string Notes)
        {
            var request = _context.Requests.Find(RequestId);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                Requeststatuslog rsl = new()
                {
                    Requestid = RequestId,
                    Status = 7,
                    Notes = Notes,
                    Createddate = DateTime.Now
                };
                _context.Requeststatuslogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion SendAgreement_Reject

        #region GetCloseCase
        public CloseCaseModel GetCloseCase(int RequestId)
        {
            CloseCaseModel request = new();

            var result = from Requestwisefile in _context.Requestwisefiles
                         join Request in _context.Requests
                         on Requestwisefile.Requestid equals Request.Requestid
                         join Physician in _context.Physicians
                         on Request.Physicianid equals Physician.Physicianid into PhysicianGroup
                         from Phys in PhysicianGroup.DefaultIfEmpty()
                         join Admin in _context.Admins
                         on Requestwisefile.Adminid equals Admin.Adminid into AdminGroup
                         from Adm in AdminGroup.DefaultIfEmpty()
                         where Request.Requestid == RequestId
                         select new
                         {
                             Uploader = Requestwisefile.Physicianid != null ? Phys.Firstname : (Requestwisefile.Adminid != null ? Adm.Firstname : Request.Firstname),
                             Requestwisefile.Filename,
                             Requestwisefile.Createddate,
                             Requestwisefile.Requestwisefileid
                         };

            List<Documents> documents = new();
            foreach(var item in result)
            {
                documents.Add(new Documents
                {
                    CreatedDate = item.Createddate,
                    FileName = item.Filename,
                    Uploader = item.Uploader,
                    RequestwiseFilesId = item.Requestwisefileid
                });
            }

            request.documents = documents;

            Request req = _context.Requests.FirstOrDefault(req => req.Requestid == RequestId);
            request.RequestId = req.Requestid;
            request.FirstName = req.Firstname;
            request.LastName = req.Lastname;
            request.ConfirmationNumber = req.Confirmationnumber;

            var RequestClient = _context.Requestclients.FirstOrDefault(rc => rc.Requestid == RequestId);
            request.Client_FirstName = RequestClient.Firstname;
            request.Client_LastName = RequestClient.Lastname;
            request.Client_Email = RequestClient.Email;
            request.Client_PhoneNumber = RequestClient.Phonenumber;
            request.Client_DateOfBirth = new DateTime((int)RequestClient.Intyear, DateTime.ParseExact(RequestClient.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)RequestClient.Intdate);
            return request;
        }
        #endregion GetCloseCase

        #region EditCloseCase
        public bool EditCloseCase(CloseCaseModel closeCase)
        {
            try
            {
                Requestclient client = _context.Requestclients.FirstOrDefault(E => E.Requestid == closeCase.RequestId);
                if (client != null)
                {
                    client.Phonenumber = closeCase.Client_PhoneNumber;
                    client.Email = closeCase.Client_Email;
                    _context.Requestclients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion EditCloseCase

        #region CaseClosed
        public bool CaseClosed(int RequestId)
        {
            try
            {
                var request = _context.Requests.FirstOrDefault(req => req.Requestid == RequestId);
                if (request != null)
                {

                    request.Status = 9;
                    request.Modifieddate = DateTime.Now;

                    _context.Requests.Update(request);
                    _context.SaveChanges();

                    Requeststatuslog rsl = new()
                    {
                        Requestid = RequestId,
                        Status = 9,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion CaseClosed

        #region GetEncounterData
        public EncounterModel GetEncounterData(int RequestId)
        {
            var datareq = _context.Requestclients.FirstOrDefault(e => e.Requestid == RequestId);
            var Data = _context.Encounterforms.FirstOrDefault(e => e.Requestid == RequestId);
            DateTime? fd = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate);
            if (Data != null)
            {
                EncounterModel enc = new()
                {
                    ABD = Data.Abd,
                    EncounterId = Data.Encounterformid,
                    Allergies = Data.Allergies,
                    BloodPressureD = Data.Bloodpressurediastolic,
                    BloodPressureS = Data.Bloodpressurediastolic,
                    Chest = Data.Chest,
                    CV = Data.Cv,
                    DateOfBirth = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                    Date = DateTime.Now,
                    Diagnosis = Data.Diagnosis,
                    Hr = Data.Hr,
                    HistoryOfMedical = Data.Medicalhistory,
                    Heent = Data.Heent,
                    Extr = Data.Extremities,
                    PhoneNumber = datareq.Phonenumber,
                    Email = datareq.Email,
                    HistoryOfIllness = Data.Historyofpresentillnessorinjury,
                    FirstName = datareq.Firstname,
                    LastName = datareq.Lastname,
                    Followup = Data.Followup,
                    Location = datareq.Address,
                    Medications = Data.Medications,
                    MedicationsDispensed = Data.Medicaldispensed,
                    Neuro = Data.Neuro,
                    O2 = Data.O2,
                    Other = Data.Other,
                    Pain = Data.Pain,
                    Procedures = Data.Procedures,
                    Isfinalize = Data.Isfinalize,
                    RequesId = RequestId,
                    Rr = Data.Rr,
                    Skin = Data.Skin,
                    Temp = Data.Temp,
                    Treatment = Data.TreatmentPlan
                };
                return enc;
            }
            else
            {
                if (datareq != null)
                {
                    EncounterModel enc = new()
                    {
                        FirstName = datareq.Firstname,
                        PhoneNumber = datareq.Phonenumber,
                        LastName = datareq.Lastname,
                        Location = datareq.Address,
                        DateOfBirth = new DateTime((int)datareq.Intyear, DateTime.ParseExact(datareq.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)datareq.Intdate),
                        Date = DateTime.Now,
                        RequesId = RequestId,
                        Email = datareq.Email,
                    };
                    return enc;
                }
                else
                {
                    return new EncounterModel();
                }
            }
        }
        #endregion GetEncounterData

        #region EditEncounterData
        public bool EditEncounterData(EncounterModel Data, string id)
        {
            try
            {
                var admindata = _context.Admins.FirstOrDefault(e => e.Aspnetuserid == id);
                if (Data.EncounterId == 0)
                {
                    Encounterform enc = new()
                    {
                        Abd = Data.ABD,
                        Encounterformid = (int)Data.EncounterId,
                        Allergies = Data.Allergies,
                        Bloodpressurediastolic = Data.BloodPressureD,
                        Bloodpressuresystolic = Data.BloodPressureS,
                        Chest = Data.Chest,
                        Cv = Data.CV,
                        Diagnosis = Data.Diagnosis,
                        Hr = Data.Hr,
                        Medicalhistory = Data.HistoryOfMedical,
                        Heent = Data.Heent,
                        Extremities = Data.Extr,
                        Historyofpresentillnessorinjury = Data.HistoryOfIllness,
                        Followup = Data.Followup,
                        Medications = Data.Medications,
                        Medicaldispensed = Data.MedicationsDispensed,
                        Neuro = Data.Neuro,
                        O2 = Data.O2,
                        Other = Data.Other,
                        Pain = Data.Pain,
                        Procedures = Data.Procedures,
                        Requestid = Data.RequesId,
                        Rr = Data.Rr,
                        Skin = Data.Skin,
                        Temp = Data.Temp,
                        TreatmentPlan = Data.Treatment,
                        Adminid = admindata.Adminid,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                    };
                    _context.Encounterforms.Add(enc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    var encdetails = _context.Encounterforms.FirstOrDefault(e => e.Encounterformid == Data.EncounterId);
                    if (encdetails != null)
                    {
                        encdetails.Abd = Data.ABD;
                        encdetails.Encounterformid = (int)Data.EncounterId;
                        encdetails.Allergies = Data.Allergies;
                        encdetails.Bloodpressurediastolic = Data.BloodPressureD;
                        encdetails.Bloodpressuresystolic = Data.BloodPressureS;
                        encdetails.Chest = Data.Chest;
                        encdetails.Cv = Data.CV;
                        encdetails.Diagnosis = Data.Diagnosis;
                        encdetails.Hr = Data.Hr;
                        encdetails.Medicalhistory = Data.HistoryOfMedical;
                        encdetails.Heent = Data.Heent;
                        encdetails.Extremities = Data.Extr;
                        encdetails.Historyofpresentillnessorinjury = Data.HistoryOfIllness;
                        encdetails.Followup = Data.Followup;
                        encdetails.Medications = Data.Medications;
                        encdetails.Medicaldispensed = Data.MedicationsDispensed;
                        encdetails.Neuro = Data.Neuro;
                        encdetails.O2 = Data.O2;
                        encdetails.Other = Data.Other;
                        encdetails.Pain = Data.Pain;
                        encdetails.Procedures = Data.Procedures;
                        encdetails.Requestid = Data.RequesId;
                        encdetails.Rr = Data.Rr;
                        encdetails.Skin = Data.Skin;
                        encdetails.Temp = Data.Temp;
                        encdetails.TreatmentPlan = Data.Treatment;
                        encdetails.Adminid = admindata.Adminid;
                        encdetails.Modifieddate = DateTime.Now;
                        _context.Encounterforms.Update(encdetails);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion EditEncounterData

        #region SendLink
        public bool SendLink(string FirstName, string LastName, string Email, string PhoneNumber)
        {
            var baseUrl = "https://localhost:44362/CreateRequest/Index";
            _emailConfiguration.SendMail(Email, "Create New Request", FirstName + " " + LastName + "  " + $"<a href='{baseUrl}'>Create New Request</a>");
            return true;
        }
        #endregion SendLink
    }
}