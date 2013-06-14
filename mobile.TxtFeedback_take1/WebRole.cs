using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.Administration;

namespace mobile.TxtFeedback_take1
{
   public class WebRole : RoleEntryPoint
   {
      public override void Run()
      {
         //using (var serverManager = new ServerManager())
         //{
         //   var mainSite = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];
         //   var mainApplication = mainSite.Applications["/"];
         //   mainApplication["preloadEnabled"] = true;

         //   var mainApplicationPool = serverManager.ApplicationPools[mainApplication.ApplicationPoolName];
         //   mainApplicationPool["startMode"] = "AlwaysRunning";

         //   serverManager.CommitChanges();
         //}

         base.Run();
      }

      public override bool OnStart()
      {
         // For information on handling configuration changes
         // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

         return base.OnStart();
      }
   }
}