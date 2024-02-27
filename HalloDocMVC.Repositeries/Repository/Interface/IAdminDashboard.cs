using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IAdminDashboard
    {
        public CountStatusWiseRequestModel CardData();
        public List<AdminDashboardList> GetRequests(string Status);
    }
}
