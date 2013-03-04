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
import org.helpers.json.MessageStatus;
import org.helpers.json.WorkingPoint;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class RestControllerGateway {
	/* REST resources for dev 
	private String RESTGetHandlersForMessageURL = "http://dev.txtfeedback.net/Component/GetHandlerForMessage";
//	private String RESTGetHandlersForMessageURL = "http://localhost:4631/Component/GetHandlerForMessage";
	private String RESTDomain = "http://rest.txtfeedback.net/";
	private String RESTGetHandlersForMessageURL1 = "http://dev.txtfeedback.net/Component/GetHandlerForMessage1";
	//private String RESTGetHandlersForMessageURL1 = "http://localhost:4631/Component/GetHandlerForMessage1";
	private String RESTGetWorkingPointForCertainAddress = "http://dev.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	private String RESTSaveMessage = "http://localhost:4631/en-US/Component/SaveMessage";
	//private String RESTSaveMessage = "http://dev.txtfeedback.net/Component/SaveMessage";
	private String RESTParametersTest = "http://dev.txtfeedback.net/Component/GetParametersTest";
	private String RESTUpdateClientAcknowledge = "http://dev.txtfeedback.net/Component/UpdateMessageClientAcknowledgeField";
	//private String RESTUpdateClientAcknowledge = "http://localhost:4631/Component/UpdateMessageClientAcknowledgeField";
	//private String RESTUpdateClientAcknowledge = "http://localhost:4631/Component/UpdateMessageClientAcknowledgeField";
*/	 	 
	 	 
	/* REST resources for nexmo
	private String RESTGetHandlersForMessageURL = "http://demo.txtfeedback.net/Component/GetHandlerForMessage";
	private String RESTDomain = "http://rest.txtfeedback.net/";
	private String RESTGetHandlersForMessageURL1 = "http://demo.txtfeedback.net/Component/GetHandlerForMessage1";
	private String RESTGetWorkingPointForCertainAddress = "http://demo.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	//private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	private String RESTSaveMessage = "http://demo.txtfeedback.net/Component/SaveMessage";
	private String RESTParametersTest = "http://demo.txtfeedback.net/Component/GetParametersTest";
	private String RESTUpdateClientAcknowledge = "http://demo.txtfeedback.net/Component/UpdateMessageClientAcknowledgeField";
	*/
	
	/* REST resources for product */  
	private String RESTGetHandlersForMessageURL = "http://product.txtfeedback.net/Component/GetHandlerForMessage";
	private String RESTDomain = "http://rest.txtfeedback.net/";
	private String RESTGetHandlersForMessageURL1 = "http://product.txtfeedback.net/Component/GetHandlerForMessage1";
	private String RESTGetWorkingPointForCertainAddress = "http://product.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	//private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	private String RESTSaveMessage = "http://product.txtfeedback.net/Component/SaveMessage";
	private String RESTParametersTest = "http://product.txtfeedback.net/Component/GetParametersTest";
	private String RESTUpdateClientAcknowledge = "http://product.txtfeedback.net/Component/UpdateMessageClientAcknowledgeField";
	
	public ArrayList<Agent> getHandlersForMessage(String iWP, String iConversationId, boolean isSms) throws RESTException {
		try {
			StringBuilder urlSb = new StringBuilder(RESTDomain);
			urlSb.append(iWP);
			urlSb.append("/api/rules/");
			ArrayList<Agent> handlers = new ArrayList<Agent>();
			Hashtable<String, String> params = new Hashtable<String, String>();
			params.put("from", URLEncoder.encode(iConversationId, "UTF-8"));			
			JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(urlSb.toString(), RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
			if (listOfAgentsJsonObject != null) {
				JSONArray listOfAgentsArray = listOfAgentsJsonObject.getJSONArray("agents");
				for (int i=0; i<listOfAgentsArray.length(); ++i) {
					handlers.add(new Agent(listOfAgentsArray.getJSONObject(i).getString("user"), listOfAgentsArray.getJSONObject(i).getInt("priority")));												
				}
			}
			return handlers;
		} catch (JSONException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			throw new RESTException();
		}catch (UnsupportedEncodingException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			throw new RESTException();
		}		
	}
	
	public ArrayList<Agent> getHandlersForMessage1(String iWP, String iConversationId, boolean isSms) {
		ArrayList<Agent> handlers = new ArrayList<Agent>();
		Hashtable<String, String> params = new Hashtable<String, String>();
		try {
			params.put("wp", URLEncoder.encode(iWP, "UTF-8"));
			params.put("convId", URLEncoder.encode(iConversationId, "UTF-8"));
			params.put("isSms", URLEncoder.encode(String.valueOf(isSms), "UTF-8"));
		} catch (UnsupportedEncodingException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		}
		JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(RESTGetHandlersForMessageURL1, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
		if (listOfAgentsJsonObject != null) {
			try {
				JSONArray listOfAgentsArray = listOfAgentsJsonObject.getJSONArray("agents");
				for (int i=0; i<listOfAgentsArray.length(); ++i) {
					handlers.add(new Agent(listOfAgentsArray.getJSONObject(i).getString("user"), listOfAgentsArray.getJSONObject(i).getInt("priority")));												
				}
			} catch (JSONException e) {
				Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			}
		}
		return handlers;
	}
	
	public WorkingPoint getWorkingPointForCertainAddress(String iIMAddress) {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("iIMAddress", String.valueOf(iIMAddress));
		JSONObject wpJsonObject = getResourceAsJsonObject(RESTGetWorkingPointForCertainAddress, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);		
		WorkingPoint wp = null;
		if (wpJsonObject != null) {
			try {
				wp = new WorkingPoint(wpJsonObject.getString("TelNumber"),
				wpJsonObject.getString("Name"),
				wpJsonObject.getString("Description"),
				wpJsonObject.getInt("NrOfSentSmsThisMonth"),
				wpJsonObject.getInt("MaxNrOfSmsToSendPerMonth"));
			} catch (JSONException e) {
				Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			}	
		}		
		return wp;	
	}
	
	public boolean updateMessageClientAcknowledgeField(int msgID, boolean clientAcknowledge) {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("msgID", Integer.toString(msgID));
		params.put("clientAcknowledge", String.valueOf(clientAcknowledge));
		String requestResult = getResourceAsString(RESTUpdateClientAcknowledge, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
		if (requestResult != null) {
			String result = requestResult.toString();
			if (result.equals("success")) return true;
			else return false;			
		}		
		return false;
	}	
	
	// Method used just for testing purposes
	public void sendParameters() {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("id", "7");
		params.put("from","de la mine");
		params.put("to", "mihai");		
		String result = getResourceAsString(RESTParametersTest, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);		
	}
	
	public MessageStatus saveMessage(String from, String to, String convId, String text, String xmppUser, boolean isSms) throws RESTException {
		try {
			Hashtable<String, String> params = new Hashtable<String, String>();
			params.put("from", from);
			params.put("to", to);
			params.put("convId", convId);
			params.put("text", URLEncoder.encode(text, "UTF-8"));
			params.put("xmppUser", xmppUser);
			params.put("isSms", String.valueOf(isSms));
		
			JSONObject restResponse = getResourceAsJsonObject(RESTSaveMessage, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
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
		} catch (UnsupportedEncodingException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			throw new RESTException();
		} catch (Exception e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
			throw new RESTException();			
		}
	}
	
	private JSONObject getResourceAsJsonObject(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) {
		JSONObject wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, iMethod, iParams, iHeaderAccept, iContentType);
		try {
			ri.createObjectJson();
			wpJsonObj = ri.getRecvObject();
		} catch (JSONException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		}
		return wpJsonObj;
	}
	
	private String getResourceAsString(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) {
		String wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, iMethod, iParams, iHeaderAccept, iContentType);
		return ri.getResponse();
	}
	
	private RestClient callRESTResource(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) {
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
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} catch (ClientProtocolException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} catch (IOException e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), LogEntryType.ERROR, Utilities.getStackTrace(e.getCause()));
		} 
		return ri;
	}
	
}
