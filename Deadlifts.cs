using System.Windows.Media;
using Microsoft.Kinect;
using System;

namespace EhT.Intrinsecus
{
    class Deadlifts : IExercise
    {
        private int repcount;

        private bool repComplete;


        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            CameraSpacePoint Neck = body.Joints[JointType.Neck].Position;
			CameraSpacePoint SpineShoulder = body.Joints[JointType.SpineShoulder].Position;
            CameraSpacePoint SpineMid = body.Joints[JointType.SpineMid].Position;
            CameraSpacePoint SpineBase = body.Joints[JointType.SpineBase].Position;
            CameraSpacePoint Knee;
            CameraSpacePoint Ankle;

            float HipPoint = body.Joints[JointType.HipLeft].Position.Y;
			float KneePoint = body.Joints[JointType.KneeLeft].Position.Y;

			float HipKnee = HipPoint - KneePoint;

           
            //get camera coordinate positions for all required body parts

            if (body.Joints[JointType.KneeLeft].Position.Z < body.Joints[JointType.KneeRight].Position.Z)
            {
                Knee = body.Joints[JointType.KneeLeft].Position;
                Ankle = body.Joints[JointType.AnkleLeft].Position;
            }
            else
            {
                Knee = body.Joints[JointType.KneeRight].Position;
                Ankle = body.Joints[JointType.AnkleRight].Position;

            }

            //logic up
            //check z (depth)
            if (System.Math.Abs(SpineShoulder.Z - SpineBase.Z) < 0.10)
            {
                if ((System.Math.Abs(Neck.X - SpineShoulder.X) < .10) && (System.Math.Abs(SpineShoulder.X - SpineMid.X) < .10) && (System.Math.Abs(SpineMid.X - SpineBase.X) < .10))
                {
                    repcount++;
                    repComplete = true;
                }
            }
              //logic down
            else
            {
                if (HipKnee < 0.05)
			    {
				repComplete = false;
	//	repflashtick = 0;
			    }

            }

            //if neck depth = spine shoulder depth = spine mid depth = spine base depth -> good/continue
                //else -- print warning message to screen and highlight those as red

            //up

            //down



            return 0;
        }

        private bool TorsoStraight()
        {
            return true;
        }

        public string GetName()
        {
            return "Deadlift";
        }

        public string GetPhoneticName()
        {
            return "dead lifts";
        }
    }
}
