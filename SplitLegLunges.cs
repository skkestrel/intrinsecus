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
        public int Update(Body body, DrawingContext ctx, Intrinsecus intrinsecus)
        {
            return 0;
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
