using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhT.Intrinsecus
{
    class SplitLegLunges : IExercise
    {
        public int Update(System.Windows.Media.DrawingContext ctx)
        {
            return 0;
        }

        public string getName()
        {
            return "Split Leg Lunge";
        }

        public string getPhoneticName()
        {
            return "split leg lunges";
        }
    }
}
