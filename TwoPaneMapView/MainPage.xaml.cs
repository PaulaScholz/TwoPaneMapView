using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using MUXC = Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwoPaneMapView
{
    /// <summary>
    /// The DominantView reflects which Pane of the TwoPaneView has 
    /// priority of display.  Main is Pane1, Display is Pane2,
    /// and Shared reflects spanned status acrosss both screens.
    /// </summary>
    public enum DominantView { Main, Display, Shared }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        /// <summary>
        /// This lets downlevel Pages or UserControls access MainPage public instance methods and 
        /// properties through this static instance variable, set in the MainPage constructor.
        /// </summary>
        public static MainPage Current = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        // These are used to make individual panes visible or not.
        private GridLength OneStarGridLength = new GridLength(1, GridUnitType.Star);
        private GridLength ZeroStarGridLength = new GridLength(0, GridUnitType.Star);

        private bool _applicationIsSpanned = false;

        /// <summary>
        /// True if the application is spanned across two screens.  We may bind 
        /// this in the UI in future, so implement property change notification.
        /// </summary>
        public bool ApplicationIsSpanned
        {
            get { return _applicationIsSpanned; }
            set
            {
                Set(ref _applicationIsSpanned, value);
                OnPropertyChanged(nameof(ApplicationNotSpanned));
            }
        }

        /// <summary>
        /// Flag to indicate that we were previously spanned.
        /// </summary>
        private bool applicationWasSpanned = false;

        /// <summary>
        /// This is bound in the UI to the back button visibility.
        /// </summary>
        public bool ApplicationNotSpanned
        {
            get { return !_applicationIsSpanned; }
        }

        private double _buttonWidth = 140;

        /// <summary>
        /// The width of the category buttons, which changes depending on orientation.
        /// </summary>
        public double ButtonWidth
        {
            get { return _buttonWidth; }
            set
            {
                Set(ref _buttonWidth, value);
            }
        }

        /// <summary>
        /// Returns which view (MainView, DisplayView or shared views) is dominant in the display.
        /// </summary>
        public DominantView CurrentDominantView { get; set; }

        public ObservableCollection<IncidentDetail> Items { get; set; }

        // key is the incident ID sent by the server, payload is the NotificationListItem with IncidentDetail.  These incidents appear on the map and
        // can only be removed there.  They are added to this dictionary in the Notifications listview control when the Plot button is pressed.
        public static ObservableConcurrentDictionary<string, NotificationListItem> MappedIncidents = new System.Collections.Concurrent.ObservableConcurrentDictionary<string, NotificationListItem>();

        public MainPage()
        {
            this.InitializeComponent();

            // point the static instance variable to this instance of the Page.
            Current = this;

            // set our Items collection to the public static App collection
            Items = App.IncidentCollection;

            // uncomment this if you have page-specific initialization
            Loaded += MainPage_Loaded;

            SizeChanged += MainPage_SizeChanged;

            MainView.ModeChanged += MainView_ModeChanged;

            SetBothPanesEqual();
        }

        /// <summary>
        /// Do any page-specific initialization here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // put your code here
        }

        /// <summary>
        /// Used for debugging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainView_ModeChanged(MUXC.TwoPaneView sender, object args)
        {
            switch (sender.Mode)
            {
                case MUXC.TwoPaneViewMode.SinglePane:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is SinglePane");
                    break;
                case MUXC.TwoPaneViewMode.Tall:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is Tall");
                    break;
                case MUXC.TwoPaneViewMode.Wide:
                    //
                    Debug.WriteLine("MainView_ModeChanged TwoPaneView Mode is Wide");
                    break;
                default:
                    //
                    break;
            }
        }

        /// <summary>
        /// Fired when a rotation or spanning occurs. Dual-Screen experience windows
        /// are either maximized or minimized, there is no intermediate position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            switch (ApplicationView.GetForCurrentView().ViewMode)
            {
                case ApplicationViewMode.Spanning:
                    //
                    Debug.WriteLine("MainPage_SizeChanged View Mode is ApplicationViewMode.Spanning");
                    ApplicationIsSpanned = !ApplicationIsSpanned;
                    break;
                case ApplicationViewMode.Default:
                    //
                    Debug.WriteLine("MainPage_SizeChanged View Mode is ApplicationViewMode.Default");
                    ApplicationIsSpanned = false;
                    break;
                case ApplicationViewMode.CompactOverlay:
                    //
                    Debug.WriteLine("MainPage_SizeChanged View Mode is ApplicationViewMode.CompactOverlay");
                    break;
                default:
                    //
                    break;
            }


            // if we're spanned and we have a current contact
            // if the application is spanned, it doesn't matter what the Pane1Length or Pane2Length is
            //if (ApplicationIsSpanned && GroupedListView.Current.SelectedContact != null)
            //{
            //    // set the flag so we know we were spanned
            //    applicationWasSpanned = true;
            //}
            //else if (!ApplicationIsSpanned && GroupedListView.Current.SelectedContact != null)  // not spanned and have a current contact
            //{
            //    // if we were spanned and are now not
            //    if (applicationWasSpanned)
            //    {
            //        // We want to see the DisplayView dominant
            //        SetDisplayViewDominant();

            //        applicationWasSpanned = false;
            //    }
            //    else if (CurrentDominantView == DominantView.Main)
            //    {
            //        // if we weren't spanned, and still are not, set GroupInfoList dominant
            //        SetMainViewDominant();
            //    }
            //    else
            //    {
            //        // set the DisplayView Contact edit form dominant
            //        SetDisplayViewDominant();
            //    }
            //}
        }

        /// <summary>
        /// Make Pane 1 of the TwoPaneView the dominant visible pane. We are doing
        /// this explicitly, but we could also bind the MainView.Pane1Length
        /// and MainView.Pane2Length properties in the XAML if we desired.
        /// </summary>
        public void SetMainViewDominant()
        {
            MainView.Pane1Length = OneStarGridLength;
            MainView.Pane2Length = ZeroStarGridLength;
        }

        /// <summary>
        /// Make Pane 2 of the TwoPaneView the dominant visible pane.
        /// </summary>
        public void SetDisplayViewDominant()
        {
            MainView.Pane1Length = ZeroStarGridLength;
            MainView.Pane2Length = OneStarGridLength;
        }

        /// <summary>
        /// Make both panes of equal GridLength, and thus both equally visible.
        /// </summary>
        public void SetBothPanesEqual()
        {
            MainView.Pane1Length = OneStarGridLength;
            MainView.Pane2Length = OneStarGridLength;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
