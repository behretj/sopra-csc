using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using System;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SopraProjekt.Renderer
{
    /// <summary>
    /// The camera class handles the position and zoom of the camera 
    /// </summary>
    public sealed class Camera
    {
        // TODO: change constants to fitting values
        private const float PositionXLowerLimit = -7000;
        private const float PositionXUpperLimit = 17200;
        private const float PositionYLowerLimit = 0;
        private const float PositionYUpperLimit = 12500;
        public const float ZoomLowerLimit = 0.6f;
        private const float ZoomUpperLimit = 2f;
        private const float ZoomStep = .007f;

        private Vector2 mPosition;
        public Vector2 Position
        {
            get => mPosition;
            private set
            {
                mPosition.X = Math.Clamp(value.X, PositionXLowerLimit, PositionXUpperLimit);
                mPosition.Y = Math.Clamp(value.Y, PositionYLowerLimit, PositionYUpperLimit);
            }
        }

        private float mZoom;
        public float Zoom
        {
            get => mZoom;
            private set => mZoom = Math.Clamp(value, ZoomLowerLimit, ZoomUpperLimit);
        }

        private int mPreviousScrollValue;

        internal Rectangle VisiblePart
        {
            get
            {
                const float screenScaleFixFactor = 1.4f;
                var screenSize = IsoHelper.IsometricToTwoD(new Point(
                    Game1.mGraphics.PreferredBackBufferWidth / IsoHelper.sTileSize.X,
                    Game1.mGraphics.PreferredBackBufferHeight / IsoHelper.sTileSize.Y).ToVector2()) * new Vector2(screenScaleFixFactor / Zoom, screenScaleFixFactor / Zoom);
                screenSize.X = Math.Abs(screenSize.X);
                screenSize.Y = Math.Abs(screenSize.Y);

                var topLeft = IsoHelper.IsometricToTwoD(mPosition) - new Vector2(
                    (int)(Game1.mGraphics.PreferredBackBufferWidth / ((IsoHelper.sTileSize.X / screenScaleFixFactor) * Zoom)),
                    (int)(Game1.mGraphics.PreferredBackBufferHeight / ((IsoHelper.sTileSize.Y / screenScaleFixFactor) * Zoom))
                    );

                topLeft.X = Math.Clamp(topLeft.X, 0, mMap.MapSize.X);
                topLeft.Y = Math.Clamp(topLeft.Y, 0, mMap.MapSize.Y);

                screenSize.X = Math.Clamp(screenSize.X, 0, mMap.MapSize.X - topLeft.X);
                screenSize.Y = Math.Clamp(screenSize.Y, 0, mMap.MapSize.Y - topLeft.Y);

                return new Rectangle(topLeft.ToPoint(), screenSize.ToPoint());
            }
        }


        internal Matrix TransformMatrix { get; private set; }

        private readonly Map mMap;
        // set true to move camera freely with error keys
        private bool mDebug;
        private bool mDebugButtonDebounce = true;

        /// <summary>
        /// Constructor
        /// </summary>
        internal Camera(Map map)
        {
            mPosition = IsoHelper.TwoDToIsometric(Vector2.Zero);
            Zoom = ZoomLowerLimit;
            mMap = map;
            mPreviousScrollValue = Mouse.GetState().ScrollWheelValue;
        }

        /// <summary>
        /// Move the camera
        /// </summary>
        /// <param name="value">direction and amount (absolute, per millisecond) to move the camera
        /// or position to move the camera on (dependent on relative</param>
        /// <param name="gameTime">game time</param>
        /// <param name="relative">if true, change the camera position relative to the current position,
        /// if false, set the camera to the given position</param>
        private void Move(Vector2 value, GameTime gameTime, bool relative = true)
        {
            if (relative)
            {
                Position += (value * gameTime.ElapsedGameTime.Milliseconds);
            }
            else
            {
                Position = value;
            }
        }

        /// <summary>
        /// Change the zoom of the camera 
        /// </summary>
        /// <param name="value">Amount of zoom to change (per millisecond)</param>
        /// <param name="gameTime">game time</param>
        private void ChangeZoom(float value, GameTime gameTime)
        {
            Zoom += (value * gameTime.ElapsedGameTime.Milliseconds);
        }

        /// <summary>
        /// Update the TransformMatrix
        /// </summary>
        private void UpdateTransformMatrix()
        {
            const float shiftHalfScreenFactor = 0.5f;
            TransformMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                              Matrix.CreateScale(Zoom) *
                              Matrix.CreateScale(Game1.mGraphics.PreferredBackBufferWidth / 1920f) *
                              Matrix.CreateTranslation(new Vector3(Game1.mGraphics.PreferredBackBufferWidth * shiftHalfScreenFactor,
                                  Game1.mGraphics.PreferredBackBufferHeight * shiftHalfScreenFactor, 0));
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        internal void Update(GameTime gameTime, Vector2 position)
        {
            if (mDebug)
            {
                Movement(gameTime);
            }
            else
            {
                Move(IsoHelper.TwoDToIsometric(position),
                    gameTime, false);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1) && mDebugButtonDebounce)
            {
                mDebug = !mDebug;
                mDebugButtonDebounce = false;
                Debug.WriteLine(mDebug ? "Activate camera debug mode" : "Deactivate camera debug mode");
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F1))
            {
                mDebugButtonDebounce = true;
            }

            Zooming(gameTime);

            UpdateTransformMatrix();
        }


        /// <summary>
        /// Move the camera in debug mode
        /// </summary>
        /// <param name="gameTime"></param>
        private void Movement(GameTime gameTime)
        {
            var cameraMovement = Vector2.Zero;
            const int moveSpeed = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                cameraMovement.Y = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                cameraMovement.Y = moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                cameraMovement.X = -moveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                cameraMovement.X = moveSpeed;
            }
            Move(cameraMovement, gameTime);
        }

        /// <summary>
        /// Zoom the camera
        /// </summary>
        /// <param name="gameTime"></param>
        private void Zooming(GameTime gameTime)
        {
            var scrollValue = Mouse.GetState().ScrollWheelValue;
            if (scrollValue < mPreviousScrollValue)
            {
                ChangeZoom(-ZoomStep, gameTime);
            }
            else if (scrollValue > mPreviousScrollValue)
            {
                ChangeZoom(ZoomStep, gameTime);
            }

            mPreviousScrollValue = scrollValue;
        }

        /// <summary>
        /// Converts the screen coordinates into the map coordinates
        /// </summary>
        /// <param name="mouseCoordinates">Coordinates where the mouse is on the screen</param>
        /// <returns>map coordinates</returns>
        public Vector2 ConvertMouseToScreenCoordinates(Vector2 mouseCoordinates)
        {
            Vector3 helperVector = new Vector3(mouseCoordinates.X, mouseCoordinates.Y, 1);
            helperVector = Vector3.Transform(helperVector, TransformMatrix);
            Vector2 mapCoordinates = new Vector2(helperVector.X, helperVector.Y);
            return mapCoordinates;
        }

        /// <summary>
        /// Converts the screen coordinates into the map coordinates
        /// </summary>
        /// <param name="mouseCoordinates">Coordinates where the mouse is on the screen</param>
        /// <returns>map coordinates</returns>
        public Vector2 ConvertMouseToMapCoordinates(Vector2 mouseCoordinates)
        {
            Matrix invertedTransformMatrix = Matrix.Invert(TransformMatrix);
            Vector3 helperVector = new Vector3(mouseCoordinates.X, mouseCoordinates.Y, 1);
            helperVector = Vector3.Transform(helperVector, invertedTransformMatrix);
            Vector2 mapCoordinates = new Vector2(helperVector.X, helperVector.Y);

            // Everything is offset by half a square in X direction
            mapCoordinates.X -= IsoHelper.sTileSize.X / 2f;

            mapCoordinates = IsoHelper.IsometricToTwoD(mapCoordinates);
            return mapCoordinates;
        }
    }
}
