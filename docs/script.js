const events = [
	'touchstart',
	'touchmove',
	'touchend',
	'touchcancel',
	'mousedown',
	'mousemove',
	'mouseup',
	'mouseover',
	'mouseout',
	'mouseenter',
	'mouseleave',
]

const debug = 0
const val_threshold = 0.3
const diff_threshold = 0.01
const calibrate_wait = 10000

var socket
var isDown = false
var isVisible = true
var val = 0,
	curr = 0
var min = 1,
	max = 0
// https://stackoverflow.com/questions/19764018/controlling-fps-with-requestanimationframe
var fpsInterval, startTime, now, then, elapsed

window.onload = function () {
	document.documentElement.classList.remove('loading')
}

async function setup() {
	socket = connectSocket()
	socket.onopen = function () {
		startUpdate(10)
		document.documentElement.classList.add('live')
		setTimeout(function () {
			getTouch()
			getMic()
		}, 500)
	}
	socket.onerror = (err) => {
		console.error(err)
	}
}

function connectSocket() {
	return new WebSocket('wss://womu-server.jonasjohansson.repl.co')
}

function startUpdate(fps) {
	fpsInterval = 1000 / fps
	then = Date.now()
	startTime = then
	update()
}

function getTouch() {
	for (let event of events) {
		document.addEventListener(event, function (e) {
			sendTouchData(e)
		})
	}
}

function sendTouchData(e) {
	let x, y
	if (e.type == 'touchstart' || e.type == 'touchmove' || e.type == 'touchend' || e.type == 'touchcancel') {
		const evt = typeof e.originalEvent === 'undefined' ? e : e.originalEvent
		const touch = evt.touches[0] || evt.changedTouches[0]
		x = touch.pageX
		y = touch.pageY
	} else if (
		e.type == 'mousedown' ||
		e.type == 'mouseup' ||
		e.type == 'mousemove' ||
		e.type == 'mouseover' ||
		e.type == 'mouseout' ||
		e.type == 'mouseenter' ||
		e.type == 'mouseleave'
	) {
		x = e.clientX
		y = e.clientY
	}
	if (e.type == 'mousedown' || e.type == 'touchstart') {
		isDown = true
	} else if (e.type == 'mouseup' || e.type == 'touchend') {
		isDown = false
	}
	if (isDown) {
		y = clamp(y, 0, window.innerHeight)
		val = y / window.innerHeight
		val = 1 - val // invert
	}
}

const getMic = () => {
	setInterval(function () {
		console.log(`val: ${val}  min: ${min} max: ${max}`)
		console.log('Calibrate!')
		min = 1
		max = 0
	}, calibrate_wait)
	navigator.mediaDevices
		.getUserMedia({ audio: true, video: false })
		.then(function (stream) {
			const audioContext = new AudioContext()
			const analyser = audioContext.createAnalyser()
			const microphone = audioContext.createMediaStreamSource(stream)
			const javascriptNode = audioContext.createScriptProcessor(2048, 1, 1)

			analyser.smoothingTimeConstant = 0.5 // 0.8
			analyser.fftSize = 1024

			microphone.connect(analyser)
			analyser.connect(javascriptNode)
			javascriptNode.connect(audioContext.destination)
			javascriptNode.onaudioprocess = function () {
				const array = new Uint8Array(analyser.frequencyBinCount)
				analyser.getByteFrequencyData(array)
				var values = 0

				const length = array.length
				for (var i = 0; i < length; i++) {
					values += array[i] / 100
				}

				var average = values / length

				average = clamp(average, 0, 1)

				if (average < min) min = average
				if (average > max) max = average
				if (isDown == false && min !== max) {
					val = map(average, min, max, 0, 1)
					if (val < val_threshold) val = 0
				}
			}
		})
		.catch(function (err) {
			console.error(err)
		})
}

function update() {
	requestAnimationFrame(update)

	now = Date.now()
	elapsed = now - then

	const diff = Math.abs(val - curr)
	if (diff > diff_threshold) {
		val = round(val)
		curr = lerp(curr, val, 0.05)
		curr = round(curr)
		// console.log(curr);

		document.documentElement.style.setProperty('--level', curr)
		if (socket.readyState == socket.OPEN) {
			if (elapsed > fpsInterval) {
				then = now - (elapsed % fpsInterval)
				socket.send(curr)
				if (debug) console.log(`Sending message => ${val}`)
			}
		}
	}
}

function lerp(v0, v1, t) {
	return v0 * (1 - t) + v1 * t
}

function round(val) {
	return Math.round(val * 1000) / 1000
}

function map(x, in_min, in_max, out_min, out_max) {
	return ((x - in_min) * (out_max - out_min)) / (in_max - in_min) + out_min
}

function clamp(val, min, max) {
	return Math.min(Math.max(min, val), max)
}

window.addEventListener(
	'visibilitychange',
	function () {
		isVisible = !isVisible
		if (document.webkitHidden || isVisible === false) {
			socket.close()
			alert('Då din röst inte längre var aktiv har du blivit frånkopplad, sidan laddas om…')
			location.reload()
		}
	},
	false
)
