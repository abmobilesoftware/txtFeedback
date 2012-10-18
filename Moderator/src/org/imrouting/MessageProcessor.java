package org.imrouting;

import java.util.ArrayList;

import log.Log;
import log.LogEntryType;

import org.helpers.TxtPacket;
import org.helpers.json.Agent;
import org.xmpp.packet.Message;

import rest.RestControllerGateway;

public class MessageProcessor {
	private TxtFeedbackModerator moderator;
	private Message message;
	private RestControllerGateway restGtw;	
	
	public MessageProcessor(TxtFeedbackModerator tfm, Message xm) {
		moderator = tfm;	
		message = xm;	
		restGtw = new RestControllerGateway();
	}
	
	public void processInternalPacket() {
		try {
			TxtPacket internalPacket = new TxtPacket(message.getBody());
			if (!internalPacket.getIsSms()) {
				if (internalPacket.getIsForStaff()) {
					SendImMessageToStaff(message, internalPacket);
				} else {
					SendImMessageToClient(message, internalPacket);				
				}
			} else {
				if (internalPacket.getIsForStaff()) {
					sendSmsMessageToStaff(message, internalPacket);				
				} else {
					sendSmsMessageToClient(message, internalPacket);				
				}				
			}
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getStackTrace().toString());
		}				
	}

	private void SendImMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		try {
			restGtw.saveMessage(
					internalPacket.getFromAddress(), 
					internalPacket.getToAddress(), 
					internalPacket.getConversationId(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					false);
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage1(iPacket.getTo().toBareJID(), internalPacket.getConversationId(), false);
			for (int i=0; i<handlers.size(); ++i) {
				moderator.sendInternalMessage(iPacket.getBody(), 
						handlers.get(i).getUser(),
						internalPacket.getFromAddress());			
			}						
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
		}
	}
	
	private void SendImMessageToClient(Message iPacket, TxtPacket internalPacket) {
		try {
			restGtw.saveMessage(
					internalPacket.getToAddress(),
					internalPacket.getFromAddress(),
					internalPacket.getConversationId(), 
					internalPacket.getBody(),
					iPacket.getFrom().toBareJID(),
					false);
			moderator.sendInternalMessage(iPacket.getBody(), 
					internalPacket.getFromAddress(),
					internalPacket.getToAddress());
			} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
		}
		
	}
	
	private void sendSmsMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		try {
			//restGtw.saveMessage(internalPacket.getFromAddress(), internalPacket.getToAddress(), internalPacket.getConversationId(), internalPacket.getBody(), iPacket.getFrom().toBareJID(), true);
			//String WPTelNumber = getWPForThisAddress(internalPacket.getToAddress());
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage1(iPacket.getTo().toBareJID(), internalPacket.getConversationId(), true);
			for (int i=0; i<handlers.size(); ++i) {
				moderator.sendInternalMessage(iPacket.getBody(), 
						handlers.get(i).getUser(),
						internalPacket.getToAddress());			
			}
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
		}
	}
	
	private void sendSmsMessageToClient(Message iPacket, TxtPacket internalPacket) {
		try {
			restGtw.saveMessage(
					internalPacket.getToAddress(), 
					internalPacket.getFromAddress(), 
					internalPacket.getConversationId(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					true);
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());	
		}		
	}

}
