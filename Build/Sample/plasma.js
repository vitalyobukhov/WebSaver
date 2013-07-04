function Plasma(window, canvas, settings) {
	
	var started = false;
	var srcCanvas, srcContent, srcImage, srcData;
	var dstCanvas, dstContent;

	var colorMinVal, colorHalfVal, colorMaxVal;
	var minSpeedDiv, maxSpeedDiv, timeDiv, scaleDiv;
	var width, height, diag;

	var rcx1, rcy1, gcx1, gcy1, bcx1, bcy1;
	var rcx2, rcy2, gcx2, gcy2, bcx2, bcy2;
	
	var rsx1, rsy1, gsx1, gsy1, bsx1, bsy1;
	var rsx2, rsy2, gsx2, gsy2, bsx2, bsy2;
	
	
	function initRequestAnimationFrame(w) {
		var requestAnimationFrame = w.requestAnimationFrame || 
			w.mozRequestAnimationFrame ||
			w.webkitRequestAnimationFrame || 
			w.msRequestAnimationFrame;
			
		window.requestAnimationFrame = requestAnimationFrame;
	}
	
	function initCanvas(d, c) {

		srcCanvas = d.createElement('canvas');
		if (srcCanvas.getContext === undefined || 
			(srcContent = srcCanvas.getContext('2d')) === undefined) {
			throw 'Canvas is not supported';
		}
		
		dstCanvas = c;
		if (dstCanvas === null ||
			dstCanvas.getContext === undefined ||
			(dstContent = dstCanvas.getContext('2d')) === undefined) {
			throw 'Canvas was not found';
		}
	}
	
	function getSetting(s, k, d) {
		return (s[k] !== undefined && s[k] > 0) ? s[k] : d;
	}
	
	function initSettings(s) {
		var colorMinValDef = 0;
		var colorMaxValDef = 255;
		var speedMinDivDef = 40;
		var speedMaxDivDef = 20;
		var timeDivDef = 100;
		var scaleDivDef = 20;
		
		colorMinVal = getSetting(s, 'colorMinVal', colorMinValDef);
		colorMaxVal = getSetting(s, 'colorMaxVal', colorMaxValDef);
		colorHalfVal = Math.floor((colorMinVal + colorMaxVal) / 2);
		speedMinDiv = getSetting(s, 'speedMinDiv', speedMinDivDef);
		speedMaxDiv = getSetting(s, 'speedMaxDiv', speedMaxDivDef);
		timeDiv = getSetting(s, 'timeDiv', timeDivDef);
		scaleDiv = getSetting(s, 'scaleDiv', scaleDivDef); 
	}
	
	function initDimensions() {
		width = Math.round(dstCanvas.width / scaleDiv);
		height = Math.round(dstCanvas.height / scaleDiv);
		diag = Math.sqrt(width * width + height * height);
		
		srcCanvas.width = width;
		srcCanvas.height = height;
		srcImage = srcContent.getImageData(0, 0, srcCanvas.width, srcCanvas.height);
		srcData = srcImage.data;
		
		dstContent.scale(dstCanvas.width / width, dstCanvas.height / height);
	}
	
	function initCordinates() {
		rcx1 = Math.random() * width;
		rcy1 = Math.random() * height;
		gcx1 = Math.random() * width;
		gcy1 = Math.random() * height;
		bcx1 = Math.random() * width;
		bcy1 = Math.random() * height;

		rcx2 = Math.random() * width;
		rcy2 = Math.random() * height;
		gcx2 = Math.random() * width;
		gcy2 = Math.random() * height;
		bcx2 = Math.random() * width;
		bcy2 = Math.random() * height;
	}
	
	function initSpeeds() {
		var sxb = width / speedMinDiv;
		var sxa = width / speedMaxDiv;
		var syb = height / speedMinDiv;
		var sya = height / speedMaxDiv;
		
		rsx1 = sxb + Math.random() * sxa;
		rsy1 = syb + Math.random() * sya;
		gsx1 = sxb + Math.random() * sxa;
		gsy1 = syb + Math.random() * sya;
		bsx1 = sxb + Math.random() * sxa;
		bsy1 = syb + Math.random() * sya;
		
		rsx2 = sxb + Math.random() * sxa;
		rsy2 = syb + Math.random() * sya;
		gsx2 = sxb + Math.random() * sxa;
		gsy2 = syb + Math.random() * sya;
		bsx2 = sxb + Math.random() * sxa;
		bsy2 = syb + Math.random() * sya;
	}
	
	
	var getColorWeight = function (cx, cy, tx, ty) {
        var dx = cx - tx;
        var dy = cy - ty;
		
        return 1 - Math.sqrt(dx * dx + dy * dy) / diag;
    };
	
	var getNewCordinate = function (cord, speed, size, time) {
        var rem = (speed * time) % (2 * size);
        var val;
		
        if (cord + rem >= 0 && cord + rem < size) {
            val = cord + rem;
        } else if (cord + rem > size) {
            if (cord + rem > 2 * size) {
                val = cord + rem - 2 * size;
            } else {
                val = size - (cord + rem - size);
            }
        } else {
            if ( cord + rem < -1 * size) {
                val = size - (-1 * (cord + rem) - size);
            } else {
                val = -1 * (cord + rem);
            }
        }

        return val;
    };
	

	this.draw = function (time) {
		if (time === undefined) {
			time = Date.now();
		}
	
        var t = time / timeDiv;
        var rcx1n, rcy1n, gcx1n, gcy1n, bcx1n, bcy1n;
        var rcx2n, rcy2n, gcx2n, gcy2n, bcx2n, bcy2n;
        var rn, gn, bn;
		var o;

        for(var x = 0; x < width; x++) {
            for(var y = 0; y < height; y++) {
                rcx1n = getNewCordinate(rcx1, rsx1, width, t);
                rcy1n = getNewCordinate(rcy1, rsy1, height, t);
                gcx1n = getNewCordinate(gcx1, gsx1, width, t);
                gcy1n = getNewCordinate(gcy1, gsy1, height, t);
                bcx1n = getNewCordinate(bcx1, bsx1, width, t);
                bcy1n = getNewCordinate(bcy1, bsy1, height, t);

                rcx2n = getNewCordinate(rcx2, rsx2, width, t);
                rcy2n = getNewCordinate(rcy2, rsy2, height, t);
                gcx2n = getNewCordinate(gcx2, gsx2, width, t);
                gcy2n = getNewCordinate(gcy2, gsy2, height, t);
                bcx2n = getNewCordinate(bcx2, bsx2, width, t);
                bcy2n = getNewCordinate(bcy2, bsy2, height, t);

                rn = colorHalfVal + colorMaxVal * (getColorWeight(rcx1n, rcy1n, x, y) - getColorWeight(rcx2n, rcy2n, x, y));
                gn = colorHalfVal + colorMaxVal * (getColorWeight(gcx1n, gcy1n, x, y) - getColorWeight(gcx2n, gcy2n, x, y));
                bn = colorHalfVal + colorMaxVal * (getColorWeight(bcx1n, bcy1n, x, y) - getColorWeight(bcx2n, bcy2n, x, y));

                rn = rn > colorMaxVal ? colorMaxVal : (rn < colorMinVal ? colorMinVal : rn);
                gn = gn > colorMaxVal ? colorMaxVal : (gn < colorMinVal ? colorMinVal : gn);
                bn = bn > colorMaxVal ? colorMaxVal : (bn < colorMinVal ? colorMinVal : bn);

				o = (y * width + x) * 4;
                srcData[o + 0] = rn;
				srcData[o + 1] = gn;
				srcData[o + 2] = bn;
				srcData[o + 3] = 255;
            }
        }

        srcContent.putImageData(srcImage, 0, 0);	
		dstContent.drawImage(srcCanvas, 0, 0);
		
		if (started) {
			var that = this;
			requestAnimationFrame(function() { that.draw(); });
		}
    };
	
	this.start = function() {
		started = true;
		var that = this;
		requestAnimationFrame(function() { that.draw(); });
	};
	
	this.stop = function() {
		started = false;
	};
	
	
	if (window === undefined) {
		throw 'Window was not provided';
	}
	
	initRequestAnimationFrame(window);
	initCanvas(window.document, canvas);
	initSettings(settings || {});
	initDimensions();
	initCordinates();
	initSpeeds();
}