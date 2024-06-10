using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Functional;
using SopraProjekt.GameState;
using SopraProjekt.Renderer;


#if verboseDebug
    using System.Diagnostics;
#endif

namespace SopraProjekt.Input
{
    /// <summary>
    /// Class to handel keyboard input
    /// </summary>
    internal sealed class KeyboardInput
    {
        private readonly Map mMap;
        private readonly GameState.GameState mGameState;
        // private readonly MouseInput mMouseInput;
        // private readonly Camera mCamera;

        private bool mKeySpaceReleased = true;
        private bool mKeyQReleased = true;
        private bool mKeyWReleased = true;
        private bool mKeyEReleased = true;

        // private bool mChooseTarget;
        // private bool mTargetEnemy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameState">game state</param>
        /// <param name="map">map</param>
        /// <param name="mouseInput">mouse input handler</param>
        /// <param name="camera">camera</param>
        internal KeyboardInput(GameState.GameState gameState, Map map, MouseInput mouseInput, Camera camera)
        {
            mGameState = gameState;
            mMap = map;
            _ = mouseInput;
            _ = camera;
        }

        /// <summary>
        /// Handel the keyboard inputs
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            var hero = mGameState.ActiveHero;

            if (state.IsKeyDown(Keys.Space) && mKeySpaceReleased)
            {
                ActionSpace(hero, gameTime);
            }

            if (state.IsKeyDown(Keys.A) && mKeyQReleased)
            {
                ActionA();
            }

            if (state.IsKeyDown(Keys.S) && mKeyQReleased)
            {
                ActionS();
            }

            if (state.IsKeyDown(Keys.D) && mKeyQReleased)
            {
                ActionD();
            }

            if (state.IsKeyDown(Keys.F) && mKeyQReleased)
            {
                ActionF();
            }

            if (state.IsKeyDown(Keys.G) && mKeyQReleased)
            {
                ActionG();
            }

            if (state.IsKeyDown(Keys.Q) && mKeyQReleased)
            {
                MouseInput.SpecialSkill(mGameState.ActiveHero, mMap);
                // ActionQ(hero);
            }

            if (state.IsKeyDown(Keys.W) && mKeyWReleased)
            {
                ActionW(hero);
            }

            if (state.IsKeyDown(Keys.E) && mKeyEReleased)
            {
                ActionE(hero);
            }

            /*
            if (mChooseTarget)
            {
                var target = SkillChooseTarget();
                if (target != null)
                {
                    hero.Skill(target);
                    mChooseTarget = false;
                    mMouseInput.mButtonMoveReleased = false;
                    mMouseInput.mActive = true;
                }
            }
            */

            mGameState.ActiveHero.Update(gameTime);
            DebounceButtons(state);
        }

        private void ChooseHero(int heroIndex)
        {
            var hero = mGameState.Heroes.mTeamMembers[heroIndex];
            if (!hero.IsAlive)
            {
                return;
            }

            mGameState.ActiveHero = hero;
        }

        /// <summary>
        /// Choose heroes on button press
        /// </summary>
        private void ActionA()
        {
            //Press A choose Carry
            //missing: hero must be alive
            ChooseHero(mGameState.mCarryHero);
        }
        private void ActionS()
        {
            //Press S choose Healer
            ChooseHero(mGameState.mHealerHero);
        }
        private void ActionD()
        {
            //Press D choose Sniper
            ChooseHero(mGameState.mSniperHero);
        }
        private void ActionF()
        {
            //Press F choose Crusher
            ChooseHero(mGameState.mCrusherHero);
        }
        private void ActionG()
        {
            //Press G choose Tank
            ChooseHero(mGameState.mTankHero);
        }


        /// <summary>
        /// Handel action if space is pressed
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="gameTime"></param>
        private void ActionSpace(Hero hero, GameTime gameTime)
        {
            mKeySpaceReleased = false;


            foreach (var entity in mMap.GetEntitiesIn(new Rectangle(hero.mPosition.X - 3, hero.mPosition.Y - 3, 6, 6)))
            {
                switch (entity.Title)
                {
                    case "herb_blau1":
                    case "herb_lila2":
                    case "herb_pink3":
                        if (mMap.mEntities.Remove(entity))
                        {
                            hero.AddPotionIngredients(entity.Title);
                        }
                        break;
                    case "health_fountain":
                        UseHealthFountain(entity, hero);
                        return;
                    case "oxygen_source":
                        UseOxygenSource(entity, hero);
                        return;
                    case "npc_buyer":
                        UseNonEnemyNpc(entity, hero, gameTime);
                        return;
                    case "brewing_stand":
                        UseBrewingStand(entity, hero, gameTime);
                        return;
                }
            }
        }


        /// <summary>
        /// Use health potion with W
        /// </summary>
        /// <param name="hero"></param>
        private void ActionW(Hero hero)
        {
            mKeyWReleased = false;
            bool valid = hero.UseHealthPotion();
            // Update Achievement/Statistic State
            if (mGameState.ActiveHero.mTeam == MovableEntity.OwnTeam && valid)
            {
                AchievementState.UpdateAchievement("Hoch Geheilt");
                AchievementState.UpdateAchievement("Neubelebung");
                AchievementState.UpdateAchievement("Unsterblich");
                StatisticState.mTotalUsedHealPotions.mValue += 1;
            }
        }

        /// <summary>
        /// Use oxygen potion with E
        /// </summary>
        /// <param name="hero"></param>
        private void ActionE(Hero hero)
        {
            mKeyEReleased = false;
            hero.UseOxygenPotion();
        }

        /// <summary>
        /// Function to debounce buttons
        /// </summary>
        /// <param name="state">keyboard state</param>
        private void DebounceButtons(KeyboardState state)
        {
            if (state.IsKeyUp(Keys.Space) && !mKeySpaceReleased)
            {
                mKeySpaceReleased = true;
            }

            if (state.IsKeyUp(Keys.Q) && !mKeyQReleased)
            {
                mKeyQReleased = true;
            }

            if (state.IsKeyUp(Keys.W) && !mKeyWReleased)
            {
                mKeyWReleased = true;
            }

            if (state.IsKeyUp(Keys.E) && !mKeyEReleased)
            {
                mKeyEReleased = true;
            }
        }


        /// <summary>
        /// Use the health fountain
        /// </summary>
        /// <param name="entity">health fountain</param>
        /// <param name="hero">hero to heal</param>
        private static void UseHealthFountain(Entity entity, MovableEntity hero)
        {
            ((HealthFountain)entity).HealMe(hero);
        }

        /// <summary>
        /// Use the oxygen source
        /// </summary>
        /// <param name="entity">oxygen source</param>
        /// <param name="hero">hero to supply</param>
        private static void UseOxygenSource(Entity entity, MovableEntity hero)
        {
            ((OxygenSource)entity).SupplyMe(hero);
        }

        /// <summary>
        /// Communicate with the non enemy npc
        /// </summary>
        private static void UseNonEnemyNpc(Entity entity, MovableEntity hero, GameTime gameTime)
        {
#if verboseDebug
            Debug.WriteLine("Use alien buyer at:\t" + entity.mPosition);
#endif
            ((Npc)entity).Interact(hero, gameTime);
        }

        /// <summary>
        /// Use the brew stand
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="hero"></param>
        /// <param name="gameTime"></param>
        private static void UseBrewingStand(Entity entity, Hero hero, GameTime gameTime)
        {
#if verboseDebug
            Debug.WriteLine("Use Brewing stand");
#endif
            ((BrewStand)entity).Use(hero, gameTime);
        }
    }
}