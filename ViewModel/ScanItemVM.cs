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
            //FileHashList = new List<String>();
        }

        // ICON（图标）转换成照片
        private Icon _Iconimage;
        public Icon IconImage
        {
            get { return _Iconimage; }
            set { _Iconimage = value; OnPropertyChanged(); }
        }

        // 文件MD5哈希值
        private string _fileHashMD5;
        public string FileHashMD5
        {
            get { return _fileHashMD5; }
            set { _fileHashMD5 = value; OnPropertyChanged(); }
        }

        // 文件SHA1哈希值
        private string _fileHashSHA1;

        public string FileHashSHA1
        {
            get { return _fileHashSHA1; }
            set { _fileHashSHA1 = value; }
        }
        // 文件SHA256哈希值
        private string _fileHashSHA256;

        public string FileHashSHA256
        {
            get { return _fileHashSHA256; }
            set { _fileHashSHA256 = value; }
        }

        // 文件CRC32校验值
        private string _fileHashCRC32;

        public string FileHashCRC32
        {
            get { return _fileHashCRC32; }
            set { _fileHashCRC32 = value; }
        }

        //// 文件哈希值列表
        //private List<string> _fileHashList;
        //public List<string> FileHashList
        //{
        //    get { return _fileHashList; }
        //    set { _fileHashList = value; OnPropertyChanged(); }
        //}

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

        // 检测结果
        private string _detectResult;
        public string DetectResult
        {
            get { return _detectResult; }
            set { _detectResult = value; OnPropertyChanged(); }
        }

        // 检测结果字体颜色
        private System.Windows.Media.Brush _detectResultColor;
        public System.Windows.Media.Brush DetectResultColor
        {
            get { return _detectResultColor; }
            set { _detectResultColor = value; OnPropertyChanged(); }
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
