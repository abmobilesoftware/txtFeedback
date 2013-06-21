package org.imrouting;

import java.util.logging.*;
import org.jivesoftware.whack.*;
import org.xmpp.component.*;
import log.*;
import log.Log;

public class Main {
	private final static String HOST = "46.137.26.124";
	private final static int PORT = 5270;
	/**
	 * PRODUCT moderator.txtfeedback.net - im123!
	 * PRODUCT STAGING prodstaging.txtfeedback.net - im123!
	 * DEMO compdev.txtfeedback.net - im1234!
	 * DEV devxmpp.txtfeedback.net - im123456!
	 */
	public final static String DOMAIN = "txtfeedback.net";
	public final static String SUBDOMAIN = "prodstaging";
	private final static String SECRET_KEY = "im123!";
	
	public static void main(String[] args) {
	  ExternalComponentManager mgr = new ExternalComponentManager(HOST, PORT);
	  mgr.setServerName(DOMAIN);
	  mgr.setSecretKey(SUBDOMAIN, SECRET_KEY);
      
      try {
           mgr.addComponent(SUBDOMAIN, new TxtFeedbackModerator());
      } catch (ComponentException e) {
    	  Log.addLogEntry("Main", LogEntryType.ERROR, e.getMessage());
      }
      while (true)
         try {
            Thread.sleep(10000);
         } catch (Exception e) {
        	 Log.addLogEntry("Main", LogEntryType.ERROR, e.getMessage());
         }
   }
}