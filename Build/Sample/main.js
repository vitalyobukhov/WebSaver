window.onload = function() {

		var canvas = document.getElementById('canvas');
		canvas.width = window.innerWidth;
		canvas.height = window.innerHeight;

		var settings = { 
			colorMinVal: 0,
			colorMaxVal: 255,
			speedMinDiv: 20,
			speedMaxDiv: 50,
			timeDiv: 400,
			scaleDiv: 50 
		};
		
		var plasma = new Plasma(window, canvas, settings); 
		plasma.start();
};
