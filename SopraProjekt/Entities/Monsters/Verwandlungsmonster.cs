using Microsoft.Xna.Framework;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;

namespace SopraProjekt.Entities.Monsters
{
    /// <summary>
    /// Class to represent the Verwandlungsmonster
    /// </summary>
    [Serializable]
    public class Verwandlungsmonster : Monster
    {
        private const int Speed = 6;
        private const int FreezeFrequence = 10000;

        private int mFreezeEnemy;
        private int mNextFreeze;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="camera"></param>
        public Verwandlungsmonster(Point position, Point textureSize, Camera camera) :
            base("Images/Monsters/MonsterBlau", position, textureSize, Speed, camera)
        {
            // Agrorange has to be greater than Attackingrange
            mAgrorange = 7;
            mAttackingRange = 4;
            mFreezeEnemy = 2000;
            mDamagePoints = 6;
            mCamera = camera;
        }

        /// <summary>
        /// Implements a skill that makes the target entity unconscious
        /// </summary>
        public override void Skill(GameTime gameTime)
        {
            var distance = Distance(mPosition, mTarget.mPosition);

            if (distance > mAttackingRange)
            {
                mCurrentState = State.Follow;
                return;
            }

            if (gameTime.TotalGameTime.Milliseconds % 500 == 0)
            {
                mTarget.GetDamage(mDamagePoints);
                SoundManager.Default.PlaySoundEffect("SoundEffects/Nahkampfmonster", mPosition, mCamera);
            }

            if (gameTime.TotalGameTime.TotalMilliseconds < mNextFreeze)
            {
                return;
            }
            SoundManager.Default.PlaySoundEffect("SoundEffects/ice_crumble", mPosition, mCamera);
            mTarget.GetDamage(40);
            mTarget.Freeze(gameTime, mFreezeEnemy);
            mNextFreeze = (int)gameTime.TotalGameTime.TotalMilliseconds + FreezeFrequence;
        }
    }
}