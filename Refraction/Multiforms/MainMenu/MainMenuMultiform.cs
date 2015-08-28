using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.Effectors;
using Refraction_V2.Multiforms.ForegroundContent;
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Credits;
using Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Options;
using Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Play;
using System;
using System.Collections.Generic;

namespace Refraction_V2.Multiforms.MainMenu
{
    public class MainMenuMultiform : RefractionGameMultiform
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

        public const string OnPlayButtonClickAnimationFormName = "OnPlayButtonClick_Animation";

        public const string OnOptionsButtonClickAnimationFormName = "OnOptionsButtonClick_Animation";

        public const string OnCreditsButtonClickAnimationFormName = "OnCreditsButtonClick_Animation";

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

            RegisterForm(new ClickParticleSpawnerForm());

            if (args != null && args.SenderName == LevelSelectMultiform.MultiformName)
            {
                FadeIn(20, Color.White, Update_Main, Render_Main);
            }
            else
            {
                SetUpdater(Update_Main);
                SetRenderer(Render_Main);
            }
        }

        public void Update_Main()
        {
            UpdateForms();
            UpdateTime();

            if (GetForm<GUIButton>(PlayButtonFormName).IsReleased(MouseButtons.Left))
            {
                PrepareOnButtonClick(
                    Update_OnPlayButtonClick, Render_OnPlayButtonClick,
                    OnPlayButtonClickAnimationFormName, new PlayAnimationForm(PlayButtonCenter),
                    1, 1, PlayButtonFormName, OptionsButtonFormName, CreditsButtonFormName);
            }

            else if (GetForm<GUIButton>(OptionsButtonFormName).IsReleased(MouseButtons.Left))
            {
                PrepareOnButtonClick(
                    Update_OnOptionsButtonClick, Render_OnOptionsButtonClick,
                    OnOptionsButtonClickAnimationFormName, new OptionsAnimationForm(PlayButtonCenter),
                    20, 2.2f, OptionsButtonFormName, PlayButtonFormName, CreditsButtonFormName);
            }

            else if (GetForm<GUIButton>(CreditsButtonFormName).IsReleased(MouseButtons.Left))
            {
                PrepareOnButtonClick(
                    Update_OnCreditsButtonClick, Render_OnCreditsButtonClick,
                    OnCreditsButtonClickAnimationFormName, new CreditsAnimationForm(PlayButtonCenter),
                    25, 1.8f, CreditsButtonFormName, PlayButtonFormName, OptionsButtonFormName);
            }
        }

        private void PrepareOnButtonClick(
            Action updater, Action renderer, string animationName, Form animation, 
            int floatDuration, float floatTension, string mainButton, params string[] otherButtons)
        {
            // Resetting the timer makes it easier to align time based events in the
            // transition-out, since it's easier to let the initial time be zero than
            // it is to be anything else.
            ResetTime();

            SetUpdater(updater);
            SetRenderer(renderer);

            RegisterForm(animationName, animation);
            GetForm(mainButton).AddEffector(
                new FloatToPositionEffector(
                    DisplayManager.WindowResolution.Center, 
                    floatDuration, 
                    floatTension
                    )
                );

            GetForm<GUIButton>(mainButton).LockInteraction();

            foreach (var name in otherButtons)
            {
                GetForm(name).AddEffector(new FadeOutEffector(15));
            }
        }

        public void Render_Main()
        {
            RenderForms();
        }

        #region Animation Related Methods and Properties

        #region Animation Related Constants

        /// <summary>
        /// The number of frames of the play button animation until the multiform exits.
        /// </summary>
        private const int PlayButtonAnimation_ExitFrame = 230;

        /// <summary>
        /// The number of frames of the options button animation until the multiform exits.
        /// </summary>
        private const int OptionsButtonAnimation_ExitFrame = 3300;

        /// <summary>
        /// The number of frames of the credits button animation until the multiform exits.
        /// </summary>
        private const int CreditsButtonAnimation_ExitFrame = 330;

        #endregion

        public void Update_OnPlayButtonClick()
        {
            UpdateForms();
            UpdateTime();

            if (AtFrame(PlayButtonAnimation_ExitFrame) || MouseInput.IsReleased(MouseButtons.Left))
            {
                ExitTo(LevelSelectMultiform.MultiformName);
            }
        }

        public void Update_OnOptionsButtonClick()
        {
            UpdateForms();
            UpdateTime();

            if (AtFrame(OptionsButtonAnimation_ExitFrame) || MouseInput.IsReleased(MouseButtons.Left))
            {
                ExitTo(LevelSelectMultiform.MultiformName);
                // ExitTo(OptionsMenu.MultiformName);
            }
        }

        public void Update_OnCreditsButtonClick()
        {
            UpdateForms();
            UpdateTime();

            if (AtFrame(CreditsButtonAnimation_ExitFrame) || MouseInput.IsReleased(MouseButtons.Left))
            {
                ExitTo(LevelSelectMultiform.MultiformName);
                // ExitTo(CreditsMenu.MultiformName);
            }
        }

        public void Render_OnPlayButtonClick()
        {
            RenderFormsExcept(OnPlayButtonClickAnimationFormName);

            RenderForm(OnPlayButtonClickAnimationFormName);
        }

        public void Render_OnOptionsButtonClick()
        {
            RenderFormsExcept(OnOptionsButtonClickAnimationFormName);

            RenderForm(OnOptionsButtonClickAnimationFormName);
        }

        public void Render_OnCreditsButtonClick()
        {
            RenderFormsExcept(OnCreditsButtonClickAnimationFormName);

            RenderForm(OnCreditsButtonClickAnimationFormName);
        }

        #endregion

    }
}
