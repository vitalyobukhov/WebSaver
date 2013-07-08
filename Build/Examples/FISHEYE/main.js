(function() {
	window.onload = function() {
		var canvas = document.getElementById('canvas');
		canvas.width = canvas.parentNode.clientWidth;
		canvas.height = canvas.parentNode.clientHeight;

		var settings = { 
			draw: true
		};
		
		try {
			var checkers = new Checkers(canvas, settings);
		}
		catch(ex) {
			window.alert(ex.toString());
			return;
		}
		
		settings = {
			ratioA: -0.25,
			ratioB: -0.5,
			ratioC: -2,
			speedMin: 1,
			speedMax: 2.5,
			started: true
		};
		
		try {
			var fisheye = new Fisheye(window, canvas, settings);
		}
		catch(ex) {
			window.alert(ex.toString());
		}
	};
})();