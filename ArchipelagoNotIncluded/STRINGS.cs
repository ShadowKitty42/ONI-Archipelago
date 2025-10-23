using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoNotIncluded
{
    internal class STRINGS
    {
        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class  ARCHIPELAGOCONNECTED
                {
                    public static LocString NAME = "Connected to Archipelago";
                    public static LocString TOOLTIP = $"Connected to Archipelago server.";
                }
                public class ARCHIPELAGODISCONNECTED
                {
                    public static LocString NAME = "Not Connected to Archipelago";
                    public static LocString TOOLTIP = "Archipelago connection has been lost. Click the Notification to reconnect.";
                    public static LocString NOTIFICATION_NAME = "Archipelago Disconnected";
                    public static LocString NOTIFICATION_TOOLTIP = "Click to reconnect.";
                }
            }
        }

        public class UI
        {
            public class GOALS
            {
                public static LocString TITLE = "\nGoal: {0}";
                public static LocString LAUNCH_ROCKET = "Launch your first rocket";
                public static LocString MONUMENT = "Build a monument";
                public static LocString RESEARCH_ALL = "Complete the Research Tree";
                public static LocString HOME_SWEET_HOME = "Complete the Home Sweet Home directive";
                public static LocString GREAT_ESCAPE = "Complete the Great Escape directive";
                public static LocString COSMIC_ARCHAEOLOGY = "Complete the Cosmic Archaeology directive";
                public static LocString UNKNOWN = "Unknown Goal";
            }
        }
    }
}
