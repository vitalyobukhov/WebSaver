function Checkers(canvas, settings) {
	
	var canvas, context, width, height;

	var firstColor, secondColor, minCount, draw;
	
	
	function validate(cnv) {
		if (!cnv)
			throw 'Canvas element was not found';
			
		if (!cnv.getContext('2d'))
			throw 'Canvas 2D is not supported by browser';
	}
	
	function initCanvas(cnv) {
		canvas = cnv;
		context = canvas.getContext('2d');
		
		width = canvas.width;
		height = canvas.height;
	}

	function isColor(val) {
		return 	/(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(val);
	}
	
	function initSettings(settings) {
		var firstColorDef = 'black';
		var secondColorDef = 'white';
		var minCountDef = 16;
		
		firstColor = isColor(settings.firstColor) ? settings.firstColor : firstColorDef;
		secondColor = isColor(settings.secondColor) ? settings.secondColor : secondColorDef;
		minCount = settings.minCount > 0 ? settings.minCount : minCountDef;
		draw = settings.draw !== undefined ? settings.draw : false;
	}
	

	this.draw = function() {
		var step = Math.min(width, height) / minCount;
		var w = width / step, wf = Math.floor(w);
		var h = height / step, hf = Math.floor(h);
		var stepX = w === wf ? step : width / wf;
		var stepY = h === hf ? step : height / hf;
		
		for (var y = 0; y < hf; y++) {
			for (var x = 0; x < wf; x++) {
				context.fillStyle = (y + x) % 2 === 0 ? firstColor : secondColor;
				context.fillRect(x * stepX, y * stepY, stepX, stepY);
			}
		}
	};
	
	
	validate(canvas);
	
	initCanvas(canvas);
	initSettings(settings || {});
	
	if (draw)
		this.draw();
}