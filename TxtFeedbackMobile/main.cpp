/**
 * @file main.cpp
 *
 * This file contains the support code at the C++ level for
 * an HTML5/JS application that has access to device services.
 *
 * You don't need to change anything in this code file unless
 * you wish to add support for functions that are not available
 * out-of-the box in wormhole.js.
 */

#include <Wormhole/HybridMoblet.h>

#include "MAHeaders.h" // Defines BEEP_WAV

// Namespaces we want to access.
using namespace NativeUI; // WebView widget.
using namespace Wormhole; // Wormhole library.

/**
 * The application class.
 */
class MyMoblet : public HybridMoblet
{
public:
	MyMoblet()
	{
		maNotificationPushSetDisplayFlag(true);
		maNotificationPushSetTickerText("New message");
		maNotificationPushSetMessageTitle("New message");
		showSplashScreen();
		showPage("index.html");

		// Set the sound used by the PhoneGap beep notification API.
		// BEEP_WAV is defined in file Resources/Resources.lst.
		setBeepSound(BEEP_WAV);
		maScreenSetOrientation(SCREEN_ORIENTATION_DYNAMIC);
		maScreenSetSupportedOrientations(MA_SCREEN_ORIENTATION_LANDSCAPE_LEFT |
		  MA_SCREEN_ORIENTATION_LANDSCAPE_RIGHT | MA_SCREEN_ORIENTATION_PORTRAIT |
		  MA_SCREEN_ORIENTATION_PORTRAIT_UPSIDE_DOWN);
	};
	void showSplashScreen()
	    {
	        // Compute coordinates to center image.
	        int screenSize = maGetScrSize();
	        int screenWidth = EXTENT_X(screenSize);
	        int screenHeight = EXTENT_Y(screenSize);

	        int imageSize = maGetImageSize(SPLASH_SCREEN);
	        int imageWidth = EXTENT_X(imageSize);
	        int imageHeight = EXTENT_Y(imageSize);

	        int imageX = (screenWidth - imageWidth) / 2;
	        int imageY = (screenHeight - imageHeight) / 2;

	        // Fill background.
	        maSetColor(0x000000);
	        maFillRect(0, 0, screenWidth, screenHeight);

	        // Draw image centered.
	        maDrawImage(SPLASH_SCREEN, imageX, imageY);

	        // Display updates.
	        maUpdateScreen();
	    }
};

/**
 * Main function that is called when the program starts.
 * Here an instance of the MyMoblet class is created and
 * the program enters the main event loop.
 */
extern "C" int MAMain()
{
	(new MyMoblet())->enterEventLoop();
	return 0;
}