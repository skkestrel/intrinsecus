using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class SplitLegLunges : IExercise
    {
        private Transition state;
        private int reps;
        private int repFlashTicks;

        enum Transition
        {
            UpToDown,
            DownToUp,
        }

        public SplitLegLunges()
		{
            state = Transition.DownToUp;
            reps = 0;
            repFlashTicks = 4;
		}

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            intrinsecus.InstructionLabel.Content = "Let's start with that left foot forward champ!";

            if (reps == 10)
            {
                intrinsecus.InstructionLabel.Content = "Let's switch to the right foot now!";
            }
          
            CameraSpacePoint leftAnkle = body.Joints[JointType.AnkleLeft].Position;
            CameraSpacePoint leftKnee = body.Joints[JointType.KneeLeft].Position;

            CameraSpacePoint rightAnkle = body.Joints[JointType.AnkleRight].Position;
            CameraSpacePoint rightKnee = body.Joints[JointType.KneeRight].Position;

            CameraSpacePoint centerHip = body.Joints[JointType.SpineBase].Position;
            CameraSpacePoint spine = body.Joints[JointType.SpineShoulder].Position;

            double leftLegAngle = MathUtil.CosineLaw(leftAnkle, centerHip, leftKnee);
            double rightLegAngle = MathUtil.CosineLaw(rightAnkle, centerHip, rightKnee);

            double lean;

            if (reps <= 10)
            {
                lean = MathUtil.CosineLaw(leftKnee, spine, centerHip);
            }
            else
            {
                lean = MathUtil.CosineLaw(rightKnee, spine, centerHip);
            }

            if ((leftLegAngle > 140) && (rightLegAngle > 140))
            {
                if (state == Transition.UpToDown)
                {
                    reps++;
                    state = Transition.DownToUp;
                }
            }
            else if ((leftLegAngle < 100) && (rightLegAngle < 100))
            {
                if (state == Transition.DownToUp)
                {
                    state = Transition.UpToDown;
                    repFlashTicks = 0;
                }
            }

            if ((leftLegAngle < 75) || (rightLegAngle < 75))
            {
                intrinsecus.InstructionLabel.Content = "Put that foot further forward!";
            }

            if ((lean < 70) || (lean > 110))
            {
                intrinsecus.InstructionLabel.Content = "Don't lean!";
            }

            if (repFlashTicks++ <= 3)
            {
                Pen highlightPen = new Pen(Brushes.Green, 10);

                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.HipLeft].Position), 
                    intrinsecus.CameraToScreen(body.Joints[JointType.KneeLeft].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.KneeLeft].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.AnkleLeft].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.HipRight].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.KneeRight].Position));
                ctx.DrawLine(highlightPen, intrinsecus.CameraToScreen(body.Joints[JointType.KneeRight].Position),
                    intrinsecus.CameraToScreen(body.Joints[JointType.AnkleRight].Position));
            }

            return reps;
        }

        public int GetTargetReps()
        {
            return 20;
        }

        public string GetName()
        {
            return "Split Leg Lunge";
        }

        public string GetPhoneticName()
        {
            return "split leg lunges";
        }
    }
}
