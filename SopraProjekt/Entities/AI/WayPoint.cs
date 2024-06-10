using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.AI
{
    [Serializable]
    public class WayPoint
    {
        [NonSerialized]
        internal Point mPosition;
        private int mPositionX;
        private int mPositionY;

        internal int mNumberNeighbours;

        internal WayPoint[]
            mNeighbours = new WayPoint[4]; // Indexes of Neighbors: [0] North [1] East [2] South [3] West

        internal Dictionary<Hero, int[]> mMarks = new Dictionary<Hero, int[]>();

        /// <summary>
        /// Constructor
        /// </summary>
        public WayPoint(Point position)
        {
            mPosition = position;
        }

        /// <summary>
        /// Prepares class before Serialization
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mPositionX = mPosition.X;
            mPositionY = mPosition.Y;
        }

        /// <summary>
        /// Checks out class after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mPosition = new Point(mPositionX, mPositionY);
        }
    }
}