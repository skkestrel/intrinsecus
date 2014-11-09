using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
	/// <summary>
	/// Interaction logic for Intrinsecus
	/// </summary>
	public partial class Intrinsecus
	{
		/// <summary>
		/// Thickness of drawn joint lines
		/// </summary>
		private const double JointThickness = 3;

		/// <summary>
		/// Gets the bitmap to display
		/// </summary>
		public ImageSource ImageSource { get; private set; }

		/// <summary>
		/// Constant for clamping Z values of camera space points from being negative
		/// </summary>
		private const float InferredZPositionClamp = 0.1f;

		/// <summary>
		/// Thickness of clip edge rectangles
		/// </summary>
		private const double ClipBoundsThickness = 10;

		/// <summary>
		/// Pen used for drawing bones that are currently inferred
		/// </summary>        
		private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

		/// <summary>
		/// Drawing group for body rendering output
		/// </summary>
		private readonly DrawingGroup drawingGroup;

		/// <summary>
		/// Active Kinect sensor
		/// </summary>
		private KinectSensor kinectSensor;

		/// <summary>
		/// Speech recognition engine using audio data from Kinect.
		/// #JUSTHACKATHONTHINGS making what should be a private variable public
		/// </summary>
		public AudioSpeechEngine SpeechEngine;

		/// <summary>
		/// Coordinate mapper to map one type of point to another
		/// </summary>
		public readonly CoordinateMapper CoordinateMapper;

		/// <summary>
		/// Reader for body frames
		/// </summary>
		private readonly BodyFrameReader bodyFrameReader;

		/// <summary>
		/// color frame reader
		/// </summary>
		private readonly ColorFrameReader colorFrameReader;

		/// <summary>
		/// Array for the bodies
		/// </summary>
		public Body[] Bodies;

		/// <summary>
		/// Width of display (depth space)
		/// </summary>
		private readonly int displayWidth;

		/// <summary>
		/// Height of display (depth space)
		/// </summary>
		private readonly int displayHeight;

		/// <summary>
		/// List of colors for each body tracked
		/// </summary>
		private readonly List<Pen> bodyColors;

		/// <summary>
		/// synth
		/// </summary>
		public readonly SpeechSynthesizer synth;

		/// <summary>
		/// the current exercise in play
		/// </summary>
		public IExercise CurrentExercise;

		/// <summary>
		/// the current exercise in play
		/// </summary>
		public SelectionDialogue SingletonSelectionDialogue = null;

		/// <summary>
		/// Radius of drawn hand circles
		/// </summary>
		private const double HandSize = 30;

		/// <summary>
		/// color bitmap
		/// </summary>
		private readonly WriteableBitmap colorBitmap;

		/// <summary>
		/// brush for closed hand
		/// </summary>
		private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));

		/// <summary>
		/// Brush used for drawing hands that are currently tracked as opened
		/// </summary>
		private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(0, 0, 255, 0));

		/// <summary>
		/// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
		/// </summary>
		private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 255));

		/// <summary>
		/// Brush used for drawing joints that are currently tracked
		/// </summary>
		private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));

		/// <summary>
		/// Brush used for drawing joints that are currently inferred
		/// </summary>        
		private readonly Brush inferredJointBrush = Brushes.Yellow;

		/// <summary>
		/// Initializes a new instance of the Intrinsecus class.
		/// </summary>
		public Intrinsecus()
		{
			// one sensor is currently supported
			kinectSensor = KinectSensor.GetDefault();

			// get the coordinate mapper
			CoordinateMapper = kinectSensor.CoordinateMapper;

			// get the depth (display) extents
			FrameDescription frameDescription = kinectSensor.DepthFrameSource.FrameDescription;

			// get the depth (display) extents
			FrameDescription colorFrameDescription = kinectSensor.ColorFrameSource.FrameDescription;

			// get size of joint space
			displayWidth = frameDescription.Width;
			displayHeight = frameDescription.Height;

			if (kinectSensor != null)
			{
				SpeechEngine = new AudioSpeechEngine(kinectSensor);
				SpeechEngine.CommandRecieved += AudioCommandReceived;
			}

			// open the reader for the body frames
			bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

			// open the reader for the body frames
			colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();

			// populate body colors, one for each BodyIndex
			bodyColors = new List<Pen>
            {
				/*
	            new Pen(Brushes.Red, 6),
	            new Pen(Brushes.Orange, 6),
	            new Pen(Brushes.Green, 6),
	            new Pen(Brushes.Blue, 6),
	            new Pen(Brushes.Indigo, 6),
	            new Pen(Brushes.Violet, 6)
				*/
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)), 6),
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 255, 0)), 6),
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 0, 255, 0)), 6),
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 0, 255, 255)), 6),
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 0, 0, 255)), 6),
	            new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 0, 255)), 6),
            };

			// set IsAvailableChanged event notifier
			kinectSensor.IsAvailableChanged += Sensor_IsAvailableChanged;

			// open the sensor
			kinectSensor.Open();

			// Create the drawing group we'll use for drawing
			drawingGroup = new DrawingGroup();

			// Create an image source that we can use in our image control
			ImageSource = new DrawingImage(drawingGroup);

			// use the window object as the view model in simple example
			DataContext = this;

			colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

			synth = new SpeechSynthesizer();

			try
			{
				synth.SetOutputToDefaultAudioDevice();
			}
			catch (PlatformNotSupportedException)
			{
				synth = null;
			}

			// initialize the components (controls) of the window
			InitializeComponent();
		}

		/// <summary>
		/// Execute start up tasks
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void IntrinsecusWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (bodyFrameReader != null)
			{
				bodyFrameReader.FrameArrived += Reader_FrameArrived;
			}
			if (colorFrameReader != null)
			{
				colorFrameReader.FrameArrived += Reader_ColorFrameArrived;
			}

			// set the status text
			StatusLabel.Content = kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
															: Properties.Resources.NoSensorStatusText;

			RepCountLabel.Content = "";
		}

		/// <summary>
		/// Execute shutdown tasks
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void IntrinsecusWindow_Closing(object sender, CancelEventArgs e)
		{
			CurrentExercise = null;

			if (bodyFrameReader != null)
			{
				// BodyFrameReader is IDisposable
				bodyFrameReader.Dispose();
			}

			if (colorFrameReader != null)
			{
				colorFrameReader.Dispose();
			}

			if (kinectSensor != null)
			{
				kinectSensor.Close();
				kinectSensor = null;
			}
		}

		/// <summary>
		/// Handles the body frame data arriving from the sensor
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
		{
			bool dataReceived = false;

			using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
			{
				if (bodyFrame != null)
				{
					if (Bodies == null)
					{
						Bodies = new Body[bodyFrame.BodyCount];
					}

					// The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
					// As long as those body objects are not disposed and not set to null in the array,
					// those body objects will be re-used.
					bodyFrame.GetAndRefreshBodyData(Bodies);
					dataReceived = true;
				}
			}

			if (!dataReceived) return;

            if (this.SingletonSelectionDialogue != null) {

                this.SingletonSelectionDialogue.ViewReps();
            }
			using (DrawingContext dc = drawingGroup.Open())
			{
				// Draw a transparent background to set the render size
				dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, displayWidth, displayHeight));

				const float ratio = 0.325f;
				const int offsetX = 12;
				const int offsetY = 16;

				dc.DrawImage(colorBitmap,
					new Rect(-(colorBitmap.PixelWidth * ratio - displayWidth) / 2 - offsetX, -(colorBitmap.PixelHeight * ratio - displayHeight) / 2 - offsetY,
						colorBitmap.PixelWidth * ratio, colorBitmap.PixelHeight * ratio));

				int penIndex = 0;
				foreach (Body body in Bodies)
				{
					Pen drawPen = bodyColors[penIndex++];

					if (body.IsTracked)
					{
						DrawClippedEdges(body, dc);

						IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

						// convert the joint points to depth (display) space
						Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

						foreach (JointType jointType in joints.Keys)
						{
							// sometimes the depth(Z) of an inferred joint may show as negative
							// clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
							CameraSpacePoint position = joints[jointType].Position;
							if (position.Z < 0)
							{
								position.Z = InferredZPositionClamp;
							}

							DepthSpacePoint depthSpacePoint = CoordinateMapper.MapCameraPointToDepthSpace(position);
							jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
						}

						DrawBody(joints, jointPoints, dc, drawPen);

						DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
						DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);

						if (CurrentExercise == null)
						{
							RepCountLabel.Content = "";
							continue;
						}

						int t = CurrentExercise.Update(body, dc, this);
						RepCountLabel.Content = t.ToString(CultureInfo.InvariantCulture) + "/"
							+ CurrentExercise.GetTargetReps().ToString(CultureInfo.InvariantCulture);
						if (t >= CurrentExercise.GetTargetReps())
						{
                            synth.SpeakAsync("Exercise finished");
                            SetExercise(null);
                            InstructionLabel.Content = "None";
                            ExerciseLabel.Content = "None";
						}
					}
				}

				// prevent drawing outside of our render area
				drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, displayWidth, displayHeight));
			}
		}

		/// <summary>
		/// Handles the color frame data arriving from the sensor
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
		{
			// ColorFrame is IDisposable
			using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
			{
				if (colorFrame != null)
				{
					FrameDescription colorFrameDescription = colorFrame.FrameDescription;

					using (colorFrame.LockRawImageBuffer())
					{
						colorBitmap.Lock();

						// verify data and write the new color frame data to the display bitmap
						if ((colorFrameDescription.Width == colorBitmap.PixelWidth) && (colorFrameDescription.Height == colorBitmap.PixelHeight))
						{
							colorFrame.CopyConvertedFrameDataToIntPtr(
								colorBitmap.BackBuffer,
								(uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
								ColorImageFormat.Bgra);

							colorBitmap.AddDirtyRect(new Int32Rect(0, 0, colorBitmap.PixelWidth, colorBitmap.PixelHeight));
						}

						colorBitmap.Unlock();
					}
				}
			}
		}

		public Point CameraToScreen(CameraSpacePoint point)
		{
			if (point.Z < 0)
			{
				point.Z = 0.1f;
			}

			DepthSpacePoint depthSpacePoint1 = CoordinateMapper.MapCameraPointToDepthSpace(point);

			return new Point(depthSpacePoint1.X, depthSpacePoint1.Y);
		}

		public void SetExercise(IExercise e, int reps = 10)
		{
			CurrentExercise = e;
		}

		void AudioCommandReceived(object sender, AudioCommandEventArgs e)
		{
			switch (e.command)
			{
				// not implemented BACK, ENTER, SQUAT, DEADLIFT, LUNGES, SHOULDERPRESS
				case AudioCommand.SELECT:
					if (SingletonSelectionDialogue == null)
					{
						SingletonSelectionDialogue = new SelectionDialogue(this);
						SingletonSelectionDialogue.Show();
					}

					break;
			}
		}

		/// <summary>
		/// Draws a body
		/// </summary>
		/// <param name="joints">joints to draw</param>
		/// <param name="jointPoints">translated positions of joints to draw</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		/// <param name="drawingPen">specifies color to draw a specific body</param>
		private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
		{
			// Draw the bones
			foreach (var bone in Bone.Bones)
			{
				DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
			}

			// Draw the joints
			foreach (JointType jointType in joints.Keys)
			{
				Brush drawBrush = null;

				TrackingState trackingState = joints[jointType].TrackingState;

				if (trackingState == TrackingState.Tracked)
				{
					drawBrush = trackedJointBrush;
				}
				else if (trackingState == TrackingState.Inferred)
				{
					drawBrush = inferredJointBrush;
				}

				if (drawBrush != null)
				{
					drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
				}
			}
		}

		/// <summary>
		/// Draws one bone of a body (joint to joint)
		/// </summary>
		/// <param name="joints">joints to draw</param>
		/// <param name="jointPoints">translated positions of joints to draw</param>
		/// <param name="jointType0">first joint of bone to draw</param>
		/// <param name="jointType1">second joint of bone to draw</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		/// /// <param name="drawingPen">specifies color to draw a specific bone</param>
		private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
		{
			Joint joint0 = joints[jointType0];
			Joint joint1 = joints[jointType1];

			// If we can't find either of these joints, exit
			if (joint0.TrackingState == TrackingState.NotTracked ||
				joint1.TrackingState == TrackingState.NotTracked)
			{
				return;
			}

			// We assume all drawn bones are inferred unless BOTH joints are tracked
			Pen drawPen = inferredBonePen;
			if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
			{
				drawPen = drawingPen;
			}

			drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
		}

		/// <summary>
		/// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
		/// </summary>
		/// <param name="handState">state of the hand</param>
		/// <param name="handPosition">position of the hand</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
		{
			switch (handState)
			{
				case HandState.Closed:
					drawingContext.DrawEllipse(handClosedBrush, null, handPosition, HandSize, HandSize);
					break;

				case HandState.Open:
					drawingContext.DrawEllipse(handOpenBrush, null, handPosition, HandSize, HandSize);
					break;

				case HandState.Lasso:
					drawingContext.DrawEllipse(handLassoBrush, null, handPosition, HandSize, HandSize);
					break;
			}
		}

		/// <summary>
		/// Draws indicators to show which edges are clipping body data
		/// </summary>
		/// <param name="body">body to draw clipping information for</param>
		/// <param name="drawingContext">drawing context to draw to</param>
		private void DrawClippedEdges(Body body, DrawingContext drawingContext)
		{
			FrameEdges clippedEdges = body.ClippedEdges;

			if (clippedEdges.HasFlag(FrameEdges.Bottom))
			{
				drawingContext.DrawRectangle(
					Brushes.Red,
					null,
					new Rect(0, displayHeight - ClipBoundsThickness, displayWidth, ClipBoundsThickness));
			}

			if (clippedEdges.HasFlag(FrameEdges.Top))
			{
				drawingContext.DrawRectangle(
					Brushes.Red,
					null,
					new Rect(0, 0, displayWidth, ClipBoundsThickness));
			}

			if (clippedEdges.HasFlag(FrameEdges.Left))
			{
				drawingContext.DrawRectangle(
					Brushes.Red,
					null,
					new Rect(0, 0, ClipBoundsThickness, displayHeight));
			}

			if (clippedEdges.HasFlag(FrameEdges.Right))
			{
				drawingContext.DrawRectangle(
					Brushes.Red,
					null,
					new Rect(displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, displayHeight));
			}
		}

		/// <summary>
		/// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
		{
			// on failure, set the status text
			StatusLabel.Content = kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
															: Properties.Resources.SensorNotAvailableStatusText;
		}

		private void SelectionDialogueButton_Click(object sender, RoutedEventArgs e)
		{
			new SelectionDialogue(this).Show();
		}
	}
}
