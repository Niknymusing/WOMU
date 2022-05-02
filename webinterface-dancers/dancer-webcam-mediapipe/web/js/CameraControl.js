(function(global) {
   
    
    var CameraControl = {
      
    };

    CameraControl.UpdateSources = (sourceSelectElement)=>{
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
    };

    CameraControl.StartCamera = (videoElement, cameraDeviceID)=>{
        navigator.mediaDevices.getUserMedia({ video : { deviceId : cameraDeviceID }})
            .then((stream)=>{
                console.log("Starting camera");                
                videoElement.srcObject = stream;                          
                videoElement.play();

            }).catch((err)=>{
                console.error("ERROR - could not start stream for " + cameraDeviceID);
            });
    };

    global.CameraControl = CameraControl;

})(window);