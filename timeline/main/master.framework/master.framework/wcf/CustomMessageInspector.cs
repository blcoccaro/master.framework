using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.wcf
{
    /// <summary>
    /// This class is used to inspect the message and headers on the server side,
    /// This class is also used to intercept the message on the
    /// client side, before/after any request is made to the server.
    /// </summary>
    public class CustomMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        #region Message Inspector of the Service

        /// <summary>
        /// This method is called on the server when a request is received from the client.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request,
               IClientChannel channel, InstanceContext instanceContext)
        {
            // Create a copy of the original message so that we can mess with it.
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();
            Message messageCopy = buffer.CreateMessage();

            // Read the custom context data from the headers
            ServiceHeader customData = CustomHeader.ReadHeader(request);

            // Add an extension to the current operation context so
            // that our custom context can be easily accessed anywhere.
            ServerContext customContext = new ServerContext();

            if (customData != null)
            {
                customContext.System = customData.System;
                customContext.Token = customData.Token;
                customContext.IdUser = customData.IdUser;
                customContext.UserLogin = customData.UserLogin;
                customContext.WebSessionId = customData.WebSessionId;
                customContext.ClientIP = customData.ClientIP;
                customContext.ClientDate = customData.ClientDate;
                customContext.ClientVersion = customData.ClientVersion;
                
            }
            OperationContext.Current.IncomingMessageProperties.Add(
                     "CurrentContext", customContext);
            return null;
        }

        /// <summary>
        /// This method is called after processing a method on the server side and just
        /// before sending the response to the client.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // Do some cleanup
            if(ServerContext.Current != null)
                OperationContext.Current.Extensions.Remove(ServerContext.Current);
        }

        #endregion

        #region Message Inspector of the Consumer

        /// <summary>
        /// This method will be called from the client side just before any method is called.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // Prepare the request message copy to be modified
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();

            ServiceHeader customData = new ServiceHeader();

            customData.System = ClientContext.System;
            customData.Token = ClientContext.Token;
            customData.IdUser = ClientContext.IdUser;
            customData.UserLogin = ClientContext.UserLogin;
            customData.WebSessionId = ClientContext.WebSessionId;
            customData.ClientIP = ClientContext.ClientIP;
            customData.ClientDate = ClientContext.ClientDate;
            customData.ClientVersion = ClientContext.ClientVersion;
            

            CustomHeader header = new CustomHeader(customData);

            // Add the custom header to the request.
            request.Headers.Add(header);

            return null;
        }

        /// <summary>
        /// This method will be called after completion of a request to the server.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }

        #endregion
    }
}
