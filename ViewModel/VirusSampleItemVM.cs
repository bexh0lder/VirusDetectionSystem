using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusDetectionSystem.ViewModel
{
    public class VirusSampleItemVM
    {
        public VirusSampleItemVM(string sampleId, string sampleName, string sampleHash, string createdTime)
        {
            SampleId = sampleId;
            SampleName = sampleName;
            SampleHash = sampleHash;
            CreatedTime = createdTime;
        }

        public string SampleId { get; set; }
        public string SampleName { get; set; }
        public string SampleHash { get; set; }
        public string CreatedTime { get; set; }
    }
}
