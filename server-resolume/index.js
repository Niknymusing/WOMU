const WebSocket = require('ws')
const axios = require('axios')

const serverAddress = 'wss://womu-server.jonasjohansson.repl.co'

const socket = new WebSocket(serverAddress, {
	headers: {
		'user-agent': 'Mozilla',
	},
})

socket.on('open', function () {
	console.log('Opened…')
})

socket.on('close', function () {
	var now = new Date().getTime()
	console.log(new Date(), 'Socket closed, TTL', (now - socket._created) / 1000)
})
socket.on('message', function incoming(message) {
	console.log(`Received message => ${message}`)
	console.log(parseFloat(message))
	axios
		.put('http://localhost:8080/api/v1/composition', {
			speed: {
				value: parseFloat(message),
			},
		})
		.then(
			(response) => {
				// console.log(response)
			},
			(error) => {
				// console.log(error)
			}
		)
})
