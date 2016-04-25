#region License

// Copyright (c) 2015 FCDM
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region Header

/* Author: Michael Ala
 * 
 * Description
 * ===========
 * The game kernel is a wrapper for monogame's Game class, which ensures all things
 * game related get updated accordingly without interference or mishap. Initialization
 * of the game and it's multiforms happens here.
 */

#endregion

#region Using Statements

using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

#endregion

namespace DemeterEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public abstract class GameKernel : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The path of the setup file.
        /// </summary>
        public string SetupFileName { get; private set; }

        /// <summary>
        /// The main game setup reader.
        /// </summary>
        private GameSetupReader GameSetup;

        protected MultiformManager MultiformManager;

        public GameKernel(string setupFileName)
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            SetupFileName = setupFileName;
            GameSetup = new GameSetupReader(setupFileName);
            GameSetup.ReadAll();

            Content.RootDirectory = GameSetup.ContentFolder;
        }

        sealed protected override void Initialize()
        {
            base.Initialize();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ArtManager.Initialize(Content);
            DisplayManager.Initialize(this, graphics, spriteBatch, GameSetup);
            GlobalGameTimer.Initialized = true;

            MultiformManager = new MultiformManager();

            InitializeGame();
            LoadMultiforms();
        }

        sealed protected override void LoadContent() { }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        sealed protected override void UnloadContent() 
        {
            ArtManager.Unload();
        }

        public virtual void InitializeGame() { }

        public abstract void LoadMultiforms();

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        sealed protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            GlobalGameTimer.GameTime = gameTime;
            GlobalGameTimer.DeltaTime = Math.Max(
                GlobalGameTimer.MIN_DELTA_TIME, 
                gameTime.ElapsedGameTime.Milliseconds
                );

            MouseInput.Update();
            KeyboardInput.Update();

            MultiformManager.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        sealed protected override void Draw(GameTime gameTime)
        {
            DisplayManager.BeginRender();

            MultiformManager.Render();

            DisplayManager.EndRender();

            base.Draw(gameTime);
        }
    }
}
