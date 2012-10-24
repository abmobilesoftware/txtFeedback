package org.helpers;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.io.Writer;

public class Utilities {
	/* 
	 * Helper methods 
	 */
	public static String createConvId(TxtPacket iInternalPacket) {
		String conversationId = ""; 
		if (iInternalPacket.getIsSms()) {
			conversationId = cleanupPhoneNumber(iInternalPacket.getFromAddress()) + "-" + cleanupPhoneNumber(iInternalPacket.getToAddress());	
		} else {
			// ConversationId is SomGUID@txtfeedback.net-wp1@lidl.txtfeedback.net
			conversationId = iInternalPacket.getFromAddress() + "-" + iInternalPacket.getToAddress();			
		}
		return conversationId;
	}
	
	public static String cleanupPhoneNumber(String phoneNumber) {
		//take into account that they could start with + or 00 - so we strip away any leading + or 00
		String transformedString;
		String pattern1 = "^00";
		String pattern2 = "^+";
		// delete 00
		transformedString = phoneNumber.replaceAll(pattern1, "");
		// delete +
		transformedString = transformedString.replaceAll(pattern2, "");
		return transformedString;
	}
	
	public static String extractUserFromAddress(String address) {
		return address.substring(0,address.indexOf('@'));
	}
	
	public static String getStackTrace(Throwable aThrowable) {
		final Writer result = new StringWriter();
		final PrintWriter printWriter = new PrintWriter(result);
		aThrowable.printStackTrace(printWriter);
		return result.toString();
	}

}
