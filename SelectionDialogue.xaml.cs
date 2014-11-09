using System.Windows;

namespace EhT.Intrinsecus
{
    /// <summary>
    /// Interaction logic for SelectionDialogue.xaml
    /// </summary>
    public partial class SelectionDialogue
    {
        AudioSpeechEngine audioEngine;

        public SelectionDialogue(AudioSpeechEngine audio)
        {
            audioEngine = audio;
            InitializeComponent();
        }

        void AudioCommandReceived(object sender, AudioCommandEventArgs e)
        {
            switch (e.command)
            {
                case AudioCommand.BACK:
                    break;
                case AudioCommand.ENTER:
                    break;
                case AudioCommand.SQUAT:
                    break;
                case AudioCommand.DEADLIFT:s
                    break;
                case AudioCommand.LUNGES:
                    break;
                case AudioCommand.SHOULDERPRESS:
                    break;
                case AudioCommand.SELECT:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
