package rest;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.List;
import java.util.Map.Entry;

import org.apache.http.client.ClientProtocolException;
import org.helpers.Agent;
import org.helpers.Constants;
import org.helpers.WorkingPoint;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class RestControllerGateway {
	private String RESTGetHandlersForMessageURL = "http://localhost:4631/Component/GetHandlerForMessage";
	private String RESTGetWorkingPointForCertainAddress = "http://localhost:4631/Component/GetWorkingPointForCertainAddress";
	private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	private String RESTParametersTest = "http://localhost:4631/Component/GetParametersTest";
	
	public ArrayList<Agent> getHandlersForMessage(String iWP, String iConversationId) {
		ArrayList<Agent> handlers = new ArrayList<Agent>();
		JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(RESTGetHandlersForMessageURL, RestClient.GET, null, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
		if (listOfAgentsJsonObject != null) {
			try {
				JSONArray listOfAgentsArray = listOfAgentsJsonObject.getJSONArray("agents");
				for (int i=0; i<listOfAgentsArray.length(); ++i) {
					handlers.add(new Agent(listOfAgentsArray.getJSONObject(i).getString("user"), listOfAgentsArray.getJSONObject(i).getInt("priority")));												
				}
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		return handlers;
	}
	
	public WorkingPoint getWorkingPointForCertainAddress(String iIMAddress) {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("iIMAddress", iIMAddress);
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
				// TODO: Log this error
				System.out.println(e.getMessage());
			}	
		}		
		return wp;	
	}
	
	// Method used just for testing purposes
	public void sendParameters() {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("id", "7");
		params.put("from","de la mine");
		params.put("to", "mihai");		
		String result = getResourceAsString(RESTParametersTest, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);		
	}
	
	public boolean saveMessage(String from, String to, String convId, String text, String xmppUser, boolean isSms) {
		Hashtable<String, String> params = new Hashtable<String, String>();
		params.put("from", from);
		params.put("to", to);
		params.put("convId", convId);
		params.put("text", text);
		params.put("xmppUser", xmppUser);
		params.put("isSms", String.valueOf(isSms));
		String restCallResponse = getResourceAsString(RESTSaveMessage, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);		
		if (restCallResponse.equals("{success}")) {
			return true;
		} else {
			return false;
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
			// TODO Auto-generated catch block
			e.printStackTrace();
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
			System.out.println(ri.getResponse());
		} catch (IllegalStateException e) {
			e.printStackTrace();
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} 
		return ri;
	}
	
}
