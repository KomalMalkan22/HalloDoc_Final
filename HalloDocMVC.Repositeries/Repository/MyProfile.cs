using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        #region ResetPassword
        public async Task<bool> ResetPassword(string Password, int AdminId)
        {
            var request = await _context.Admins.Where(a => a.Adminid == AdminId ).FirstOrDefaultAsync();
            Aspnetuser? user = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Id == request.Aspnetuserid);

            if (user != null)
            {
                user.Passwordhash = Password;
                _context.Aspnetusers.Update(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion ResetPassword

        #region EditAdministratorInformation
        public async Task<bool> EditAdministratorInformation(AdminProfile profile)
        {
            try
            {
                if (profile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.Adminid == profile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = profile.Email;
                        DataForChange.Firstname = profile.FirstName;
                        DataForChange.Lastname = profile.LastName;
                        DataForChange.Mobile = profile.PhoneNumber;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.Adminregions.Where(r => r.Adminid == profile.AdminId).Select(req => req.Regionid).ToListAsync();
                        List<int> priceList = profile.RegionsId.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                Adminregion ar = new()
                                {
                                    Regionid = item,
                                    Adminid = (int)profile.AdminId
                                };
                                _context.Adminregions.Update(ar);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                Adminregion ar = await _context.Adminregions.Where(r => r.Adminid == profile.AdminId && r.Regionid == item).FirstAsync();
                                _context.Adminregions.Remove(ar);
                                await _context.SaveChangesAsync();
                            }
                        }
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
        #endregion EditAdministratorInformation

        #region BillingInfoEdit
        public async Task<bool> EditBillingInformation(AdminProfile profile)
        {
            try
            {
                if (profile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.Adminid == profile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = profile.Address1;
                        DataForChange.Address2 = profile.Address2;
                        DataForChange.City = profile.City;
                        DataForChange.Mobile = profile.PhoneNumber;
                        _context.Admins.Update(DataForChange);
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
        #endregion
    }
}
