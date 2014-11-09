using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class Deadlifts : IExercise
    {
        public int Update(Body body, System.Windows.Media.DrawingContext ctx)
        {
            return 0;
        }

        public string getName()
        {
            return "Deadlift";
        }

        public string getPhoneticName()
        {
            return "dead lifts";
        }
    }
}
