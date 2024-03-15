using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Models;
using HalloDocMVC.Repositeries.Patient.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositeries.Patient.Repository
{
    public class PatientDashboard : IPatientDashboard
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public PatientDashboard(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region UploadDocuments
        public async Task<bool> UploadDocuments(IFormFile? UploadFile, int RequestId)
        {
            if (UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(UploadFile, RequestId);
                var requestwisefile = new Requestwisefile
                {
                    Requestid = RequestId,
                    Filename = upload,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestwisefile);
                _context.SaveChanges();
            }
            return true;
        }
        #endregion UploadDocuments

        #region RequestForMe
        public CreatePatientRequestModel RequestForMe()
        {
            var patientRequest = _context.Users
                               .Where(r => r.Userid == CV.UserID())
                               .Select(r => new CreatePatientRequestModel
                               {
                                   FirstName = r.Firstname,
                                   LastName = r.Lastname,
                                   Email = r.Email,
                                   PhoneNumber = r.Mobile,
                                   DateOfBirth = new DateTime((int)r.Intyear, DateTime.ParseExact(r.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)r.Intdate)
                               })
                               .FirstOrDefault();
            return patientRequest;
        }
        #endregion RequestForMe

        #region CreateRequestForMe
        public async Task<bool> CreateRequestForMe(CreatePatientRequestModel patientRequest)
        {
            var isExist = _context.Users.FirstOrDefault(x => x.Email == patientRequest.Email);
            var request = new DBEntity.DataModels.Request()
            {
                Requesttypeid = 2,
                Userid = isExist.Userid,
                Firstname = patientRequest.FirstName,
                Lastname = patientRequest.LastName,
                Email = patientRequest.Email,
                Status = 1,
                Phonenumber = patientRequest.PhoneNumber,
                Isurgentemailsent = new BitArray(1),
                Createddate = DateTime.Now
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var requestClient = new Requestclient()
            {
                Requestid = request.Requestid,
                Firstname = patientRequest.FirstName,
                Lastname = patientRequest.LastName,
                Address = patientRequest.Street + " " + patientRequest.City + " " + patientRequest.State + " " + patientRequest.ZipCode,
                Notes = patientRequest.Symptoms,
                Email = patientRequest.Email,
                Strmonth = patientRequest.DateOfBirth.ToString("MMMM"),
                Intdate = patientRequest.DateOfBirth.Day,
                Intyear = patientRequest.DateOfBirth.Year,
                Phonenumber = patientRequest.PhoneNumber,
                Street = patientRequest.Street,
                City = patientRequest.City,
                State = patientRequest.State,
                Zipcode = patientRequest.ZipCode
            };
            _context.Requestclients.Add(requestClient);
            await _context.SaveChangesAsync();


            if (patientRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(patientRequest.UploadFile, request.Requestid);
                
                var requestwisefile = new Requestwisefile
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
        #endregion CreateRequestForMe

        #region CreateRequestForSomeoneElse
        public async Task<bool> CreateRequestForSomeoneElse(CreatePatientRequestModel patientRequest)
        {
            var request = new DBEntity.DataModels.Request()
            {
                Requesttypeid = 2,
                Userid = CV.UserID(),
                Firstname = patientRequest.FirstName,
                Lastname = patientRequest.LastName,
                Email = patientRequest.Email,
                Status = 1,
                Phonenumber = patientRequest.PhoneNumber,
                Relationname = patientRequest.FF_RelationWithPatient,
                Isurgentemailsent = new BitArray(1),
                Createddate = DateTime.Now
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var Requestclient = new Requestclient()
            {
                Requestid = request.Requestid,
                Firstname = patientRequest.FirstName,
                Lastname = patientRequest.LastName,
                Address = patientRequest.Street + " " + patientRequest.City + " " + patientRequest.State + " " + patientRequest.ZipCode,
                Email = patientRequest.Email,
                Phonenumber = patientRequest.PhoneNumber,
                Notes = patientRequest.Symptoms,
                Strmonth = patientRequest.DateOfBirth.ToString("MMMM"),
                Intdate = patientRequest.DateOfBirth.Day,
                Intyear = patientRequest.DateOfBirth.Year,
                Street = patientRequest.Street,
                City = patientRequest.City,
                State = patientRequest.State,
                Zipcode = patientRequest.ZipCode
            };
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();


            if (patientRequest.UploadFile != null)
            {
                string upload = SaveFileModel.UploadDocument(patientRequest.UploadFile, request.Requestid);
                
                var requestwisefile = new Requestwisefile
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
        #endregion CreateRequestForSomeoneElse
    }
}
