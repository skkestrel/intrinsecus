using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class Deadlifts : IExercise
    {
        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            return 0;
        }

        public string GetName()
        {
            return "Deadlift";
        }

        public string GetPhoneticName()
        {
            return "dead lifts";
        }
    }
}
