package org.imrouting;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedList;
import java.util.TimeZone;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import log.Log;
import log.LogEntryType;

import org.exceptions.RESTException;
import org.helpers.TxtPacket;
import org.helpers.Utilities;
import org.helpers.json.Agent;
import org.xmpp.packet.Message;

import rest.RestControllerGateway;

public class MessageProcessor {
	private TxtFeedbackModerator moderator;
	private RestControllerGateway restGtw;	
	private LinkedList<Message> failedMessages = new LinkedList<Message>();
	private static final ScheduledExecutorService worker = Executors.newScheduledThreadPool(5);
	private static int CALL_DELAY = 2;
	
	public MessageProcessor(TxtFeedbackModerator tfm) {
		moderator = tfm;	
		restGtw = new RestControllerGateway();
	}	
	public void processInternalPacket(Message message) {
		//System.out.println("Received message = " + message.toXML());
		try {
			TxtPacket internalPacket = new TxtPacket(message.getBody());
			//DA since the date coming from the Javascript client is unreliable we use the server side time as reference
			Date now = new Date();
			internalPacket.setDateSent(now);
			if (message.getType().equals(Message.Type.ClientMsgDeliveryReceipt)) {
				// For this message update WasReceivedByClient state in DB 
				//String ackId = message.getReceivedID();
				//String[] ackIds = ackId.split("##");
				// TODO update message with ID = ackIds[1]
				//String ackDest = message.getAckDestination();
				//moderator.sendAcknowledgeMessage(ackDest, Message.Type.ClientMsgDeliveryReceipt, ackIds[0]);
				
			} else {
			
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
			}
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getStackTrace().toString());
		}				
	}

	private void SendImMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		final Message receivedPacket = iPacket;
		long t1, t2;
		try {
			String[] fromTo = internalPacket.getConversationId().split("-");
			if (!Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()).equals(fromTo[1])) {
				//this is the case when we have staff site to staff site communication
				StringBuilder reversedConvIdSb = new StringBuilder();
				reversedConvIdSb.append(fromTo[1]);
				reversedConvIdSb.append("-");
				reversedConvIdSb.append(fromTo[0]);
				t1 = System.currentTimeMillis();
				restGtw.saveMessage(
					internalPacket.getFromAddress(), 
					internalPacket.getToAddress(), 
					internalPacket.getConversationId(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					false);
				restGtw.saveMessage(
					internalPacket.getFromAddress(),
					internalPacket.getToAddress(),
					reversedConvIdSb.toString(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					false);
				t2 = System.currentTimeMillis();
				System.out.println("\"" + receivedPacket.getID() + "\", \"" + "SAVE\", \"" +  (t2 - t1) + "\"");
			} else {
				
				t1 = System.currentTimeMillis();
				restGtw.saveMessage(
						internalPacket.getFromAddress(), 
						internalPacket.getToAddress(), 
						internalPacket.getConversationId(),
						internalPacket.getBody(), 
						iPacket.getFrom().toBareJID(),
						false);				
				t2 = System.currentTimeMillis();
				System.out.println(receivedPacket.getID() + " SAVE: " +  (t2 - t1));
			}
			//moderator.sendAcknowledgeMessage(iPacket.getFrom().toBareJID(), Message.Type.ServerMsgDeliveryReceipt, "srvAck", iPacket.getID());
			
			t1 = System.currentTimeMillis();			
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage(Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()), internalPacket.getConversationId(), false);
			t2 = System.currentTimeMillis();
			System.out.println("\"" + receivedPacket.getID() + "\", \"GET HANDLERS\", \"" + (t2 - t1) + "\"");
			for (int i=0; i<handlers.size(); ++i) {
				String from = internalPacket.getFromAddress();
				moderator.sendInternalMessage(internalPacket.toXML(), from, 
						iPacket.getID(), internalPacket.getFromAddress());			
			}						
		} catch (RESTException e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
			Runnable task = new Runnable() {
				public void run() {
					System.out.println("Re");
					processInternalPacket(receivedPacket);
				}				
			};
				worker.schedule(task, CALL_DELAY, TimeUnit.SECONDS);
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}
	}
	
	private void SendImMessageToClient(Message iPacket, TxtPacket internalPacket) {
		final Message receivedPacket = iPacket;
		try {
			restGtw.saveMessage(
					internalPacket.getFromAddress(),
					internalPacket.getToAddress(),
					internalPacket.getConversationId(), 
					internalPacket.getBody(),
					iPacket.getFrom().toBareJID(),
					false);
			
			// The request message sent to the client will have an ID of this format UUID##DBid
			//String computedId = iPacket.getID() + "##" + "dbID";
			// Send acknowledge to staff
			//moderator.sendAcknowledgeMessage(iPacket.getFrom().toString(), Message.Type.ServerMsgDeliveryReceipt, "srvAck", iPacket.getID());
			/* 
			* send both temporary and db id - the db id will be used on callback 
			* to mark that the message was successfully received
			* and the temporary will be forwarded to staff.
			*/
			
			/* TODO: replace with computedId. The current ID it's used to trace the message */
			moderator.sendInternalMessage(iPacket.getBody(), internalPacket.getToAddress(), receivedPacket.getID(), internalPacket.getFromAddress());
		} catch (RESTException e) {
				Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
				Runnable task = new Runnable() {
					public void run() {
						System.out.println("Re");
						processInternalPacket(receivedPacket);
					}				
				};
			worker.schedule(task, CALL_DELAY, TimeUnit.SECONDS);
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}
		
	}
	
	private void sendSmsMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		final Message receivedPacket = iPacket;
		try {				
			restGtw.saveMessage(
						internalPacket.getFromAddress(), 
						internalPacket.getToAddress(), 
						internalPacket.getConversationId(),
						internalPacket.getBody(), 
						iPacket.getFrom().toBareJID(),
						true);		
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage(Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()), internalPacket.getConversationId(), true);
			for (int i=0; i<handlers.size(); ++i) {
				String from = internalPacket.getFromAddress();
				moderator.sendInternalMessage(internalPacket.toXML(), from, 
						handlers.get(i).getUser(), internalPacket.getFromAddress());			
			}
		} catch (RESTException e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
			Runnable task = new Runnable() {
				public void run() {
					System.out.println("Re");
					processInternalPacket(receivedPacket);
				}				
			};
			worker.schedule(task, CALL_DELAY, TimeUnit.SECONDS);			
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}
	}
	
	private void sendSmsMessageToClient(Message iPacket, TxtPacket internalPacket) {
		final Message receivedPacket = iPacket;
		try {
			restGtw.saveMessage(
			 		internalPacket.getFromAddress(), 
					internalPacket.getToAddress(), 
					internalPacket.getConversationId(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					true);
		} catch (RESTException e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
			Runnable task = new Runnable() {
				public void run() {
					System.out.println("Re");
					processInternalPacket(receivedPacket);
				}				
			};
			worker.schedule(task, CALL_DELAY, TimeUnit.SECONDS);			
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}	
	}

}
