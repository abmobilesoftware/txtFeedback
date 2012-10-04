package org.imrouting;

import java.io.IOException;
import java.util.AbstractMap.SimpleEntry;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

import org.helpers.Agent;
import org.helpers.Constants;
import org.helpers.TxtPacket;
import org.helpers.WorkingPoint;
import org.jivesoftware.whack.ExternalComponentManager;
import org.xmpp.component.Component;
import org.xmpp.component.ComponentManager;
import org.xmpp.component.ComponentManagerFactory;
import org.xmpp.component.ComponentException;
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
	private String COMPONENT_NAME = "moderator.txtfeedback.net";
	private Hashtable<String, WorkingPoint> WPAddressHash = new Hashtable(); 
	private RestControllerGateway restGtw = new RestControllerGateway();
	
	public String getName() {
		return ("Txtfeedback component");
	}

	public String getDescription() {
		return ("Txtfeedback heart");
	}

	public void processPacket(Packet iReceivedPacket) {
		System.out.println(iReceivedPacket.toXML());
		if (iReceivedPacket instanceof Message) {
			Message lReceivedMessage = (Message) iReceivedPacket;
			// TODO : Process body.
			if (lReceivedMessage.getSubject() != null) {
				if (lReceivedMessage.getSubject().equals(Constants.INTERNAL_PACKET)) {
					processInternalPacket(lReceivedMessage.getBody());				
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
	
	private String processInternalPacket(String iPacketContent) {
		TxtPacket internalPacket = new TxtPacket(iPacketContent);
		if (!internalPacket.getIsSms()) {
			if (internalPacket.getIsForStaff()) {
				String WPTelNumber = getWPForThisAddress(internalPacket.getToAddress());
				ArrayList<Agent> handlers = restGtw.getHandlersForMessage(WPTelNumber, internalPacket.getConversationId());
				for (int i=0; i<handlers.size(); ++i) {
					sendInternalMessage(iPacketContent, 
							handlers.get(i).getUser(),
							internalPacket.getToAddress());			
				}
				return "MsgToStaff";
			} else {
				sendInternalMessage(iPacketContent, 
						internalPacket.getFromAddress(),
						internalPacket.getToAddress());
				return "MsgToClient";
			}
		} else {
			if (internalPacket.getIsForStaff()) {
				String WPTelNumber = getWPForThisAddress(internalPacket.getToAddress());
				ArrayList<Agent> handlers = restGtw.getHandlersForMessage(WPTelNumber, internalPacket.getConversationId());
				for (int i=0; i<handlers.size(); ++i) {
					sendInternalMessage(iPacketContent, 
							handlers.get(i).getUser(),
							internalPacket.getToAddress());			
				}				
			} else {
				restGtw.sendMessage(internalPacket.getToAddress(), internalPacket.getFromAddress(), internalPacket.getConversationId(), internalPacket.getBody());
				
			}
			System.out.println("Message sent to " + internalPacket.getToAddress() 
					+ " , from " + internalPacket.getFromAddress());
		}
		return "";		
	}
	
	private String createConvId(TxtPacket iInternalPacket) {
		String conversationId = ""; 
		if (iInternalPacket.getIsSms()) {
			conversationId = cleanupPhoneNumber(iInternalPacket.getFromAddress()) + "-" + cleanupPhoneNumber(iInternalPacket.getToAddress());	
		} else {
			// ConversationId is SomGUID@txtfeedback.net-wp1@lidl.txtfeedback.net
			conversationId = iInternalPacket.getFromAddress() + "-" + iInternalPacket.getToAddress();			
		}
		return conversationId;
	}
	
	private String getWPForThisAddress(String iAddress) {
		if (WPAddressHash.containsKey(iAddress)) return WPAddressHash.get(iAddress).getTelNumber(); 
		else {
			WorkingPoint lWP = restGtw.getWorkingPointForCertainAddress(iAddress);
			if (lWP != null) {
				WPAddressHash.put(iAddress, lWP);
				return lWP.getTelNumber();
			} else {
				return Constants.REQUEST_WITH_NO_RESULT;
			}			
		}	
	}
	
	private String cleanupPhoneNumber(String phoneNumber) {
		//take into account that they could start with + or 00 - so we strip away any leading + or 00
		String transformedString;
		String pattern1 = "^00";
		String pattern2 = "^+";
		// delete 00
		transformedString = phoneNumber.replaceAll(pattern1, "");
		// delete +
		transformedString = transformedString.replaceAll(pattern2, "");
		return transformedString;
	}
	
	private void sendMessage(String iBody, String iSubject, Message.Type iType, String iTo, String iFrom) {
		try {
			Message lResponseMessage = new Message();
			lResponseMessage.setType(iType);
			lResponseMessage.setBody(iBody);
			lResponseMessage.setSubject(iSubject);
			lResponseMessage.setTo(iTo);
			lResponseMessage.setFrom(iFrom);
			mMgr.sendPacket(this, lResponseMessage);
		} catch (Exception e) {
			System.out.println(e.getMessage());
		}
	}
	
	private void sendInternalMessage(String iBody, String iTo, String iFrom) {
		sendMessage(iBody, Constants.INTERNAL_PACKET, Type.chat, iTo, iFrom);
	}
	
	public void initialize(JID iJid, ComponentManager iComponentManager)
			throws ComponentException {
		System.out.println("Initializing component.");
		mMgr = (ExternalComponentManager) iComponentManager;
				
	}

	public void start() {
		System.out.println("Component started.");
	}

	public void shutdown() {
		System.out.println("Component is shutted down.");			
	}
	
}
