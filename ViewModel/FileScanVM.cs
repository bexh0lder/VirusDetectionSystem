using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
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
        enum ScanType
        {
            扫描成功,
            疑似病毒,
            发生错误,
            终止扫描
        }

        private readonly FileScanModel _fileScanModel;
        public ObservableCollection<ScanItemVM> FileScanResults { get; set; }

        public FileScanVM()
        {
            FileScanResults = new ObservableCollection<ScanItemVM>();
            _fileScanModel = new FileScanModel();
            WhiteTableReady();

            SelectScanPathCommand = new RelayCommand(SelectScanPath);
            FileScanCommand = new RelayCommand(FileScan);
            VirusDetectCommand = new RelayCommand(VirusDetect);
        }


        #region 指令
        public ICommand SelectScanPathCommand { get; set; }
        public ICommand FileScanCommand { get; set; }
        public ICommand VirusDetectCommand { get; set; }
        #endregion

        #region  扫描路径选择模块
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

        // 白名单
        ObservableCollection<string> WhiteList = null;

        /// <summary>
        /// 准备白名单字典
        /// </summary>
        private void WhiteTableReady()
        {
            DataTable WhiteTable = SQLiteHelper.Instance.GetFileWhiteListData();

            WhiteList = new ObservableCollection<string>();

            Task.Run(() =>
            {
                for (int i = 0; i < WhiteTable.Rows.Count; i++)
                {
                    // 添加文件哈希值
                    WhiteList.Add(WhiteTable.Rows[i][2].ToString());
                }
            });
        }

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

                        // 重置病毒检测进度条
                        InitVirusDetectProgress();
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
                        DetectResultColor = Brushes.White,
                        DetectResult = "未扫描",
                        IsSkipScan = WhiteList.Contains(HashHelper.ComputeMD5(path)) ? true : false,
                        IsPE = false,
                        FileType = FileTypeState.File,
                        FileHashMD5 = HashHelper.ComputeMD5(path),
                        FileHashCRC32 = HashHelper.ComputeCRC32(path),
                        FileHashSHA1 = HashHelper.ComputeSHA1(path),
                        FileHashSHA256 = HashHelper.ComputeSHA256(path)
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
                        DetectResultColor = Brushes.White,
                        DetectResult = "未扫描",
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

        #region 病毒检测模块

        // 按钮内容
        private string virusDetectButtonContent = "病毒检测";
        public string VirusDetectButtonContent
        {
            get { return virusDetectButtonContent; }
            set { virusDetectButtonContent = value; OnPropertyChanged(); }
        }

        // 进度条值
        private int detectProgress = 0;
        public int DetectProgress
        {
            get { return detectProgress; }
            set { detectProgress = value; OnPropertyChanged(); }
        }

        // 进度条总值
        private int detectFileCount = int.MaxValue;
        public int DetectFileCount
        {
            get { return detectFileCount; }
            set { detectFileCount = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 是否正在扫描（病毒）
        /// </summary>
        public bool IsDetecting
        {
            get { return _fileScanModel.IsDetecting; }
            set { _fileScanModel.IsDetecting = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 能否进行病毒检测
        /// </summary>
        public bool CanDetect
        {
            get { return _fileScanModel.CanDetect; }
            set { _fileScanModel.CanDetect = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 停止病毒检测
        /// </summary>
        public void StopDetecting()
        {
            SetDetectState(false);
        }

        public string? CurrentDetect
        {
            get { return _fileScanModel.CurrentDetect; }
            set { _fileScanModel.CurrentDetect = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 设置病毒检测状态
        /// </summary>
        /// <param name="isDetecting"></param>
        public void SetDetectState(bool isDetecting)
        {
            IsDetecting = isDetecting;
            CanDetect = isDetecting;
            CurrentDetect = "";
        }

        /// <summary>
        /// 初始化病毒检测进度条
        /// </summary>
        private void InitVirusDetectProgress()
        {
            DetectFileCount = int.MaxValue;
            DetectProgress = 0;
        }

        /// <summary>
        /// 病毒检测
        /// </summary>
        /// <param name="o"></param>
        private void VirusDetect(object o)
        {
            if (VirusDetectButtonContent == "病毒检测")
            {
                //正在扫描文件，不能病毒检查
                if (IsScanning)
                {
                    return;
                }

                //已经检测一次
                if (DetectProgress == DetectFileCount)
                {
                    return;
                }

                //先停止之前的检测
                StopDetecting();

                //检测文件或文件夹数量，小于等于0直接返回
                if (FileScanResults.Count <= 0)
                {
                    return;
                }

                //设置为启动模式
                SetDetectState(true);

                //改变按钮内容
                VirusDetectButtonContent = "取消病毒检测";

                var t1 = Task.Run(() =>
                {
                    VirusDetectModule();

                    //完成后，状态设置为false
                    SetDetectState(false);
                });
            }
            else
            {
                //先停止之前的搜索
                StopDetecting();

                VirusDetectButtonContent = "病毒检测";

                InitVirusDetectProgress();

                MessageBox.Show("已停止扫描");
            }
        }

        /// <summary>
        /// 跳过扫描
        /// </summary>
        /// <param name="item"></param>
        private void SkipScan(ScanItemVM item)
        {
            item.DetectResult = "跳过扫描";
            //item.IsScanCompleteColor = Brushes.Green;
        }

        /// <summary>
        /// 根据文件MD5哈希值判断文件是否为病毒
        /// </summary>
        /// <param name="fileHashMD5"></param>
        /// <returns></returns>
        private bool SearchVirus(string fileHashMD5)
        {
            // 测试用
            //SQLiteHelper.Instance.InsertVirusSampleData("test",fileHashMD5, DateTime.Now.ToString("s"));
            
            int result = SQLiteHelper.Instance.GetVirusSampleDataBySimpleHash(fileHashMD5);

            if (result != 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void IsVirus(ScanItemVM item, ref int virusCount)
        {
            bool result = SearchVirus(item.FileHashMD5);//病毒检测结果

            if (result)//不是病毒
            {
                item.DetectResult = "扫描完成";
                item.DetectResultColor = Brushes.Green;
            }
            else
            {
                virusCount++;
                item.DetectResult = "疑似病毒";
                item.DetectResultColor = Brushes.Red;
            }
        }

        /// <summary>
        /// 病毒检测模块
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void VirusDetectModule()
        {
            try
            {
                int virusCount = 0;

                // 文件+文件夹数量
                DetectFileCount = FileScanResults.Count;

                // 遍历文件
                for (int i = 0; i < FileScanResults.Count; i++)
                {
                    if (!CanDetect) return;

                    if (FileScanResults[i].IsSkipScan)//跳过扫描
                    {
                        SkipScan(FileScanResults[i]);
                    }
                    else
                    {
                        // 如果是文件夹则跳过
                        if (FileScanResults[i].FileType == FileTypeState.Folder)
                        {
                            FileScanResults[i].DetectResult = "跳过扫描";
                        }
                        else//文件
                        {
                            IsVirus(FileScanResults[i], ref virusCount);
                        }

                        //再去数据库进行匹配
                    }

                    DetectProgress++;
                }

                if (DetectFileCount == DetectProgress)
                {
                    MessageBox.Show("扫描完成");

                    VirusDetectButtonContent = "病毒检测";

                    //扫描完成后，将数据保存到数据库
                    SQLiteHelper.Instance.InsertScanVirusData(ScanPath, FileCount.ToString(),
                        FolderCount.ToString(), virusCount.ToString(), DateTime.Now.ToString("s"));
                }

                StopDetecting();//停止搜索
            }
            catch (Exception ex)
            {
                StopDetecting();//停止搜索
                throw new Exception($"病毒扫描错误{ex.Message}");
            }
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
