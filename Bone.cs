using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
    public class Bone
    {
        public Joint FirstJoint { get; private set; }
        public Joint SecondJoint { get; private set; }

        public Tuple<JointType, JointType> Joints { get; private set; }
        public Bone(Tuple<JointType, JointType> joints)
        {
            Joints = joints;
        }

        public void Update(IReadOnlyDictionary<JointType, Joint> joints) {
            if (joints.ContainsKey(Joints.Item1))
            {
	            FirstJoint = joints[Joints.Item1];
            }

            if (joints.ContainsKey(Joints.Item2))
            {
	            SecondJoint = joints[Joints.Item2];
            }
        }

        public static Bone Skull = new Bone(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
        public static Bone Cervical  = new Bone(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
        public static Bone UpperBack  = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
        public static Bone LowerBack = new Bone(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
        public static Bone ShoulderBladeRight = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
        public static Bone ShoulderBladeLeft = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
        public static Bone PelvisRight = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
        public static Bone PelvisLeft = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));
        public static Bone UpperArmRight = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
        public static Bone LowerArmRight = new Bone(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
        public static Bone PalmRight = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
        public static Bone FingerRight = new Bone(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
        public static Bone OpposableRight = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));
        public static Bone UpperArmLeft = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
        public static Bone LowerArmLeft= new Bone(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
        public static Bone PalmLeft = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
        public static Bone FingerLeft = new Bone(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
        public static Bone OpposableLeft = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
        public static Bone FemurRight = new Bone(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
        public static Bone CalfRight = new Bone(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
        public static Bone PedalRight = new Bone(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));
        public static Bone FemurLeft = new Bone(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
        public static Bone CalfLeft = new Bone(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
        public static Bone PedalLeft = new Bone(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft)); 

		public static Bone[] Bones =
		{
			Skull, Cervical, UpperBack, LowerBack, ShoulderBladeRight, ShoulderBladeLeft, PelvisRight,
			PelvisLeft, UpperArmRight, LowerArmRight, PalmRight, FingerRight, OpposableRight, 
			UpperArmLeft, LowerArmLeft, PalmLeft, FingerLeft, OpposableLeft, FemurRight, CalfRight, 
			PedalRight, FemurLeft, CalfLeft, PedalLeft
		};
    }
}
