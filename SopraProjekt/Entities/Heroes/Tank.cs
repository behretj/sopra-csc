using Microsoft.Xna.Framework;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities.Heroes
{
    /// <summary>
    /// Class to represent the Tank
    /// </summary>
    [Serializable]
    public sealed class Tank : Hero
    {
        private const int MaxHealthPoints = 800;
        private const int MaxOxygenPoints = 800;

        private const int TextureSizeX = 95;
        private const int TextureSizeY = 180;
        private const string OwnAssetName = "Images/HeroesFront/TankFront";
        private const string AbilityAssetName = "Images/Abilities/Tank";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="camera">camera</param>
        public Tank(Point position, int team, Camera camera) :
            base(OwnAssetName, "hero_tank", position, new Point(TextureSizeX, TextureSizeY), team, AbilityAssetName, camera)
        {
            // Todo: Values must be adjusted
            mMaxHealthPoints = MaxHealthPoints;
            mMaxOxygenPoints = MaxOxygenPoints;

            mCurrentHealthPoints = mMaxHealthPoints;
            mCurrentOxygenPoints = mMaxOxygenPoints;

            mDamage = 30;
            mAttackingRange = 4;
            mSkillRange = 7;
            mSkillCosts = 200;

            mCamera = camera;

            //assign the right spritesheet
            if (team == OwnTeam)
            {
                InitializeAnimation();
            }
            else
            {
                InitializeAnimationEnemy();
            }


        }

        /// <summary>
        /// Initializes Animations of Own Team Tank
        /// </summary>
        public override void InitializeAnimation()
        {
            List<Animation> carrySpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                carrySpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/TankSpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(carrySpriteInfos);
        }
        /// <summary>
        /// Initializes Animations of Enemy Tank
        /// </summary>
        public override void InitializeAnimationEnemy()
        {
            List<Animation> carrySpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                carrySpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/GegnerSpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(carrySpriteInfos);
        }

        /// <summary>
        /// Implements special oxygen skill
        /// </summary>
        /// <param name="entities">Entities that were in range of tank</param>
        public void UseSkill(List<Entity> entities)
        {
            if (mCurrentOxygenPoints - mSkillCosts < LowerAbilityLimit || mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            mRemainingSkillCoolDownTime = MaxSkillCoolDownTime;
            mCurrentOxygenPoints -= mSkillCosts;
            mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
            SoundManager.Default.PlaySoundEffect("SoundEffects/bubbling_short", mPosition, mCamera);
            foreach (var entity in entities)
            {
                if (entity is MovableEntity target && target.Id != Id)
                {
                    Skill(target);
                }
            }
        }

        /// <summary>
        /// Implements a skill that transfers oxygen to an other allie
        /// </summary>
        /// <param name="target">Entity which will be affected by the skill</param>
        public override void Skill(MovableEntity target)
        {
            if (DistanceToTarget(target) >= mSkillRange || !IsInTeam(target))
            {
                return;
            }

            if (target is Hero hero)
            {
                hero.ActivateFillOxygen();
            }
        }
    }
}