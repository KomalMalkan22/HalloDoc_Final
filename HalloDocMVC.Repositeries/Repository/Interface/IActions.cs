using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IActions
    {
        public ViewCaseModel GetRequestForViewCase(int id);
        public bool EditCase(ViewCaseModel model);
        Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
    }
}
