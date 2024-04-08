using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace VirusDetectionSystem.Utilities
{
    /// <summary>
    /// 处理文件图标相关的功能，例如获取文件的图标，将Icon类型转换为ImageSource类型
    /// </summary>
    public static class IconHelper
    {
        // 导入所需的DLL
        #region DLLs

        // 从GDI+库导入DeleteObject函数，用于删除GDI对象
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObjetc);

        // 从shell32库导入SHGetFileInfo函数，用于获取文件信息
        [DllImport("shell32")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        #endregion

        // 定义了一系列的标识，用于SHGetFileInfo函数
        #region Flags

        private const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        private const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        private const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        private const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        private const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        private const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        private const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        private const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        private const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        private const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
        private const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        private const uint SHGFI_ICON = 0x000000100;     // get icon
        private const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
        private const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        private const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
        private const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        private const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
        private const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        private const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
        private const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        private const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
        private const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
        private const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        private const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
        private const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute

        #endregion

        // 定义了SHFILEINFO结构，用于接收SHGetFileInfo函数的返回值
        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        #endregion

        // 定义了一个扩展方法，用于将Icon类型转换为ImageSource类型
        public static ImageSource ToImageSource(this Icon ico)
        {
            Bitmap bitmap;
            IntPtr hBitmap = IntPtr.Zero;

            try
            {
                bitmap = ico.ToBitmap();
                hBitmap = bitmap.GetHbitmap();

                ImageSource image =
                    Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                if (!DeleteObject(hBitmap))
                {
                    //MessageBox.Show("Failed to delete unused bitmap object");
                }

                return image;
            }
            catch { }
            finally
            {
                if (hBitmap != null && hBitmap == IntPtr.Zero)
                {
                    try { DeleteObject(hBitmap); }
                    catch { }
                }
            }

            return null;
        }

        // 定义了一个方法，用于获取文件的图标
        public static Icon GetIconOfFile(string path, bool smallIcon, bool isDirectoryOrDrive)
        {
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (smallIcon)
                flags |= SHGFI_SMALLICON;

            uint attributes = FILE_ATTRIBUTE_NORMAL;
            if (isDirectoryOrDrive)
                attributes |= FILE_ATTRIBUTE_DIRECTORY;

            int success =
                SHGetFileInfo(
                    path,
                    attributes,
                    out SHFILEINFO shfi,
                    (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                    flags);

            if (success == 0)
                return null;

            return Icon.FromHandle(shfi.hIcon);
        }
    }
}
