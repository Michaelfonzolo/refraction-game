using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Refraction_V2.Multiforms
{
    public class GUIButtonInfo
    {

        public string Text { get; private set; }

        public Texture2D Texture { get; private set; }

        public SpriteFont Font { get; private set; }

        public Color? InitialTextColor { get; set; }

        public GUIButtonInfo(string text, Texture2D texture)
            : this(text, texture, Assets.Shared.Fonts.GUIButtonFont_Medium) { }

        public GUIButtonInfo(string text, Texture2D texture, SpriteFont font)
        {
            Text = text;
            Texture = texture;
            Font = font;
            InitialTextColor = null;
        }
    }
}
