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
        }

		private void DeadliftsButton_Click(object sender, RoutedEventArgs e)
		{
            StartDeadlifts();
		}

        private void StartDeadlifts()
        {
            this.parent.SetExercise(new Deadlifts());
            this.Close();

        }

		private void ShoulderPressesButton_Click(object sender, RoutedEventArgs e)
		{
            StartShoulderPresses();
		}

        private void StartShoulderPresses()
        {
            this.parent.SetExercise(new ShoulderPresses());
            this.Close();

        }

		private void SplitLegLungesButton_Click(object sender, RoutedEventArgs e)
		{
            StartSplitLegLunges();
		}

        private void StartSplitLegLunges()
        {
            this.parent.SetExercise(new SplitLegLunges());
            this.Close();
        }

		private void JumpingJacksButton_Click(object sender, RoutedEventArgs e)
		{
            StartJumpingJacks();
		}

        private void StartJumpingJacks()
        {
            this.parent.SetExercise(new JumpingJacks());
            this.Close();
        }

        private void VerticalJumpButton_Click(object sender, RoutedEventArgs e)
        {
            StartVerticalJump();
        }

        private void StartVerticalJump()
        {
            this.parent.SetExercise(new VerticalJumpTest());
            this.Close();
        }
        private void LateralFlyButton_Click(object sender, RoutedEventArgs e)
        {
            StartLateralFly();
        }

        private void StartLateralFly()
        {
            this.parent.SetExercise(new LateralFly());
            this.Close();
        }
        void AudioCommandReceived(object sender, AudioCommandEventArgs e)
        {
            switch (e.command)
            {
                // not implemented ENTER, SELECT
                case AudioCommand.BACK:
                    this.Close();
                    break;
                case AudioCommand.DEADLIFT:
                    StartDeadlifts();
                    break;
                case AudioCommand.JUMPINGJACK:
                    StartJumpingJacks();
                    break;
                case AudioCommand.LUNGES:
                    StartSplitLegLunges();
                    break;
                case AudioCommand.LATERALFLY:
                    StartLateralFly();
                    break;
                case AudioCommand.SHOULDERPRESS:
                    StartShoulderPresses();
                    break;
                case AudioCommand.SQUAT:
                    StartSquats();
                    break;
                case AudioCommand.VERTICALJUMP:
                    StartVerticalJump();
                    break;
            }
        }
    }
}
