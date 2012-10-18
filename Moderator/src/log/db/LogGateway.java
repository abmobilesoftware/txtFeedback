package log.db;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

public class LogGateway {
	private Connection dbCon = null;
        	
	public LogGateway() {
		dbCon = Database.getSingletonInstance().getConnection();
    }
	
	public ResultSet selectLogEntryByType(String type) throws SQLException {
	    String createString = "SELECT * FROM log WHERE type='" + type + "'";
	    Statement stmt = null;
	    stmt = dbCon.createStatement();	   
	    ResultSet rs = stmt.executeQuery(createString);
	    return rs;
	 }	
        
     public void insertLogEntry(String entry, String type, String details) throws SQLException {
        String createString = "INSERT INTO log (entry, type, details) VALUES('" + entry + "','" + type + "','" + details + "')";
	    Statement stmt = null;
	    stmt = dbCon.createStatement();
	    stmt.executeUpdate(createString);
	 }
         
     public boolean deleteLogEntry(int id)  {
	    String createString = "DELETE FROM log WHERE id='"+id+"'";
	    Statement stmt = null;
	    try {
                stmt = dbCon.createStatement();
                stmt.executeUpdate(createString);
                return true;
            }catch (SQLException e) {
                return false;
            }           
	}            
         
}
