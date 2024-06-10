using Microsoft.Xna.Framework;
using SopraProjekt.AnimationManagement;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities.Heroes
{
    /// <summary>
    /// Class to represent the Sniper
    /// </summary>
    [Serializable]
    public sealed class Sniper : Hero
    {
        private const int MaxHealthPoints = 300;
        private const int MaxOxygenPoints = 400;

        private const int TextureSizeX = 95;
        private const int TextureSizeY = 180;
        private const string OwnAssetName = "Images/HeroesFront/SniperFront";
        private const string AbilityAssetName = "Images/Abilities/Sniper";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">initial position of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="camera"></param>
        public Sniper(Point position, int team, Camera camera) :
            base(OwnAssetName, "hero_sniper", position, new Point(TextureSizeX, TextureSizeY), team, AbilityAssetName, camera)
        {
            mMaxHealthPoints = MaxHealthPoints;
            mMaxOxygenPoints = MaxOxygenPoints;

            mCurrentHealthPoints = mMaxHealthPoints;
            mCurrentOxygenPoints = mMaxOxygenPoints;

            mDamage = 10;
            mAttackingRange = 4;
            mSkillRange = 15;
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
            List<Animation> sniperSpriteInfos = new List<Animation>();
            for (int i = 0; i != 8; i++)
            {
                sniperSpriteInfos.Add(new Animation(
                    i, Content.LoadTexture("Images/HeroSpriteSheets/SniperSpriteSheet"), 5, mTextureSize, this, mCamera));
            }

            mTheSprite = new AnimationSprite(sniperSpriteInfos);
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
        /// Implements a special sniping fighting attack
        /// </summary>
        /// <param name="target">Entity which will be affected by the skill</param>
        public override void Skill(MovableEntity target)
        {
            if (DistanceToTarget(target) > mSkillRange || mCurrentOxygenPoints - mSkillCosts < LowerAbilityLimit || IsInTeam(target) || mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            mRemainingSkillCoolDownTime = MaxSkillCoolDownTime;

            mCurrentOxygenPoints -= mSkillCosts;
            mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
            // Missile is targeted onto the target entity:
            target.Missiles.Add(new Missile(mPosition.ToVector2(), target.mPosition.ToVector2(), mTeam, Id, Content, mCamera));
        }
    }
}