using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public string CreatedTime { get; set; }
    }
}
