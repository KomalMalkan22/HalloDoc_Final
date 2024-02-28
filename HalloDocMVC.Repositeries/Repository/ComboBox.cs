using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class ComboBox : IComboBox
    {
        #region Configuration
        private readonly HalloDocContext _context;
        public ComboBox(HalloDocContext context)
        {
            _context = context;
        }
        #endregion Configuration

        #region ComboBoxRegions
        public async Task<List<ComboBoxRegion>> ComboBoxRegions()
        {
            return await _context.Regions.Select(region => new ComboBoxRegion()
            {
                RegionId = region.Regionid,
                RegionName = region.Name
            })
            .OrderBy(region => region.RegionName)
            .ToListAsync();
        }
        #endregion ComboBoxRegions

        #region ComboBoxCaseReasons
        public async Task<List<ComboBoxCaseReason>> ComboBoxCaseReasons()
        {
            return await _context.Casetags.Select(ct => new ComboBoxCaseReason()
            {
                CaseReasonId = ct.Casetagid,
                CaseReasonName = ct.Name
            })
            .OrderBy(ct => ct.CaseReasonName)
            .ToListAsync();
        }
        #endregion ComboBoxCaseReasons

        #region ProviderByRegion
        public List<Physician> ProviderByRegion(int? regionId)
        {
            var data = _context.Physicians
                .Where(r => r.Regionid == regionId)
                .OrderByDescending(r => r.Createddate).ToList();
            return data;
        }
        #endregion ProviderByRegion
    }
}
