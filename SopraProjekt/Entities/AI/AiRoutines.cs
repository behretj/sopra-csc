using Microsoft.Xna.Framework;
using SopraProjekt.Entities.AI;
using SopraProjekt.Entities.Functional;
using System;
using System.Collections;
using System.Collections.Generic;

// TODO: write function RunAirshipRoutine

namespace SopraProjekt.Entities.Ai
{
    public sealed class AiRoutines
    {
        private const double AlarmDistance = 30;

        private readonly Map mMap;

        private readonly List<Point> mAvailableHealthSources;
        private readonly List<Point> mAvailableOxygenSources;

        internal readonly List<Collectable> mTargetedHerbs;

        private readonly Tremaux mTremaux;

        // Variables to store common locations in
        public MovableEntity mAlarmEntity;
        public Hashtable mOldDest; // The key is the Hash Code of the entity

        // Handles the last state change to prevent infinite loops
        public Dictionary<Entity, KeyValuePair<int, Point>> mLastPosition;

        internal AiRoutines(Map map)
        {
            mMap = map;
            mOldDest = new Hashtable();
            mLastPosition = new Dictionary<Entity, KeyValuePair<int, Point>>();

            foreach (var enemy in mMap.mGameState.Enemies.mTeamMembers)
            {
                enemy.mMovetoField = Point.Zero;
                mLastPosition[enemy] = new KeyValuePair<int, Point>(0, new Point());
            }

            mAvailableHealthSources = new List<Point>();
            mAvailableOxygenSources = new List<Point>();

            for (var y = 0; y < Map.MapSizeMaxX - 1; y++)
            {
                for (var x = 0; x < Map.MapSizeMaxY - 1; x++)
                {
                    switch (MapTextFormat.sMapText[y, x])
                    {
                        // Oxygen
                        case 2:
                            mAvailableOxygenSources.Add(new Point(x, y));
                            break;
                        // Health
                        case 3:
                            mAvailableHealthSources.Add(new Point(x, y));
                            break;
                    }
                }
            }
            mTargetedHerbs = new List<Collectable>();
            mTremaux = new Tremaux();
        }


        internal void Run(GameTime gameTime)
        {
            foreach (var enemy in mMap.mGameState.Enemies.mTeamMembers)
            {
                // Debug.WriteLine("Entity: " + enemy.Title + " has AI state: " + enemy.AiState);
                if (!enemy.IsAlive)
                {
                    continue;
                }

                var (lastUpdate, lastPosition) = mLastPosition[enemy];
                if (enemy.mPosition == lastPosition && enemy.mMovetoField != Point.Zero)
                {
                    if (gameTime.TotalGameTime.Seconds - lastUpdate > 5)
                    {
                        enemy.mMovePath.Clear();
                        enemy.mMovetoField = enemy.AiCurrentWayPoint.mPosition;
                    }
                }
                else
                {
                    mLastPosition[enemy] = new KeyValuePair<int, Point>(gameTime.TotalGameTime.Seconds, enemy.mPosition);
                }

                // If there is a change in the state store or load the last destination
                switch (enemy.AiState)
                {
                    case AiHandler.State.Oxygen:
                    case AiHandler.State.Health:
                    case AiHandler.State.Fight:
                    case AiHandler.State.Brew:
                    case AiHandler.State.Npc:
                    case AiHandler.State.Alarmed:
                    case AiHandler.State.Collect:
                    case AiHandler.State.Escape:
                        if (mOldDest.ContainsKey(enemy) || enemy.mMovetoField == Point.Zero)
                        {
                            break;
                        }
                        mOldDest[enemy] = enemy.mMovetoField;
                        break;
                    case AiHandler.State.Default:
                        if (mOldDest.ContainsKey(enemy) && mOldDest[enemy] != null && ((Point)mOldDest[enemy] != Point.Zero))
                        {
                            enemy.mMovetoField = (Point)mOldDest[enemy];
                            mOldDest.Remove(enemy);
                            ChangeAiState(enemy, AiHandler.State.Returning);
                        }
                        break;
                    case AiHandler.State.Returning:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                switch (enemy.AiState)
                {
                    case AiHandler.State.Default:
                        RunDefaultRoutine(enemy);
                        break;
                    case AiHandler.State.Oxygen:
                        RunOxygenRoutine(enemy);
                        break;
                    case AiHandler.State.Health:
                        RunHealingRoutine(enemy);
                        break;
                    case AiHandler.State.Fight:
                        RunFightRoutine(enemy, gameTime);
                        break;
                    case AiHandler.State.Brew:
                        RunBrewRoutine(enemy, gameTime);
                        break;
                    case AiHandler.State.Npc:
                        RunNpcRoutine(enemy, gameTime);
                        break;
                    case AiHandler.State.Alarmed:
                        RunAlarmedRoutine(enemy, gameTime);
                        break;
                    case AiHandler.State.Collect:
                        RunCollectRoutine(enemy);
                        break;
                    case AiHandler.State.Escape:
                        RunEscapeRoutine(enemy);
                        break;
                    case AiHandler.State.Returning:
                        // Reached destination goto default routine
                        if (enemy.mMovetoField == enemy.mPosition || enemy.mMovetoField == Point.Zero)
                        {
                            ChangeAiState(enemy, AiHandler.State.Default);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void RunDefaultRoutine(Entity enemy)
        {
            var hero = (Hero)enemy;

            if (hero.AiHasChanged)
            {
                hero.mMovetoField = Point.Zero;
                enemy.mNextPositions.Clear();
                enemy.mMovePath.Clear();
                hero.AiHasChanged = false;
            }


            if (enemy.mMovePath.Count != 0 || hero.mMovetoField != Point.Zero)
            {
                return;
            }

            mTremaux.FindNextPoint(hero);

            enemy.mNextPositions.Clear();
            enemy.mMovePath.Clear();
            var move = new Point(0, 0);

            enemy.mMovetoField = hero.AiCurrentWayPoint.mPosition + move;
        }

        private void RunOxygenRoutine(Hero enemy)
        {
            if (enemy.mCurrentOxygenPoints <= 0.25 * enemy.mMaxOxygenPoints && enemy.mNumberOxygenPotions > 0)
            {
                enemy.UseOxygenPotion();
            }
            
            // check if target is already set and otherwise set target
            if (!(enemy.AiTarget is OxygenSource))
            {
                OxygenSource nearest = null;
                var dist = double.PositiveInfinity;
                foreach (var pos in mAvailableOxygenSources)
                {
                    var sources = mMap.GetEntitiesIn(new Rectangle(pos - new Point(1), new Point(3)));

                    if (sources.Count == 0 || !(sources[0] is OxygenSource))
                    {
                        continue;
                    }

                    var source = sources[0];

                    var d = CalculateDistance(enemy, source.mPosition);
                    if (d >= dist)
                    {
                        continue;
                    }

                    nearest = (OxygenSource)source;
                    dist = d;
                }

                enemy.AiTarget = nearest;
            }

            // move to target or use oxygen source
            if (enemy.AiTarget != null && CalculateDistance(enemy, enemy.AiTarget.mPosition) <= 4)
            {
                ((OxygenSource)enemy.AiTarget).SupplyMe(enemy);

                if (enemy.mCurrentOxygenPoints >= 0.95 * enemy.mMaxOxygenPoints)
                {
                    enemy.AiTarget = null;
                    ChangeAiState(enemy, AiHandler.State.Default);
                    return;
                }
            }

            if (enemy.AiTarget != null && enemy.AiTarget.mPosition != enemy.mMovetoField)
            {
                enemy.mMovetoField = enemy.AiTarget.mPosition;
            }
        }

        /// <summary>
        /// Run if the enemy need healing
        /// </summary>
        /// <param name="enemy">enemy to control</param>
        private void RunHealingRoutine(Hero enemy)
        {
            if (enemy.mCurrentHealthPoints <= 0.25 * enemy.mMaxHealthPoints && enemy.mNumberHealthPotions > 0)
            {
                enemy.UseHealthPotion();
            }

            // check if target is already set and otherwise set target
            if (!(enemy.AiTarget is HealthFountain))
            {
                enemy.AiTarget = GetNearestHealthFountain(enemy);
            }

            // move to target or use oxygen source
            if (enemy.AiTarget != null && CalculateDistance(enemy, enemy.AiTarget.mPosition) <= 4)
            {
                ((HealthFountain)enemy.AiTarget).HealMe(enemy);

                if (enemy.mCurrentHealthPoints >= 0.95 * enemy.mMaxHealthPoints)
                {
                    enemy.AiTarget = null;
                    ChangeAiState(enemy, AiHandler.State.Default);
                    return;
                }
            }

            if (enemy.AiTarget != null && enemy.AiTarget.mPosition != enemy.mMovetoField)
            {
                enemy.mMovetoField = enemy.AiTarget.mPosition;
            }
        }

        /// <summary>
        /// Returns nearest Health Fountain
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private HealthFountain GetNearestHealthFountain(Entity enemy)
        {
            HealthFountain nearest = null;
            var dist = double.PositiveInfinity;

            foreach (var pos in mAvailableHealthSources)
            {
                var sources = mMap.GetEntitiesIn(new Rectangle(pos - new Point(1), new Point(3)));

                if (sources.Count == 0 || !(sources[0] is HealthFountain))
                {
                    continue;
                }

                var source = sources[0];

                var d = CalculateDistance(enemy, source.mPosition);
                if (d >= dist)
                {
                    continue;
                }

                nearest = (HealthFountain)source;
                dist = d;
            }

            return nearest;
        }

        private void RunAlarmedRoutine(Hero enemy, GameTime gameTime)
        {
            // If there is no alarmed entity return to normal behaviour
            if (mAlarmEntity == null)
            {
                enemy.AiTarget = null;
                ChangeAiState(enemy, AiHandler.State.Default);
                return;
            }

            // Do fight while entity alive
            if (mAlarmEntity.IsAlive || mAlarmEntity.mCurrentHealthPoints > 0)
            {
                enemy.Fight(mAlarmEntity, gameTime);
                return;
            }
            // Else goto Default and reset
            mAlarmEntity = null;
            enemy.mTargetedEntity = null;
            ChangeAiState(enemy, AiHandler.State.Returning);
        }

        /// <summary>
        /// routine to run if herbs should be collected
        /// </summary>
        private void RunCollectRoutine(Hero enemy)
        {
            if (enemy.AiTarget is Collectable && enemy.AiHasChanged)
            {
                enemy.mAiDestination = enemy.AiTarget.mPosition;
                enemy.mMovetoField = enemy.mAiDestination;
            }

            var target = (Collectable)enemy.AiTarget;
            if (!enemy.CollectHerb(target) && mTargetedHerbs.Contains(target) && target.mPosition != enemy.mPosition && Collectable.AllCollectables.Contains(target))
            {
                return;
            }

            mMap.mEntities.Remove(target);
            mTargetedHerbs.Remove(target);
            enemy.AddPotionIngredients(target.Title);
            enemy.AiTarget = null;
            ChangeAiState(enemy, AiHandler.State.Default);
        }

        /// <summary>
        /// routine to run if a potion should be brewed
        /// </summary>
        /// <param name="enemy">enemy to control</param>
        /// <param name="gameTime">game time</param>
        private void RunBrewRoutine(Hero enemy, GameTime gameTime)
        {
            if (enemy.AiTarget is BrewStand && enemy.AiHasChanged && enemy.AiTarget.mPosition != enemy.mPosition)
            {
                enemy.mAiDestination = enemy.AiTarget.mPosition;
                enemy.mMovetoField = enemy.mAiDestination;
                enemy.AiHasChanged = false;
            }

            var brewStand = (BrewStand)enemy.AiTarget;
            if (enemy.AiTarget == null || !(CalculateDistance(enemy, enemy.AiTarget.mPosition) <= 10) && enemy.AiTarget.mPosition != enemy.mPosition)
            {
                return;
            }

            brewStand.Use(enemy, gameTime);
            enemy.AiTarget = null;
            ChangeAiState(enemy, AiHandler.State.Returning);
        }

        /// <summary>
        /// If two enemies meet, they interchange 
        /// there known health fountains etc.
        /// </summary>
        /// <param name="enemy1"></param>
        /// <param name="enemy2"></param>
        internal static void Communicate(MovableEntity enemy1, MovableEntity enemy2)
        {
            var spaceship = enemy1.mKnownSpaceShip ?? enemy2.mKnownSpaceShip;
            enemy1.mKnownSpaceShip = spaceship;
            enemy2.mKnownSpaceShip = spaceship;
        }

        /// <summary>
        /// Ai Fighting implementation
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="gameTime"></param>
        private void RunFightRoutine(Hero enemy, GameTime gameTime)
        {
            // Alarms other team members
            AlarmOthers(enemy);
            enemy.Fight((MovableEntity)enemy.AiTarget, gameTime);

            // End fight if enemy is dead
            if (((MovableEntity)enemy.AiTarget).IsAlive && ((MovableEntity)enemy.AiTarget).mCurrentHealthPoints > 0)
            {
                return;
            }

            enemy.AiTarget = null;
            mAlarmEntity = null;
            ChangeAiState(enemy, AiHandler.State.Default);
        }

        /// <summary>
        /// Ai Escape Plan implementation
        /// </summary>
        private void RunEscapeRoutine(Hero enemy)
        {
            bool escapeToTeammate = false;
            float distanceTo = 50;
            Entity escapeDestination = null;
            // If possible Escape to an non fighting teammate
            foreach (var teammate in mMap.mGameState.Enemies.mTeamMembers)
            {
                if (teammate.AiState != AiHandler.State.Fight && teammate.AiState != AiHandler.State.Escape && !teammate.IsAlive)
                {
                    // Choose closest one
                    if (enemy.GetDistanceTo(teammate.mPosition) < distanceTo)
                    {
                        escapeToTeammate = true;
                        escapeDestination = teammate;
                        distanceTo = enemy.GetDistanceTo(teammate.mPosition);
                    }
                }
            }
            if (escapeToTeammate)
            {
                enemy.mMovetoField = ((MovableEntity)escapeDestination).mMovetoField == new Point(0, 0) ? ((MovableEntity)escapeDestination).mPosition : ((MovableEntity)escapeDestination).mMovetoField;
            }
            else
            {
                RunHealingRoutine(enemy);
            }
            enemy.AiEscape = true;
        }

        private void AlarmOthers(Hero caller)
        {
            const double healthLimit = 0.4;
            const double oxygenLimit = 0.25;

            if (mAlarmEntity != null)
            {
                return;
            }

            mAlarmEntity = (MovableEntity)caller.AiTarget;

            foreach (var enemy in mMap.mGameState.Enemies.mTeamMembers)
            {
                // Higher Priority then helping (surviving themselves, ...)
                if (
                    enemy.mCurrentHealthPoints < healthLimit * enemy.mMaxHealthPoints ||  // Health below limit
                    enemy.mCurrentOxygenPoints < oxygenLimit * enemy.mMaxOxygenPoints ||  // Oxygen below limit
                    (
                        enemy.AiState == AiHandler.State.Fight &&
                        enemy.mTargetedEntity?.mCurrentHealthPoints < healthLimit * enemy.mTargetedEntity?.mMaxHealthPoints
                    ) ||  // If the team member is fighting and nearly has it
                    enemy == caller || // Do not modify yourself
                    CalculateDistance(caller, enemy.mPosition) > AlarmDistance  // Too far away
                )
                {
                    continue;
                }

                enemy.AiHasChanged = enemy.AiState != AiHandler.State.Alarmed;
                enemy.AiState = AiHandler.State.Alarmed;
            }
        }

        /// <summary>
        /// routine to run if the enemy should interact with a non enemy npc
        /// </summary>
        /// <param name="enemy">enemy to control</param>
        /// <param name="gameTime">game time</param>
        private void RunNpcRoutine(Hero enemy, GameTime gameTime)
        {
            if (enemy.AiTarget is Npc && enemy.AiHasChanged)
            {
                enemy.mAiDestination = enemy.AiTarget.mPosition;
                enemy.mMovetoField = enemy.mAiDestination;
            }

            var npc = (Npc)enemy.AiTarget;
            if (enemy.AiTarget == null || !(CalculateDistance(enemy, enemy.AiTarget.mPosition) <= 10))
            {
                return;
            }

            npc.Interact(enemy, gameTime);
            if (!enemy.mForbiddenNpc.ContainsKey(npc))
            {
                enemy.mForbiddenNpc.Add(npc, gameTime);
            }
            ChangeAiState(enemy, AiHandler.State.Default);
            enemy.AiTarget = null;
        }

        /// <summary>
        /// Calculate the distance between the enemy and the position
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static double CalculateDistance(Entity entity, Point position)
        {
            return Math.Sqrt(Math.Pow(entity.mPosition.X - position.X, 2) +
                             Math.Pow(entity.mPosition.Y - position.Y, 2));
        }

        private void ChangeAiState(Hero enemy, AiHandler.State state)
        {
            enemy.AiHasChanged = true;
            enemy.AiState = state;
        }
    }
}
