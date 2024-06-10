using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using System;
using System.Collections.Generic;

namespace SopraProjekt.AnimationManagement
{
    /// <summary>
    /// Class to represent one sprite of a spritesheet.
    /// </summary>

    public sealed class AnimationSprite
    {
        private readonly AnimationManager mAnimationManager;

        /// <summary>
        /// Constructor.
        /// Class Sprite is handed the dictionary that contains the spritesheets
        /// </summary>
        /// <param name="animationList"></param>
        internal AnimationSprite(List<Animation> animationList)
        {
            mAnimationManager = new AnimationManager(animationList);
        }

        /// <summary>
        /// Draw the animation
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        internal void Draw(SpriteBatch spriteBatch, Point position)
        {
            if (mAnimationManager != null)
            {
                mAnimationManager.Draw(spriteBatch, position);
            }
            else
            {
                throw new Exception("Animation Draw Error");
            }
        }

        /// <summary>
        /// Update the animation manager
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            mAnimationManager.Update(gameTime);
        }

        public void Attack(bool attack, MovableEntity target, Point position)
        {
            mAnimationManager.Attack(attack, target, position);
        }
        /*
        /// <summary>
        /// Fight
        /// </summary>
        /// TODO: commented out to fix resharper
        internal void Fight()
        {
            mAnimationManager.Fight();
        }
        */

        /*
        /// <summary>
        /// Get the active animation
        /// </summary>
        /// <returns></returns>
        /// TODO: commentend out for resharper
        internal Animation GetActiveAnimation()
        {
            return mAnimationManager.GetActiveAnimation();
        }
        */
    }
}
