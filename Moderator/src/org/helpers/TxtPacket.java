package org.helpers;

import java.io.StringReader;
import java.io.StringWriter;
import java.util.Date;
import java.util.Locale;
import java.util.TimeZone;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.w3c.dom.Text;
import org.xml.sax.InputSource;

public class TxtPacket {
	private static final String FROM_ADDRESS = "from";
	private static final String TO_ADDRESS = "to";
	private static final String DATE_SENT = "datesent";
	private static final String BODY = "body";
	private static final String STAFF = "staff";
	private static final String SMS = "sms";
	private static final String CONVERSATION_ID = "convID";
	private ArrayList<String> mTxtPacketTagsName = new ArrayList<String>();
	private ArrayList<String> mTxtPacketTagsValue = new ArrayList<String>();
	private DocumentBuilderFactory mDbfac = null;
    private DocumentBuilder mDocBuilder = null;
	
    private final int FROM_ADDRESS_POS = 0;
    private final int TO_ADDRESS_POS = FROM_ADDRESS_POS + 1;
    private final int CONVERSATION_ID_POS = TO_ADDRESS_POS + 1;
    private final int BODY_POS = CONVERSATION_ID_POS + 1;
    private final int STAFF_POS = BODY_POS + 1;
    private final int SMS_POS = STAFF_POS + 1;
    private final int DATE_SENT_POS = SMS_POS + 1;
    
    //private static final String dateFormat = "dd-MMM-yy";
	final static String ISO_FORMAT = "EEE MMM dd yyyy, HH:mm:ss";
	public TxtPacket(String iFrom, String iTo, Date iDate, String iBody, boolean iStaff, boolean iSms, String iConversationId) {
		initialize();
		Locale enUS = new Locale("en", "US");
		SimpleDateFormat formatter = new SimpleDateFormat(ISO_FORMAT,enUS);
		final TimeZone utc = TimeZone.getTimeZone("UTC");
		formatter.setTimeZone(utc);
		
		mTxtPacketTagsName.add(FROM_ADDRESS_POS, FROM_ADDRESS);
		mTxtPacketTagsValue.add(FROM_ADDRESS_POS, iFrom);
		
		mTxtPacketTagsName.add(TO_ADDRESS_POS,TO_ADDRESS);
		mTxtPacketTagsValue.add(TO_ADDRESS_POS, iTo);
				
		mTxtPacketTagsName.add(CONVERSATION_ID_POS, CONVERSATION_ID);
		mTxtPacketTagsValue.add(CONVERSATION_ID_POS, iConversationId);
		
		mTxtPacketTagsName.add(BODY_POS, BODY);
		mTxtPacketTagsValue.add(BODY_POS, iBody);
		
		mTxtPacketTagsName.add(STAFF_POS, STAFF);
		mTxtPacketTagsValue.add(STAFF_POS, String.valueOf(iStaff));
		
		mTxtPacketTagsName.add(SMS_POS, SMS);
		mTxtPacketTagsValue.add(SMS_POS, String.valueOf(iSms));
		
		mTxtPacketTagsName.add(DATE_SENT_POS, DATE_SENT);
		mTxtPacketTagsValue.add(DATE_SENT_POS, formatter.format(iDate));		
		
	}
	
	public TxtPacket(String iXmlContent) {
		initialize();
		
		try {
			InputSource lIs = new InputSource();
		    lIs.setCharacterStream(new StringReader(iXmlContent));

		    Document lXmlDoc = mDocBuilder.parse(lIs);
		        
		    Node lRoot = lXmlDoc.getFirstChild();
		    NodeList lChildNodes = lRoot.getChildNodes();
		        
		    int k = 0;
		    for (int i = 0; i < lChildNodes.getLength(); i++) {
		    	Node lNode = (Node) lChildNodes.item(i);
		        if (!lNode.getNodeName().equals("#text")) {
		        	++k;
		        	mTxtPacketTagsName.add(i-k, lNode.getNodeName());
		        	mTxtPacketTagsValue.add(i-k, lNode.getTextContent());
		        	//System.out.println((i-k) + "." + "name=" + lNode.getNodeName());
		        	//System.out.println((i-k) + "." + "value=" + lNode.getTextContent());
		        }
		                 
		    }
		 }
		 catch (Exception e) {
			 System.out.println("XML bad formatted - TxtPacket(String iXmlContent)   - " + iXmlContent);			 
		 }
	}
	
	public void initialize() {
		mDbfac = DocumentBuilderFactory.newInstance();
	    try {
			mDocBuilder = mDbfac.newDocumentBuilder();
		} catch (ParserConfigurationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public String toXML() {
		String xmlString = "";
		try {
                       
            Document lXmlDoc = mDocBuilder.newDocument();

            Element lPartyNode = lXmlDoc.createElement("msg");
            lXmlDoc.appendChild(lPartyNode);

            for (int i=0; i<mTxtPacketTagsName.size(); ++i) {
            	Element lNode = lXmlDoc.createElement(mTxtPacketTagsName.get(i));
                lPartyNode.appendChild(lNode);
            	
                //add text in this node
                Text lText = lXmlDoc.createTextNode(mTxtPacketTagsValue.get(i));
                lNode.appendChild(lText);            	
            }
            
            TransformerFactory lTransfac = TransformerFactory.newInstance();
            Transformer lTrans = lTransfac.newTransformer();
            lTrans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
            lTrans.setOutputProperty(OutputKeys.INDENT, "yes");

            //create string from xml tree
            StringWriter lSw = new StringWriter();
            StreamResult result = new StreamResult(lSw);
            DOMSource source = new DOMSource(lXmlDoc);
            lTrans.transform(source, result);
            xmlString = lSw.toString();

		} catch (Exception e) {
            System.out.println("toXML exception = " + e);
        }
		return xmlString;
	}
	
	/* Getters & setters for class attributes */	
	public String getFromAddress() {
		return mTxtPacketTagsValue.get(FROM_ADDRESS_POS);		
	}
	
	public void setFromAddress(String iFromAddress) {
		mTxtPacketTagsName.set(FROM_ADDRESS_POS, FROM_ADDRESS);
		mTxtPacketTagsValue.set(FROM_ADDRESS_POS, iFromAddress);
	}
	
	public String getToAddress() {
		return mTxtPacketTagsValue.get(TO_ADDRESS_POS);		
	}
	
	public void setToAddress(String iToAddress) {
		mTxtPacketTagsName.set(TO_ADDRESS_POS, TO_ADDRESS);
		mTxtPacketTagsValue.set(TO_ADDRESS_POS, iToAddress);
	}
		
	public Date getDateSent() {
		Locale enUS = new Locale("en", "US");
		SimpleDateFormat formatter = new SimpleDateFormat(ISO_FORMAT,enUS);
		final TimeZone utc = TimeZone.getTimeZone("UTC");
		formatter.setTimeZone(utc);
		
		Date formattedDate = null;
		try {
			formattedDate = (Date)formatter.parse(mTxtPacketTagsValue.get(DATE_SENT_POS));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		} catch (IndexOutOfBoundsException e) {
			return null;
		}
		return formattedDate;		
	}	

	public void setDateSent(Date iDateSent) {
		mTxtPacketTagsName.set(DATE_SENT_POS, DATE_SENT);
		Locale enUS = new Locale("en", "US");
		SimpleDateFormat formatter = new SimpleDateFormat(ISO_FORMAT,enUS);		
		final TimeZone utc = TimeZone.getTimeZone("UTC");
		formatter.setTimeZone(utc);		
		mTxtPacketTagsValue.set(DATE_SENT_POS, formatter.format(iDateSent));
	}

	public String getBody() {
		try {
			return mTxtPacketTagsValue.get(BODY_POS);
		}catch (IndexOutOfBoundsException e) {
			return null;
		}
	}
	
	public void setBody(String iBody) {
		mTxtPacketTagsName.set(BODY_POS, BODY);
		mTxtPacketTagsValue.set(BODY_POS, iBody);
	}
	
	public boolean getIsForStaff() {
		try {
			return Boolean.parseBoolean(mTxtPacketTagsValue.get(STAFF_POS));
		}catch (IndexOutOfBoundsException e) {
			return false;
		}
	}
	
	public void setIsForStaff(boolean iIsForStaff) {
		mTxtPacketTagsName.set(STAFF_POS, STAFF);
		mTxtPacketTagsValue.set(STAFF_POS, String.valueOf(iIsForStaff));
	}
	
	public boolean getIsSms() {
		try {
			return Boolean.parseBoolean(mTxtPacketTagsValue.get(SMS_POS));
		}catch (IndexOutOfBoundsException e) {
			return false;
		}
	}
	
	public void setIsSms(boolean iIsSms) {
		mTxtPacketTagsName.set(SMS_POS, SMS);
		mTxtPacketTagsValue.set(SMS_POS, String.valueOf(iIsSms));
	}
	
	public String getConversationId() {
		return mTxtPacketTagsValue.get(CONVERSATION_ID_POS);		
	}
	
	public void setConversationId(String iConversationId) {
		mTxtPacketTagsName.set(CONVERSATION_ID_POS, CONVERSATION_ID);
		mTxtPacketTagsValue.set(CONVERSATION_ID_POS, iConversationId);
	}
}