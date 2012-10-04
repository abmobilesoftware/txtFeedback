package org.imrouting;

import java.util.logging.*;
import org.jivesoftware.whack.*;
import org.xmpp.component.*;

public class Main {
	private final static String HOST = "176.34.122.48";
	//private final static int PORT = 5275; -- Openfire component port
	private final static int PORT = 5270;
	public static void main(String args[]) {
		
	  ExternalComponentManager mgr = new ExternalComponentManager(HOST, PORT);
	  mgr.setServerName("txtfeedback.net");
	  mgr.setSecretKey("moderator", "im123!");
      
      try {
           mgr.addComponent("moderator", new TxtFeedbackModerator());
      } catch (ComponentException e) {
         Logger.getLogger(Main.class.getName()).log(Level.SEVERE, "main", e);
         System.exit(-1);
      }
      while (true)
         try {
            Thread.sleep(10000);
         } catch (Exception e) {
            Logger.getLogger(Main.class.getName()).log(Level.SEVERE, "main", e);
         }
   }
}