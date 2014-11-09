using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class JumpingJacks : IExercise
    {
        private Transition state = Transition.DOWNTOUP;
        private int reps = 0;

        enum Transition
        {
            UPTODOWN, 
            DOWNTOUP,
        }

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            double aRight = MathUtil.CosineLaw(body.Joints[JointType.ElbowRight].Position, body.Joints[JointType.HipRight].Position, body.Joints[JointType.ShoulderRight].Position);
            double aLeft = MathUtil.CosineLaw(body.Joints[JointType.ElbowLeft].Position, body.Joints[JointType.HipLeft].Position, body.Joints[JointType.ShoulderLeft].Position);
            double groin = MathUtil.CosineLaw(body.Joints[JointType.FootLeft].Position, body.Joints[JointType.FootRight].Position, body.Joints[JointType.SpineBase].Position);

            if ((groin < 20) && (aRight < 30) && (aLeft < 30))
            {
                if (state == Transition.UPTODOWN)
                {
                    reps++;
                    state = Transition.DOWNTOUP;
                }
            }
            else if ((groin > 60) && (aRight > 150) && (aLeft > 150))
            {
                if (state == Transition.DOWNTOUP)
                {
                    state = Transition.UPTODOWN;

                }
            }                   
            return reps;
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
