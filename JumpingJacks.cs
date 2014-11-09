using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class JumpingJacks : IExercise
    {
        public int Update(Body body, System.Windows.Media.DrawingContext ctx)
        {
            return 0;
        }

        public string getName()
        {
            return "Jumping Jack";
        }

        public string getPhoneticName()
        {
            return "jumping jacks";
        }
    }
}
