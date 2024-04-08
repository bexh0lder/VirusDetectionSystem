using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Ookii.Dialogs.Wpf;
using VirusDetectionSystem.Model;
using VirusDetectionSystem.Utilities;

namespace VirusDetectionSystem.ViewModel
{
    class FileScanVM : ViewModelBase
    {
        private readonly FileScanModel _fileScanModel;
        public ObservableCollection<ScanItemVM> FileScanResults { get; set; }

        public FileScanVM()
        {
            FileScanResults = new ObservableCollection<ScanItemVM>();
            _fileScanModel = new FileScanModel();
            SelectScanPathCommand = new RelayCommand(SelectScanPath);
            VirusDetectCommand = new RelayCommand(FileScan);
        }

        #region 指令
        public ICommand SelectScanPathCommand { get; set; }
        public ICommand VirusDetectCommand { get; set; }
        #endregion

        #region  扫描路径选择
        /// <summary>
        /// 扫描路径
        /// </summary>
        public string? ScanPath
        {
            get { return _fileScanModel.ScanPath; }
            set { _fileScanModel.ScanPath = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 浏览并选择要扫描的目录路径
        /// </summary>
        /// <param name="o"></param>
        private void SelectScanPath(object o)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.UseDescriptionForTitle = true;
            fbd.Description = "请选择扫描路径";
            if (fbd.ShowDialog() == true)
            {
                if (IsDirectory(fbd.SelectedPath))
                {
                    ScanPath = fbd.SelectedPath;
                }
                else
                {
                    MessageBox.Show("选择的文件夹不存在");
                }
            }
        }

        #endregion

        #region 扫描模块

        /// <summary>
        /// 扫描文件夹路径内的文件和文件夹
        /// </summary>
        /// <param name="o"></param>
        private void FileScan(object o)
        {
            try
            {
                // 停止之前的扫描
                CancelScan();

                // 搜索到的文件、文件夹数目清零
                ClearFileScanCounters();

                if (!string.IsNullOrEmpty(ScanPath))//判断文件夹是否为空
                {
                    if (IsDirectory(ScanPath))
                    {
                        ClearFileScanResults();
                        SetScanState(true);

                        Task.Run(() =>
                        {
                            //if (SearchRecursive)
                            //{
                            //    StartSearchRecursively();
                            //}
                            //else
                            //{
                            //    StartSearchNonRecursively();
                            //}

                            StartScanNonRecursively();

                            SetScanState(false);
                        });

                        //SQLiteHelper.Instance.InsertData(DateTime.Now.ToString("yyyy-MMM-dd-HH-mm-ss"), folderPath.Split("\\").Last(), folderPath, "1");

                        //RefreshDbData();
                    }
                }
            }
            catch (Exception e) { MessageBox.Show($"{e.Message} -- Cancelling Search"); CancelScan(); }
        }
        
        /// <summary>
        /// 扫描到的文件夹数目
        /// </summary>
        public int FolderCount
        {
            get { return _fileScanModel.FolderCount; }
            set { _fileScanModel.FolderCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 扫描到的文件数目
        /// </summary>
        public int FileCount
        {
            get { return _fileScanModel.FileCount; }
            set { _fileScanModel.FileCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 是否正在扫描
        /// </summary>
        public bool IsScanning
        {
            get { return _fileScanModel.IsScanning; }
            set { _fileScanModel.IsScanning = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 能否进行扫描
        /// </summary>
        public bool CanScan
        {
            get { return _fileScanModel.CanScan; }
            set { _fileScanModel.CanScan = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 当前扫描的内容
        /// </summary>
        public string? CurrentScan
        {
            get { return _fileScanModel.CurrentScan; }
            set { _fileScanModel.CurrentScan = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 设置搜索的状态：true（正在进行）、false（停止）
        /// </summary>
        /// <param name="IsScanning"></param>
        public void SetScanState(bool isScanning)
        {
            IsScanning = isScanning;
            CanScan = isScanning ? true : false;
            CurrentScan = "";
        }
        
        /// <summary>
        /// 取消扫描
        /// </summary>
        public void CancelScan()
        {
            SetScanState(false);
        }

        /// <summary>
        /// 将扫描文件数和文件夹数置零
        /// </summary>
        public void ClearFileScanCounters()
        {
            FileCount = 0;
            FolderCount = 0;
        }

        /// <summary>
        /// 将扫描结果清空
        /// </summary>
        public void ClearFileScanResults()
        {
            FileScanResults.Clear();
        }

        /// <summary>
        /// 扫描当前目录
        /// </summary>
        /// <param name="toSearchDir"></param>
        public void DirectorySearch(string toSearchDir)
        {
            //这将循环遍历每个文件夹，然后做同样的事情
            //对于子文件夹等等，直到没有
            //更多子文件夹
            foreach (string folder in Directory.GetDirectories(toSearchDir))
            {
                // Cancel search if needed
                if (!CanScan) return;

                foreach (string file in Directory.GetFiles(folder))
                {
                    if (!CanScan) return;

                    SearchFileName(file);
                }

                FolderCount++;

                // This is what makes this run recursively, the fact you
                // can the same function in the same function... sort of
                //这就是它递归运行的原因
                //可以在同一个函数中使用同一个函数…的
                DirectorySearch(folder);
            }
        }

        /// <summary>
        /// 非递归扫描，即不会扫描子文件夹内的文件
        /// </summary>
        public void StartScanNonRecursively()
        {
            foreach (string file in Directory.GetDirectories(ScanPath))
            {
                if (!CanScan) return;
                SearchFolderName(file);
            }

            foreach (string file in Directory.GetFiles(ScanPath))
            {
                if (!CanScan) return;
                bool hasFoundFile = SearchFileName(file);

                // Again this stops items from being searched when they've
                // Already been found in the above code from the name.
                if (!hasFoundFile)
                {
                    ReadAndSearchFile(file, false);
                }
            }

        }

        public void ReadAndSearchFile(string file, bool increaseSearchedFiles)
        {
            try
            {
                CurrentScan = file;

                // FileStreams are better because they wont load
                // The entire file into memory which is very good
                // If the file to be searched is maybe 1 gigabyte.
                using (FileStream fs = File.OpenRead(file))
                {
                    // Read the file in chunks of 1kb.
                    byte[] b = new byte[1024];
                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        // Cancels the search if CanSearch is false
                        if (!CanScan) return;

                        // Get text from buffer
                        string txt = Encoding.ASCII.GetString(b);

                        // Inline convert the text to lower if CaseSensitive is false
                        //if ((CaseSensitive ? txt : txt.ToLower()).Contains(searchText))
                        //{
                        ResultFound(file);
                        break;
                        //}
                    }
                }

                if (increaseSearchedFiles)
                    FileCount++;
            }
            catch (Exception e) { MessageBox.Show($"{e.Message} -- Cancelling Search"); CancelScan(); }
        }

        /// <summary>
        /// 查询文件名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool SearchFileName(string filePath)
        {
            CurrentScan = filePath;
            // 是否区分大小写
            //string fPath = CaseSensitive ? name : name.ToLower();
            ResultFound(filePath);
            FileCount++;
            return true;
        }

        /// <summary>
        /// 查询文件夹名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public bool SearchFolderName(string folderPath)
        {
            ResultFound(folderPath);
            FolderCount++;
            return true;
        }

        public void ResultFound(string path)
        {
            ScanItemVM result = CreateResultFromPath(path);
            if (result != null)
                AddResultAsync(result);
        }

        /// <summary>
        /// 异步添加扫描结果
        /// </summary>
        /// <param name="result"></param>
        public void AddResultAsync(ScanItemVM result)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                FileScanResults.Add(result);
            });
        }

        public ScanItemVM CreateResultFromPath(string path)
        {
            if (IsFile(path))
            {
                try
                {
                    FileInfo fInfo = new FileInfo(path);
                    ScanItemVM result = new ScanItemVM()
                    {
                        IconImage = IconHelper.GetIconOfFile(path, false, false),
                        FileName = fInfo.Name,
                        FilePath = fInfo.FullName,
                        //Selection = selectionText,
                        FileSizeBytes = fInfo.Length,
                        IsScanCompleteColor = Brushes.White,
                        IsScanComplete = "未扫描",
                        IsSkipScan = false,
                        IsPE = false,
                        FileType = FileTypeState.File
                    };

                    return result;
                }
                catch (Exception e) { MessageBox.Show($"{e.Message}"); return null; }
            }

            else if (IsDirectory(path))
            {
                try
                {
                    DirectoryInfo dInfo = new DirectoryInfo(path);
                    ScanItemVM result = new ScanItemVM()
                    {
                        IconImage = IconHelper.GetIconOfFile(path, false, true),
                        FileName = dInfo.Name,
                        FilePath = dInfo.FullName,
                        //Selection = selectionText,
                        // This is the flag used before
                        // In the FileSizeFormatterConverter
                        IsScanCompleteColor = Brushes.White,
                        IsScanComplete = "未扫描",
                        IsSkipScan = false,
                        IsPE = false,
                        FileSizeBytes = long.MaxValue,
                        FileType = FileTypeState.Folder
                    };

                    return result;
                }
                catch (Exception e) { MessageBox.Show($"{e.Message}"); return null; }
            }

            return null;
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 检查路径是否为文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFile(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        /// <summary>
        /// 判断路径是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        /// <summary>
        /// 检查路径是否为驱动器
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDrive(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        /// <summary>
        /// 获取路径中文件的名称
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static string GetFileName(string fullpath)
        {
            return System.IO.Path.GetFileName(fullpath);
        }

        /// <summary>
        /// 返回目录/文件夹的名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetDirectoryName(string path)
        {
            return IsDirectory(path) ? new DirectoryInfo(path).Name : "";
        }


        #endregion
    }
}
