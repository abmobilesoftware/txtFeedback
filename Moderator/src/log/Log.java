package log;

import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.util.Date;

import log.db.LogGateway;

public class Log {
	/* 
	 * Storage mode -> current issue:
	 * java.sql.SQLException: Communication link failure: java.io.EOFException, underlying cause: null
	 */
	private static StorageMode storageMode = StorageMode.FILE;
	private static final String dateFormat = "dd-MMM-yy";
	private static SimpleDateFormat  formatter = new SimpleDateFormat(dateFormat);;
	private static LogGateway logGtw = new LogGateway();
	
	private static void addLogEntryInDB(String entry, String type, String details) {
		try {			
			logGtw.insertLogEntry(entry, type, details);
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	private static void printLogEntryInConsole(String entry, String type, String details) {
		System.out.println(formatter.format(new Date()) + " ::: " + type + ": " + entry + " ::: Details: " + details);
	}
	
	public static void addLogEntry(String entry, String type) {
		String details = "no details";
		if (storageMode.equals(StorageMode.DB)) {
			addLogEntryInDB(entry, type, details);
		} else if (storageMode.equals(StorageMode.FILE)) {
			printLogEntryInConsole(entry, type, details);
		} else if (storageMode.equals(StorageMode.MIXT)) {
			addLogEntryInDB(entry, type, details);
			printLogEntryInConsole(entry, type, details);
		}
	}
	
	public static void addLogEntry(String entry, String type, String details) {
		if (storageMode.equals(StorageMode.DB)) {
			addLogEntryInDB(entry, type, details);
		} else if (storageMode.equals(StorageMode.FILE)) {
			printLogEntryInConsole(entry, type, details);
		} else if (storageMode.equals(StorageMode.MIXT)) {
			addLogEntryInDB(entry, type, details);
			printLogEntryInConsole(entry, type, details);
		}
	}
	
	private static void addLogEntry(String entry, String type, StorageMode mode) {
		String details = "no details";
		StorageMode oldStorageMode = storageMode;
		storageMode = mode;
		if (storageMode.equals(StorageMode.DB)) {
			addLogEntryInDB(entry, type, details);
		} else if (storageMode.equals(StorageMode.FILE)) {
			printLogEntryInConsole(entry, type, details);
		} else if (storageMode.equals(StorageMode.MIXT)) {
			addLogEntry(entry, type, details);
			printLogEntryInConsole(entry, type, details);
		}
		storageMode = oldStorageMode;
	}
	
	private static void addLogEntry(String entry, String type, String details, StorageMode mode) {
		StorageMode oldStorageMode = storageMode;
		storageMode = mode;
		if (storageMode.equals(StorageMode.DB)) {
			addLogEntryInDB(entry, type, details);
		} else if (storageMode.equals(StorageMode.FILE)) {
			printLogEntryInConsole(entry, type, details);
		} else if (storageMode.equals(StorageMode.MIXT)) {
			addLogEntry(entry, type, details);
			printLogEntryInConsole(entry, type, details);
		}
		storageMode = oldStorageMode;
	}
	
	public static void setStorageMode(StorageMode mode) {
		storageMode = mode;
	}
	
	public StorageMode getStorageMode() {
		return storageMode;
	}
}

enum StorageMode {
	DB,
	FILE,
	MIXT
}