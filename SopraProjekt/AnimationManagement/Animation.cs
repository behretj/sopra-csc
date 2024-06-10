using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using SopraProjekt.Renderer;
using SopraProjekt.Sound;
using System.Linq;

namespace SopraProjekt.AnimationManagement
{
    /// <summary>
    /// Sets three different Actions for spritesheets
    /// </summary>
    public enum Actions
    {
        Stop,
        Move,
        Fight
    }

    /// <summary>
    /// Class to represent an animation
    /// </summary>
    public class Animation
    {
        private readonly int mHoldPos = 0; //frame 0
        private readonly int mWalkpos1 = 1; //frame 1
        private readonly int mWalkpos2 = 2; //frame 2
        private readonly int mFightpos1 = 3; //frame 3
        private readonly int mFightpos2 = 4; //frame 4
        private readonly int mSpriteRow; //which row of spritesheet
        public int CurrentFrame { get; set; }
        private int FrameCount { get; } //how many frames per row (5)
        private readonly int mSpriteSheetRows = 8;
        private int FrameHeight => Texture.Height / mSpriteSheetRows;
        private float FrameSpeed { get; } //how fast frames switch
        private int FrameWidth => Texture.Width / FrameCount;
        private Texture2D Texture { get; }
        private readonly Point mTextureSize;
        public Actions mCurrentAction;
        private float mTimer; //counts time of one frame traversal
        private float mAttackTimer;
        private readonly float mAttackTime;

        private readonly Entity mSource;
        private readonly Camera mCamera;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spriteRow">row of spritesheet</param>
        /// <param name="texture">spritesheet</param>
        /// <param name="frameCount">total amount of frames per spritesheet</param>
        /// <param name="textureSize">size of the texture</param>
        /// <param name="source">entity to animate</param>
        /// <param name="camera">camera</param>
        public Animation(int spriteRow, Texture2D texture, int frameCount, Point textureSize, Entity source, Camera camera)
        {
            mSpriteRow = spriteRow;
            Texture = texture;
            mTextureSize = textureSize;
            FrameCount = frameCount;
            FrameSpeed = 0.5f;
            mAttackTime = 1.2f;
            CurrentFrame = mHoldPos; //initial frame
            mCurrentAction = Actions.Stop; //initial action is stop (front)
            mSource = source;
            mCamera = camera;
        }

        /// <summary>
        /// Update method for animation class
        /// </summary>
        /// <param name="gameTime">gameTime</param>
        public void Update(GameTime gameTime)
        {
            if (CurrentFrame == mHoldPos)
            {
                return;
            }

            mTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            mAttackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((mAttackTimer > mAttackTime) && mCurrentAction == Actions.Fight)
            {
                mCurrentAction = Actions.Stop;
                CurrentFrame = mHoldPos;
                mAttackTimer = 0f;
                SoundManager.Default.PlaySoundEffect("SoundEffects/soundFight", mSource.mPosition, mCamera);
            }

            if (!(mTimer > FrameSpeed))
            {
                return;
            }

            if (mCurrentAction != Actions.Fight)
            {
                SoundManager.Default.PlaySoundEffect("SoundEffects/soundEffectWalking", mSource.mPosition, mCamera);
            }
            mTimer = 0f;
            UpdateCurrentFrame();
        }

        /// <summary>
        /// Helper function for update method
        /// Defines switching of frames for walking and fighting
        /// </summary>
        private void UpdateCurrentFrame()
        {
            //switch between two walking positions (frame 1 and 2)
            if (Enumerable.Range(mWalkpos1, mWalkpos2).Contains(CurrentFrame))
            {
                CurrentFrame = CurrentFrame == mWalkpos1 ? mWalkpos2 : mWalkpos1;
            }
            //switch between two fight positions (frame 3 and 4)
            if (Enumerable.Range(mFightpos1, mFightpos2).Contains(CurrentFrame))
            {
                CurrentFrame = CurrentFrame == mFightpos1 ? mFightpos2 : mFightpos1;
            }
        }

        /// <summary>
        /// Draw method for animation class
        /// </summary>
        /// /// <param name="spriteBatch">spritesheet</param>
        /// /// <param name="position">position on map</param>
        public void Draw(SpriteBatch spriteBatch, Point position)
        {
            Point p = new Point(CurrentFrame * FrameWidth, mSpriteRow * FrameHeight);
            Point p1 = new Point(FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
                position.Y - mTextureSize.Y), mTextureSize), new Rectangle(p, p1), Color.White);
            //spriteBatch.Draw(Texture,
            //    new Vector2(position.X, position.Y),
            //    new Rectangle(p, p1),
            //    Color.White,
            //    0.0f,
            //    Vector2.Zero,
            //    0.4f,
            //    SpriteEffects.None,
            //    0.1f);
        }

        /// <summary>
        /// Helper function for frame setting
        /// </summary>
        /// /// <param name="action">stop, walk or fight</param>
        public void SetCurrentFrame(Actions action)
        {
            if (mCurrentAction == Actions.Fight)
            {
                return;
            }

            switch (action)
            {
                case Actions.Stop:
                    CurrentFrame = mHoldPos;
                    mCurrentAction = Actions.Stop;
                    SoundManager.Default.StopSoundEffects();

                    break;
                case Actions.Fight:
                    mCurrentAction = Actions.Fight;
                    CurrentFrame = mFightpos1;
                    break;
                case Actions.Move:
                    if (mCurrentAction == Actions.Move)
                    {
                        return;
                    }

                    mCurrentAction = Actions.Move;
                    CurrentFrame = mWalkpos1;
                    break;
            }
        }
    }
}
