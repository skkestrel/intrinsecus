using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class JumpingJacks : IExercise
    {
        private Transition state;
        private int reps;
        private double prevGroin;
        private int targetReps;
        private Intrinsecus parent;

        public JumpingJacks(int tarReps, Intrinsecus parent)
        {
            parent.synth.SpeakAsync("Starting a set of " + GetPhoneticName());
            parent.InstructionLabel.Content = "None";
            parent.ExerciseLabel.Content = GetName();
            this.parent = parent;

            reps = 0;
            state = Transition.DOWNTOUP;
            this.targetReps = tarReps;
        }

        ~JumpingJacks()
        {
            Dispose();
        }

        public void Dispose()
        {
            parent.synth.SpeakAsync("Exercise finished");
        }

        enum Transition
        {
            UPTODOWN, 
            DOWNTOUP,
        }

        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            double aRight = MathUtil.CosineLaw(body.Joints[JointType.ElbowRight].Position, body.Joints[JointType.HipRight].Position, body.Joints[JointType.ShoulderRight].Position);
            double aLeft = MathUtil.CosineLaw(body.Joints[JointType.ElbowLeft].Position, body.Joints[JointType.HipLeft].Position, body.Joints[JointType.ShoulderLeft].Position);
            double groin = MathUtil.CosineLaw(body.Joints[JointType.FootLeft].Position, body.Joints[JointType.FootRight].Position, body.Joints[JointType.SpineBase].Position);

            if ((groin < 20) && (aRight < 30) && (aLeft < 30))
            {
                if (state == Transition.UPTODOWN)
                {
                    reps++;
                    intrinsecus.InstructionLabel.Content = "Great Jumping Jack, Bro!";
                    state = Transition.DOWNTOUP;
                }
            }
            else if ((groin > 50) && (aRight > 150) && (aLeft > 150))
            {
                if (state == Transition.DOWNTOUP)
                {
                    state = Transition.UPTODOWN;

                }
            }                   
            else if ((prevGroin > groin) && (state == Transition.DOWNTOUP))
            {
                intrinsecus.InstructionLabel.Content = "Make a bigger star, bro!";
            }

            prevGroin = groin;

            return reps;
        }

        public int GetTargetReps()
        {
            return targetReps;
        }

        public string GetName()
        {
            return "Jumping Jack";
        }

        public string GetPhoneticName()
        {
            return "jumping jacks";
        }
    }
}
