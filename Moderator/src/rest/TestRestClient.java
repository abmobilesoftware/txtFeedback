package rest;

import java.io.IOException;

import org.apache.http.client.ClientProtocolException;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class TestRestClient {
	public static void main(String args[]) {
		TestRestClient.testParametersSending();
	}
	
	public static void testParametersSending() {
		RestControllerGateway restGtw = new RestControllerGateway();
		restGtw.sendParameters();
	}
	
	public static void testRestClient() {
		RestClient ri = new RestClient("http://localhost:4631/Component/GetHandlerForMessage");
		//ri.addParam("httpcontext", "application/json");
		ri.addHeader("Accept", "application/json");
		ri.addHeader("Content-type", "application/json");
		try {
			ri.callWebService(RestClient.GET);
			System.out.println(ri.getResponse());
		} catch (IllegalStateException e) {
			e.printStackTrace();
		} catch (ClientProtocolException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		try {
			
			ri.createObjectJson();
			JSONObject myObj = ri.getRecvObject();
			System.out.println(myObj.get("agents"));
			
			
			JSONArray myArray = myObj.getJSONArray("agents");
			System.out.println(myArray.getJSONObject(0).get("user"));
		} catch (JSONException e) {
			e.printStackTrace();
		}
	}
}
