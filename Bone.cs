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
/*
    static Bone Skull = new Bone(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
    static Bone Cervical  = new Bone(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
    static Bone ShoulderBladeLight
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
static Bone  = new Bone(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft); */
    }

     
}
