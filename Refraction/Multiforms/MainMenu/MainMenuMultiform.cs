using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.Effectors;
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Play;
using System;
using System.Collections.Generic;

namespace Refraction_V2.Multiforms.MainMenu
{
    public class MainMenuMultiform : Multiform
    {

        private static readonly Random Random = new Random();

        #region Form Names

        /// <summary>
        /// The name of this multiform.
        /// </summary>
        public const string MultiformName = "MainMenu";

        /// <summary>
        /// The name of the "Play" button.
        /// </summary>
        public const string PlayButtonFormName = "PlayButton";

        /// <summary>
        /// The name of the "Options" button.
        /// </summary>
        public const string OptionsButtonFormName = "OptionsButton";

        /// <summary>
        /// The name of the "Credits" button.
        /// </summary>
        public const string CreditsButtonFormName = "CreditsButton";

        #endregion

        #region Form Properties

        /// <summary>
        /// The center of the "Play" button.
        /// </summary>
        public static readonly Vector2 PlayButtonCenter = DisplayManager.WindowResolution.Center;

        public static readonly GUIButtonInfo PlayButtonInfo = new GUIButtonInfo(
            "PLAY", Assets.MainMenu.Images.PlayButton);

        /// <summary>
        /// The center of the "Options" button.
        /// </summary>
        public static readonly Vector2 OptionsButtonCenter = new Vector2(
            DisplayManager.WindowWidth / 2f,
            DisplayManager.WindowHeight / 2f + 100);

        public static readonly GUIButtonInfo OptionsButtonInfo = new GUIButtonInfo(
            "OPTIONS", Assets.MainMenu.Images.PlayButton);

        /// <summary>
        /// The center of the "Credits" button.
        /// </summary>
        public static readonly Vector2 CreditsButtonCenter = new Vector2(
            DisplayManager.WindowWidth / 2f,
            DisplayManager.WindowHeight / 2f + 200);

        public static readonly GUIButtonInfo CreditsButtonInfo = new GUIButtonInfo(
            "CREDITS", Assets.MainMenu.Images.PlayButton);

        #endregion

        public override void Construct(MultiformTransmissionData args)
        {
            RegisterForm(PlayButtonFormName, new GUIButton(PlayButtonInfo, PlayButtonCenter));
            RegisterForm(OptionsButtonFormName, new GUIButton(OptionsButtonInfo, OptionsButtonCenter));
            RegisterForm(CreditsButtonFormName, new GUIButton(CreditsButtonInfo, CreditsButtonCenter));

            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
        }

        public void Update_Main()
        {
            UpdateForms();
            UpdateTime();

            if (GetForm<GUIButton>(PlayButtonFormName).IsReleased(MouseButtons.Left))
            {
                // Resetting the timer makes it easier to align time based events in the
                // transition-out, since it's easier to let the initial time be zero than
                // it is to be anything else.
                ResetTime();

                // These updaters perform the transition out after clicking the play button.
                SetUpdater(Update_OnPlayButtonClick);
                SetRenderer(Render_OnPlayButtonClick);

                RegisterForm("OnPlayButtonClick_Animation", new PlayAnimationForm(PlayButtonCenter));
                GetForm<GUIButton>(PlayButtonFormName).LockInteraction();
                return;
            }
        }

        private List<AnimatedLaserSegment> TransitionOutLasers = new List<AnimatedLaserSegment>();

        public void Update_OnPlayButtonClick()
        {
            UpdateForms();
            UpdateTime();

            if (AtFrame(1)) 
            {
                GetForm(OptionsButtonFormName).AddEffector(new FadeOutEffector(15));
                GetForm(CreditsButtonFormName).AddEffector(new FadeOutEffector(15));
            }

            if (AtFrame(230))
            {
                Manager.Close(this);
                Manager.Construct(LevelSelectMultiform.MultiformName);
                ClearForms();
            }
        }

        public void Render_Main()
        {
            RenderForms();
        }

        public void Render_OnPlayButtonClick()
        {
            RenderFormsExcept("OnPlayButtonClick_Animation");

            RenderForm("OnPlayButtonClick_Animation");
        }

    }
}
