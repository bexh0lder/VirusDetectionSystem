using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using VirusDetectionSystem.Utilities;

namespace VirusDetectionSystem.ViewModel
{
    class FileWhiteListItemVM
    {
        public FileWhiteListItemVM(string fileId, string fileName, string fileHash, string createdTime)
        {
            FileId = fileId;
            FileName = fileName;
            FileHash = fileHash;
            CreatedTime = createdTime;

            ItemDeleteCommand = new RelayCommand(ItemDelete);
        }

        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public string CreatedTime { get; set; }

        public ICommand ItemDeleteCommand { get; set; }

        private void ItemDelete(object e)
        {
            if (SQLiteHelper.Instance.DeleteWhiteFile(FileHash))
            {
                MessageBox.Show("删除成功，请重新刷新!");
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }
    }
}
