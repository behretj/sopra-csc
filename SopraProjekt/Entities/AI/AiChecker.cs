using Microsoft.Xna.Framework;
using SopraProjekt.Entities.Heroes;
using System.Collections.Generic;
using System.Linq;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SopraProjekt.Entities.Ai
{
    internal sealed class AiChecker
    {
        private const float HealthFactor = 0.5f;
        private const float OxygenFactor = 0.5f;

        private readonly Map mMap;
        private readonly AiRoutines mRoutines;

        internal AiChecker(Map map, AiRoutines routines)
        {
            mMap = map;
            mRoutines = routines;
        }

        internal void Run()
        {
            foreach (var enemy in mMap.mGameState.Enemies.mTeamMembers)
            {
                if (!enemy.IsAlive)
                {
                    continue;
                }

                CheckSkill(enemy);

                if (CheckOxygen(enemy))
                {
                    enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Oxygen);
                    enemy.AiState = AiHandler.State.Oxygen;
                    continue;
                }

                if (CheckHealth(enemy))
                {
                    enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Health);
                    enemy.AiState = AiHandler.State.Health;
                    continue;
                }

                if (CheckEscape(enemy))
                {
                    enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Escape);
                    enemy.AiState = AiHandler.State.Escape;
                    continue;
                }
                enemy.AiEscape = false;

                if (enemy.AiHasChanged || enemy.AiState != AiHandler.State.Default)
                {
                    continue;
                }

                CheckArea(enemy);
            }
        }

        /// <summary>
        /// Check the oxygen supply
        /// </summary>
        /// <param name="enemy">enemy to check</param> 
        /// <returns>true, if oxygen is needed</returns>
        private static bool CheckOxygen(MovableEntity enemy)
        {
            return enemy.mCurrentOxygenPoints < enemy.mMaxOxygenPoints * OxygenFactor;
        }

        /// <summary>
        /// Checks if escaping is necessary
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private bool CheckEscape(Entity enemy)
        {
            MovableEntity enemyMovable = (MovableEntity)enemy;
            // Dont escape if enough health
            if ((double)enemyMovable.mCurrentHealthPoints / enemyMovable.mMaxHealthPoints > 0.5)
            {
                return false;
            }
            // Try to escape if other team is stronger in close range
            const int size = 15;
            var entities = mMap.GetEntitiesIn(new Rectangle(new Point(enemy.mPosition.X - size, enemy.mPosition.Y - size), new Point(enemy.mPosition.X + size, enemy.mPosition.Y + size)));
            var allieRemainingHealth = 0;
            var enemyRemainingHealth = 0;
            var allieTotalDamage = 1;
            var enemyTotalDamage = 1;
            foreach (var entity in entities)
            {
                if (!(entity is MovableEntity movableEntity))
                {
                    continue;
                }

                if (movableEntity.mTeam == MovableEntity.EnemyTeam)
                {
                    allieRemainingHealth += movableEntity.mCurrentHealthPoints;
                    allieTotalDamage += movableEntity.mDamage;
                }
                else
                {
                    enemyRemainingHealth += movableEntity.mCurrentHealthPoints;
                    enemyTotalDamage += movableEntity.mDamage;
                }
            }

            // if own team dies first with normal fighting => escape
            return (double)allieRemainingHealth / enemyTotalDamage < (double)enemyRemainingHealth / allieTotalDamage;
        }

        /// <summary>
        /// Decides if a Hero should use his Skill
        /// </summary>
        /// <param name="enemy"></param>
        private void CheckSkill(Hero enemy)
        {
            // Proof if cool down is there
            if (enemy.mRemainingSkillCoolDownTime > 0)
            {
                return;
            }
            if (enemy is Carry carry)
            {
                CheckCarrySkill(carry);
            }
            if (enemy is Sniper sniper)
            {
                CheckSniperSkill(sniper);
            }
            if (enemy is Healer healer)
            {
                CheckHealerSkill(healer);
            }
            if (enemy is Tank tank)
            {
                CheckTankSkill(tank);
            }
            if (enemy is Crusher crusher)
            {
                CheckCrusherSkill(crusher);
            }
        }

        /// <summary>
        /// Checks if hero does his skill
        /// </summary>
        /// <param name="carry"></param>
        private void CheckCarrySkill(Hero carry)
        {
            // Activate Shield if Missile is incoming
            var activate = carry.Missiles.Count != 0 && !carry.mHasShield;
            // Activate if teammate is in fight
            foreach (var teammate in mMap.mGameState.Enemies.mTeamMembers)
            {
                if (teammate.AiState == AiHandler.State.Fight && !teammate.mHasShield && carry.GetDistanceTo(teammate.mPosition) <= carry.mSkillRange && !teammate.IsAlive)
                {
                    activate = true;
                }
            }

            if (activate)
            {
                carry.Skill(carry);
            }
        }

        /// <summary>
        /// Checks if hero does his skill
        /// </summary>
        /// <param name="sniper"></param>
        private void CheckSniperSkill(Hero sniper)
        {
            if (sniper.AiState == AiHandler.State.Fight)
            {
                sniper.Skill((MovableEntity)sniper.AiTarget);
            }
        }

        /// <summary>
        /// Checks if hero does his skill
        /// </summary>
        /// <param name="healer"></param>
        private void CheckHealerSkill(Healer healer)
        {
            const double lifePointLimit = 0.5;
            // Heal if teammate in range has low health
            bool activateSkill = false;
            List<Entity> entities = new List<Entity>();
            foreach (var teammate in mMap.mGameState.Enemies.mTeamMembers)
            {
                if (teammate == healer || !teammate.IsAlive)
                {
                    continue;
                }
                if (healer.GetDistanceTo(teammate.mPosition) <= healer.mSkillRange && (double)teammate.mCurrentHealthPoints / teammate.mMaxHealthPoints < lifePointLimit)
                {
                    entities.Add(teammate);
                    activateSkill = true;
                }
            }
            if (activateSkill)
            {
                healer.UseSkill(entities);
            }
        }

        /// <summary>
        /// Checks if hero does his skill
        /// </summary>
        /// <param name="tank"></param>
        private void CheckTankSkill(Tank tank)
        {
            const double oxygenPointLimit = 0.3;
            // If your teammate in range has low oxygen
            bool activateSkill = false;
            List<Entity> entities = new List<Entity>();
            foreach (var teammate in mMap.mGameState.Enemies.mTeamMembers)
            {
                if (teammate == tank || !teammate.IsAlive)
                {
                    continue;
                }
                if (tank.GetDistanceTo(teammate.mPosition) <= tank.mSkillRange && (float)teammate.mCurrentHealthPoints / teammate.mMaxHealthPoints < oxygenPointLimit)
                {
                    entities.Add(teammate);
                    activateSkill = true;
                }
            }

            if (activateSkill)
            {
                tank.UseSkill(entities);
            }
        }

        /// <summary>
        /// Checks if hero does his skill
        /// </summary>
        /// <param name="crusher"></param>
        private void CheckCrusherSkill(Crusher crusher)
        {
            bool activateSkill = false;
            int x1 = crusher.mPosition.X - crusher.mSkillRange;
            int x2 = crusher.mPosition.X + crusher.mSkillRange;
            int y1 = crusher.mPosition.Y - crusher.mSkillRange;
            int y2 = crusher.mPosition.Y + crusher.mSkillRange;
            List<Entity> possibleTargets = mMap.GetEntitiesIn(new Rectangle(new Point(x1, y1), new Point(x2, y2)));
            // Crush if enemy is in skillrange
            foreach (Entity entity in possibleTargets)
            {
                if (entity is MovableEntity movableEntity)
                {
                    if (movableEntity.mTeam != crusher.mTeam && crusher.GetDistanceTo(movableEntity.mPosition) <= crusher.mSkillRange)
                    {
                        activateSkill = true;
                        break;
                    }
                }
            }

            if (activateSkill)
            {
                crusher.Skill(possibleTargets);
            }
        }

        /// <summary>
        /// Check the health supply
        /// </summary>
        /// <param name="enemy">enemy to check</param> 
        /// <returns>true, if healing is needed</returns>
        private static bool CheckHealth(MovableEntity enemy)
        {
            return enemy.mCurrentHealthPoints < enemy.mMaxHealthPoints * HealthFactor;
        }

        private void CheckArea(Hero enemy)
        {
            var area = mMap.GetEntitiesIn(new Rectangle(
                enemy.mPosition.X - 5,
                enemy.mPosition.Y - 5,
                10,
                10));

            // 0: oxygen, 1: health, 2: brew stand, 3: npc_buyer, 4:herb

            const int oxygen = 0;
            const int health = 1;
            const int brewStand = 2;
            const int npcBuyer = 3;
            const int herb = 4;

            const int minType = 0;
            const int maxType = 4;

            bool[] found = { false, false, false, false, false };
            var possibleTargets = new Entity[5];

            // Get weakest enemy and attack it
            int healthpointsWeakestEntity = (from entity in area where entity.GetType().IsSubclassOf(typeof(MovableEntity)) where ((MovableEntity)entity).mTeam != MovableEntity.EnemyTeam select ((MovableEntity)entity).mCurrentHealthPoints).Prepend(10000).Min();

            foreach (var entity in area.Where(entity => entity.Id != enemy.Id && entity.Title != "bush" && entity.Title != "grass"))
            {
                if (entity.GetType().IsSubclassOf(typeof(MovableEntity)))
                {
                    if (((MovableEntity)entity).mTeam == MovableEntity.EnemyTeam)
                    {
                        AiRoutines.Communicate(enemy, entity as MovableEntity);
                    }
                    else
                    {
                        // Attacking weakest enemy
                        if (((MovableEntity)entity).mCurrentHealthPoints == healthpointsWeakestEntity)
                        {
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Fight);
                            enemy.AiState = AiHandler.State.Fight;
                            enemy.AiTarget = entity;
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                // Debug.WriteLine(entity.Title + " " + entity.Id);

                switch (entity.Title)
                {
                    case "oxygen_source" when enemy.mCurrentOxygenPoints <= 0.5 * enemy.mMaxOxygenPoints:
                        if (possibleTargets[0] == null)
                        {
                            possibleTargets[0] = entity;
                            break;
                        }

                        if (AiRoutines.CalculateDistance(enemy, possibleTargets[0].mPosition) < AiRoutines.CalculateDistance(enemy, entity.mPosition))
                        {
                            break;
                        }

                        possibleTargets[0] = entity;
                        found[oxygen] = true;
                        break;
                    case "health_fountain" when enemy.mCurrentHealthPoints <= 0.5 * enemy.mMaxHealthPoints:
                        if (possibleTargets[1] == null)
                        {
                            possibleTargets[1] = entity;
                            break;
                        }

                        if (AiRoutines.CalculateDistance(enemy, possibleTargets[1].mPosition) < AiRoutines.CalculateDistance(enemy, entity.mPosition))
                        {
                            break;
                        }

                        possibleTargets[1] = entity;
                        found[health] = true;
                        break;
                    case "brewing_stand":
                        if (possibleTargets[2] == null)
                        {
                            possibleTargets[2] = entity;
                            break;
                        }

                        if (AiRoutines.CalculateDistance(enemy, possibleTargets[2].mPosition) < AiRoutines.CalculateDistance(enemy, entity.mPosition))
                        {
                            break;
                        }

                        possibleTargets[2] = entity;
                        found[brewStand] = true;
                        break;
                    case "npc_buyer":
                        if (possibleTargets[3] == null)
                        {
                            possibleTargets[3] = entity;
                            break;
                        }

                        if (AiRoutines.CalculateDistance(enemy, possibleTargets[3].mPosition) < AiRoutines.CalculateDistance(enemy, entity.mPosition))
                        {
                            break;
                        }

                        possibleTargets[3] = entity;
                        found[npcBuyer] = true;
                        break;
                    case "herb_blau1":
                    case "herb_lila2":
                    case "herb_pink3":
                        if (mRoutines.mTargetedHerbs.Contains(entity))
                        {
                            break;
                        }
                        mRoutines.mTargetedHerbs.Add((Collectable)entity);
                        if (possibleTargets[herb] == null)
                        {
                            possibleTargets[herb] = entity;
                            break;
                        }

                        if (AiRoutines.CalculateDistance(enemy, possibleTargets[herb].mPosition) < AiRoutines.CalculateDistance(enemy, entity.mPosition))
                        {
                            break;
                        }
                        possibleTargets[herb] = entity;
                        found[herb] = true;
                        break;
                }
            }

            for (var i = minType; i <= maxType; i++)
            {
                if (found[i])
                {
                    switch (i)
                    {
                        case oxygen:
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Oxygen);
                            enemy.AiState = AiHandler.State.Oxygen;
                            enemy.AiTarget = possibleTargets[i];
                            return;
                        case health:
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Health);
                            enemy.AiState = AiHandler.State.Health;
                            enemy.AiTarget = possibleTargets[i];
                            return;
                        case brewStand:
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Brew);
                            enemy.AiState = AiHandler.State.Brew;
                            enemy.AiTarget = possibleTargets[i];
                            return;
                        case npcBuyer:
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Npc);
                            enemy.AiState = AiHandler.State.Npc;
                            enemy.AiTarget = possibleTargets[i];
                            return;
                        case herb:
                            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Collect);
                            enemy.AiState = AiHandler.State.Collect;
                            enemy.AiTarget = possibleTargets[i];
                            return;
                    }
                }
            }

            enemy.AiHasChanged = (enemy.AiState != AiHandler.State.Default);
            enemy.AiState = AiHandler.State.Default;
        }

    }
}
