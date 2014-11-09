using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class JumpingJacks : IExercise
    {
        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            return 0;
        }

        public string GetName()
        {
            return "Jumping Jack";
        }

        public string GetPhoneticName()
        {
            return "jumping jacks";
        }
    }
}
