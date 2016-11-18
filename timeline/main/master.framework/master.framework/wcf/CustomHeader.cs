using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace master.framework.wcf
{
    [DataContract]
    public class ServiceHeader
    {
        [DataMember]
        public string UserLogin { get; set; }
        [DataMember]
        public string IdUser { get; set; }
        [DataMember]
        public string System { get; set; }
        [DataMember]
        public DateTime? ClientDate { get; set; }
        [DataMember]
        public string WebSessionId { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string ClientIP { get; set; }
        [DataMember]
        public string ClientVersion { get; set; }
    }

    public class CustomHeader : MessageHeader
    {
        private static string CUSTOM_HEADER_NAME = master.framework.Configuration.MasterFramework.WCF.CustomHeaderName;
        private static string CUSTOM_HEADER_NAMESPACE = master.framework.Configuration.MasterFramework.WCF.CustomHeaderNamespace;

        private ServiceHeader _customData;

        public ServiceHeader CustomData
        {
            get
            {
                return _customData;
            }
        }

        public CustomHeader()
        {
        }

        public CustomHeader(ServiceHeader customData)
        {
            _customData = customData;
        }

        public override string Name
        {
            get { return (CUSTOM_HEADER_NAME); }
        }

        public override string Namespace
        {
            get { return (CUSTOM_HEADER_NAMESPACE); }
        }

        protected override void OnWriteHeaderContents(
            System.Xml.XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ServiceHeader));
            StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, _customData);
            textWriter.Close();

            string text = textWriter.ToString();

            writer.WriteElementString(CUSTOM_HEADER_NAME, "Key", text.Trim());
        }

        public static ServiceHeader ReadHeader(Message request)
        {
            Int32 headerPosition = request.Headers.FindHeader(CUSTOM_HEADER_NAME, CUSTOM_HEADER_NAMESPACE);
            if (headerPosition == -1)
                return null;

            MessageHeaderInfo headerInfo = request.Headers[headerPosition];

            XmlNode[] content = request.Headers.GetHeader<XmlNode[]>(headerPosition);

            string text = content[0].InnerText;

            XmlSerializer deserializer = new XmlSerializer(typeof(ServiceHeader));
            TextReader textReader = new StringReader(text);
            ServiceHeader customData = (ServiceHeader)deserializer.Deserialize(textReader);
            textReader.Close();

            return customData;
        }
    }
}
