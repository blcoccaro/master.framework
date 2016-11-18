using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class TimestampResult
    {
        
        public string Key { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public List<DateTime> Middle { get; set; }

        public override string ToString()
        {
            return string.Format("Key: {0} executed in {1}", Key, Seconds);
        }

        public string Milliseconds
        {
            get
            {
                if (End.HasValue)
                {
                    TimeSpan span = (End.Value - Start);

                    return string.Format("{0} milliseconds", span.TotalMilliseconds); 
                }
                else
                {
                    return "not ended.";
                }
            }
            set { }
        }
        public string Seconds
        {
            get
            {
                if (End.HasValue)
                {
                    TimeSpan span = (End.Value - Start);

                    return string.Format("{0} seconds", span.TotalSeconds);
                }
                else
                {
                    return "not ended.";
                }
            }
            set { }
        }
        public string Minutes
        {
            get
            {
                if (End.HasValue)
                {
                    TimeSpan span = (End.Value - Start);

                    return string.Format("{0} minutes", span.TotalMinutes);
                }
                else
                {
                    return "not ended.";
                }
            }
            set { }
        }
        public string Hours
        {
            get
            {
                if (End.HasValue)
                {
                    TimeSpan span = (End.Value - Start);

                    return string.Format("{0} hours", span.TotalHours);
                }
                else
                {
                    return "not ended.";
                }
            }
            set { }
        }
        public string Days
        {
            get
            {
                if (End.HasValue)
                {
                    TimeSpan span = (End.Value - Start);

                    return string.Format("{0} days", span.TotalDays);
                }
                else
                {
                    return "not ended.";
                }
            }
            set { }
        }
    }
}
