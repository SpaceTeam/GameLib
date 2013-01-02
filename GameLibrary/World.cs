﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Dependencies.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Helpers;
using GameLibrary.Entities.Systems;

namespace GameLibrary
{
    /// <summary>
    /// The world class for all world/games
    /// </summary>
    public abstract class World : EntityWorld
    {
        #region Constructors
        /// <summary>
        /// Constructs a world with a specified gravity.
        /// </summary>
        /// <param name="gravity">The gravity of a world.</param>
        /// <param name="camera"> The camera which views the world.</param>
        /// <param name="spriteBatch">The spritebatch to which the world may render.</param>
        public World(Camera camera, SpriteBatch spriteBatch, Vector2 gravity)
            : base(gravity)
        {
            this.Camera = camera;
            this.SpriteBatch = spriteBatch;
        }

        /// <summary>
        /// Constructs a world with no gravity.
        /// </summary>
        public World(Camera camera, SpriteBatch spriteBatch)
            : this(camera, spriteBatch, Vector2.Zero)
        {
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the world.
        /// </summary>
        public virtual void Initialize()
        {
            this.BuildSystems();
            SystemManager.InitializeAll();
        }

        /// <summary>
        /// Loads the world's content.
        /// </summary>
        /// <param name="Content"></param>
        public virtual void LoadContent(ContentManager Content, params object[] args)
        {
#if DEBUG   //Debug render system
            this._DebugRenderSystem.LoadContent(SpriteBatch.GraphicsDevice, Content);
#endif

            //Builds templates in world.

            this.BuildTemplates(Content, args);
            this.BuildEntities(Content, args);
        }

        #region Building
        /// <summary>
        /// Builds all of the systems.
        /// </summary>
        protected virtual void BuildSystems()
        {
            //Default Systems
            _MovementSystem = this.SystemManager.SetSystem(new ParticleMovementSystem(), ExecutionType.Update);
#if DEBUG
            _DebugRenderSystem = this.SystemManager.SetSystem(new DebugRenderSystem(this.Camera), ExecutionType.Draw, 0);
#endif
            _RenderSystem = this.SystemManager.SetSystem(new RenderSystem(SpriteBatch), ExecutionType.Draw, 1);
        }

        /// <summary>
        /// Builds all of the templates in the world.
        /// </summary>
        protected virtual void BuildTemplates(ContentManager Content, params object[] args)
        {

        }

        /// <summary>
        /// Builds all of the entities in the world (initially)
        /// </summary>
        protected virtual void BuildEntities(ContentManager Content, params object[] args)
        {
        }


        #endregion

        #endregion

        #region Functioning Loop
        /// <summary>
        /// Updates the world
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            this.LoopStart(); //Start the functioning loop
            {
                this.Delta = (int)gameTime.ElapsedGameTime.TotalMilliseconds; //set change in time.
                this.Step((float)gameTime.ElapsedGameTime.TotalSeconds); //Update physical world.
                this.SystemManager.UpdateSynchronous(ExecutionType.Update); //Update the entity world.
                Camera.Update(gameTime); //Finally update the camera.
            }
        }

        /// <summary>
        /// Dtaws the world with some gameTime and spriteBatch
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
            //Render all of the systems (entities)
            this.SystemManager.UpdateSynchronous(ExecutionType.Draw);
        }
        #endregion

        #region Fields

        public Camera Camera;
        protected SpriteBatch SpriteBatch;

        //Systems
        protected RenderSystem _RenderSystem;
        protected ParticleMovementSystem _MovementSystem;
        protected DebugRenderSystem _DebugRenderSystem;

        #endregion
    }
}
