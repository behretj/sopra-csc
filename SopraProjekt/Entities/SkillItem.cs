using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SopraProjekt.Entities
{
    /// <summary>
    /// Class that represents a shot missile
    /// </summary>
    [Serializable]
    public class SkillItem
    {
        // todo: sound effect for shooting/reaching target?
        // private SoundEffect mSoundEffect
        [NonSerialized]
        private Texture2D mTexture;
        private string mAssetName;
        // holds all the upcoming positions of the Missile except the current mPosition:
        [NonSerialized]
        protected Queue<Vector2> mMovePath;
        private List<float> mMovePathX;
        private List<float> mMovePathY;

        [NonSerialized]
        protected Vector2 mCurrentPosition;
        private float mCurrentPositionX;
        private float mCurrentPositionY;
        // represents the team Id of the team that shot the missile
        protected readonly int mTeam;

        // Id of the entity that shot the missile
        protected readonly int mShotById;

        [NonSerialized]
        protected Point mTextureSize;
        private int mTextureSizeX;
        private int mTextureSizeY;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">source position (missile was shot at this position)</param>
        /// <param name="target">target position (missile will fly to that point)</param>
        /// <param name="team">team that the missile belongs to</param>
        /// <param name="id">Id of the entity that shot the missile</param>
        /// <param name="content">content loader to get texture of missile</param>
        /// <param name="assetName">name of texture</param>
        internal SkillItem(Vector2 source, Vector2 target, int team, int id, ContentLoader content, string assetName)
        {
            mAssetName = assetName;
            mTexture = content.LoadTexture(assetName);
            mShotById = id;
            mMovePath = new Queue<Vector2>();
            mCurrentPosition = source;
            mTeam = team;
            CalculatePath(source, target);
        }

        /// <summary>
        /// Calculates the direct path from source to target
        /// </summary>
        /// <param name="source">beginning of the path</param>
        /// <param name="target">end of the path</param>
        private void CalculatePath(Vector2 source, Vector2 target)
        {
            var sourceToTarget = target - source;
            var sourceToTargetHelper = sourceToTarget;
            sourceToTarget.Normalize();
            sourceToTarget /= 4;
            var steps = (int)(sourceToTargetHelper.Length() / sourceToTarget.Length());
            for (var i = 1; i <= steps; i++)
            {
                mMovePath.Enqueue(source + Vector2.Multiply(sourceToTarget, i));
            }
        }

        /// <summary>
        /// Drawer of the Missile
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw on</param>
        internal void Draw(SpriteBatch spriteBatch)
        {
            var position = (IsoHelper.TwoDToIsometric(mCurrentPosition)).ToPoint();
            spriteBatch.Draw(mTexture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
                position.Y - mTextureSize.Y), mTextureSize), Color.White);
        }

        /// <summary>
        /// Checks whether the entity has arrived at its last calculated position
        /// </summary>
        internal bool HasArrived()
        {
            return mMovePath.Count == 0;
        }

        /// <summary>
        /// Updates the logic of the missile
        /// </summary>
        /// <param name="map">map, which the missile flies on</param>
        internal virtual void Update(Map map) { }

        /// <summary>
        /// Prepares Class for Serialization
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing()]
        private void PrepareStorage(StreamingContext context)
        {
            mTextureSizeX = mTextureSize.X;
            mTextureSizeY = mTextureSize.Y;
            mCurrentPositionX = mCurrentPosition.X;
            mCurrentPositionY = mCurrentPosition.Y;
            mMovePathX = new List<float>();
            mMovePathY = new List<float>();
            Queue<Vector2> path = new Queue<Vector2>();
            while (mMovePath.Count > 0)
            {
                var helper = mMovePath.Dequeue();
                mMovePathX.Add(helper.X);
                mMovePathY.Add(helper.Y);
                path.Enqueue(helper);
            }
            mMovePath = path;
        }

        /// <summary>
        /// Checks out class after deserialization
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized()]
        private void CheckOutStorage(StreamingContext context)
        {
            mTexture = Entity.Content.LoadTexture(mAssetName);
            mTextureSize = new Point(mTextureSizeX, mTextureSizeY);
            mCurrentPosition = new Vector2(mCurrentPositionX, mCurrentPositionY);
            mMovePath = new Queue<Vector2>();
            while (mMovePathX.Count > 0)
            {
                mMovePath.Enqueue(new Vector2(mMovePathX[0], mMovePathY[0]));
                mMovePathX.RemoveAt(0);
                mMovePathY.RemoveAt(0);
            }
        }
    }
}
