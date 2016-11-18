using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using master.framework;

namespace master.framework.dto
{
    public class HistoricalEvent
    {
        public Enumerators.EntityStatus Status { get; set; }
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string SerializedEntity { get; set; }

        public static dto.HistoricalEvent InstanceDelete(object obj, string entityName, string keyName, string keyValue)
        {
            var ret = new dto.HistoricalEvent() { Status = Enumerators.EntityStatus.Delete };
            ret.EntityName = entityName;
            ret.KeyName = keyName;
            ret.KeyValue = keyValue;
            ret.SerializedEntity = obj.Serialize();
            return ret;
        }
        public static dto.HistoricalEvent InstanceAdd(object obj, string entityName, string keyName, string keyValue)
        {
            var ret = new dto.HistoricalEvent() { Status = Enumerators.EntityStatus.Add };
            ret.EntityName = entityName;
            ret.KeyName = keyName;
            ret.KeyValue = keyValue;
            ret.SerializedEntity = obj.Serialize();
            return ret;
        }
        public static dto.HistoricalEvent InstanceChanged(object obj, string entityName, string keyName, string keyValue, string propertyName, string oldValue, string newValue)
        {
            var ret = new dto.HistoricalEvent() { Status = Enumerators.EntityStatus.Changed };
            ret.EntityName = entityName;
            ret.KeyName = keyName;
            ret.KeyValue = keyValue;
            ret.SerializedEntity = obj.Serialize();
            ret.PropertyName = propertyName;
            ret.OldValue = oldValue;
            ret.NewValue = newValue;
            return ret;
        }
    }
}
