var socket

async function setup() {
	socket = connectSocket()
	socket.onopen = function () {
		getInputData()
	}
	socket.onerror = (err) => {
		console.error(err)
	}
}

function connectSocket() {
	return new WebSocket('wss://womu-server.jonasjohansson.repl.co')
}

function getInputData() {
	const inputs = document.querySelectorAll('input')
	for (let input of inputs) {
		input.onchange = function () {
			if (input.hasAttribute('data-osc')) {
				const data = {
					address: input.getAttribute('data-osc'),
					value: input.value / 100,
				}
				socket.send(JSON.stringify(data))
				// console.log(msg)
			}
		}
	}
}

setup()
