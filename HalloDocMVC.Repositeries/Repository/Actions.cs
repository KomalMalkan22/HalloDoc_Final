using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.ViewUploadModel;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class Actions : IActions
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public Actions(HalloDocContext context)
        {
            _context = context;
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
            catch (Exception ex)
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

            ViewNotesModel vdm = new ViewNotesModel();
            vdm.RequestId = id;
            vdm.PatientNotes = symptoms.Notes;

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

            List<TransferNotesModel> trans = new List<TransferNotesModel>();
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

            List<TransferNotesModel> cancelProvider = new List<TransferNotesModel>();
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

            List<TransferNotesModel> cancelRequest = new List<TransferNotesModel>();
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
                    Requestnote requestnote = new Requestnote
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
            catch (Exception ex)
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
                    requestData.Status = 8;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    Requeststatuslog rsl = new Requeststatuslog
                    {
                        Requestid = RequestId,
                        Notes = Note,
                        Status = 8,
                        Createddate = DateTime.Now
                    };
                    _context.Requeststatuslogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
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
                    Blockrequest blc = new Blockrequest
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            ViewUploadModel upload = new ViewUploadModel();
            upload.ConfirmationNumber = req.Confirmationnumber;
            upload.RequestId = req.Requestid;
            upload.FirstName = reqClient.Firstname;
            upload.LastName = reqClient.Lastname;           

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
                        List<Documents> doclist = new List<Documents>();
                        foreach (var item in result)
                        {
                            doclist.Add(new Documents
                            {
                                Uploader = item.Uploader,
                                isDeleted = item.isDeleted,
                                RequestwiseFilesId = item.RequestwisefilesId,
                                Status = item.Status,
                                CreatedDate = item.CreatedDate,
                                Filename = item.filename
                            });
                        }
            upload.documents = doclist;
            return upload;
        }
        #endregion GetDocuments

        #region UploadDocuments
        public Boolean UploadDocuments(int Requestid, IFormFile file)
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
    }
}
