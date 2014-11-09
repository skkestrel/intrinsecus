using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class VerticalJumpTest : IExercise
    {
        private int reps = 0;
        private double[] prevX = new double[3];
        private double[] prevY = new double[3];
        private double[] time = new double[3];

        private double startingY;
        private double highestJump;

        private double highestAcceleration;
        private double averageAcceleration;

        bool jumpDetected;

        Stopwatch watch;
        private Intrinsecus parent;

        int count;
        int targetCount;

        public VerticalJumpTest(int tarCount, Intrinsecus parent)
        {        
            parent.synth.SpeakAsync("Starting a set of " + GetPhoneticName());
            parent.InstructionLabel.Content = "None";
            parent.ExerciseLabel.Content = GetName();
            this.parent = parent;

            count = 0;
            jumpDetected = false;
            watch = new Stopwatch();
            watch.Start();
            highestAcceleration = 0;
            this.targetCount = tarCount;

        }

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            CameraSpacePoint leftFoot = body.Joints[JointType.FootLeft].Position;
            CameraSpacePoint rightFoot = body.Joints[JointType.FootRight].Position;

            int timeNow = watch.Elapsed.Milliseconds;

            prevX[2] = prevX[1];
            prevX[1] = prevX[0];
            prevX[0] = leftFoot.X;

            prevY[2] = prevY[1];
            prevY[1] = prevY[0];
            prevY[0] = leftFoot.Y;

            time[2] = time[1];
            time[1] = time[0];
            time[0] = (double)(timeNow/1000.0);

            if (!jumpDetected)
            {
                count++;

                if (count >= 3)
                {
                    double curAccelX = (prevX[0] - prevX[1])/(time[0] - time[1]) - (prevX[1] - prevX[2])/(time[1] - time[2]);
                    double curAccelY = (prevY[0] - prevY[1])/(time[0] - time[1]) - (prevY[1] - prevY[2])/(time[1] - time[2]);

                    if (curAccelY > 5)
                    {
                        startingY = prevY[2];
                        intrinsecus.InstructionLabel.Content = "Jump Detected";
                        jumpDetected = true;
                    }

                    if (curAccelY > highestAcceleration)
                    {
                        highestAcceleration = curAccelY;

                        //intrinsecus.InstructionLabel.Content = "Highest Acceleration: " + highestAcceleration.ToString();
                    }
                }
            }
            else
            {
                if ((prevY[2] > prevY[1]) && (prevY[1] > prevY[0]))
                {
                    highestJump = prevY[2] - startingY;
                    jumpDetected = false;
                    reps++;
                    intrinsecus.InstructionLabel.Content = "Jumped - " + highestJump + "m";
                }
            }

            return reps;
        }

        public int GetTargetReps()
        {
            return targetCount;
        }

        public string GetName()
        {
            return "Vertical Jump Test";
        }

        public string GetPhoneticName()
        {
            return "Vertical Jump Test";
        }
    }
}
