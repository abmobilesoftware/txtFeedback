using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.XMPP;
using System.Threading;

namespace WS2
{
   public delegate void ConnectedEventHandler(object sender, EventArgs e);
   class XmppClientWrapper      
   {
      public event ConnectedEventHandler Connected;
      private XMPPClient client = new XMPPClient();
      private const string USERNAME = "dotnet";
      private const string PASSWORD = "123456";
      private const string SERVER = "46.137.26.124";
      private const string DOMAIN = "txtfeedback.net";
      private const int PORT = 5222;
      public bool IsConnected { get; set; }

      public XmppClientWrapper()
      {
         IsConnected = false;
         client.UserName = USERNAME;
         client.Password = PASSWORD;
         client.Server = SERVER;
         client.Domain = DOMAIN;
         client.Resource = Guid.NewGuid().ToString();
         client.Port = PORT;
         client.OnStateChanged += new EventHandler(stateChanged);         
      }

      public void sendXmppMessage(string message, string address)
      {
         client.SendChatMessage(message, address);
      }

      public void connect()
      {
         client.Connect();
      }

      public void disconnect()
      {
         client.Disconnect();
      }

      private void stateChanged(object sender, EventArgs e)
      {
         if (client.XMPPState.Equals(XMPPState.Ready))
         {
            IsConnected = true;
            Connected(this, null);
         }

      }

     
   }
}
