using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VirusDetectionSystem.Model;
using VirusDetectionSystem.Utilities;

namespace VirusDetectionSystem.ViewModel
{
    class SettingVM : ViewModelBase
    {
        private readonly FileScanModel _pageModel;

        public SettingVM()
        {
            _pageModel = new FileScanModel();
        }
    }
}
