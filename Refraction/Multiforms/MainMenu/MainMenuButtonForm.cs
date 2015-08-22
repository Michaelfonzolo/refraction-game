using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;

namespace Refraction_V2.Multiforms.MainMenu
{
    public class MainMenuButtonForm : ButtonForm
    {

        public const string PlayButton = "MainMenu_PlayButton";

        public static readonly Vector2 BUTTON_DIMENSIONS = new Vector2(200, 70);

        public Sprite ButtonSprite { get; private set; }

        public MainMenuButtonForm(RectCollider collider, string buttonName)
            : base(collider)
        {
            ButtonSprite = ArtManager.Sprite(buttonName);
            ButtonSprite.Position = collider.TopLeft;
        }

        public override void Render()
        {
            ButtonSprite.Tint = CollidingWithMouse ? Color.Red : Color.White;
            ButtonSprite.Render();
        }

    }
}
