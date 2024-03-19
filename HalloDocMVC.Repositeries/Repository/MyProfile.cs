using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDocMVC.DBEntity.ViewModels.AdminPanel.AdminProfile;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class MyProfile : IMyProfile
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public MyProfile(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration
        #region GetProfile
        public async Task<AdminProfile> GetProfile(int UserId)
        {
            AdminProfile? profile = await (from req in _context.Admins
                                         join Aspnetuser in _context.Aspnetusers
                                         on req.Aspnetuserid equals Aspnetuser.Id into aspGroup
                                         from asp in aspGroup.DefaultIfEmpty()
                                         where req.Adminid == UserId
                                         select new AdminProfile
                                         {
                                             AdminId = req.Adminid,
                                             AspnetuserId = req.Aspnetuserid,
                                             Status = req.Status,
                                             RoleId = req.Roleid,
                                             FirstName = req.Firstname,
                                             LastName = req.Lastname,
                                             Email = req.Email,
                                             PhoneNumber = req.Mobile,
                                             AltPhoneNumber = req.Altphone,
                                             Address1 = req.Address1,
                                             Address2 = req.Address2,
                                             City = req.City,
                                             ZipCode = req.Zip,
                                             RegionId = req.Regionid,
                                             UserName = asp.Username,
                                             Createdby = req.Createdby,
                                             Createddate = req.Createddate,
                                             Modifiedby = req.Modifiedby,
                                             Modifieddate = req.Modifieddate
                                         })
                                         .FirstOrDefaultAsync();

            List<Regions> regions = new List<Regions>();
            regions = await _context.Adminregions
                  .Where(r => r.Adminid == UserId)
                  .Select(req => new Regions()
                  {
                      RegionId = req.Regionid
                  })
                  .ToListAsync();
            profile.RegionIds = regions;
            return profile;
        }
        #endregion GetProfile
    }
}
