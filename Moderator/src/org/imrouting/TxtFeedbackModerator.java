package org.imrouting;

import java.io.IOException;
import java.util.AbstractMap.SimpleEntry;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import log.Log;
import log.LogEntryType;

import org.helpers.Constants;
import org.helpers.TxtPacket;
import org.helpers.Utilities;
import org.helpers.json.Agent;
import org.helpers.json.WorkingPoint;
import org.jivesoftware.whack.ExternalComponentManager;
import org.xmpp.component.Component;
import org.xmpp.component.ComponentException;
import org.xmpp.component.ComponentManager;
import org.xmpp.component.ComponentManagerFactory;
import org.xmpp.packet.JID;
import org.xmpp.packet.Message;
import org.xmpp.packet.Message.Type;
import org.xmpp.packet.Packet;
import org.xmpp.packet.Presence;

import org.apache.http.client.ClientProtocolException;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

 
import rest.RestClient;
import rest.RestControllerGateway;


public class TxtFeedbackModerator implements Component {
	private ExternalComponentManager mMgr = null;
	
	/* Store users who can send administrator commands */
	private LinkedList<String> privilegedUser = new LinkedList<String>();
	private Hashtable<String, WorkingPoint> WPAddressHash = new Hashtable(); 
	private RestControllerGateway restGtw = new RestControllerGateway();
	private int clientToStaffCounter = 0;
	private int staffToClientCounter = 0;
	private TxtFeedbackModerator self;
	private MessageProcessor mp;
	
	public String getName() {
		return ("Txtfeedback component");
	}

	public String getDescription() {
		return ("Txtfeedback heart");
	}

	public void processPacket(Packet iReceivedPacket) {
		//System.out.println(iReceivedPacket.toXML());
		/*Log.addLogEntry("Received packet ID=" + iReceivedPacket.getID() + 
				", FROM = " + iReceivedPacket.getFrom().toBareJID() +
				"  BODY = " + iReceivedPacket.toXML(), LogEntryType.INFO);*/
		if (iReceivedPacket instanceof Message) {
			final Message lReceivedMessage = (Message) iReceivedPacket;
			if (lReceivedMessage.getSubject() != null) {
				if (lReceivedMessage.getSubject().equals(Constants.INTERNAL_PACKET)) {
					mp.processInternalPacket(lReceivedMessage);					
				}
			}			
		} else if (iReceivedPacket instanceof Presence) {
			Presence lPresence = (Presence) iReceivedPacket;
			if (lPresence.getType() == Presence.Type.unavailable) {
				/* 
				 * Detect who logged off. But first that user must send an
				 * available presence stanza. 
				 */
			} 
		}
	}
	
	private void sendMessage(String iFrom, String iBody, String iSubject, Message.Type iType, String iTo) {
		try {
			Message lResponseMessage = new Message();
			lResponseMessage.setType(iType);
			lResponseMessage.setBody(iBody);
			lResponseMessage.setSubject(iSubject);
			lResponseMessage.setTo(iTo);
			String[] split = iFrom.split("@");
			String from = String.format("%s@%s.%s",split[0],Main.SUBDOMAIN,Main.DOMAIN);
			lResponseMessage.setFrom(from);//split[0] + "@"+ Main.SUBDOMAIN+ "." + Main.DOMAIN);
			
			mMgr.sendPacket(this, lResponseMessage);			
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
		}
	}
	
	public void sendInternalMessage(String iBody,String iFrom, String iTo) {
		sendMessage(iFrom, iBody, Constants.INTERNAL_PACKET, Type.chat, iTo);
	}
	
	public void initialize(JID iJid, ComponentManager iComponentManager)
			throws ComponentException {
		Log.addLogEntry("Component initializing", LogEntryType.INFO);
		mMgr = (ExternalComponentManager) iComponentManager;
		mp = new MessageProcessor(this);
		self = this;		
	}

	public void start() {
		Log.addLogEntry("Component started", LogEntryType.INFO);
	}

	public void shutdown() {
		Log.addLogEntry("Component is shutted down", LogEntryType.INFO);			
	}	
	
}
