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
        private Transition state = Transition.DownToUp;
        private int reps = 0;
        private int targetReps;
        private int repFlashTicks = 4;
        private double prevLeftAngle;
        private double prevRightAngle;
        private Intrinsecus parent;

        enum Transition
        {
            UpToDown,
            DownToUp,
        }

        public ShoulderPresses(int goalReps, Intrinsecus parent)
		{
            parent.synth.SpeakAsync("Starting a set of " + GetPhoneticName());
            parent.InstructionLabel.Content = "None";
            parent.ExerciseLabel.Content = GetName();
            this.parent = parent;

            state = Transition.DownToUp;
            this.targetReps = goalReps;
		}

        ~ShoulderPresses()
        {
            Dispose();
        }

        public void Dispose()
        {
            parent.synth.SpeakAsync("Exercise finished");
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

            if ((leftAngle < 70) && (rightAngle < 70))
            {
                if (state == Transition.UpToDown)
                {
                    reps++;
                    intrinsecus.InstructionLabel.Content = "Great shoulder press, keep going!";
                    state = Transition.DownToUp;
                }
            }
            else if ((leftAngle > 130) && (rightAngle > 130) && (leftShoulder.Y < leftElbow.Y) && (rightShoulder.Y < rightElbow.Y))
            {
                if (state == Transition.DownToUp)
                {
                    state = Transition.UpToDown;
                    intrinsecus.InstructionLabel.Content = "";
                    repFlashTicks = 0;
                }
            }
            else if(reps>1)
            {
                if ((state == Transition.DownToUp) && ((leftAngle < prevRightAngle) || (rightAngle < prevRightAngle)))
                {
                    intrinsecus.InstructionLabel.Content = "You need to keep straightening those arms bro";
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

            prevLeftAngle = leftAngle;
            prevRightAngle = rightAngle;

            return reps;
        }

        public int GetTargetReps()
        {
            return targetReps;
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
