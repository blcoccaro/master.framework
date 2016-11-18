using System;
using System.Collections.Generic;

namespace master.framework.interfaces
{
    public interface IServiceBase
    {
        string SharedUUID { get; set; }
        string ServiceID { get; set; }
        bool isDebugMode { get; set; }
        dto.service.Message Messages { get; set; }
        void Beacon(string serviceId = null, string message = null);
    }
}
