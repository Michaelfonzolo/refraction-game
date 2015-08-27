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
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Utils;
using System;
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.LevelComplete
{
	public class LevelCompleteMultiform : Multiform
	{

		#region Form Info

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

        /// <summary>
        /// The position of the "level complete" text on screen.
        /// </summary>
        public static readonly Vector2 LVL_CMPL_TXT_CENTER = new Vector2(
            DisplayManager.WindowWidth / 2f,
            DisplayManager.WindowHeight / 2f - 75
            );

        /// <summary>
        /// The center of the "replay level" button on screen.
        /// </summary>
        public static readonly Vector2 ReplayButtonCenter = DisplayManager.WindowResolution.Center;

        public static readonly GUIButtonInfo ReplayButtonInfo = new GUIButtonInfo(
            "REPLAY", Assets.LevelComplete.Images.ReplayButton);

        /// <summary>
        /// The center of the "previous level" button on screen.
        /// </summary>
        public static readonly Vector2 PrevButtonCenter = new Vector2(
            DisplayManager.WindowWidth / 2f - 175,
            DisplayManager.WindowHeight / 2f
            );

        public static readonly GUIButtonInfo PrevButtonInfo = new GUIButtonInfo(
            "PREV", Assets.LevelComplete.Images.PrevButton);

        /// <summary>
        /// The center of the "next level" button on screen.
        /// </summary>
        public static readonly Vector2 NextButtonCenter = new Vector2(
            DisplayManager.WindowWidth / 2f + 175,
            DisplayManager.WindowHeight / 2f
            );

        public static readonly GUIButtonInfo NextButtonInfo = new GUIButtonInfo(
            "NEXT", Assets.LevelComplete.Images.NextButton);

        /// <summary>
        /// The center of the "back" button on screen.
        /// </summary>
        public static readonly Vector2 BackButtonBottomLeft = new Vector2(
            10,
            DisplayManager.WindowHeight - 10
            );

        public static readonly GUIButtonInfo BackButtonInfo = new GUIButtonInfo(
            "BACK", Assets.LevelComplete.Images.BackButton, Assets.Shared.Fonts.GUIButtonFont_Small);

		#endregion

		public LevelNameInfo LevelNameInfo { get; private set; }

		public override void Construct(MultiformTransmissionData args)
		{
			if (args.SenderName == LevelMultiform.MultiformName)
			{
				LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");
			}
			else
            {
                throw new MultiformException(
                    String.Format("Unknown multiform layout: '{0}' -> '{1}'.", args.SenderName, MultiformName)
                    );
            }

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
				text = String.Format("LEVEL {0} COMPLETE", LevelNameInfo.LevelNumber + 1);
			else
				text = "LEVEL COMPLETE";

			RegisterForm(
                LevelCompleteTextFormName, 
                new LevelCompleteTextForm(text, LVL_CMPL_TXT_CENTER)
                );
		}

        private void RegisterButton(
            string name, GUIButtonInfo info, Vector2 position, List<string> navigatorButtons,
            PositionType positionType = PositionType.Center)
        {
            RegisterForm(name, new GUIButton(info, position, positionType));
            navigatorButtons.Add(name);
        }

		/// <summary>
		/// Construct the button forms.
		/// </summary>
		private void Construct_Buttons()
		{
			var navigatorButtons = new List<string>();

			// The reason for the weird order in which we add the button form names
			// to the list of navigator buttons is because it determines the order
			// of the buttons in the navigator.

			if (LevelNameInfo.HasPrevLevel())
			{
                RegisterButton(PrevButtonFormName, PrevButtonInfo, PrevButtonCenter, navigatorButtons);
			}

			RegisterButton(ReplayButtonFormName, ReplayButtonInfo, ReplayButtonCenter, navigatorButtons);

			if (LevelNameInfo.HasNextLevel())
			{
                RegisterButton(NextButtonFormName, NextButtonInfo, NextButtonCenter, navigatorButtons);
			}

			RegisterButton(
                BackButtonFormName, BackButtonInfo, BackButtonBottomLeft, 
                navigatorButtons, PositionType.BottomLeft);

			RegisterForm(ButtonNavigatorFormName, new KeyboardButtonNavigatorForm(navigatorButtons.ToArray()));
		}

        #region Constants indicating which level to go to next

        private const int SAME_LEVEL = 0;
		private const int NEXT_LEVEL = 1;
		private const int PREV_LEVEL = -1;

        #endregion

        /// <summary>
        /// Return to the level multiform. The "increment" argument indicates whether
        /// we stay on the same level, go to the previous level, or go to the next level.
        /// </summary>
        /// <param name="increment"></param>
        public void ReturnToLevel(int increment)
		{
			Manager.Close(this);

			var data = new MultiformTransmissionData(MultiformName);

			if (increment == NEXT_LEVEL)
				LevelNameInfo.IncrementLevel();
			else if (increment == PREV_LEVEL)
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
				ReturnToLevel(SAME_LEVEL);
			}
			else if (hasPrevLevel &&
				prevButton.IsReleased(mButton)
				|| releasedButton == 0)
			{
				ReturnToLevel(PREV_LEVEL);
			}
			else if (hasNextLevel && 
				nextButton.IsReleased(mButton)
				|| (hasPrevLevel && releasedButton == 2)
				|| (!hasPrevLevel && releasedButton == 1))
			{
				ReturnToLevel(NEXT_LEVEL);
			}
			else if (backButton.IsReleased(mButton)
				|| (hasPrevLevel && hasNextLevel && releasedButton == 3)
				|| (!hasPrevLevel && hasNextLevel && releasedButton == 2)
				|| (!hasPrevLevel && !hasNextLevel && releasedButton == 1))
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
