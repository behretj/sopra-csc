using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SopraProjekt.Entities.AI
{
    internal class WayPointMap
    {
        // Access mWayPointMap[Y-Koordinate, X-Koordinate]
        private readonly WayPoint[,] mWayPointMap = new WayPoint[25, 25];

        /// <summary>
        /// Creates Empty WayPointMap with correct coordinates
        /// </summary>
        private void CreateEmptyWayPointMap()
        {
            for (int y = 0; y < 25; y++)
            {
                for (int x = 0; x < 25; x++)
                {
                    mWayPointMap[y, x] = new WayPoint(new Point(5 + 10 * x, 5 + 10 * y));
                    mWayPointMap[y, x].mMarks = new Dictionary<Hero, int[]>();
                }
            }

        }

        /// <summary>
        /// Calculates if in one direction is a wall or a neighbor
        /// </summary>
        /// <param name="waypoint"></param> Waypoint which to check of neighbor
        /// <param name="direction"></param> Direction in which direction should be searched for neighbor
        /// <returns> Returns if in Direction is neighbor </returns> 
        private bool IsNeighbor(Point waypoint, Point direction)
        {
            for (int i = 0; i < 12; i++)
            {
                waypoint += direction;
                if (MapTextFormat.sMapText[waypoint.Y, waypoint.X] == 1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates WayPointMap
        /// </summary>
        /// <param name="enemies"></param>
        internal WayPoint[,] CreateWayPointMap(IList<Hero> enemies)
        {
            CreateEmptyWayPointMap();
            // Fill Empty Map with correct neighbors
            for (int y = 0; y < 25; y++)
            {
                for (int x = 0; x < 25; x++)
                {
                    int neighbors = 0;
                    Point point = mWayPointMap[y, x].mPosition;
                    foreach (var enemy in enemies)
                    {
                        mWayPointMap[y, x].mMarks.Add(enemy, new[] { 5, 5, 5, 5 });
                    }
                    // Proof North
                    if (y > 0 && IsNeighbor(point, new Point(0, -1)))
                    {
                        mWayPointMap[y, x].mNeighbours[0] = mWayPointMap[y - 1, x];
                        foreach (var enemy in enemies)
                        {
                            mWayPointMap[y, x].mMarks[enemy][0] = 0;
                        }
                        neighbors += 1;
                    }
                    // Proof East
                    if (x < 24 && IsNeighbor(point, new Point(1, 0)))
                    {
                        mWayPointMap[y, x].mNeighbours[1] = mWayPointMap[y, x + 1];
                        foreach (var enemy in enemies)
                        {
                            mWayPointMap[y, x].mMarks[enemy][1] = 0;
                        }
                        neighbors += 1;
                    }
                    // Proof South
                    if (y < 24 && IsNeighbor(point, new Point(0, 1)))
                    {
                        mWayPointMap[y, x].mNeighbours[2] = mWayPointMap[y + 1, x];
                        foreach (var enemy in enemies)
                        {
                            mWayPointMap[y, x].mMarks[enemy][2] = 0;
                        }
                        neighbors += 1;
                    }
                    // Proof West
                    if (x > 0 && IsNeighbor(point, new Point(-1, 0)))
                    {
                        mWayPointMap[y, x].mNeighbours[3] = mWayPointMap[y, x - 1];
                        foreach (var enemy in enemies)
                        {
                            mWayPointMap[y, x].mMarks[enemy][3] = 0;
                        }
                        neighbors += 1;
                    }
                    mWayPointMap[y, x].mNumberNeighbours = neighbors;
                }
            }

            foreach (var enemy in enemies)
            {
                enemy.AiCurrentWayPoint = mWayPointMap[0, 23];
                enemy.AiLastWayPoint = mWayPointMap[0, 24];
                enemy.AiDirection = 3;
            }

            return mWayPointMap;
        }
    }
}
