//
// HTML5 Canvas "Fisheye Lens" Experiment
//
// for WebSaver project
// by Vitaly Obukhov, 2013
//

function Fisheye(window, canvas, settings) {

	// rendering state flag
	var started;
	
	// source image canvas
	var srcCanvas, srcContext;
	
	// temporary canvas objects
	var inrCanvas, inrContext, inImage, inData, outImage, outData;
	
	// output canvas
	var dstCanvas, dstContext, dstImage, dstData;
	
	//dimensions
	var srcWidth, scrHeight, tmpWidth, tmpHeight, dstWidth, dstHeight;
	
	// filter params
	var ra, rb, rc, rd;
	
	// dividers
	var speedMinDiv, speedMaxDiv, timeDiv, sizeDiv, scaleDiv;
	
	// calculated vars
	var cx, cy, sx, sy;
	var c, cw, ch;

	
	// validate passed params
	function validate(w, c) {

		if (w === undefined) 
			throw 'Window object was not provided';
		
		w.requestAnimationFrame = w.requestAnimationFrame || 
			w.mozRequestAnimationFrame ||
			w.webkitRequestAnimationFrame || 
			w.msRequestAnimationFrame;
		
		if (!w.requestAnimationFrame) 
			throw 'RequestAnimationFrame is not supported by browser'
	
		if (!c)
			throw 'Canvas element was not found';
			
		if (!c.getContext('2d'))
			throw 'Canvas 2D is not supported by browser';
	}
	
	// init canvases
	function initCanvas(w, c) {
		srcCanvas = c;
		srcContext = c.getContext('2d');
		
		inrCanvas = w.document.createElement('canvas');
		inrContext = inrCanvas.getContext('2d');
		
		dstCanvas = w.document.createElement('canvas');
		dstContext = dstCanvas.getContext('2d');
		
		dstCanvas.className = 'fisheye';
		dstCanvas.style.position = 'absolute';
		w.document.body.appendChild(dstCanvas);
	}
	
	function isNumber(val) {
		return !isNaN(parseFloat(val)) && isFinite(val);
	}
	
	function initSettings(s) {
	
		// defaults
		var ratioADef = 0;
		var ratioBDef = 0;
		var ratioCDef = 0;
		var speedMinDivDef = 100;
		var speedMaxDivDef = 50;
		var timeDivDef = 100;
		var sizeDivDef = 5;
		var scaleDivDef = 1;
		
		// init settings
		ra = isNumber(s.ratioA) ? s.ratioA : ratioADef;
		rb = isNumber(s.ratioB) ? s.ratioB : ratioBDef;
		rc = isNumber(s.ratioC) ? s.ratioC : ratioCDef;
		rd = 1 - ra - rb - rc;
		
		speedMinDiv = s.speedMinDiv > 0 ? s.speedMinDiv : speedMinDivDef;
		speedMaxDiv = s.speedMaxDiv > 0 ? s.speedMaxDiv : speedMaxDivDef;
		timeDiv = s.timeDiv > 0 ? s.timeDiv : timeDivDef;
		sizeDiv = s.sizeDiv > 1 ? s.sizeDiv : sizeDivDef;
		scaleDiv = s.scaleDiv >= 1 ? s.scaleDiv : scaleDivDef;
	}
	
	// init object dimensions and scalling
	function initDimensions() {	
	
		// dimensions
		srcWidth = srcCanvas.width;
		srcHeight = srcCanvas.height;
	
		var dstSize = Math.round(Math.min(srcWidth, srcHeight) / sizeDiv);
		dstWidth = dstSize;
		dstHeight = dstSize;
		
		var tmpSize = Math.round(dstSize / scaleDiv);
		tmpWidth = tmpSize;
		tmpHeight = tmpSize;
		
		// canvases
		inrCanvas.width = tmpWidth;
		inrCanvas.height = tmpHeight;
		
		dstCanvas.width = dstWidth;
		dstCanvas.height = dstHeight;		
		
		// data
		inImage = inrContext.getImageData(0, 0, tmpWidth, tmpHeight);
		inData = inImage.data;
		
		outImage = inrContext.createImageData(tmpWidth, tmpHeight);
		outData = outImage.data;
		
		dstImage = dstContext.getImageData(0, 0, dstWidth, dstHeight);
		dstData = dstImage.data;
	}
	
	// init misc vars
	function initVars() {
	
		cx = dstWidth / 2 + (srcWidth - 2 * dstWidth) * Math.random();
		cy = dstHeight / 2 + (srcHeight - 2 * dstHeight) * Math.random(); 
		
		var sx1 = srcWidth / speedMinDiv;
		var sy1 = srcHeight / speedMinDiv;
		var sx2 = srcWidth / speedMaxDiv;
		var sy2 = srcHeight / speedMaxDiv;
		
		sx = (Math.random() < 0.5 ? -1 : 1) * (sx1 + Math.random() * (sx2 - sx1));
		sy = (Math.random() < 0.5 ? -1 : 1) * (sy1 + Math.random() * (sy2 - sy1));
		
		c = Math.floor(Math.min(tmpWidth, tmpHeight) / 2);
		cw = Math.floor(tmpWidth / 2);
		ch = Math.floor(tmpHeight / 2);
	}
	
	// get new coordinate using provided data
	function getNewCoordinate(coord, speed, border, obj, time) {
	
		var inr;
		var box = border - obj;
		var off = (speed * time) % (2 * box);
		var pos = coord + off;
	
		if (pos >= 0 && pos < box)
			inr = pos;
		else if (pos > box)
			inr = (pos < 2 * box) ?
				(box - (pos - box)) :
				(pos - 2 * box);
		else inr = (pos >= -1 * box) ?
			-pos :
			(box - (-pos - box));
			
		return obj / 2 + inr;
    }
	
	
	// draw effect using provided time
	this.draw = function(time) {
	
		var t = time / timeDiv;
		
		// dst point coords
		var cxn = getNewCoordinate(cx, sx, srcWidth, dstWidth, t);
		var cyn = getNewCoordinate(cy, sy, srcHeight, dstHeight, t);
		
		// dst canvas coords
		var left = Math.round(cxn - dstWidth / 2);
		var top = Math.round(cyn - dstHeight / 2);
		
		// scale, clip and draw source canvas to inner one
		inrContext.drawImage(srcCanvas, left, top, dstWidth, dstHeight, 0, 0, tmpWidth, tmpHeight);
		
		// get related image data
		inImage = inrContext.getImageData(0, 0, tmpWidth, tmpHeight);
		inData = inImage.data;

		// barrel distorning effect
		for (var y = 0; y < tmpHeight; y++) {
			for (var x = 0; x < tmpWidth; x++) {
			
				var dx = (x - cw) / c;
				var dy = (y - ch) / c;

				var r1 = Math.sqrt(dx * dx + dy * dy);
				var r2 = (ra * r1 * r1 * r1 + rb * r1 * r1 + rc * r1 + rd) * r1;
				var f = Math.abs(r1 / r2);

				var xd = Math.round(cw + (dx * f * c));
				var yd = Math.round(ch + (dy * f * c));

				if (xd >= 0 && yd >= 0 && xd < tmpWidth && yd < tmpHeight) {
				
					var p1 = (yd * tmpWidth + xd) * 4;
					var p2 = (y * tmpWidth + x) * 4;
					
					outData[p2 + 0] = inData[p1 + 0];
					outData[p2 + 1] = inData[p1 + 1];
					outData[p2 + 2] = inData[p1 + 2];
					outData[p2 + 3] = inData[p1 + 3];
				}
			}
		}
	
		// put data back
		inrContext.putImageData(outImage, 0, 0);
		
		// draw result
		dstContext.drawImage(inrCanvas, 0, 0, dstWidth, dstHeight);
		
		// move canvas
		dstCanvas.style.left = left + 'px';
		dstCanvas.style.top = top + 'px';
	
		if (started) {
			var that = this;
			requestAnimationFrame(function() { that.draw(Date.now()); });
		}
	};
	
	// start effect drawing
	this.start = function() {
		started = true;
		var that = this;
		requestAnimationFrame(function() { that.draw(Date.now()); });
	};
	
	// stop effect drawing
	this.stop = function() {
		started = false;
	};
	
	
	validate(window, canvas);
	
	// init
	initCanvas(window, canvas);
	initSettings(settings || {});
	initDimensions(settings);
	initVars();
	
	if (settings.started)
		this.start();
}