using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class ShoulderPresses : IExercise
    {
        private Transition state = Transition.DOWNTOUP;
        private int reps = 0;

        enum Transition
        {
            UPTODOWN,
            DOWNTOUP,
        }

        public ShoulderPresses()
		{
            state = Transition.DOWNTOUP;
            reps = 0;
		}

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            CameraSpacePoint leftShoulder = body.Joints[JointType.ShoulderLeft].Position;
            CameraSpacePoint leftElbow = body.Joints[JointType.ElbowLeft].Position;
            CameraSpacePoint leftWrist = body.Joints[JointType.WristLeft].Position;

            CameraSpacePoint rightShoulder = body.Joints[JointType.ShoulderRight].Position;
            CameraSpacePoint rightElbow = body.Joints[JointType.ElbowRight].Position;
            CameraSpacePoint rightWrist = body.Joints[JointType.WristRight].Position;

            double leftAngle = MathUtil.CosineLaw(leftWrist, leftShoulder, leftElbow);
            double rightAngle = MathUtil.CosineLaw(rightWrist, rightShoulder, rightElbow);

            if ((leftAngle < 40) && (rightAngle < 40))
            {
                if (state == Transition.UPTODOWN)
                {
                    reps++;
                    state = Transition.DOWNTOUP;
                }
            }
            else if ((leftAngle > 130) && (rightAngle > 130))
            {
                if (state == Transition.DOWNTOUP)
                {
                    state = Transition.UPTODOWN;

                }
            }
            return reps;

            return reps;
        }

        public string GetName()
        {
            return "Shoulder Press";
        }

        public string GetPhoneticName()
        {
            return "shoulder presses";
        }
    }
}
