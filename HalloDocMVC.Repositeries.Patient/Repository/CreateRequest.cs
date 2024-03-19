using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositeries.Patient.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositeries.Patient.Repository
{
    public class CreateRequest : ICreateRequest
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public CreateRequest(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region PatientRequest
        [HttpPost]
        public async Task<bool> PatientRequest(CreatePatientRequestModel createPatientRequest)
        {
            var User = new User();
            var isUserExist = _context.Users.FirstOrDefault(x => x.Email == createPatientRequest.Email);

            if (isUserExist == null)
            {
                Guid g = Guid.NewGuid();
                var aspnetuser = new Aspnetuser()
                {
                    Id = g.ToString(),
                    Username = createPatientRequest.FirstName,
                    Passwordhash = createPatientRequest.PassWord,
                    Email = createPatientRequest.Email,
                    Phonenumber = createPatientRequest.PhoneNumber,
                    CreatedDate = DateTime.Now
                };
                _context.Aspnetusers.Add(aspnetuser);
                await _context.SaveChangesAsync();

                var user = new User()
                {
                    Aspnetuserid = aspnetuser.Id,
                    Firstname = createPatientRequest.FirstName,
                    Lastname = createPatientRequest.LastName,
                    Email = createPatientRequest.Email,
                    Mobile = createPatientRequest.PhoneNumber,
                    Street = createPatientRequest.Street,
                    City = createPatientRequest.City,
                    State = createPatientRequest.State,
                    Zipcode = createPatientRequest.ZipCode,
                    Strmonth = createPatientRequest.DateOfBirth.ToString("MMMM"),
                    Intdate = createPatientRequest.DateOfBirth.Day,
                    Intyear = createPatientRequest.DateOfBirth.Year,
                    Createdby = aspnetuser.Id,
                    Createddate = DateTime.Now
                };
                User = user;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }


            var request = new Request()
            {
                Requesttypeid = 2,
                Status = 1,
                Userid = isUserExist == null ? User.Userid : isUserExist.Userid,
                Firstname = createPatientRequest.FirstName,
                Lastname = createPatientRequest.LastName,
                Email = createPatientRequest.Email,
                Phonenumber = createPatientRequest.PhoneNumber,
                Isurgentemailsent = new BitArray(1),
                Createddate = DateTime.Now
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestclient = new Requestclient()
            {
                Requestid = request.Requestid,
                Firstname = createPatientRequest.FirstName,
                Lastname = createPatientRequest.LastName,
                Street = createPatientRequest.Street,
                City = createPatientRequest.City,
                State = createPatientRequest.State,
                Zipcode = createPatientRequest.ZipCode,
                Email = createPatientRequest.Email,
                Phonenumber = createPatientRequest.PhoneNumber,
                Notes = createPatientRequest.Symptoms,
                Strmonth = createPatientRequest.DateOfBirth.ToString("MMMM"),
                Intdate = createPatientRequest.DateOfBirth.Day,
                Intyear = createPatientRequest.DateOfBirth.Year,
                Address = createPatientRequest.Street + " " + createPatientRequest.City + " " + createPatientRequest.State + " " + createPatientRequest.ZipCode
            };
            _context.Requestclients.Add(requestclient);
            await _context.SaveChangesAsync();

            if (createPatientRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(createPatientRequest.UploadFile, request.Requestid);
                
                var requestwisefile = new Requestwisefile()
                {
                    Requestid = request.Requestid,
                    Filename = upload,
                    Createddate = DateTime.Now
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion PatientRequest

        #region FamilyFriendRequest
        public async Task<bool> FamilyFriendRequest(CreateFamilyFriendRequestModel createFamilyFriendRequest)
        {
            var request = new Request()
            {
                Requesttypeid = 3,
                Status = 1,
                Firstname = createFamilyFriendRequest.FF_FirstName,
                Lastname = createFamilyFriendRequest.FF_LastName,
                Email = createFamilyFriendRequest.FF_Email,
                Phonenumber = createFamilyFriendRequest.FF_PhoneNumber,
                Relationname = createFamilyFriendRequest.FF_RelationWithPatients,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1)
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestClient = new Requestclient()
            {
                Requestid = request.Requestid,
                Notes = createFamilyFriendRequest.Symptoms,
                Firstname = createFamilyFriendRequest.FirstName,
                Lastname = createFamilyFriendRequest.LastName,
                Strmonth = createFamilyFriendRequest.DateOfBirth.ToString("MMMM"),
                Intdate = createFamilyFriendRequest.DateOfBirth.Day,
                Intyear = createFamilyFriendRequest.DateOfBirth.Year,
                Email = createFamilyFriendRequest.Email,
                Phonenumber = createFamilyFriendRequest.PhoneNumber,
                Location = createFamilyFriendRequest.RoomSuite,
                Street = createFamilyFriendRequest.Street,
                City = createFamilyFriendRequest.City,
                State = createFamilyFriendRequest.State,
                Zipcode = createFamilyFriendRequest.ZipCode,
                Address = createFamilyFriendRequest.Street + " " + createFamilyFriendRequest.City + " " + createFamilyFriendRequest.State + " " + createFamilyFriendRequest.ZipCode
            };
            _context.Requestclients.Add(requestClient);
            await _context.SaveChangesAsync();

            if (createFamilyFriendRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(createFamilyFriendRequest.UploadFile, request.Requestid);

                var requestwisefile = new Requestwisefile()
                {
                    Requestid = request.Requestid,
                    Filename = upload,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion FamilyFriendRequest

        #region ConciergeRequest
        public async Task<bool> ConciergeRequest(CreateConciergeRequestModel createConciergeRequest)
        {
            var concierge = new Concierge
            {
                Conciergename = createConciergeRequest.C_FirstName + " " + createConciergeRequest.C_LastName,
                Street = createConciergeRequest.C_Street,
                City = createConciergeRequest.C_City,
                State = createConciergeRequest.C_State,
                Zipcode = createConciergeRequest.C_ZipCode,
                Address = createConciergeRequest.C_Street + " " + createConciergeRequest.C_City + " " + createConciergeRequest.C_State + " " + createConciergeRequest.C_ZipCode,
                Regionid = 1,
                Createddate = DateTime.Now
            };
            _context.Concierges.Add(concierge);
            await _context.SaveChangesAsync();

            var request = new Request
            {
                Requesttypeid = 4,
                Status = 1,
                Firstname = createConciergeRequest.C_FirstName,
                Lastname = createConciergeRequest.C_LastName,
                Phonenumber = createConciergeRequest.C_PhoneNumber,
                Email = createConciergeRequest.C_Email,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1)
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestClient = new Requestclient
            {
                Requestid = request.Requestid,
                Notes = createConciergeRequest.Symptoms,
                Firstname = createConciergeRequest.FirstName,
                Lastname = createConciergeRequest.LastName,
                Strmonth = createConciergeRequest.DateOfBirth.ToString("MMMM"),
                Intdate = createConciergeRequest.DateOfBirth.Day,
                Intyear = createConciergeRequest.DateOfBirth.Year,
                Email = createConciergeRequest.Email,
                Phonenumber = createConciergeRequest.PhoneNumber,
                Location = createConciergeRequest.RoomSuite
            };
            _context.Requestclients.Add(requestClient);
            await _context.SaveChangesAsync();

            var requestConcierge = new Requestconcierge
            {
                Requestid = request.Requestid,
                Conciergeid = concierge.Conciergeid
            };
            _context.Requestconcierges.Add(requestConcierge);
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion ConciergeRequest

        #region BusinessRequest
        public async Task<bool> BusinessRequest(CreateBusinessRequestModel createBusinessRequest)
        {
            var business = new Business
            {
                Name = createBusinessRequest.BUS_FirstName + " " + createBusinessRequest.BUS_LastName,
                Phonenumber = createBusinessRequest.BUS_PhoneNumber,
                Address1 = createBusinessRequest.Street,
                City = createBusinessRequest.City,
                Zipcode = createBusinessRequest.ZipCode,
                Createddate = DateTime.Now
            };
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            var request = new Request
            {
                Requesttypeid = 1,
                Status = 1,
                Firstname = createBusinessRequest.BUS_FirstName,
                Lastname = createBusinessRequest.BUS_LastName,
                Phonenumber = createBusinessRequest.BUS_PhoneNumber,
                Email = createBusinessRequest.BUS_Email,
                Createddate = DateTime.Now,
                Isurgentemailsent = new BitArray(1)
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestClient = new Requestclient
            {
                Request = request,
                Requestid = request.Requestid,
                Notes = createBusinessRequest.Symptoms,
                Firstname = createBusinessRequest.FirstName,
                Lastname = createBusinessRequest.LastName,
                Strmonth = createBusinessRequest.DateOfBirth.ToString("MMMM"),
                Intdate = createBusinessRequest.DateOfBirth.Day,
                Intyear = createBusinessRequest.DateOfBirth.Year,
                Email = createBusinessRequest.Email,
                Phonenumber = createBusinessRequest.PhoneNumber,
                Street = createBusinessRequest.Street,
                City = createBusinessRequest.City,
                State = createBusinessRequest.State,
                Zipcode = createBusinessRequest.ZipCode,
                Location = createBusinessRequest.RoomSuite,
                Address = createBusinessRequest.Street + " " + createBusinessRequest.City + " " + createBusinessRequest.State + " " + createBusinessRequest.ZipCode
            };
            _context.Requestclients.Add(requestClient);
            await _context.SaveChangesAsync();

            var requestBusiness = new Requestbusiness
            {
                Requestid = request.Requestid,
                Businessid = business.Businessid
            };
            _context.Requestbusinesses.Add(requestBusiness);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion BusinessRequest
    }
}
