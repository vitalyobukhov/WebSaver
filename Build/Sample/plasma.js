function Plasma(document, canvas, settings) {

	var srccnv, srccnt, image, data;
	var dstcnv, dstcnt;

	var COLOR_VAL_MAX, COLOR_VAL_HALF, COLOR_VAL_MIN;
	var SPEED_MIN_DIV, SPEED_MAX_DIV;
	var TIME_DIV, SCALE_DIV;

	var scale, width, height, diag;

	var rcx1, rcy1, gcx1, gcy1, bcx1, bcy1;
	var rcx2, rcy2, gcx2, gcy2, bcx2, bcy2;
	
	var rsx1, rsy1, gsx1, gsy1, bsx1, bsy1;
	var rsx2, rsy2, gsx2, gsy2, bsx2, bsy2;
	
	
	function initSourceCanvas(document) {
		srccnv = document.createElement('canvas');
		if (srccnv.getContext === undefined || 
			(srccnt = srccnv.getContext('2d')) === undefined)
			throw 'Canvas not supported';
	}
	
	function initDestinationCanvas(document, canvas) {
		dstcnv = canvas;
		if (dstcnv === null ||
			dstcnv.getContext === undefined ||
			(dstcnt = dstcnv.getContext('2d')) === undefined)
			throw 'Canvas not found';
	}
	
	function initSettings(settings) {
		COLOR_VAL_MAX = 255;
		COLOR_VAL_HALF = 127;
		COLOR_VAL_MIN = 0;

		SPEED_MIN_DIV = 40.0;
		SPEED_MAX_DIV = 20.0;
		TIME_DIV = 100.0;
		SCALE_DIV = 20;
		
		scale = (settings.scale === undefined || settings.scale <= 0) ? SCALE_DIV : settings.scale;
	}
	
	function initDimensions() {
		width = Math.round(dstcnv.width / scale);
		height = Math.round(dstcnv.height / scale);
		diag = Math.sqrt(width * width + height * height);
		
		srccnv.width = width;
		srccnv.height = height;
		image = srccnt.getImageData(0, 0, srccnv.width, srccnv.height);
		data = image.data;
		
		dstcnt.scale(dstcnv.width / width, dstcnv.height / height);
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
		var sxb = 1.0 * width / SPEED_MIN_DIV;
		var sxa = 1.0 * width / SPEED_MAX_DIV;
		var syb = 1.0 * height / SPEED_MIN_DIV;
		var sya = 1.0 * height / SPEED_MAX_DIV;
		
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
		
        return 1.0 - Math.sqrt(dx * dx + dy * dy) / diag;
    };
	
	var getNewCordinate = function (cord, speed, size, time) {
        var rem = (speed * time) % (2.0 * size);
        var val;
		
        if (cord + rem >= 0.0 && cord + rem < size){
            val = cord + rem;
        } else if (cord + rem > size) {
            if (cord + rem > 2.0 * size) {
                val = cord + rem - 2.0 * size;
            } else {
                val = size - (cord + rem - size);
            }
        } else {
            if ( cord + rem < -1.0 * size) {
                val = size - (-1.0 * (cord + rem) - size);
            } else {
                val = -1.0 * (cord + rem);
            }
        }

        return val;
    };
	

	this.draw = function (time) {
        var t = time / TIME_DIV;
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

                rn = COLOR_VAL_HALF + COLOR_VAL_MAX * (getColorWeight(rcx1n, rcy1n, x, y) - getColorWeight(rcx2n, rcy2n, x, y));
                gn = COLOR_VAL_HALF + COLOR_VAL_MAX * (getColorWeight(gcx1n, gcy1n, x, y) - getColorWeight(gcx2n, gcy2n, x, y));
                bn = COLOR_VAL_HALF + COLOR_VAL_MAX * (getColorWeight(bcx1n, bcy1n, x, y) - getColorWeight(bcx2n, bcy2n, x, y));

                rn = rn > COLOR_VAL_MAX ? COLOR_VAL_MAX : (rn < COLOR_VAL_MIN ? COLOR_VAL_MIN : rn);
                gn = gn > COLOR_VAL_MAX ? COLOR_VAL_MAX : (gn < COLOR_VAL_MIN ? COLOR_VAL_MIN : gn);
                bn = bn > COLOR_VAL_MAX ? COLOR_VAL_MAX : (bn < COLOR_VAL_MIN ? COLOR_VAL_MIN : bn);

				o = (y * width + x) * 4;
                data[o + 0] = rn;
				data[o + 1] = gn;
				data[o + 2] = bn;
				data[o + 3] = 255;
            }
        }

        srccnt.putImageData(image, 0, 0);	
		dstcnt.drawImage(srccnv, 0, 0);
    };
	
	
	initSourceCanvas(document);
	initDestinationCanvas(document, canvas);
	initSettings(settings || {});
	initDimensions();
	initCordinates();
	initSpeeds();
};