using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities.Functional;
using SopraProjekt.Entities.Monsters;
using SopraProjekt.GameState;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class to represent one Movable Entity
    /// </summary>
    [Serializable]
    public class MovableEntity : Entity
    {
        public const int LowerAbilityLimit = 5;
        public const double HerbPickupDistance = 5.0;

        public static List<MovableEntity> mAllMovableEntities = new List<MovableEntity>();
        public MovableEntity mTargetedEntity;


        [NonSerialized]
        public List<Point> mForbiddenfields = new List<Point>();
        private List<int> mForbiddenfieldsX;
        private List<int> mForbiddenfieldsY;


        // Outcommepublic float mMovementSpeed; (nt for resharper)
        internal int mCurrentHealthPoints;
        internal int mMaxHealthPoints;
        internal int mCurrentOxygenPoints;
        internal int mMaxOxygenPoints;
        internal int mDamage;
        protected int mAttackingRange;

        public bool mAttacking;
        public bool mInteracted;
        
        internal Spaceship mKnownSpaceShip;


        // Söldner = 1; Monster = 0; Allies = -1
        public readonly int mTeam;
        public const int EnemyTeam = 1;
        public const int NeutralTeam = 0;
        public const int OwnTeam = -1;

        public bool IsAlive { get; set; } = true;
        public HealthBar Bar { get; set; }

        protected bool mFreeze = false;
        protected int mFreezeTime;

        private int mNextAttack;
        public bool mWalkedNextPositions;
        public int mOldPositionCount;

        public List<SkillItem> Missiles { get; set; }

        [NonSerialized]
        private Random mRandom;

        [NonSerialized]
        protected Camera mCamera;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="title">title of the entity</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="team">team of the figure</param>
        /// <param name="minimapColor">Color on the MiniMap.</param>
        /// <param name="stepSpeed">speed of the entity - lower = faster</param>
        /// <param name="camera"></param>
        protected MovableEntity(string assetName, string title, Point position, Point textureSize, int team, Color minimapColor, int stepSpeed, Camera camera) :
            base(assetName, title, position, textureSize, minimapColor, stepSpeed)
        {
            mTeam = team;
            mAllMovableEntities.Add(this);
            mCollision = true;

            mDrawPriority = 2;

            // for testing:
            mMaxHealthPoints = 250;
            mCurrentHealthPoints = mMaxHealthPoints;

            Bar = new HealthBar(Content, position, mTextureSize.X);

            Missiles = new List<SkillItem>();

            mKnownSpaceShip = null;

            mTargetedEntity = null;

            mRandom = new Random();

            mCamera = camera;
        }

        /// <summary>
        /// Draws the entity with HealthBar and fine positions
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        public override void Draw(SpriteBatch spriteBatch, Point position)
        {

            if (mNextPositions.Count > 0)
            {
                position = IsoHelper.TwoDToIsometric(mNextPositions.Peek()).ToPoint();
            }

            //spriteBatch.Draw(mTexture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
            //    position.Y - mTextureSize.Y), mTextureSize), Color.White);
            base.Draw(spriteBatch, position);

            // Added for loading from stored game
            if (Bar is null)
            {
                Bar = new HealthBar(Entity.Content, position, mTextureSize.X);
                Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
            }

            Bar.Move(new Point(position.X - mTextureSize.X / 2,
                position.Y - mTextureSize.Y));
            if (this is Monster && mCurrentHealthPoints >= mMaxHealthPoints) { }
            else
            {
                Bar.Draw(spriteBatch);
            }

            // Draw Missiles
            if (Missiles == null)
            {
                Missiles = new List<SkillItem>();
            }
            for (var i = Missiles.Count - 1; i >= 0; i--)
            {
                Missiles[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Handles the damage which the object gets
        /// </summary>
        /// <param name="value"></param> Value of Damage 
        public virtual void GetDamage(int value)
        {
            mCurrentHealthPoints -= value;
            mCurrentHealthPoints = Math.Clamp(mCurrentHealthPoints, -1, mMaxHealthPoints);
            // entity lost health points, so HealthBar needs to be updated
            Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
        }

        /// <summary>
        /// Returns the distance between the current entity and the target
        /// </summary>
        /// <param name="target"></param> 
        protected float DistanceToTarget(MovableEntity target)
        {
            return (mPosition - target.mPosition).ToVector2().Length();
        }

        /// <summary>
        /// Returns distance between current entity and herb to be collected
        /// </summary>
        /// <param name="herb"></param> 
        internal float DistanceToHerb(Collectable herb)
        {
            return herb == null ? float.PositiveInfinity : (mPosition - herb.mPosition).ToVector2().LengthSquared();
        }


        /// <summary>
        /// Returns if target is in same team
        /// </summary>
        /// <param name="target"></param>
        /// <returns>True if target is in your team, else false</returns>
        public bool IsInTeam(MovableEntity target)
        {
            return (mTeam == target.mTeam);
        }

        /// <summary>
        /// Set the target entity
        /// </summary>
        /// <param name="target">target entity</param>
        public void SetTargetedEntity(MovableEntity target)
        {
            mTargetedEntity = target;
        }

        /// <summary>
        /// Method that implements attacking between entities
        /// </summary>
        /// <param name="target"></param> Target which will be attacked
        /// <param name="gameTime">game time</param>
        public void Attack(MovableEntity target, GameTime gameTime)
        {
            if (IsInTeam(target) || (DistanceToTarget(target) > mAttackingRange) || mFreeze)
            {
                return;
            }

            if (mNextAttack <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                mNextAttack = (int)gameTime.TotalGameTime.TotalMilliseconds + 1000;
                if (target.mCurrentHealthPoints > 0)
                {
                    target.GetDamage(mDamage);
                    // Added for Statistic
                    if (mTeam == MovableEntity.OwnTeam)
                    {
                        StatisticState.mTotalDealtDamage.mValue += mDamage;
                    }

                    if (target.mCurrentHealthPoints <= 0 && mTeam == OwnTeam)
                    {
                        if (target is Monster)
                        {
                            StatisticState.mTotalKilledMonsters.mValue += 1;
                            AchievementState.UpdateAchievement("Monster Hunter");
                            AchievementState.UpdateAchievement("Monster Killer");
                            AchievementState.UpdateAchievement("Monster Master");

                            SpawnNewMonster();
                        }

                        if (target is Hero)
                        {
                            StatisticState.mTotalKilledEnemyHeroes.mValue += 1;
                        }
                    }
                }
            }
        }

        private void SpawnNewMonster()
        {
            Point position;
            do
            {
                position = new Point(mRandom.Next(2, Map.MapSizeMaxX - 2), mRandom.Next(2, Map.MapSizeMaxX - 2));
            } while (!mMap.IsSpaceIn(new Rectangle(position.X - 1, position.Y - 1, 2, 2), MovableEntity.NeutralTeam) ||
                     mMap.mGameState.Heroes.mTeamMembers.Any(hero => Distance(hero.mPosition, position) < 20));

            switch (mRandom.Next(4))
            {
                case 0:
                    mMap.Register(new Fernkampfmonster(position, new Point(107, 200), mCamera));
                    break;
                case 1:
                    mMap.Register(new Kamikazemonster(position, new Point(107, 200), mCamera));
                    break;
                case 2:
                    mMap.Register(new Nahkampfmonster(position, new Point(107, 200), mCamera));
                    break;
                case 3:
                    mMap.Register(new Verwandlungsmonster(position, new Point(107, 200), mCamera));
                    break;
            }
        }



        /// <summary>
        /// Help-function that calculates the distance from startpoint to endpoint, needed for agro-range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double Distance(Point start, Point end)
        {
            int deltax = end.X - start.X;
            int deltay = end.Y - start.Y;
            double distance = MathF.Sqrt((MathF.Pow(deltax, 2) + MathF.Pow(deltay, 2)));
            return distance;
        }

        /// <summary>
        /// Method that implements to follow an other entity
        /// </summary>
        /// <param name="target"></param> Target which you wanna follow
        private void Follow(MovableEntity target)
        {
            if (mFreeze)
            {
                return;
            }

            mTargetedEntity = target;
            var destination = mTargetedEntity.mPosition;

            mMovetoField = destination;

            var saveDestination = false;
            if (mMovetoField.X > 0 && mMovetoField.Y > 0 && mMovetoField.X < mMap.MapSize.X && mMovetoField.Y < mMap.MapSize.Y)
            {
                if (mMap.IsSpaceIn(new Rectangle(mMovetoField, new Point(2, 2)), -1))
                {
                    saveDestination = true;
                }
            }

            if (!saveDestination)
            {
                mMovetoField = mMap.FindEmptySpace(mMovetoField);
            }
        }

        /// <summary>
        /// Method that implements fighting between entities
        /// </summary>
        /// <param name="target"></param> Target against to Fight
        /// <param name="gameTime">gameTime</param>
        public void Fight(MovableEntity target, GameTime gameTime)
        {
            if (target == null)
            {
                return;
            }
            
            SetTargetedEntity(target);

            if (!IsAlive || mFreeze)
            {
                mTargetedEntity = null;
                return;
            }
            if (!target.IsAlive)
            {
                mTargetedEntity = null;
                return;
            }

            if (DistanceToTarget(target) <= mAttackingRange)
            {
                if (mNextPositions.Count != 0 && mMovePath.Count != 0)
                {
                    mMovePath.Pop();
                    mMovetoField = mNextPositions.Dequeue().ToPoint();
                }
                Attack(target, gameTime);
            }
            else
            {
                if (target.IsAlive)
                {
                    Follow(target);
                }
            }
        }

        /// <summary>
        /// Set the mIsAlive Variable to false if entity is dead
        /// </summary>
        private void CheckDeath()
        {
            if (mCurrentHealthPoints <= 0)
            {
                IsAlive = false;
            }
        }


        /// <summary>
        /// Update the logic of the entity
        /// </summary>
        /// <param name="gameTime">Game time</param>
        internal override void Update(GameTime gameTime)
        {
            if (mNextPositions.Count > 0)
            {
                mNextPositions.Dequeue();
            }

            if (mOldPositionCount > 0 && mNextPositions.Count == 0)
            {
                mWalkedNextPositions = true;
            }

            mOldPositionCount = mNextPositions.Count;

            if (!IsAlive)
            {
                return;
            }

            if (!(mTargetedEntity is null))
            {
                if (!mTargetedEntity.IsAlive)
                {
                    mTargetedEntity = null;
                    mAttacking = false;
                }
                Fight(mTargetedEntity, gameTime);
            }

            if (this is Monster)
            {
                CheckDeath();
            }
        }


        /// <summary>
        /// Update the logic of the missiles
        /// </summary>
        /// <param name="map">our game map</param>
        public override void UpdateMissiles(Map map)
        {
            Missiles ??= new List<SkillItem>();

            var i = Missiles.Count - 1;
            for (; i >= 0; i--)
            {
                Missiles[i].Update(map);
                if (Missiles[i].HasArrived())
                {
                    Missiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Updates the healthBar of this entity
        /// </summary>
        public virtual void UpdateHealthBar()
        {
            Bar.Update(mCurrentHealthPoints, mMaxHealthPoints);
        }

        /// <summary>
        /// Updates the oxygenBar of this entity
        /// </summary>
        public virtual void UpdateOxygenBar()
        {}

        /// <summary>
        /// Simulates a snipe on this entity 
        /// </summary>
        /// <param name="team">snipe is only effective if the shooter and this entity are in different teams</param>
        public override void Sniped(int team)
        {
            if (mTeam != team)
            {
                // snipe damage:
                if (mCurrentHealthPoints > 0)
                {
                    const int damage = 40;
                    GetDamage(damage);
                    // Added for Statistic
                    if (team == OwnTeam)
                    {
                        StatisticState.mTotalDealtDamage.mValue += damage;
                        if (mCurrentHealthPoints <= 0)
                        {
                            if (this is Monster)
                            {
                                StatisticState.mTotalKilledMonsters.mValue += 1;
                                AchievementState.UpdateAchievement("Monster Hunter");
                                AchievementState.UpdateAchievement("Monster Killer");
                                AchievementState.UpdateAchievement("Monster Master");

                                SpawnNewMonster();
                            }

                            if (this is Hero)
                            {
                                StatisticState.mTotalKilledEnemyHeroes.mValue += 1;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Interpolates between two points by a certain factor, controls the speed of the entity as well
        /// </summary>
        /// <param name="currentPosition">start point</param>
        /// <param name="nextPosition">end point</param>
        public void InterpolateSteps(Point currentPosition, Point nextPosition)
        {
            if (mFreeze)
            {
                return;
            }
            var curVec = currentPosition.ToVector2();
            var nextVec = nextPosition.ToVector2();
            if (!curVec.Equals(nextVec))
            {
                // via the parameter "steps" the speed of a entity can be controlled later
                var steps = StepSpeed;
                // if entity moves diagonally, it has to do more steps because the distance is longer
                if (!(Math.Abs(curVec.X - nextVec.X) < 0.001 || Math.Abs(curVec.Y - nextVec.Y) < 0.001))
                {
                    steps = (int)Math.Sqrt(2 * steps * steps);
                }
                var direction = Vector2.Multiply((Vector2.Subtract(nextVec, curVec)), (float)(1.0 / steps));
                foreach (var scalar in Enumerable.Range(Globals.One, steps - Globals.One))
                {
                    mNextPositions.Enqueue(Vector2.Add(curVec, Vector2.Multiply(direction, scalar)));
                }
                // mNextPositions.Enqueue(nextVec);
            }
        }

        /// <summary>
        /// Prepares class for Serialization
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mForbiddenfieldsX = new List<int>();
            mForbiddenfieldsY = new List<int>();
            foreach (var point in mForbiddenfields)
            {
                mForbiddenfieldsX.Add(point.X);
                mForbiddenfieldsY.Add(point.Y);
            }
            
        }

        /// <summary>
        /// Checks out class after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mRandom = new Random();
            mForbiddenfields = new List<Point>();
            for (int i = 0; i < mForbiddenfieldsX.Count; i++)
            {
                mForbiddenfields.Add(new Point(mForbiddenfieldsX[i], mForbiddenfieldsY[i]));
            }
        }
    }
}
