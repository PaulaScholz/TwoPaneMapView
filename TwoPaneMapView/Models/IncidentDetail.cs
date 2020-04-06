using System;
using System.Collections.Generic;
using System.Text;

namespace TwoPaneMapView
{
    public class IncidentDetail
    {
        /// <summary>
        /// This is a GUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Title of the Incident
        /// </summary>
        public string IncidentHeader { get; set; }

        /// <summary>
        /// What happened in words.
        /// </summary>
        public string IncidentDescription { get; set; }

        /// <summary>
        /// String describing type of incident, i.e. Security, Medical, Police, Fired
        /// </summary>
        public string IncidentType { get; set; }

        /// <summary>
        /// Incident Location latitude
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Incident Location longitude
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Date/Time of indicent creation
        /// </summary>
        public string DateTimeOffset { get; set; }

        /// <summary>
        /// String describing status of the incident
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// True means incident location currently plotted on map
        /// </summary>
        public bool IsPlotted { get; set; }

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
