using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhT.Intrinsecus
{
	public interface IExercise
    {
		/// <summary>
		/// update the exercise status
		/// </summary>
		/// <param name="ctx">the drawing context for the viewbox</param>
		/// <returns>the number of reps completed</returns>returns>
        int Update(System.Windows.Media.DrawingContext ctx);

        string getName();

        string getPhoneticName();
    }
}
