using ActivityLogger.Tracing;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Data.SQLite;

namespace ActivityLogger.Datalayer
{
    public class ActivityDataStore
    {

        protected static Dictionary<String, ActivityDataStore> gInstances = new Dictionary<string, ActivityDataStore>(1);
        protected static Object gInstancesLock = new Object();
        protected const string cCreateActivityTableQry = "CREATE TABLE Activity(timestamp INTEGER, description STRING)";
        protected const string cInsertActivityQry = "INSERT INTO Activity (timestamp, description) VALUES(@timestamp, @description)";
        protected const string cSelectDayActivity = "SELECT timestamp, description FROM Activity WHERE timestamp >= @beginTime AND timestamp <= @endTime ORDER BY timestamp";

        protected string mFilepath;
        protected Object writeLock = new Object();

        #region Construction

        private ActivityDataStore(string filepath)
        {
            ALT.TraceStartConstructor("ActivityDataStore");

            this.mFilepath = filepath;

            this.EnsureDataFile();

            ALT.TraceStopConstructor("ActivityDataStore");
        }

        public static ActivityDataStore CreateInstance(string filepath)
        {
            ALT.TraceStart("ActivityDataStore", "CreateInstance");

            ActivityDataStore instance;

            // Wait if someone else is asking for an instance also, just to be thread save. Once they are done and have their instance
            // then we can continue and possibly also receive the same instance.
            lock (gInstancesLock)
            {
                if (!ActivityDataStore.gInstances.ContainsKey(filepath))
                {
                    ActivityDataStore.gInstances.Add(filepath, new ActivityDataStore(filepath));
                }
                instance = ActivityDataStore.gInstances[filepath];
            }

            instance.EnsureDataFile();

            ALT.TraceStop("ActivityDataStore", "CreateInstance");

            return instance;
        }

        protected void EnsureDataFile()
        {
            ALT.TraceStart("ActivityDataStore", "EnsureDataFile");
            
            // 1) Check if the file exists, if it does, try to open it and verify table structure (or just assume it's ok)
            // 2) If the file doesn't exist, create it and create the table structures.
            if (!File.Exists(this.mFilepath))
            {
                CreateDataFile(this.mFilepath);
            }

            ALT.TraceStop("ActivityDataStore", "EnsureDataFile");
        }

        protected void CreateDataFile(string filepath)
        {
            ALT.TraceStart("ActivityDataStore", "CreateDataFile");

            lock (this.writeLock)
            {
                SQLiteConnection conn = CreateConnection(filepath, false);

                try
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(cCreateActivityTableQry, conn);
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }

            ALT.TraceStop("ActivityDataStore", "CreateDataFile");
        }

        protected SQLiteConnection CreateConnection(string filepath, Boolean readonlyConnection)
        {
            return new SQLiteConnection(String.Format("Data Source = {0}; Version = 3;Read Only = {1}", filepath, (readonlyConnection ? "True" : "False")));
        }

        #endregion

        #region Input

        public void AddActivity(Activity activity)
        {
            ALT.TraceStart("ActivityDataStore", "AddActivity");

            DateTime timestamp = activity.Timestamp.ToUniversalTime();

            lock (this.writeLock)
            {
                SQLiteConnection conn = CreateConnection(this.mFilepath, false);

                try
                {
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(cInsertActivityQry, conn);

                    cmd.Parameters.Add(new SQLiteParameter("@timestamp", timestamp.Ticks));
                    cmd.Parameters.Add(new SQLiteParameter("@description", activity.Description));

                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }

            ALT.TraceStop("ActivityDataStore", "AddActivity");
        }

        public ActivityCollection GetActivities(int year, int month, int day)
        {
            // cSelectDayActivity
            ActivityCollection result = new ActivityCollection();

            ALT.TraceStart("ActivityDataStore", "GetActivities");

            // We get input in local time, but we need to query in UTC time ...
            DateTime universalBeginDate = new DateTime(year, month, day).ToUniversalTime();
            DateTime universalEndDate = new DateTime(year, month, day, 23, 59, 59).ToUniversalTime(); 

            // Do we want a read/write locking mechanism? Now we just lock ...
            lock (this.writeLock)
            {
                SQLiteConnection conn = CreateConnection(this.mFilepath, true);

                try
                {
                    conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand(cSelectDayActivity, conn);

                    cmd.Parameters.Add(new SQLiteParameter("@beginTime", universalBeginDate.Ticks));
                    cmd.Parameters.Add(new SQLiteParameter("@endTime", universalEndDate.Ticks));

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            long ticks = long.Parse(dr["timestamp"].ToString());
                            DateTime timestamp = new DateTime(ticks);
                            Activity act = new Activity(dr["description"].ToString(), timestamp.ToLocalTime());
                            result.Add(act);
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            ALT.TraceStop("ActivityDataStore", "GetActivities");

            return result;
        }

        #endregion

    }
}
