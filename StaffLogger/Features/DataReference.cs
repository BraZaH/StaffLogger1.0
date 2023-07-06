using System;

namespace StaffLogger.Features
{
    public class DataReference
    {
        public DataReference(string userId, int time, DateTime date)
        {
            UserID = userId;
            Time = time;
            Date = date;
        }
        public string UserID { get; set; }
        public int Time { get; set; }
        public DateTime Date { get; set; }
    }
}
