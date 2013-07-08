/**
 * Change all img tags to reflect what device we are on.
 * 
 * To add an alternative image resource based on platform and/or screen size,
 * consider this example:
 * 
 * <img src="image_0.png" data-xhdpi="image_1.png"
 * data-android-xhdpi="image_2.png" data-ios="image_3.png"/>
 * 
 * The above example will use image_0.png as the default image. If we run on a
 * xhdpi display it will use image_1.png unless it is an android device in which
 * case image_2.png is used. If we are using iOS, image_3.png will be used even
 * though it is an xhdpi device since platform has priority.
 * 
 * So, to define an alternative image resource, add a data-xxxx-yyyy-zzzz
 * attribute with a value pointing to the file location. xxxx, yyyy and zzzz may
 * be a platform (eg 'android'), a screen density (eg 'hdpi'), or a screen size
 * (eg 'large'). The order of these is irrelevant, but if a device would match
 * more than one of these alternative image resources, this is ths order of
 * precedence: 1. platform, 2. screen density, 3. screen size.
 */

(function() {	
	var platform = "";

	// Platforms
	if (mosync.isIOS) {
		platform = "ios";
	}
	if (mosync.isAndroid) {
		platform = "android";
	}
	
	if (mosync.isWindowsPhone) {
		platform = "wp";
	}
	
	var platformScoring = function(actualPlatform, test) {
		return actualPlatform == test ? 1 : 0;
	};
	
	// Screen density.
	// 1 = low/medium density (ldpi/mdpi)
	// 1.5 = high density (hdpi)
	// 2 = extra high density (xhdpi)
	var getPixelDevicePixelRatio = function() {
		return window.devicePixelRatio === undefined ? 1 : window.devicePixelRatio;
	};
	
	var getDips = function() {
		// This is how it works on Android...
		if (mosync.isAndroid) {
		    return window.screen.width / mosync.getPixelDevicePixelRatio();
		} else if (mosync.isIOS) {
			// ...and this on iOS.
			return window.screen.width;
		}
		// We don't support anything else:
		return 0;
	}
	
	var densityOrdinals = new Object();
	densityOrdinals.ldpi = 0.5; // Artifical value for smallish screens
	densityOrdinals.mdpi = 1.0;
	densityOrdinals.hdpi = 1.5;
	densityOrdinals.xhdpi = 2.0;
	
	var density = getPixelDevicePixelRatio();
	// Special case: low-res; assume pixel/device pixel ratio = 1.
	if (window.screen.width <= 426 && getPixelDevicePixelRatio() == 1) {
		density = densityOrdinals.ldpi;
	}
	
	var densityScoring = function(actualDensity, test) {
		// The actual density is a number, but we'll
		// get ldpi, mdpi, etc in the attribute:
		var testDensity = densityOrdinals[test];
		if (testDensity !== undefined && actualDensity >= testDensity) {
			return 2 * (actualDensity - testDensity) + 1;
		}
	}
	
	var sizeOrdinals = new Object();
	sizeOrdinals.small = 426;
	sizeOrdinals.medium = 470;
	sizeOrdinals.large = 640;
	sizeOrdinals.xlarge = 960;
	
	var screenSize = getDips();
	
	var variables = [ platform, density, screenSize ];
	
	var screenSizeScoring = function(actualSize, test) {
		// The actual size is a number, whereas we'll get
		// the attribute as 'large', 'small', etc.
		var testOrdinal = sizeOrdinals[test];
		
		if (typeof(testOrdinal) !== undefined && actualSize >= testOrdinal) {
			// Must return something > 0.
			return actualSize - testOrdinal + 1;
		}
		
		return 0;
	}
	
	var scoringFns = [ platformScoring, densityScoring, screenSizeScoring ];

	var rewriteImgTags = function() {
		var imageTags = document.getElementsByTagName("img");

		for (var i=0; i < imageTags.length; i++) {
			var imageTag = imageTags[i];
			var bestMatchPriority = 0;	
			var bestMatchScore = 0;
			var matchAttr = null;
			var newSrc = null;
			var attrs = imageTag.attributes;
			$.each(attrs, function(ix, attr) {
				var name = attr.name;
				var components = name.split("-");
				if (components.length > 1 && components[0] == "data") {
					var matchScore = 0;
					var matchPriority = 0;
					var match = true;
					for (var j = 1; j < components.length; j++) {
						var component = components[j];
						var score = 0;
						var variableMatch = false;
						for (var k = 0; k < variables.length; k++) {
							var variablePriority = 0;
							var variable = variables[k];
							var scoringFn = scoringFns[k];
							score = scoringFn(variable, component);
							if (score > 0) {
								variableMatch = true;
								matchScore += score;
								variablePriority = 1;
							}
							matchPriority <<= 1;
							matchPriority |= variablePriority;
						}
						match = match && variableMatch;
					}
					if (match && matchPriority >= bestMatchPriority) {
						if (matchPriority >= bestMatchPriority ||
							matchScore > bestMatchScore) {
							bestMatchScore = matchScore;
							bestMatchPriority = matchPriority;
							newSrc = attr.value;
						}
					}
				}
			});
			if (newSrc) {
				imageTag.src = newSrc;
			}
		}
	};

	$(document).ready(rewriteImgTags);
})();