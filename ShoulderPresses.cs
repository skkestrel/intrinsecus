using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class ShoulderPresses : IExercise
    {
        public int Update(Body body, System.Windows.Media.DrawingContext ctx)
        {
            return 0;
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
