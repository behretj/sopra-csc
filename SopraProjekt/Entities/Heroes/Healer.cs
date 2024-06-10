using Microsoft.Xna.Framework;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities.Heroes
{
    /// <summary>
    /// Class to represent the Healer
    /// </summary>
    [Serializable]
    public sealed class Healer : Hero
    {
        private const int MaxHealthPoints = 300;
        private const int MaxOxygenPoints = 400;

        private const int TextureSizeX = 95;
        private const int TextureSizeY = 180;
        private const string OwnAssetName = "Images/HeroesFront/DoctorFront";
        private const string AbilityAssetName = "Images/Abilities/Doctor";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="team"></param>
        /// <param name="camera"></param>
        public Healer(Point position, int team, Camera camera) :
            base(OwnAssetName, "hero_healer", position, new Point(TextureSizeX, TextureSizeY), team, AbilityAssetName, camera)
        {
            mMaxHealthPoints = MaxHealthPoints;
            mMaxOxygenPoints = MaxOxygenPoints;

            mCurrentHealthPoints = mMaxHealthPoints;
            mCurrentOxygenPoints = mMaxOxygenPoints;

            mDamage = 10;
            mAttackingRange = 4;
            mSkillRange = 10;
            mSkillCosts = 100;

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
        /// Initializes Animations
        /// </summary>
        public override void InitializeAnimation()
        {
            List<Animation> healerSpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                healerSpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/HealerSpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(healerSpriteInfos);
        }

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
        /// Implements special healing skill
        /// </summary>
        /// <param name="entities">Entities that were in range of healer</param>
        public void UseSkill(List<Entity> entities)
        {
            if (mCurrentOxygenPoints - mSkillCosts < LowerAbilityLimit || mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            mRemainingSkillCoolDownTime = MaxSkillCoolDownTime;
            mCurrentOxygenPoints -= mSkillCosts;
            mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
            SoundManager.Default.PlaySoundEffect("SoundEffects/power_increase", mPosition, mCamera);
            foreach (var entity in entities)
            {
                if (entity is MovableEntity target && target.Id != Id)
                {
                    Skill(target);
                }
            }
        }

        /// <summary>
        /// Implements a skill that heals an other allie
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
                hero.ActivateHealing();
            }
        }
    }
}