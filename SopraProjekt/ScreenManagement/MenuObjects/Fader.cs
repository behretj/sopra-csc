using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    public sealed class Fader : IMenuObject
    {
        private readonly Sprite mBackgroundSprite;
        private readonly Sprite mKnobSprite;
        private readonly Text mOutput;

        private MenuObjectState mPreviousMouseState;

        private Point mLastMousePoint;
        private bool mDisposedValue;

        /// <summary>
        /// stores the internal value of this fader
        /// </summary>
        internal float Value { get; private set; }

        internal Fader(Sprite backgroundSprite, Sprite knobSprite, Text output, float startPosition)
        {
            mBackgroundSprite = backgroundSprite;
            mKnobSprite = knobSprite;
            Value = Math.Clamp(startPosition, 0, 1);
            mOutput = output;
            UpdateKnobPosition();
            UpdateOutput();
        }

        ~Fader()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mBackgroundSprite.Draw(gameTime, spriteBatch);
            mKnobSprite.Draw(gameTime, spriteBatch);
            mOutput.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Method returns true if the checked position is contained within the bounds of our sprite
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns>true if the checked position is contained within the bounds of our sprite, false otherwise</returns>
        private bool ContainsPosition(Vector2 position)
        {
            return mBackgroundSprite.Bounds.Contains(position);
        }

        public void HandleInput()
        {
            var mouseState = Mouse.GetState();
            switch (mPreviousMouseState)
            {
                // Fader was not grabbed before - don't think about that
                case MenuObjectState.Hovered:
                case MenuObjectState.None:
                    break;

                case MenuObjectState.Clicked:
                    // still clicked
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        var mousePosition = mouseState.Position;
                        Value = mousePosition.X > mLastMousePoint.X
                            ? Math.Clamp(Value + (mousePosition.ToVector2() - mLastMousePoint.ToVector2()).Length() / mBackgroundSprite.Bounds.Width, 0.0f, 1.0f)
                            : Math.Clamp(Value - (mousePosition.ToVector2() - mLastMousePoint.ToVector2()).Length() / mBackgroundSprite.Bounds.Width, 0.0f, 1.0f);
                        mLastMousePoint = mousePosition;
                        UpdateKnobPosition();
                        UpdateOutput();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (mouseState.LeftButton)
            {
                case ButtonState.Released:
                    mPreviousMouseState = MenuObjectState.None;
                    break;

                case ButtonState.Pressed:
                    if (ContainsPosition(mouseState.Position.ToVector2()))
                    {
                        mPreviousMouseState = MenuObjectState.Clicked;
                        mLastMousePoint = mouseState.Position;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the knob position
        /// </summary>
        private void UpdateKnobPosition()
        {
            const double halfFactor = 2.0;
            mKnobSprite.Center(new Rectangle(
                mBackgroundSprite.mPosition.ToPoint() + new Point(
                    (int)(mBackgroundSprite.Bounds.Width * Value - mKnobSprite.mTexture.Width / halfFactor),
                    (int)(-mKnobSprite.Bounds.Height / halfFactor)),
            new Point(mKnobSprite.mTexture.Bounds.Width, mKnobSprite.mTexture.Bounds.Height))
);
        }

        /// <summary>
        /// Updated the output text
        /// </summary>
        private void UpdateOutput()
        {
            const int percentFactor = 100;
            mOutput.Message = ((int)(Math.Round(percentFactor * Value))) + "%";
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

            mBackgroundSprite.Dispose();
            mKnobSprite.Dispose();
            mOutput.Dispose();
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