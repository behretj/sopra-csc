using Microsoft.Xna.Framework;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Entities.Monsters;
using SopraProjekt.GameState;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities.Heroes
{
    /// <summary>
    /// Class to represent the Crusher
    /// </summary>
    [Serializable]
    public sealed class Crusher : Hero
    {
        public const int SkillDamage = 20;

        private const int MaxHealthPoints = 700;
        private const int MaxOxygenPoints = 400;

        private const int TextureSizeX = 95;
        private const int TextureSizeY = 180;
        private const string OwnAssetName = "Images/HeroesFront/BrecherFront";
        private const string AbilityAssetName = "Images/Abilities/Crusher"; // do we need this

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="camera">camera</param>
        internal Crusher(Point position, int team, Camera camera) :
            base(OwnAssetName, "hero_crusher", position, new Point(TextureSizeX, TextureSizeY), team, AbilityAssetName, camera)
        {
            // Todo: Values must be adjusted
            mMaxHealthPoints = MaxHealthPoints;
            mMaxOxygenPoints = MaxOxygenPoints;

            mCurrentHealthPoints = mMaxHealthPoints;
            mCurrentOxygenPoints = mMaxOxygenPoints;

            mDamage = 15;
            mAttackingRange = 4;
            mSkillRange = 5;
            mSkillCosts = 50;

            //assign the right spritesheet
            if (team == OwnTeam)
            {
                InitializeAnimation();
            }
            else
            {
                InitializeAnimationEnemy();
            }

            mCamera = camera;
        }

        /// <summary>
        /// Initializes Animations
        /// </summary>
        public override void InitializeAnimation()
        {
            List<Animation> crusherSpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                crusherSpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/CrusherSpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(crusherSpriteInfos);
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

        public override void Skill(MovableEntity target)
        { }

        /// <summary>
        /// Implements a Special Fighting attack for the Crusher
        /// </summary>
        /// <param name="targets">Entities in near range of crusher</param>
        public void Skill(List<Entity> targets)
        {
            if (mCurrentOxygenPoints - mSkillCosts < LowerAbilityLimit || mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            mRemainingSkillCoolDownTime = MaxSkillCoolDownTime;
            mCurrentOxygenPoints -= mSkillCosts;
            ActivateCrush();
            foreach (var target in targets)
            {
                if (target is MovableEntity movableEntity && !IsInTeam(movableEntity) && DistanceToTarget(movableEntity) <= mSkillRange)
                {
                    movableEntity.GetDamage(SkillDamage);
                    // Added for Statistic
                    if (mTeam == MovableEntity.OwnTeam)
                    {
                        StatisticState.mTotalDealtDamage.mValue += SkillDamage;
                    }

                    if (movableEntity.mCurrentHealthPoints <= 0 && mTeam == OwnTeam)
                    {
                        if (movableEntity is Monster)
                        {
                            StatisticState.mTotalKilledMonsters.mValue += 1;
                            AchievementState.UpdateAchievement("Monster Hunter");
                            AchievementState.UpdateAchievement("Monster Killer");
                            AchievementState.UpdateAchievement("Monster Master");
                        }

                        if (movableEntity is Hero)
                        {
                            StatisticState.mTotalKilledEnemyHeroes.mValue += 1;
                        }
                    }
                }

            }
            SoundManager.Default.PlaySoundEffect("SoundEffects/crushingSound", mPosition, mCamera);
        }
    }
}