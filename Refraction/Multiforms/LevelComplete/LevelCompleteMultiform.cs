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
 * The Level Complete screen multiform.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Refraction_V2.Multiforms.Level;
using System;
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.LevelComplete
{
	public class LevelCompleteMultiform : Multiform
	{

		#region Name Constants

		/// <summary>
		/// The name of this multiform.
		/// </summary>
		public const string MultiformName = "LevelComplete";

		/// <summary>
		/// The name of the Level Complete text form.
		/// </summary>
		public const string LevelCompleteTextFormName = "LevelCompleteText";

		/// <summary>
		/// The name of the Previous Level Button form.
		/// </summary>
		public const string PrevButtonFormName = "PrevButton";

		/// <summary>
		/// The name of the Replay Level Button form.
		/// </summary>
		public const string ReplayButtonFormName = "ReplayButton";

		/// <summary>
		/// The name of the Next Level Button form.
		/// </summary>
		public const string NextButtonFormName = "NextButton";

		/// <summary>
		/// The name of the Back Button form.
		/// </summary>
		public const string BackButtonFormName = "BackButton";

		/// <summary>
		/// The name of the button navigator form.
		/// </summary>
		public const string ButtonNavigatorFormName = "ButtonNavigator";

		#endregion

		#region Form Offsets

		/// <summary>
		/// The y-offset of the level complete text from the screen center.
		/// </summary>
		public const int LEVEL_COMPLETE_TEXT_Y_OFFSET = 75;

		/// <summary>
		/// The x-offset of the previous level and next level buttons relative to
		/// the screen center.
		/// </summary>
		public const int BUTTON_X_OFFSET = 175;

		/// <summary>
		/// Th y-offset of the previous level, replay level, and next level buttons
		/// relative to the screen center.
		/// </summary>
		public const int BUTTON_Y_OFFSET = 0;

		/// <summary>
		/// The x-offset of the back button relative to the left side of the screen.
		/// </summary>
		public const int BACK_BUTTON_X_OFFSET = 50;

		/// <summary>
		/// The y-offset of the back button relative to the bottom of the screen.
		/// </summary>
		public const int BACK_BUTTON_Y_OFFSET = 28;

		#endregion

		public LevelNameInfo LevelNameInfo { get; private set; }

		public override void Construct(MultiformTransmissionData args)
		{
			if (args.SenderName == LevelMultiform.MultiformName)
			{
				LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");
			}
			else
				throw new MultiformException(
					String.Format("Unknown multiform layout '{0}' -> '{1}'.", args.SenderName, MultiformName)
					);

			Construct_Text();
			Construct_Buttons();

			SetUpdater(Update_Main);
			SetRenderer(Render_Main);
		}

		/// <summary>
		/// Construct the LevelCompleteTextForm.
		/// </summary>
		private void Construct_Text()
		{
			string text;
			if (LevelNameInfo.Sequential)
				text = String.Format("Level {0} Complete", LevelNameInfo.LevelNumber + 1);
			else
				text = "Level Complete";

			var textPosition = DisplayManager.WindowResolution.Center - new Vector2(0, LEVEL_COMPLETE_TEXT_Y_OFFSET);
			RegisterForm(LevelCompleteTextFormName, new LevelCompleteTextForm(text, textPosition));
		}

		/// <summary>
		/// Construct the button forms.
		/// </summary>
		private void Construct_Buttons()
		{
			Vector2 screenCenter       = DisplayManager.WindowResolution.Center;
			Vector2 offset             = new Vector2(BUTTON_X_OFFSET, BUTTON_Y_OFFSET);

			Vector2 replayButtonCenter = screenCenter;
			Vector2 prevButtonCenter   = screenCenter - offset;
			Vector2 nextButtonCenter   = screenCenter + offset;
			Vector2 backButtonCenter   = new Vector2(
											BACK_BUTTON_X_OFFSET, 
											DisplayManager.WindowResolution.Height - BACK_BUTTON_Y_OFFSET);

			var navigatorButtons = new List<string>();

			// The reason for the weird order in which we add the button form names
			// to the list of navigator buttons is because it determines the order
			// of the buttons in the navigator.

			if (LevelNameInfo.HasPrevLevel())
			{
				RegisterForm(
					PrevButtonFormName,
					new LevelCompleteButtonForm(
						LevelCompleteButtonForm.PREV_BUTTON_NAME, prevButtonCenter));

				navigatorButtons.Add(PrevButtonFormName);
			}

			navigatorButtons.Add(ReplayButtonFormName);

			if (LevelNameInfo.HasNextLevel())
			{
				RegisterForm(
					NextButtonFormName,
					new LevelCompleteButtonForm(
						LevelCompleteButtonForm.NEXT_BUTTON_NAME, nextButtonCenter));

				navigatorButtons.Add(NextButtonFormName);
			}

			navigatorButtons.Add(BackButtonFormName);

			RegisterForm(
					ReplayButtonFormName,
					new LevelCompleteButtonForm(
						LevelCompleteButtonForm.REPLAY_BUTTON_NAME, replayButtonCenter));

			RegisterForm(
					BackButtonFormName,
					new LevelCompleteButtonForm(
						LevelCompleteButtonForm.BACK_BUTTON_NAME, backButtonCenter));

			RegisterForm(ButtonNavigatorFormName, new KeyboardButtonNavigatorForm(navigatorButtons.ToArray()));
		}

		private const int DONT_INCREMENT = 0;
		private const int INCREMENT_NEXT = 1;
		private const int INCREMENT_PREV = -1;

		public void ReturnToLevel(int increment)
		{
			Manager.Close(this);

			var data = new MultiformTransmissionData(MultiformName);

			if (increment == INCREMENT_NEXT)
				LevelNameInfo.IncrementLevel();
			else if (increment == INCREMENT_PREV)
				LevelNameInfo.DecrementLevel();

			data.SetAttr<LevelNameInfo>("LevelNameInfo", LevelNameInfo);
			Manager.Construct(LevelMultiform.MultiformName, data);

			ClearForms();
		}

		public void Update_Main()
		{
			UpdateFormsExcept(ButtonNavigatorFormName);

			// We have to update this form after everything else because the navigator
			// forcefully sets the values of CollidingWithMouse on the button forms to true.
			// If we updated this before the buttons the buttons' update method would override
			// this effect, and the navigator would be useless.
			UpdateForm(ButtonNavigatorFormName);

			var navigator = GetForm<KeyboardButtonNavigatorForm>(ButtonNavigatorFormName);
			int releasedButton = -1;

			if (navigator.HasFocus)
			{
				if (KeyboardInput.IsReleased(Keys.Enter))
					releasedButton = navigator.SelectedButtonIndex.Value;
			}

			var hasPrevLevel = LevelNameInfo.HasPrevLevel();
			var hasNextLevel = LevelNameInfo.HasNextLevel();

			// These are the two buttons guaranteed to be onscreen regardless of the
			// values of hasPrevLevel and hasNextLevel.
			var backButton   = GetForm<ButtonForm>(BackButtonFormName);
			var replayButton = GetForm<ButtonForm>(ReplayButtonFormName);

			var nextButton   = hasNextLevel 
							 ? GetForm<ButtonForm>(NextButtonFormName) 
							 : null;
			var prevButton   = hasPrevLevel
							 ? GetForm<ButtonForm>(PrevButtonFormName)
							 : null;

			var mButton = MouseButtons.Left;

			if (replayButton.IsReleased(mButton)
				|| (hasPrevLevel && releasedButton == 1)
				|| (!hasPrevLevel && releasedButton == 0))
			{
				ReturnToLevel(DONT_INCREMENT);
			}
			else if (hasPrevLevel &&
				prevButton.IsReleased(mButton)
				|| releasedButton == 0)
			{
				ReturnToLevel(INCREMENT_PREV);
			}
			else if (hasNextLevel && 
				nextButton.IsReleased(mButton)
				|| (hasPrevLevel && releasedButton == 2)
				|| (!hasPrevLevel && releasedButton == 1))
			{
				ReturnToLevel(INCREMENT_NEXT);
			}
			else if (backButton.IsReleased(mButton)
				|| (hasPrevLevel && hasNextLevel && releasedButton == 3)
				|| (!hasPrevLevel && hasNextLevel && releasedButton == 2)
				|| (!hasPrevLevel && !hasNextLevel && releasedButton == 1))
			{
				// Do some shit.	
			}
		}

		public void Render_Main()
		{
			RenderForms();
		}

	}
}
