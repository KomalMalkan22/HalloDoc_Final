using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositeries.Patient.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositeries.Patient.Repository
{
    public class Profile : IProfile
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public Profile(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region GetProfile
        public UserProfileModel GetProfile()
        {
            var userProfile = _context.Users
                                .Where(r => r.Userid == CV.UserID())
                                .Select(r => new UserProfileModel
                                {
                                    Userid = r.Userid,
                                    FirstName = r.Firstname,
                                    LastName = r.Lastname,
                                    PhoneNumber = r.Mobile,
                                    Email = r.Email,
                                    Street = r.Street,
                                    State = r.State,
                                    City = r.City,
                                    ZipCode = r.Zipcode,
                                    DateOfBirth = new DateTime((int)r.Intyear, DateTime.ParseExact(r.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)r.Intdate),
                                })
                                .FirstOrDefault();

            return userProfile;
        }
        #endregion GetProfile

        #region EditProfile
        public async Task<bool> EditProfile(UserProfileModel userprofile)
        {
            User userToUpdate = await _context.Users.FindAsync(userprofile.Userid);

            userToUpdate.Firstname = userprofile.FirstName;
            userToUpdate.Lastname = userprofile.LastName;
            userToUpdate.Mobile = userprofile.PhoneNumber;
            userToUpdate.Email = userprofile.Email;
            userToUpdate.State = userprofile.State;
            userToUpdate.Street = userprofile.Street;
            userToUpdate.City = userprofile.City;
            userToUpdate.Zipcode = userprofile.ZipCode;
            userToUpdate.Intdate = userprofile.DateOfBirth.Day;
            userToUpdate.Intyear = userprofile.DateOfBirth.Year;
            userToUpdate.Strmonth = userprofile.DateOfBirth.ToString("MMMM");
            userToUpdate.Modifiedby = userprofile.Createdby;
            userToUpdate.Modifieddate = DateTime.Now;
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion EditProfile
    }
}
