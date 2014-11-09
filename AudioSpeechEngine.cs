using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;    
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace EhT.Intrinsecus
{
    
    public enum AudioCommand
    {
        BACK,
        ENTER,
        SQUAT,
        DEADLIFT,
        LUNGES,
        SHOULDERPRESS,
        SELECT,
        JUMPINGJACK,
        VERTICALJUMP,
        LATERALFLY
    }

    public class AudioSpeechEngine
    {
        /// <summary>
        /// Stream for 32b-16b conversion.
        /// </summary>
        private KinectAudioStream convertStream = null;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine = null;

        public event EventHandler<AudioCommandEventArgs> CommandRecieved;


        public AudioSpeechEngine(KinectSensor kinectSensor)
        {

            // open the sensor
            kinectSensor.Open();

            // grab the audio stream
            IReadOnlyList<AudioBeam> audioBeamList = kinectSensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            // create the convert stream
            this.convertStream = new KinectAudioStream(audioStream);

            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    this.speechEngine.LoadGrammar(g);
                }

                this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////SpeechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }


        

        /// <summary>
        /// Gets the metadata for the speech recognizer (acoustic model) most suitable to
        /// process audio from Kinect device.
        /// </summary>
        /// <returns>
        /// RecognizerInfo if found, <code>null</code> otherwise.
        /// </returns>
        public RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

                /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            EventHandler<AudioCommandEventArgs> handler = CommandRecieved;

            AudioCommandEventArgs args = new AudioCommandEventArgs();

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "BACK":
                        args.command = AudioCommand.BACK;
                        break;

                    case "DEADLIFT":
                        args.command = AudioCommand.DEADLIFT;
                        break;

                    case "ENTER":
                        args.command = AudioCommand.ENTER;
                        break;

                    case "JUMPINGJACK":
                        args.command = AudioCommand.JUMPINGJACK;
                        break;

                    case "LATERALFLY":
                        args.command = AudioCommand.LATERALFLY;
                        break;

                    case "LUNGES":
                        args.command = AudioCommand.LUNGES;
                        break;

                    case "SHOULDERPRESS":
                        args.command = AudioCommand.SHOULDERPRESS;
                        break;

                    case "SELECT":
                        args.command = AudioCommand.SELECT;
                        break;

                    case "SQUAT":
                        args.command = AudioCommand.SQUAT;
                        break;

                    case "VERTICALJUMPTEST":
                        args.command = AudioCommand.VERTICALJUMP;
                        break;

                }

                handler(this, args);
            }
        }

        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            // OOPS
        }
    }

    public class AudioCommandEventArgs : EventArgs
    {
        public AudioCommand command { get; set; }
    }
}
