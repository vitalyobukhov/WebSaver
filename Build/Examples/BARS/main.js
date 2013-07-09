(function() {
	window.onload = function() {
		var canvas = document.getElementById('canvas');
		canvas.width = canvas.parentNode.clientWidth;
		canvas.height = canvas.parentNode.clientHeight;

		var settings = { 
			background: '#111',
			speed: 1.75,
			started: true
		};
		
		try {
			var bars = new Bars(window, canvas, settings); 
		}
		catch(ex) {
			window.alert(ex.toString());
		}
	};
})();
