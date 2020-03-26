using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TwoPaneMapView
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        // holds incident locations
        public static ObservableCollection<IncidentDetail> IncidentCollection = new ObservableCollection<IncidentDetail>();

        // key is the incident ID, payload is the NotificationListItem with IncidentDetail.  These incidents appear on the map and
        // can only be removed there.  They are added to this dictionary in the Notifications listview control when the Plot button is pressed.
        public static ObservableConcurrentDictionary<string, NotificationListItem> MappedIncidents = new System.Collections.Concurrent.ObservableConcurrentDictionary<string, NotificationListItem>();

        // used to build test data, not important otherwise
        private static int incidentCount = 0;
        
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // create the static collection of Incident Locations
                if (IncidentCollection.Count < 1)
                {
                    CreateIncidentLocations();
                }

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void CreateIncidentLocations()
        {
            // this only gets executed once. These are some randomly
            // chosen map locations on the Microsoft campus, mostly
            // in the rebuild area, now demolished.
            IncidentCollection.Add(MakeIncidentDetail("47.642054", "-122.131759"));
            IncidentCollection.Add(MakeIncidentDetail("47.641969", "-122.129852"));
            IncidentCollection.Add(MakeIncidentDetail("47.641320", "-122.131681"));
            IncidentCollection.Add(MakeIncidentDetail("47.640568", "-122.131630"));
            IncidentCollection.Add(MakeIncidentDetail("47.637459", "-122.131007"));
            IncidentCollection.Add(MakeIncidentDetail("47.639305", "-122.126292"));
            IncidentCollection.Add(MakeIncidentDetail("47.639497", "-122.128260"));
            IncidentCollection.Add(MakeIncidentDetail("47.645151", "-122.142118"));
            IncidentCollection.Add(MakeIncidentDetail("47.645289", "-122.334768"));
        }

        private IncidentDetail MakeIncidentDetail(string latitude, string longitude)
        {
            IncidentDetail retValue = new IncidentDetail();
            incidentCount++;

            retValue.Id = new Guid().ToString();
            retValue.DateTimeOffset = DateTime.UtcNow.ToString();
            retValue.IncidentHeader = string.Format("Security Alarm - Incident {0}", incidentCount);
            retValue.StatusCode = "Alarm";
            retValue.IncidentDescription = "Security Alarm Trip";
            retValue.Latitude = latitude;
            retValue.Longitude = longitude;

            return retValue;
        }

            /// <summary>
            /// Invoked when Navigation to a certain page fails
            /// </summary>
            /// <param name="sender">The Frame which failed navigation</param>
            /// <param name="e">Details about the navigation failure</param>
            void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
