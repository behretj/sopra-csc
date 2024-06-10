using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SopraProjekt.Entities;
using SopraProjekt.Entities.Heroes;
using SopraProjekt.Renderer;
using System;
using System.Linq;

#if verboseDebug
    using System.Diagnostics;
#endif

namespace SopraProjekt.Input
{
    /// <summary>
    /// Class to handel the mouse input
    /// </summary>
    internal sealed class MouseInput
    {
        private readonly GameState.GameState mGameState;
        private readonly Camera mCamera;
        private readonly Map mMap;
        public bool mButtonMoveReleased;
        public bool mButtonSelectReleased;
        private bool mActive = true;
        private bool mWasPressed;
        private Point mMousePositionClick;

        private Point mSaveDestination;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameState">game state</param>
        /// <param name="camera">camera</param>
        /// <param name="map">map</param>
        internal MouseInput(GameState.GameState gameState, Camera camera, Map map)
        {
            mGameState = gameState;
            mCamera = camera;
            mMap = map;
        }

        /// <summary>
        /// handel mouse inputs
        /// </summary>
        internal void Update(GameTime gameTime)
        {
            if (!mActive)
            {
                return;
            }

            var state = Mouse.GetState();
            var activeHero = mGameState.ActiveHero;

            HandelLeftMouseButton(state);
            HandleRightMouseButton(state, activeHero, gameTime);
            // removed all mousewheel input
            // HandleMouseWheelButton(state, activeHero);
        }

        /// <summary>
        /// Handel events for the left mouse button
        /// </summary>
        /// <param name="state"></param>
        private void HandelLeftMouseButton(MouseState state)
        {
            switch (state.LeftButton)
            {
                // select different hero
                case ButtonState.Released when !mButtonSelectReleased && !mWasPressed:
                    mButtonSelectReleased = true;
                    mWasPressed = false;
                    break;

                case ButtonState.Pressed: // when mButtonSelectReleased:
#if verboseDebug
                    Debug.WriteLine("Started selection rectangle at:\t" + mCamera.ConvertMouseToMapCoordinates(state.Position.ToVector2()));
#endif
                    mButtonSelectReleased = false;
                    if (!mWasPressed)
                    {
                        mWasPressed = true;
                        mMousePositionClick = mCamera.ConvertMouseToMapCoordinates(Mouse.GetState().Position.ToVector2()).ToPoint();
                        mMousePositionClick = new Point(mMousePositionClick.X - 1, mMousePositionClick.Y);
                    }
                    mWasPressed = true;
                    var selectStartPoint = mMousePositionClick;
                    var selectEndPoint = mCamera.ConvertMouseToMapCoordinates(
                        state.Position.ToVector2()).ToPoint();
                    selectEndPoint = new Point(selectEndPoint.X - 1, selectEndPoint.Y);
                    mMap.mMarkedFields = new Tuple<Point, Point>(selectStartPoint, selectEndPoint);
                    break;



                case ButtonState.Released when (mWasPressed):
                    mMap.mMarkedFields = null;
                    mGameState.FollowingHeroes.Clear();
                    mWasPressed = false;
                    mButtonSelectReleased = false;
                    var position = mCamera.ConvertMouseToMapCoordinates(
                        state.Position.ToVector2()).ToPoint();

                    // build selection rectangle
                    var start = new Point(0, 0);
                    var end = new Point(0, 0);

                    // decide in which direction the selection field is set
                    if (mMousePositionClick.X >= position.X && mMousePositionClick.Y >= position.Y)
                    {
                        start = mMousePositionClick;
                        end = position;
                    }
                    else
                    {
                        if (mMousePositionClick.X <= position.X && mMousePositionClick.Y >= position.Y)
                        {
                            start.X = position.X;
                            start.Y = mMousePositionClick.Y;
                            end.X = mMousePositionClick.X;
                            end.Y = position.Y;
                        }

                        if (mMousePositionClick.X >= position.X && mMousePositionClick.Y <= position.Y)
                        {
                            start.X = mMousePositionClick.X;
                            start.Y = position.Y;
                            end.X = position.X;
                            end.Y = mMousePositionClick.Y;
                        }

                        if (mMousePositionClick.X <= position.X && mMousePositionClick.Y <= position.Y)
                        {
                            start = position;
                            end = mMousePositionClick;
                        }
                    }
                    //Debug.WriteLine(start + "und" + end);
                    var entities = mMap.GetEntitiesIn(new Rectangle(
                        end.X, end.Y,
                        start.X - end.X + 1, start.Y - end.Y + 1));

                    // just one click for choosing one hero
                    if (position.Y == mMousePositionClick.Y && position.X == mMousePositionClick.X)
                    {
                        entities = mMap.GetEntitiesIn(new Rectangle(position, new Point(1, 1)));
                    }

#if verboseDebug
                    Debug.WriteLine("Made selection rectangle from: \t" + mMousePositionClick + " to " + position);
#endif
                    var set = false;
                    //var entities = mMap.GetEntitiesIn(new Rectangle(position, new Point(5, 5)));
                    foreach (var entity in entities)
                    {
                        if (mGameState.ActiveHero == entity)
                        {
                            set = true;
                        }

                        if (!(entity is MovableEntity movableEntity) || movableEntity.mTeam != MovableEntity.OwnTeam)
                        {
                            continue;
                        }

                        if (mGameState.ActiveHero != (Hero)entity)
                        {
                            mGameState.FollowingHeroes.Add((Hero)entity);
                        }
                    }
                    if (!set && mGameState.FollowingHeroes.Count > 0)
                    {
                        mGameState.ActiveHero = mGameState.FollowingHeroes[0];
                        mGameState.FollowingHeroes.Remove(mGameState.ActiveHero);
                    }

                    break;
            }
        }

        /// <summary>
        /// Handle events for the right mouse button
        /// </summary>
        /// <param name="state"></param>
        /// <param name="activeHero"></param>
        /// <param name="gameTime"></param>
        private void HandleRightMouseButton(MouseState state, Entity activeHero, GameTime gameTime)
        {
            var movableHero = (MovableEntity)activeHero;
            switch (state.RightButton)
            {
                case ButtonState.Released when !mButtonMoveReleased:
                    mButtonMoveReleased = true;
                    break;

                case ButtonState.Pressed when mButtonMoveReleased:
                    mButtonMoveReleased = false;
                    // move
                    movableHero.mTargetedEntity = null;
                    movableHero.mAttacking = false;
                    foreach (var followingHeroes in mGameState.FollowingHeroes)
                    {
                        followingHeroes.mTargetedEntity = null;
                        followingHeroes.mAttacking = false;
                    }
                    MouseEntityInteraction();
                    break;
            }

            void MouseEntityInteraction()
            {
                var destination = mCamera.ConvertMouseToMapCoordinates(state.Position.ToVector2()).ToPoint();

                var entities = mMap.GetEntitiesIn(new Rectangle(
                    destination.X, destination.Y, 4, 5));

                foreach (var entity in entities.OfType<MovableEntity>().Where(
                    entity => entity.Title == "monster" || (entity.Title.Substring(0, 4) == "hero" && (entity).mTeam != MovableEntity.OwnTeam)))
                {
                    // follow entity and fight if in attack range
                    movableHero.mAttacking = true;
                    movableHero.Fight(entity, gameTime);
                }

                if (destination != activeHero.mPosition)
                {
                    //while (!mMap.IsSpaceIn(new Rectangle(destination.X, destination.Y, 2, 2), movableHero.mTeam))
                    while (mMap.GetEntity(destination).Count > 0)
                    {
                        destination.X += 1;
                    }
                }

                activeHero.mNextPositions.Clear();
                activeHero.mMovePath.Clear();
                // checking whether the new position is outside of our map and not to near to the old position
                if (destination.X > 0 && destination.Y > 0 && destination.X < mMap.MapSize.X && destination.Y < mMap.MapSize.Y)
                {
                    mSaveDestination = destination;
                    if (activeHero.mNextPositions.Count > 0 && activeHero.mNextPositions.Peek() == activeHero.mPosition.ToVector2())
                    {
                        activeHero.mMovetoField = mSaveDestination;
                        mSaveDestination = Point.Zero;
                    }
                    activeHero.mMovetoField = destination;
                }

                foreach (var entity in mGameState.FollowingHeroes)
                {
                    entity.mNextPositions.Clear();
                    entity.mMovePath.Clear();
                   
         
                    entity.mMovetoField = mMap.FindEmptySpace(destination);
               
                    var saveDestination = false;
                    if (entity.mMovetoField.X > 0 && entity.mMovetoField.Y > 0 && entity.mMovetoField.X < mMap.MapSize.X && entity.mMovetoField.Y < mMap.MapSize.Y)
                    {
                        if (mMap.IsSpaceIn(new Rectangle(entity.mMovetoField, new Point(2, 2)), -1))
                        {
                            saveDestination = true;
                        }
                    }

                    if (!saveDestination)
                    {
                        entity.mMovetoField = mMap.FindEmptySpace(entity.mMovetoField);
                    }

                    if (movableHero.mAttacking)
                    {
                        if (!(movableHero.mTargetedEntity is null))
                        {
                            entity.mAttacking = true;
                            entity.Fight(movableHero.mTargetedEntity, gameTime);
                        }
                    }
                }
            }
        }


        internal static void SpecialSkill(Hero hero, Map map)
        {
            var destination = hero.mPosition;

            switch (hero)
            {
                case Sniper sniper:
                    {
                        var entities = map.GetEntitiesIn(new Rectangle(destination.X - 15, destination.Y - 15, 30, 30)).OfType<MovableEntity>().
                            Where(entity => entity.Title == "monster" || (entity.Title.Substring(0, 4) == "hero" && (entity).mTeam != MovableEntity.OwnTeam)).ToList();
                        if (entities.Count > 0)
                        {
                            sniper.Skill(entities[0]);
                        }

                        break;
                    }
                case Healer healer:
                    {
                        var entities = map.GetEntitiesIn(new Rectangle(destination.X - 30, destination.Y - 30, 60, 60));
                        healer.UseSkill(entities);
                        break;
                    }
                case Tank tank:
                    {
                        var entities = map.GetEntitiesIn(new Rectangle(destination.X - 30, destination.Y - 30, 60, 60));
                        tank.UseSkill(entities);
                        break;
                    }
                case Carry _:
                    hero.Skill(hero);
                    break;
                case Crusher crusher:
                    crusher.Skill(map.GetEntitiesIn(new Rectangle(destination.X - 20, destination.Y - 20, 40, 40)));
                    break;
            }
        }
    }
}
