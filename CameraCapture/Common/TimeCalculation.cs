using System;
using System.Collections.Generic;
using System.Linq;
using CameraCapture.Modules;

namespace CameraCapture.Common
{
    public static class TimeCalculation
    {
        public static List<(string name, TimeSpan absentTime)> CalculateSpan(List<DetectorResult> data)
        {
            var personList = data.Select(x => x.Name).Distinct();
            var personSpanList = new List<(string name, TimeSpan absentTime)>();

            foreach (var personName in personList)
            {
                var personSpan = new TimeSpan();
                var personActivityList = data.Where(z => z.Name == personName).OrderBy(x => x.Time.Value).ToList();
                foreach (var personActivityExit in personActivityList.Where(z => z.Zone == ZoneModuleEnum.ExitZone))
                {
                    var time1 = personActivityExit.Time.Value;
                    var time2 = personActivityList.Where(z => z.Time.Value >= time1 && z.Zone == ZoneModuleEnum.EnterZone).Select(x => x.Time.Value).FirstOrDefault();
                    if (time2 == default(DateTime)) time2 = DateTime.Now;
                    var span = time2.Subtract(time1);
                    personSpan = personSpan.Add(span);
                }
                personSpanList.Add((personName, personSpan));
            }

            return personSpanList;
        }
    }
}