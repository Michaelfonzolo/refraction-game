using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Refraction_V2.Multiforms
{
    public abstract class RefractionGameMultiform : Multiform
    {

        private float FadeAlpha = 0f;

        private int FadeDuration = 0;

        private Color FadeColour = Color.White;

        private Action updater, renderer, updaterWhenFinished, rendererWhenFinished;

        private string multiformToConstruct;

        private MultiformTransmissionData transmissionData;

        private bool clearForms;

        protected void FadeIn(
            int duration, Color colour, Action updater, Action renderer,
            Action updaterWhenFinished = null,
            Action rendererWhenFinished = null)
        {
            FadeAlpha = 1f;
            FadeDuration = duration;
            FadeColour = colour;

            if (updater == null || renderer == null)
            {
                throw new ArgumentNullException("Updater and Renderer cannot be null.");
            }
            this.updater = updater;
            this.renderer = renderer;
            this.updaterWhenFinished = updaterWhenFinished;
            this.rendererWhenFinished = rendererWhenFinished;

            SetUpdater(Update_FadeIn);
            SetRenderer(Render_Fade);
        }

        protected void FadeOutAndClose(
            int duration, Color colour, string multiformName, 
            MultiformTransmissionData data = null, bool clearForms = true,
            Action backgroundUpdater = null, Action backgroundRenderer = null)
        {
            FadeAlpha = 0f;
            FadeDuration = duration;
            FadeColour = colour;

            multiformToConstruct = multiformName;
            transmissionData = data;
            this.clearForms = clearForms;

            updater = backgroundUpdater;
            renderer = backgroundRenderer;

            SetUpdater(Update_FadeOut);
            SetRenderer(Render_Fade);
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
