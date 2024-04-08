using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Data;
using VirusDetectionSystem.ViewModel;

namespace VirusDetectionSystem.Converters
{
    public class FileNameToExtensionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string name && values[1] is FileTypeState type)
            {
                if (type == FileTypeState.File)
                {
                    return Path.GetExtension(name);
                }
                else if (type == FileTypeState.Folder)
                {
                    return "Folder";
                }
            }
            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
