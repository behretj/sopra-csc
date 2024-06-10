using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SopraProjekt.Renderer;
using System;

namespace SopraProjekt.ScreenManagement
{
    public interface IDrawAble : IDisposable
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }

    public interface IClickAble : IDisposable
    {
        void HandleInput();
    }

    internal interface IMenuObject : IDrawAble, IClickAble
    {
    }

    internal interface IGameScreen : IMenuObject
    {
        bool IsPaused { get; }
        bool IsIsometric { get; }
        Camera Camera { get; }
        void Pause();
        void Resume();
        void Update(GameTime gameTime);
        void ChangeBetweenScreens();
    }

    /// enum represents the current state of our button
    internal enum MenuObjectState
    {
        //Button is in default state
        None,
        //Button is hovered on
        Hovered,
        //Button has been clicked
        Clicked
    }
}
