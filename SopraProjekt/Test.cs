using Microsoft.Xna.Framework;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Heroes;
using System.Diagnostics;
using System.Linq;

namespace SopraProjekt
{
    /// <summary>
    /// Class for test and presentation purposes
    /// </summary>
    internal static class Test
    {
        /// <summary>
        /// Tests the Attacking System
        /// </summary>
        public static void TestAttacking()
        {
            /*
            var friendlyHealer = new Healer(Point.Zero, 1);
            var enemyTank = new Tank(Point.Zero, -1);

            // Successful Attack
            Debug.Assert(enemyTank.mCurrentHealthPoints == enemyTank.mMaxHealthPoints);
            friendlyHealer.Fight(enemyTank);
            Debug.Assert(enemyTank.mCurrentHealthPoints == enemyTank.mMaxHealthPoints - friendlyHealer.mDamage);

            // Not Successful Attack (Enemy out of Range)
            var oldHealth = enemyTank.mCurrentHealthPoints;
            Debug.Assert(enemyTank.mCurrentHealthPoints == oldHealth);
            enemyTank.Position = new Point(100, 100);
            friendlyHealer.Fight(enemyTank);
            Debug.Assert(enemyTank.mCurrentHealthPoints == oldHealth);
            friendlyHealer.KillHero();
            enemyTank.KillHero();
            */
        }

        /// <summary>
        /// Tests the Skills of every Hero
        /// </summary>
        public static void TestSkills()
        {
            // Initializing every Hero for both team
            var friendlyHealer = new Healer(Point.Zero, 1, null);
            var enemyHealer = new Healer(Point.Zero, -1, null);
            var friendlyCarry = new Carry(Point.Zero, 1, null);
            var friendlyCrusher = new Crusher(Point.Zero, 1, null);
            var friendlySniper = new Sniper(Point.Zero, 1, null);
            var friendlyTank = new Tank(Point.Zero, 1, null);

            _ = friendlyTank;

            // Testing Carry Skill
            friendlyCarry.Skill(friendlyCarry);
            foreach (var hero in Hero.sAllHeroes.Where(hero => hero.IsInTeam(friendlyCarry)))
            {
                _ = hero;
                // Debug.Assert(hero.mHasShield);
            }

            // Testing Healer Skill
            var oldHealthPoints = friendlyCrusher.mCurrentHealthPoints;
            Debug.Assert(friendlyCrusher.mCurrentHealthPoints == oldHealthPoints);
            friendlyHealer.Skill(friendlyCrusher);
            Debug.Assert(friendlyCrusher.mCurrentHealthPoints == oldHealthPoints);

            friendlyCrusher.mCurrentHealthPoints -= 50;
            //Debug.WriteLine(friendlyCrusher.mCurrentHealthPoints);
            oldHealthPoints = friendlyCrusher.mCurrentHealthPoints;
            Debug.Assert(friendlyCrusher.mCurrentHealthPoints == oldHealthPoints);
            friendlyHealer.Skill(friendlyCrusher);
            Debug.Assert(friendlyCrusher.mHealing);
            Debug.Assert(friendlyCrusher.mCurrentHealthPoints == oldHealthPoints);


            // Testing Crusher Skill
            //oldHealthPoints = friendlyHealer.mCurrentHealthPoints;
            //Debug.Assert(friendlyHealer.mCurrentHealthPoints == oldHealthPoints);
            //friendlyCrusher.Skill(friendlyHealer);
            ////Debug.Assert(friendlyHealer.mCurrentHealthPoints == oldHealthPoints);

            //oldHealthPoints = enemyHealer.mCurrentHealthPoints;
            //Debug.Assert(enemyHealer.mCurrentHealthPoints == oldHealthPoints);
            //friendlyCrusher.Skill(enemyHealer);
            //Debug.Assert(enemyHealer.mCurrentHealthPoints == oldHealthPoints - Crusher.SkillDamage);

            // out-commented for changing the sniper skill 
            // Testing Sniper Skill
            //oldHealthPoints = friendlyHealer.mCurrentHealthPoints;
            //Debug.Assert(friendlyHealer.mCurrentHealthPoints == oldHealthPoints);
            //friendlySniper.Skill(friendlyHealer);
            //Debug.Assert(friendlyHealer.mCurrentHealthPoints == oldHealthPoints);

            //oldHealthPoints = enemyHealer.mCurrentHealthPoints;
            //Debug.Assert(enemyHealer.mCurrentHealthPoints == oldHealthPoints);
            friendlySniper.Skill(enemyHealer);
            //Debug.WriteLine(enemyHealer.Missiles.Count);
            //Debug.Assert(enemyHealer.mCurrentHealthPoints == oldHealthPoints - Sniper.SkillDamage);


            // Testing Tank Skill
            //friendlyHealer.mCurrentOxygenPoints -= 40;
            //enemyHealer.mCurrentOxygenPoints -= 40;

            //var oldOxygen = enemyHealer.mCurrentOxygenPoints;
            //Debug.Assert(enemyHealer.mCurrentOxygenPoints == oldOxygen);
            //friendlyTank.Skill(enemyHealer);
            //Debug.Assert(enemyHealer.mCurrentOxygenPoints == oldOxygen);


            //oldOxygen = friendlyHealer.mCurrentOxygenPoints;
            //Debug.Assert(friendlyHealer.mCurrentOxygenPoints == oldOxygen);
            //friendlyTank.Skill(friendlyHealer);
            //Debug.Assert(friendlyHealer.mCurrentOxygenPoints == oldOxygen + Tank.OxygenSkillValue);
        }

        /// <summary>
        /// Tests the fountains healing functionality
        /// </summary>
        public static void TestFountains()
        {
            /*
            // Testing the health fountain
            var testEntity = new Carry(new Point(5, 5), -1);
            var healthFountain = new HealthFountain(new Point(6, 5));

            const int baseHealthPoints = 100;

            testEntity.mCurrentHealthPoints = baseHealthPoints;

            // Tests that the fountain doesn't heal at any Update without sufficient time
            healthFountain.HealMe(testEntity);
            var gameTime = new GameTime();
            healthFountain.Update(gameTime);
            Debug.Assert(testEntity.mCurrentHealthPoints == baseHealthPoints);

            // Tests if the fountain does actually heal
            gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            healthFountain.Update(gameTime);
            Debug.Assert(testEntity.mCurrentHealthPoints == baseHealthPoints + HealthFountain.HealValue);

            // Tests if the fountain does stop healing if the entity is far away enough
            testEntity.Position = new Point(20, 20);
            gameTime = new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            Debug.Assert((testEntity.Position.ToVector2() - healthFountain.Position.ToVector2()).Length() > 10.0f);
            healthFountain.Update(gameTime);
            Debug.Assert(testEntity.mCurrentHealthPoints == baseHealthPoints + HealthFountain.HealValue);

            const int baseOxygenPoints = 100;

            // Testing the oxygen source
            testEntity.Position = new Point(6, 5);
            testEntity.mCurrentOxygenPoints = baseOxygenPoints;
            var oxygenSource = new OxygenSource(new Point(6, 5));

            // Tests that the fountain doesn't supply at any Update
            oxygenSource.SupplyMe(testEntity);
            gameTime = new GameTime();
            oxygenSource.Update(gameTime);
            Debug.Assert(testEntity.mCurrentOxygenPoints == baseOxygenPoints);

            // Tests if the supply does actually work
            gameTime = new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            oxygenSource.Update(gameTime);
            Debug.Assert(testEntity.mCurrentOxygenPoints == baseOxygenPoints + OxygenSource.SupplyValue);

            // Tests if the fountain does stop healing if the entity is far away enough
            testEntity.Position = new Point(20, 20);
            gameTime = new GameTime(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            Debug.Assert((testEntity.Position.ToVector2() - oxygenSource.Position.ToVector2()).Length() > 10.0f);
            oxygenSource.Update(gameTime);
            Debug.Assert(testEntity.mCurrentOxygenPoints == baseOxygenPoints + OxygenSource.SupplyValue);
            */
        }

        public static void TestQuadTree()
        {
            /*
            QuadTree tree = new QuadTree(new Rectangle(new Point(0, 0), new Point(250, 250)), 3);
            List<Entity> entList = new List<Entity>()
            {
                new Healer(new Point(100, 20), 1),
                new Healer(new Point(60, 20), 1),
                new Healer(new Point(20, 200), 1),
                new Healer(new Point(30, 20), 1),
                new Healer(new Point(230, 202), 1)
            };

            foreach (var entity in entList)
            {
                tree.Insert(entity);
            }

            foreach (var item in tree.GetElements(new Rectangle(new Point(0, 0), new Point(250, 250))))
            {
                Debug.WriteLine(item.Title);
            }

            Debug.WriteLine("change");

            
            tree.Remove(entList[1]);
            entList[1].SetPosition(new Point(1, 1), tree);
            tree.Insert(entList[1]);

            foreach (var item in tree.GetElements(new Rectangle(new Point(0, 0), new Point(250, 250))))
            {
                Debug.WriteLine(item.Title);
            }

            Debug.WriteLine("Find");

            Debug.WriteLine(tree.GetElements(new Rectangle(entList[1].Position, new Point(250, 250))));
            */

            // var x = new Rectangle(0, 0, 10, 10);
            //Debug.WriteLine(Contains(x, new Point(0, 5)));
            //Debug.WriteLine(Contains(x, new Point(5, 0)));
            //Debug.WriteLine(Contains(x, new Point(10, 5)));
            //Debug.WriteLine(Contains(x, new Point(5, 10)));
        }

        /*
         * resharper error
        private static bool Contains(Rectangle rectangle, Point point)
        {
            return (point.X >= rectangle.X &&
                    point.X < rectangle.X + rectangle.Width &&
                    point.Y >= rectangle.Y &&
                    point.Y < rectangle.Y + rectangle.Height);
        }
        */

    }
}
