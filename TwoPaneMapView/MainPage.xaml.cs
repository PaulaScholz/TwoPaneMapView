using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using MUXC = Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;    // for ApplicationView
using Windows.Graphics.Display;     // for DisplayOrientations enum
using TwoPaneMapView.UserControls;


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

        // the number of toggleButtons we use on Pane 1
        private double _numberOfButtons = 5;

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

        private DisplayOrientations _currentDisplayOrientation = DisplayOrientations.Portrait;

        public DisplayOrientations CurrentDisplayOrientation
        {
            get { return _currentDisplayOrientation; }
            set
            {
                Set(ref _currentDisplayOrientation, value);
            }
        }


        /// <summary>
        /// This is bound in the UI to the back button visibility.
        /// </summary>
        public bool ApplicationNotSpanned
        {
            get { return !_applicationIsSpanned; }
        }

        MUXC.TwoPaneViewMode _currentTPVMode = MUXC.TwoPaneViewMode.Wide;

        public MUXC.TwoPaneViewMode CurrentTPVMode
        {
            get { return _currentTPVMode; }
            set
            {
                Set(ref _currentTPVMode, value);
            }
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

        private double _beginningWindowWidth = 0;
        private double _beginningWindowHeight = 0;

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

            // this is for the page to compute spanning status and orientation
            SizeChanged += MainPage_SizeChanged;

            // this is for the TwoPaneView to compute UI element size changes
            MainView.SizeChanged += MainView_SizeChanged;

            SetBothPanesEqual();
        }

        /// <summary>
        /// Fired when the size of the TwoPaneView changes.  This is where we adjust
        /// UI element sizes within the TwoPaneView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("MainView_SizeChanged fired on TwoPaneView object.");

            // need to adjust by the enclosing Grid's padding and margin values
            double tpvWidth = MainView.ActualWidth - Pane1Grid.Padding.Left - Pane1Grid.Padding.Right
                - Pane1Grid.Margin.Left - Pane1Grid.Margin.Right; 

            if (CurrentDisplayOrientation == DisplayOrientations.Portrait && !ApplicationIsSpanned)
            {
                ButtonWidth = tpvWidth / _numberOfButtons;

                SecurityIncidentListControl.Current.ListViewHeight = 300;

                Debug.WriteLine("Portrait and not spanned");
                Debug.WriteLine(string.Format("MainView.ActualWidth = {0}", MainView.ActualWidth));
                Debug.WriteLine(string.Format("MainView.ActualHeight = {0}", MainView.ActualHeight));
                Debug.WriteLine(string.Format("ButtonWidth = {0}", ButtonWidth));
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Landscape && !ApplicationIsSpanned)
            {
                ButtonWidth = tpvWidth / (_numberOfButtons * 2);

                SecurityIncidentListControl.Current.ListViewHeight = 500;

                Debug.WriteLine("Landscape and not spanned");
                Debug.WriteLine(string.Format("MainView.ActualWidth = {0}", MainView.ActualWidth));
                Debug.WriteLine(string.Format("MainView.ActualHeight = {0}", MainView.ActualHeight));
                Debug.WriteLine(string.Format("ButtonWidth = {0}", ButtonWidth));
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Portrait && ApplicationIsSpanned)
            {
                ButtonWidth = tpvWidth / (_numberOfButtons * 2);

                SecurityIncidentListControl.Current.ListViewHeight = 500;

                Debug.WriteLine("Portrait and spanned");
                Debug.WriteLine(string.Format("MainView.ActualWidth = {0}", MainView.ActualWidth));
                Debug.WriteLine(string.Format("MainView.ActualHeight = {0}", MainView.ActualHeight));
                Debug.WriteLine(string.Format("ButtonWidth = {0}", ButtonWidth));
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Landscape && ApplicationIsSpanned)
            {
                ButtonWidth = tpvWidth / _numberOfButtons;

                SecurityIncidentListControl.Current.ListViewHeight = 500;

                Debug.WriteLine("Landscape and spanned");
                Debug.WriteLine(string.Format("MainView.ActualWidth = {0}", MainView.ActualWidth));
                Debug.WriteLine(string.Format("MainView.ActualHeight = {0}", MainView.ActualHeight));
                Debug.WriteLine(string.Format("ButtonWidth = {0}", ButtonWidth));
            }

            Debug.WriteLine("--------------------------------------------------------------");
        }

        /// <summary>
        /// Do any page-specific initialization here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // put your code here
            _beginningWindowWidth = Window.Current.Bounds.Width;
            _beginningWindowHeight = Window.Current.Bounds.Height;

            Debug.WriteLine(string.Format("Beginning Window Width {0}, Beginning Window Height {1}", _beginningWindowWidth, _beginningWindowHeight));
        }

        /// <summary>
        /// Fired when a rotation or spanning occurs. Dual-Screen experience windows
        /// are either maximized or minimized, there is no intermediate position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine("MainPage_SizeChanged fired on Page object.");

            Debug.WriteLine(string.Format("Previous size: {0} width  {1} height", e.PreviousSize.Width, e.PreviousSize.Height));
            Debug.WriteLine(string.Format("New size: {0} width  {1} height", e.NewSize.Width, e.NewSize.Height));

            // determine orientation & spanning state without using ApplicationView object
            // through this little state machine

            // If the PreviousSize values are zero, then the NewSize
            // values are those at application launch.  If the app
            // is minimized and then maximized, the PreviousSize values
            // are whatever they were before minimized.  The app will
            // always be maximized at an unspanned state, regardless of
            // whether it was spanned before minimization.

            // this if clause determines the initial conditions
            if(e.PreviousSize.Width == 0 && e.PreviousSize.Height == 0)
            {
                _beginningWindowWidth = e.NewSize.Width;
                _beginningWindowHeight = e.NewSize.Height;

                // Right now, all we know is we started from application launch
                // and are unspanned. Let's determine whether or not we're 
                // Landscape or Portrait orientation.
                if(e.NewSize.Width < e.NewSize.Height)
                {
                    CurrentDisplayOrientation = DisplayOrientations.Portrait;
                }
                else
                {                  
                    CurrentDisplayOrientation = DisplayOrientations.Landscape;
                }

                // We always start out unspanned. Spanning is a result of user action.
                ApplicationIsSpanned = false;
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Portrait && !ApplicationIsSpanned)
            {
                // we're transitioning from Portrait-Unspanned to either Portrait-Spanned (spanning action)
                // or Landscape-Unspanned (rotation action)

                // If height does not change, we're going to Portrait-spanned
                if(e.PreviousSize.Height == e.NewSize.Height)
                {
                    ApplicationIsSpanned = true;
                }
                else
                {
                    // the height changed, we're now in Landscape unspanned
                    CurrentDisplayOrientation = DisplayOrientations.Landscape;
                }
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Landscape && !ApplicationIsSpanned)
            {
                // we're transitioning from Landscape-Unspanned to either Landscape-Spanned (spanning action)
                // or Portrait-Unspanned (rotation action)
                if(e.PreviousSize.Width == e.NewSize.Width)
                {
                    ApplicationIsSpanned = true;
                }
                else
                {
                    // the width changed, we're now in Portrait-Unspanned
                    CurrentDisplayOrientation = DisplayOrientations.Portrait;
                }
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Portrait && ApplicationIsSpanned)
            {
                // we're transitioning from Portrait-Spanned to either Portrait-Unspanned (spanning action)
                // or Landscape-Spanned (rotation action)
                if(e.PreviousSize.Height == e.NewSize.Height)
                {
                    ApplicationIsSpanned = false;
                }
                else
                {
                    // the height changed, we're now in Landscape-Spanned
                    CurrentDisplayOrientation = DisplayOrientations.Landscape;
                }
            }
            else if (CurrentDisplayOrientation == DisplayOrientations.Landscape && ApplicationIsSpanned)
            {
                // we're transitioning from Landscape-Spanned to either Landscape-Unspanned (spanning action)
                // or Portrait-Spanned (rotation action)
                if( e.PreviousSize.Width == e.NewSize.Width)
                {
                    ApplicationIsSpanned = false;
                }
                else
                {
                    // the width changed, we're now in Portrait-Spanned
                    CurrentDisplayOrientation = DisplayOrientations.Portrait;
                }
            }            
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
