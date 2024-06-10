using Microsoft.Xna.Framework;
using SopraProjekt.Helper;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{


    /// <summary>
    /// Quad tree to represent the data in the map
    /// </summary>
    [Serializable]
    public class QuadTree
    {
        [NonSerialized]
        private Box mBoundary;

        private float mBoundaryX;
        private float mBoundaryY;
        private float mBoundaryWidth;
        private float mBoundaryHeight;

        private Entity mEntity;
        private bool mDivided;

        private List<QuadTree> mChildren;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="boundary">boundaries of the quad tree</param>
        public QuadTree(Box boundary)
        {
            mBoundary = boundary;
            mEntity = null;
            mDivided = false;
            mChildren = new List<QuadTree>(4);
        }

        /// <summary>
        /// Subdivide a cell
        /// </summary>
        private void Subdivide()
        {
            var nw = new Box(mBoundary.X, mBoundary.Y,
                mBoundary.Width / 2,
                mBoundary.Height / 2);
            var ne = new Box(mBoundary.X + mBoundary.Width / 2,
                mBoundary.Y,
                mBoundary.Width / 2,
                mBoundary.Height / 2);
            var se = new Box(mBoundary.X + mBoundary.Width / 2,
                mBoundary.Y + mBoundary.Height / 2,
                mBoundary.Width / 2,
                mBoundary.Height / 2);
            var sw = new Box(mBoundary.X,
                mBoundary.Y + mBoundary.Height / 2,
                mBoundary.Width / 2,
                mBoundary.Height / 2);

            mChildren.Add(new QuadTree(nw));
            mChildren.Add(new QuadTree(ne));
            mChildren.Add(new QuadTree(se));
            mChildren.Add(new QuadTree(sw));


            mDivided = true;
        }

        /// <summary>
        /// Insert an entity into the tree
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(Entity entity)
        {
   if (!mBoundary.Contains(entity.mPosition))
            {
                return false;
            }

            if (mEntity == null)
            {
                mEntity = entity;
                return true;
            }

            if (!mDivided)
            {
                Subdivide();
            }

            foreach (var child in mChildren)
            {
                if (child.Insert(entity))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Get all entities in a given rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="list">list with already found entities</param>
        /// <returns></returns>
        public List<Entity> GetElements(Rectangle rectangle, List<Entity> list = null)
        {
            if (list == null)
            {
                list = new List<Entity>();
            }

            if (mBoundary != null && !mBoundary.Intersects(rectangle))
            {
                return new List<Entity>();
            }

            if (mEntity != null && rectangle.Contains(mEntity.mPosition))
            {
                list.Add(mEntity);
            }

            if (mDivided)
            {
                foreach (var child in mChildren)
                {
                    child.GetElements(rectangle, list);
                }
            }


            return list;
        }


        /// <summary>
        /// Remove the entity from the quad tree
        /// </summary>
        /// <param name="entity">entity to remove</param>
        /// <returns>true, if the entity could be removed, false otherwise</returns>
        public bool Remove(Entity entity)
        {
            if (!mBoundary.Contains(entity.mPosition))
            {
                return false;
            }

            if (mEntity == entity)
            {
                mEntity = null;
                

                // Collaps();
            }
            else
            {
                foreach (var child in mChildren)
                {
                    if (child != null && child.Remove(entity))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Prepares serialization
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mBoundaryX = mBoundary.X;
            mBoundaryY = mBoundary.Y;
            mBoundaryWidth = mBoundary.Width;
            mBoundaryHeight = mBoundary.Height;
        }

        /// <summary>
        /// Checks out deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mBoundary = new Box(mBoundaryX, mBoundaryY, mBoundaryWidth, mBoundaryHeight);
        }
    }
}



