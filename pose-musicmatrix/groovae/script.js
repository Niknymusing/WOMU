let onsetModel;
let grooveModel;

let modelSequence;

init();

function init() {
  onsetModel = new mm.OnsetsAndFrames('https://storage.googleapis.com/magentadata/js/checkpoints/transcription/onsets_frames_uni');
  grooveModel = new mm.MusicVAE('https://storage.googleapis.com/magentadata/js/checkpoints/music_vae/groovae_2bar_humanize');

  onsetModel.initialize().then(() => {
    console.log('Onset and Frame initialized')
  });

  grooveModel.initialize().then(() => {
    drumifyControls.removeAttribute('disabled');
    btnDrumify.textContent = 'groove and send midi';
  });
}

//TODO: define input
async function transcribeFromFile(blob) {
  model.transcribeFromAudioFile(blob).then((ns) => {
    groove(ns);
  });
}

async function groove(sequence) {
  const temp = parseFloat(1.0);

  const z = await grooveModel.encode([sequence]);
  const recon = await grooveModel.decode(z, temp, undefined, 4, parseInt(inputTempo.value));
  modelSequence = recon[0];

  z.dispose();

  sendGrooveToMidi()
}

function sendGrooveToMidi() {
  const player = new mm.MIDIPlayer();
  player.requestMIDIAccess().then(() => {
    player.outputs = [player.availableOutputs[0]];
    player.start(modelSequence);
  })
}