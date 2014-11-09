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
        private int targetReps;
        private int repFlashTicks;

        enum Transition
        {
            UpToDown,
            DownToUp,
        }

        public SplitLegLunges(int tarReps)
		{
            state = Transition.UpToDown;
            reps = 0;
            repFlashTicks = 4;
            this.targetReps = tarReps;
		}

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            intrinsecus.InstructionLabel.Content = "Alternate left and right foot forward champ!";
          
            CameraSpacePoint leftAnkle = body.Joints[JointType.AnkleLeft].Position;
            CameraSpacePoint leftKnee = body.Joints[JointType.KneeLeft].Position;

            CameraSpacePoint rightAnkle = body.Joints[JointType.AnkleRight].Position;
            CameraSpacePoint rightKnee = body.Joints[JointType.KneeRight].Position;

            CameraSpacePoint centerHip = body.Joints[JointType.SpineBase].Position;
            CameraSpacePoint spine = body.Joints[JointType.SpineShoulder].Position;

            double leftLegAngle = MathUtil.CosineLaw(leftAnkle, centerHip, leftKnee);
            double rightLegAngle = MathUtil.CosineLaw(rightAnkle, centerHip, rightKnee);

            double leanL, leanR;
            leanL = MathUtil.CosineLaw(leftKnee, spine, centerHip);
            leanR = MathUtil.CosineLaw(rightKnee, spine, centerHip);
    

            if ((leftLegAngle > 140) && (rightLegAngle > 140))
            {
                if (state == Transition.DownToUp)
                {
                    reps++;
                    state = Transition.UpToDown;
                }
            }
            else if ((leftLegAngle < 100) || (rightLegAngle < 100))
            {
                if ((leanL < 70) || (leanL > 110))
                {
                    intrinsecus.InstructionLabel.Content = "Don't lean!";
                }
                else if (state == Transition.UpToDown)
                {
                    
                    intrinsecus.InstructionLabel.Content = "Great lunge! Good form";
                    state = Transition.DownToUp;
                    repFlashTicks = 0;
                }
               
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
            return targetReps;
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
