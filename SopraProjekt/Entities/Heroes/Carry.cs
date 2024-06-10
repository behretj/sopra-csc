using Microsoft.Xna.Framework;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SopraProjekt.Entities.Heroes
{
    /// <summary>
    /// Class to represent the Carry
    /// </summary>

    [Serializable]
    public sealed class Carry : Hero
    {
        private const int TextureSizeX = 95;
        private const int TextureSizeY = 180;
        private const string OwnAssetName = "Images/HeroesFront/CarryFront";
        private const string AbilityAssetName = "Images/Abilities/Carry";

        private const int MaxHealthPoints = 700;
        private const int MaxOxygenPoints = 400;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="camera"></param>
        public Carry(Point position, int team, Camera camera) : base(OwnAssetName, "hero_carry", position, new Point(TextureSizeX, TextureSizeY), team, AbilityAssetName, camera)
        {
            // Todo: Values must be adjusted
            mMaxHealthPoints = MaxHealthPoints;
            mMaxOxygenPoints = MaxOxygenPoints;

            mCurrentHealthPoints = mMaxHealthPoints;
            mCurrentOxygenPoints = mMaxOxygenPoints;

            mDamage = 30;
            mAttackingRange = 4;
            mSkillRange = 20;
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
        }

        /// <summary>
        /// Initializes Animations
        /// </summary>
        public override void InitializeAnimation()
        {
            List<Animation> carrySpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                carrySpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/CarrySpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(carrySpriteInfos);
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
        /// Implements a skill which gives other allies a shield
        /// </summary>
        /// <param name="target">Entity which will be affected by the skill</param>
        public override void Skill(MovableEntity target)
        {
            if (mCurrentOxygenPoints - mSkillCosts < LowerAbilityLimit || mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            mRemainingSkillCoolDownTime = MaxSkillCoolDownTime;
            mCurrentOxygenPoints -= mSkillCosts;
            ActivateShield();
            foreach (var hero in sAllHeroes.Where(hero => IsInTeam(hero) && DistanceToTarget(hero) <= mSkillRange))
            {
                MovableEntity entity = hero;
                entity.Missiles.Add(new Shield(mPosition.ToVector2(), entity.mPosition.ToVector2(), mTeam, Id, Content, mCamera));
            }
        }
    }
}

