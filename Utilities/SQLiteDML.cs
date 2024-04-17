using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusDetectionSystem.Utilities
{
    public class SQLiteDML
    {
        public const string InsertVirusSampleData = $"INSERT OR IGNORE INTO VirusSample (FileName,FileHash,CreatedTime) " +
            $"VALUES (@FileName,@FileHash,@CreatedTime)";

        public const string InsertScanVirusData = $"INSERT OR IGNORE INTO ScanRecord (ScanPath,FileCount,FolderCount,VirusCount,ScanTime) " +
           $"VALUES (@Path,@FiCou,@FoCou,@ViCou,@Time)";

        public const string InsertFileWhiteListData = $"INSERT OR IGNORE INTO FileWhiteList (File_name,File_hash,CreatedTime) " +
           $"VALUES (@File_name,@File_hash,@CreatedTime)";

    }
}
