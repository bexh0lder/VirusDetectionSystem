using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using VirusDetectionSystem.Model;
using VirusDetectionSystem.Utilities;


namespace VirusDetectionSystem.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand FileScanCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Setting(object obj) => CurrentView = new SettingVM();
        private void FileScanView(object obj) => CurrentView = new FileScanVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            SettingsCommand = new RelayCommand(Setting);

            FileScanCommand = new RelayCommand(FileScanView);

            // Startup Page
            CurrentView = new HomeVM();
        }
    }
}
