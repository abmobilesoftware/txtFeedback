using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TxtPushService
{
   public partial class Service1 : ServiceBase
   {
      private Logic logic;
      private const int INTERVAL_LENGTH = 20;
      public Service1()
      {
         InitializeComponent();
      }

      protected override void OnStart(string[] args)
      {
         logic = new Logic();
      }

      protected override void OnStop()
      {
         logic.dispose();
      }

      private void executePeriodical()
      {
         AutoResetEvent exit = new AutoResetEvent(false);
         RegisteredWaitHandle h = ThreadPool.RegisterWaitForSingleObject(exit,
            new WaitOrTimerCallback(logic.checkInbox), null, INTERVAL_LENGTH * 1000, false);
      }
   }
}
