using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
	public interface IExercise
    {
		/// <summary>
		/// update the exercise status
		/// </summary>
		/// <param name="body">the body to update for</param>
		/// <param name="ctx">the drawing context for the viewbox</param>
		/// <returns>the number of reps completed</returns>returns>
        int Update(Body body, System.Windows.Media.DrawingContext ctx);

        string GetName();

        string GetPhoneticName();
    }
}
