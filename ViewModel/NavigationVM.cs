using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using VirusDetectionSystem.Model;
using VirusDetectionSystem.Utilities;
using VirusDetectionSystem.View;


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

        public ICommand DetectionRecordCommand { get; set; }
        public ICommand FileScanCommand { get; set; }
        public ICommand PEFileAnalysisCommand { get; set; }
        public ICommand VirusDatabaseCommand { get; set; }
        public ICommand FileWhiteListCommand { get; set; }
        public ICommand HexEditorCommand { get; set; }

        public ICommand SettingsCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();

        private void DetectionRecordView(object obj) => CurrentView = new DetectionRecordVM();
        private void FileScanView(object obj) => CurrentView = new FileScanVM();
        private void PEFileAnalysisView(object obj) => CurrentView = new PEFileAnalysisVM();
        private void VirusDatabaseView(object obj) => CurrentView = new VirusDatabaseVM();
        private void FileWhiteListView(object obj) => CurrentView = new FileWhiteListVM();
        private void HexEditorView(object obj) => CurrentView = new HexEditorVM();

        private void Setting(object obj) => CurrentView = new SettingVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);

            DetectionRecordCommand = new RelayCommand(DetectionRecordView);
            FileScanCommand = new RelayCommand(FileScanView);
            PEFileAnalysisCommand = new RelayCommand(PEFileAnalysisView);
            VirusDatabaseCommand = new RelayCommand(VirusDatabaseView);
            FileWhiteListCommand = new RelayCommand(FileWhiteListView);
            HexEditorCommand = new RelayCommand(HexEditorView);

            SettingsCommand = new RelayCommand(Setting);

            // 起始页
            //CurrentView = new HomeVM();
            CurrentView = new DetectionRecordVM();
        }
    }
}
