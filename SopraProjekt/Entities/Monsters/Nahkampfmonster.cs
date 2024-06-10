using Microsoft.Xna.Framework;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;

namespace SopraProjekt.Entities.Monsters
{
    /// <summary>
    /// Class to represent the Kamikazemonster
    /// </summary>
    [Serializable]
    public class Nahkampfmonster : Monster
    {
        private const int Speed = 6;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="camera"></param>
        public Nahkampfmonster(Point position, Point textureSize, Camera camera) :
            base("Images/Monsters/MonsterGrün", position, textureSize, Speed, camera)
        {
            // Agrorange has to be greater than Attackingrange
            mAgrorange = 8;
            mAttackingRange = 2;
            mDamagePoints = 25;
            mCamera = camera;
        }

        /// <summary>
        /// Implements a skill that gives high damage to entities nearby
        /// </summary>
        public override void Skill(GameTime gameTime)
        {
            var distance = Distance(mPosition, mTarget.mPosition);

            if (distance > mAttackingRange)
            {
                mCurrentState = State.Follow;
                return;
            }

            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                mTarget.GetDamage(mDamagePoints);
                SoundManager.Default.PlaySoundEffect("SoundEffects/Nahkampfmonster", mPosition, mCamera);
            }
        }
    }
}
