using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.DBEntity.ViewModels
{
    public class Constant
    {        
        public enum RequestType
        {
            Business = 1,
            Patient,
            Family,
            Concierge
        }
    }
}
