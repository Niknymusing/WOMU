const WebSocket = require('ws')
const osc = require('osc')

const serverAddress = 'wss://womu-server.jonasjohansson.repl.co'

const socket = new WebSocket(serverAddress, {
	headers: {
		'user-agent': 'Mozilla',
	},
})

const udpPort = new osc.UDPPort({
	localAddress: '0.0.0.0',
	localPort: 7400,
	remoteAddress: '127.0.0.1',
	remotePort: 7500,
})

udpPort.open()

socket.on('open', function () {
	console.log('Opened…')
})

socket.on('message', function incoming(message) {
	console.log(`Received message => ${message}`)
	const data = JSON.parse(message)
	udpPort.send({
		address: data.address,
		args: [
			{
				type: 'f',
				value: data.value,
			},
		],
	})
})
