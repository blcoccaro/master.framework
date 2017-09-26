using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using master.framework;
using master.framework.dto.service;

namespace master.framework.services.windows
{
    public class BaseService : ServiceBase, master.framework.interfaces.IServiceBase
    {
        #region Properties

        #region Private
        private static object lockObject = new object();
        private System.Threading.Timer timer;
        #endregion

        #region Public
        public bool isDebugMode { get; set; }

        public string ServiceID { get; set; }

        public string SharedUUID { get; set; }

        public Message Messages { get; set; }
        #endregion

        #endregion

        #region Delegates
        public delegate void InitHandler(string[] args, master.framework.interfaces.IServiceBase source);
        public delegate void FinalizeHandler(master.framework.interfaces.IServiceBase source);
        public delegate void DoProcessHandler(object obj, master.framework.interfaces.IServiceBase source);
        public delegate bool DebugHandler(object obj, master.framework.interfaces.IServiceBase source);
        #endregion

        #region Events
        /// <summary>
        /// Event called before OnStart
        /// </summary>
        public event InitHandler Init;
        /// <summary>
        /// Event called before OnStop
        /// </summary>
        public event FinalizeHandler Finalize;
        /// <summary>
        /// Event called in the middle of Process
        /// </summary>
        public event DoProcessHandler DoProcess;
        /// <summary>
        /// Event called when debug mode is on
        /// </summary>
        public event DebugHandler DebugProcess;
        #endregion

        #region Methods

        #region Override
        protected override void OnStart(string[] args)
        {
            if (Init != null)
            {
                this.logInfo("Executing Init");
                Init(args, this);
            }
            if (Debugger.IsAttached)
            {
                this.logInfo("Executing Init");
                Process(null);
            }
            else
            {
                this.logInfo("Executing Init");
                timer = new System.Threading.Timer(Process, null, 0, master.framework.Configuration.MasterFramework.Service.TimerInMilliseconds);
            }
        }
        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
            CustomCommand((master.framework.Enumerators.WinServiceCommand)command);
        }

        protected override void OnStop()
        {
            this.logInfo("Stop");
            if (Finalize != null)
            {
                this.logInfo("Executing Finalize");
                Finalize(this);
            }
            timer = null;
        }
        #endregion

        #region Private
        private void CustomCommand(master.framework.Enumerators.WinServiceCommand command)
        {
            switch (command)
            {
                case Enumerators.WinServiceCommand.Beacon:
                    Beacon("Beacon Command Activated.");
                    this.logDebug("Beacon sended");
                    break;
                case Enumerators.WinServiceCommand.StartDebugMode:
                    isDebugMode = true;
                    this.logDebug("DebugMode Started");
                    break;
                case Enumerators.WinServiceCommand.StopDebugMode:
                    isDebugMode = false;
                    this.logDebug("DebugMode Stopped");
                    break;
            }
        }

        public void ProcessWhileDebugIsAttached()
        {
            Process(null);
        }
        private void Process(object state)
        {
            if (master.framework.Configuration.MasterFramework.Service.Debug.isON)
            {
                bool cont = true;
                do
                {
                    if (DebugProcess != null)
                    {
                        cont = DebugProcess(null, this);
                    }
                } while (cont);
            }

            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    #region Process
                    this.logDebug("Start");
                    try
                    {
                        if (DoProcess != null)
                        {
                            DoProcess(null, this);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logError("Error", ex);
                    }
                    #endregion
                    this.logDebug("End");
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

        }
        #endregion

        #region Public      
        public virtual void Beacon(string serviceId = null, string message = null)
        {
            throw new NotImplementedException();
        }

        public virtual void CallBack(int messageType, string message)
        {
            throw new NotImplementedException();
        }

        public virtual void CallBackMultiple(int id, params string[] message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
