var osc = require('node-osc');
var Server = require('socket.io').Server;
const httpServer = require('http').createServer();
const io = new Server(httpServer, {
  cors : {
    origin: '*',
    methods: ["GET", "POST"]
    }
});
httpServer.listen(8081)

var oscServer, oscClient;

io.on('connection', function (socket) {
  socket.on('config', function (obj) {
    console.log('config', obj);
    oscServer = new osc.Server(obj.server.port, obj.server.host);
    oscClient = new osc.Client(obj.client.host, obj.client.port);

    oscClient.send('/status', socket.id + ' connected');

    oscServer.on('message', function(msg, rinfo) {
      socket.emit('message', msg);
      console.log('sent OSC message to WS', msg, rinfo);
    });
  });
  socket.on('message', function (obj) {
    oscClient.send('/pose',...obj);
    console.log('sent pose to Unity', obj);
  });
  socket.on("disconnect", function () {
    oscServer.kill();
  })
});