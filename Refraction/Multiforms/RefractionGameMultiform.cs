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
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Refraction_V2.Multiforms.ForegroundContent;

using System;

#endregion

namespace Refraction_V2.Multiforms
{

    /// <summary>
    /// The base type for multiforms in this game. This class contains a bunch of
    /// utility methods and properties common to all multiforms in this game.
    /// </summary>
    public abstract class RefractionGameMultiform : Multiform
    {

        #region Fade-In and Fade-Out Related Fields

        /// <summary>
        /// The alpha value of the fade-in/out overlay.
        /// </summary>
        private float FadeAlpha = 0f;

        /// <summary>
        /// The duration of the current fade-in/out.
        /// </summary>
        private int FadeDuration = 0;

        /// <summary>
        /// The colour of the current fade-in/out overlay.
        /// </summary>
        private Color FadeColour = Color.White;

        // The updater/renderer to use whilst fading in/out, and the updater/renderer
        // to switch to after the fade in is complete.
        private Action updater, renderer, updaterWhenFinished, rendererWhenFinished;

        /// <summary>
        /// The name of the multiform to construct after a fade out.
        /// </summary>
        private string multiformToConstruct;

        /// <summary>
        /// The data to send to the next multiform after a fade out.
        /// </summary>
        private MultiformTransmissionData transmissionData;

        private bool clearForms;

        #endregion

        protected void FadeIn(
            int duration, Color colour, Action updater, Action renderer,
            Action updaterWhenFinished = null,
            Action rendererWhenFinished = null)
        {
            FadeAlpha    = 1f;
            FadeDuration = duration;
            FadeColour   = colour;

            if (updater == null || renderer == null)
            {
                throw new ArgumentNullException("Updater and Renderer cannot be null.");
            }
            this.updater              = updater;
            this.renderer             = renderer;
            this.updaterWhenFinished  = updaterWhenFinished;
            this.rendererWhenFinished = rendererWhenFinished;

            SetUpdater(Update_FadeIn);
            SetRenderer(Render_Fade);

            Manager.GetActiveMultiform<ForegroundContentMultiform>(
                ForegroundContentMultiform.MultiformName).UnhideCursor();
        }

        protected void FadeOutAndClose(
            int duration, Color colour, string multiformName, 
            MultiformTransmissionData data = null, bool clearForms = true,
            Action backgroundUpdater = null, Action backgroundRenderer = null,
            bool hideMouseTrail = true)
        {
            FadeAlpha    = 0f;
            FadeDuration = duration;
            FadeColour   = colour;

            multiformToConstruct = multiformName;
            transmissionData     = data;
            this.clearForms      = clearForms;

            updater  = backgroundUpdater;
            renderer = backgroundRenderer;

            SetUpdater(Update_FadeOut);
            SetRenderer(Render_Fade);

            if (hideMouseTrail)
            {
                // Hide the mouse cursor trail. The way the trail is rendered makes it
                // look strange on top of backgrounds of certain colours.
                Manager.GetActiveMultiform<ForegroundContentMultiform>(ForegroundContentMultiform.MultiformName)
                       .GetForm<MouseTrailForm>(ForegroundContentMultiform.MouseTrailFormName).Hide();
            }
        }

        protected void Update_FadeIn()
        {
            if (FadeAlpha <= 0f)
            {
                SetUpdater(updaterWhenFinished == null ? updater : updaterWhenFinished);
                SetRenderer(rendererWhenFinished == null ? renderer : rendererWhenFinished);
            }

            FadeAlpha -= 1f / FadeDuration;

            updater();
        }

        protected void Update_FadeOut()
        {
            if (FadeAlpha >= 1.5f)
            {
                Manager.Close(this);
                Manager.Construct(multiformToConstruct, transmissionData);

                if (clearForms)
                {
                    ClearForms();
                }

                FadeAlpha = 0f;
            }

            FadeAlpha += 1f / FadeDuration;

            if (updater != null)
            {
                updater();
            }
        }

        private void Render_Fade()
        {
            if (renderer != null)
            {
                renderer();
            }

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            var dummyTexture = new Texture2D(DisplayManager.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { FadeColour });

            var color = new Color(Color.White, MathHelper.Clamp((float)Math.Pow(FadeAlpha, 1.6), 0, 1));
            DisplayManager.Draw(
                dummyTexture, Vector2.Zero, null, color, 0f, Vector2.Zero,
                new Vector2(DisplayManager.WindowWidth, DisplayManager.WindowHeight),
                SpriteEffects.None, 0f);

            DisplayManager.ClearSpriteBatchProperties();
        }
    }
}
