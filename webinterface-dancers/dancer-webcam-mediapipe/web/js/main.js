
document.addEventListener("DOMContentLoaded", function(){
    
    console.log("Starting...");

    /*
      SERVER CONNECTION
    */
    SocketConnection.connect('http://' + window.location.hostname + ':8081');
    
    /*
      WEBCAM INPUT
    */
    const videoElement = document.getElementsByClassName('input_video')[0];    
    const sourceSelectElement = document.querySelector("#sourceSelect");
    CameraControl.UpdateSources(sourceSelectElement);
    
    // use helper functions in CameraControl to start camera
    const startButton = document.querySelector("#buttonStartCamera");
    startButton.addEventListener("click", function(){
        if (videoElement.srcObject == null){
          var cameraDevice = sourceSelectElement.value;

          CameraControl.StartCamera(videoElement, cameraDevice);
          videoElement.requestVideoFrameCallback(onCameraFrame);
        }
        pose.reset();
    });

    // send video data to pose estimator on frame
    var onCameraFrame = async ()=>{
      if (pose != null && videoElement.srcObject != null) { 
        await pose.send({ image : videoElement}); 
      }
      videoElement.requestVideoFrameCallback(onCameraFrame);
    }

    

    /*
      POSE ESTIMATION
    */
    const pose = new Pose({locateFile: (file) => {
      return `https://cdn.jsdelivr.net/npm/@mediapipe/pose/${file}`;
    }});

    pose.setOptions({
      modelComplexity: 1,
      smoothLandmarks: true,
      enableSegmentation: true,
      smoothSegmentation: true,
      minDetectionConfidence: 0.5,
      minTrackingConfidence: 0.5
    });

    const canvasElement = document.getElementsByClassName('output_canvas')[0];    
    const landmarkContainer = document.getElementsByClassName('landmark-grid-container')[0];    
    
    PoseVisualiser.setup(canvasElement, landmarkContainer);

    const enableTransmit = document.querySelector("#enableTransmit");
    const enableVisualise = document.querySelector("#enableVisualise");
    
    // handle results from pose calculator
    function onResults(results) {
      if (enableVisualise.checked) PoseVisualiser.visualise(results);
      if (enableTransmit.checked) SocketConnection.sendPoseData(results);
    }

    pose.onResults(onResults);
    
    
   
  

});




function showError(msg){
    console.error("ERROR");
    console.error(msg);
}