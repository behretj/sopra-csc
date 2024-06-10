using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities.Ai;
using SopraProjekt.Entities.AI;
using SopraProjekt.GameState;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SopraProjekt.Entities.Functional;

namespace SopraProjekt.Entities
{

    /// <summary>
    /// Class to represent a Hero
    /// </summary>
    [Serializable]
    public abstract class Hero : MovableEntity
    {
        //List containing the five heroes.
        internal static readonly List<Hero> sAllHeroes = new List<Hero>();
        public int mSkillRange;
        protected int mSkillCosts;

        // Variables for shield
        internal bool mHasShield;
        private int mShieldHealth;
        private const int MaxShieldHealth = 50;
        private const int ShieldLossTime = 100;
        private const int MaxShieldRemainingTime = 5000;
        private int mShieldRemainingTime;
        private bool mShieldLossDebounced;

        // Variables for crusher
        protected bool mCrushing;
        private bool mNewCrushLocation;
        private int mCrushingLocationX;
        private int mCrushingLocationY;
        private const int CrushingLossTime = 100;
        private const int MaxCrushingRemainingTime = 1000;
        private int mCrushingRemainingTime;
        private bool mCrushingLossDebounced;

        // Variables for healer
        internal bool mHealing;
        private const int MaxHealingRemainingTime = 1500;
        private int mHealingRemainingTime;
        private bool mHealingLossDebounced;
        private const int HealingTickTime = 100;
        private const int HealingValue = 10;

        // Variables for tank
        internal bool mOxygenFill;
        private const int MaxOxygenRemainingTime = 1500;
        private int mOxygenRemainingTime;
        private bool mOxygenFillDebounced;
        private const int OxygenTickTime = 100;
        private const int OxygenValue = 10;

        // Variable for Skill cool down
        protected const int MaxSkillCoolDownTime = 500;
        public int mRemainingSkillCoolDownTime;
        private const int SkillCoolDownTickTime = 100;
        private bool mSkillCoolDownDebounced;

        private const int OxygenLossTime = 600;
        private bool mOxygenLossDebounced;
        internal string mAbilityTextureName;

        public int mNumberHealthPotions;
        public int mNumberOxygenPotions;
        internal Dictionary<Npc, GameTime> mForbiddenNpc;
        public List<int> mNumberHerbs;

        public const int HealingPotionHp = 200;
        public const int OxygenPotionOxygen = 200;


        // variables for the ai
        internal AiHandler.State AiState { get; set; }
        public bool AiEscape { get; set; }
        public bool AiInFightOrFlight { get; set; }
        public Entity AiTarget { get; set; }
        public bool AiHasChanged { get; set; }
        public WayPoint AiCurrentWayPoint { get; set; }
        public WayPoint AiLastWayPoint { get; set; }
        public int AiDirection { get; set; }


        [NonSerialized]
        public Point mAiDirection;
        private int mAiDirectionX;
        private int mAiDirectionY;

        [NonSerialized]
        public Point mAiDestination;
        private int mAiDestinationX;
        private int mAiDestinationY;


        [NonSerialized]
        internal Texture2D mAbilityTexture;

        public OxygenBar mOxygenBar;

        [NonSerialized]
        private Texture2D mShieldTexture;

        [NonSerialized]
        private Texture2D mCrushingTexture;

        [NonSerialized]
        private Texture2D mHealingTexture;

        [NonSerialized]
        private Texture2D mOxygenTexture;

        [NonSerialized]
        private Texture2D mFreezeTexture;

        [NonSerialized]
        public Point mOldPosition;



        // currently no need for HeightOffset:
        // private const int HeightOffset = 90;

        private const int Speed = 15;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the hero</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="abilityTextureName">the name of the texture of this hero's ability</param>
        /// <param name="camera">camera</param>
        protected Hero(string assetName,
            string title,
            Point position,
            Point textureSize,
            int team,
            string abilityTextureName,
            Camera camera) :
            base(assetName, title, position, textureSize, team, team == MovableEntity.EnemyTeam ? Color.Red : Color.Blue, Speed, camera)
        {
            sAllHeroes.Add(this);
            mAbilityTexture = Content.LoadTexture(abilityTextureName);
            mAbilityTextureName = abilityTextureName;
            mForbiddenNpc = new Dictionary<Npc, GameTime>();
            mOxygenBar = new OxygenBar(Content, new Point(position.X, position.Y - Globals.Eight), mTextureSize.X);
            AiInFightOrFlight = false;
            mNumberHealthPotions = 0;
            mNumberOxygenPotions = 0;
            mNumberHerbs = new List<int>(3) { 0, 0, 0 };
            mOldPosition = new Point(0, 0);

            mShieldTexture = Content.LoadTexture("Images/Abilities/shieldTexture1");
            mCrushingTexture = Content.LoadTexture("Images/Abilities/shatteredGlass1");
            mHealingTexture = Content.LoadTexture("Images/Abilities/heart");
            mOxygenTexture = Content.LoadTexture("Images/Abilities/oxygen_bubbles");
            mFreezeTexture = Content.LoadTexture("Images/Abilities/ice_block");

            // mCross = Content.LoadTexture("cross");
        }

        /// <summary>
        /// Implements a space holder for different skills of the heroes
        /// </summary>
        /// <param name="target">Entity which will be affected by the skill</param>
        public abstract void Skill(MovableEntity target);

        /// <summary>
        /// Method which activates a shield for the hero
        /// </summary>
        internal void ActivateShield()
        {
            mHasShield = true;
            mShieldHealth = MaxShieldHealth;
            mShieldRemainingTime = MaxShieldRemainingTime;
        }

        /// <summary>
        /// Method that activates crushing for Hero: Crusher
        /// </summary>
        protected void ActivateCrush()
        {
            mCrushing = true;
            mNewCrushLocation = true;
            mCrushingRemainingTime = MaxCrushingRemainingTime;
        }

        /// <summary>
        /// Method that activates healing if hero was in range of healer
        /// </summary>
        internal void ActivateHealing()
        {
            mHealing = true;
            mHealingRemainingTime = MaxHealingRemainingTime;
        }

        /// <summary>
        /// Method that activates oxygen refill if hero was in range of tank
        /// </summary>
        internal void ActivateFillOxygen()
        {
            mOxygenFill = true;
            mOxygenRemainingTime = MaxOxygenRemainingTime;
        }


        /// <summary>
        /// Draws the shield
        /// </summary>
        private void DrawShield(SpriteBatch spriteBatch, Point position)
        {
            int x;
            if (mShieldRemainingTime > 1000)
            {
                x = 50 + 2 * mShieldRemainingTime / ShieldLossTime;
            }
            else
            {
                x = 70;
            }

            Point textureSize = new Point(x, x);
            Point middle = new Point(position.X, position.Y - mTextureSize.Y / 2);
            spriteBatch.Draw(mShieldTexture,
                new Rectangle(new Point(middle.X - textureSize.X / 2, middle.Y - textureSize.Y / 2), textureSize),
                Color.White);
        }

        private void DrawCrushingTexture(SpriteBatch spriteBatch)
        {
            Point position = new Point(mCrushingLocationX, mCrushingLocationY);
            Point textureSize = new Point(400, 400);
            // Point middle = new Point(position.X, position.Y - heroSize.Y / 2);
            spriteBatch.Draw(mCrushingTexture,
                new Rectangle(new Point(position.X - textureSize.X / 2, position.Y - textureSize.Y / 2), textureSize),
                Color.White);
        }

        /// <summary>
        /// Draws a heart symbolizing the healing process
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        private void DrawHealing(SpriteBatch spriteBatch, Point position)
        {
            var transparency = (mHealingRemainingTime % 400) switch
            {
                300 => 0.9f,
                200 => 0.65f,
                100 => 0.3f,
                _ => 0.65f
            };

            var textureSize = new Point(100, 100);
            var middle = new Point(position.X, position.Y - mTextureSize.Y / 2);
            spriteBatch.Draw(mHealingTexture,
                new Rectangle(new Point(middle.X - textureSize.X / 2, middle.Y - textureSize.Y / 2), textureSize),
                Color.White * transparency);
        }

        /// <summary>
        /// Draws O2 bubbles symbolizing the refilling of oxygen
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        private void DrawOxygen(SpriteBatch spriteBatch, Point position)
        {
            var moveY = 50 - 5 * mOxygenRemainingTime / 50;
            var textureSize = new Point(100, 100);
            var middle = new Point(position.X, position.Y - mTextureSize.Y / 2 + moveY);
            spriteBatch.Draw(mOxygenTexture,
                new Rectangle(new Point(middle.X - textureSize.X / 2, middle.Y - textureSize.Y / 2), textureSize),
                Color.White);
        }

        /// <summary>
        /// Draws a heart symbolizing the healing process
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        private void DrawFreeze(SpriteBatch spriteBatch, Point position)
        {
            var textureSize = new Point(mTextureSize.X - Globals.Twenty, mTextureSize.Y - Globals.Twenty);
            spriteBatch.Draw(mFreezeTexture,
                new Rectangle(new Point(position.X - textureSize.X / Globals.Two, position.Y - textureSize.Y - Globals.Ten), textureSize),
                Color.White * 0.7f);
        }

        /// <summary>
        /// Draws Hero on the map
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        public override void Draw(SpriteBatch spriteBatch, Point position)
        {
            if (mNextPositions.Count > 0)
            {
                position = IsoHelper.TwoDToIsometric(mNextPositions.Peek()).ToPoint();
            }

            // Draw crushing texture
            if (mCrushing)
            {
                if (mNewCrushLocation)
                {
                    mCrushingLocationX = position.X;
                    mCrushingLocationY = position.Y;
                    mNewCrushLocation = false;
                }
                DrawCrushingTexture(spriteBatch);
            }

            var heroPosition = new Point(position.X - mTextureSize.X / 2, position.Y - mTextureSize.Y);

            if (mTheSprite != null)
            {
                mTheSprite.Draw(spriteBatch, position);

                // unused??
                // var activeAnimation = mTheSprite.GetActiveAnimation();
                // Added for loading a stored game
                if (Bar is null)
                {
                    Bar = new HealthBar(Entity.Content, heroPosition, mTextureSize.X);
                    Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
                }

                Bar.Move(heroPosition);
                //Bar.Move(new Point((position.X - activeAnimation.FrameWidth / 3),
                //    (position.Y - activeAnimation.FrameHeight / 10) - HeightOffset));
                Bar.Draw(spriteBatch);

                // Added for loading a stored game
                if (mOxygenBar is null)
                {
                    mOxygenBar = new OxygenBar(Entity.Content,
                        new Point(heroPosition.X, heroPosition.Y - Globals.Eight),
                        mTextureSize.X);
                    mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
                }

                mOxygenBar.Move(new Point(heroPosition.X, heroPosition.Y - Globals.Eight));
                //mOxygenBar.Move(new Point((position.X - activeAnimation.FrameWidth/3),
                //    (position.Y - activeAnimation.FrameHeight / 10 - Globals.Eight) - HeightOffset));
                mOxygenBar.Draw(spriteBatch);
            }

            // Draw missiles
            for (var i = Missiles.Count - 1; i >= 0; i--)
            {
                Missiles[i].Draw(spriteBatch);
            }

            // Draw Shield
            if (mHasShield)
            {
                DrawShield(spriteBatch, position);
            }

            // Draw healing process
            if (mHealing)
            {
                DrawHealing(spriteBatch, position);
            }

            // Draw refill of oxygen
            if (mOxygenFill)
            {
                DrawOxygen(spriteBatch, position);
            }

            if (mFreeze)
            {
                DrawFreeze(spriteBatch, position);
            }
        }


        /// <summary>
        /// Initializes Animations if needed
        /// </summary>
        public abstract void InitializeAnimation();
        public abstract void InitializeAnimationEnemy();

        /// <summary>
        /// Handles the damage which the object gets
        /// </summary>
        /// <param name="value"></param> Value of Damage
        public override void GetDamage(int value)
        {
            if (!mHasShield)
            {
                mCurrentHealthPoints -= value;
            }
            else
            {
                mShieldHealth -= value;
                if (mShieldHealth <= 0)
                {
                    mHasShield = false;
                    mCurrentHealthPoints += mShieldHealth;
                    mShieldHealth = 0;
                }
            }
            // Added for Statistic
            if (mTeam == MovableEntity.OwnTeam)
            {
                StatisticState.mTotalSufferedDamage.mValue += value;
            }

            mCurrentHealthPoints = Math.Clamp(mCurrentHealthPoints, -1, mMaxHealthPoints);
            Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
        }

        public bool CheckAttackRange(MovableEntity target)
        {
            if (DistanceToTarget(target) <= mAttackingRange && target.IsAlive && mAttacking)
            {
                // stay if in range to attack
                //mMovetoField = new Vector2(0, 0);
                //mMovePath = new Stack<Point>();
                return true;
            }

            return false;
        }

        internal override void Update(GameTime gameTime)
        {
            // already in base.Update():
            //if (mNextPositions.Count > 0)
            //{
            //    mNextPositions.Dequeue();
            //}
            //else
            //{
            //    mWalkedNextPositions = true;
            //}

            var doAttack = false;
            base.Update(gameTime);

            if (mTargetedEntity != null)
            {
                doAttack = CheckAttackRange(mTargetedEntity);
            }

            if (mTheSprite == null)
            {
                return;
            }

            if (mCurrentHealthPoints <= 0 && IsAlive)
            {
                KillHero();
            }

            mTheSprite.Update(gameTime);
            if (doAttack)
            {
                mTheSprite.Attack(true, mTargetedEntity, mPosition);
            }

            if (mFreeze && gameTime.TotalGameTime.TotalMilliseconds >= mFreezeTime)
            {
                mFreeze = false;
            }

            // Cool down update
            if (gameTime.TotalGameTime.Milliseconds % SkillCoolDownTickTime == 0)
            {
                if (!mSkillCoolDownDebounced)
                {
                    return;
                }

                if (mRemainingSkillCoolDownTime > 0)
                {
                    mRemainingSkillCoolDownTime -= SkillCoolDownTickTime;
                }
                else
                {
                    mRemainingSkillCoolDownTime = 0;
                }

                mSkillCoolDownDebounced = false;
            }
            else
            {
                mSkillCoolDownDebounced = true;
            }

            UpdateCarrySkill(gameTime);
            UpdateCrusherSkill(gameTime);
            UpdateTankSkill(gameTime);
            UpdateHealerSkill(gameTime);
            UpdateOxygen(gameTime);
            UpdateHealth(gameTime);
        }

        /// <summary>
        /// Updates Crusher's special skill.
        /// </summary>
        private void UpdateCarrySkill(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % ShieldLossTime == 0)
            {
                if (!mShieldLossDebounced)
                {
                    return;
                }

                if (mShieldRemainingTime > 0)
                {
                    mShieldRemainingTime -= ShieldLossTime;
                }
                else
                {
                    mHasShield = false;
                    mShieldHealth = 0;
                }

                mShieldLossDebounced = false;
            }
            else
            {
                mShieldLossDebounced = true;
            }

        }

        /// <summary>
        /// Updates Crusher's special skill.
        /// </summary>
        private void UpdateCrusherSkill(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % CrushingLossTime == 0)
            {
                if (!mCrushingLossDebounced)
                {
                    return;
                }

                if (mCrushingRemainingTime > 0)
                {
                    mCrushingRemainingTime -= 100;
                }
                else
                {
                    mCrushing = false;
                }

                mCrushingLossDebounced = false;
            }
            else
            {
                mCrushingLossDebounced = true;
            }

        }

        /// <summary>
        /// Updates Tank's special skill.
        /// </summary>
        private void UpdateTankSkill(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % OxygenTickTime == 0)
            {
                if (!mOxygenFillDebounced)
                {
                    return;
                }

                if (mOxygenRemainingTime > 0)
                {
                    mCurrentOxygenPoints += OxygenValue;
                    if (mCurrentOxygenPoints > mMaxOxygenPoints)
                    {
                        mCurrentOxygenPoints = mMaxOxygenPoints;
                    }

                    mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
                    mOxygenRemainingTime -= OxygenTickTime;
                }
                else
                {
                    mOxygenFill = false;
                }

                mOxygenFillDebounced = false;
            }
            else
            {
                mOxygenFillDebounced = true;
            }
        }

        /// <summary>
        /// Updates Healer's special skill.
        /// </summary>
        private void UpdateHealerSkill(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % HealingTickTime == 0)
            {
                if (!mHealingLossDebounced)
                {
                    return;
                }

                if (mHealingRemainingTime > 0)
                {
                    mCurrentHealthPoints += HealingValue;
                    if (mCurrentHealthPoints > mMaxHealthPoints)
                    {
                        mCurrentHealthPoints = mMaxHealthPoints;
                    }

                    Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
                    mHealingRemainingTime -= HealingTickTime;
                }
                else
                {
                    mHealing = false;
                }
                mHealingLossDebounced = false;

            }
            else
            {
                mHealingLossDebounced = true;
            }
        }

        /// <summary>
        /// Updates oxygen bar.
        /// </summary>
        private void UpdateOxygen(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % OxygenLossTime == 0)
            {
                if (!mOxygenLossDebounced)
                {
                    return;
                }

                mCurrentOxygenPoints -= 1;
                mOxygenLossDebounced = false;
                mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
            }
            else
            {
                mOxygenLossDebounced = true;
            }
        }

        /// <summary>
        /// Updates health bar if
        /// </summary>
        private void UpdateHealth(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % OxygenLossTime == 0)
            {
                if (mCurrentOxygenPoints <= 0)
                {
                    mCurrentHealthPoints -= 3;
                }

                Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
            }
        }

        /// <summary>
        /// Updates the oxygenBar of this entity
        /// </summary>
        public override void UpdateOxygenBar()
        {
            mOxygenBar.Update(mCurrentOxygenPoints, mMaxOxygenPoints);
        }


        /// <summary>
        /// Kills the Hero
        /// </summary>
        internal void KillHero()
        {
            if (!IsAlive)
            {
                return;
            }

            IsAlive = false;
            sAllHeroes.Remove(this);
            SoundManager.Default.PlaySoundEffect("SoundEffects/Sterben", mPosition, mCamera);
            if (mTeam == OwnTeam)
            {
                StatisticState.mTotalDiedAllies.mValue += 1;
            }

            //if (sAllHeroes.Count == 0)
            //{
            //    Debug.WriteLine("end");
            //}
        }

        /// <summary>
        /// Prepares serialization
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mAiDestinationX = mAiDestination.X;
            mAiDestinationY = mAiDestination.Y;
            mAiDirectionX = mAiDirection.X;
            mAiDirectionY = mAiDirection.Y;
        }

        /// <summary>
        /// Checks out deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mAiDestination = new Point(mAiDestinationX, mAiDestinationY);
            mAiDirection = new Point(mAiDirectionX, mAiDirectionY);
            mTexture = Content.LoadTexture(mAssetName);
            mAbilityTexture = Content.LoadTexture(mAbilityTextureName);
            mShieldTexture = Content.LoadTexture("Images/Abilities/shieldTexture1");
            mCrushingTexture = Content.LoadTexture("Images/Abilities/shatteredGlass1");
            mHealingTexture = Content.LoadTexture("Images/Abilities/heart");
            mOxygenTexture = Content.LoadTexture("Images/Abilities/oxygen_bubbles");
            mFreezeTexture = Content.LoadTexture("Images/Abilities/ice_block");
            if (mTeam == 1)
            {
                InitializeAnimationEnemy();
            }
            else
            {
                InitializeAnimation();
            }
            SetMiniMapTexture(mTeam == 1 ? Color.Red : Color.Blue);
            StepSpeed = Speed;
        }

        /// <summary>
        /// Returns true if herb was collected and
        /// increases amount of herbs in hero inventory by 1 for each collected herb.
        /// </summary>
        /// <param name="herb"></param>
        internal bool CollectHerb(Collectable herb)
        {
            if (DistanceToHerb(herb) > HerbPickupDistance)
            {
                return false;
            }

            mNumberHerbs[GetIngredientIndex(herb.Title)] += 1;
            herb.Collected();
            return true;
        }

        /// <summary>
        /// Use a health potion
        /// </summary>
        public bool UseHealthPotion()
        {
            if (mNumberHealthPotions < 1)
            {
                return false;
            }

#if verboseDebug
            Debug.WriteLine("Hero" + Title + " at " + mPosition + " uses health potion");
#endif

            mNumberHealthPotions -= 1;
            SoundManager.Default.PlaySoundEffect("SoundEffects/bubbling_short", mPosition, mCamera);
            mCurrentHealthPoints = Math.Clamp(mCurrentHealthPoints + HealingPotionHp, 0, mMaxHealthPoints);
            return true;
        }

        /// <summary>
        /// Use a health potion
        /// </summary>
        public void UseOxygenPotion()
        {
            if (mNumberOxygenPotions < 1)
            {
                return;
            }

#if verboseDebug
            Debug.WriteLine("Hero" + Title + " at " + mPosition + " uses oxygen potion");
#endif
            mNumberOxygenPotions -= 1;
            SoundManager.Default.PlaySoundEffect("SoundEffects/bubbling_short", mPosition, mCamera);
            mCurrentOxygenPoints = Math.Clamp(mCurrentOxygenPoints + OxygenPotionOxygen, 0, mMaxOxygenPoints);
        }


        /// <summary>
        /// Add ingredients
        /// </summary>
        /// <param name="ingredientAssentName">name of the ingredient to add</param>
        /// <param name="number">number of ingredients to add</param>
        public void AddPotionIngredients(string ingredientAssentName, int number = 1)
        {
            mNumberHerbs[GetIngredientIndex(ingredientAssentName)] += number;
        }

        /// <summary>
        /// get the index of the ingredient
        /// </summary>
        /// <param name="ingrediantAssentName">asset name of the ingredient</param>
        /// <returns></returns>
        private int GetIngredientIndex(string ingrediantAssentName)
        {
            return ingrediantAssentName switch
            {
                "herb_blau1" => 0,
                "herb_lila2" => 1,
                "herb_pink3" => 2,
                _ => -1
            };
        }

        /// <summary>
        /// Freeze the entity
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="freezeTime"></param>
        public void Freeze(GameTime gameTime, int freezeTime)
        {
            mFreezeTime = (int)gameTime.TotalGameTime.TotalMilliseconds + freezeTime;
            mFreeze = true;
        }
    }
}
