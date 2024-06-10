using Microsoft.Xna.Framework;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class that represents a shot shield
    /// </summary>
    [Serializable]
    public sealed class Shield : SkillItem
    {
        [NonSerialized]
        private Camera mCamera;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">source position (shield was shot at this position)</param>
        /// <param name="target">target position (shield will fly to that point)</param>
        /// <param name="team">team that the shield belongs to</param>
        /// <param name="id">Id of the entity that shot the shield</param>
        /// <param name="content">content loader to get texture of shield</param>
        /// <param name="camera">camera</param>
        internal Shield(Vector2 source, Vector2 target, int team, int id, ContentLoader content, Camera camera) :
            base(source, target, team, id, content, "Images/Abilities/shieldTexture1")
        {
            mTextureSize = new Point(40, 40);
            mCamera = camera;
        }

        /// <summary>
        /// Updates the logic of the shield
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
                // checks whether something is in intersects with the missile
                var collidedEntities =
                    map.GetEntitiesIn(new Rectangle(new Point(currentPointRounded.X - 1, currentPointRounded.Y - 1),
                        new Point(2, 2)));
                // if the missile hits something, the collidedEntities list is not empty
                if (collidedEntities.Count > 0)
                {
                    if (mShotById != collidedEntities[0].Id)
                    {
                        if (collidedEntities[0] is Hero hero)
                        {
                            if (hero.mTeam == mTeam)
                            {
                                hero.ActivateShield();
                                SoundManager.Default.PlaySoundEffect("SoundEffects/shieldSound", mCurrentPosition.ToPoint(), mCamera);
                            }
                        }
                    }
                }
            }
        }
    }
}
