using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto.service
{
    public class Message
    {
        public List<dto.service.MessageDetail> List { get; set; }

        public void Clear()
        {
            List = new List<MessageDetail>();
        }
        public void Add(string message, bool isError = false)
        {

        }
        public void AddError(string message)
        {
            Add(message, true);
        }
        public string Get(string separator = "")
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in List.Select((value, index) => new dto.ForEach<dto.service.MessageDetail>() { Index = index, Value = value }).ToList())
            {
                if (item.Index != 0) { sb.Append(separator); }
                sb.Append(item.Value.Description);
            }

            return sb.ToString();
        }
    }
}
