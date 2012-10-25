package org.helpers.tests;

import static org.junit.Assert.*;

import org.helpers.Utilities;
import org.junit.Test;

public class UtilitiesTest {

	@Test
	public void testExtractUserFromAddress() {
		String address = "mike-pc@txtfeedback.net";
		String user = "mike-pc";
		assertEquals(user, Utilities.extractUserFromAddress(address));
	}

}
