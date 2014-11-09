using System;
using System.Windows;

namespace EhT.Intrinsecus
{
    /// <summary>
    /// Interaction logic for SelectionDialogue.xaml
    /// </summary>
    public partial class SelectionDialogue
    {

        private Intrinsecus parent;
        private int reps;

        public SelectionDialogue(Intrinsecus parent)
        {
	        this.parent = parent;

            // register event handler
            this.parent.SpeechEngine.CommandRecieved += AudioCommandReceived;
            
            InitializeComponent();
        }

         ~SelectionDialogue()
        {
            this.parent.SpeechEngine.CommandRecieved -= AudioCommandReceived;
        }

		private void SquatsButton_Click(object sender, RoutedEventArgs e)
		{
            StartSquats();
		}

        private void StartSquats()
        {
            this.parent.SetExercise(new Squat());
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

		private void DeadliftsButton_Click(object sender, RoutedEventArgs e)
		{
            StartDeadlifts();
		}

        private void StartDeadlifts()
        {
            this.parent.SetExercise(new Deadlifts());
            this.Close();
            this.parent.SingletonSelectionDialogue = null;

        }

		private void ShoulderPressesButton_Click(object sender, RoutedEventArgs e)
		{
            StartShoulderPresses();
		}

        private void StartShoulderPresses()
        {
            this.parent.SetExercise(new ShoulderPresses());
            this.Close();
            this.parent.SingletonSelectionDialogue = null;

        }

		private void SplitLegLungesButton_Click(object sender, RoutedEventArgs e)
		{
            StartSplitLegLunges();
		}

        private void StartSplitLegLunges()
        {
            this.parent.SetExercise(new SplitLegLunges());
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

		private void JumpingJacksButton_Click(object sender, RoutedEventArgs e)
		{
            StartJumpingJacks();
		}

        private void StartJumpingJacks()
        {
            this.parent.SetExercise(new JumpingJacks());
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

        private void SetRepsButton_Click()
        {
            SetReps();
        }

        private void SetReps()
        {
            string textBoxContents = this.RepTextBox.Text;
            Int32.TryParse(textBoxContents, out reps);
        }

        void AudioCommandReceived(object sender, AudioCommandEventArgs e)
        {
            switch (e.command)
            {
                // not implemented ENTER
                case AudioCommand.BACK:
                    this.Close();
                    this.parent.SingletonSelectionDialogue = null;
                    break;
                case AudioCommand.SQUAT:
                    StartSquats();
                    break;
                case AudioCommand.DEADLIFT:
                    StartDeadlifts();
                    break;
                case AudioCommand.LUNGES:
                    StartSplitLegLunges();
                    break;
                case AudioCommand.SHOULDERPRESS:
                    StartShoulderPresses();
                    break;
                case AudioCommand.JUMPINGJACK:
                    StartJumpingJacks();
                    break;
                case AudioCommand.SELECT:
                    break;
            }
        }

    }
}
