using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

using TwoPaneMapView.Services;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.Extensions;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace TwoPaneMapView.UserControls
{
    public sealed partial class SecurityIncidentMapControl : UserControl, INotifyPropertyChanged
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static SecurityIncidentMapControl Current = null;
#pragma warning restore CA2211

        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 17;

        private readonly LocationService _locationService;

        private bool _mapElementClickedGate = false;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition _defaultPosition = new BasicGeoposition()
        {
            Latitude = 47.609425,
            Longitude = -122.3417
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { Set(ref _zoomLevel, value); }
        }

        private Geopoint _center;

        public Geopoint Center
        {
            get { return _center; }
            set { Set(ref _center, value); }
        }

        private ObservableCollection<MapLayer> _EffectLayers = new ObservableCollection<MapLayer>();

        public ObservableCollection<MapLayer> EffectLayers
        {
            get { return _EffectLayers; }
            set { Set(ref _EffectLayers, value); }
        }

        private bool _canDelete = false;

        public bool CanDeleteTarget
        {
            get { return _canDelete; }
            set
            {
                Set(ref _canDelete, value);
            }
        }
        public SecurityIncidentMapControl()
        {
            // give access to this instance
            Current = this;

            _locationService = new LocationService();
            Center = new Geopoint(_defaultPosition);
            ZoomLevel = DefaultZoomLevel;
            InitializeComponent();

            // this page needs the dark theme for the Expander text colors to look right
            this.RequestedTheme = ElementTheme.Dark;
        }


        private void DeleteEnableSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            CanDeleteTarget = !CanDeleteTarget;
        }

        // event handler for the TargetListView on the Target Control Panel
        private void TargetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotificationListItem selectedIncident = (sender as ListView).SelectedItem as NotificationListItem;

            if (selectedIncident != null)
            {
                SelectedIncident = selectedIncident;

                //NotifyUser(String.Format("{0} Attack Selected", selectedAttack.TargetCity), NotifyType.StatusMessage);

                DeleteEnableSwitch.IsEnabled = true;

                FlytoButton.IsEnabled = true;
            }
        }

        //protected override async void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    await InitializeAsync();

        //    // Turn the fog off with a custom MapStyleSheet.
        //    // Note that double-quotes are escaped here. No need to do this when loading from a JSON file.
        //    string jsonString = @"{""version"":""1.*"", ""settings"":{""fogColor"":""#00FFFFFF""}}";
        //    MapStyleSheet myCustomStyleSheet = MapStyleSheet.ParseFromJson(jsonString);

        //    MapStyleSheet builtInSheet = MapStyleSheet.AerialWithOverlay();

        //    mapControl.StyleSheet = MapStyleSheet.Combine(new List<MapStyleSheet> { builtInSheet, myCustomStyleSheet });

        //    RenderIncidents();

        //    TargetListExpander.IsExpanded = true;

        //    if (e.Parameter != null)
        //    {
        //        string clickedId = e.Parameter as string;

        //        NotificationListItem item;

        //        MainPage.MappedIncidents.TryGetValue(clickedId, out item);

        //        if (item != null)
        //        {
        //            SelectedIncident = item;
        //        }

        //        FlytoButton_Click(this, null);
        //    }
        //}

        private void RenderIncidents()
        {
            foreach (var item in MainPage.MappedIncidents.Values)
            {
                BasicGeoposition bPos = new BasicGeoposition();
                bPos.Latitude = item.Details.Lat;
                bPos.Longitude = item.Details.Long;

                Geopoint position = new Geopoint(bPos);

                AddIncidentIcon(position, item.Details.IncidentHeader, item.AppNotificationId);
            }
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    Cleanup();
        //}

        public async Task InitializeAsync()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await _locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await _locationService.StartListeningAsync();
                }

                if (initializationSuccessful && _locationService.CurrentPosition != null)
                {
                    Center = _locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    Center = new Geopoint(_defaultPosition);
                }
            }

            if (mapControl != null)
            {
                // This map service token is registered to swiftress@outlook.com, GNChemAlarm project.
                mapControl.MapServiceToken = "rcx5lwRA2KLMHBgYmwaH~RPUbQxscYp8rPtfPsiR68w~Ar-PHr-lJPK2D0Sil1yG-O4nHlp1ag49iZbq8-pgorWqlHVQfKQcMAWvUMAugN4n";

                AddMapIcon(Center, "Map_YourLocation".GetLocalized());
            }
        }

        public void Cleanup()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged -= LocationService_PositionChanged;
                _locationService.StopListening();
            }
        }

        private void LocationService_PositionChanged(object sender, Geoposition geoposition)
        {
            if (geoposition != null)
            {
                Center = geoposition.Coordinate.Point;
            }
        }

        private void AddMapIcon(Geopoint position, string title)
        {
            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                ZIndex = 0
            };
            mapControl.MapElements.Add(mapIcon);
        }

        private void AddIncidentIcon(Geopoint position, string title, string appNotificationId)
        {
            MapIcon mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/48x48ChemicalIncidentIcon.png")),
                ZIndex = 0,
                Tag = appNotificationId
            };
            mapControl.MapElements.Add(mapIcon);
        }

        // event handler for the FlytoButton.  
        private async void FlytoButton_Click(object sender, RoutedEventArgs e)
        {
            FlytoButton.IsEnabled = false;

            if (null != SelectedIncident)
            {
                await SetMapScene(SelectedIncident);
            }
        }

        // tells the map to fly to the target from either the  Flyto button
        private async Task SetMapScene(NotificationListItem atkObj)
        {
            if (null != atkObj && mapControl.Is3DSupported)
            {
                mapControl.Style = MapStyle.Aerial3DWithRoads;

                Geopoint detonationPoint = new Geopoint(new BasicGeoposition() { Latitude = atkObj.Details.Lat, Longitude = atkObj.Details.Long });

                MapScene detonationScene = MapScene.CreateFromLocationAndRadius(detonationPoint,
                                                                                    500, /* show this many meters around */
                                                                                    0, /* looking at it north */
                                                                                    70 /* degrees pitch */);
                await mapControl.TrySetSceneAsync(detonationScene);
            }
            else
            {
                //string status = "3D views are not supported on this device.";
                //ViewModel.NotifyUser(status, NotifyType.ErrorMessage);
            }
        }

        // event handler for the TargetDeleteButton
        private void TargetDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIncident != null)
            {
                string attackID = SelectedIncident.AppNotificationId.ToString();

                // finally, the incident object itself
                MainPage.MappedIncidents.Remove(attackID);

                MapElement theElement = null;

                foreach (var item in mapControl.MapElements)
                {
                    string tag = item.Tag as string;

                    if (tag == attackID)
                    {
                        theElement = item;
                        break;
                    }
                }

                if (null != theElement)
                {
                    mapControl.MapElements.Remove(theElement);
                }

                // turn off the delete button, because the user has to select
                // another and enable delete to delete another
                DeleteEnableSwitch.IsOn = false;
                DeleteEnableSwitch.IsEnabled = false;

            }
        }

        private NotificationListItem _selectedIncident;
        public NotificationListItem SelectedIncident
        {
            get { return _selectedIncident; }
            set
            {
                Set(ref _selectedIncident, value);
            }
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
