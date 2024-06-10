using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using System.Collections.Generic;


namespace SopraProjekt.AnimationManagement
{
    /// <summary>
    /// Internal class to manage 8 sprite directions.
    /// </summary>
    internal static class Direction
    {
        internal const int Front = 0;
        internal const int FrontLeft = 1; //diagonal
        internal const int Left = 2;
        internal const int BackLeft = 3; //diagonal
        internal const int Back = 4;
        internal const int BackRight = 5; //diagonal
        internal const int Right = 6;
        internal const int FrontRight = 7; //diagonal
    }

    /// <summary>
    /// Class to manage Spritesheet animations.
    /// </summary>
    public class AnimationManager
    {
        private Animation mAnimation;

        private readonly List<Animation> mAnimationList;

        //position of hero in map before movement
        private Point mPreviousPosition;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="animationList">List with 8 animations</param>
        public AnimationManager(List<Animation> animationList)
        {
            //animation lists are stored in individual hero classes
            mAnimationList = animationList;

            //initial position is front i.e. first frame in first row
            mAnimation = animationList[Direction.Front];
        }

        /// <summary>
        /// Update method for AnimationManager.
        /// Hands update function to Animation class.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            mAnimation.Update(gameTime);
        }

        /// <summary>
        /// Draw method for AnimationManager.
        /// Hands draw method to Animation class.
        /// <param name="spriteBatch">an animation</param>
        /// <param name="position">new position</param>
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Point position)
        {
            DirectionHandler(position);
            mAnimation.Draw(spriteBatch, mPreviousPosition);
        }

        /// <summary>
        /// Determines which frame of each row to choose.
        /// <param name="position">new position</param>
        /// </summary>
        private void DirectionHandler(Point position)
        {
            //if hero stops back to initial position
            if (position == mPreviousPosition)
            {
                mAnimation.SetCurrentFrame(Actions.Stop);
                return;
            }

            var prevAnimation = mAnimation;
            //walks right
            if (position.X > mPreviousPosition.X && position.Y == mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.Right];
            }
            //walks left
            else if (position.X < mPreviousPosition.X && position.Y == mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.Left];
            }
            //walks back
            else if (position.X == mPreviousPosition.X && position.Y < mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.Back];
            }
            //walks diagonal (front right)
            else if (position.X > mPreviousPosition.X && position.Y > mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.FrontRight];
            }
            //walks diagonal (back right)
            else if (position.X > mPreviousPosition.X && position.Y < mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.BackRight];
            }
            //walks diagonal (front left)
            else if (position.X < mPreviousPosition.X && position.Y > mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.FrontLeft];
            }
            //walks diagonal (back left)
            else if (position.X < mPreviousPosition.X && position.Y < mPreviousPosition.Y)
            {
                mAnimation = mAnimationList[Direction.BackLeft];
            }
            //walks front
            else /*(position.X == mPreviousPosition.X && position.Y > mPreviousPosition.Y)*/
            {
                mAnimation = mAnimationList[Direction.Front];
            }

            mAnimation.CurrentFrame = prevAnimation.CurrentFrame;
            mAnimation.mCurrentAction = prevAnimation.mCurrentAction;

            mPreviousPosition = position;
            mAnimation.SetCurrentFrame(Actions.Move);
        }

        /// <summary>
        /// Checks if fight animation is needed.
        /// </summary>
        /// <param name="attack">true if hero is attacking</param>
        /// <param name="target">target that is attacked</param>
        /// <param name="currentPosition">current position of the attacking hero</param>
        public void Attack(bool attack, MovableEntity target, Point currentPosition)
        {
            if (!attack)
            {
                return;
            }

            var helpPosition = mPreviousPosition;
            mPreviousPosition = currentPosition;
            //_ = target;
            if (target != null)
            {
                DirectionHandler(target.mPosition);
            }

            mPreviousPosition = helpPosition;
            mAnimation.SetCurrentFrame(Actions.Fight);
        }
    }
}
