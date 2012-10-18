package log.db;

import java.sql.Connection;
import java.sql.DriverManager;

public class Database {
	private static Database db;
	private Connection con = null;
        private String status = "Failed";
	
	private Database() {
	    try {
	    	Class.forName("com.mysql.jdbc.Driver").newInstance();
		    con = DriverManager.getConnection("jdbc:mysql://89.38.209.15:3306/txtcomponent", "txtuser", "txt123!");
		      if(!con.isClosed()) {
                          status = "Successfully connected to " + "MySQL server using TCP/IP...";
                          System.out.println("Successfully connected to " + "MySQL server using TCP/IP...");
                      }
            } catch (Exception e) {
               	System.err.println("Exception: " + e.getMessage());
            } 
	}
	
	public static synchronized Database getSingletonInstance() {
		if (db == null) {
			db = new Database();
		}
		return db;
	}
	
	public Object clone() throws CloneNotSupportedException {
            throw new CloneNotSupportedException();
	}
	
	public Connection getConnection() {
            return con;
	}
       
        public String getStatus() {
            return status;
        }
}
