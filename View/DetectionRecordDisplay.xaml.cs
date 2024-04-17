using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using VirusDetectionSystem.ViewModel;

namespace VirusDetectionSystem.View
{
    /// <summary>
    /// DetectionRecordDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class DetectionRecordDisplay : Window
    {
        public DetectionRecordDisplay(DetectionRecordItemVM detectionRecordItem)
        {
            InitializeComponent();
            this.DataContext = detectionRecordItem;
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
