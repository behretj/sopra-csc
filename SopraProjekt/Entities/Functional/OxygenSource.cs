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
    internal sealed class OxygenSource : Colliding
    {
        private double mLastSupplyTick;
        private readonly List<MovableEntity> mSupplyingEntities = new List<MovableEntity>();

        private bool mActive;

        private const float SupplyRange = 10.0f;
        public const int SupplyValue = 30;
        private const int TickDelay = 1;
        private const int TimeInactive = 15;

        [NonSerialized]
        internal Texture2D mInactiveTexture;

        // Rendering constants
        [NonSerialized]
        private static readonly Color sMiniMapColor = Color.Cyan;
        private const int TextureSize = 128;
        private const string OwnAssetName = "Images/Entities/OxygenFountain2";

        [NonSerialized]
        private Camera mCamera;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="position">Position of the health fountain.</param>
        /// <param name="camera"></param>
        internal OxygenSource(Point position, Camera camera) : base(OwnAssetName, "oxygen_source", position, new Point(TextureSize), sMiniMapColor)
        {
            mInactiveTexture = Content.LoadTexture("Images/Entities/OxygenFountainInactive");
            mActive = true;
            mCamera = camera;
        }

        /// <summary>
        /// Updates this fountains entities.
        /// </summary>
        /// <param name="gameTime"></param>
        internal override void Update(GameTime gameTime)
        {
            var supplyCount = mSupplyingEntities.Count;

            // check if any entity has walked out of range
            mSupplyingEntities.RemoveAll(entity => !InRange(mPosition, entity.mPosition));

            // if all entities have left the range of the oxygen source, the source gets inactive for a certain amount of time
            if (supplyCount > 0 && mSupplyingEntities.Count == 0)
            {
                mActive = false;
            }

            // For every second a entity is healed 20 HP Todo: determine better value
            var nowTime = gameTime.TotalGameTime.TotalSeconds;

            if (mActive)
            {
                if (nowTime - mLastSupplyTick < TickDelay)
                {
                    return;
                }

                foreach (var entity in mSupplyingEntities)
                {
                    entity.mCurrentOxygenPoints = Math.Clamp(entity.mCurrentOxygenPoints + SupplyValue, Globals.Zero, entity.mMaxOxygenPoints);
                    entity.UpdateOxygenBar();
                }
            }
            else
            {
                // oxygen source is 15 seconds inactive after usage
                if (nowTime - mLastSupplyTick < TimeInactive)
                {
                    return;
                }

                mActive = true;
            }

            mLastSupplyTick = nowTime;
        }

        /// <summary>
        /// Adds the given entity to this fountains supply receivers. The entity is not added if it's not close enough.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        internal void SupplyMe(MovableEntity entity)
        {
            if (!InRange(mPosition, entity.mPosition) || mSupplyingEntities.Contains(entity))
            {
                return;
            }

            mSupplyingEntities.Add(entity);
            SoundManager.Default.PlaySoundEffect("SoundEffects/Boost", mPosition, mCamera);
        }

        private static bool InRange(Point ownPos, Point otherPos)
        {
            return (ownPos - otherPos).ToVector2().Length() < SupplyRange;
        }

        /// <summary>
        /// Draws the oxygen source according to its activeness
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        public override void Draw(SpriteBatch spriteBatch, Point position)
        {
            if (mSupplyingEntities.Count > 0 && mActive)
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
        /// Checks out class after Deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mInactiveTexture = Content.LoadTexture("Images/Entities/OxygenFountainInactive");
            mMiniMapColor = sMiniMapColor;
            SetMiniMapTexture(mMiniMapColor);
        }
    }
}
