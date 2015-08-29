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
 * A form representing a simple piece of text on the screen. The word "simple" has no
 * rigorous definition in this context.
 */

#endregion

#region Using Statement

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace DemeterEngine.Multiforms.Forms
{
	public class SimpleTextForm : Form
	{

		public string Text { get; protected set; }

		public SpriteFont Font { get; protected set; }

		public Vector2 Position { get; protected set; }

		public bool Centred { get; private set; }

		/// <summary>
		/// The red value of the sprite's tint.
		/// </summary>
		public float TintRed { get; set; }

		/// <summary>
		/// The green value of the sprite's tint.
		/// </summary>
		public float TintGreen { get; set; }

		/// <summary>
		/// The blue value of the sprite's tint.
		/// </summary>
		public float TintBlue { get; set; }

		/// <summary>
		/// The alpha value of the sprite.
		/// </summary>
		public float Alpha { get; set; }

		/// <summary>
		/// The tint to colour the sprite when rendering.
		/// </summary>
		public Color Tint
		{
			get
			{
				return new Color((int)TintRed, (int)TintGreen, (int)TintBlue, (int)Alpha);
			}
			set
			{
				TintRed = value.R;
				TintGreen = value.G;
				TintBlue = value.B;
				Alpha = value.A;
			}
		}

		public float Scale { get; set; }

		public float Rotation { get; set; }

        public SimpleTextForm(
            string text, string fontName, Vector2 position, bool centred = false, bool keepTime = true)
            : this(text, ArtManager.Font(fontName), position, centred, keepTime) { }

		public SimpleTextForm(
            string text, SpriteFont font, Vector2 position, bool centred = false, bool keepTime = true)
			: base(keepTime)
		{
			Text = text;
			Font = font;
			Position = position;
			Centred = centred;
			Tint = Color.White;
			Scale = 1;
			Rotation = 0;
		}

		public void CenterOn(Vector2 position)
		{
			Position = position;
			Centred = true;
		}

		public void SetPosition(Vector2 position)
		{
			Position = position;
			Centred = false;
		}

        public void SetText(string text)
        {
            Text = text;
        }

		public override void Render()
		{
            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

			var offset = Centred ? Font.MeasureString(Text) / 2f : Vector2.Zero;
			DisplayManager.DrawString(Font, Text, Position, Tint, Rotation, offset, Scale, SpriteEffects.None, 0f);

            DisplayManager.ClearSpriteBatchProperties();
		}
	}
}
