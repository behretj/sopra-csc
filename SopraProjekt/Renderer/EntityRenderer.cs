using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SopraProjekt.Entities.Functional;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// Class to render all entities
    /// </summary>
    internal sealed class EntityRenderer : IRenderer
    {
        private readonly Map mMap;
        // private Rectangle mOldSpace;
        public static Stack<Message> mNpcMessages = new Stack<Message>();
        // private Vector2 mOldCameraPos;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map">Map with all entities</param>
        public EntityRenderer(Map map)
        {
            mMap = map;
        }

        /// <summary>
        /// Draw a given entity
        /// </summary>
        private static void DrawEntity(SpriteBatch spriteBatch, Entity entity, GameTime gameTime)
        {
            Debug.Assert(entity.mTexture != null, "Tried to draw entity without texture");
            // moved the Drawing inside the according classes
            var position = IsoHelper.TwoDToIsometric(entity.mPosition);
            _ = gameTime;
            entity.Draw(spriteBatch, position);
        }

        /// <summary>
        /// Draw all entities in mMap
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Rectangle space, GameTime gameTime)
        {
            var entities = mMap.GetEntitiesIn(space);
            // var sortedEntities = entities.OrderBy(entity => (entity.mPosition.X + entity.mPosition.Y)).ToList();
            var sortedEntities = entities.OrderBy(entity => (entity.mPosition.X + entity.mPosition.Y))
                .ThenBy(entity => entity.mDrawPriority);
            foreach (var entity in sortedEntities)
            {
                DrawEntity(spriteBatch, entity, gameTime);
            }

            var oldStack = new Stack<Message>();
            Globals.Swap(ref mNpcMessages, ref oldStack);
            // Draws the messages from NPCs
            while (oldStack.TryPop(out var message))
            {
                message.mSpeechBubble.Draw(spriteBatch);
                if (gameTime.TotalGameTime.TotalSeconds - message.mFirstRender < Npc.DisplayTime)
                {
                    mNpcMessages.Push(message);
                }
            }
        }
    }
}