using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using VirusDetectionSystem.Utilities;

using VirusDetectionSystem.View;

namespace VirusDetectionSystem.ViewModel
{
    class FileWhiteListVM : ViewModelBase
    {
        public ObservableCollection<FileWhiteListItemVM> WhiteList { get; set; }

        public FileWhiteListVM()
        {
            AddFileIntoWhiteListCommand = new RelayCommand(AddFileIntoWhiteList);

            WhiteList = new ObservableCollection<FileWhiteListItemVM>();

            RefreshDbData();
        }

        public ICommand AddFileIntoWhiteListCommand { get; set; }

        private void AddFileIntoWhiteList(object e)
        {
            AddFileIntoWhiteListDisplay addFileIntoWhiteListDisplay = new AddFileIntoWhiteListDisplay();

            addFileIntoWhiteListDisplay.ShowDialog();

            RefreshDbData();
        }

        //刷新扫描记录数据
        private void RefreshDbData()
        {
            DataTable dt = SQLiteHelper.Instance.GetFileWhiteListData();

            int RowCount = dt.Rows.Count;

            WhiteList.Clear();

            for (int i = 0; i < RowCount; i++)
            {
                WhiteList.Add(new FileWhiteListItemVM(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString()));
            }
        }
    }
}
