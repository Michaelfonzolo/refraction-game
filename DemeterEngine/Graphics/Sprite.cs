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
 * A sprite is a representation of a graphical object in the DemeterEngine, which encapsulates
 * all the functionality of manipulating an image. This means, instead of having to supply all
 * the necessary arguments to DisplayManager.Draw to get your image to turn out the way you want,
 * you can simply apply them to the sprite and call Sprite.Render.
 */

#endregion

#region Using Statements

using DemeterEngine.Maths;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion

namespace DemeterEngine.Graphics
{
    public class Sprite
    {

        /// <summary>
        /// The texture representing this sprite.
        /// </summary>
        public Texture2D Texture { get; private set; }

		public int Width
		{
			get
			{
				return Texture.Width;
			}
		}

		public int Height
		{
			get
			{
				return Texture.Height;
			}
		}

        public Vector2 Bounds
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }

        /// <summary>
        /// The red value of the sprite's tint.
        /// </summary>
        public double TintRed { get; set; }

        /// <summary>
        /// The green value of the sprite's tint.
        /// </summary>
        public double TintGreen { get; set; }

        /// <summary>
        /// The blue value of the sprite's tint.
        /// </summary>
        public double TintBlue { get; set; }

        /// <summary>
        /// The alpha value of the sprite.
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        /// The center of the sprite's texture's bounds.
        /// </summary>
        public Vector2 TextureCenter
        {
            get
            {
                return VectorUtils.FromPoint(Texture.Bounds.Center);
            }
        }

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

        public Color TintRGB
        {
            get
            {
                return new Color((int)TintRed, (int)TintGreen, (int)TintBlue);   
            }
            set
            {
                TintRed = value.R;
                TintGreen = value.G;
                TintBlue = value.B;
            }
        }

        /// <summary>
        /// The position of the sprite.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Whether or not the sprite is centered at its Position.
        /// </summary>
        public bool Centred { get; set; }

		/// <summary>
		/// The rotation of this sprite in radians.
		/// </summary>
        public double Rotation { get; set; }

		/// <summary>
		/// The rotation of this sprite in angles.
		/// </summary>
        public double AngleOfRotation
        {
			get
			{
				return Rotation * 180d / Math.PI;
			}
            set
            {
                Rotation = value * Math.PI / 180d;
            }
        }

        public Sprite(Texture2D texture)
            : this(texture, Vector2.Zero) { }

        public Sprite(Texture2D texture, Vector2 position, bool centred = false)
        {
            Texture = texture;
            Position = position;
            Centred = centred;
            Tint = Color.White;
            Rotation = 0;
        }

        public void Center()
        {
            Centred = true;
        }

        public void CenterOn(Vector2 position)
        {
            Position = position;
            Centred = true;
        }

        public void CenterOn(Point point)
        {
            CenterOn(VectorUtils.FromPoint(point));
        }

        public void MakeTransparent()
        {
            Alpha = 0;
        }

        public void MakeOpaque()
        {
            Alpha = 1;
        }

        /// <summary>
        /// Render the sprite at it's internal position.
        /// </summary>
        public void Render()
        {
            var origin = Vector2.Zero;
            if (Centred)
                origin = TextureCenter;
            DisplayManager.Draw(Texture, Position, null, Tint, (float)Rotation, origin, 1, SpriteEffects.None, 1f);
        }

        /// <summary>
        /// Render the sprite at the given position.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="centred"></param>
        public void Render(Point point, bool centred = false)
        {
            Render(VectorUtils.FromPoint(point), centred);
        }

        /// <summary>
        /// Render the sprite at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="centred"></param>
        public void Render(Vector2 position, bool centred = false)
        {
            if (centred)
                DisplayManager.Draw(
                    Texture, position, null, Tint, (float)Rotation, 
                    TextureCenter, 1, SpriteEffects.None, 1f
                    );
            else
                DisplayManager.Draw(Texture, position, Tint);
        }

        /// <summary>
        /// Render the sprite at the given position with the given origin (overrides
        /// the Centred property of the sprite).
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        public void Render(Point point, Point origin)
        {
            Render(VectorUtils.FromPoint(point), VectorUtils.FromPoint(origin));
        }

        /// <summary>
        /// Render the sprite at the given position with the given origin (overrides
        /// the Centred property of the sprite).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        public void Render(Vector2 position, Point origin)
        {
            Render(position, VectorUtils.FromPoint(origin));
        }

        /// <summary>
        /// Render the sprite at the given position with the given origin (overrides
        /// the Centred property of the sprite).
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        public void Render(Point point, Vector2 origin)
        {
            Render(VectorUtils.FromPoint(point), origin);
        }

        /// <summary>
        /// Render the sprite at the given position with the given origin (overrides
        /// the Centred property of the sprite).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="origin"></param>
        public void Render(Vector2 position, Vector2 origin)
        {
            DisplayManager.Draw(Texture, position, null, Tint, (float)Rotation, origin, 1, SpriteEffects.None, 1f);
        }

    }
}
