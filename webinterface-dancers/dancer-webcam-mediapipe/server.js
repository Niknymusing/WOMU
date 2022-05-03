//
//
//

// Settings
var SOCKET_PORT = 8081;
var HTML_PORT = 8080;
var OSC_LOCAL_IP = "127.0.0.1"
var OSC_LOCAL_PORT = 8888;


// Websocket server
var osc = require('node-osc');
var Server = require('socket.io').Server;
const httpServerSockets = require('http').createServer();

const io = new Server(httpServerSockets, {
  cors : {
    origin: '*',
    methods: ["GET", "POST"]
    }
});
httpServerSockets.listen(SOCKET_PORT)
console.log("Starting socket server on port " + SOCKET_PORT);

// HTML server
const finalhandler = require('finalhandler');
const serveStatic = require('serve-static');

var serve = serveStatic('web', { index : ['index.html']});

const httpServerHTML = require('http').createServer(function onRequest(req, res){
  serve(req, res, finalhandler(req, res));
});

httpServerHTML.listen(HTML_PORT);
console.log("Starting web server on port " + HTML_PORT);

var oscClient = new osc.Client(OSC_LOCAL_IP, OSC_LOCAL_PORT);
console.log("creating OSC client pointing at " + OSC_LOCAL_IP + " : " + OSC_LOCAL_PORT);

var clientIDCount = 0;

io.on('connection', function (socket) { 
  
  var clientID = ++clientIDCount;
  var poseAddress = '/scene/dancer/pose/' + clientID;
  console.log("New websocket connection, clientID = " + clientID);
  socket.on('message', function (obj) {
    // console.log(obj);
    
    // first send that a pose is starting
    // oscClient.send(poseAddress + '/start', 1);
    // then send the individual landmark positions, with the clientID
    
    for (var i=0; i < obj.length; i++){
      //console.log("Sending " + poseAddress + '/' + i + " : " + [obj[i].x, obj[i].y, obj[i].z]);
      oscClient.send(poseAddress + '/' + i, [obj[i].x, obj[i].y, obj[i].z]);
    }

    // oscClient.send('/pose',...obj);
    // console.log('sent pose to Unity', obj);
  });
  socket.on("disconnect", function () {
    console.log("Websocket connection closed, clientID = " + clientID);
  })
});