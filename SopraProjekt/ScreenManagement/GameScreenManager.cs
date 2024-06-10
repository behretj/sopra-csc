using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Helper;
using SopraProjekt.ScreenManagement.Screens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SopraProjekt.ScreenManagement
{
    /// <summary>
    /// Class to manage all screens
    /// </summary>
    internal sealed class GameScreenManager
    {
        private readonly SpriteBatch mSpriteBatch;
        private readonly ContentManager mContentManager;
        private Action mOnGameExit;
        private readonly GraphicsDeviceManager mGraphics;
        private readonly Texture2D mGameBackground;

        //Screens are handled via a GameScreen list
        private readonly List<IGameScreen> mGameScreens = new List<IGameScreen>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        /// <param name="contentManager">content manager</param>
        /// <param name="graphics">graphics device manager</param>
        internal GameScreenManager(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics)
        {
            mSpriteBatch = spriteBatch;
            mContentManager = contentManager;
            mGraphics = graphics;
            mGameBackground = new ContentLoader(contentManager).LoadTexture("Design/background");
        }


        /// <summary>
        /// checks if screen list is empty
        /// </summary>
        /// <returns>true if screen list is empty, false otherwise</returns>
        private bool IsScreenListEmpty() => mGameScreens.Count <= 0;

        /// <summary>
        /// helper function
        /// </summary>
        /// <returns>current GameScreen at the back of the GameScreenList</returns>
        private IGameScreen GetCurrentScreen()
        {
            return mGameScreens[^1];
        }

        /// <summary>
        /// Removes current GameScreen, i.e. screen at the back of the GameScreenList
        /// </summary>
        private void RemoveCurrentScreen()
        {
            var screen = GetCurrentScreen();
            screen.Dispose();
            mGameScreens.Remove(screen);
        }

        /// <summary>
        /// Removes all screens from GameScreenList (e.g. when game is over)
        /// </summary>
        internal void RemoveAllScreens()
        {
            while (!IsScreenListEmpty())
            {
                RemoveCurrentScreen();
            }
        }

        /// <summary>
        /// Update all screens
        /// </summary>
        /// <param name="gameTime"></param>
        internal void Update(GameTime gameTime)
        {
            if (IsScreenListEmpty())
            {
                return;
            }

            foreach (var curScreen in mGameScreens.Where(curScreen => !curScreen.IsPaused))
            {
                curScreen.Update(gameTime);
            }
        }

        /// <summary>
        /// Draw all screens
        /// </summary>
        /// <param name="gameTime">game time</param>
        internal void Draw(GameTime gameTime)
        {
            if (IsScreenListEmpty())
            {
                return;
            }

            foreach (var curScreen in mGameScreens.Where(curScreen => !curScreen.IsPaused))
            {
                if (curScreen.IsIsometric)
                {
                    mSpriteBatch.Begin();
                    mSpriteBatch.Draw(mGameBackground, new Rectangle(0, 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight), Color.White);
                    mSpriteBatch.End();
                    mSpriteBatch.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        null, null, null, null,
                        curScreen.Camera.TransformMatrix);
                    curScreen.Draw(gameTime, mSpriteBatch);
                    mSpriteBatch.End();
                }
                else
                {
                    mSpriteBatch.Begin();
                    curScreen.Draw(gameTime, mSpriteBatch);
                    mSpriteBatch.End();
                }
            }
        }

        /// <summary>
        /// Handle input
        /// </summary>
        internal void HandleInput()
        {
            if (IsScreenListEmpty())
            {
                return;
            }

            foreach (var curScreen in mGameScreens.Where(curScreen => !curScreen.IsPaused))
            {
                curScreen.HandleInput();
            }
        }

        /// <summary>
        /// removes all screens from GameScreenList and initializes a new screen
        /// </summary>
        /// <param name="screen">new screen</param>
        internal void ChangeScreen(GameScreen screen)
        {
            RemoveAllScreens();
            mGameScreens.Add(screen);
            screen.Initialize(mContentManager);
        }

        /// <summary>
        /// Adds a new screen to the GameScreenList while the previous screens are paused
        /// </summary>
        /// <param name="screen"></param>
        internal void PushScreen(GameScreen screen)
        {
            if (!IsScreenListEmpty())
            {
                var curScreen = GetCurrentScreen();
                curScreen.Pause();
            }
            mGameScreens.Add(screen);
            screen.Initialize(mContentManager);
        }


        /// <summary>
        /// removes most current screen, i.e. screen at the end of the GameScreenList
        /// and ends pause on previous screen - not yet used and commented out
        /// </summary>
        internal void PopScreen()
        {
            if (!IsScreenListEmpty())
            {
                RemoveCurrentScreen();
            }

            if (IsScreenListEmpty())
            {
                return;
            }
            var newCurScreen = GetCurrentScreen();
            newCurScreen.Resume();
        }

        /// <summary>
        /// Change between screens
        /// </summary>
        internal void ChangeBetweenScreens()
        {
            if (IsScreenListEmpty())
            {
                return;
            }

            // Please keep this like that!!!!!!!!!!!!!!!!!!!!!!!!!!! else the compile will fail. -> enumerator invalidation
            foreach (IGameScreen v in mGameScreens.ToList())
            {
                if (!v.IsPaused)
                {
                    v.ChangeBetweenScreens();
                }
            }
        }

        /// <summary>
        /// Exit game function, can be invoked from Intro Screen via "Spiel Verlassen" button.
        /// </summary>
        internal void Exit()
        {
            mOnGameExit?.Invoke();
        }

        /// <summary>
        /// Action handling for game exit
        /// </summary>
        internal event Action OnGameExit
        {
            add => mOnGameExit += value;
            remove => mOnGameExit = (Action)Delegate.Remove(mOnGameExit, value);
        }
    }
}
