//Position Class
//Author: Edward Jack Chandler
//Student Number: 120232420

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect12_02_15
{
    
    class Position
    {
        private double x;
        private double y;
        private double z;

        //Default constructor
        public Position()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        //3D Constructor
        public Position(double xPos, double yPos, double zPos)
        {
            x = xPos;
            y = yPos;
            z = zPos;
        }

        //2D Constructor
        public Position(double xPos, double yPos)
        {
            x = xPos;
            y = yPos;
            z = 0;
        }

        //copy constructor
        public Position(Position p)
        {
            x = p.x;
            y = p.y;
            z = p.z;
        }

        //GET METHODS
        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }


        public double getZ()
        {
            return z;
        }

        //SET VALUES
        public void setX(double x)
        {
            this.x = x;
        }

        public void setY(double y)
        {
            this.y = y;
        }

        public void setZ(double z)
        {
            this.z = z;
        }
        
        public void setPosition(double x, double y, double z)
        {
            setX(x);
            setY(y);
            setZ(z);
        }

        public void setPosition(Position p)
        {
            setX(p.getX());
            setY(p.getY());
            setZ(p.getZ());
        }

        //Works out the absolute difference between two positions.
        public Position absDifferenceBetweenPositions(Position p2)
        {
            Position diff = new Position(Math.Abs(this.getX() - p2.getX()), 
                Math.Abs(this.getY() - p2.getY()), Math.Abs(this.getZ() - p2.getZ()));
            return diff;
        }

        //Works out the difference between two positions without absolute values, for translating positions
        public Position differenceBetweenPositions(Position p2)
        {
            Position diff = new Position(this.getX() - p2.getX(), this.getY() - p2.getY(), this.getZ() - p2.getZ());
            return diff;
        }

        //Prints the position to the console for testing purposes
        public void printPosition()
        {
            Console.WriteLine("Position: (" + x + ", " + y + ", " + z + ")" + "\n");
        }

        //Checks whether an X position is within specific distance from another X position
        public bool withinPositionX(Position p, double difference)
        {
            if (this.absDifferenceBetweenPositions(p).getX() > difference)
            {
                return false;
            }

            else return true;
        }

        //Checks whether a Y position is within specific distance from another X position
        public bool withinPositionY(Position p, double difference)
        {
            if (this.absDifferenceBetweenPositions(p).getY() > difference)
            {
                return false;
            }

            else return true;
        }

        //Checks whether a Z position is within specific distance from another X position
        public bool withinPositionZ(Position p, double difference)
        {
            if (this.differenceBetweenPositions(p).getZ() > difference)
            {
                return false;
            }

            else return true;
        }
                    
    }
}
