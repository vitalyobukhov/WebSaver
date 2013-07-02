var requestAnimFrame = window.requestAnimationFrame ||
    window.webkitRequestAnimationFrame ||
    window.mozRequestAnimationFrame ||
    window.msRequestAnimationFrame ||
    function (c) { window.setTimeout(c, 15) };

// bind to window onload event
if (window.addEventListener) {
  window.attachEvent('onload', onloadHandler, false);
}
else if (window.attachEvent) {
  window.attachEvent('onload', onloadHandler );
}

function onloadHandler() {
    // get the canvas DOM element and the 2D drawing context
    var canvas = document.getElementById('canvas');
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    // create the scene and setup camera, perspective and viewport
    var scene = new Phoria.Scene();
    scene.camera.position = { x: 0.0, y: 5.0, z: -15.0 };
    scene.perspective.aspect = canvas.width / canvas.height;
    scene.viewport.width = canvas.width;
    scene.viewport.height = canvas.height;

    // create a canvas renderer
    var renderer = new Phoria.CanvasRenderer(canvas);

    // GENERATE TEST ENTITIES
    // add a grid to help visualise camera position etc.
    var plane = Phoria.Util.generateTesselatedPlane(16, 16, 0, 20);
    scene.graph.push(Phoria.Entity.create({
        points: plane.points,
        edges: plane.edges,
        polygons: plane.polygons,
        style: {
            drawmode: "wireframe",
            linewidth: 0.5,
            sortmode: "unsorted"
        }
    }));
    var cube = Phoria.Util.generateUnitCube(1);
    var testCube = Phoria.Entity.create({
        points: cube.points,
        edges: cube.edges,
        polygons: cube.polygons,
        style: {
            drawmode: "solid"
        }
    });
    scene.graph.push(testCube);
    cube = Phoria.Util.generateUnitCube(0.5);
    var childCube = Phoria.Entity.create({
        points: cube.points,
        edges: cube.edges,
        polygons: cube.polygons,
        style: {
            color: [0, 0, 0],
            drawmode: "wireframe"
        }
    });
    // add child object
    testCube.children.push(childCube);

    var blueLightObj = Phoria.Entity.create({
        points: [{ x: 0, y: 2, z: -5 }],
        style: {
            color: [0, 0, 255],
            drawmode: "point",
            shademode: "plain",
            linewidth: 5,
            linescale: 2
        }
    });
    scene.graph.push(blueLightObj);
    var blueLight = Phoria.PointLight.create({
        position: { x: 0, y: 2, z: -5 },
        color: [0, 0, 1]
    });
    blueLightObj.children.push(blueLight);

    var redLightObj = Phoria.Entity.create({
        points: [{ x: 0, y: 2, z: 5 }],
        style: {
            color: [255, 0, 0],
            drawmode: "point",
            shademode: "plain",
            linewidth: 5,
            linescale: 2
        }
    });
    scene.graph.push(redLightObj);
    var redLight = Phoria.PointLight.create({
        position: { x: 0, y: 2, z: 5 },
        color: [1, 0, 0]
    });
    redLightObj.children.push(redLight);

    var greenLightObj = Phoria.Entity.create({
        points: [{ x: 0, y: 2, z: 5 }],
        style: {
            color: [0, 255, 0],
            drawmode: "point",
            shademode: "plain",
            linewidth: 5,
            linescale: 2
        }
    });
    scene.graph.push(greenLightObj);
    var greenLight = Phoria.PointLight.create({
        position: { x: 0, y: 2, z: 5 },
        color: [0, 1, 0]
    });
    greenLightObj.children.push(greenLight);

    var rotates = [0, 0, 0];
    var fnAnimate = function () {
        // rotate local matrix of an object
        testCube.rotateY(0.5 * RADIANS);
        // translate local matrix of child object - will receive rotation from parent
        childCube.identity().translateY(Math.sin(Date.now() / 1000) + 3);
        // translate visible light objects around the origin - will rotate child Light emitters
        var sine = Math.sin(Date.now() / 500);
        blueLightObj.identity().rotateY(rotates[0] += 1 * RADIANS).translateY(sine);
        redLightObj.identity().rotateY(rotates[1] += 0.5 * RADIANS).translateY(sine);
        greenLightObj.identity().rotateY(rotates[2] += 1.5 * RADIANS).translateY(sine);

        // execute the model view 3D pipeline
        scene.modelView();
        // and render the scene
        renderer.render(scene);

        requestAnimFrame(fnAnimate);
    };
    requestAnimFrame(fnAnimate);
}