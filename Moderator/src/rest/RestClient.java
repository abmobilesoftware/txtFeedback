package rest;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.io.Writer;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map.Entry;
 
import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
 

 
public class RestClient {
 
	public static final String TAG = "RestClient";
	public static final int POST = 0;
	public static final int GET = 1;
	public static final int JSON = 2;
	public static final int XML = 3;
 
	private String url;
	private String responsePhrase;
	private String response;
	private JSONArray recvArray;
	private JSONObject recvObject;
	private int respCode;
	private HashMap params;
	private HashMap headers;
 
	public RestClient(String restUrl) {
		this.url = restUrl;
		this.params = new HashMap();
		this.headers = new HashMap();
	}
 
	public String getResponse() {
		return this.response;
	}
	
	public JSONObject getRecvObject() {
		return recvObject;
	}
	
	public JSONArray getRecvArray() {
		return recvArray;
	}
 
	public String getErrorMsg() {
		return this.responsePhrase;
	}
 
	public int getResponseCode(){
		return this.respCode;
	}
 
	public void addParam(String key, String value){
		this.params.put(key, value);
	}
 
	public void addHeader(String key, String value){
		this.headers.put(key, value);
	}
 
	public String buildParams() throws UnsupportedEncodingException {
		Iterator<Entry> it = params.entrySet().iterator();
		String res = "?";
		while (it.hasNext()) {
			Entry pair = it.next();
			String add = pair.getKey() + "="
					+ URLEncoder.encode(pair.getValue().toString(), "UTF-8");
			if (params.size() > 1) {
				res += "&" + add;
			} else {
				res += add;
			}
		}
		return res;
	}
 
	public String convertStreamToString(InputStream is) throws IOException  {
		if (is != null) {
			Writer writer = new StringWriter();
			char[] buffer = new char[1024];
			try {
				Reader reader = new BufferedReader(new InputStreamReader(is,
						"UTF-8"));
				int n;
				while ((n = reader.read(buffer)) != -1) {
					writer.write(buffer, 0, n);
				}
			} catch (UnsupportedEncodingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} finally {
				is.close();
			}
			return writer.toString();
		} else {
			return "";
		}
	}
 
	public void sendHttpPost() throws ClientProtocolException, IOException{
		HttpPost httpPostRequest = new HttpPost(url + buildParams());
 
		// add headers
		Iterator<Entry> it = headers.entrySet().iterator();
		while (it.hasNext()) {
			Entry header = it.next();
			httpPostRequest.addHeader(header.getKey().toString(), header.getValue().toString());
		}
 
		HttpClient client = new DefaultHttpClient();
		HttpResponse resp;
 
		resp = client.execute(httpPostRequest);
 
		this.respCode = resp.getStatusLine().getStatusCode();
		this.responsePhrase = resp.getStatusLine().getReasonPhrase();
		HttpEntity entity = resp.getEntity();
 
		if (entity != null){
			InputStream is = entity.getContent();
			//Header contentEncoding = resp.getFirstHeader("Content-encoding");
			//Log.i(TAG, "endoding" + contentEncoding.getValue());
			response = convertStreamToString(is);
			//response = response.substring(1,response.length()-1);
			//response = "{" + response + "}";
			is.close();
		}
	}
 
	public void callWebService(int method) throws IllegalStateException, ClientProtocolException, IOException {
		switch (method) {
		case GET:
			// build correct url
			HttpGet request = new HttpGet(url + buildParams());
 
			// add headers
			Iterator<Entry> it = headers.entrySet().iterator();
			while (it.hasNext()) {
				Entry header = it.next();
				request.addHeader(header.getKey().toString(), header.getValue().toString());
			}
 
			// exec request
			HttpClient client = new DefaultHttpClient();
			HttpResponse resp;
 
			resp = client.execute(request);
			this.respCode = resp.getStatusLine().getStatusCode();
			this.responsePhrase = resp.getStatusLine().getReasonPhrase();
 
			HttpEntity entity = resp.getEntity();
 
			if (entity != null) {
				InputStream is = entity.getContent();
				response = this.convertStreamToString(is);
				response = response.substring(1,response.length()-1);
				response = "{" + response + "}";
				is.close();
			}
 
			break;
		case POST:
			sendHttpPost();
		}
	}
 
	public void createObjectJson() throws JSONException{
		this.recvObject = new JSONObject(this.response);
	}
	
	public void createArrayOfObjects() throws JSONException{
		this.recvArray = new JSONArray(this.response);
	}
}