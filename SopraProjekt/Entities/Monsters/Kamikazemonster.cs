using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Monsters
{
    /// <summary>
    /// Class to represent the Kamikazemonster
    /// </summary>
    [Serializable]
    public class Kamikazemonster : Monster
    {
        private const int SkillDamage = 180;
        private const int Speed = 6;

        private int mExplosionState;
        private int mExplosionCounter;

        [NonSerialized]
        private List<Texture2D> mExplosionTextures;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="camera"></param>
        public Kamikazemonster(Point position, Point textureSize, Camera camera) :
            base("Images/Monsters/MonsterRot", position, textureSize, Speed, camera)
        {
            // Agrorange has to be greater than Attackingrange
            mAgrorange = 10;
            mAttackingRange = 4;
            mDamagePoints = 2;
            mExplosionTextures = new List<Texture2D>(4)
            {
                Content.LoadTexture("Images/Abilities/Explosion1"),
                Content.LoadTexture("Images/Abilities/Explosion2"),
                Content.LoadTexture("Images/Abilities/Explosion3"),
                Content.LoadTexture("Images/Abilities/Explosion4")
            };
            mCamera = camera;
        }
        /// <summary>
        /// Implements a skill that lets the monster explode and deals high damage to entities around
        /// </summary>
        public override void Skill(GameTime gameTime)
        {
            var distance = Distance(mPosition, mTarget.mPosition);

            if (distance > mAttackingRange && mExplosionState == 0)
            {
                mCurrentState = State.Follow;
                return;
            }

            if (mExplosionState == 0)
            {
                var area = mMap.mEntities.GetElements(
                    new Rectangle(mPosition.X - mAttackingRange,
                        mPosition.Y - mAttackingRange,
                        mAttackingRange * 2,
                        mAttackingRange * 2));



                foreach (Entity entity in area)
                {
                    if (entity is MovableEntity movableEntity)
                    {
                        movableEntity.GetDamage(SkillDamage);
                    }
                }

                mExplosionState++;
            }
            else
            {
                mTexture = mExplosionTextures[mExplosionState - 1];
                SoundManager.Default.PlaySoundEffect("SoundEffects/Kamikazemonster", mPosition, mCamera);
                mExplosionCounter++;

                if (mExplosionCounter % 8 == 0)
                {
                    mExplosionState++;
                    if (mExplosionState == 5)
                    {
                        mMap.mEntities.Remove(this);
                        mMap.mAllEntities.Remove(this);
                        IsAlive = false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks out storage after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mExplosionTextures = new List<Texture2D>(4)
            {
                Content.LoadTexture("Images/Abilities/Explosion1"),
                Content.LoadTexture("Images/Abilities/Explosion2"),
                Content.LoadTexture("Images/Abilities/Explosion3"),
                Content.LoadTexture("Images/Abilities/Explosion4")
            };
        }
    }
}