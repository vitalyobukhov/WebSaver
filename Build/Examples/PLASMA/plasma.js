//
// HTML5 Canvas "Plasma" Experiment
//
// for WebSaver project
// by Vitaly Obukhov, 2013
//

function Plasma(window, canvas, settings) {
	
	// rendering state flag
	var started = false;
	
	// canvas, content, image and data objects
	var srcCanvas, srcContext, srcImage, srcData;
	var dstCanvas, dstContext;

	// color intervals
	var colorMinVal, colorHalfVal, colorMaxVal;
	
	// animation dividers
	var minSpeedDiv, maxSpeedDiv, timeDiv, scaleDiv;
	
	// dimensions
	var width, height, diag;

	// attractors and repulsors initial coordinates
	var rcx1, rcy1, gcx1, gcy1, bcx1, bcy1;
	var rcx2, rcy2, gcx2, gcy2, bcx2, bcy2;
	
	// attractors and repulsors initial speeds
	var rsx1, rsy1, gsx1, gsy1, bsx1, bsy1;
	var rsx2, rsy2, gsx2, gsy2, bsx2, bsy2;
	
	
	// validate passed params
	function validate(w, c) {
		
		if (w === undefined) 
			throw 'Window object was not provided';
		
		w.requestAnimationFrame = w.requestAnimationFrame || 
			w.mozRequestAnimationFrame ||
			w.webkitRequestAnimationFrame || 
			w.msRequestAnimationFrame;
		
		if (!w.requestAnimationFrame) 
			throw 'RequestAnimationFrame is not supported by browser';
		
		if (!c)
			throw 'Canvas element was not found';
			
		if (!c.getContext('2d'))
			throw 'Canvas 2D is not supported by browser';
	}

	// init source and destination canvases
	function initCanvas(w, c) {
	
		// invisible small drawing canvas element
		srcCanvas = w.document.createElement('canvas');
		srcContext = srcCanvas.getContext('2d');
		
		// provided large visible canvas element
		dstCanvas = c;
		dstContext = dstCanvas.getContext('2d');
	}
	
	// get settings value by key and default
	function getPositiveSetting(s, k, d) {
		return (s[k] !== undefined && s[k] > 0) ? s[k] : d;
	}
	
	function initSettings(s) {
	
		// default values for settings
		var colorMinValDef = 0;
		var colorMaxValDef = 255;
		var speedMinDivDef = 40;
		var speedMaxDivDef = 20;
		var timeDivDef = 100;
		var scaleDivDef = 20;
		
		// init settings
		colorMinVal = getPositiveSetting(s, 'colorMinVal', colorMinValDef);
		colorMaxVal = getPositiveSetting(s, 'colorMaxVal', colorMaxValDef);
		colorHalfVal = Math.floor((colorMinVal + colorMaxVal) / 2);
		speedMinDiv = getPositiveSetting(s, 'speedMinDiv', speedMinDivDef);
		speedMaxDiv = getPositiveSetting(s, 'speedMaxDiv', speedMaxDivDef);
		timeDiv = getPositiveSetting(s, 'timeDiv', timeDivDef);
		scaleDiv = getPositiveSetting(s, 'scaleDiv', scaleDivDef);
		started = s.started !== undefined ? s.started : false;
	}
	
	// init objects dimensions and scaling
	function initDimensions() {
	
		// init dimensions
		width = Math.round(dstCanvas.width / scaleDiv);
		height = Math.round(dstCanvas.height / scaleDiv);
		diag = Math.sqrt(width * width + height * height);
		
		// init source canvas objects
		srcCanvas.width = width;
		srcCanvas.height = height;
		srcImage = srcContext.getImageData(0, 0, srcCanvas.width, srcCanvas.height);
		srcData = srcImage.data;
		
		// scale destination canvas content to paint it from source canvas content
		dstContext.scale(dstCanvas.width / width, dstCanvas.height / height);
	}
	
	// init start attractors and repulsors coordinates
	function initCoordinates() {
	
		// attractors
		rcx1 = Math.random() * width;
		rcy1 = Math.random() * height;
		gcx1 = Math.random() * width;
		gcy1 = Math.random() * height;
		bcx1 = Math.random() * width;
		bcy1 = Math.random() * height;

		// repulsors
		rcx2 = Math.random() * width;
		rcy2 = Math.random() * height;
		gcx2 = Math.random() * width;
		gcy2 = Math.random() * height;
		bcx2 = Math.random() * width;
		bcy2 = Math.random() * height;
	}
	
	// init start attractors and repulsors speeds
	function initSpeeds() {
	
		// multipliers
		var sxb = width / speedMinDiv;
		var sxa = width / speedMaxDiv;
		var syb = height / speedMinDiv;
		var sya = height / speedMaxDiv;
		
		// attractors
		rsx1 = sxb + Math.random() * sxa;
		rsy1 = syb + Math.random() * sya;
		gsx1 = sxb + Math.random() * sxa;
		gsy1 = syb + Math.random() * sya;
		bsx1 = sxb + Math.random() * sxa;
		bsy1 = syb + Math.random() * sya;
		
		// repulsors
		rsx2 = sxb + Math.random() * sxa;
		rsy2 = syb + Math.random() * sya;
		gsx2 = sxb + Math.random() * sxa;
		gsy2 = syb + Math.random() * sya;
		bsx2 = sxb + Math.random() * sxa;
		bsy2 = syb + Math.random() * sya;
	}
	
	// get color component weight value
	function getColorWeight(cx, cy, tx, ty) {
	
		// deltas
        var dx = cx - tx;
        var dy = cy - ty;
		
		// calc measure
        return 1 - Math.sqrt(dx * dx + dy * dy) / diag;
    }
	
	// get new attractor or repulsor coordinate 
	// from current one, speed, size and time
	function getNewCoordinate(c, s, d, t) {
        var r = (s * t) % (2 * d);
		
		// handle all position cases
        if (c + r >= 0 && c + r < d)
            return c + r;
        if (c + r > d)
			return (c + r > 2 * d) ?
				(c + r - 2 * d) :
				(d - (c + r - d));
        return (c + r < -1 * d) ?
				(d - (-1 * (c + r) - d)) :
				(-1 * (c + r));
    }
	
	
	// draw plasma effect on source canvas using
	// current time and move it to destination one
	this.draw = function (time) {
	
        var t = time / timeDiv;
		
		// calc new attractors coordinates
		var rcx1n = getNewCoordinate(rcx1, rsx1, width, t);
		var rcy1n = getNewCoordinate(rcy1, rsy1, height, t);
		var gcx1n = getNewCoordinate(gcx1, gsx1, width, t);
		var gcy1n = getNewCoordinate(gcy1, gsy1, height, t);
		var bcx1n = getNewCoordinate(bcx1, bsx1, width, t);
		var bcy1n = getNewCoordinate(bcy1, bsy1, height, t);
		
		// calc new repulsors coordinates
		var rcx2n = getNewCoordinate(rcx2, rsx2, width, t);
		var rcy2n = getNewCoordinate(rcy2, rsy2, height, t);
		var gcx2n = getNewCoordinate(gcx2, gsx2, width, t);
		var gcy2n = getNewCoordinate(gcy2, gsy2, height, t);
		var bcx2n = getNewCoordinate(bcx2, bsx2, width, t);
		var bcy2n = getNewCoordinate(bcy2, bsy2, height, t);
		
        var rn, gn, bn, o;
		
        for(var x = 0; x < width; x++) {
            for(var y = 0; y < height; y++) {

				// calc new point color components
                rn = colorHalfVal + colorMaxVal * (getColorWeight(rcx1n, rcy1n, x, y) - getColorWeight(rcx2n, rcy2n, x, y));
                gn = colorHalfVal + colorMaxVal * (getColorWeight(gcx1n, gcy1n, x, y) - getColorWeight(gcx2n, gcy2n, x, y));
                bn = colorHalfVal + colorMaxVal * (getColorWeight(bcx1n, bcy1n, x, y) - getColorWeight(bcx2n, bcy2n, x, y));

				// handle boundaries
                rn = rn > colorMaxVal ? colorMaxVal : (rn < colorMinVal ? colorMinVal : rn);
                gn = gn > colorMaxVal ? colorMaxVal : (gn < colorMinVal ? colorMinVal : gn);
                bn = bn > colorMaxVal ? colorMaxVal : (bn < colorMinVal ? colorMinVal : bn);

				// set canvas context image data
				o = (y * width + x) * 4;
                srcData[o + 0] = rn;
				srcData[o + 1] = gn;
				srcData[o + 2] = bn;
				srcData[o + 3] = 255;
            }
        }

		// draw calculated data on source canvas
        srcContext.putImageData(srcImage, 0, 0);

		// move source canvas drawing to destination one
		dstContext.drawImage(srcCanvas, 0, 0);
		
		// handle stop condition
		if (started) {
			var that = this;
			requestAnimationFrame(function() { that.draw(Date.now()); });
		}
    };
	
	// start draw animation on canvas
	this.start = function() {
		started = true;
		var that = this;
		requestAnimationFrame(function() { that.draw(Date.now()); });
	};
	
	// stop draw animation on canvas
	this.stop = function() {
		started = false;
	};
	
	
	validate(window, canvas);
			
	// init
	initCanvas(window, canvas);
	initSettings(settings || {});
	initDimensions();
	initCoordinates();
	initSpeeds();
	
	if (started) 
		this.start();
}