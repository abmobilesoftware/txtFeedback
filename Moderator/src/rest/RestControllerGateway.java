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

import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.utils.URIBuilder;
import org.helpers.Constants;
import org.helpers.json.Agent;
import org.helpers.json.WorkingPoint;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class RestControllerGateway {
	private String RESTGetHandlersForMessageURL = "http://dev.txtfeedback.net/Component/GetHandlerForMessage";
	private String RESTDomain = "http://rest.txtfeedback.net/";
	private String RESTGetHandlersForMessageURL1 = "http://dev.txtfeedback.net/Component/GetHandlerForMessage1";
	private String RESTGetWorkingPointForCertainAddress = "http://dev.txtfeedback.net/Component/GetWorkingPointForCertainAddress";
	//private String RESTSaveMessage = "http://localhost:4631/Component/SaveMessage";
	private String RESTSaveMessage = "http://dev.txtfeedback.net/Component/SaveMessage";
	private String RESTParametersTest = "http://dev.txtfeedback.net/Component/GetParametersTest";
	
	public ArrayList<Agent> getHandlersForMessage(String iWP, String iConversationId, boolean isSms) {
		StringBuilder urlSb = new StringBuilder(RESTDomain);
		urlSb.append(iWP);
		urlSb.append("/api/rules/");
		ArrayList<Agent> handlers = new ArrayList<Agent>();
		Hashtable<String, String> params = new Hashtable<String, String>();
		try {
			params.put("from", URLEncoder.encode(iConversationId, "UTF-8"));			
		} catch (UnsupportedEncodingException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		System.out.println("URL=" + urlSb.toString());
		JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(urlSb.toString(), RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
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
	
	public ArrayList<Agent> getHandlersForMessage1(String iWP, String iConversationId, boolean isSms) {
		ArrayList<Agent> handlers = new ArrayList<Agent>();
		Hashtable<String, String> params = new Hashtable<String, String>();
		try {
			params.put("wp", URLEncoder.encode(iWP, "UTF-8"));
			params.put("convId", URLEncoder.encode(iConversationId, "UTF-8"));
			params.put("isSms", URLEncoder.encode(String.valueOf(isSms), "UTF-8"));
		} catch (UnsupportedEncodingException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		JSONObject listOfAgentsJsonObject = getResourceAsJsonObject(RESTGetHandlersForMessageURL1, RestClient.GET, params, Constants.APPLICATION_JSON, Constants.APPLICATION_JSON);
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
			//System.out.println(ri.getResponse());
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
