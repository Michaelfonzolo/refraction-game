using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Refraction_V2.Multiforms.LevelSelect;

namespace Refraction_V2.Multiforms.MainMenu
{
    public class MainMenuMultiform : Multiform
    {

        public const string MultiformName = "MainMenu";

        public const string PlayButtonFormName = "PlayButton";

        public static readonly double PLAY_BUTTON_X = DisplayManager.WindowResolution.Width / 2f;

        public static readonly double PLAY_BUTTON_Y = DisplayManager.WindowResolution.Height / 2f;

        public static readonly Vector2 PLAY_BUTTON_TOPLEFT =
            DisplayManager.WindowResolution.Center
            - MainMenuButtonForm.BUTTON_DIMENSIONS / 2f;

        public override void Construct(MultiformTransmissionData args)
        {
            var playButtonRect = new RectCollider(PLAY_BUTTON_TOPLEFT, MainMenuButtonForm.BUTTON_DIMENSIONS);
            var playButton = new MainMenuButtonForm(playButtonRect, MainMenuButtonForm.PlayButton);
            RegisterForm(PlayButtonFormName, playButton);

            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
        }

        public void Update_Main()
        {
            UpdateForms();

            if (GetForm<MainMenuButtonForm>(PlayButtonFormName).IsReleased(MouseButtons.Left))
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

    }
}
