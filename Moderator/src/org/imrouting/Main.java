package org.imrouting;

import java.util.logging.*;
import org.jivesoftware.whack.*;
import org.xmpp.component.*;
import log.*;
import log.Log;

public class Main {
	private final static String HOST = "46.137.26.124";
	private final static int PORT = 5270;
	
	private final static String DOMAIN = "txtfeedback.net";
	private final static String SUBDOMAIN = "moderator";
	private final static String SECRET_KEY = "im123!";
	
	public static void main(String[] args) {
	  ExternalComponentManager mgr = new ExternalComponentManager(HOST, PORT);
	  mgr.setServerName(DOMAIN);
	  mgr.setSecretKey(SUBDOMAIN, SECRET_KEY);
      
      try {
           mgr.addComponent(SUBDOMAIN, new TxtFeedbackModerator());
      } catch (ComponentException e) {
    	  Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
      }
      while (true)
         try {
            Thread.sleep(10000);
         } catch (Exception e) {
        	 Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
         }
   }
}