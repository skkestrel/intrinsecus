using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    class Bone
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }


        public Tuple<JointType, JointType> Joints { get; private set; }
        public Bone(Tuple<JointType, JointType> joints)
        {
            Joints = joints;
        }

        static Bone Skull = new Bone(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
        static Bone Cervical  = new Bone(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
        static Bone LowerBack = new Bone(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
        static Bone ShoulderBladeRight = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
        static Bone ShoulderBladeLeft = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
        static Bone PelvisRight = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
        static Bone PelvisLeft = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));
        static Bone UpperArmRight = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
        static Bone LowerArmRight = new Bone(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
        static Bone PalmRight = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
        static Bone FingerRight = new Bone(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
        static Bone OpposableRight = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));
        static Bone UpperArmLeft = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
        static Bone LowerArmLeft= new Bone(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
        static Bone PalmLeft = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
        static Bone FingerLeft = new Bone(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
        static Bone OpposableLeft = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
        static Bone FemurRight = new Bone(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
        static Bone CalfRight = new Bone(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
        static Bone PedalRight = new Bone(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));
        static Bone FemurLeft = new Bone(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
        static Bone CalfLeft = new Bone(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
        static Bone PedalLeft = new Bone(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft)); 
    }

     
}
