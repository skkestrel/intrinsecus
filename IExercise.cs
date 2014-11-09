using System.Windows.Media;
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
		/// <param name="intrinsecus"></param>
		/// <returns>the number of reps completed</returns>returns>
		int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus);

        string GetName();

        string GetPhoneticName();
    }
}
