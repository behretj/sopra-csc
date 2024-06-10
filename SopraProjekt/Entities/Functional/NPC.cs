using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.GameState;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities.Functional
{
    /// <summary>
    /// Class to represent a non-player-character
    /// </summary>
    [Serializable]
    internal class Npc : Colliding
    {
        // Todo: add more messages
        private static readonly string[][] sPositiveMessages = new string[][]
        {
            new []
            {
                "Hier fremder, du siehst nett aus",
                "Nimm dies!"
            },
        };

        private static readonly string[][] sNegativeMessages = new string[][]
        {
            new [] {"Ein Fremder!!! Geh weg!"},
        };

        private const string OwnAssetName = "Images/Entities/NPC";
        private const int TextureSize = 80;

        private const int Damage = 100;
        private const int Healing = 100;
        internal const double DisplayTime = 5;

        private double mLastInteraction;

        [NonSerialized]
        public Camera mCamera;
        [NonSerialized]
        public Dictionary<string, Texture2D> mTextures;

        [NonSerialized]
        private SpriteFont mFont;

        /// <summary>
        /// Constructor
        /// </summary>
        public Npc(Point position, Camera camera, Dictionary<string, Texture2D> textures) :
            base(OwnAssetName, "npc_buyer", position, new Point(TextureSize), Color.Yellow)
        {
            mCamera = camera;
            mTextures = textures;
            mFont = Content.LoadSpriteFont("Design/Button");
        }

        /// <summary>
        /// Draws the entity
        /// </summary>
        /// <param name="spriteBatch">drawing on the SpriteBatch</param>
        /// <param name="position">draw entity at this position</param>
        // todo: duplicates the entity.draw() method, maybe NPC should inherit from something else than MoveableEntity
        // todo: yes, it should probably inherit from Entity base class
        public override void Draw(SpriteBatch spriteBatch, Point position)
        {
            spriteBatch.Draw(mTexture, new Rectangle(new Point(position.X - mTextureSize.X / 2,
                position.Y - mTextureSize.Y), mTextureSize), Color.White);
        }

        /// <summary>
        /// Method that implements an interaction between NPC and a movable entity
        /// </summary>
        public void Interact(MovableEntity entity, GameTime gameTime)
        {
            // Update Achievement "Gesprächig"
            if (entity.mTeam == MovableEntity.OwnTeam)
            {
                AchievementState.UpdateAchievement("Gespraechig");
            }

            if (gameTime.TotalGameTime.TotalSeconds - mLastInteraction < DisplayTime)
            {
                return;
            }

            mLastInteraction = gameTime.TotalGameTime.TotalSeconds;
            // Apply damage or healing
            var rand = new Random();
            int messageNumber;
            switch (rand.Next(0, 2))
            {
                // Do Damage
                case 0:
                    entity.GetDamage(Damage);
                    // Displays message
                    messageNumber = rand.Next(sNegativeMessages.Length);
                    EntityRenderer.mNpcMessages.Push(new Message(mPosition, sNegativeMessages[messageNumber], gameTime.TotalGameTime.TotalSeconds,
                        new Tuple<bool, bool>(false, false), entity as Hero, this, mCamera, mTextures, mFont));
                    break;
                // Do Healing
                case 1:
                    entity.GetDamage(-Healing);
                    // Displays message
                    messageNumber = rand.Next(sPositiveMessages.Length);
                    EntityRenderer.mNpcMessages.Push(new Message(mPosition, sPositiveMessages[messageNumber], gameTime.TotalGameTime.TotalSeconds,
                        new Tuple<bool, bool>(false, false), entity as Hero, this, mCamera, mTextures, mFont));
                    break;
            }
        }
    }
}
