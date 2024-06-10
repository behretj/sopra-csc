using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Input;
using SopraProjekt.Renderer;
using SopraProjekt.ScreenManagement.MenuObjects;
using System;
using System.Collections.Generic;

namespace SopraProjekt.ScreenManagement.Screens
{
    /// <summary>
    /// Class to handle screen elements
    /// </summary>
    internal abstract class GameScreen : IGameScreen
    {
        protected GameScreenManager mScreenManager;
        protected InputHandler mInputHandler;
        private bool mDisposedValue;
        protected readonly List<IDrawAble> mDrawAbles = new List<IDrawAble>();
        protected readonly List<IClickAble> mClickAbles = new List<IClickAble>();

        public bool IsPaused { get; private set; }
        public bool IsIsometric { get; protected set; }

        public Camera Camera { get; protected set; }

        ~GameScreen()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }


        /// <summary>
        /// Initialize the basic screen.
        /// </summary>
        /// <param name="contentManager">content manager</param>
        /// <param name="screenBounds">bounds of screen</param>
        /// <param name="message">message to show</param>
        /// <param name="titlePosition">position of the message</param>
        /// <param name="assetName">asset name of the background image</param>
        protected void Initialize(ContentManager contentManager,
            Rectangle screenBounds,
            string message,
            Point titlePosition,
            string assetName)
        {
            //Adding a background to the Intro Screen
            var background = new Sprite
            {
                mTexture = contentManager.Load<Texture2D>(assetName),
                mPosition = Vector2.Zero
            };

            background.Center(new Rectangle(0, 0, screenBounds.Width, screenBounds.Height));
            mDrawAbles.Add(background);

            var spriteFontAssetName = (screenBounds.Width < 1440) ? "Design/Title_small" : "Design/Title";

            var title = new Text()
            {
                SpriteFont = contentManager.Load<SpriteFont>(spriteFontAssetName),
                Color = Color.White,
                Message = message
            };

            title.CenterHorizontal(new Rectangle(titlePosition.X, titlePosition.Y, screenBounds.Width, 0));
            mDrawAbles.Add(title);

            if (mInputHandler != null)
            {
                mInputHandler.mMouseInput.mButtonMoveReleased = false;
                mInputHandler.mMouseInput.mButtonSelectReleased = false;
            }
        }

        internal abstract void Initialize(ContentManager content);

        /// <summary>
        /// helper function pauses current screen
        /// </summary>
        public virtual void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// helper function ends pause on current screen
        /// </summary>
        public virtual void Resume()
        {
            IsPaused = false;
        }


        /// <summary>
        /// empty update function
        /// </summary>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// empty ChangeBetweenScreens function
        /// </summary>
        public abstract void ChangeBetweenScreens();

        /// <summary>
        /// Draw the menu
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var drawable in mDrawAbles)
            {
                drawable.Draw(gameTime, spriteBatch);
            }
        }

        /// <summary>
        /// handle mouse input
        /// </summary>
        public void HandleInput()
        {
            foreach (var clickAble in mClickAbles)
            {
                clickAble.HandleInput();
            }
        }

        /// <summary>
        /// Adds a button to the screen.
        /// </summary>
        /// <param name="content">Global content Manager</param>
        /// <param name="positionSpace">Rectangle in which the button is centered.</param>
        /// <param name="title">Printed text on the button.</param>
        /// <param name="textColor">Color of this text.</param>
        /// <param name="buttonTexture">Path to the texture of the button.</param>
        /// <param name="defaultSpriteColor">Default sprite color.</param>
        /// <param name="hoverSpriteColor">Color of sprite when hovered.</param>
        /// <param name="defaultTextColor">Default color of the text.</param>
        /// <param name="hoverTextColor">Color of the text when hovered.</param>
        /// <returns>The created button.</returns>
        protected Button AddButton(
            ContentManager content,
            Rectangle positionSpace, string title, Color textColor, string buttonTexture,
            Color defaultSpriteColor, Color hoverSpriteColor, Color defaultTextColor, Color hoverTextColor)
        {
            var buttonSprite = new Sprite
            {
                mTexture = content.Load<Texture2D>(buttonTexture)
            };
            buttonSprite.Center(positionSpace);

            var buttonText = new Text
            {
                Message = title,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/Button")
            };
            buttonText.Center(buttonSprite.Bounds);


            var button = new Button
            {
                mSprite = buttonSprite,
                mDefaultSpriteColor = defaultSpriteColor,
                mHoverSpriteColor = hoverSpriteColor,
                mText = buttonText,
                mDefaultTextColor = defaultTextColor,
                mHoverTextColor = hoverTextColor
            };

            mClickAbles.Add(button);
            mDrawAbles.Add(button);

            return button;
        }

        protected void AddText(
            ContentManager content,
            Rectangle positionSpace, string title, Color textColor, string buttonTexture)
        {
            var messageSprite = new Sprite
            {
                mTexture = content.Load<Texture2D>(buttonTexture)
            };
            messageSprite.Center(positionSpace);

            var messageText = new Text
            {
                Message = title,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/StatisticFont")
            };

            messageText.Center(messageSprite.Bounds);

            mDrawAbles.Add(messageText);

            // return messageText;
        }


        /// <summary>
        /// Adds an achievement to the screen
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="positionSpace">Position where to draw</param>
        /// <param name="title">Title of Achievement</param>
        /// <param name="description">Description of Achievement</param>
        /// <param name="achieved">Is the achievement already achieved</param>
        /// <param name="currentState">Actual state of done</param>
        /// <param name="toAchieve">Number to have to get the achievement</param>
        /// <param name="textColor"></param>
        protected void AddAchievement(
            ContentManager content,
            Rectangle positionSpace, string title, string description, bool achieved, int currentState, int toAchieve, Color textColor)
        {
            if (achieved)
            {
                textColor = Color.Green;
            }
            // Create Title
            Rectangle titleSpace = new Rectangle(positionSpace.X, positionSpace.Y, positionSpace.Width, positionSpace.Height * (1 / 3));
            var achievementTitle = new Text
            {
                Message = title,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/AchievementTitle")
            };
            achievementTitle.Center(titleSpace);

            // Create Description
            Rectangle descriptionSpace = new Rectangle(positionSpace.X, positionSpace.Y + 30, positionSpace.Width, positionSpace.Height * (1 / 3));
            var achievementDescription = new Text
            {
                Message = description,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/AchievementDescription")
            };
            achievementDescription.Center(descriptionSpace);

            // Create Ratio
            Rectangle ratioRectangle = new Rectangle(positionSpace.X, positionSpace.Y + 60, positionSpace.Width, positionSpace.Height * (1 / 3));
            var achievementRatio = new Text
            {
                Message = Convert.ToString(currentState) + " / " + Convert.ToString(toAchieve),
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/AchievementDescription")
            };
            achievementRatio.Center(ratioRectangle);

            // Add to screen
            mDrawAbles.Add(achievementTitle);
            mDrawAbles.Add(achievementDescription);
            mDrawAbles.Add(achievementRatio);
        }


        /// <summary>
        /// Adds a statistic to the screen
        /// </summary>
        /// <param name="content">Content Manager</param>
        /// <param name="positionSpace">Position where to draw</param>
        /// <param name="title">Title of the Statistic</param>
        /// <param name="number">Value of statistic</param>
        /// <param name="textColor">Color of the text</param>
        protected void AddStatistic(
            ContentManager content,
            Rectangle positionSpace, string title, int number, Color textColor)
        {
            // Creating Title
            Rectangle titleSpace = new Rectangle(positionSpace.X, positionSpace.Y, Convert.ToInt32(positionSpace.Width * 0.45), positionSpace.Height);
            var statisticTitle = new Text
            {
                Message = title,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/StatisticFont")
            };
            statisticTitle.Center(titleSpace);

            // Creating :
            Rectangle middleSpace = new Rectangle(Convert.ToInt32(positionSpace.Width * 0.45), positionSpace.Y, Convert.ToInt32(positionSpace.Width * 0.1), positionSpace.Height);
            var middleText = new Text
            {
                Message = ":",
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/StatisticFont")
            };
            middleText.Center(middleSpace);

            // Creating Number
            Rectangle numberSpace = new Rectangle(Convert.ToInt32(positionSpace.Width * 0.55), positionSpace.Y, Convert.ToInt32(positionSpace.Width * 0.45), positionSpace.Height);
            var numberText = new Text
            {
                Message = Convert.ToString(number),
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/StatisticFont")
            };
            numberText.Center(numberSpace);

            // Add to screen
            mDrawAbles.Add(statisticTitle);
            mDrawAbles.Add(middleText);
            mDrawAbles.Add(numberText);
        }


        /// <summary>
        /// Adds a label to the screen
        /// </summary>
        /// <param name="content"></param>
        /// <param name="positionSpace"></param>
        /// <param name="title"></param>
        /// <param name="textColor"></param>
        protected void AddLabel(
            ContentManager content,
            Rectangle positionSpace, string title, Color textColor)
        {
            var label = new Text
            {
                Message = title,
                Color = textColor,
                SpriteFont = content.Load<SpriteFont>("Design/Button")
            };
            label.Center(positionSpace);

            mDrawAbles.Add(label);
        }

        /// <summary>
        /// Adds a fader to the screen.
        /// </summary>
        protected Fader AddFader(
            ContentManager content,
            Rectangle titlePositionSpace,
            Rectangle faderPositionSpace,
            string backgroundTexture,
            string knobTexture,
            string faderTitle,
            Color titleColor,
            Color outputColor,
            float initialValue)
        {
            var faderBackground = new Sprite
            {
                mTexture = content.Load<Texture2D>(backgroundTexture)
            };
            faderBackground.Center(faderPositionSpace);

            var faderKnob = new Sprite
            {
                mTexture = content.Load<Texture2D>(knobTexture)
            };

            var faderTitleText = new Text
            {
                Message = faderTitle,
                Color = titleColor,
                SpriteFont = content.Load<SpriteFont>("Design/Title_small"),
            };
            faderTitleText.Center(titlePositionSpace);
            mDrawAbles.Add(faderTitleText);

            var faderOutput = new Text
            {
                Message = "",
                Color = outputColor,
                SpriteFont = content.Load<SpriteFont>("Design/Button")
            };
            var spaceFilled = (int)faderBackground.mPosition.X + faderBackground.Bounds.Width + faderKnob.Bounds.Width;
            faderOutput.CenterHorizontal(new Rectangle(spaceFilled, faderBackground.Bounds.Y, faderPositionSpace.Width - spaceFilled - faderPositionSpace.Width / 4, 0));

            var fader = new Fader(faderBackground, faderKnob, faderOutput, initialValue);

            mClickAbles.Add(fader);
            mDrawAbles.Add(fader);

            return fader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose of managed resources.
            }

            foreach (var clickAble in mClickAbles)
            {
                clickAble.Dispose();
            }

            foreach (var drawAble in mDrawAbles)
            {
                drawAble.Dispose();
            }

            mDisposedValue = true;
        }

        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
