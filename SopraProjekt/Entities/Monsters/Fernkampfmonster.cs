using Microsoft.Xna.Framework;
using SopraProjekt.Renderer;
using System;

namespace SopraProjekt.Entities.Monsters
{
    /// <summary>
    /// Class to represent the Fernkampfmonster
    /// </summary>
    [Serializable]
    public class Fernkampfmonster : Monster
    {
        private const int Speed = 6;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="camera"></param>
        public Fernkampfmonster(Point position, Point textureSize, Camera camera) : base("Images/Monsters/MonsterGelb", position, textureSize, Speed, camera)
        {
            // Agrorange has to be greater than Attackingrange
            mAgrorange = 10;
            mAttackingRange = 9;
            mDamagePoints = 1;
        }

        /// <summary>
        /// Implements a skill that causes damage with a high range
        /// </summary>
        public override void Skill(GameTime gameTime)
        {
            var distance = Distance(mPosition, mTarget.mPosition);

            if (distance > mAttackingRange)
            {
                mCurrentState = State.Follow;
                return;
            }

            if (gameTime.TotalGameTime.Milliseconds % 2000 == 0)
            {
                mTarget.Missiles.Add(
                    new Missile(mPosition.ToVector2(),
                        mTarget.mPosition.ToVector2(), mTeam, Id, Content, mCamera));
            }


        }
    }
}
