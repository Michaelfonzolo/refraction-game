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
 * A navigator is an abstract form that allows the user to interact with a set
 * of buttons via keyboard controls on top of being able to click on them. For
 * example, pressing the left or right arrow keys will cause the navigator to
 * select a different button, and pressing escape will cause the navigator to
 * loose focus (i.e. stop selecting any button).
 * 
 * The keys that represent these navigation actions are configurable.
 */

#endregion

#region Using Statements

using DemeterEngine.Input;
using DemeterEngine.Maths;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DemeterEngine.Multiforms.Forms
{
	public class KeyboardButtonNavigatorForm : Form
	{

		/// <summary>
		/// The list of buttons this navigator can interact with.
		/// </summary>
		public List<ButtonForm> RegisteredButtons = new List<ButtonForm>();

		/// <summary>
		/// The index of the currently selected button, or null if the navigator
		/// has no focus.
		/// </summary>
		public int? SelectedButtonIndex { get; private set; }

		/// <summary>
		/// The previous index from the last time the navigator has focus.
		/// </summary>
		private int _prevIndex = 0;

		private string[] buttonNames;

		/// <summary>
		/// The key to navigate to the left.
		/// </summary>
		public Keys NavigateLeft { get; private set; }

		/// <summary>
		/// The key to navigate to the right.
		/// </summary>
		public Keys NavigateRight { get; private set; }

		/// <summary>
		/// The key to loose focus.
		/// </summary>
		public Keys LooseFocusKey { get; private set; }

		/// <summary>
		/// Whether or not the navigator has focus.
		/// </summary>
		public bool HasFocus { get { return SelectedButtonIndex.HasValue; } }

		public KeyboardButtonNavigatorForm(
			string[] buttonNames, Keys navigLeft = Keys.Left, Keys navigRight = Keys.Right, 
			Keys looseFocus = Keys.Escape, bool keepTime = true)
			: base(keepTime)
		{
			this.buttonNames = buttonNames;
			Initialize(navigLeft, navigRight, looseFocus);
		}

		public KeyboardButtonNavigatorForm(
			ButtonForm[] buttons, Keys navigLeft = Keys.Left, Keys navigRight = Keys.Right, 
			Keys looseFocus = Keys.Escape, bool keepTime = true)
			: base(keepTime)
		{
			RegisteredButtons = buttons.ToList();
			Initialize(navigLeft, navigRight, looseFocus);
		}

		private void Initialize(
			Keys navigLeft = Keys.Left, Keys navigRight = Keys.Right, Keys looseFocus = Keys.Escape)
		{
			NavigateLeft = navigLeft;
			NavigateRight = navigRight;
			LooseFocusKey = looseFocus;
		}

		public override void PostConstruct()
		{
			if (buttonNames != null)
			{
				foreach (var name in buttonNames)
					RegisteredButtons.Add(Parent.GetForm<ButtonForm>(name));
				buttonNames = null;
			}
		}

		private void Navigate(int increment)
		{
			if (SelectedButtonIndex.HasValue)
			{
				SelectedButtonIndex = MathUtils.PyMod(
					SelectedButtonIndex.Value + increment, RegisteredButtons.Count);
			}
			else
			{
				SelectedButtonIndex = _prevIndex;
			}
		}

		public void LooseFocus()
		{
			_prevIndex = SelectedButtonIndex.Value;
			SelectedButtonIndex = null;
		}

		protected virtual bool ValidKeyPressedState(Keys key)
		{
			return KeyboardInput.IsClicked(key) ||
				  (KeyboardInput.IsHeld(key, 600d) &&
				  (KeyboardInput.FramesSinceKeyPressed[KeyboardInput.KeyToInt(key)] - 600d) % 4 == 0);
		}

		public override void Update()
		{
			base.Update();

			if (ValidKeyPressedState(NavigateLeft))
				Navigate(-1);

			if (ValidKeyPressedState(NavigateRight))
				Navigate(1);

			if (SelectedButtonIndex.HasValue)
			{
				if (KeyboardInput.IsPressed(LooseFocusKey)
					|| RegisteredButtons.Count(b => b.CollidingWithMouse) > 0)
					LooseFocus();
				else
					RegisteredButtons[SelectedButtonIndex.Value].CollidingWithMouse = true;
			}
		}

	}
}
