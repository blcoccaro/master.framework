using System.Collections.Generic;
using System.Linq;

namespace master.framework.dto
{
    public class DropDownListResult : interfaces.IDropDownListResult
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public int Order { get; set; }
        public bool Selected { get; set; }
        public object Object { get; set; }

        public static List<DropDownListResult> GetList(List<DropDownListResult> result, bool keepOrder = false, string idfirst = null, string textfirst = null)
        {
            List<DropDownListResult> ret = new List<DropDownListResult>();
            if (!string.IsNullOrWhiteSpace(idfirst) || !string.IsNullOrWhiteSpace(textfirst))
            {
                ret.Add(new DropDownListResult()
                {
                    Value = idfirst,
                    DisplayText = textfirst,
                    Order = -1,
                });
            }
            ret.AddRange(result);
            if (!keepOrder) { ret = ret.OrderBy(o => o.Order).ThenBy(o => o.DisplayText).ToList(); }
            return ret;
        }
    }
}
