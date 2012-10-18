package org.helpers.json;

public class Agent {
	public String user;
	public int priority;
	
	public Agent(String iUser, int iPriority) {
		user = iUser;
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
