(function(global) {

    var socket;

    var SocketConnection = {

    };

    SocketConnection.connect = (address)=> {

        if (socket != null) socket.disconnect();

        socket = io(address);
        socket.on('connect', ()=>{
            console.log("Connected to socket server");

        });

    }

    SocketConnection.sendPoseData = (results)=>{
        if (socket != null && results.poseWorldLandmarks){
            socket.send(results.poseWorldLandmarks);
        }
    }







    


    
    global.SocketConnection = SocketConnection;

})(window);