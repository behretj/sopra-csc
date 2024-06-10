using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Renderer;
using SopraProjekt.ScreenManagement.MenuObjects;
using System;
using System.Collections.Generic;

namespace SopraProjekt.Entities
{
    public sealed class Message
    {
        public readonly SpeechBubble mSpeechBubble;
        public readonly double mFirstRender;



        public Message(Point position,
            string[] message,
            double firstRender,
            Tuple<bool, bool> buttons,
            Hero hero,
            Entity source,
            Camera camera,
            Dictionary<string, Texture2D> textures,
            SpriteFont font)
        {
            mSpeechBubble = new SpeechBubble(
                IsoHelper.TwoDToIsometric(position.ToVector2()),
                message,
                buttons,
                hero,
                source,
                camera,
                textures,
                font);
            mFirstRender = firstRender;
        }
    }
}