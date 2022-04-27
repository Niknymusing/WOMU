


document.addEventListener("DOMContentLoaded", function(){
    
    console.log("Starting...");
    const pose = new Pose({locateFile: (file) => {
        return `https://cdn.jsdelivr.net/npm/@mediapipe/pose/${file}`;
      }});
    const videoElement = document.getElementsByClassName('input_video')[0];
    const canvasElement = document.getElementsByClassName('output_canvas')[0];
    const canvasCtx = canvasElement.getContext('2d');
    const landmarkContainer = document.getElementsByClassName('landmark-grid-container')[0];
    const grid = new LandmarkGrid(landmarkContainer);
    const sourceSelectElement = document.querySelector("#sourceSelect");
    UpdateSources(sourceSelectElement);
    sourceSelectElement.addEventListener("onchange", function(){
        console.log("selected source changed");

    });
    const restartButton = document.querySelector("#sourceSelect");
    restartButton.addEventListener("click", function(){
        pose.reset();
    });
    
    // handle results from pose calculator
    function onResults(results) {
      if (!results.poseLandmarks) {
        grid.updateLandmarks([]);
        return;
      }
    
      canvasCtx.save();
      canvasCtx.clearRect(0, 0, canvasElement.width, canvasElement.height);
      canvasCtx.drawImage(results.segmentationMask, 0, 0,
                          canvasElement.width, canvasElement.height);
    
      // Only overwrite existing pixels.
      canvasCtx.globalCompositeOperation = 'source-in';
      canvasCtx.fillStyle = '#00FF00';
      canvasCtx.fillRect(0, 0, canvasElement.width, canvasElement.height);
    
      // Only overwrite missing pixels.
      canvasCtx.globalCompositeOperation = 'destination-atop';
      canvasCtx.drawImage(
          results.image, 0, 0, canvasElement.width, canvasElement.height);
    
      canvasCtx.globalCompositeOperation = 'source-over';
      drawConnectors(canvasCtx, results.poseLandmarks, POSE_CONNECTIONS,
                     {color: '#00FF00', lineWidth: 4});
      drawLandmarks(canvasCtx, results.poseLandmarks,
                    {color: '#FF0000', lineWidth: 2});
      canvasCtx.restore();
    
      grid.updateLandmarks(results.poseWorldLandmarks);
    }
    
    
    pose.setOptions({
      modelComplexity: 1,
      smoothLandmarks: true,
      enableSegmentation: true,
      smoothSegmentation: true,
      minDetectionConfidence: 0.5,
      minTrackingConfidence: 0.5
    });
    pose.onResults(onResults);
    
    const camera = new Camera(videoElement, {
      onFrame: async () => {
        await pose.send({image: videoElement});
      },
      width: 1280,
      height: 720
    });
    camera.start();

});

function onFrame(){


    
}

function UpdateSources(sourceSelectElement){
    var videoInputs = [];
    navigator.mediaDevices.enumerateDevices()
        .then(function(devices){
            devices.forEach(function(d){
                if (d.kind == "videoinput") videoInputs.push(d);                
            });

            sourceSelectElement.innerHTML = "";
            videoInputs.forEach(function(input){
                var opt = document.createElement("option");
                opt.setAttribute("value", input.deviceId);
                opt.innerHTML = input.label;
                sourceSelectElement.appendChild(opt);
            });

        });

    
}


function showError(msg){
    console.error("ERROR");
    console.error(msg);
}