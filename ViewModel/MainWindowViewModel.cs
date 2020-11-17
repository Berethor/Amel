using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextClassificator.DataParser;
using TextClassificator.Model;

namespace TextClassificator.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {

        private MainWindowModel _model;

        #region RelayCommands
        public RelayCommand LoadScience { get; private set; }
        public RelayCommand LoadFun { get; private set; }
        public RelayCommand LoadPoem { get; private set; }
        public RelayCommand LoadForCheck { get; private set; }
        public RelayCommand LoadForCheckFiles { get; private set; }
        public RelayCommand Clean { get; private set; }
        #endregion
        public ObservableCollection<ParserFileInfo> Infos
        {
            get
            {
                return _model.Infos;
            }
            set
            {
                _model.Infos = value;
                OnPropertyChanged("Infos");
            }
        }
        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
            LoadScience = new RelayCommand(LoadScienceFile);
            LoadFun = new RelayCommand(LoadFunFile);
            LoadPoem = new RelayCommand(LoadPoemFile);
            LoadForCheck = new RelayCommand(LoadCheckFile);
            LoadForCheckFiles = new RelayCommand(LoadCheckFiles);
            Clean = new RelayCommand(CleanFiles);
        }

        #region Commands
        
        public void LoadScienceFile()
        {
            _model.LoadFile("Эталон Научной");
        }
        public void LoadFunFile()
        {
            _model.LoadFile("Эталон художественного");
        }
        public void LoadPoemFile()
        {
            _model.LoadFile("Эталон стихотворения");
        }
        public void LoadCheckFile()
        {
            _model.LoadFile("Проверка");
            _model.CheckFile();
        }
        public void LoadCheckFiles()
        {
            _model.LoadFile("Проверка",true);
            _model.CheckFile();
        }
        public void CleanFiles()
        {
            _model.Clean();
            OnPropertyChanged("Infos");
        }
        #endregion
    }
}
