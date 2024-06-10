using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Functional
{
    [Serializable]
    internal sealed class HealthFountain : Colliding
    {
        private double mLastHealTick;
        private readonly List<MovableEntity> mHealingEntities = new List<MovableEntity>();

        private bool mActive;

        public const int HealValue = 30;
        private const float HealingRange = 10.0f;
        private const int TickDelay = 1;
        private const int TimeInactive = 15;

        [NonSerialized]
        internal Texture2D mInactiveTexture;

        private const string OwnAssetName = "Images/Entities/HealthFountain2";
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.DarkSalmon;
        private const int TextureSize = 128;

        [NonSerialized]
        private Camera mCamera;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="position">Position of the health fountain.</param>
        /// <param name="camera">camera</param>
        internal HealthFountain(Point position, Camera camera) : base(OwnAssetName, "health_fountain", position, new Point(TextureSize), sMiniMapColor)
        {
            mInactiveTexture = Content.LoadTexture("Images/Entities/HealthFountainInactive");
            mActive = true;
            mCamera = camera;
        }

        /// <summary>
        /// Updates this fountains entities.
        /// </summary>
        /// <param name="gameTime"></param>
        internal override void Update(GameTime gameTime)
        {
            var healCount = mHealingEntities.Count;

            // check if any entity has walked out of range
            mHealingEntities.RemoveAll(entity => (entity.mPosition.ToVector2() - this.mPosition.ToVector2()).Length() > HealingRange);

            // if all entities have left the range of the health fountain, the fountain gets inactive for a certain amount of time
            if (healCount > 0 && mHealingEntities.Count == 0)
            {
                mActive = false;
            }

            // For every second a entity is healed
            var nowTime = gameTime.TotalGameTime.TotalSeconds;

            if (mActive)
            {
                if (nowTime - mLastHealTick < TickDelay)
                {
                    return;
                }

                foreach (var entity in mHealingEntities)
                {
                    entity.mCurrentHealthPoints = Math.Clamp(entity.mCurrentHealthPoints + HealValue, Globals.Zero, entity.mMaxHealthPoints);
                    entity.UpdateHealthBar();
                }
            }
            else
            {
                // health fountain is 15 seconds inactive after usage
                if (nowTime - mLastHealTick < TimeInactive)
                {
                    return;
                }

                mActive = true;
            }



            mLastHealTick = nowTime;
        }

        /// <summary>
        /// Adds the given entity to this fountains healing receivers. The entity is not added if it's not close enough.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        internal void HealMe(MovableEntity entity)
        {
            if ((entity.mPosition.ToVector2() - mPosition.ToVector2()).Length() > HealingRange || mHealingEntities.Contains(entity))
            {
                return;
            }
            mHealingEntities.Add(entity);
            SoundManager.Default.PlaySoundEffect("SoundEffects/Boost", mPosition, mCamera);
        }

        /// <summary>
        /// Draws the health fountain according to its activeness
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        public override void Draw(SpriteBatch spriteBatch, Point position)
        {
            if (mHealingEntities.Count > 0 && mActive)
            {
                base.Draw(spriteBatch, position);
            }
            else
            {
                spriteBatch.Draw(mInactiveTexture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
                    position.Y - mTextureSize.Y), mTextureSize), Color.White);
            }
        }

        /// <summary>
        /// Checks out class after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mInactiveTexture = Content.LoadTexture("Images/Entities/HealthFountainInactive");
            mMiniMapColor = sMiniMapColor;
            SetMiniMapTexture(mMiniMapColor);
        }
    }
}
