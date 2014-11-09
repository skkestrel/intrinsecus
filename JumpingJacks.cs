using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhT.Intrinsecus
{
    class JumpingJacks : IExercise
    {
        public int Update(System.Windows.Media.DrawingContext ctx)
        {
            return 0;
        }

        public string getName()
        {
            return "Jumping Jack";
        }

        public string getPhoneticName()
        {
            return "jumping jacks";
        }
    }
}
