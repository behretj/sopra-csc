using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Helper;
using SopraProjekt.Renderer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SopraProjekt.ScreenManagement.MenuObjects
{
    internal sealed class MiniMap : IMenuObject
    {
        internal const int EntityPixelSize = 3;
        private static readonly Color sDiscoveredColor = Color.Gray;

        private readonly Map mMap;
        private readonly Camera mCamera;

        private readonly Sprite mBackgroundSprite;
        private readonly Point mPosition;
        private readonly Point mSize;
        private readonly float mScale;
        private bool mDisposedValue;

        private readonly Texture2D mConcreteTexture;
        private readonly Texture2D mDirtTexture;
        private readonly Texture2D mRockTexture;
        private readonly Texture2D mStoneTexture;
        private readonly Texture2D mDiscoveredTexture;

        private readonly List<Entity> mDiscoveredEntities;

        private static readonly List<string> sDrawableEntities = new List<string>()
        {
            "bush",
            "hero_carry",
            "hero_crusher",
            "hero_healer",
            "hero_sniper",
            "hero_tank",
            "brewing_stand",
            "health_fountain",
            "oxygen_source",
            "spaceship"
        };

        private static readonly List<string> sDiscoverableEntities = new List<string>
        {
            "bush"
        };

        internal MiniMap(Map map, Camera camera, ContentLoader content, GraphicsDeviceManager graphics)
        {
            const float mapWidth = 0.15f;
            var screenDimensions =
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            mMap = map;
            mSize = new Point((int)(screenDimensions.Width * mapWidth), (int)(screenDimensions.Width * mapWidth));
            mPosition = new Point((int)(screenDimensions.Width - (screenDimensions.Width * mapWidth)), 0);
            mBackgroundSprite = new Sprite
            {
                mColor = Color.Gray,
                mPosition = mPosition.ToVector2(),
                mTexture = content.LoadTexture("Images/Menu/MapBackground")
            };
            mDiscoveredEntities = new List<Entity>();
            mScale = mMap.MapSize.X / (screenDimensions.Width * mapWidth);
            mCamera = camera;

            mConcreteTexture = new Texture2D(Game1.mGraphics.GraphicsDevice, 1, 1);
            mConcreteTexture.SetData(new[] { Color.AliceBlue });

            mDirtTexture = new Texture2D(Game1.mGraphics.GraphicsDevice, 1, 1);
            mDirtTexture.SetData(new[] { Color.LightSalmon });

            mRockTexture = new Texture2D(Game1.mGraphics.GraphicsDevice, 1, 1);
            mRockTexture.SetData(new[] { Color.Bisque });

            mStoneTexture = new Texture2D(Game1.mGraphics.GraphicsDevice, 1, 1);
            mStoneTexture.SetData(new[] { Color.DarkKhaki });

            mDiscoveredTexture = new Texture2D(Game1.mGraphics.GraphicsDevice, EntityPixelSize, EntityPixelSize);
            mDiscoveredTexture.SetData(new[] {
                sDiscoveredColor,sDiscoveredColor,sDiscoveredColor,
                sDiscoveredColor,sDiscoveredColor,sDiscoveredColor,
                sDiscoveredColor,sDiscoveredColor,sDiscoveredColor
            });
            
            foreach (var entity in mMap.GetEntitiesIn(new Rectangle(0, 0, mMap.MapSize.X, mMap.MapSize.Y)))
            {
                if (entity.mDiscovered)
                {
                    mDiscoveredEntities.Add(entity);
                }
            }
        }

        ~MiniMap()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        private bool ContainsPosition(Vector2 position)
        {
            return mBackgroundSprite.ContainsPosition(position);
        }

        public void HandleInput()
        {
            // TODO: add clicking if asked for
            _ = ContainsPosition(Mouse.GetState().Position.ToVector2());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mBackgroundSprite.Draw(gameTime, spriteBatch, mSize);

            var textureMap = mMap.GroundTextureMap;

            for (var x = mPosition.X; x < mPosition.X + mSize.X; x++)
            {
                for (var y = mPosition.Y; y < mPosition.Y + mSize.Y; y++)
                {
                    var mapCoordinate = new Point(
                        (int)Math.Clamp((x - mPosition.X) * mScale, 0, mMap.MapSize.X - 1),
                        (int)Math.Clamp((y - mPosition.Y) * mScale, 0, mMap.MapSize.Y - 1));
                    var pixel = textureMap[mapCoordinate.X, mapCoordinate.Y] switch
                    {
                        Map.EMapTexture.Concrete => mConcreteTexture,
                        Map.EMapTexture.Dirt => mDirtTexture,
                        Map.EMapTexture.Rock => mRockTexture,
                        Map.EMapTexture.Stone => mStoneTexture,
                        _ => null
                    };
                    spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), Color.White);
                }
            }

            foreach (var entity in mDiscoveredEntities)
            {
                var coordinate = mPosition + (entity.mPosition.ToVector2() * new Vector2(1 / mScale)).ToPoint();
                Debug.Assert(entity.MiniMapPixel != null, "entity.MiniMapPixel == null on drawing");
                spriteBatch.Draw(mDiscoveredTexture, new Rectangle(coordinate.X, coordinate.Y, EntityPixelSize, EntityPixelSize), Color.White);
            }

            foreach (var entity in mMap.GetEntitiesIn(mCamera.VisiblePart).Where(entity => sDrawableEntities.Contains(entity.Title)))
            {
                if (sDiscoverableEntities.Contains(entity.Title) && !mDiscoveredEntities.Contains(entity))
                {
                    mDiscoveredEntities.Add(entity);
                    entity.mDiscovered = true;
                }

                var coordinate = mPosition + (entity.mPosition.ToVector2() * new Vector2(1 / mScale)).ToPoint();
                Debug.Assert(entity.MiniMapPixel != null, "entity.MiniMapPixel == null on drawing");
                spriteBatch.Draw(entity.MiniMapPixel, new Rectangle(coordinate.X, coordinate.Y, EntityPixelSize, EntityPixelSize), Color.White);
            }
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