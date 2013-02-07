package org.helpers.json;

public class MessageStatus {
	private int MessageID;
	private boolean MessageSent;
	private boolean WarningLimitReached;
	private boolean SpendingLimitReached;
	private String Reason;
	private String JsonFormat;
	
	public MessageStatus(int messageID, 
			boolean messageSent, 
			boolean warningLimitReached,
			boolean spendingLimitReached, 
			String reason, 
			String jsonFormat) {
		MessageID = messageID; 
		MessageSent = messageSent;
		WarningLimitReached = warningLimitReached;
		SpendingLimitReached = spendingLimitReached;
		Reason = reason;
		JsonFormat = jsonFormat;
	}

	public int getMessageID() {
		return MessageID;
	}

	public void setMessageID(int messageID) {
		MessageID = messageID;
	}

	public String getJsonFormat() {
		return JsonFormat;
	}

	public void setJsonFormat(String jsonFormat) {
		this.JsonFormat = jsonFormat;
	}

	public boolean isMessageSent() {
		return MessageSent;
	}

	public void setMessageSent(boolean messageSent) {
		MessageSent = messageSent;
	}

	public boolean isWarningLimitReached() {
		return WarningLimitReached;
	}

	public void setWarningLimitReached(boolean warningLimitReached) {
		WarningLimitReached = warningLimitReached;
	}

	public boolean isSpendingLimitReached() {
		return SpendingLimitReached;
	}

	public void setSpendingLimitReached(boolean spendingLimitReached) {
		SpendingLimitReached = spendingLimitReached;
	}

	public String getReason() {
		return Reason;
	}

	public void setReason(String reason) {
		Reason = reason;
	}

	
}
