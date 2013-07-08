(function() {
	window.onload = function() {
		var canvas = document.getElementById('canvas');
		canvas.width = canvas.parentNode.clientWidth;
		canvas.height = canvas.parentNode.clientHeight;

		var settings = { 
			speedMin: 1,
			speedMax: 4,
			scaleDiv: 40,
			started: true
		};
		
		try {
			// demo never dies!
			var plasma = new Plasma(window, canvas, settings); 
		}
		catch(ex) {
			// ...except some awful cases
			window.alert(ex.toString());
		}
	};
})();
