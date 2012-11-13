package org.imrouting;

import java.util.ArrayList;
import java.util.LinkedList;
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
	
	public MessageProcessor(TxtFeedbackModerator tfm) {
		moderator = tfm;	
		restGtw = new RestControllerGateway();
	}
	
	public void processInternalPacket(Message message) {
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
		final Message receivedPacket = iPacket;
		try {
			String[] fromTo = internalPacket.getConversationId().split("-");
			if (!Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()).equals(fromTo[1])) {
				StringBuilder reversedConvIdSb = new StringBuilder();
				reversedConvIdSb.append(fromTo[1]);
				reversedConvIdSb.append("-");
				reversedConvIdSb.append(fromTo[0]);
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
			} else {
				restGtw.saveMessage(
						internalPacket.getFromAddress(), 
						internalPacket.getToAddress(), 
						internalPacket.getConversationId(),
						internalPacket.getBody(), 
						iPacket.getFrom().toBareJID(),
						false);				
			}
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage(Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()), internalPacket.getConversationId(), false);
			for (int i=0; i<handlers.size(); ++i) {
				String from = internalPacket.getFromAddress();
				moderator.sendInternalMessage(iPacket.getBody(), from, 
						handlers.get(i).getUser());			
			}						
		} catch (RESTException e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
			Runnable task = new Runnable() {
				public void run() {
					System.out.println("Re");
					processInternalPacket(receivedPacket);
				}				
			};
				worker.schedule(task, 5, TimeUnit.SECONDS);
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
			String from = internalPacket.getFromAddress();
			moderator.sendInternalMessage(iPacket.getBody(), from,
					internalPacket.getToAddress());
		} catch (RESTException e) {
				Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
				Runnable task = new Runnable() {
					public void run() {
						System.out.println("Re");
						processInternalPacket(receivedPacket);
					}				
				};
			worker.schedule(task, 5, TimeUnit.SECONDS);
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}
		
	}
	
	private void sendSmsMessageToStaff(Message iPacket, TxtPacket internalPacket) {
		final Message receivedPacket = iPacket;
		try {
			//restGtw.saveMessage(internalPacket.getFromAddress(), internalPacket.getToAddress(), internalPacket.getConversationId(), internalPacket.getBody(), iPacket.getFrom().toBareJID(), true);
			//String WPTelNumber = getWPForThisAddress(internalPacket.getToAddress());
			ArrayList<Agent> handlers = restGtw.getHandlersForMessage(Utilities.extractUserFromAddress(iPacket.getTo().toBareJID()), internalPacket.getConversationId(), true);
			for (int i=0; i<handlers.size(); ++i) {
				String from = internalPacket.getFromAddress();
				moderator.sendInternalMessage(iPacket.getBody(), from, 
						handlers.get(i).getUser());			
			}
		} catch (RESTException e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());
			Runnable task = new Runnable() {
				public void run() {
					System.out.println("Re");
					processInternalPacket(receivedPacket);
				}				
			};
			worker.schedule(task, 5, TimeUnit.SECONDS);			
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
			worker.schedule(task, 5, TimeUnit.SECONDS);			
		} catch (Exception e) {
			Log.addLogEntry(e.getMessage(), LogEntryType.ERROR, e.getMessage());			
		}	
	}

}
