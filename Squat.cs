using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhT.Intrinsecus
{
    class Squat:IExercise
    {
        public int Update(System.Windows.Media.DrawingContext ctx)
        {
            return 0;
        }

        public string getName()
        {
            return "Squat";
        }

        public string getPhoneticName()
        {
            return "Squats";
        }
    }
}
