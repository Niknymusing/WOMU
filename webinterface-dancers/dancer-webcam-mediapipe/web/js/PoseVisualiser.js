(function(global) {

    var grid, canvasCtx, canvasElement, landmarkContainer;
     
    var PoseVisualiser = {
      
    };

    PoseVisualiser.setup = (canvas, landmarks)=>{        
        canvasElement = canvas;
        canvasCtx = canvas.getContext("2d");
        landmarkContainer = landmarks;
        grid = new LandmarkGrid(landmarkContainer);
    }

    PoseVisualiser.visualise = (results)=>{
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

    };





    
    global.PoseVisualiser = PoseVisualiser;

})(window);