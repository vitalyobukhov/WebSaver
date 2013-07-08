//
// HTML5 Canvas "Fisheye Lens" Experiment
//
// for WebSaver project
// by Vitaly Obukhov, 2013
//

function Fisheye(window, canvas, settings) {

	var speedDiv = 10000;

	var started;
	
	var srcCanvas, srcContext;
	var inrCanvas, inrContext, inImage, inData, outImage, outData;
	var dstCanvas, dstContext, dstImage, dstData;
	var srcWidth, scrHeight, tmpWidth, tmpHeight, dstWidth, dstHeight;
	
	var ra, rb, rc, rd;
	var speedMin, speedMax, sizeDiv, scaleDiv;
	
	var cx, cy, sx, sy;
	var c, cw, ch;

	
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
	
	function initCanvas(wnd, cnv) {
		srcCanvas = cnv;
		srcContext = cnv.getContext('2d');
		
		inrCanvas = wnd.document.createElement('canvas');
		inrContext = inrCanvas.getContext('2d');
		
		dstCanvas = wnd.document.createElement('canvas');
		dstContext = dstCanvas.getContext('2d');
		
		dstCanvas.className = 'fisheye';
		dstCanvas.style.position = 'absolute';
		wnd.document.body.appendChild(dstCanvas);
	}
	
	function isNumber(val) {
		return !isNaN(parseFloat(val)) && isFinite(val);
	}
	
	function initSettings(settings) {
		var ratioADef = 0;
		var ratioBDef = 0;
		var ratioCDef = 0;
		
		var speedMinDef = 2;
		var speedMaxDef = 1;
		
		var sizeDivDef = 4;
		var scaleDivDef = 1;
		
		ra = isNumber(settings.ratioA) ? settings.ratioA : ratioADef;
		rb = isNumber(settings.ratioB) ? settings.ratioB : ratioBDef;
		rc = isNumber(settings.ratioC) ? settings.ratioC : ratioCDef;
		rd = 1 - ra - rb - rc;
		
		speedMin = settings.speedMin > 0 ? settings.speedMin : speedMinDef;
		speedMax = settings.speedMax > 0 ? settings.speedMax : speedMaxDef;
		
		sizeDiv = settings.sizeDiv > 1 ? settings.sizeDiv : sizeDivDef;
		scaleDiv = settings.scaleDiv >= 1 ? settings.scaleDiv : scaleDivDef;
	}
	
	function initDimensions() {	
		srcWidth = srcCanvas.width;
		srcHeight = srcCanvas.height;
	
		var dstSize = Math.round(Math.min(srcWidth, srcHeight) / sizeDiv);
		dstWidth = dstSize;
		dstHeight = dstSize;
		
		var tmpSize = Math.round(dstSize / scaleDiv);
		tmpWidth = tmpSize;
		tmpHeight = tmpSize;
		
		inrCanvas.width = tmpWidth;
		inrCanvas.height = tmpHeight;
		
		dstCanvas.width = dstWidth;
		dstCanvas.height = dstHeight;		
		
		inImage = inrContext.getImageData(0, 0, tmpWidth, tmpHeight);
		inData = inImage.data;
		
		outImage = inrContext.createImageData(tmpWidth, tmpHeight);
		outData = outImage.data;
		
		dstImage = dstContext.getImageData(0, 0, dstWidth, dstHeight);
		dstData = dstImage.data;
	}
	
	function initVars() {
		cx = dstWidth / 2 + (srcWidth - 2 * dstWidth) * Math.random();
		cy = dstHeight / 2 + (srcHeight - 2 * dstHeight) * Math.random(); 
		
		var sx1 = srcWidth * speedMin
		var sy1 = srcHeight * speedMin;
		var sx2 = srcWidth * speedMax;
		var sy2 = srcHeight * speedMax;
		
		sx = (Math.random() < 0.5 ? -1 : 1) * (sx1 + Math.random() * (sx2 - sx1)) / speedDiv;
		sy = (Math.random() < 0.5 ? -1 : 1) * (sy1 + Math.random() * (sy2 - sy1)) / speedDiv;
		
		c = Math.floor(Math.min(tmpWidth, tmpHeight) / 2);
		cw = Math.floor(tmpWidth / 2);
		ch = Math.floor(tmpHeight / 2);
	}
	
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
	
	
	this.draw = function(time) {
		var cxn = getNewCoordinate(cx, sx, srcWidth, dstWidth, time);
		var cyn = getNewCoordinate(cy, sy, srcHeight, dstHeight, time);
		
		var left = Math.round(cxn - dstWidth / 2);
		var top = Math.round(cyn - dstHeight / 2);
		
		inrContext.drawImage(srcCanvas, left, top, dstWidth, dstHeight, 0, 0, tmpWidth, tmpHeight);
		
		inImage = inrContext.getImageData(0, 0, tmpWidth, tmpHeight);
		inData = inImage.data;

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
	
		inrContext.putImageData(outImage, 0, 0);
		
		dstContext.drawImage(inrCanvas, 0, 0, dstWidth, dstHeight);
		
		dstCanvas.style.left = left + 'px';
		dstCanvas.style.top = top + 'px';
	
		if (started) {
			var that = this;
			requestAnimationFrame(function() { that.draw(Date.now()); });
		}
	};
	
	this.start = function() {
		started = true;
		var that = this;
		requestAnimationFrame(function() { that.draw(Date.now()); });
	};
	
	this.stop = function() {
		started = false;
	};
	
	
	validate(window, canvas);
	
	initCanvas(window, canvas);
	initSettings(settings || {});
	initDimensions(settings);
	initVars();
	
	if (settings.started)
		this.start();
}