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
        bool backstraight;
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
            repComplete = true;
            repcount = 0;
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

            double anklekneehip = MathUtil.CosineLaw(Ankle, SpineBase, Knee);
           
            TorsoStraight();
            intrinsecus.InstructionLabel.Content = "MathUtil.CosineLaw(Neck, SpineMid, SpineShoulder) = " + MathUtil.CosineLaw(Neck, SpineMid, SpineShoulder).ToString();
            if (state == Transition.DOWNTOUP)
            {
                //complete 
                if (backstraight && (Math.Abs(Neck.Y - SpineBase.Y) < 0.1))
                {
                    //now upright
                   // intrinsecus.InstructionLabel.Content = "Upright! Now go down!";
                    state = Transition.UPTODOWN;
                    if (repComplete == false) repComplete = true;

                }
                else
                {
                    //intrinsecus.InstructionLabel.Content = "Almost there, back straight and tall!";
                }
            }

            else
            {
                if (backstraight && (anklekneehip < 120 && anklekneehip > 90))
                {
                    //good
                  //  intrinsecus.InstructionLabel.Content = "You're all the way down. Now go up!";
                    state = Transition.DOWNTOUP;
                    if (repComplete == false)
                    {
                        repComplete = true;
                        repcount++;
                    }
                }
                else
                {
                //    intrinsecus.InstructionLabel.Content = "Get lower, get lower!";
                }
            }
            
            /*
             
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
 * */
            return repcount;
        }

        public int GetTargetReps()
        {
            return 10;
        }

        private void TorsoStraight()
        {
            if ((MathUtil.CosineLaw(Neck, SpineMid, SpineShoulder) > 170) && (MathUtil.CosineLaw(SpineShoulder, SpineBase, SpineMid) > 170)) backstraight = true;
            else backstraight =  false;
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
