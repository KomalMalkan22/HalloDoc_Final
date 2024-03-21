using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IMyProfile
    {
        Task<AdminProfile> GetProfile(int UserId);
        Task<bool> ResetPassword(string Password, int AdminId);
        Task<bool> EditAdministratorInformation(AdminProfile profile);
        Task<bool> EditBillingInformation(AdminProfile profile);
    }
}
