using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ActivityLogger.Datalayer;

namespace ActivityLogger.Sandbox
{
    class Program
    {
        static Random r = new Random();

        static void Main(string[] args)
        {
            Regex reg = new Regex("^[0-9]{1,2}:[0-9]{2} ?([PpAa][Mm])?( ){1}");
            Match m = reg.Match("09:00 PM L Blabla");
            if (m.Success)
            {
                Console.WriteLine(m.Value);
                DateTime time = DateTime.Now;
                if (DateTime.TryParse(m.Value, out time))
                {
                    Console.WriteLine("DateTime: " + time);
                }
            }
            else
            {
                Console.WriteLine("No match!");
            }

            Console.ReadLine();

           // ActivityDataStore ds = ActivityDataStore.CreateInstance("test.dat");
           // //ds.AddActivity(new Activity("First Activity, at midnight 00:00 of oct 1", new DateTime(2011, 10, 1, 0, 0, 0)));
           // //ds.AddActivity(new Activity("An Activity, at 12:00 of oct 1", new DateTime(2011, 10, 1, 12, 0, 0)));
           // //ds.AddActivity(new Activity("Last Activity, at 23:59 of oct 1", new DateTime(2011, 10, 1, 23, 59, 59)));

           // Console.WriteLine("Activities yesterday: ");
           // ActivityCollection activities = ds.GetActivities(2011, 9, 30);
           // foreach (Activity activity in activities)
           // {
           //     Console.WriteLine("{0} - {1}", activity.Timestamp.ToString(), activity.Description);
           // }

           // Console.WriteLine("Activities today: ");
           // activities = ds.GetActivities(2011, 10, 1);
           // foreach (Activity activity in activities)
           // {
           //     Console.WriteLine("{0} - {1}", activity.Timestamp.ToString(), activity.Description);
           // }

           //// Console.ReadLine();

           // //int repeat = 100;

           // //for (int i = 0; i < repeat; i++)
           // //{
           // //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(LogActivity), i);
           // //}

           // Console.ReadLine();
        }

        static void LogActivity(Object activityObj)
        {
            int activity = (int)activityObj;
            int delay = r.Next(10, 100);

            System.Threading.Thread.Sleep(delay);

            ActivityDataStore ds = ActivityDataStore.CreateInstance("test.dat");
            ds.AddActivity(new Activity("Activity: " + activity));
        }

        static void ReadActivities()
        {
            int delay = r.Next(10, 100);

            System.Threading.Thread.Sleep(delay);

            ActivityDataStore ds = ActivityDataStore.CreateInstance("test.dat");
            ActivityCollection activities = ds.GetActivities(2011, 9, 29);
            foreach (Activity activity in activities)
            {
                Console.WriteLine("{0} - {1}", activity.Timestamp.ToString(), activity.Description);
            }
        }

    }
}
