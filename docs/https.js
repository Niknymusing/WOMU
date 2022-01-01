if (location.hostname !== 'localhost' && location.hostname !== '127.0.0.1') {
	if (location.protocol != 'https:') {
		location.href = 'https:' + window.location.href.substring(window.location.protocol.length);
	}
}
