using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IComboBox
    {
        Task<List<ComboBoxRegion>> ComboBoxRegions();
        Task<List<ComboBoxCaseReason>> ComboBoxCaseReasons();
        List<Physician> ProviderByRegion(int? regionId);
    }
}
