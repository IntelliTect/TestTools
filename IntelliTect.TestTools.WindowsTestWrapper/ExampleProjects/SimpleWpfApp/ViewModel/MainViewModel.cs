using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace SimpleWpfApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _MyListEntry;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            
            SaveCommand = new RelayCommand( OnClickSaveCommand );
            MyList = new BindingList<ListItemViewModel>();
        }

        private void OnClickSaveCommand()
        {
            MyList.Add( new ListItemViewModel {MyListItem = MyListEntry} );
            MyListEntry = "";
        }

        public string MyListEntry
        {
            get { return _MyListEntry; }
            set { base.Set( ref _MyListEntry, value ); }
        }

        public ICommand SaveCommand { get; }
        public BindingList<ListItemViewModel> MyList { get; }
    }
}