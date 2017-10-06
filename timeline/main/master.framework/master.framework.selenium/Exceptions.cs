using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.selenium
{
    [Serializable]
    public class ElementTimeoutException : Exception
    {
        public ElementTimeoutException() { }
        public ElementTimeoutException(string message) : base(message) { }
        public ElementTimeoutException(string message, Exception inner) : base(message, inner) { }
        protected ElementTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class NotAnswerException : Exception
    {
        public NotAnswerException() { }
        public NotAnswerException(string message) : base(message) { }
        public NotAnswerException(string message, Exception inner) : base(message, inner) { }
        protected NotAnswerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
