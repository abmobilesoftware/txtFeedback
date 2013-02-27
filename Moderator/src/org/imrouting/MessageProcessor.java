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
import org.helpers.Constants;
import org.helpers.TxtPacket;
import org.helpers.Utilities;
import org.helpers.json.Agent;
import org.helpers.json.MessageStatus;
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
		long t1, t2;
		t1 = System.currentTimeMillis();
		try {
			if (message.getSubject().equals(Constants.CLIENT_ACK)) {
				// For this message update WasReceivedByClient state in DB 
				String ackId = message.getReceivedID();  // ackId = PACKET_ID**DB_ID
				String[] ackIds = ackId.split("##");
								
				moderator.sendAcknowledgeMessage("",message.getAckDestination(), "ClientMsgDeliveryReceipt", ackIds[0]);
				restGtw.updateMessageClientAcknowledgeField(Integer.parseInt(ackIds[1]), true);
			} else if (message.getSubject().equals(Constants.INTERNAL_PACKET)) {
				TxtPacket internalPacket = new TxtPacket(message.getBody());
				//DA since the date coming from the Javascript client is unreliable we use the server side time as reference
				Date now = new Date();
				internalPacket.setDateSent(now);
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
		t2 = System.currentTimeMillis();
		System.out.println("PROCESSING " + (t2 - t1));
	}

	private void SendImMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		System.out.println(iPacket.getID() + " IN TO STAFF - " + System.currentTimeMillis());
		final Message receivedPacket = iPacket;
		long t1, t2, t3, t4;
		try {
			String[] fromTo = internalPacket.getConversationId().split("-");
			MessageStatus msgStatus; 
			if (!Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()).equals(fromTo[1])) {
				//this is the case when we have staff site to staff site communication
				StringBuilder reversedConvIdSb = new StringBuilder();
				reversedConvIdSb.append(fromTo[1]);
				reversedConvIdSb.append("-");
				reversedConvIdSb.append(fromTo[0]);
				t1 = System.currentTimeMillis();
				msgStatus = restGtw.saveMessage(
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
				msgStatus = restGtw.saveMessage(
						internalPacket.getFromAddress(), 
						internalPacket.getToAddress(), 
						internalPacket.getConversationId(),
						internalPacket.getBody(), 
						iPacket.getFrom().toBareJID(),
						false);				
				t2 = System.currentTimeMillis();
				System.out.println(receivedPacket.getID() + " SAVE: " +  (t2 - t1));
			}
			t3 = System.currentTimeMillis();
			moderator.sendAcknowledgeMessage("",iPacket.getFrom().toBareJID(), Constants.SERVER_ACK, iPacket.getID());
			t4 = System.currentTimeMillis();
			
			t1 = System.currentTimeMillis();			
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage(Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()), internalPacket.getConversationId(), false);
			t2 = System.currentTimeMillis();
			System.out.println("\"" + receivedPacket.getID() + "\", \"GET HANDLERS\", \"" + (t2 - t1) + "\", \"" + (t4 - t3) + "\"");
			
			String computedID = iPacket.getID() + "##" + String.valueOf(msgStatus.getMessageID()); // PACKET_ID**DB_ID
			for (int i=0; i<handlers.size(); ++i) {
				//String from = internalPacket.getFromAddress();
				String to = handlers.get(i).getUser();
				String from = internalPacket.getFromAddress();
				moderator.sendInternalMessage(internalPacket.toXML(), from, to, 
						computedID, internalPacket.getFromAddress());			
			}			
			System.out.println(iPacket.getID() + " OUT TO STAFF - " + System.currentTimeMillis());
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
		System.out.println(iPacket.getID() + " IN: TO CLIENT " + System.currentTimeMillis());
		final Message receivedPacket = iPacket;
		try {
			MessageStatus msgStatus;
			msgStatus = restGtw.saveMessage(
					internalPacket.getFromAddress(),
					internalPacket.getToAddress(),
					internalPacket.getConversationId(), 
					internalPacket.getBody(),
					iPacket.getFrom().toBareJID(),
					false);
			
			// The request message sent to the client will have an ID of this format UUID##DBid
			//String computedId = iPacket.getID() + "##" + "dbID";
			// Send acknowledge to staff
			long t1, t2;
			t1 = System.currentTimeMillis();
			moderator.sendAcknowledgeMessage("",iPacket.getFrom().toString(), Constants.SERVER_ACK, iPacket.getID());
			t2 = System.currentTimeMillis();
			System.out.println("\"ACK2\", \"" +  (t2 - t1) + "\"");
			 
			/* send both temporary and db id - the db id will be used on callback 
			* to mark that the message was successfully received
			* and the temporary will be forwarded to staff.
			*/
			
			/* TODO: replace with computedId. The current ID it's used to trace the message */
			String computedId = iPacket.getID() + "##" + msgStatus.getMessageID();
			String from = internalPacket.getFromAddress();
			moderator.sendInternalMessage(iPacket.getBody(),from, internalPacket.getToAddress(), computedId, iPacket.getFrom().toBareJID());
			System.out.println(iPacket.getID() + " OUT: TO CLIENT " + System.currentTimeMillis());
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
				moderator.sendInternalMessage(internalPacket.toXML(), from, from, 
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
			MessageStatus msgStatus = 
			restGtw.saveMessage(
			 		internalPacket.getFromAddress(), 
					internalPacket.getToAddress(), 
					internalPacket.getConversationId(),
					internalPacket.getBody(), 
					iPacket.getFrom().toBareJID(),
					true);
			moderator.sendAcknowledgeMessage("",iPacket.getFrom().toString(), Constants.SERVER_ACK, iPacket.getID());
			moderator.sendSmsAcknowledgeMessage(iPacket.getFrom().toString(), Constants.CLIENT_ACK, iPacket.getID(), msgStatus.getJsonFormat());
			restGtw.updateMessageClientAcknowledgeField(msgStatus.getMessageID(), true);
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
