//
// HTML5 Canvas "Plasma" Experiment
//
// for WebSaver project
// by Vitaly Obukhov, 2013
//

function Plasma(window, canvas, settings) {
	
	var speedDiv = 10000;
	
	var started = false;
	
	var inrCanvas, inrContext, inrImage, inrData;
	var dstCanvas, dstContext;

	var colorMinVal, colorHalfVal, colorMaxVal;
	var speedMin, speedMax, scaleDiv;
	var inrWidth, inrHeight, inrDiag;

	var rcx1, rcy1, gcx1, gcy1, bcx1, bcy1;
	var rcx2, rcy2, gcx2, gcy2, bcx2, bcy2;
	var rsx1, rsy1, gsx1, gsy1, bsx1, bsy1;
	var rsx2, rsy2, gsx2, gsy2, bsx2, bsy2;
	
	
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
		inrCanvas = wnd.document.createElement('canvas');
		inrContext = inrCanvas.getContext('2d');
		
		dstCanvas = cnv;
		dstContext = dstCanvas.getContext('2d');
	}
	
	function getSetting(set, key, def) {
		return (set[key] !== undefined && set[key] > 0) ? set[key] : def;
	}
	
	function initSettings(settings) {
		var colorMinValDef = 0;
		var colorMaxValDef = 255;
		
		var speedMinDef = 2;
		var speedMaxDef = 1;
		var scaleDivDef = 10;
		
		colorMinVal = getSetting(settings, 'colorMinVal', colorMinValDef);
		colorMaxVal = getSetting(settings, 'colorMaxVal', colorMaxValDef);
		colorHalfVal = Math.floor((colorMinVal + colorMaxVal) / 2);
		
		speedMin = getSetting(settings, 'speedMin', speedMinDef);
		speedMax = getSetting(settings, 'speedMax', speedMaxDef);
		scaleDiv = getSetting(settings, 'scaleDiv', scaleDivDef);
		
		started = settings.started !== undefined ? settings.started : false;
	}
	
	function initDimensions() {
		inrWidth = Math.round(dstCanvas.width / scaleDiv);
		inrHeight = Math.round(dstCanvas.height / scaleDiv);
		inrDiag = Math.sqrt(inrWidth * inrWidth + inrHeight * inrHeight);
		
		inrCanvas.width = inrWidth;
		inrCanvas.height = inrHeight;
		inrImage = inrContext.getImageData(0, 0, inrCanvas.width, inrCanvas.height);
		inrData = inrImage.data;
		
		dstContext.scale(dstCanvas.width / inrWidth, dstCanvas.height / inrHeight);
	}
	
	function initCoordinates() {
		rcx1 = Math.random() * inrWidth;
		rcy1 = Math.random() * inrHeight;
		gcx1 = Math.random() * inrWidth;
		gcy1 = Math.random() * inrHeight;
		bcx1 = Math.random() * inrWidth;
		bcy1 = Math.random() * inrHeight;

		rcx2 = Math.random() * inrWidth;
		rcy2 = Math.random() * inrHeight;
		gcx2 = Math.random() * inrWidth;
		gcy2 = Math.random() * inrHeight;
		bcx2 = Math.random() * inrWidth;
		bcy2 = Math.random() * inrHeight;
	}
	
	function getSpeed(sa, sb) {
		return (Math.random() < 0.5 ? -1 : 1) * (sa + Math.random() * (sb - sa));
	}
	
	function initSpeeds() {
		var sxa = inrWidth * speedMin / speedDiv;
		var sxb = inrWidth * speedMax / speedDiv;
		var sya = inrHeight * speedMin / speedDiv;
		var syb = inrHeight * speedMax / speedDiv;
		
		rsx1 = getSpeed(sxa, sxb);
		rsy1 = getSpeed(sya, syb);
		gsx1 = getSpeed(sxa, sxb);
		gsy1 = getSpeed(sya, syb);
		bsx1 = getSpeed(sxa, sxb);
		bsy1 = getSpeed(sya, syb);
		
		rsx2 = getSpeed(sxa, sxb);
		rsy2 = getSpeed(sya, syb);
		gsx2 = getSpeed(sxa, sxb);
		gsy2 = getSpeed(sya, syb);
		bsx2 = getSpeed(sxa, sxb);
		bsy2 = getSpeed(sya, syb);
	}
	
	function getColorWeight(nx, ny, x, y) {
        var dx = nx - x;
        var dy = ny - y;
		
        return 1 - Math.sqrt(dx * dx + dy * dy) / inrDiag;
    }
	
	function getNewCoordinate(coord, speed, border, time) {
        var off = (speed * time) % (2 * border);
		var pos = coord + off;
		
        if (pos >= 0 && pos < border)
            return pos;
        if (pos > border)
			return (pos > 2 * border) ?
				(pos - 2 * border) :
				(border - (pos - border));
        return (pos < -border) ?
				(border - (-pos - border)) :
				-pos;
    }
	
	
	this.draw = function (time) {
		var rcx1n = getNewCoordinate(rcx1, rsx1, inrWidth, time);
		var rcy1n = getNewCoordinate(rcy1, rsy1, inrHeight, time);
		var gcx1n = getNewCoordinate(gcx1, gsx1, inrWidth, time);
		var gcy1n = getNewCoordinate(gcy1, gsy1, inrHeight, time);
		var bcx1n = getNewCoordinate(bcx1, bsx1, inrWidth, time);
		var bcy1n = getNewCoordinate(bcy1, bsy1, inrHeight, time);
		
		var rcx2n = getNewCoordinate(rcx2, rsx2, inrWidth, time);
		var rcy2n = getNewCoordinate(rcy2, rsy2, inrHeight, time);
		var gcx2n = getNewCoordinate(gcx2, gsx2, inrWidth, time);
		var gcy2n = getNewCoordinate(gcy2, gsy2, inrHeight, time);
		var bcx2n = getNewCoordinate(bcx2, bsx2, inrWidth, time);
		var bcy2n = getNewCoordinate(bcy2, bsy2, inrHeight, time);
		
        for(var x = 0; x < inrWidth; x++) {
            for(var y = 0; y < inrHeight; y++) {
                var rn = colorHalfVal + colorMaxVal * (getColorWeight(rcx1n, rcy1n, x, y) - getColorWeight(rcx2n, rcy2n, x, y));
                var gn = colorHalfVal + colorMaxVal * (getColorWeight(gcx1n, gcy1n, x, y) - getColorWeight(gcx2n, gcy2n, x, y));
                var bn = colorHalfVal + colorMaxVal * (getColorWeight(bcx1n, bcy1n, x, y) - getColorWeight(bcx2n, bcy2n, x, y));

                rn = rn > colorMaxVal ? colorMaxVal : (rn < colorMinVal ? colorMinVal : rn);
                gn = gn > colorMaxVal ? colorMaxVal : (gn < colorMinVal ? colorMinVal : gn);
                bn = bn > colorMaxVal ? colorMaxVal : (bn < colorMinVal ? colorMinVal : bn);

				var o = (y * inrWidth + x) * 4;
                inrData[o + 0] = rn;
				inrData[o + 1] = gn;
				inrData[o + 2] = bn;
				inrData[o + 3] = 255;
            }
        }

        inrContext.putImageData(inrImage, 0, 0);

		dstContext.drawImage(inrCanvas, 0, 0);
		
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
	initDimensions();
	initCoordinates();
	initSpeeds();
	
	if (started) 
		this.start();
}