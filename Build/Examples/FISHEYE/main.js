(function() {
	window.onload = function() {

		var canvas = document.getElementById('checkers');
		canvas.width = canvas.parentNode.clientWidth;
		canvas.height = canvas.parentNode.clientHeight;

		var settings = { 
			firstColor: 'black',
			secondColor: 'white',
			minCount: 16,
			draw: true
		};
		
		try {
			var checkers = new Checkers(canvas, settings);
			checkers.draw();
		}
		catch(ex) {
			window.alert(ex.toString());
			return;
		}
		
		settings = {
			ratioA: -0.25,
			ratioB: -0.5,
			ratioC: -2,
			sizeDiv: 4,
			speedMinDiv: 50,
			speedMaxDiv: 100,
			timeDiv: 100,
			scaleDiv: 1,
			started: true
		};
		
		try {
			var fisheye = new Fisheye(window, canvas, settings);
		}
		catch(ex) {
			window.alert(ex.toString());
			return;
		}
	};
})();