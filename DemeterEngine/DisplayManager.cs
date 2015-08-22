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
 * A singleton encapsulating all data regarding the game's display. This includes the
 * window form, it's resolution, the sprite batch, the graphics device, and the graphics
 * device manager. Updating the window properties and rendering objects all happens here.
 */

#endregion

#region Using Statements

using DemeterEngine.Maths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Windows.Forms;

#endregion

namespace DemeterEngine
{
    public class DisplayManager
    {

		// The amounts by which the display area is offset by the window border.
        public const int WINDOW_BORDER_OFFSET_X = 8;
        public const int WINDOW_BORDER_OFFSET_Y = 30;

        private DisplayManager() { }

        private static DisplayManager Instance = new DisplayManager();

        /// <summary>
        /// The global game object.
        /// </summary>
        private Game game;

        /// <summary>
        /// The global game GraphicsDevice.
        /// </summary>
        private GraphicsDevice graphicsDevice;
        public static GraphicsDevice GraphicsDevice { get { return Instance.graphicsDevice; } }

        /// <summary>
        /// The global game GraphicsDeviceManager.
        /// </summary>
        private GraphicsDeviceManager graphicsManager;
        public static GraphicsDeviceManager GraphicsManager { get { return Instance.graphicsManager; } }

        /// <summary>
        /// The global game SpriteBatch.
        /// </summary>
        private SpriteBatch spriteBatch;
        public static SpriteBatch SpriteBatch { get { return Instance.spriteBatch; } }

        /// <summary>
        /// The main game window.
        /// </summary>
        private GameWindow window;
        public static GameWindow Window { get { return Instance.window; } }

        private GameSetupReader GameSetup;

        /// <summary>
        /// A flag indicating when the screen's properties have changed.
        /// </summary>
        private bool dirty = true;

        /// <summary>
        /// Whether or not spriteBatch.Begin has been called.
        /// </summary>
        private bool spriteBatchBegun = false;

        /// <summary>
        /// Whether or not DisplayManager.BeginRender has been called.
        /// </summary>
        private bool begun = false;

        private bool initialized = false;

        /// <summary>
        /// Whether or not the screen is borderless.
        /// </summary>
        private bool borderless = false;
        public static bool Borderless { get { return Instance.borderless; } }

        /// <summary>
        /// Whether or not the display is fullscreen.
        /// </summary>
        private bool fullscreen = false;
        public static bool Fullscreen { get { return Instance.fullscreen; } }

        /// <summary>
        /// Whether or not the mouse is visible.
        /// </summary>
        private bool mouseVisible = false;
        public static bool MouseVisibile { get { return Instance.mouseVisible; } }

        private Resolution windowResolution;
        public static Resolution WindowResolution { get { return Instance.windowResolution; } }

        public static int WindowWidth { get { return WindowResolution.Width; } }

        public static int WindowHeight { get { return WindowResolution.Height; } }

        public static Rectangle WindowRect
        {
            get
            {
                var bounds = Instance.window.ClientBounds;
                var pos = ((Form)Control.FromHandle(Instance.window.Handle)).Location;
                return new Rectangle(
                    pos.X, pos.Y, bounds.Width, bounds.Height);
            }
        }

        public static Vector2 WindowTopLeftCorner
        {
            get
            {
                return VectorUtils.FromPoint(
                    ((Form)Control.FromHandle(Instance.window.Handle)).Location
                    );
            }
        }

        public static void Initialize(
            Game game, GraphicsDeviceManager graphicsManager,
            SpriteBatch spriteBatch, GameSetupReader gameSetup)
        {
            Instance.game = game;
            Instance.graphicsDevice = game.GraphicsDevice;
            Instance.graphicsManager = graphicsManager;
            Instance.spriteBatch = spriteBatch;
            Instance.window = game.Window;
            Instance.GameSetup = gameSetup;

            Instance.windowResolution = gameSetup.WindowResolution;
            Instance.borderless = gameSetup.BorderlessOnStartup;
            Instance.fullscreen = gameSetup.FullscreenOnStartup;
            Instance.mouseVisible = gameSetup.MouseVisible;

            Instance.initialized = true;

            Instance.ReinitScreenProperties();
        }

        private static DisplayManagerException UninitializedException(string op)
        {
            return new DisplayManagerException(
                String.Format("Invalid operation '{0}' to perform before initialization.", op)
                );
        }

        /// <summary>
        /// Initialize the properties of the screen form. These properties are whether or not the
        /// screen is borderless, whether or not it's fullscreen, its position on the screen,
        /// its width and its height.
        /// </summary>
        private void ReinitScreenProperties()
        {
            if (!initialized)
                throw UninitializedException("ReinitScreenProperties");
            game.IsMouseVisible = mouseVisible;
            window.IsBorderless = borderless;

            graphicsManager.IsFullScreen = fullscreen;
            graphicsManager.PreferredBackBufferWidth = windowResolution.Width;
            graphicsManager.PreferredBackBufferHeight = windowResolution.Height;

            // Center the display based on the native resolution.
            var form = (Form)Control.FromHandle(window.Handle);
            var position = new System.Drawing.Point(
                (Resolution.Native.Width - windowResolution.Width) / 2,
                (Resolution.Native.Height - windowResolution.Height) / 2
                );

            // This offset seems to width and height of the windows border,
            // so it accounts for the slight off-centering (unless the window
            // is larger than the native display).
            if (!borderless)
            {
                position.X -= WINDOW_BORDER_OFFSET_X;
                position.Y -= WINDOW_BORDER_OFFSET_Y;
            }

            graphicsManager.ApplyChanges();

            /* We have to reposition the form after we apply changes to the graphics manager
             * otherwise if the user changes the resolution while in fullscreen, then goes into
             * windowed mode, the window would be position relative to the previous resolution
             * rather than the new resolution.
             */
            form.Location = position;

            dirty = false;
        }

        private static DisplayManagerException UntogglableException(string property, string element)
        {
            return new DisplayManagerException(
                String.Format(
                    "{0} is not set to be togglable. To change this, add a " +
                    "{1} element to the root element of the .gamesetup file.",
                    property, element
                    )
                );
        }

        public static void ToggleFullscreen()
        {
            if (!Instance.initialized)
                throw UninitializedException("ToggleFullscreen");
            if (!Instance.GameSetup.FullscreenTogglable)
                throw UntogglableException("Fullscreen", GameSetupReader.ALLOW_FULLSCREEN_TOGGLE_ELEMENT);
            Instance.fullscreen ^= true;
            Instance.dirty = true;
        }

        public static void ToggleMouseVisibility()
        {
            if (!Instance.initialized)
                throw UninitializedException("ToggleMouseVisibility");
            if (!Instance.GameSetup.MouseVisibilityTogglable)
                throw UntogglableException("Mouse visibility", GameSetupReader.ALLOW_MOUSE_VISIBILITY_TOGGLE_ELEMENT);
            Instance.mouseVisible ^= true;
            Instance.dirty = true;
        }

        public static void ToggleBorderlessness()
        {
            if (!Instance.initialized)
                throw UninitializedException("ToggleBorderlessness");
            if (!Instance.GameSetup.BorderlessnessTogglable)
                throw UntogglableException("Borderlessness", GameSetupReader.ALLOW_BORDERLESS_TOGGLE_ELEMENT);
            Instance.borderless ^= true;
            Instance.dirty = true;
        }

        /// <summary>
        /// Apply changes to the display and prepare for rendering.
        /// </summary>
        public static void BeginRender()
        {
            if (!Instance.initialized)
                throw UninitializedException("BeginRender");
            if (Instance.dirty)
                Instance.ReinitScreenProperties();
            Instance.graphicsDevice.Clear(Instance.GameSetup.BackgroundFillColour);
            Instance.begun = true;
        }

        /// <summary>
        /// Finish the render phase.
        /// </summary>
        public static void EndRender()
        {
            if (!Instance.initialized)
                throw UninitializedException("EndRender");
            if (!Instance.begun)
                throw new DisplayManagerException("EndRender called before BeginRender.");
            if (Instance.spriteBatchBegun)
            {
                Instance.spriteBatch.End();
                Instance.spriteBatchBegun = false;
            }
            Instance.begun = false;
        }

        /// <summary>
        /// Set new properties on the SpriteBatch.
        /// </summary>
        /// <param name="sortMode"></param>
        /// <param name="blendState"></param>
        /// <param name="samplerState"></param>
        /// <param name="depthStencilState"></param>
        /// <param name="rasterizerState"></param>
        /// <param name="effect"></param>
        /// <param name="transformMatrix"></param>
        public static void SetSpriteBatchProperties(
            SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            if (!Instance.initialized)
                throw UninitializedException("SetSpriteBatchProperties");
            if (!Instance.begun)
                throw new DisplayManagerException("Cannot set sprite batch properties before call to BeginRender.");
            if (Instance.spriteBatchBegun)
                Instance.spriteBatch.End();

            Instance.spriteBatch.Begin(
                sortMode, blendState, samplerState,
                depthStencilState, rasterizerState,
                effect, transformMatrix
                );
            Instance.spriteBatchBegun = true;
        }

        public static void ClearSpriteBatchProperties()
        {
            if (!Instance.initialized)
                throw UninitializedException("ClearSpriteBatchProperties");
            if (!Instance.begun)
                throw new DisplayManagerException("Cannot clear sprite batch properties before call to BeginRender.");
            if (Instance.spriteBatchBegun)
            {
                Instance.spriteBatch.End();
                Instance.spriteBatchBegun = false;
            }
        }

        private static void CheckSpriteBatchBegun()
        {
            if (!Instance.initialized)
                throw UninitializedException("Draw");
            if (!Instance.begun)
                throw new DisplayManagerException("Cannot draw outside of render cycle.");
            if (!Instance.spriteBatchBegun)
            {
                Instance.spriteBatch.Begin();
                Instance.spriteBatchBegun = true;
            }
        }

        public static void Draw(
            Texture2D texture,
            Rectangle destinationRectangle,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(texture, destinationRectangle, color);
        }

        public static void Draw(
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public static void Draw(
            Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(
                texture, destinationRectangle, sourceRectangle, 
                color, rotation, origin, effects, layerDepth
                );
        }

        public static void Draw(
            Texture2D texture,
            Vector2 position,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(texture, position, color);
        }

        public static void Draw(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(texture, position, sourceRectangle, color);
        }

        public static void Draw(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(
                texture, position, sourceRectangle, color, 
                rotation, origin, scale, effects, layerDepth
                );
        }

        public static void Draw(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.Draw(
                texture, position, sourceRectangle, color, 
                rotation, origin, scale, effects, layerDepth
                );
        }

        public static void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(spriteFont, text, position, color);
        }

        public static void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(
                spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth
                );
        }

        public void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(
                spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth
                );
        }

        public static void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(spriteFont, text, position, color);
        }

        public void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(
                spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth
                );
        }

        public void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth)
        {
            CheckSpriteBatchBegun();
            Instance.spriteBatch.DrawString(
                spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth
                );
        }
    }
}
