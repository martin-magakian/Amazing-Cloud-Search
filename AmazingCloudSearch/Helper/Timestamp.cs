using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazingCloudSearch.Helper
{
    public class Timestamp
    {
        public static int CurrentTimeStamp()
        {
            return DateToTimeStamp(DateTime.Now);
        }

        public static int DateToTimeStamp(DateTime date)
        {
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000; //Convert windows ticks to seconds
            return (int) ticks;
        } 
    }
}
