using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VirusDetectionSystem.Converters;
using VirusDetectionSystem.Utilities;
using VirusDetectionSystem.ViewModel;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VirusDetectionSystem.View
{
    /// <summary>
    /// FileScanView.xaml 的交互逻辑
    /// </summary>
    public partial class FileScanView : UserControl
    {
        public FileScanView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 文件项双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemInfoPageMove_Click(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            ScanItemVM item = (ScanItemVM)listBox.SelectedItem;

            if (item == null || item.FileType == FileTypeState.Folder)
            {
                return;
            }

            FileNameTextBlock.Text = item.FileName;
            FileIconImage.Source = item.IconImage.ToImageSource();
            FileHashMD5TextBlock.Text = item.FileHashMD5;
            FileHashSHA1TextBlock.Text = item.FileHashSHA1;
            FileHashSHA256TextBlock.Text = item.FileHashSHA256;
            FileCRC32TextBlock.Text = item.FileHashCRC32;
            FilePathTextBlock.Text = item.FilePath;
            FileSizeBytesTextBlock.Text = item.FileSizeBytes.ToString() + " bytes";

            FileHashWindowShow();
        }

        /// <summary>
        /// FileHash窗口显示动画
        /// </summary>
        private void FileHashWindowShow()
        {
            //初始化System.Windows.Media.Animation.Storyboard类的新实例。Storyboard(脚本)
            Storyboard sb = new Storyboard();

            DoubleAnimation da1 = new DoubleAnimation(Window.ActualWidth, new Duration(TimeSpan.FromSeconds(0.5)));//界面一X轴移动动画

            FileScanWindow.RenderTransform = new TranslateTransform();

            da1.AutoReverse = false;//不重复执行

            Storyboard.SetTarget(da1, FileScanWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da1, new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));//依赖的属性

            DoubleAnimation da1Opacity = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));//不透明度设置为不可见

            Storyboard.SetTarget(da1Opacity, FileScanWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da1Opacity, new PropertyPath(Border.OpacityProperty));//依赖的属性

            DoubleAnimation da2 = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));//界面二X轴移动动画

            FileHashWindow.RenderTransform = new TranslateTransform();

            da2.AutoReverse = false;//不重复执行

            Storyboard.SetTarget(da2, FileHashWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da2, new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));//依赖的属性

            DoubleAnimation da2Opacity = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0.5)));//不透明度设置为可见

            Storyboard.SetTarget(da2Opacity, FileHashWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da2Opacity, new PropertyPath(Border.OpacityProperty));//依赖的属性


            //向故事板中加入此浮点动画
            sb.Children.Add(da1);

            sb.Children.Add(da1Opacity);

            sb.Children.Add(da2);

            sb.Children.Add(da2Opacity);

            sb.Begin();//播放此动画
        }
        /// <summary>
        /// 展示文件的哈希值信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileHashWindowReturn_Click(object sender, RoutedEventArgs e)
        {
            FileHashWindowConceal();
        }
        /// <summary>
        /// FileHash窗口隐藏动画
        /// </summary>
        private void FileHashWindowConceal()
        {
            Storyboard sb = new Storyboard();


            DoubleAnimation da1 = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));//界面一X轴移动动画

            FileScanWindow.RenderTransform = new TranslateTransform();

            da1.AutoReverse = false;

            Storyboard.SetTarget(da1, FileScanWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da1, new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));//依赖的属性

            DoubleAnimation da1Opacity = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0.5)));//不透明度设置为可见

            Storyboard.SetTarget(da1Opacity, FileScanWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da1Opacity, new PropertyPath(Border.OpacityProperty));//依赖的属性




            DoubleAnimation da2 = new DoubleAnimation(Window.ActualWidth, new Duration(TimeSpan.FromSeconds(0.5)));//界面二X轴移动动画

            FileHashWindow.RenderTransform = new TranslateTransform();

            da2.AutoReverse = false;

            Storyboard.SetTarget(da2, FileHashWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da2, new PropertyPath("(Border.RenderTransform).(TranslateTransform.X)"));//依赖的属性

            DoubleAnimation da2Opacity = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));//不透明度设置为不可见

            Storyboard.SetTarget(da2Opacity, FileHashWindow);//绑定动画为这个按钮执行的浮点动画

            Storyboard.SetTargetProperty(da2Opacity, new PropertyPath(Border.OpacityProperty));//依赖的属性


            sb.Children.Add(da1);//向故事板中加入此浮点动画

            sb.Children.Add(da1Opacity);

            sb.Children.Add(da2);//向故事板中加入此浮点动画

            sb.Children.Add(da2Opacity);

            sb.Begin();//播放此动画
        }

        private void SearchVirusTotalWeb_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileHashMD5TextBlock.Text))
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://www.virustotal.com/gui/file/" + FileHashMD5TextBlock.Text);
            }
            else
            {
                MessageBox.Show("哈希值为空");
            }
        }
    }
}
