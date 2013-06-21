package rest;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URISyntaxException;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.List;
import java.util.Map.Entry;

import log.Log;
import log.LogEntryType;

import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.utils.URIBuilder;
import org.exceptions.RESTException;
import org.helpers.Constants;
import org.helpers.Utilities;
import org.helpers.json.Agent;
import org.helpers.json.Device;
import org.helpers.json.MessageStatus;
import org.helpers.json.WorkingPoint;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class RestControllerGateway {
	/**
	 * REST domain (for handlers)
	 * 1. Development http://rest.txtfeedback.net/
	 * 2. Demo
	 * 		2.1 Staging: http://46683d8e44b54144bb6f8c6e21181bfe.cloudapp.net:81/
	 * 		2.2 Production http://demot3xt.cloudapp.net:81/
	 * 3. Production 
	 * 		3.1.Livehosting http://rest.txtfeedback.net/
	 * 	    3.2 Azure staging http://35881b38ae9a45f99b2c8c437e081399.cloudapp.net:81/
	 * 		3.3 Azure production t3xt.cloudapp.net:81/
	 * 
	 * API (Save & update message)
	 * 1. Development http://dev.txtfeedback.net
	 * 2. Demo 
	 * 		2.1 Staging http://8025dfa481dc4e13944c72c96e6afb3b.cloudapp.net
	 * 		2.2 Production http://demotxtfeedback.cloudapp.net
	 * 3. Production 
	 * 		3.1 Livehosting http://product.txtfeedback.net
	 * 		3.2 Azure staging http://cc894eae5c5145cb92f7dad77b0f9a67.cloudapp.net
	 * 		3.3 Azure production http://txtfeedback.cloudapp.net
	 */
	private String RESTDomain = "http://35881b38ae9a45f99b2c8c437e081399.cloudapp.net:81/";
	private String APIDomain = "http://cc894eae5c5145cb92f7dad77b0f9a67.cloudapp.net";
	
	private String RESTSaveMessage = APIDomain + "/Component/SaveMessage";
	private String RESTUpdateClientAcknowledge = APIDomain + "/Component/UpdateMessageClientAcknowledgeField";
		
	public ArrayList<Agent> getHandlersForMessage(
			String iWP, 
			String iConversationId, 
			boolean isSms) throws RESTException {
		try {
			StringBuilder urlSb = new StringBuilder(RESTDomain);
			urlSb.append(iWP);
			urlSb.append("/api/rules/");
			ArrayList<Agent> handlers = new ArrayList<Agent>();
			Hashtable<String, String> params = new Hashtable<String, String>();
			params.put("from", URLEncoder.encode(iConversationId, "UTF-8"));			
			JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(urlSb.toString(), 
					RestClient.GET, 
					params, 
					Constants.APPLICATION_JSON, 
					Constants.APPLICATION_JSON);
			if (listOfAgentsJsonObject != null) {
				JSONArray listOfAgentsArray = listOfAgentsJsonObject.getJSONArray("agents");
				for (int i=0; i<listOfAgentsArray.length(); ++i) {
					JSONArray listOfDevices = listOfAgentsArray.getJSONObject(i).getJSONArray("devices");
					ArrayList<Device> devices = new ArrayList<Device>();					
					for (int j=0; j<listOfDevices.length(); ++j) {
						devices.add(new Device(listOfDevices.getJSONObject(j).getString("id")));	
					}
					handlers.add(new Agent(
							listOfAgentsArray.getJSONObject(i).getString("xmppId"),
							devices,
							listOfAgentsArray.getJSONObject(i).getInt("priority"))
					);												
				}
			}
			return handlers;
		}catch (Exception e) {
			Log.addLogEntry(
					"getHandlersForMessage", 
					LogEntryType.ERROR,
					Utilities.getStackTrace(e.getCause()));				
			throw new RESTException();
		}		
	}												
	
	public boolean updateMessageClientAcknowledgeField(
			int msgID, 
			boolean clientAcknowledge) throws RESTException {
		try {
			Hashtable<String, String> params = new Hashtable<String, String>();
			params.put("msgID", Integer.toString(msgID));
			params.put("clientAcknowledge", String.valueOf(clientAcknowledge));
			String requestResult = getResourceAsString(RESTUpdateClientAcknowledge, 
					RestClient.GET, params, 
					Constants.APPLICATION_JSON, 
					Constants.APPLICATION_JSON);
			if (requestResult != null) {
				String result = requestResult.toString();
				if (result.equals("success")) return true;
				else return false;			
			}	
		} catch (Exception e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), 
					LogEntryType.ERROR, 
					Utilities.getStackTrace(e.getCause()));
			throw new RESTException();
		}
		return false;
	}	
		
	public MessageStatus saveMessage(String from, 
			String to, 
			String convId, 
			String text, 
			String xmppUser, 
			boolean isSms) throws RESTException {
		try {
			Hashtable<String, String> params = new Hashtable<String, String>();
			params.put("from", from);
			params.put("to", to);
			params.put("convId", convId);
			params.put("text", URLEncoder.encode(text, "UTF-8"));
			params.put("xmppUser", xmppUser);
			params.put("isSms", String.valueOf(isSms));
		
			JSONObject restResponse = getResourceAsJsonObject(RESTSaveMessage, 
					RestClient.GET, params, 
					Constants.APPLICATION_JSON, 
					Constants.APPLICATION_JSON);
			MessageStatus msgStatus = null;
			if (restResponse != null) {
				msgStatus = new MessageStatus(restResponse.getInt("MessageID"), 
						restResponse.getBoolean("MessageSent"),
						restResponse.getBoolean("WarningLimitReached"),
						restResponse.getBoolean("SpendingLimitReached"),
						restResponse.getString("Reason"),
						restResponse.toString());
			}
			return msgStatus;			
		} catch (Exception e) {
			Log.addLogEntry("saveMessage", 
					LogEntryType.ERROR, 
					Utilities.getStackTrace(e.getCause()));					
			throw new RESTException();			
		}
	}
	
	private JSONObject getResourceAsJsonObject(
			String iURL,
			int iMethod, 
			Hashtable<String, String> iParams,
			String iHeaderAccept, 
			String iContentType) throws Exception {
		JSONObject wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, iMethod, iParams, iHeaderAccept, iContentType);
		try {
			ri.createObjectJson();
			wpJsonObj = ri.getRecvObject();
		} catch (JSONException e) {
			Log.addLogEntry("getResourceAsJsonObject", LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		}
		return wpJsonObj;
	}
	
	private String getResourceAsString(
			String iURL,
			int iMethod, 
			Hashtable<String, String> iParams,
			String iHeaderAccept, 
			String iContentType) throws Exception {
		String wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, 
				iMethod, 
				iParams, 
				iHeaderAccept, 
				iContentType);
		return ri.getResponse();
	}
	
	private RestClient callRESTResource(
			String iURL,
			int iMethod, 
			Hashtable<String, String> iParams,
			String iHeaderAccept, 
			String iContentType) throws Exception {
		String restResource = iURL;
		JSONObject wpJsonObj = null;
		RestClient ri = new RestClient(restResource);
		if (iParams != null) {
			Iterator<Entry<String, String>> paramsIterator = iParams.entrySet().iterator();
			while (paramsIterator.hasNext()) {
				Entry<String, String> parameter = paramsIterator.next();
				ri.addParam(parameter.getKey(), parameter.getValue());
			}
		}
		ri.addHeader("Accept", iHeaderAccept);
		ri.addHeader("Content-type", iContentType);
		try {
			ri.callWebService(iMethod);									
			// INFO Log the event - a REST resource was accessed 
			//System.out.println(ri.getResponse());
		} catch (IllegalStateException e) {
			Log.addLogEntry("callRESTResource", LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} catch (ClientProtocolException e) {
			Log.addLogEntry("callRESTResource", LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} catch (IOException e) {
			Log.addLogEntry("callRESTResource", LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} 
		return ri;
	}
	
	public String callGCMRest(
			ArrayList<Device> devices, 
			String message) {
		if (devices.size() > 0) {
			RestClient ri = new RestClient("https://android.googleapis.com/gcm/send");
			StringBuilder registrationIds = new StringBuilder();
				registrationIds.append("\"" + devices.get(0).id + "\"");
				for (int i=1; i<devices.size(); ++i) {
					registrationIds.append(", \"" + devices.get(i).id + "\"");
				};
			ri.addParam("\"registration_ids\"", "[" + registrationIds.toString() + "]");
			ri.addHeader("Authorization", "key=AIzaSyAzp0RTyzXCuI8dkw6FxViK8Rn2hTl1ecw");
			ri.addHeader("Content-type", "application/json");
			ri.addParam("\"data\"", "{\"payload\":\"" + message + "\"}");
			try {
				ri.sendHttpPostWithJsonBody();				
			} catch (Exception e) {
				System.out.println(e.getMessage());
			}
			return ri.getResponse();
		} else {
			return "0 devices";
		}
	}
	
}
