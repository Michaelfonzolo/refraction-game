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
 * A form representing a button form in the LevelComplete multiform.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;

#endregion

namespace Refraction_V2.Multiforms.LevelComplete
{
	public class LevelCompleteButtonForm : ButtonForm
	{

		/// <summary>
		/// The name of the sprite file for the Previous Level Button.
		/// </summary>
		public const string PREV_BUTTON_NAME = "LevelComplete_PrevButton";

		/// <summary>
		/// The name of the sprite file for the Replay Level Button.
		/// </summary>
		public const string REPLAY_BUTTON_NAME = "LevelComplete_ReplayButton";

		/// <summary>
		/// The name of the sprite file for the Next Level Button.
		/// </summary>
		public const string NEXT_BUTTON_NAME = "LevelComplete_NextButton";

		/// <summary>
		/// The name of the sprite file for the Back Button.
		/// </summary>
		public const string BACK_BUTTON_NAME = "LevelComplete_BackButton";

		public Sprite Sprite { get; private set; }

		public LevelCompleteButtonForm(string textureName, Vector2 center)
			: base(null)
		{
			Sprite = ArtManager.Sprite(textureName);
			Sprite.CenterOn(center);

			var collider = new RectCollider(center - Sprite.TextureCenter, Sprite.Width, Sprite.Height);
			PostSetCollider(collider);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Render()
		{
			Sprite.Tint = CollidingWithMouse ? Color.Red : Color.White;
			Sprite.Render();
		}

	}
}
