using System;
using System.Windows;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    /// <summary>
    /// Interaction logic for SelectionDialogue.xaml
    /// </summary>
    public partial class SelectionDialogue
    {

        private Intrinsecus parent;
        private int reps;
        bool repflag;

        public SelectionDialogue(Intrinsecus parent)
        {
	        this.parent = parent;

            // register event handler
            this.parent.SpeechEngine.CommandRecieved += AudioCommandReceived;
            
            InitializeComponent();
            repflag = false;
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
            this.parent.SetExercise(new Squat(reps), reps);
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

		/*private void DeadliftsButton_Click(object sender, RoutedEventArgs e)
		{
            StartDeadlifts();
		}*/

        /*private void StartDeadlifts()
        {
            this.parent.SetExercise(new Deadlifts(), reps);
            this.Close();
            this.parent.SingletonSelectionDialogue = null;

        }*/

		private void ShoulderPressesButton_Click(object sender, RoutedEventArgs e)
		{
            StartShoulderPresses();
		}

        private void StartShoulderPresses()
        {
            this.parent.SetExercise(new ShoulderPresses(reps), reps);
            this.Close();
            this.parent.SingletonSelectionDialogue = null;

        }

		private void SplitLegLungesButton_Click(object sender, RoutedEventArgs e)
		{
            StartSplitLegLunges();
		}

        private void StartSplitLegLunges()
        {
            this.parent.SetExercise(new SplitLegLunges(reps), reps);
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

		private void JumpingJacksButton_Click(object sender, RoutedEventArgs e)
		{
            StartJumpingJacks();
		}

        private void StartJumpingJacks()
        {
            this.parent.SetExercise(new JumpingJacks(), reps);
            this.Close();
            this.parent.SingletonSelectionDialogue = null;
        }

        private void VerticalJumpButton_Click(object sender, RoutedEventArgs e)
        {
            StartVerticalJump();
        }

        private void StartVerticalJump()
        {
            this.parent.SetExercise(new VerticalJumpTest(reps), reps);
            this.Close();
        }
        private void LateralFlyButton_Click(object sender, RoutedEventArgs e)
        {
            StartLateralFly();
        }

        private void StartLateralFly()
        {
            this.parent.SetExercise(new LateralFly(), reps);
            this.Close();
        }

        private void SetReps()
        {
            string textBoxContents = this.RepTextBox.Text;
            Int32.TryParse(textBoxContents, out reps);

            foreach (Body body in parent.Bodies)
            {
                if (body.IsTracked)
                {
                    reps = (int) (Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.HandRight].Position.Y) * 50);
                }
            }

            this.RepTextBox.Text = "The Rep you entered is " + reps.ToString();
            repflag = true;
            
        }

        public void ViewReps()
        {
            string textBoxContents = this.RepTextBox.Text;
            int tempReps = 0;
            
            foreach (Body body in parent.Bodies)
            {
                if (body.IsTracked)
                {
                    tempReps = (int)(Math.Abs(body.Joints[JointType.HandLeft].Position.Y - body.Joints[JointType.HandRight].Position.Y) * 50);
                }
            }
            if (!repflag)
            {
                this.RepTextBox.Text = tempReps.ToString();
            }

        }

        void AudioCommandReceived(object sender, AudioCommandEventArgs e)
        {
            switch (e.command)
            {
                // not implemented ENTER, SELECT
                case AudioCommand.BACK:
                    this.Close();
                    this.parent.SingletonSelectionDialogue = null;
                    break;
                /*case AudioCommand.DEADLIFT:
                    StartDeadlifts();
                    break;*/
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
                case AudioCommand.REPCOMMAND:
                    SetReps();
                    break;
            }
        }

    }
}
