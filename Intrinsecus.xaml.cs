using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace EhT.Intrinsecus
{
	/// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource { get; private set; }

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

		/// <summary>
		/// Zpos clamp
		/// </summary>
		private const float ZPosClamp = 0.1F;

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
        /// </summary>
        private AudioSpeechEngine speechEngine;

		/// <summary>
		/// Coordinate mapper to map one type of point to another
		/// </summary>
		private readonly CoordinateMapper coordinateMapper;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies;

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
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            coordinateMapper = kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            displayWidth = frameDescription.Width;
            displayHeight = frameDescription.Height;

            if (kinectSensor != null)
            {
                speechEngine = new AudioSpeechEngine(kinectSensor);
                speechEngine.CommandRecieved += AudioCommandReceived;
            }

            // open the reader for the body frames
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

	        // populate body colors, one for each BodyIndex
            bodyColors = new List<Pen>
            {
	            new Pen(Brushes.Red, 6),
	            new Pen(Brushes.Orange, 6),
	            new Pen(Brushes.Green, 6),
	            new Pen(Brushes.Blue, 6),
	            new Pen(Brushes.Indigo, 6),
	            new Pen(Brushes.Violet, 6)
            };

	        // set IsAvailableChanged event notifier
            kinectSensor.IsAvailableChanged += Sensor_IsAvailableChanged;

            // open the sensor
            kinectSensor.Open();

            // Create the drawing group we'll use for drawing
            drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            ImageSource = new DrawingImage(drawingGroup);

            // use the window object as the view model in this simple example
            DataContext = this;

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

            // set the status text
            StatusLabel.Content = kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
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
                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

	        if (!dataReceived) return;

	        using (DrawingContext dc = drawingGroup.Open())
	        {
		        // Draw a transparent background to set the render size
		        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, displayWidth, displayHeight));

		        int penIndex = 0;

		        foreach (Body body in bodies)
		        {
			        Pen drawPen = bodyColors[penIndex++];

			        if (!body.IsTracked) continue;

			        DrawClippedEdges(body, dc);

					foreach (Bone bone in Bone.Bones)
			        {
						bone.Update(body.Joints);
			        }

			        DrawBody(dc, drawPen);
		        }

		        // prevent drawing outside of our render area
		        drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, displayWidth, displayHeight));
	        }
        }

        void AudioCommandReceived(object sender, AudioCommandEventArgs e)
        {
            switch (e.command)
            {
                case AudioCommand.BACK:
                    break;
                case AudioCommand.ENTER:
                    break;
                case AudioCommand.SQUAT:
                    ExerciseLabel.Content = "Squat";
                    break;
                case AudioCommand.DEADLIFT:
                    ExerciseLabel.Content = "Deadlift";
                    break;
                case AudioCommand.LUNGES:
                    ExerciseLabel.Content = "Lunges";
                    break;
                case AudioCommand.SHOULDERPRESS:
                    ExerciseLabel.Content = "Shoulder Press";
                    break;
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
		private void DrawBody(DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (Bone bone in Bone.Bones)
            {
                DrawBone(bone, drawingContext, drawingPen);
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
		/// <param name="bone">the bone</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
		private void DrawBone(Bone bone, DrawingContext drawingContext, Pen drawingPen)
        {
            // If we can't find either of these joints, exit
            if (bone.FirstJoint.TrackingState == TrackingState.NotTracked ||
                bone.SecondJoint.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if ((bone.FirstJoint.TrackingState == TrackingState.Tracked) && (bone.FirstJoint.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

	        CameraSpacePoint point1 = bone.FirstJoint.Position;
			if (point1.Z < 0f)
			{
				point1.Z = ZPosClamp;
			}
	        CameraSpacePoint point2 = bone.SecondJoint.Position;
			if (point2.Z < 0f)
			{
				point2.Z = ZPosClamp;
			}

	        DepthSpacePoint firstDepthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(point1);
	        DepthSpacePoint secondDepthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(point2);
	        Point firstPoint = new Point(firstDepthSpacePoint.X, firstDepthSpacePoint.Y);
	        Point secondPoint = new Point(secondDepthSpacePoint.X, secondDepthSpacePoint.Y);

	        drawingContext.DrawLine(drawPen, firstPoint, secondPoint);
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
            new SelectionDialogue().Show();
        }
    }
}
