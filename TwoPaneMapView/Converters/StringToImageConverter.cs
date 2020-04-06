using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace TwoPaneMapView.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string incidentType = (string)value;

            // Security Incident default in case the incident type is not defined
            BitmapImage retImage = new BitmapImage(new Uri("ms-appx:///Assets/SecurityIncidentIcon_16x16.png"));

            if ("Fire" == incidentType)
            {
                retImage = new BitmapImage(new Uri("ms-appx:///Assets/FireIncidentIcon_16x16.png"));
            }
            else if ("Medical" == incidentType)
            {
                retImage = new BitmapImage(new Uri("ms-appx:///Assets/MedicalIncidentIcon_16x16.png"));
            }
            else if ("Police" == incidentType)
            {
                retImage = new BitmapImage(new Uri("ms-appx:///Assets/PoliceIncidentIcon_16x16.png"));
            }

            return retImage;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
