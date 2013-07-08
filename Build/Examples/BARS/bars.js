//
// HTML5 Canvas "Raster Bars" Experiment
//
// for WebSaver project
// by Vitaly Obukhov, 2013
//

function Bars(window, canvas, settings) {

	var angDiv = 1000;

	var canvas, context, width, height, image, data;
	var started, background, bars, barsLength, speed;

	
	function validate(wnd, cnv) {
		if (wnd === undefined) 
			throw 'Window object was not provided';
		
		wnd.requestAnimationFrame = wnd.requestAnimationFrame || 
			wnd.mozRequestAnimationFrame ||
			wnd.webkitRequestAnimationFrame || 
			wnd.msRequestAnimationFrame;
		
		if (!wnd.requestAnimationFrame) 
			throw 'RequestAnimationFrame is not supported by browser';
		
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
		
		image = context.getImageData(0, 0, width, height);
		data = image.data;
	}
	
	function isColor(val) {
		return 	/(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(val);
	}
	
	function isBar(src) {
		return src.hasOwnProperty('firstColor') &&
			src.hasOwnProperty('secondColor') &&
			src.hasOwnProperty('phase') &&
			src.hasOwnProperty('size') &&
			isColor(src.firstColor) &&
			isColor(src.secondColor) &&
			src.phase >= 0 &&
			src.size > 0 && src.size <= 1;
	}
	
	function initBar(src) {
		return {
			firstColor: src.firstColor,
			secondColor: src.secondColor,
			phase: src.phase,
			size: height * src.size / 2
		};
	}
	
	function initSettings(stg) {
		var startedDef = false;
		var backgroundDef = 'black';
		var barsDef = [
			{ firstColor: '#f00', secondColor: '#400', phase: 0.000, size: 0.2000 },
			{ firstColor: '#f80', secondColor: '#420', phase: 0.250, size: 0.1500 },
			{ firstColor: '#ff0', secondColor: '#440', phase: 0.450, size: 0.1000 },
			{ firstColor: '#0f0', secondColor: '#040', phase: 0.575, size: 0.0800 },
			{ firstColor: '#08f', secondColor: '#024', phase: 0.675, size: 0.0650 },
			{ firstColor: '#00f', secondColor: '#004', phase: 0.750, size: 0.0550 },
			{ firstColor: '#80f', secondColor: '#204', phase: 0.800, size: 0.0475 }
		];
		var speedDef = 1;
	
		started = stg.started !== undefined ? stg.started : startedDef;
	
		background = isColor(stg.background) ? stg.background : backgroundDef;
		
		bars = [];
		if (stg.bars instanceof Array) {
			for (var i = 0; i < stg.bars.length; i++)
				if (isBar(stg.bars[i]))
					bars.push(initBar(stg.bars[i]));
		} else {
			for (var i = 0; i < barsDef.length; i++)
				bars.push(initBar(barsDef[i]));
		}
		barsLength = bars.length;
		
		speed = stg.speed > 0 ? stg.speed : speedDef;
	}
	
	
	this.draw = function (time) {
		context.fillStyle = background;
		context.fillRect(0, 0, width, height);
		
		var ang = (time * speed) / angDiv % (2 * Math.PI);
		
		for (var i = barsLength - 1; i >= 0; i--) {
			var bar = bars[i];
			var y = height / 2 * (1 + Math.sin(ang - bar.phase));
			
			var gradient = context.createLinearGradient(0, y - bar.size, 0, y + bar.size);
			gradient.addColorStop(0.0, bar.secondColor);
			gradient.addColorStop(0.5, bar.firstColor);
			gradient.addColorStop(1.0, bar.secondColor);
			
			context.fillStyle = gradient;
			context.fillRect(0, y - bar.size, width, bar.size * 2);
		}
		
		if (started) {
			var self = this;
			requestAnimationFrame(function() { self.draw(Date.now()); });
		}
    };
	
	this.start = function() {
		started = true;
		var self = this;
		requestAnimationFrame(function() { self.draw(Date.now()); });
	};
	
	this.stop = function() {
		started = false;
	};
	
	
	validate(window, canvas);
			
	initCanvas(canvas);
	initSettings(settings || {});
	
	if (started) 
		this.start();
}