using System;
using System.Collections.Generic;
using System.Text;

namespace TwoPaneMapView
{
    public class IncidentDetail
    {
        public string Id { get; set; }

        public string IncidentHeader { get; set; }

        public string IncidentDescription { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string DateTimeOffset { get; set; }

        public string StatusCode { get; set; }

        public double Lat
        {
            get
            {
                double result = 0.0;

                if (Double.TryParse(Latitude, out result))
                {
                    return result;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double Long
        {
            get
            {
                double result = 0.0;

                if (Double.TryParse(Longitude, out result))
                {
                    return result;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public string IncidentHeaderShort
        {
            get
            {
                int dashIndex = IncidentHeader.IndexOf('-');

                return IncidentHeader.Substring(dashIndex + 1);
            }
        }
    }
}
