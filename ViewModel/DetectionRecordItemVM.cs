using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using VirusDetectionSystem.Utilities;
using VirusDetectionSystem.View;

namespace VirusDetectionSystem.ViewModel
{
    public class DetectionRecordItemVM : ViewModelBase
    {
        public DetectionRecordItemVM Instance = null;
        public DetectionRecordItemVM(string recordId, string scanPath, string fileCount, string folderCount, string virusCount, string scanTime)
        {
            RecordId = recordId;
            ScanPath = scanPath;
            FileCount = fileCount;
            FolderCount = folderCount;
            VirusCount = virusCount;
            ScanTime = scanTime;
            VirusCount = virusCount;

            Instance = this;

            RecoverRecordItemCommand = new RelayCommand(ShowDataView);
        }

        public string RecordId { get; set; }
        public string ScanPath { get; set; }
        public string FileCount { get; set; }
        public string FolderCount { get; set; }
        public string VirusCount { get; set; }
        public string ScanTime { get; set; }

        /// <summary>
        /// 点击查看扫描记录详细数据
        /// </summary>
        public ICommand RecoverRecordItemCommand { get; set; }

        private void ShowDataView(object e)
        {
            DetectionRecordDisplay detectionRecordDisplay = new DetectionRecordDisplay(Instance);

            detectionRecordDisplay.ShowDialog();
        }
    }
}
