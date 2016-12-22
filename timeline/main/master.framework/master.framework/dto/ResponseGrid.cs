using System.Collections.Generic;

namespace master.framework.dto
{
    public class ResponseGrid<T> : ResponseBase where T : class
    {
        public TimeStampKeeper Timestamp { get; set; } = new TimeStampKeeper();
        public int TotalRows { get; set; }
        public List<T> List { get; set; }
    }
}
