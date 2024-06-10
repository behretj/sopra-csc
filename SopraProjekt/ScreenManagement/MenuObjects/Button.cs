using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    /// <summary>
    /// The button class is a composition based class. It is composed mainly using methods/features of other classes.
    /// </summary>
    public sealed class Button : IMenuObject
    {
        //represents the texture of a button
        internal Sprite mSprite;

        //represents the default color that we render our sprites
        internal Color mDefaultSpriteColor;

        //represents the color we render our sprite when we are hovered over the button
        internal Color mHoverSpriteColor;

        //represents the text of our button
        internal Text mText;

        internal Color mDefaultTextColor;
        internal Color mHoverTextColor;

        private MenuObjectState mPreviousButtonState;
        private MenuObjectState mCurrentButtonState;
        private Action mOnClicked;
        private bool mDisposedValue;

        // private Action mOnHovered;

        ~Button()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        //Action Handler1
        internal event Action OnClicked
        {
            add => mOnClicked += value;
            remove => mOnClicked = (Action)Delegate.Remove(mOnClicked, value);
        }

        /*
        not used
        TODO: use or remove

        //Action Handler2
        internal event Action OnHovered
        {
            add => mOnHovered += value;
            remove => mOnHovered = (Action)Delegate.Remove(mOnHovered, value);
        }
        */

        /// <summary>
        /// Method returns true if the checked position is contained within the bounds of our sprite
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns>true if the checked position is contained within the bounds of our sprite, false otherwise</returns>
        private bool ContainsPosition(Vector2 position)
        {
            return mSprite.ContainsPosition(position);
        }

        /// <summary>
        /// Handle mouse state
        /// </summary>
        public void HandleInput()
        {
            var mouseState = Mouse.GetState();
            mSprite.mColor = mDefaultSpriteColor;
            mText.Color = mDefaultTextColor;

            if (ContainsPosition(new Vector2(mouseState.X, mouseState.Y)))
            {
                mSprite.mColor = mHoverSpriteColor;
                mText.Color = mHoverTextColor;

                mCurrentButtonState = MenuObjectState.Hovered;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    mCurrentButtonState = MenuObjectState.Clicked;
                }
            }
            else
            {
                mCurrentButtonState = MenuObjectState.None;
            }

            FireEvents();

            mPreviousButtonState = mCurrentButtonState;
        }

        /// <summary>
        /// Run an event based on the current button state
        /// </summary>
        private void FireEvents()
        {
            if (mCurrentButtonState == mPreviousButtonState)
            {
                return;
            }

            switch (mCurrentButtonState)
            {
                case MenuObjectState.Hovered:
                    //    mOnHovered?.Invoke();
                    break;

                case MenuObjectState.Clicked:
                    mOnClicked?.Invoke();
                    break;

                case MenuObjectState.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Method that draws our sprite and text onto screen
        /// </summary>
        /// <param name="gameTime">global game time.</param>
        /// <param name="spriteBatch">sprite batch to draw on</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mSprite.Draw(gameTime, spriteBatch);
            mText.Draw(gameTime, spriteBatch);
        }

        private void Dispose(bool disposing)
        {
            if (mDisposedValue)
            {
                return;
            }

            if (disposing)
            {
                // dispose of managed resources.
            }

            mSprite.Dispose();
            mText.Dispose();
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