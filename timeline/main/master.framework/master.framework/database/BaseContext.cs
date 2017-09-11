using master.framework.attribute.database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace master.framework.database
{
    public class BaseContext : DbContext
    {
        public static string GetConnectionString(string connectionStringName)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionStringName);
            builder.MultipleActiveResultSets = true;
            return builder.ConnectionString.ToString();

        }
        private System.Data.Entity.DbContextTransaction transaction;
        public Dictionary<string, string> DicHelper { get; set; } = new Dictionary<string, string>();
        public Enumerators.Database DatabaseType { get; set; }

        #region Delegates
        public delegate void LogSaveHandler(string log);
        public delegate void HistoricalHandler(BaseContext source, dto.HistoricalEvent entity);
        public delegate string GetCurrentUserIdHandler();
        public delegate string GetCurrentSystemIdHandler();
        public delegate string GetCurrentClientIPHandler();
        #endregion

        #region Events
        public event LogSaveHandler LogSave;
        public event HistoricalHandler HistoricalCreated;
        public event HistoricalHandler HistoricalModified;
        public event HistoricalHandler HistoricalDeleted;
        public event GetCurrentUserIdHandler GetCurrentUserId;
        public event GetCurrentSystemIdHandler GetCurrentSystemId;
        public event GetCurrentClientIPHandler GetCurrentClientIP;
        #endregion

        #region Constructors
        public BaseContext()
            : base(GetConnectionString("cnDefault"))
        {
            Initialize();
        }
        public BaseContext(string cnName)
            : base(GetConnectionString(cnName))
        {
            Initialize();
        }
        #endregion

        #region Privates
        private void SetDatabaseType()
        {
            Enumerators.Database ret = Enumerators.Database.Unknow;
            try
            {
                var providerName = ((((System.Data.Entity.DbContext)this))).Database.Connection.GetType().Name.ToLower();
                this.logDebug(message: string.Format("Provider Name: {0}", providerName));

                switch (providerName)
                {
                    case "pgsqlconnection":
                        ret = Enumerators.Database.PostgreSQL;
                        break;
                    case "db2connection":
                        ret = Enumerators.Database.DB2;
                        break;
                    case "sqlconnection":
                        ret = Enumerators.Database.SQLServer;
                        break;
                    case "oracleconnection":
                        ret = Enumerators.Database.Oracle;
                        break;
                    case "mysqlconnection":
                        ret = Enumerators.Database.MySQL;
                        break;
                    default:
                        ret = Enumerators.Database.Unknow;
                        break;
                }

            }
            catch (System.Exception ex)
            {
                this.logError(ex: ex);
                throw;
            }
            DatabaseType = ret;
        }

        private void Initialize()
        {
            //System.Data.Entity.Database.SetInitializer<BaseContext>(null);
            SetDatabaseType();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext.Connection.Open();
            //transaction = this.Database.Connection.BeginTransaction(System.Data.IsolationLevel.Snapshot);
            if (LogSave != null) { this.Database.Log = l => LogSave(l); }
        }
        #endregion

        #region
        public void OpenTransaction()
        {
            transaction = this.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }
        public void Commit()
        {
            transaction?.Commit();
        }
        public void RollBack()
        {
            transaction?.Rollback();
        }
        #endregion

        public void ClearContext()
        {
            foreach (System.Data.Entity.Infrastructure.DbEntityEntry dbEntityEntry in this.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        #region SaveChanges - History
        public override int SaveChanges()
        {
            Dictionary<Guid, System.Data.Entity.Core.Objects.ObjectStateEntry> added = new Dictionary<Guid, System.Data.Entity.Core.Objects.ObjectStateEntry>();

            string userid = null;
            string systemid = null;
            string clientIP = null;

            if (GetCurrentUserId != null) { userid = GetCurrentUserId(); }
            if (GetCurrentSystemId != null) { systemid = GetCurrentSystemId(); }
            if (GetCurrentClientIP != null) { clientIP = GetCurrentClientIP(); }


            ChangeTracker.DetectChanges();

            System.Data.Entity.Core.Objects.ObjectContext ctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext;

            List<System.Data.Entity.Core.Objects.ObjectStateEntry> objectStateEntryList =
                ctx.ObjectStateManager.GetObjectStateEntries(System.Data.Entity.EntityState.Added
                                                           | System.Data.Entity.EntityState.Modified
                                                           | System.Data.Entity.EntityState.Deleted)
                .ToList();

            this.logDebug(string.Format("Founded {0} Entity to Save.", objectStateEntryList.Count));

            var doHistorical = true;

            if (master.framework.Configuration.MasterFramework.Database.Historical.isOFF) { doHistorical = false; }

            if (doHistorical)
            {
                foreach (var item in objectStateEntryList.Select((value, index) => new dto.ForEach<System.Data.Entity.Core.Objects.ObjectStateEntry>() { Index = index, Value = value }).ToList())
                {
                    var tableName = item.Value.EntitySet.Name;
                    this.logDebug(string.Format("Checking {0} - {1}", item.Index, tableName));

                    if (!item.Value.IsRelationship)
                    {
                        var withoutHistorical = item.Value.Entity.GetType().GetCustomAttributes(typeof(WithoutHistorical), true).Length > 0;
                        var withHistorical = item.Value.Entity.GetType().GetCustomAttributes(typeof(WithHistorical), true).Length > 0;

                        if (withoutHistorical) { break; }

                        WithHistorical historicalAtt = null;

                        if (withHistorical)
                        {
                            historicalAtt = item.Value.Entity.GetType().GetCustomAttributes(typeof(WithHistorical), true).FirstOrDefault() as WithHistorical;
                        }

                        var properties = item.Value.Entity.GetType().GetProperties();

                        switch (item.Value.State)
                        {
                            case System.Data.Entity.EntityState.Added:
                                #region Added
                                {
                                    #region Key Attribute
                                    {
                                        var propertyId = properties.Where(o => o.GetCustomAttributes(typeof(Key), true).Length > 0).ToList();
                                        if (propertyId?.Count > 0)
                                        {
                                            var property = propertyId.FirstOrDefault();
                                            if (property.PropertyType == typeof(Guid))
                                            {
                                                if ((Guid)property.GetValue(item.Value.Entity) == Guid.Empty)
                                                {
                                                    bool setvalue = false;
                                                    var att = property.GetCustomAttributes(typeof(Key), true).FirstOrDefault() as Key;
                                                    if (att != null) { setvalue = att.AutoGenerated; }
                                                    if (setvalue) { property.SetValue(item.Value.Entity, Guid.NewGuid()); }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Created Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(Created), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                var createdAtt = itemProperty.GetCreatedAtt();
                                                switch (createdAtt?.CreatedType)
                                                {
                                                    case Enumerators.CreatedType.User:
                                                        itemProperty.SetValue(item.Value.Entity, userid);
                                                        break;
                                                    case Enumerators.CreatedType.Date:
                                                        itemProperty.SetValue(item.Value.Entity, DateTime.Now);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region CreatedOrUpdated Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(CreatedOrUpdated), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                var createdAtt = itemProperty.GetCreatedOrUpdatedAtt();
                                                switch (createdAtt?.CreatedType)
                                                {
                                                    case Enumerators.CreatedOrUpdatedType.User:
                                                        {
                                                            System.Type propertyType = itemProperty.GetType();
                                                            System.TypeCode propertyTypeCode = Type.GetTypeCode(propertyType);
                                                            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                            {
                                                                propertyTypeCode = Type.GetTypeCode(itemProperty.PropertyType.GetGenericArguments()[0]);
                                                            }
                                                            switch (propertyTypeCode)
                                                            {
                                                                case TypeCode.Int16:
                                                                case TypeCode.UInt16:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, short.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.Int32:
                                                                case TypeCode.UInt32:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, int.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.Int64:
                                                                case TypeCode.UInt64:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, long.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.String:
                                                                    itemProperty.SetValue(item.Value.Entity, userid);
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case Enumerators.CreatedOrUpdatedType.Date:
                                                        itemProperty.SetValue(item.Value.Entity, DateTime.Now);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region SystemId Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(SystemId), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                itemProperty.SetValue(item.Value.Entity, systemid);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region UserIP Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(UserIP), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                itemProperty.SetValue(item.Value.Entity, clientIP);
                                            }
                                        }
                                    }
                                    #endregion

                                    if (historicalAtt != null)
                                    {
                                        added.Add(Guid.NewGuid(), item.Value);
                                    }
                                }
                                #endregion
                                break;
                            case System.Data.Entity.EntityState.Deleted:
                                #region Deleted
                                {
                                    bool isToInactive = false;

                                    #region Historical
                                    if (HistoricalDeleted != null && historicalAtt != null)
                                    {
                                        var key = item.Value.EntityKey.EntityKeyValues.FirstOrDefault();
                                        string keyName = key?.Key;
                                        string keyValue = key?.Value.ToString();
                                        dto.HistoricalEvent histEvent = dto.HistoricalEvent.InstanceDelete(item.Value.Entity, tableName, keyName, keyValue);
                                        HistoricalDeleted(this, histEvent);
                                    }
                                    #endregion

                                    #region Inactive Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(Inactive), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            item.Value.ChangeState(System.Data.Entity.EntityState.Modified);
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                itemProperty.SetValue(item.Value.Entity, true);
                                            }
                                            isToInactive = true;
                                        }
                                    }
                                    #endregion

                                    if (isToInactive)
                                    {
                                        #region UpdateDate Attribute
                                        {
                                            var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(Updated), true).Length > 0).ToList();
                                            if (propertyRegister?.Count > 0)
                                            {
                                                foreach (var itemProperty in propertyRegister)
                                                {
                                                    var updatedAtt = itemProperty.GetUpdatedAtt();
                                                    switch (updatedAtt?.UpdatedType)
                                                    {
                                                        case Enumerators.UpdatedType.User:
                                                            itemProperty.SetValue(item.Value.Entity, userid);
                                                            break;
                                                        case Enumerators.UpdatedType.Date:
                                                            itemProperty.SetValue(item.Value.Entity, DateTime.Now);
                                                            break;
                                                    }

                                                }
                                            }
                                        }
                                        #endregion

                                        #region SystemId Attribute
                                        {
                                            var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(SystemId), true).Length > 0).ToList();
                                            if (propertyRegister?.Count > 0)
                                            {
                                                foreach (var itemProperty in propertyRegister)
                                                {
                                                    itemProperty.SetValue(item.Value.Entity, systemid);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region UserIP Attribute
                                        {
                                            var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(UserIP), true).Length > 0).ToList();
                                            if (propertyRegister?.Count > 0)
                                            {
                                                foreach (var itemProperty in propertyRegister)
                                                {
                                                    itemProperty.SetValue(item.Value.Entity, clientIP);
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                }
                                #endregion
                                break;
                            case System.Data.Entity.EntityState.Modified:
                                #region Modified
                                {
                                    var propertyUpdate = properties.Where(o => o.GetCustomAttributes(typeof(Updated), true).Length > 0).ToList();
                                    var propertyInactive = properties.Where(o => o.GetCustomAttributes(typeof(Inactive), true).Length > 0).ToList();
                                    var propertyCreated = properties.Where(o => o.GetCustomAttributes(typeof(Created), true).Length > 0).ToList();
                                    var propertySystemId = properties.Where(o => o.GetCustomAttributes(typeof(SystemId), true).Length > 0).ToList();
                                    var propertyUserIP = properties.Where(o => o.GetCustomAttributes(typeof(UserIP), true).Length > 0).ToList();
                                    var propertyCreatedOrUpdated = properties.Where(o => o.GetCustomAttributes(typeof(CreatedOrUpdated), true).Length > 0).ToList();

                                    #region Update Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(Updated), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                var updatedAtt = itemProperty.GetUpdatedAtt();
                                                switch (updatedAtt?.UpdatedType)
                                                {
                                                    case Enumerators.UpdatedType.User:
                                                        itemProperty.SetValue(item.Value.Entity, userid);
                                                        break;
                                                    case Enumerators.UpdatedType.Date:
                                                        itemProperty.SetValue(item.Value.Entity, DateTime.Now);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region CreatedOrUpdated Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(CreatedOrUpdated), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                var createdAtt = itemProperty.GetCreatedOrUpdatedAtt();
                                                switch (createdAtt?.CreatedType)
                                                {
                                                    case Enumerators.CreatedOrUpdatedType.User:
                                                        {
                                                            System.Type propertyType = itemProperty.GetType();
                                                            System.TypeCode propertyTypeCode = Type.GetTypeCode(propertyType);
                                                            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                            {
                                                                propertyTypeCode = Type.GetTypeCode(itemProperty.PropertyType.GetGenericArguments()[0]);
                                                            }
                                                            switch (propertyTypeCode)
                                                            {
                                                                case TypeCode.Int16:
                                                                case TypeCode.UInt16:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, short.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.Int32:
                                                                case TypeCode.UInt32:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, int.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.Int64:
                                                                case TypeCode.UInt64:
                                                                    if (!string.IsNullOrWhiteSpace(userid))
                                                                    {
                                                                        itemProperty.SetValue(item.Value.Entity, long.Parse(userid));
                                                                    }
                                                                    break;
                                                                case TypeCode.String:
                                                                    itemProperty.SetValue(item.Value.Entity, userid);
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                    case Enumerators.CreatedOrUpdatedType.Date:
                                                        itemProperty.SetValue(item.Value.Entity, DateTime.Now);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region SystemId Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(SystemId), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                itemProperty.SetValue(item.Value.Entity, systemid);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region UserIP Attribute
                                    {
                                        var propertyRegister = properties.Where(o => o.GetCustomAttributes(typeof(UserIP), true).Length > 0).ToList();
                                        if (propertyRegister?.Count > 0)
                                        {
                                            foreach (var itemProperty in propertyRegister)
                                            {
                                                itemProperty.SetValue(item.Value.Entity, clientIP);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Historical
                                    if (HistoricalModified != null && historicalAtt != null)
                                    {
                                        var key = item.Value.EntityKey.EntityKeyValues.FirstOrDefault();
                                        string keyName = key?.Key;
                                        string keyValue = key?.Value.ToString();

                                        switch (historicalAtt.HistoricalType)
                                        {
                                            case Enumerators.HistoricalType.ByProperty:
                                                {
                                                    foreach (string propertyName in
                                                                     item.Value.GetModifiedProperties())
                                                    {
                                                        if (propertyUpdate.Any(o => o.Name.ToLower() == propertyName.ToLower())
                                                            || propertyInactive.Any(o => o.Name.ToLower() == propertyName.ToLower())
                                                            || propertyCreated.Any(o => o.Name.ToLower() == propertyName.ToLower())
                                                            || propertySystemId.Any(o => o.Name.ToLower() == propertyName.ToLower())
                                                            || propertyCreatedOrUpdated.Any(o => o.Name.ToLower() == propertyName.ToLower())
                                                            || propertyUserIP.Any(o => o.Name.ToLower() == propertyName.ToLower()))
                                                        {
                                                            continue;
                                                        }

                                                        System.Data.Common.DbDataRecord original = item.Value.OriginalValues;
                                                        string oldValue = original.GetValue(original.GetOrdinal(propertyName)).ToString();

                                                        System.Data.Entity.Core.Objects.CurrentValueRecord current = item.Value.CurrentValues;
                                                        string newValue = current.GetValue(current.GetOrdinal(propertyName)).ToString();

                                                        if (oldValue != newValue) // probably not necessary
                                                        {
                                                            dto.HistoricalEvent histEvent = dto.HistoricalEvent.InstanceChanged(item.Value.Entity, tableName, keyName, keyValue,
                                                                                                                                propertyName, oldValue, newValue);
                                                            HistoricalModified(this, histEvent);
                                                        }
                                                    }
                                                }
                                                break;
                                            case Enumerators.HistoricalType.ByObject:
                                                {
                                                    dto.HistoricalEvent histEvent = dto.HistoricalEvent.InstanceChanged(item.Value.Entity, tableName, keyName, keyValue,
                                                                                                                        string.Empty, string.Empty, string.Empty);
                                                    HistoricalModified(this, histEvent);
                                                }
                                                break;
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                                break;
                        }
                    }
                }
            }

            var ret = base.SaveChanges();

            #region Historical Created
            if (doHistorical)
            {
                if (HistoricalCreated != null)
                {
                    foreach (KeyValuePair<Guid, System.Data.Entity.Core.Objects.ObjectStateEntry> item in added)
                    {
                        var key = item.Value.EntityKey.EntityKeyValues.FirstOrDefault();
                        string keyName = key?.Key;
                        string keyValue = key?.Value.ToString();
                        dto.HistoricalEvent histEvent = dto.HistoricalEvent.InstanceAdd(item.Value.Entity, item.Value.EntitySet.Name, keyName, keyValue);
                        HistoricalCreated(this, histEvent);
                    }
                }
            }
            #endregion

            return ret;
        }
        #endregion
    }
}
