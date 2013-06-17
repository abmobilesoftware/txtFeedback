package org.helpers.json;
import java.util.*;

public class Agent {
	public String user;
	public int priority;
	public ArrayList<Device> devices;
	
	public Agent(String iUser, ArrayList iDevices, int iPriority) {
		user = iUser;
		devices = iDevices;
		priority = iPriority;
	}
	
	public String getUser() {
		return user;
	}

	public void setUser(String user) {
		this.user = user;
	}

	public int getPriority() {
		return priority;
	}

	public void setPriority(int priority) {
		this.priority = priority;
	}
}
