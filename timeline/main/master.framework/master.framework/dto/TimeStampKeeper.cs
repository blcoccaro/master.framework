using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class TimeStampKeeper
    {
        public List<TimeStamp> Times { get; set; } = new List<TimeStamp>();

        public void ClearCustom()
        {
            if (Times.Count > 0 && Times.Any(o => o.Type == Enumerators.TimeStampType.Custom))
            {
                Times.RemoveAll(o => o.Type == Enumerators.TimeStampType.Custom);
            }
        }
        public void ClearNative()
        {
            if (Times.Count > 0 && Times.Any(o => o.Type == Enumerators.TimeStampType.Native))
            {
                Times.RemoveAll(o => o.Type == Enumerators.TimeStampType.Native);
            }
        }
        public void ClearAll()
        {
            Times = new List<TimeStamp>();
        }

        private void AddStartInternal(string key, DateTime date, Enumerators.TimeStampType type)
        {
            Times.Add(new TimeStamp()
            {
                Key = key,
                Date = date,
                Action = Enumerators.TimeStampAction.Start,
                Type = type,
            });
        }
        private void AddEndInternal(string key, DateTime date, Enumerators.TimeStampType type)
        {
            Times.Add(new TimeStamp()
            {
                Key = key,
                Date = date,
                Action = Enumerators.TimeStampAction.End,
                Type = type,
            });
        }
        internal void AddStartNative(string key, DateTime date)
        {
            AddStartInternal(key, date, Enumerators.TimeStampType.Native);
        }

        public void AddStartCustom(string key, DateTime date)
        {
            AddStartInternal(key, date, Enumerators.TimeStampType.Custom);
        }
        internal void AddEndNative(string key, DateTime date)
        {
            AddEndInternal(key, date, Enumerators.TimeStampType.Native);
        }

        public void AddEndCustom(string key, DateTime date)
        {
            AddEndInternal(key, date, Enumerators.TimeStampType.Custom);
        }

        public List<TimestampResult> GetResult()
        {
            List<TimestampResult> ret = new List<TimestampResult>();
            foreach (var item in Times.Where(o => o.Action == Enumerators.TimeStampAction.Start))
            {
                var result = new TimestampResult();
                result.Key = item.Key;
                result.Start = item.Date;

                var end = Times.FirstOrDefault(o => o.Action == Enumerators.TimeStampAction.End && o.Key == item.Key);
                result.End = end == null ? null : (DateTime?)end.Date;
                ret.Add(result);
            }
            return ret;
        }
    }
}
