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
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

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

					/*
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

				        DepthSpacePoint depthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(position);
				        jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
			        }
					*/

			        DrawBody(dc, drawPen);
		        }

		        // prevent drawing outside of our render area
		        drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, displayWidth, displayHeight));
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

			/*
            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                switch (trackingState)
                {
	                case TrackingState.Tracked:
		                drawBrush = trackedJointBrush;
		                break;
	                case TrackingState.Inferred:
		                drawBrush = inferredJointBrush;
		                break;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
			*/
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

			DepthSpacePoint firstDepthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(bone.FirstJoint.Position);
			DepthSpacePoint secondDepthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(bone.SecondJoint.Position);
	        Point firstPoint = new Point(firstDepthSpacePoint.X, firstDepthSpacePoint.Y);
	        Point secondPoint = new Point(secondDepthSpacePoint.X, secondDepthSpacePoint.Y);

	        drawingContext.DrawLine(drawPen, firstPoint, secondPoint);
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
            new SelectionDialogue().Show();
        }
    }
}
