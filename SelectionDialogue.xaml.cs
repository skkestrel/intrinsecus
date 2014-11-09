using System;
using System.Windows;

namespace EhT.Intrinsecus
{
    /// <summary>
    /// Interaction logic for SelectionDialogue.xaml
    /// </summary>
    public partial class SelectionDialogue
    {
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
                case AudioCommand.DEADLIFT:
                    break;
                case AudioCommand.LUNGES:
                    break;
                case AudioCommand.SHOULDERPRESS:
                    break;
                case AudioCommand.SELECT:
                    break;
            }
        }

	    private Intrinsecus parent;

        public SelectionDialogue(Intrinsecus parent)
        {
	        this.parent = parent;
            InitializeComponent();
        }

		private void SquatsButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeadliftsButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ShoulderPressesButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SplitLegLungesButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SquatsButton1_Click(object sender, RoutedEventArgs e)
		{

		}
    }
}
