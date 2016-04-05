//Exercise Class
//Author: Edward Jack Chandler
//Student Number: 120232420

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using SkeletalTracking;
using Microsoft.Kinect;
using Microsoft.Win32;

namespace Kinect12_02_15
{
    class Exercise
    {
        //Exercies holds the current position, previous position, and model position for the current frame, and also the starting position of the exercise
        private Position currentPos = new Position();
        private Position previousPos = new Position();
        private Position modelPos = new Position();
        private Position startingPos = new Position();

        //total percentages of the exercise
        private double totalPercentageX = 0;
        private double totalPercentageY = 0;

        //text file location for the model exercises
        private String modelTextFile;

        //List of model positions, and a list of strings,
        private List<Position> posList = new List<Position>();

        //joint type for the  current exercise, initialised as the hip
        JointType t = JointType.HipCenter;
        //boolean to check if the exercise is seated
        bool seated = false;

        //leniency value to alter the dimensions of the percentage box
        private double leniency;

        //Constructor takes the exercise name as a parameter, then depending on the name, will set the text file location, fill the list of positions with model positions, set the relevant joint type,
        //set the correct starting position for those model exercises, and set the leniency
        public Exercise(String exercise)
        {
            if (exercise == "right arm")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/rightHand.txt");
                fillPositionList(modelTextFile);
                t = JointType.HandRight;
                startingPos.setPosition(-0.0926099568605423, 0.324595719575882, 2.42309355735779);
                leniency = 0.75;
            }

            if (exercise == "left arm")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/leftHand.txt");
                fillPositionList(modelTextFile);
                t = JointType.HandLeft;
                startingPos.setPosition(0.0216575562953949, 0.329336374998093, 2.34459733963013);
                leniency = 0.75;
            }

            if (exercise == "right arm seated")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/seatedRightHand.txt");
                fillPositionList(modelTextFile);
                t = JointType.HandRight;
                startingPos.setPosition(0.0509357675909996, 0.549594581127167, 1.84863603115082);
                seated = true;
                leniency = 0.75;
            }

            if (exercise == "left arm seated")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/seatedLeftHand.txt");
                fillPositionList(modelTextFile);
                t = JointType.HandLeft;
                seated = true;
                startingPos.setPosition(0.102827176451683, 0.570048153400421, 2.06467723846436);
                leniency = 0.75;
            }

            if (exercise == "right leg")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/rightFoot.txt");
                fillPositionList(modelTextFile);
                t = JointType.FootRight;
                startingPos.setPosition(0.0411089211702347, 0.320578664541245, 2.37570142745972);
                leniency = 0.8;

            }

            if (exercise == "left leg")
            {
                modelTextFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Joint Samples/leftFoot.txt");
                fillPositionList(modelTextFile);
                t = JointType.FootLeft;
                startingPos.setPosition(0.0647142082452774, 0.315704494714737, 2.31226325035095);
                leniency = 0.8;

            }

        }

        //GET METHODS
        public Position getCurrentPosition()
        {
            return currentPos;
        }

        public Position getPreviousPosition()
        {
            return previousPos;
        }

        public Position getModelPosition()
        {
            return modelPos;
        }

        public List<Position> getPositionList()
        {
            return posList;
        }

        public double getTotalDiffX()
        {
            return totalPercentageX;
        }

        public double getTotalDiffY()
        {
            return totalPercentageY;
        }

        public JointType getJointType()
        {
            return t;
        }

        public Position getStartingPosition()
        {
            return startingPos;
        }

        public double getLeniency()
        {
            return leniency;
        }

        public double getTotalPercentageX()
        {
            return totalPercentageX;
        }

        public double getTotalPercentageY()
        {
            return totalPercentageY;
        }

        public bool getSeated()
        {
            return seated;
        }

        //Adds to the total percentageX
        public void addToTotalPercentageX(double percentage)
        {
            totalPercentageX += percentage;
        }
        //Adds to the total percentageY
        public void addToTotalPercentageY(double percentage)
        {
            totalPercentageY += percentage;
        }


        //makes the total percentage x and y equal to zero
        public void resetPercentageTotals()
        {
            totalPercentageX = 0;
            totalPercentageY = 0;

        }

        //SET METHODS
        public void setCurrentPosition(Position p)
        {
            currentPos.setPosition(p);
        }

        public void setCurrentPosition(double x, double y, double z)
        {
            currentPos.setPosition(x, y, z);
        }

        public void setPreviousPosition(Position p)
        {
            previousPos.setPosition(p);
        }

        public void setModelPosition(int counter)
        {
            modelPos.setPosition(posList[counter]);
        }

        //Takes a line of coordinates eg. 0.41251325, 1.523525, 2.523525, split by the delimiter ", "
        //Then adds each string value to a list of strings. Once in the list of strings
        //the string values are converted to doubles, and a new position is returned with
        // the 3 values as x, y and z
        public Position stringSplitToPosition(String line)
        {
            String[] stringSeperator = new String[] { ", " };
            String[] result = line.Split(stringSeperator, StringSplitOptions.None);
            List<String> stringList = new List<String>();

            foreach (String s in result)
            {
                stringList.Add(s);

            }

            double x = Convert.ToDouble(stringList[0]);
            double y = Convert.ToDouble(stringList[1]);
            double z = Convert.ToDouble(stringList[2]);

            return new Position(x, y, z);
        }

        //Takes in the the model exercise file, and creates a position out of every line, and
        //then puts every position into a list of positions
        public void fillPositionList(String file)
        {
            String line;
            using (StreamReader sr = new StreamReader(file))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    posList.Add(stringSplitToPosition(line));

                }

            }
        }

        
    }
}
