using System.Windows.Media;
using Microsoft.Kinect;
using System;

namespace EhT.Intrinsecus
{
    class Deadlifts : IExercise
    {
        Transition state;
        CameraSpacePoint Neck;
        CameraSpacePoint SpineShoulder;
        CameraSpacePoint SpineMid;
        CameraSpacePoint SpineBase;
        CameraSpacePoint Knee;
        CameraSpacePoint Ankle;
        enum Transition
        {
            UPTODOWN,
            DOWNTOUP
        }

        private int repcount;
        private bool repComplete;

        public Deadlifts()
        {
            state = Transition.DOWNTOUP;
        }


        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            Neck = body.Joints[JointType.Neck].Position;
			SpineShoulder = body.Joints[JointType.SpineShoulder].Position;
            SpineMid = body.Joints[JointType.SpineMid].Position;
            SpineBase = body.Joints[JointType.SpineBase].Position;
            
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
            if (state == Transition.DOWNTOUP)
            { 
                if (System.Math.Abs(SpineShoulder.Z - SpineBase.Z) < 0.10)
                    {
                        if (TorsoStraight())
                        {
                            repcount++;
                            repComplete = true;
                            intrinsecus.InstructionLabel.Content = "Back Straight, Great Rep. Good Form. Way to Go.";
                            state = Transition.UPTODOWN;
                        }
                }

                else intrinsecus.InstructionLabel.Content = "Bro, back ain't straight. You ain't gonna get no gains with that form.";
            }
              //logic down
            else
            {
                if (state == Transition.UPTODOWN)
                {
                    if (HipKnee < 0.10)
			        {
                        if (TorsoStraight()) { 
                            intrinsecus.InstructionLabel.Content = "Bro, your low. Good job. Tight. Nice Form.";
                            repComplete = false;
                            state = Transition.DOWNTOUP;
                        }
                    }
                    else
                    {
                        intrinsecus.InstructionLabel.Content = "Bro. Need to get lower. No pain, no gain.";
                    }
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
            if ((MathUtil.CosineLaw(Neck, SpineShoulder, SpineMid) > 170) && (MathUtil.CosineLaw(SpineShoulder, SpineMid, SpineBase) > 170)) return true;
            else return false;
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
