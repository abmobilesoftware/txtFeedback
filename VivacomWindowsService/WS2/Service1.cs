using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WS2
{
   public partial class X1 : ServiceBase
   {
      private Logic logic;
      private const int INTERVAL_LENGTH = 2;

      public X1()
      {
         InitializeComponent();
         if (!System.Diagnostics.EventLog.SourceExists("Vivacom"))
         {
            System.Diagnostics.EventLog.CreateEventSource(
               "Vivacom", "Vivacom");
         }
         eventLog1.Source = "VivacomSource";
         eventLog1.Log = "VivacomLog";
      }

      protected override void OnStart(string[] args)
      {
         logic = new Logic(eventLog1);
         executePeriodical();
      }

      protected override void OnStop()
      {
      
      }

      private void executePeriodical()
      {
         AutoResetEvent exit = new AutoResetEvent(false);
         RegisteredWaitHandle h = ThreadPool.RegisterWaitForSingleObject(exit,
            new WaitOrTimerCallback(logic.checkInbox), null, INTERVAL_LENGTH * 1000, false);
      }
   }
}
