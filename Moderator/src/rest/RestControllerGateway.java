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
	//private String RESTGetHandlersForMessageURL = "http://dev.txtfeedback.net/Component/GetHandlerForMessage";
	private String RESTGetHandlersForMessageURL = "http://localhost:4631/Component/GetHandlerForMessage";
	private String RESTDomain = "http://rest.txtfeedback.net/";
	//private String RESTGetHandlersForMessageURL1 = "http://dev.txtfeedback.net/Component/GetHandlerForMessage1";
	//private String RESTGetHandlersForMessageURL1 = "http://localhost:4631/Component/GetHandlerForMessage1";
	//private String RESTGetWorkingPointForCertainAddress = "http://dev.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	//private String RESTSaveMessage = "http://dev.txtfeedback.net/Component/SaveMessage";
	//private String RESTParametersTest = "http://dev.txtfeedback.net/Component/GetParametersTest";
	//private String RESTUpdateClientAcknowledge = "http://dev.txtfeedback.net/Component/UpdateMessageClientAcknowledgeField";
	private String RESTUpdateClientAcknowledge = "http://localhost:4631/Component/UpdateMessageClientAcknowledgeField";
 	 */
	 	 
	/* REST resources for nexmo */
	private String RESTGetHandlersForMessageURL = "http://demotxtfeedback.cloudapp.net/Component/GetHandlerForMessage";
	private String RESTDomain = "http://demot3xt.cloudapp.net:81/";
	private String RESTGetHandlersForMessageURL1 = "http://demotxtfeedback.cloudapp.net/Component/GetHandlerForMessage1";
	private String RESTGetWorkingPointForCertainAddress = "http://demotxtfeedback.cloudapp.net/Component/GetWorkingPointForCertainAddress";
	//private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	private String RESTSaveMessage = "http://demotxtfeedback.cloudapp.net/Component/SaveMessage";
	private String RESTParametersTest = "http://demotxtfeedback.cloudapp.net/Component/GetParametersTest";
	private String RESTUpdateClientAcknowledge = "http://demotxtfeedback.cloudapp.net/Component/UpdateMessageClientAcknowledgeField";
		
	/* REST resources for product */  
//	private String RESTGetHandlersForMessageURL = "http://product.txtfeedback.net/Component/GetHandlerForMessage";
//	private String RESTDomain = "http://rest.txtfeedback.net/";
//	private String RESTGetHandlersForMessageURL1 = "http://product.txtfeedback.net/Component/GetHandlerForMessage1";
//	private String RESTGetWorkingPointForCertainAddress = "http://product.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	//private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
//	private String RESTSaveMessage = "http://product.txtfeedback.net/Component/SaveMessage";
//	private String RESTParametersTest = "http://product.txtfeedback.net/Component/GetParametersTest";
//	private String RESTUpdateClientAcknowledge = "http://product.txtfeedback.net/Component/UpdateMessageClientAcknowledgeField";
	
	public ArrayList<Agent> getHandlersForMessage(String iWP, 
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
					handlers.add(new Agent(listOfAgentsArray.getJSONObject(i).getString("user"), 
							listOfAgentsArray.getJSONObject(i).getInt("priority")));												
				}
			}
			return handlers;
		}catch (Exception e) {
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), 
					LogEntryType.ERROR, 
					Utilities.getStackTrace(e.getCause()));
			throw new RESTException();
		}		
	}
	
	
	public boolean updateMessageClientAcknowledgeField(int msgID, 
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
	
	/* Method used just for testing purposes
	public void sendParameters() {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("id", "7");
		params.put("from","de la mine");
		params.put("to", "mihai");		
		String result = getResourceAsString(RESTParametersTest, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);		
	}*/
	
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
			Log.addLogEntry(Utilities.getStackTrace(e.getCause()), 
					LogEntryType.ERROR, 
					Utilities.getStackTrace(e.getCause()));
			throw new RESTException();			
		}
	}
	
	private JSONObject getResourceAsJsonObject(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) throws Exception {
		JSONObject wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, iMethod, iParams, iHeaderAccept, iContentType);
		ri.createObjectJson();
		wpJsonObj = ri.getRecvObject();		
		return wpJsonObj;
	}
	
	private String getResourceAsString(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) throws Exception {
		String wpJsonObj = null;
		RestClient ri  = callRESTResource(iURL, 
				iMethod, 
				iParams, 
				iHeaderAccept, 
				iContentType);
		return ri.getResponse();
	}
	
	private RestClient callRESTResource(String iURL,
			int iMethod, Hashtable<String, String> iParams,
			String iHeaderAccept, String iContentType) throws Exception {
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
		ri.callWebService(iMethod);		
		return ri;
	}
	
}
