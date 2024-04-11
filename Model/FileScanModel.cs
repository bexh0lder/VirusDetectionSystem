using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusDetectionSystem.Model
{
    public class FileScanModel
    {
        public string? ScanPath { get; set; }

        /// <summary>
        /// 搜索到的文件夹数目
        /// </summary>
        public int FolderCount { get; set; }

        /// <summary>
        /// 搜索到的文件数目
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 当前的搜索的文件
        /// </summary>
        public string? CurrentDetect { get; set; }

        /// <summary>
        /// 当前的扫描的文件
        /// </summary>
        public string? CurrentScan { get; set; }

        /// <summary>
        /// 是否正在扫描
        /// </summary>
        public bool IsScanning { get; set; }

        /// <summary>
        /// 能否进行扫描
        /// </summary>
        public bool CanScan { get; set; }

        /// <summary>
        /// 是否正在搜索
        /// </summary>
        public bool IsDetecting { get; set; }

        /// <summary>
        /// 能否进行搜索
        /// </summary>
        public bool CanDetect { get; set; }

        ///// <summary>
        ///// 是否区分大小写
        ///// </summary>
        //public bool CaseSensitive { get; set; }

        ///// <summary>
        ///// 是否递归扫描
        ///// </summary>
        //public bool SearchRecursive { get; set; }
    }
}
