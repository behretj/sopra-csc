using Microsoft.Xna.Framework;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class that represents a shot missile
    /// </summary>
    [Serializable]
    public sealed class Missile : SkillItem
    {
        [NonSerialized]
        private Camera mCamera;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">source position (missile was shot at this position)</param>
        /// <param name="target">target position (missile will fly to that point)</param>
        /// <param name="team">team that the missile belongs to</param>
        /// <param name="id">Id of the entity that shot the missile</param>
        /// <param name="content">content loader to get texture of missile</param>
        /// <param name="camera">camera</param>
        internal Missile(Vector2 source, Vector2 target, int team, int id, ContentLoader content, Camera camera) :
            base(source, target, team, id, content, "Images/Abilities/Pfeil2")
        {
            mTextureSize = new Point(50, 10);
            mCamera = camera;
        }

        /// <summary>
        /// Updates the logic of the missile
        /// </summary>
        /// <param name="map">map, which the missile flies on</param>
        internal override void Update(Map map)
        {
            if (mMovePath.Count > 0)
            {
                // get the next position of the MovePath
                mCurrentPosition = mMovePath.Dequeue();
                var currentPoint = mCurrentPosition;
                currentPoint.Round();
                var currentPointRounded = currentPoint.ToPoint();
                // checks whether something intersects with the missile
                var collidedEntities = map.GetEntity(currentPointRounded);
                if (collidedEntities.Count > 0)
                {
                    if (mShotById != collidedEntities[0].Id)

                    {
                        if (collidedEntities[0] is MovableEntity shotEntity)
                        {
                            SoundManager.Default.PlaySoundEffect("SoundEffects/soundEffectLaserShoot", mCurrentPosition.ToPoint(), mCamera);
                            shotEntity.Sniped(mTeam);
                            mMovePath.Clear();
                        }
                        else if (collidedEntities[0] is Colliding && collidedEntities[0].mCollision)
                        {
                            mMovePath.Clear();
                        }
                    }
                }
            }
        }
    }
}
