using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Services.Scheduling;

namespace FortyFingers.FilecuumCleaner.Library
{
    public static class SchedulerHelper
    {
        public static ScheduleItem GetScheduleItem(string fullTypeName)
        {
            ScheduleItem retval = null;

            var provider = SchedulingProvider.Instance();

            var list = new List<ScheduleItem>();
            list.AddRange(provider.GetSchedule().OfType<ScheduleItem>());

            if (list.Any(s => s.TypeFullName == fullTypeName))
                retval = list.First(s => s.TypeFullName == fullTypeName);

            return retval;
        }

        public static void EnableScheduleItem(string fullTypeName)
        {
            var provider = SchedulingProvider.Instance();
            var item = GetScheduleItem(fullTypeName);

            if (item == null)
            {
                item = new ScheduleItem();
                item.CatchUpEnabled = false;
                item.FriendlyName = "40Fingers FilecuumCleaner";
                item.RetainHistoryNum = 50;
                item.RetryTimeLapse = 1;
                item.RetryTimeLapseMeasurement = "h";
                item.TimeLapse = 1;
                item.TimeLapseMeasurement = "d";
                item.TypeFullName = fullTypeName;

                item.ScheduleID = provider.AddSchedule(item);
            }

            item.Enabled = true;
            provider.UpdateSchedule(item);
        }

        public static void DisableScheduleItem(string fullTypeName)
        {
            var provider = SchedulingProvider.Instance();
            var item = GetScheduleItem(fullTypeName);

            if (item != null)
            {
                item.Enabled = false;
                provider.UpdateSchedule(item);
            }
        }
    }
}