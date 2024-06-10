

/*
namespace SopraProjekt.Entities
{
    public sealed class Pathfinding
    {
        // Coordinates of the Pathfinding
        internal Point mLocation;

        // Pathfinding free to walk on or not
        //private bool mUseable = true;

        // costs of the field (time to cross)
        internal int mCosts;

        // reachable neighbors of the Pathfinding
        internal readonly List<Pathfinding> mSuccessors;

        // to keep the best predecessor to this Pathfinding
        internal Pathfinding mPredecessor;

        internal double mFvalue;

        internal Pathfinding(Point location)
        {
            mLocation = location;
            //mUseable = map.IsSpaceIn(new Rectangle(location,new Point(1)));
            //mUseable = true;
            mCosts = 1;
            mSuccessors = new List<Pathfinding>();

        }
    }


    public sealed class Pathfinding
    {
        private readonly Dictionary<Pathfinding, double> mOpenlist = new Dictionary<Pathfinding, double>();
        readonly List<Point> mClosedlist = new List<Point>();
        private Tuple<Pathfinding, double> mCurrentField;

        /// <summary>
        /// Function that finds the shortest path from position pos to the destination des,
        /// returns a path as a stack of points (from top to bottom) 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dest"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public Stack<Point> ShortestPath(Point pos, Point dest, Map map)
        {
            if (!map.IsSpaceIn(new Rectangle(dest.X, dest.Y, 2, 2)))
            {
                var resultstack = new Stack<Point>();
                return resultstack; // No path found
            }
            Pathfinding position = new Pathfinding(pos);
            position.mFvalue = Math.Sqrt(
                Math.Pow(pos.X - dest.X, 2) + Math.Pow(pos.Y - dest.Y, 2)
                );
            //Pathfinding destination = new Pathfinding(dest);
            mOpenlist.Add(position, position.mFvalue);
            while (mOpenlist.Count > 0)
            {
                mCurrentField = DeleteMin(mOpenlist); // Finds a not yet detected Pathfinding with min costs
                if (mCurrentField.Item1.mLocation == dest) // If destination-Pathfinding is found
                {
                    // Destination found, return the path on a stack
                    Stack<Point> resultstack = new Stack<Point>();
                    Pathfinding checkfeld = mCurrentField.Item1;
                    while (checkfeld.mLocation != pos)
                    {
                        resultstack.Push(checkfeld.mLocation);
                        checkfeld = checkfeld.mPredecessor;
                    }

                    return resultstack;
                }

                mClosedlist.Add(mCurrentField.Item1.mLocation); // close Pathfinding to not visit it again
                if (Visit(mCurrentField, dest, map)) {}  // use output of visit to prevent resharp-errors
            }
            var resultstack2 = new Stack<Point>();
            return resultstack2; // No path found
        }


        /// <summary>
        /// Search in a Dictionary of Fields the Pathfinding with the min of costs and deletes it
        /// searchDict is a Dictionary of Tuples which includes the Pathfinding and given costs of the Pathfinding
        /// </summary>
        /// <param name="searchDict"></param>
        /// <returns></returns>
        // Search in a List of Fields the Pathfinding with the min of costs and deletes it
        // searchList is a List of Tuples which includes the Pathfinding and given costs of the Pathfinding
        private Tuple<Pathfinding, double> DeleteMin(IDictionary<Pathfinding, double> searchDict)
        {
            double minCosts = 1000000;
            Pathfinding minField = null;
            foreach (var field in searchDict)
            {
                if (field.Value <= minCosts)
                {
                    minField = field.Key;
                    minCosts = field.Value;
                }
            }

            if (minField != null)
            {
                searchDict.Remove(minField); // Delete Pathfinding from searchDict
                var result = new Tuple<Pathfinding, double>(minField, minCosts);
                return (result);
            }
            return null;  // if no Pathfinding in Dictionary (should not happen)
        }

        /// <summary>
        /// Help-function for ShortestPath. Visits a Pathfinding and create their Neighbors with their
        /// minimum distance to the destination if they aren`t yet in the openlist
        /// </summary>
        /// <param name="field"></param>
        /// <param name="dest"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        private bool Visit(Tuple<Pathfinding, double> field, Point dest, Map map)
        {
            // create neighbors of this field
            
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    
                    if (!map.IsSpaceIn(new Rectangle(field.Item1.mLocation.X + x,
                        field.Item1.mLocation.Y + y, 2, 2)))
                    {
                        continue;
                    }
                    
                    // create Pathfinding object, evaluate f-value and add to successors of present field
                    Point neighborPos = new Point(field.Item1.mLocation.X + x, field.Item1.mLocation.Y + y);
                    Pathfinding successor = new Pathfinding(neighborPos);
                    successor.mFvalue = Math.Sqrt(
                        Math.Pow(successor.mLocation.X - dest.X, 2) +
                        Math.Pow(successor.mLocation.Y - dest.Y, 2));
                    field.Item1.mSuccessors.Add(successor);
                }
            }
            
            foreach (Pathfinding neighbor in field.Item1.mSuccessors)
            {
                // check if neighbor is already closed
                if (mClosedlist.Contains(neighbor.mLocation))
                {
                    continue;
                }

                // set pathcosts for the new field to pathcosts of field before and add costs
                // of the new field
                int pathcosts = field.Item1.mCosts + neighbor.mCosts;

                // search in openlist for the neighbor
                if (mOpenlist.ContainsKey(neighbor) && pathcosts >= neighbor.mFvalue)
                {
                    continue;
                }

                // set predecessor and new costs to that field
                neighbor.mPredecessor = field.Item1;
                neighbor.mCosts = pathcosts;


                // correct or set the costs of the path in openlist
                if (mOpenlist.ContainsKey(neighbor))
                {
                    mOpenlist[neighbor] = pathcosts + neighbor.mFvalue;
                }
                else
                {
                    mOpenlist.Add(neighbor, pathcosts + neighbor.mFvalue);
                }
            }

            return true;
        }
    }

}
*/