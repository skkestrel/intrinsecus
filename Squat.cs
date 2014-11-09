using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace EhT.Intrinsecus
{
	public class Squat : IExercise
	{
		private int repflashtick = 0;
		private int reps = 0;

		private bool repComplete;

		public Squat()
		{
			reps = 0;
		}

		public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
		{
			float HipPoint = body.Joints[JointType.HipLeft].Position.Y;
			float KneePoint = body.Joints[JointType.KneeLeft].Position.Y;

			float HipKnee = HipPoint - KneePoint;

			if (HipKnee > 0)
			{
				if (repComplete)
				{
					reps++;
					repComplete = false;
				}
			}
			else
			{
				repComplete = true;
				repflashtick = 0;
			}

			if (repflashtick++ <= 3)
			{
				Pen highlightPen = new Pen(Brushes.Green, 10);

				CameraSpacePoint position1 = body.Joints[JointType.HipLeft].Position;
				CameraSpacePoint position2 = body.Joints[JointType.KneeLeft].Position;

				if (position1.Z < 0)
				{
					position1.Z = 0.1f;
				}
				if (position2.Z < 0)
				{
					position2.Z = 0.1f;
				}

				DepthSpacePoint depthSpacePoint1 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position1);
				DepthSpacePoint depthSpacePoint2 = intrinsecus.CoordinateMapper.MapCameraPointToDepthSpace(position2);

				ctx.DrawLine(highlightPen, new Point(depthSpacePoint1.X, depthSpacePoint1.Y),
					new Point(depthSpacePoint2.X, depthSpacePoint2.Y));
			}

			//if(HipJoint.y - KneeJoint.y)


			//case: angle between femur and calf is greater than 180
			//out: not low enough
			//if flag was tripped, reset flag

			// case: anglebetween femur and calf < 90 && > 75
			//out rep done, go up now.
			//if flag not tripped, add to rep

			//case: anglebetween femur < 75
			//warn too low
			//  Debug.WriteLine("{0}", reps);
			return reps;
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
