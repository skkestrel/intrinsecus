using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class LateralFly : IExercise
    {
        private Transition state;
        private int reps;
        private int repFlashTicks;
        private int targetReps;

        enum Transition
        {
            UpToDown,
            DownToUp,
        }

        public LateralFly(int tarReps)
        {
            state = Transition.DownToUp;
            reps = 0;
            repFlashTicks = 4;
            state = Transition.DownToUp;
            this.targetReps = tarReps;
        }

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            CameraSpacePoint leftShoulder = body.Joints[JointType.ShoulderLeft].Position;
            CameraSpacePoint leftElbow = body.Joints[JointType.ElbowLeft].Position;

            CameraSpacePoint rightShoulder = body.Joints[JointType.ShoulderRight].Position;
            CameraSpacePoint rightElbow = body.Joints[JointType.ElbowRight].Position;

            CameraSpacePoint centerShoulder = body.Joints[JointType.SpineShoulder].Position;

            double leftAngle = MathUtil.CosineLaw(leftElbow, centerShoulder, leftShoulder);
            double rightAngle = MathUtil.CosineLaw(rightElbow, centerShoulder, rightShoulder);

            if ((leftAngle < 120) && (rightAngle < 120))
            {
                if (state == Transition.UpToDown)
                {
                    reps++;
                    intrinsecus.InstructionLabel.Content = "You're flying bro!";
                    state = Transition.DownToUp;
                }
            }
            else if ((leftAngle > 175) && (rightAngle > 175))
            {
                if (state == Transition.DownToUp)
                {
                    state = Transition.UpToDown;
                    repFlashTicks = 0;
                }
            }

            if (repFlashTicks++ <= 3)
            {
                Pen highlightPen = new Pen(Brushes.Green, 10);

                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.ShoulderLeft].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.ElbowLeft].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.ElbowLeft].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.WristLeft].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.ShoulderRight].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.ElbowRight].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.ElbowRight].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.WristRight].Position));
            }

            return reps;
        }

        public int GetTargetReps()
        {
            return targetReps;
        }

        public string GetName()
        {
            return "Lateral Fly";
        }

        public string GetPhoneticName()
        {
            return "lateral fly";
        }
    }
}
