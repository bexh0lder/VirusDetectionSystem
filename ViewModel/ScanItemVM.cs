using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VirusDetectionSystem.Utilities;

namespace VirusDetectionSystem.ViewModel
{
    public enum FolderOrFileState
    {
        未扫描,
        已扫描,
        有病毒
    }
    public enum FileTypeState
    {
        Folder, File
    }
    public class ScanItemVM : ViewModelBase
    {
        public ScanItemVM() 
        {
            FileHashList = new List<String>();
        }

        // ICON（图标）转换成照片
        private Icon _Iconimage;
        public Icon IconImage
        {
            get { return _Iconimage; }
            set { _Iconimage = value; OnPropertyChanged(); }
        }

        // 文件哈希值
        private string _fileHash;
        public string FileHash
        {
            get { return _fileHash; }
            set { _fileHash = value; OnPropertyChanged(); }
        }

        // 文件哈希值列表
        private List<string> _fileHashList;
        public List<string> FileHashList
        {
            get { return _fileHashList; }
            set { _fileHashList = value; OnPropertyChanged(); }
        }

        // 文件名
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged(); }
        }

        // 文件路径
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged(); }
        }

        // 文件大小
        private long _fileSizeBytes;
        public long FileSizeBytes
        {
            get { return _fileSizeBytes; }
            set { _fileSizeBytes = value; OnPropertyChanged(); }
        }

        // 文件是否被选中
        private string _selection;
        public string Selection
        {
            get { return _selection; }
            set { _selection = value; OnPropertyChanged(); }
        }

        // 检测结果（未扫描、已扫描、有病毒）
        private string _isScanComplete;
        public string IsScanComplete
        {
            get { return _isScanComplete; }
            set { _isScanComplete = value; OnPropertyChanged(); }
        }

        // 检测结果字体颜色
        private System.Windows.Media.Brush _isScanCompleteColor;
        public System.Windows.Media.Brush IsScanCompleteColor
        {
            get { return _isScanCompleteColor; }
            set { _isScanCompleteColor = value; OnPropertyChanged(); }
        }

        // 是否跳过扫描
        private bool _isSkipScan;
        public bool IsSkipScan
        {
            get { return _isSkipScan; }
            set { _isSkipScan = value; OnPropertyChanged(); }
        }

        // 是否是PE文件
        private bool _isPE;
        public bool IsPE
        {
            get { return _isPE; }
            set { _isPE = value; OnPropertyChanged(); }
        }

        // 文件类型，无需绑定
        private FileTypeState _fileType;
        public FileTypeState FileType
        {
            get { return _fileType; }
            set { _fileType = value; OnPropertyChanged(); }
        }
    }
}
