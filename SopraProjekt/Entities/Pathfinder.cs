using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Path finding Class (A*)
    /// </summary>
    public sealed class Pathfinder
    {
        private readonly Map mMap;
        private readonly List<bool> mVisited;

        private readonly int mLoadingState;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map">map</param>
        /// <param name="loadingState"></param>
        public Pathfinder(Map map, int loadingState)
        {
            mMap = map;
            mVisited = new List<bool>(new bool[mMap.MapSize.X * mMap.MapSize.Y]);
            mLoadingState = loadingState;
        }

        /// <summary>
        /// Class to represent one field in the map for the pathfinding
        /// </summary>
        private sealed class Field
        {
            public Point mPosition;
            public int mCost;
            public int mDistance;
            public int CostDistance => mCost + mDistance;
            public Field Parent { get; set; }

            public void SetDistance(Point target)
            {
                mDistance = Math.Abs(target.X - mPosition.X) + Math.Abs(target.Y - mPosition.Y);
            }
        }

        public void UpdateVisits(int x, int y)
        {
            mVisited[GetVisitedPosition(x, y)] = true;
        }


        private int GetVisitedPosition(int x, int y)
        {
            return y * mMap.MapSize.Y + x;
        }


        /// <summary>
        /// Find a path from start to end
        /// </summary>
        /// <param name="startPosition">start position</param>
        /// <param name="endPosition">end position</param>
        /// <param name="forbidPoint">list of points where no figure is allowed</param>
        /// <param name="movableEntity"></param>
        /// <returns>Stack with points to follow</returns>
        public void FindPath(MovableEntity movableEntity, Point startPosition, Point endPosition, List<Point> forbidPoint)
        {
            if (endPosition == Point.Zero)
            {
                return;
            }

            var firstDest = endPosition;
            if (movableEntity.mTeam == MovableEntity.EnemyTeam)
            {
                forbidPoint.Clear();
            }


            movableEntity.mCalculatePath = true;

            if (!mMap.IsSpaceIn(new Rectangle(endPosition - new Point(1), new Point(2)),
                movableEntity.mTeam,
                movableEntity))
            {
                endPosition = mMap.FindEmptySpace(endPosition);
            }


            var start = new Field { mPosition = startPosition };
            var end = new Field { mPosition = endPosition };

            start.SetDistance(end.mPosition);

            var activeTiles = new List<Field>();
            var visitedTiles = new HashSet<Point>();

            activeTiles.Add(start);

            while (activeTiles.Any())
            {
                var minCost = activeTiles.Min(x => x.CostDistance);
                var checkTile = activeTiles.First(x => x.CostDistance == minCost);
                // If other location will be entered calculation will be ended and newer calculation gets the task
                if (movableEntity.mMovetoField != firstDest)
                {
                    movableEntity.mCalculatePath = false;
                    return;
                }

                if (checkTile.mPosition == end.mPosition)
                {
                    var tile = checkTile;
                    var path = new Stack<Point>();

                    while (true)
                    {
                        path.Push(tile.mPosition);
                        tile = tile.Parent;
                        if (tile != null)
                        {
                            continue;
                        }
                        path.Pop();
                        break;
                    }

                    movableEntity.mMovePath = path;
                    movableEntity.mCalculatePath = false;
                    return;
                }
                visitedTiles.Add(checkTile.mPosition);
                activeTiles.Remove(checkTile);

                var usableTiles = GetUsableTiles(checkTile, end, forbidPoint,
                    movableEntity.mTeam == MovableEntity.EnemyTeam, movableEntity);

                foreach (var usableTile in usableTiles)
                {
                    if (visitedTiles.Contains(usableTile.mPosition))
                    {
                        continue;
                    }

                    if (activeTiles.Any(x => x.mPosition == usableTile.mPosition))
                    {
                        var existingTiles = activeTiles.First(tile => tile.mPosition == usableTile.mPosition);
                        if (existingTiles.CostDistance <= checkTile.CostDistance)
                        {
                            continue;
                        }

                        activeTiles.Remove(existingTiles);
                        activeTiles.Add(usableTile);
                    }
                    else
                    {
                        activeTiles.Add(usableTile);
                    }
                }

                
                if (visitedTiles.Count > 200 * start.mDistance && mLoadingState == -1)
                {
                    movableEntity.mMovePath = new Stack<Point>();
                    movableEntity.mCalculatePath = false;
                    movableEntity.mMovetoField = Point.Zero;
                    return;
                }
                
            }

            movableEntity.mMovePath = new Stack<Point>();
            movableEntity.mCalculatePath = false;
        }

        /// <summary>
        /// Get all usable tiles around the current field
        /// </summary>
        /// <param name="currentField">current field</param>
        /// <param name="endField">end field of the path finding algorithm</param>
        /// <param name="forbiddenfield">list of points were no figure is allowed</param>
        /// <param name="ignoreVisibility">flag for enemy team to ignore visibility</param>
        /// <returns>List of usable Fields around the current field</returns>
        /// <param name="entity"></param>
        private IEnumerable<Field> GetUsableTiles(Field currentField, Field endField, IReadOnlyCollection<Point> forbiddenfield,
            bool ignoreVisibility, MovableEntity entity)
        {
            var possibleTiles = new List<Field>();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var position = new Point(currentField.mPosition.X + i, currentField.mPosition.Y + j);
                    if (position.X > 0 && position.X < mMap.MapSize.X
                        && position.Y > 0 && position.Y < mMap.MapSize.Y
                        && mMap.IsSpaceIn(new Rectangle(position, new Point(2, 2)), entity.mTeam))
                    {
                        var allowed = mVisited[GetVisitedPosition(position.X, position.Y)] || ignoreVisibility;
                        // var allowed = true;
                        if (!(forbiddenfield is null))
                        {
                            foreach (Point point in CopyList(forbiddenfield))
                            {
                                if (position == point)
                                {
                                    allowed = false;
                                }
                            }
                        }

                        if (allowed)
                        {
                            possibleTiles.Add(new Field
                            {
                                mPosition = position,
                                Parent = currentField,
                                mCost = currentField.mCost + 1
                            });
                        }
                    }
                }
            }

            possibleTiles.ForEach(tile => tile.SetDistance(endField.mPosition));
            return possibleTiles;
        }

        private static List<Point> CopyList(IReadOnlyCollection<Point> points)
        {
            lock (points)
            {
                try
                {
                    return points.ToList();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return new List<Point>();
                }
            }
        }

    }
}