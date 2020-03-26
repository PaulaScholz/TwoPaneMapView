using System;
using System.Collections.Generic;
using System.Text;

namespace TwoPaneMapView
{
    public class NotificationListItem
    {
        public string Id { get; set; }
        public string AppNotificationId { get; set; }
        public string Content { get; set; }
        public bool UnreadState { get; set; }
        public string UserActionState { get; set; }
        public string Priority { get; set; }
        public string ChangeTime { get; set; }
        public IncidentDetail Details { get; set; }
    }
}
