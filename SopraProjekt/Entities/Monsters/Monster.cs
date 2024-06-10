using Microsoft.Xna.Framework;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities.Monsters
{
    /// <summary>
    /// Class to represent a Monster
    /// </summary>
    [Serializable]
    public abstract class Monster : MovableEntity
    {
        protected enum State
        {
            Idle,
            Follow,
            Attack
        }

        protected State mCurrentState;

        // list of all the created monsters
        private static readonly List<Monster> sAllMonsters = new List<Monster>();
        public Hero mTarget;
        public int mAgrorange;
        protected int mDamagePoints = -1;
        private readonly int mSpeed;

        [NonSerialized]
        private Random mRandom;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetName">name of the texture asset</param>
        /// <param name="position">initial position of the texture</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="stepSpeed">movements speed - lower equals faster</param>
        /// <param name="camera"></param>
        protected Monster(string assetName, Point position, Point textureSize, int stepSpeed, Camera camera) :
            base(assetName, "monster", position, textureSize, MovableEntity.NeutralTeam, Color.Crimson, stepSpeed, camera)
        {
            sAllMonsters.Add(this);
            mTarget = null;
            mCurrentState = State.Idle;
            mRandom = new Random();
            mSpeed = stepSpeed;
        }

        /// <summary>
        /// Implements a space holder for different skills of the heroes
        /// </summary>
        public abstract void Skill(GameTime gameTime);

        /// <summary>
        /// Implements a human detection that reacts if a human is in the agrorange of the monster
        /// </summary>
        public void DetectHuman()
        {
            var detected = Hero.sAllHeroes.Where(hero => Distance(hero.mPosition, mPosition) <= mAgrorange).ToList();

            if (detected.Count <= 0)
            {
                return;
            }

            mTarget = detected[mRandom.Next(detected.Count)];
            mCurrentState = State.Follow;
        }
        /// <summary>
        /// Attack a hero that is in attacking range
        /// </summary>
        public void Attack(GameTime gameTime)
        {
            if (mTarget == null || !mTarget.IsAlive)
            {
                mTarget = null;
                mCurrentState = State.Idle;
                return;
            }

            Skill(gameTime);
        }
        /// <summary>
        /// follow a hero to get in attacking range
        /// </summary>
        public void Follow()
        {
            if (mTarget == null || !mTarget.IsAlive)
            {
                mTarget = null;
                mCurrentState = State.Idle;
                return;
            }

            double distance = Distance(mPosition, mTarget.mPosition);

            // if hero not longer in agrorange set no target and go to idle state
            if (distance > mAgrorange)
            {
                mCurrentState = State.Idle;
                mTarget = null;
                return;
            }
            if (mMovePath.Count == 0 && mTarget != null)
            {
                // if distance to hero is in attackingrange attack and not move to him
                // move right to the hero but not to same field
                mMovetoField = mTarget.mPosition + new Point(1, 1);
            }

            // if distance is close enought to attack
            if (mTarget != null && Distance(mPosition, mTarget.mPosition) <= mAttackingRange)
            {
                // stop walking
                mMovePath = new Stack<Point>();
                mMovetoField = Point.Zero;

                // attack hero
                mCurrentState = State.Attack;
            }
        }
        /// <summary>
        /// update class switch for every state the monster has
        /// </summary>
        /// <param name="gameTime"></param>
        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (mCurrentState)
            {
                case State.Idle:
                    DetectHuman();
                    // Idle state: not doing anything
                    break;
                case State.Attack:
                    // attack routine
                    Attack(gameTime);
                    break;
                case State.Follow:
                    // follows some other entity
                    Follow();
                    break;
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
            StepSpeed = mSpeed;
        }
    }
}
