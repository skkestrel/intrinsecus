using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace EhT.Intrinsecus
{
    public class Squat : IExercise
    {
        public int reps;

        bool RepComplete;
        public Squat()
        {
            reps = 0;
        }

        public bool Update(System.Windows.Media.DrawingContext ctx)
    {
        
       Joint HipJoint = Bone.FemurLeft.FirstJoint;
       Joint KneeJoint = Bone.FemurRight.SecondJoint;

       float HipKnee = HipJoint.Position.Y - KneeJoint.Position.Y;
       
       if (HipKnee > 0) {
           if (RepComplete) {
                   reps++;
                RepComplete = false;
           }
       }
       else
       {
           RepComplete = true;
       }
       

      Pen HighlightPen = new Pen(Brushes.Green, 1);
      
    
        //if(HipJoint.y - KneeJoint.y)
           
       
        //case: angle between femur and calf is greater than 180
        //out: not low enough
        //if flag was tripped, reset flag

        // case: anglebetween femur and calf < 90 && > 75
        //out rep done, go up now.
        //if flag not tripped, add to rep
        
        //case: anglebetween femur < 75
            //warn too low

      return true;
    }


    }
}