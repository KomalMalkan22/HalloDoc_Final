using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.DataModels;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly HalloDocContext _context;
        public AdminDashboard(HalloDocContext context)
        {
            _context = context;
        }
        public CountStatusWiseRequestModel CardData()
        {
            return new CountStatusWiseRequestModel()
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => r.Status == 4 || r.Status == 5).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => r.Status == 3 || r.Status == 7 || r.Status == 8).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count()
            };
        }
        public List<AdminDashboardList> GetRequests(string Status)
        {
            List<int> status = Status.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                    join reqClient in _context.Requestclients
                                                    on req.Requestid equals reqClient.Requestid into reqClientGroup
                                                    from rc in reqClientGroup.DefaultIfEmpty()
                                                    join phys in _context.Physicians
                                                    on req.Physicianid equals phys.Physicianid into physGroup
                                                    from p in physGroup.DefaultIfEmpty()
                                                    join reg in _context.Regions
                                                    on rc.Regionid equals reg.Regionid into RegGroup
                                                    from rg in RegGroup.DefaultIfEmpty()
                                                    where status.Contains(req.Status)
                                                    orderby req.Createddate descending
                                                    select new AdminDashboardList
                                                   {
                                                       RequestId = req.Requestid,
                                                       RequestTypeId = req.Requesttypeid,
                                                       Requestor = req.Firstname + " " + req.Lastname,
                                                       PatientName = rc.Firstname + " " + rc.Lastname,
                                                       DateOfBirth = new DateTime((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
                                                       RequestedDate = req.Createddate,
                                                       Email = rc.Email,

                                                       Region = rg.Name,
                                                       ProviderId = req.Physicianid,
                                                       ProviderName = p.Firstname + " " + p.Lastname,
                                                       PatientPhoneNumber = rc.Phonenumber,
                                                       Address = rc.Address + " " + rc.Street + " " + rc.City + " " + rc.State + " " + rc.Zipcode,
                                                       Notes = rc.Notes,
                                                       RequestorPhoneNumber = req.Phonenumber
                                                   })
                                                   .ToList();
            return allData;
        }

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
    }
}

