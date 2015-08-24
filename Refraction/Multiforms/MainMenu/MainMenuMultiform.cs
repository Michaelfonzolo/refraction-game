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

        public static readonly Vector2 PlayButtonCenter = DisplayManager.WindowResolution.Center;

        public static readonly GUIButtonInfo PlayButtonInfo = new GUIButtonInfo(
            "Play", Assets.MainMenu.Images.PlayButton);

        public override void Construct(MultiformTransmissionData args)
        {
            RegisterForm(PlayButtonFormName, new GUIButton(PlayButtonInfo, PlayButtonCenter));

            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
        }

        public void Update_Main()
        {
            UpdateForms();

            if (GetForm<GUIButton>(PlayButtonFormName).IsReleased(MouseButtons.Left))
            {
                Manager.Close(this);
                Manager.Construct(LevelSelectMultiform.MultiformName);
                ClearForms();

                return;
            }
        }

        public void Render_Main()
        {
            RenderForms();
        }

    }
}
