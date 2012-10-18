package org.helpers.json;

public class WorkingPoint {
	public String telNumber;
	public String description;
	public String name;
	public String provider;
	public int sentSms;
	public int maxNrOfSms;
	
	public WorkingPoint() {}
	public WorkingPoint(String iTelNumber, String iDescription, 
			String iName, int iSentSms, 
			int iMaxNrOfSms) {		
		telNumber = iTelNumber;
		description = iDescription;
		name = iName;
		sentSms = iSentSms;
		maxNrOfSms = iMaxNrOfSms;
	}
	
	public String getTelNumber() {
		return telNumber;
	}
	public void setTelNumber(String telNumber) {
		this.telNumber = telNumber;
	}
	public String getDescription() {
		return description;
	}
	public void setDescription(String description) {
		this.description = description;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getProvider() {
		return provider;
	}
	public void setProvider(String provider) {
		this.provider = provider;
	}
	public int getSentSms() {
		return sentSms;
	}
	public void setSentSms(int sentSms) {
		this.sentSms = sentSms;
	}
	public int getMaxNrOfSms() {
		return maxNrOfSms;
	}
	public void setMaxNrOfSms(int maxNrOfSms) {
		this.maxNrOfSms = maxNrOfSms;
	}
}
