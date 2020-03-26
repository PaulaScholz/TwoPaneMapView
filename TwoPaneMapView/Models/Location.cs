using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// for INotifyPropertyChanged
using System.ComponentModel;

namespace TwoPaneMapView
{
	/// <summary>
	/// A List of Location objects.
	/// </summary>
	[CollectionDataContract(Name = "LocationCollection", ItemName="Location")]
	public class LocationCollection : List<Location>
	{
	}

    /// <summary>
    /// This is a substitute for the Bing.Maps Windows 8 API Location object, because the original code was built with that.
    /// </summary>
    [DataContract(Name = "Location")]
    public class Location : INotifyPropertyChanged
    {
        private double _latitude;
        private double _longitude;

        //
        // Summary:
        //     Initializes a new instance of the Bing.Maps.Location class with latitude and
        //     longitude values set to 0,0.
        public Location()
        {
            Init();
        }

        private void Init()
        {
            Latitude = 0.0;
            Longitude = 0.0;
        }

        //
        // Summary:
        //     Creates a copy of the specified Bing.Maps.Location.
        public Location(Location other)
        {
            if (null != other)
            {
                Latitude = other.Latitude;
                Longitude = other.Longitude;
            }
            else
            {
                Init();
            }
        }
        //
        // Summary:
        //     Initializes a new instance of the Bing.Maps.Location class using the specified
        //     latitude and longitude values.
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        //
        // Summary:
        //     Gets or sets the latitude of the location as degrees of latitude.
        [DataMember(Name = "Latitude")]
        public double Latitude
        {
            get { return _latitude; }

            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }

        }
        //
        // Summary:
        //     Gets or sets the longitude of the location as degrees of longitude.
        [DataMember(Name = "Longitude")]
        public double Longitude
        {
            get { return _longitude; }

            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
