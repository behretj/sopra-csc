using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using System.Collections.Generic;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    internal sealed class HeroSelectorButton : IMenuObject
    {
        private bool mDisposeValue;

        private readonly MovableEntity mHero;
        private Rectangle mBounds;

        private readonly Sprite mHeroSprite;
        private readonly Sprite mBackgroundSprite;
        private readonly Sprite mBackgroundDeadSprite;
        private readonly Texture2D mHealthTexture;
        private readonly Texture2D mOxygenTexture;

        private const string HealthBarTexture = "Design/Red";
        private const string OxygenBarTexture = "Design/Blue";
        private const string BackgroundTexture = "Images/Menu/Panel01";
        private const string DeadBackgroundTexture = "Images/Menu/grey";

        private readonly GameState.GameState mGameState;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="content"></param>
        /// <param name="gameState"></param>
        public HeroSelectorButton(
            MovableEntity hero,
            Point position,
            Point size,
            ContentLoader content,
            GameState.GameState gameState
        )
        {
            mGameState = gameState;
            mHero = hero;
            mHealthTexture = content.LoadTexture(HealthBarTexture);
            mOxygenTexture = content.LoadTexture(OxygenBarTexture);

            mBounds = new Rectangle(position.X, position.Y, size.X, size.Y);

            mBackgroundSprite = new Sprite
            {
                mColor = Color.White,
                mPosition = new Vector2(mBounds.X, mBounds.Y + mBounds.Height / 2),
                mTexture = content.LoadTexture(BackgroundTexture)
            };

            mBackgroundDeadSprite = new Sprite
            {
                mColor = Color.White,
                mPosition = new Vector2(mBounds.X, mBounds.Y + mBounds.Height / 2),
                mTexture = content.LoadTexture(DeadBackgroundTexture)
            };

            mHeroSprite = new Sprite
            {
                mColor = Color.White,
                mPosition = new Vector2(mBounds.X, mBounds.Y + mBounds.Height / 2),
                mTexture = mHero.mTexture
            };
        }

        private void Dispose(bool dispose)
        {
            if (mDisposeValue)
            {
                return;
            }

            if (!dispose)
            {
                return;
            }

            // dispose of managed resources.
            mDisposeValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(dispose: true);
            // System.GC.SuppressFinalize(this);
        }

        public void HandleInput()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                return;
            }

            var mousePosition = Mouse.GetState().Position;
            if (mBackgroundSprite.ContainsPosition(mousePosition.ToVector2()))
            {
                if (mHero.IsAlive)
                {
                    mGameState.ActiveHero = (Hero)mHero;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            const double barScale = 0.6;

            if (mHero.IsAlive)
            {
                spriteBatch.Draw(
                    mHealthTexture,
                    new Rectangle(
                        mBounds.Location + new Point(0, (int) (mBounds.Height * barScale) + mBounds.Height),
                        new Point((int)(mBounds.Width * (mHero.mCurrentHealthPoints / (float)mHero.mMaxHealthPoints)), (int)(mBounds.Height * barScale))
                    ),
                    Color.White
                );

                spriteBatch.Draw(
                    mOxygenTexture,
                    new Rectangle(
                        mBounds.Location + new Point(0, (int)((mBounds.Height + mBounds.Height) * barScale) + mBounds.Height),
                        new Point((int)(mBounds.Width * (mHero.mCurrentOxygenPoints / (float)mHero.mMaxOxygenPoints)), (int)(mBounds.Height * barScale))
                    ),
                    Color.White
                );

                mBackgroundSprite.Draw(gameTime, spriteBatch, mBounds.Size);
            }
            else
            {
                mBackgroundDeadSprite.Draw(gameTime, spriteBatch, mBounds.Size);
            }


            var heroWidth = (int)(mHero.mTexture.Width * (mBounds.Height / (double)mHero.mTexture.Height));

            mHeroSprite.Draw(gameTime, spriteBatch, new Point(heroWidth, mBounds.Height));
        }
    }

    internal sealed class HeroSelectorElement : IMenuObject
    {
        private bool mDisposedValue;

        private const float HeightScale = 0.025f;
        private const float WidthScale = 0.15f;

        private const float DistScale = 0.05f;

        private readonly List<HeroSelectorButton> mElements = new List<HeroSelectorButton>();

        internal HeroSelectorElement(ContentLoader content, GameState.GameState gameState, GraphicsDeviceManager graphics)
        {
            var screenDimensions =
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            var size = new Point((int)(screenDimensions.Width * WidthScale), (int)(screenDimensions.Height * HeightScale));
            var buttonDistance = (int)(screenDimensions.Height * DistScale);

            for (var j = 0; j < gameState.Heroes.mTeamMembers.Count; j++)
            {
                var position = new Point(buttonDistance, j * (size.Y + buttonDistance));
                mElements.Add(new HeroSelectorButton(gameState.Heroes.mTeamMembers[j], position, size, content, gameState));
            }
        }

        public void HandleInput()
        {
            foreach (var button in mElements)
            {
                button.HandleInput();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var button in mElements)
            {
                button.Draw(gameTime, spriteBatch);
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
                // TODO: dispose managed state (managed objects)
            }

            foreach (var element in mElements)
            {
                element.Dispose();
            }
            mDisposedValue = true;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HeroSelectorElement()
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
}