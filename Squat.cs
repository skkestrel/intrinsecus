using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Speech.Synthesis;

namespace EhT.Intrinsecus
{
	public class Squat : IExercise
	{
		private int repflashtick = 0;
		private int reps = 0;
        private int targetReps;
        private Intrinsecus parent;

		private bool repComplete;

        enum Transition
        {
            UPTODOWN,
            DOWNTOUP,
        }

        private Transition state = Transition.UPTODOWN;

        public Squat(int tarReps, Intrinsecus parent)
        {
            parent.synth.SpeakAsync("Starting a set of " + GetPhoneticName());
            parent.InstructionLabel.Content = "None";
            parent.ExerciseLabel.Content = GetName();
            this.parent = parent;

			reps = 0;
            this.targetReps = tarReps;
		}

        ~Squat()
        {
			parent.synth.SpeakAsync("Exercise finished");
			parent.InstructionLabel.Content = "None";
			parent.ExerciseLabel.Content = "None";
        }

		public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
		    
            //left side
			float HipPointL = body.Joints[JointType.HipLeft].Position.Y;
			float KneePointL = body.Joints[JointType.KneeLeft].Position.Y;

            //right side
            float HipPointR = body.Joints[JointType.HipRight].Position.Y;
            float KneePointR = body.Joints[JointType.KneeRight].Position.Y;

			float HipKneeL = HipPointL - KneePointL;
            float HipKneeR = HipPointR - KneePointR;

            double HipToKneeAngle = MathUtil.CosineLaw(body.Joints[JointType.HipRight].Position, body.Joints[JointType.AnkleRight].Position, body.Joints[JointType.KneeRight].Position);

            
            // y dimension logic up
			if (HipKneeL > 0.20 && HipKneeR > 0.20)
			{
                if (state == Transition.DOWNTOUP)
                {
                    reps++;
                    state = Transition.UPTODOWN;
                } 
				
			}
            // y dimension logic down
			else if (HipKneeL < 0.10 && HipKneeR < 0.10)
			{
                if (state == Transition.UPTODOWN)
                {
                   
                    if (HipToKneeAngle <= 90)
                    {
                  
                        intrinsecus.InstructionLabel.Content = "Great squat";
                        state = Transition.DOWNTOUP;
                        repflashtick = 0;
                    }
                    else
                    {
                        intrinsecus.InstructionLabel.Content = "Squat form not low enough. Sit bro";
                    }
                }
			}
          
            

           


            //case: angle between femur and calf is greater than 180
            //out: not low enough
            //if flag was tripped, reset flag

            // case: anglebetween femur and calf < 90 && > 75
            //out rep done, go up now.
            //if flag not tripped, add to rep

            //case: anglebetween femur < 75
            //warn too low
            //  Debug.WriteLine("{0}", reps);

			if (repflashtick++ <= 3)
			{
				Pen highlightPen = new Pen(Brushes.Green, 10);

				CameraSpacePoint position1 = body.Joints[JointType.HipLeft].Position;
				CameraSpacePoint position2 = body.Joints[JointType.KneeLeft].Position;

                CameraSpacePoint position3 = body.Joints[JointType.HipRight].Position;
                CameraSpacePoint position4 = body.Joints[JointType.KneeRight].Position;

				if (position1.Z < 0)
				{
					position1.Z = 0.1f;
				}
				if (position2.Z < 0)
				{
					position2.Z = 0.1f;
				}
                if (position3.Z < 0)
                {
                    position3.Z = 0.1f;
                }
                if (position4.Z < 0)
                {
                    position4.Z = 0.1f;
                }

				DepthSpacePoint depthSpacePoint1 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position1);
				DepthSpacePoint depthSpacePoint2 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position2);
                DepthSpacePoint depthSpacePoint3 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position3);
                DepthSpacePoint depthSpacePoint4 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position4);

				ctx.DrawLine(highlightPen, new Point(depthSpacePoint1.X, depthSpacePoint1.Y),
					new Point(depthSpacePoint2.X, depthSpacePoint2.Y));
                ctx.DrawLine(highlightPen, new Point(depthSpacePoint3.X, depthSpacePoint3.Y),
                    new Point(depthSpacePoint4.X, depthSpacePoint4.Y));
			}

	
			
            if (reps == 5) intrinsecus.synth.SpeakAsync("Only five left, you can do it!");
            if (reps == 3) intrinsecus.synth.SpeakAsync("Three left, almost there!");
            if (reps == 1) intrinsecus.synth.SpeakAsync("One left...");
            if (reps == 0) intrinsecus.synth.SpeakAsync("You did your squats! Congratulations!");
            return reps;
		}

        public int GetTargetReps()
        {
            return targetReps;
        }

		public string GetName()
		{
			return "Squat";
		}

		public string GetPhoneticName()
		{
			return "Squats";
		}
	}
}
