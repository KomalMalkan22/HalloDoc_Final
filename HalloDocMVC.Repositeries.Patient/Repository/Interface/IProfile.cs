using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositeries.Patient.Repository.Interface
{
    public interface IProfile
    {
        UserProfileModel GetProfile();
        Task<bool> EditProfile(UserProfileModel userprofile);
    }
}
