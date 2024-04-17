using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using VirusDetectionSystem.Utilities;

namespace VirusDetectionSystem.ViewModel
{
    public class DetectionRecordVM : ViewModelBase
    {
        public ObservableCollection<DetectionRecordItemVM> DetectionRecordResults { get; set; }

        public DetectionRecordVM()
        {
            DetectionRecordResults = new ObservableCollection<DetectionRecordItemVM>();

            RefreshDbData();
        }

        /// <summary>
        /// 刷新扫描记录数据
        /// </summary>
        private void RefreshDbData()
        {
            DataTable dt = SQLiteHelper.Instance.GetScanVirusData();

            int RowCount = dt.Rows.Count;

            DetectionRecordResults.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                DetectionRecordResults.Add(new DetectionRecordItemVM(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString()));
            }
        }

        private ICommand recoverRecordItemCommand;
        public ICommand RecoverRecordItemCommand
        {
            get { return recoverRecordItemCommand; }
            set { recoverRecordItemCommand = value; OnPropertyChanged(); }
        }
    }
}
