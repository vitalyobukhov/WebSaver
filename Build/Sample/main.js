window.onload = function() {

		// set source canvas logical dimensions to fullscreen size
		var canvas = document.getElementById('canvas');
		canvas.width = window.innerWidth;
		canvas.height = window.innerHeight;

		// init plasma effect settings
		var settings = { 
			colorMinVal: 0,
			colorMaxVal: 255,
			speedMinDiv: 20,
			speedMaxDiv: 50,
			timeDiv: 400,
			scaleDiv: 40,
			started: true
		};
		
		var plasma;
		try {
			// demo never dies!
			plasma = new Plasma(window, canvas, settings); 
		}
		catch(ex) {
			// ...except some awful cases
			window.alert(ex.toString());
		}
};
