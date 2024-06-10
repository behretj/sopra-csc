using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.GameState;
using SopraProjekt.Helper;
using SopraProjekt.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    internal sealed class BottomHud : IMenuObject
    {
        private const double HudWidth = 0.5;
        private const double HudHeight = 0.1;
        private const double HudHorizontalPos = 0.25;
        private const double HudVerticalPos = 0.9;
        private const double SmallElementHeight = 0.04;
        private const double SmallElementSpace = 0.00666666666666666666666666666667;

        private bool mDisposedValue;
        private readonly List<IMenuObject> mElements;
        private readonly Sprite mBackgroundSprite;
        private readonly Point mBackgroundDimensions;

        /*
            25 %       |            50 % HUD               |       25 %
                       |   25 %  Bars |   Potion, Ability  |
        */

        /// <summary>
        /// Constructor
        /// </summary>
        internal BottomHud(ContentLoader content, GameState.GameState gameState, Map map, GraphicsDeviceManager graphics)
        {
            mElements = new List<IMenuObject>();
            var screenDimensions =
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            // Background
            mBackgroundSprite = new Sprite()
            {
                mColor = Color.DarkTurquoise,
                mPosition = new Vector2(
                    (float)(screenDimensions.Width * HudHorizontalPos),
                    (float)(screenDimensions.Height * HudVerticalPos)
                ),

                mTexture = content.LoadTexture("Design/hud_background2")

            };
            mBackgroundDimensions = new Point(
                (int)(screenDimensions.Width * HudWidth),
                (int)(screenDimensions.Height * HudHeight)
            );

            // Selected hero health bar
            // Selected hero oxygen bar
            mElements.Add(new SelectedHeroBars(screenDimensions, content, gameState));
            // Potion
            mElements.Add(new PotionInterface(screenDimensions, content, gameState));
            // Selected hero ability
            mElements.Add(new HeroAbilityButton(screenDimensions, gameState, map));
            // The hero image
            mElements.Add(new HeroImage(screenDimensions, gameState));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mBackgroundSprite.Draw(gameTime, spriteBatch, mBackgroundDimensions);
            foreach (var element in mElements)
            {
                element.Draw(gameTime, spriteBatch);
            }
        }

        public void HandleInput()
        {
            foreach (var element in mElements)
            {
                element.HandleInput();
            }
        }

        private void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                mElements.Clear();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            mDisposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
        }

        // The individual elements

        private sealed class SelectedHeroBars : IMenuObject
        {
            private bool mDisposedValue;

            private const double ElementWidth = 0.25 - BottomHud.SmallElementHeight - BottomHud.SmallElementSpace - BottomHud.SmallElementSpace;

            private const string HealthTextureName = "Design/Red";
            private const string OxygenTextureName = "Design/Blue";
            private const string BarBackgroundTextureName = "Design/white";

            private readonly int mBarWidth;
            private readonly int mBarHeight;

            // Health Bar
            private readonly Point mHealthBarPos;

            private readonly Texture2D mHealthTexture;

            // Oxygen Bar
            private readonly Point mOxygenBarPos;

            private readonly Texture2D mOxygenTexture;

            private readonly Texture2D mBarBackgroundTexture;

            private readonly GameState.GameState mGameState;

            internal SelectedHeroBars(Rectangle screenDimensions, ContentLoader content, GameState.GameState gameState)
            {
                mGameState = gameState;
                mHealthTexture = content.LoadTexture(HealthTextureName);
                mOxygenTexture = content.LoadTexture(OxygenTextureName);
                mBarBackgroundTexture = content.LoadTexture(BarBackgroundTextureName);

                mBarWidth = (int)(screenDimensions.Width * ElementWidth - 2 * (screenDimensions.Width * SmallElementSpace));
                mBarHeight = (int)(screenDimensions.Height * SmallElementHeight);

                var pos = new Point
                (
                    (int)(screenDimensions.Width * (BottomHud.HudHorizontalPos + BottomHud.SmallElementSpace + BottomHud.SmallElementHeight + BottomHud.SmallElementSpace)),
                    (int)(screenDimensions.Height * BottomHud.HudVerticalPos)
                );

                mHealthBarPos = new Point(
                    pos.X + (int)(screenDimensions.Width * SmallElementSpace),
                    pos.Y + (int)(screenDimensions.Height * SmallElementSpace)
                );

                mOxygenBarPos = new Point(
                    pos.X + (int)(screenDimensions.Width * SmallElementSpace),
                    pos.Y + (int)(screenDimensions.Height * SmallElementSpace + mBarHeight + screenDimensions.Height * SmallElementSpace)
                );
            }

            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                var hero = (MovableEntity)mGameState.ActiveHero;

                var healthRectangle = new Rectangle(
                    mHealthBarPos,
                    new Point((int)(mBarWidth * (hero.mCurrentHealthPoints / (double)hero.mMaxHealthPoints)), mBarHeight)
                );

                var oxygenRectangle = new Rectangle(
                    mOxygenBarPos,
                    new Point((int)(mBarWidth * (hero.mCurrentOxygenPoints / (double)hero.mMaxOxygenPoints)), mBarHeight)
                );

                spriteBatch.Draw(mBarBackgroundTexture, new Rectangle(mHealthBarPos, new Point(mBarWidth, mBarHeight)), Color.White);
                spriteBatch.Draw(mBarBackgroundTexture, new Rectangle(mOxygenBarPos, new Point(mBarWidth, mBarHeight)), Color.White);

                spriteBatch.Draw(mHealthTexture, healthRectangle, Color.White);
                spriteBatch.Draw(mOxygenTexture, oxygenRectangle, Color.White);
            }

            public void HandleInput()
            {
                // Nothing to handle here
            }

            private void Dispose(bool disposing)
            {
                if (mDisposedValue)
                {
                    return;
                }

                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                mDisposedValue = true;
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
            }
        }

        private sealed class PotionInterface : IMenuObject
        {
            private const double ElementPos = 0.5;
            private const double HerbPos = ElementPos + SmallElementHeight + SmallElementSpace + SmallElementHeight;
            private const double HerbSpace = SmallElementSpace / 2;
            private const double HerbHeight = (SmallElementHeight + HerbSpace + SmallElementHeight - HerbSpace - HerbSpace) / 3;

            private const string HealthPotionName = "Images/Menu/Heal_Potion";
            private const string OxygenPotionName = "Images/Menu/Oxygen_Potion";

            private const string BlueHerbName = "Images/Entities/HerbBlau";
            private const string PurpleHerbName = "Images/Entities/HerbLila";
            private const string PinkHerbName = "Images/Entities/HerbPink";

            private readonly Point mPotionSize;

            private readonly Sprite mHealthPotion;
            private readonly Text mHealthPotionCount;

            private readonly Sprite mOxygenPotion;
            private readonly Text mOxygenPotionCount;

            private readonly GameState.GameState mGameState;

            private readonly Sprite mBlueHerbSprite;
            private readonly Text mBlueHerbCount;

            private readonly Sprite mPurpleHerbSprite;
            private readonly Text mPurpleHerbCount;

            private readonly Sprite mPinkHerbSprite;
            private readonly Text mPinkHerbCount;

            private readonly Point mHerbSpriteSize;

            private bool mDisposedValue;
            private bool mDeBounce;

            internal PotionInterface(Rectangle screenDimensions, ContentLoader content, GameState.GameState gameState)
            {
                mGameState = gameState;

                var healthPotionPos = new Vector2(
                    (float)(screenDimensions.Width * (ElementPos + SmallElementSpace)),
                    (float)(screenDimensions.Height * (HudVerticalPos + SmallElementSpace))
                );

                var oxygenPotionPos = new Vector2(
                    (float)(screenDimensions.Width * ElementPos + screenDimensions.Width * SmallElementSpace),
                    (float)(screenDimensions.Height * HudVerticalPos + screenDimensions.Height * SmallElementSpace +
                             screenDimensions.Height * SmallElementHeight + screenDimensions.Height * SmallElementSpace)
                );

                mPotionSize = new Point((int)(screenDimensions.Height * SmallElementHeight));

                // Health Potion Icon
                mHealthPotion = new Sprite
                {
                    mColor = Color.White,
                    mPosition = healthPotionPos,
                    mTexture = content.LoadTexture(HealthPotionName)
                };

                mHealthPotionCount = new Text
                {
                    Color = Color.White,
                    Message = "",
                    SpriteFont = content.LoadSpriteFont("Design/Button"),
                    Position = healthPotionPos + new Vector2(
                        (float)(screenDimensions.Width * (SmallElementSpace + SmallElementHeight)),
                        (float)(-screenDimensions.Width * SmallElementSpace))
                };

                // Oxygen Potion Icon
                mOxygenPotion = new Sprite
                {
                    mColor = Color.White,
                    mPosition = oxygenPotionPos,
                    mTexture = content.LoadTexture(OxygenPotionName)
                };

                mOxygenPotionCount = new Text
                {
                    Color = Color.White,
                    Message = "",
                    SpriteFont = content.LoadSpriteFont("Design/Button"),
                    Position = oxygenPotionPos + new Vector2(
                        (float)(screenDimensions.Width * (SmallElementSpace + SmallElementHeight)),
                        (float)(-screenDimensions.Width * SmallElementSpace))
                };

                // Herb Position
                var herbPosition = new Vector2(
                    (float)(screenDimensions.Width * HerbPos),
                    (float)(screenDimensions.Height * (HudVerticalPos + SmallElementSpace))
                );

                mBlueHerbSprite = new Sprite
                {
                    mPosition = herbPosition,
                    mColor = Color.White,
                    mTexture = content.LoadTexture(BlueHerbName)
                };

                mBlueHerbCount = new Text
                {
                    Position = herbPosition + new Vector2((float)((HerbHeight + HerbSpace) * screenDimensions.Width), (float)((-HerbHeight / 2) * screenDimensions.Height)),
                    Color = Color.White,
                    Message = "",
                    SpriteFont = content.LoadSpriteFont("Design/Button"),
                };

                mPurpleHerbSprite = new Sprite
                {
                    mPosition = herbPosition + new Vector2(0, (float)(screenDimensions.Height * (HerbHeight + HerbSpace))),
                    mColor = Color.White,
                    mTexture = content.LoadTexture(PurpleHerbName)
                };

                mPurpleHerbCount = new Text
                {
                    Position = herbPosition + new Vector2(
                        (float)(screenDimensions.Width * (HerbHeight + HerbSpace)),
                        (float)(screenDimensions.Height * (HerbHeight + HerbSpace - (HerbHeight / 2)))
                    ),
                    Color = Color.White,
                    Message = "",
                    SpriteFont = content.LoadSpriteFont("Design/Button"),
                };

                mPinkHerbSprite = new Sprite
                {
                    mPosition = herbPosition +
                                new Vector2(0, (float)(screenDimensions.Height * (HerbHeight + HerbSpace + HerbHeight + HerbSpace))),
                    mColor = Color.White,
                    mTexture = content.LoadTexture(PinkHerbName)
                };

                mPinkHerbCount = new Text
                {
                    Position = herbPosition + new Vector2(
                        (float)((HerbHeight + HerbSpace) * screenDimensions.Width),
                        (float)(screenDimensions.Height * (HerbHeight + HerbSpace + HerbHeight + HerbSpace - (HerbHeight / 2)))
                    ),
                    Color = Color.White,
                    Message = "",
                    SpriteFont = content.LoadSpriteFont("Design/Button"),
                };

                mHerbSpriteSize = new Point((int)(screenDimensions.Height * HerbHeight));
            }

            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                mHealthPotion.Draw(gameTime, spriteBatch, mPotionSize);
                mHealthPotionCount.Message = mGameState.ActiveHero.mNumberHealthPotions.ToString();
                mHealthPotionCount.Draw(gameTime, spriteBatch);

                mOxygenPotion.Draw(gameTime, spriteBatch, mPotionSize);
                mOxygenPotionCount.Message = mGameState.ActiveHero.mNumberOxygenPotions.ToString();
                mOxygenPotionCount.Draw(gameTime, spriteBatch);

                mBlueHerbSprite.Draw(gameTime, spriteBatch, mHerbSpriteSize);
                mBlueHerbCount.Message = mGameState.ActiveHero.mNumberHerbs[0].ToString();
                mBlueHerbCount.Draw(gameTime, spriteBatch);

                mPurpleHerbSprite.Draw(gameTime, spriteBatch, mHerbSpriteSize);
                mPurpleHerbCount.Message = mGameState.ActiveHero.mNumberHerbs[1].ToString();
                mPurpleHerbCount.Draw(gameTime, spriteBatch);

                mPinkHerbSprite.Draw(gameTime, spriteBatch, mHerbSpriteSize);
                mPinkHerbCount.Message = mGameState.ActiveHero.mNumberHerbs[2].ToString();
                mPinkHerbCount.Draw(gameTime, spriteBatch);
            }

            public void HandleInput()
            {
                var mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Released)
                {
                    mDeBounce = false;
                    return;
                }

                if (mDeBounce)
                {
                    return;
                }

                mDeBounce = true;

                if (mHealthPotion.ContainsPosition(mouseState.Position.ToVector2()))
                {
                    bool valid = mGameState.ActiveHero.UseHealthPotion();
                    // Update Achievement/Statistic State
                    if (mGameState.ActiveHero.mTeam == MovableEntity.OwnTeam && valid)
                    {
                        AchievementState.UpdateAchievement("Hoch Geheilt");
                        AchievementState.UpdateAchievement("Neubelebung");
                        AchievementState.UpdateAchievement("Unsterblich");
                        StatisticState.mTotalUsedHealPotions.mValue += 1;
                    }
                }

                if (mOxygenPotion.ContainsPosition(mouseState.Position.ToVector2()))
                {
                    mGameState.ActiveHero.UseOxygenPotion();
                }
            }

            private void Dispose(bool disposing)
            {
                if (mDisposedValue)
                {
                    return;
                }

                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null

                mDisposedValue = true;
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
            }
        }

        private sealed class HeroAbilityButton : IMenuObject
        {
            private const double ButtonPos = HudHorizontalPos + HudWidth - SmallElementSpace - SmallElementHeight - SmallElementSpace;

            private bool mDeBounce;
            private bool mDisposedValue;

            private readonly GameState.GameState mGameState;
            private readonly Sprite mSprite;
            private readonly Point mDrawSpace;

            private readonly Map mMap;

            internal HeroAbilityButton(Rectangle screenDimensions, GameState.GameState gameState, Map map)
            {
                mGameState = gameState;
                mMap = map;

                mSprite = new Sprite
                {
                    mColor = Color.White,
                    mPosition = new Vector2(
                        (float)(screenDimensions.Width * (ButtonPos)),
                        (float)(screenDimensions.Height * (HudVerticalPos + SmallElementSpace))
                    ),
                    mTexture = mGameState.ActiveHero.mAbilityTexture,
                };

                mDrawSpace = new Point
                (
                    (int)(screenDimensions.Height * (SmallElementSpace + SmallElementHeight + SmallElementHeight))
                );
            }

            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                Debug.Assert(mGameState.ActiveHero.mAbilityTexture != null, "Missing Texture: " + mGameState.ActiveHero.mAbilityTextureName);
                mSprite.mTexture = mGameState.ActiveHero.mAbilityTexture;
                mSprite.Draw(gameTime, spriteBatch, mDrawSpace);
            }

            public void HandleInput()
            {
                var mouseState = Mouse.GetState();
                if (mouseState.LeftButton != ButtonState.Pressed)
                {
                    mDeBounce = false;
                    return;
                }

                if (mDeBounce)
                {
                    return;
                }

                mDeBounce = true;

                if (!new Rectangle(mSprite.mPosition.ToPoint(), mDrawSpace).Contains(mouseState.Position))
                {
                    return;
                }

                MouseInput.SpecialSkill(mGameState.ActiveHero, mMap);
            }

            private void Dispose(bool disposing)
            {
                if (mDisposedValue)
                {
                    return;
                }

                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                mDisposedValue = true;
            }

            // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~HeroAbilityButton()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                // System.GC.SuppressFinalize(this);
            }
        }

        private sealed class HeroImage : IMenuObject
        {
            private readonly Sprite mHeroImageSprite;
            private readonly GameState.GameState mGameState;
            private readonly Point mDimensions;

            private bool mDisposedValue;

            internal HeroImage(Rectangle screenDimensions, GameState.GameState gameState)
            {
                var position = new Vector2(
                    (float)(HudHorizontalPos + SmallElementSpace) * screenDimensions.Width,
                    (float)(HudVerticalPos + SmallElementSpace) * screenDimensions.Height
                );

                mHeroImageSprite = new Sprite()
                {
                    mPosition = position,
                    mColor = Color.White,
                    mTexture = gameState.ActiveHero.mTexture
                };
                mDimensions = new Point(
                    (int)((SmallElementSpace + SmallElementHeight) * screenDimensions.Width),
                    (int)((SmallElementSpace + SmallElementHeight) * screenDimensions.Width)
                );

                //Debug.WriteLine(mDimensions);

                mGameState = gameState;
            }

            ~HeroImage()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: false);
            }

            public void HandleInput()
            {
                // No Input to process
            }

            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                mHeroImageSprite.mTexture = mGameState.ActiveHero.mTexture;
                mHeroImageSprite.Draw(gameTime, spriteBatch, mDimensions);
            }

            private void Dispose(bool disposing)
            {
                if (mDisposedValue)
                {
                    return;
                }

                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                mDisposedValue = true;
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
