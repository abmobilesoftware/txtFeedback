package org.imrouting;

import java.io.IOException;
import java.util.AbstractMap.SimpleEntry;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.UUID;

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
				mp.processInternalPacket(lReceivedMessage);			
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
	
	/*
	 * Parameters: 
	 * 		- askForAck: 1.TRUE (internal message) - ackDest - filled in, ackID - null
	 * 					 2.FALSE (acknowledge message) - askDest - null, ackID - filled in 				 
	 */	
	private void sendMessage(String iTo, String iBody, String iSubject, String iMsgID, Message.Type iType, boolean askForAck, String iAckID, String iAckDest) {
		try {
			Message lResponseMessage = new Message();
			lResponseMessage.setTo(iTo);
			if (iBody != null) {
				lResponseMessage.setBody(iBody);
			}
			if (iSubject != null ) {
				lResponseMessage.setSubject(iSubject);
			}	
			if (iType != null) {
				lResponseMessage.setType(iType);
			}			
			if (iMsgID != null) {
				lResponseMessage.setID(iMsgID);
			}
			
			if (askForAck) {
				if (iAckDest != null) {
					lResponseMessage.setRequest(iAckDest);
				}
			} else {
				if (iAckID != null) {
					lResponseMessage.setReceivedID(iAckID);
					lResponseMessage.setID(iAckID);
				}				
			}
			
			mMgr.sendPacket(this, lResponseMessage);			
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
		}
	}
	
	/*
	 * Send a text message (sms or IM) inside the system
	 * Parameters:
	 * 	- ackDest : the recipient intended to receive the acknowledge for this message. 
	 *  Obs: askForAck - true
	 */
	public void sendInternalMessage(String iBody, String iTo, String iMsgID, String ackDest) {
		sendMessage(iTo, iBody, Constants.INTERNAL_PACKET, iMsgID, Type.chat, true, null, ackDest);
	}
	
	/*
	 * Send an acknowledge stanza.
	 * Parameters: 
	 *   - iAckID - is the id of the received message
	 *   - iMsgType - two possible values: ServerMsgDeliveryReceipt or ClientMsgDeliveryReceipt 
	 * Format:
	 * <message from='sender' id='random_id' to='iTo' type='ServerMsgDeliveryReceipt'>
	 * 	<received xmlns='urn:xmpp:receipts' id='iAckID' />
	 * </message> 
	 */
	public void sendAcknowledgeMessage(String iTo, String iSubject, String iAckID) {
		UUID uuid = UUID.randomUUID();
		sendMessage(iTo, null, iSubject, uuid.toString(), null, false, iAckID, null);
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
