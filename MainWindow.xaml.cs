// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//Code-behind
//Edited by: Edward Jack Chandler
//Student Number: 120232420

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Reflection;
using Kinect12_02_15;
using System.Windows.Threading;
using Microsoft.Win32;

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Instances of Kinect
        KinectSensor old;
        KinectSensor sensor;

        //Timers used
        DispatcherTimer exerciseTimer;
        DispatcherTimer countDownTimer;

        //timer integers
        int time = 0;
        int countDownTime = 10;

        public MainWindow()
        {
            //Initialise timers for WPF
            InitializeComponent();

            exerciseTimer = new DispatcherTimer();
            exerciseTimer.Interval = TimeSpan.FromSeconds(1);
            exerciseTimer.Tick += exerciseTimerTick;

            countDownTimer = new DispatcherTimer();
            countDownTimer.Interval = TimeSpan.FromSeconds(1);
            countDownTimer.Tick += countDownTick;

        }


        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        bool calibrate = false;
        Position scale;
        Joint joint;

        //current Position for head and hip
        Position hip = new Position();
        Position head = new Position();

        //Every different type of Exercise
        Exercise currentExercise = new Exercise("left arm");
        Exercise leftArm = new Exercise("left arm");
        Exercise rightArm = new Exercise("right arm");
        Exercise leftLeg = new Exercise("left leg");
        Exercise rightLeg = new Exercise("right leg");
        Exercise seatedLeftArm = new Exercise("left arm seated");
        Exercise seatedRightArm = new Exercise("right arm seated");

        //counter for the number of frames
        int frameCounter = 0;
        //when the count down should stop
        int countDownStop = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        //Checks if the instance of the Kinect has changed
        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            old = (KinectSensor)e.OldValue;

            StopKinect(old);

            sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }


            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };


            //enable the skeleton stream
            sensor.SkeletonStream.Enable(parameters);

            sensor.SkeletonStream.Enable();

            // subscribe to all frames ready
            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);

            //enable depth and colour stream
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            try
            {
                sensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }


        }

        //Check whether all frames from each data stream are ready - main loop
        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            //Get a skeleton
            Skeleton s = GetFirstSkeleton(e);

            if (s == null)
            {
                return;
            }

            //If it is a seated exercise, enable seated tracking mode and make the hip ellipse disappear
            if (currentExercise.getSeated() == true)
            {
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                hipEllipse.Opacity = 0;
            }

            //else make all other ellipses appear, and turn skeleton tracking mode to default
            else
            {
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                leftKneeEllipse.Opacity = 1;
                rightKneeEllipse.Opacity = 1;
                hipEllipse.Opacity = 1;
                leftFootEllipse.Opacity = 1;
                rightFootEllipse.Opacity = 1;
            }

            //If a skeleton is tracked
            if (s.TrackingState == SkeletonTrackingState.Tracked)
            {
                //set initial joints for head and hip
                setJointPositions(s, JointType.HipCenter, hip);
                setJointPositions(s, JointType.Head, head);

                //If standing exercise and not calibrated
                if (currentExercise.getSeated() == false & calibrate == false)
                {
                    //Check whether you right distance away to start exerise
                    if (hip.withinPositionZ(currentExercise.getStartingPosition(), 0.05))
                    {
                        calibrate = true;
                        // start countdown timer, and make video disappear
                        countDownTimer.Start();
                        Instructional_Video_Player.Stop();
                        Instructional_Video_Player.Opacity = 0;
                    }
                }

                //else if seated
                else if (currentExercise.getSeated() == true & calibrate == false)
                {
                    //Check whether you right distance away to start exerise
                    if (head.withinPositionZ(currentExercise.getStartingPosition(), 0.05))
                    {
                        calibrate = true;
                        // start countdown timer, and make video disappear
                        countDownTimer.Start();
                        Instructional_Video_Player.Stop();
                        Instructional_Video_Player.Opacity = 0;
                    }
                }
                
                //If stood right distance away from Kinect
                if (calibrate == true)
                {
                    //and still first frame
                    if (frameCounter == 0)
                    {
                        //if seated exercise
                        if (currentExercise.getSeated() == false)
                        {
                            //Set joints on the hip and work out the distances needed to translate positions. Hold values in Position scale.
                            setJointPositions(s, JointType.HipCenter, hip);
                            setJointPositions(s, currentExercise.getJointType(), currentExercise.getCurrentPosition());
                            scale = positionScale(hip);
                        }
                        
                            //if seated
                        else if (currentExercise.getSeated() == true)
                        {
                            //Set joints on the head and work out the distances needed to translate positions. Hold values in Position scale.
                            setJointPositions(s, JointType.Head, head);
                            setJointPositions(s, currentExercise.getJointType(), currentExercise.getCurrentPosition());
                            scale = positionScale(head);
                        }

                        //Set previous position as current position
                        currentExercise.setPreviousPosition(currentExercise.getCurrentPosition());
                    }

                    //if count down should stop
                    if (countDownTime == countDownStop)
                    {
                        //start comparison algorithm
                        comparisonAlgorithm(s, currentExercise);

                    }

                }
            }
            //get points on camera to set ellipses to
            GetCameraPoint(s, e);

        }


        void comparisonAlgorithm(Skeleton s, Exercise e)
        {
            //stop count down, start exercise timer
            countDownTimer.Stop();
            exerciseTimer.Start();
            //set positions and correct any inaccuracies above the offset  of 0.2
            setJointPositions(s, e, 0.2);

            //if exercise not finished
            if (frameCounter < 300)
            {
                //translate the model positions
                e.setModelPosition(frameCounter);
                e.getModelPosition().setX(e.getModelPosition().getX() - scale.getX());
                e.getModelPosition().setY(e.getModelPosition().getY() - scale.getY());
                //set percentage values for the current frame
                percentageComparedBox(currentExercise);
                //set previous position as current position
                currentExercise.setPreviousPosition(currentExercise.getCurrentPosition());
                //next frame
                frameCounter++;
            }


            //if exercise finished
            if (frameCounter == 300)
            {
                //display score
                score(e);
                //increment frame counter by one to finish exercise
                frameCounter++;
            }
        }

        //Converts joint location and gets the points to display on the colour stream, then sets the ellipses to track a user
        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {

            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }


                //Map a joint location to a point on the depth map
                //head
                DepthImagePoint headDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                //left hand
                DepthImagePoint leftDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //left knee
                DepthImagePoint leftKneeDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.KneeLeft].Position);
                //right knee
                DepthImagePoint rightKneeDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.KneeRight].Position);
                //hip
                DepthImagePoint hipDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HipCenter].Position);
                //left foot
                DepthImagePoint leftFootDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.FootLeft].Position);
                DepthImagePoint rightFootDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.FootRight].Position);

                //Map a depth point to a point on the color image
                //head
                ColorImagePoint headColorPoint =
                    depth.MapToColorImagePoint(headDepthPoint.X, headDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left hand
                ColorImagePoint leftColorPoint =
                    depth.MapToColorImagePoint(leftDepthPoint.X, leftDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightColorPoint =
                    depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left hand
                ColorImagePoint leftKneeColorPoint =
                    depth.MapToColorImagePoint(leftKneeDepthPoint.X, leftKneeDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightKneeColorPoint =
                    depth.MapToColorImagePoint(rightKneeDepthPoint.X, rightKneeDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //hip
                ColorImagePoint hipColorPoint =
                    depth.MapToColorImagePoint(hipDepthPoint.X, hipDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left foot
                ColorImagePoint leftFootColorPoint =
                    depth.MapToColorImagePoint(leftFootDepthPoint.X, leftFootDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right foot
                ColorImagePoint rightFootColorPoint =
                    depth.MapToColorImagePoint(rightFootDepthPoint.X, rightFootDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);

                //Set location
                CameraPosition(headImage, headColorPoint);
                CameraPosition(leftEllipse, leftColorPoint);
                CameraPosition(rightEllipse, rightColorPoint);
                CameraPosition(leftKneeEllipse, leftKneeColorPoint);
                CameraPosition(rightKneeEllipse, rightKneeColorPoint);
                CameraPosition(hipEllipse, hipColorPoint);
                CameraPosition(leftFootEllipse, leftFootColorPoint);
                CameraPosition(rightFootEllipse, rightFootColorPoint);

            }
        }

        //Gets the skeleton from the skeleton array (API always returns 6)
        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        //Used to stop the Kinect sensor
        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }


                }
            }
        }

        //Used to set the ellipse to a point on the colour camera
        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            //Divide by 2 for width and height so point is right in the middle 
            // instead of in top/left corner
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);

        }

        //if the window closes stop the kinect
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }

        //correcting inaccuracies and sets positions
        private void setJointPositions(Skeleton s, Exercise e, double offset)
        {
            joint = s.Joints[e.getJointType()];
            Position jointPos = new Position(joint.Position.X, joint.Position.Y, joint.Position.Z);
            //Takes joint tracked by Kinect and makes a copy of the xyz positions
            e.setCurrentPosition(joint.Position.X, joint.Position.Y, joint.Position.Z);

            //If X is out of position use previous X
            if (!e.getCurrentPosition().withinPositionX(e.getPreviousPosition(), offset))
            {
                e.setCurrentPosition(e.getPreviousPosition().getX(), e.getCurrentPosition().getY(), e.getCurrentPosition().getZ());
            }

            //If Y is out of position use previous Y
            if (!e.getCurrentPosition().withinPositionY(e.getPreviousPosition(), offset))
            {
                e.setCurrentPosition(e.getCurrentPosition().getX(), e.getPreviousPosition().getY(), e.getCurrentPosition().getZ());
            }

            e.getCurrentPosition().printPosition();

        }

        //sets joint positions without correcting inaccuracies
        private void setJointPositions(Skeleton s, JointType t, Position currentPos)
        {
            Joint joint = s.Joints[t];
            Position jointPos = new Position(joint.Position.X, joint.Position.Y, joint.Position.Z);

            currentPos.setPosition(joint.Position.X, joint.Position.Y, joint.Position.Z);

        }

        //used to set the joint positions whilst I recorded of model exercises, without the use of an exercise
        private void setJointPositionsRecord(Skeleton s, JointType t, Position currentPos, Position previousPos, double offset)
        {
            Joint joint = s.Joints[t];
            Position jointPos = new Position(joint.Position.X, joint.Position.Y, joint.Position.Z);

            currentPos.setPosition(joint.Position.X, joint.Position.Y, joint.Position.Z);

            //If X is out of position use previous X
            if (!jointPos.withinPositionX(previousPos, offset))
            {
                currentPos.setPosition(previousPos.getX(), currentPos.getY(), currentPos.getZ());
            }

            //If Y is out of position use previous Y
            if (!jointPos.withinPositionY(previousPos, offset))
            {
                currentPos.setPosition(currentPos.getX(), previousPos.getY(), currentPos.getZ());
            }

        }

        //save model exercises to a text file
        private void saveValues(String file, Position p)
        {
            File.AppendAllText(file, p.getX() + ", " + p.getY() + ", " + p.getZ() + Environment.NewLine);
        }

        //save percentages for evaluation
        private void savePercentage(String file, double percentage)
        {
            File.AppendAllText(file, percentage + Environment.NewLine);
        }

        //create the box shown in designs to turn the comparison of two points into a percentage
        private void percentageComparedBox(Exercise e)
        {
            Position difference = e.getModelPosition().absDifferenceBetweenPositions(e.getCurrentPosition());

            double percentageBoxLength = currentExercise.getLeniency();
            double dX = difference.getX();
            double dY = difference.getY();
            double percentageX;
            double percentageY;

            //if the current position not within percentage box, add 1% to the total
            if (dX >= percentageBoxLength / 2 || dY >= percentageBoxLength / 2)
            {
                currentExercise.addToTotalPercentageX(1);
                currentExercise.addToTotalPercentageY(1);
            }

            //else 
            else
            {
                percentageX = (((percentageBoxLength / 2) - dX) / (percentageBoxLength / 2)) * 100;
                percentageY = (((percentageBoxLength / 2) - dY) / (percentageBoxLength / 2)) * 100;

                //add each x and y percentage to the current exercise's totals
                currentExercise.addToTotalPercentageX(percentageX);
                currentExercise.addToTotalPercentageY(percentageY);
            }

        }

        //returns the non absolute differences between two point for use with translating positions
        private Position positionScale(Position currentPos)
        {
            return new Position(currentExercise.getStartingPosition().differenceBetweenPositions(currentPos));


        }

        //works out the score of a given exercise and outputs to a text block, and shows a motivational message to the user
        private void score(Exercise e)
        {
            // divide by 300 frames since that is the total for one 10 second exercise at 30fps
            double percentageX = e.getTotalPercentageX() / 300;
            double percentageY = e.getTotalPercentageY() / 300;

            //average of the X and Y percentages
            double averageTotalPercent = (percentageX + percentageY) / 2;
            //divide percentage by 10 and round up to the nearest whole number to get score. Then output to 'percent' text block
            percent.Text = Math.Round(averageTotalPercent / 10, 0).ToString() + "/ 10";

            //Excellent score
            if (averageTotalPercent > 80)
            {
                //reset percentages
                averageTotalPercent = 0;
                percentageX = 0;
                percentageY = 0;

                //output motivation to text block
                motivation.Text = "EXCELLENT!";
            }

            //Great score
            else if (averageTotalPercent > 50 & averageTotalPercent < 80)
            {
                averageTotalPercent = 0;
                percentageX = 0;
                percentageY = 0;

                motivation.Text = string.Format("GREAT!,{0}TRY AGAIN!", Environment.NewLine);
            }

            // < 50
            else if (averageTotalPercent < 50)
            {
                averageTotalPercent = 0;
                percentageX = 0;
                percentageY = 0;

                motivation.Text = string.Format("OK, BUT{0}TRY AGAIN!{0}YOU CAN DO IT!", Environment.NewLine);
            }
            
            //exercise better in the y axis
            else if (percentageY - percentageX > 30)
            {
                averageTotalPercent = 0;
                percentageX = 0;
                percentageY = 0;

                motivation.Text = string.Format("GOOD,{0}TRY STRETCHING{0}TO THE{0}SIDE MORE!", Environment.NewLine);
            }

            //exercise better in the x axis
            else if (percentageX - percentageY > 30)
            {
                averageTotalPercent = 0;
                percentageX = 0;
                percentageY = 0;

                motivation.Text = string.Format("GOOD,{0}TRY STRETCHING{0}HIGHER!", Environment.NewLine);
            }

        }

        private void kinectColorViewer1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //Count down event handler
        void countDownTick(object sender, EventArgs e)
        {
            if (tbTime != null)
            {
                //if countdown still counting down
                if (countDownTime > countDownStop)
                {
                    //subtract 1 second from the time
                    countDownTime--;
                    //Make text background purple and output time to text
                    tbTime.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#563D90"));
                    tbTime.Text = string.Format("00:0{0}:{1}", countDownTime / 60, countDownTime % 60);
                }

            }
        }

        void exerciseTimerTick(object sender, EventArgs e)
        {
            //whilst counting up
            if (time < 10)
            {
                //add 1 second to the time
                time++;
                //Make text background green and output time to text
                tbTime.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#83C643"));
                tbTime.Text = string.Format("00:0{0}:{1}", time / 60, time % 60);

            }

            //once exercise has finished, output "Exercise Complete!"
            else
            {
                tbTime.Text = string.Format("Exercise{0}Complete!", Environment.NewLine);
            }

        }

        //left arm event handler
        private void leftArmButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = leftArm;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/leftArmMov.mp4"));
            Instructional_Video_Player.Play();
        }

        //right arm event handler
        private void rightArmButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = rightArm;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/rightArmMov.mp4"));
            Instructional_Video_Player.Play();
        }

        //left leg event handler
        private void leftLegButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = leftLeg;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/leftLegMov.mp4"));
            Instructional_Video_Player.Play();
        }

        //right leg event handler
        private void rightLegButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = rightLeg;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/rightLegMov.mp4"));
            Instructional_Video_Player.Play();
        }

        //seated right arm event handler
        private void seatedRightArmButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = seatedRightArm;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/seatedRightArmMov.mp4"));
            Instructional_Video_Player.Play();
        }

        //seated left arm event handler
        private void seatedLeftArmButtonClick(object sender, RoutedEventArgs e)
        {
            //Reset all UI elements, the percentage totals, the timers, the calibration boolean, and play the relevant video
            currentExercise.resetPercentageTotals();
            currentExercise = seatedLeftArm;
            exerciseTimer.Stop();
            time = 0;
            countDownTime = 10;
            tbTime.Text = "";
            tbTime.Background = new SolidColorBrush(Colors.White);
            motivation.Text = "";
            percent.Text = "";
            calibrate = false;
            frameCounter = 0;
            Instructional_Video_Player.Opacity = 1;
            Instructional_Video_Player.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Instructional Videos/seatedLeftArmMov.mp4"));
            Instructional_Video_Player.Play();
        }

        private void kinectSensorChooser1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}