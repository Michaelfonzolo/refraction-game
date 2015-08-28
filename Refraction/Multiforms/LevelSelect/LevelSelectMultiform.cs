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
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.ForegroundContent;
using Refraction_V2.Multiforms.Level;
using Refraction_V2.Multiforms.LevelLoad;
using Refraction_V2.Utils;

#endregion

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class LevelSelectMultiform : RefractionGameMultiform
    {

        #region Form Info

        /// <summary>
        /// The name of this multiform.
        /// </summary>
        public const string MultiformName = "LevelSelect";

        /// <summary>
        /// The name of the scroll bar form.
        /// </summary>
		public const string ScrollBarFormName = "ScrollBar";

        /// <summary>
        /// The name of the back button form.
        /// </summary>
        public const string BackButtonFormName = "BackButton";

        /// <summary>
		/// The x offset of the first level select button from the top of the screen.
		/// </summary>
		public const int INITIAL_LEVEL_SELECT_X_OFFSET = 50;

		/// <summary>
		/// The y offset of the first level select button from the top of the screen.
		/// </summary>
		public const int INITIAL_LEVEL_SELECT_Y_OFFSET = 40;

		/// <summary>
		/// The gap between horizontally adjacent level select buttons.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_GAP_X = 37;

		/// <summary>
		/// The gap between vertically adjacent level select buttons.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_GAP_Y = 60;

		/// <summary>
		/// The number of level select buttons per row.
		/// </summary>
		public const int BUTTONS_PER_ROW = 10;

        /// <summary>
        /// The GUIButtonInfo for the back button.
        /// </summary>
        public static readonly GUIButtonInfo BackButtonInfo = new GUIButtonInfo(
            "BACK", Assets.LevelSelect.Images.BackButton, Assets.Shared.Fonts.GUIButtonFont_Small);

        /// <summary>
        /// The bottom left of the back button.
        /// </summary>
        public static readonly Vector2 BackButtonBottomLeft = new Vector2(
            10,
            DisplayManager.WindowHeight - 10
            );

        #endregion

        /// <summary>
        /// Whether or not it was necessary to include a scrollbar.
        /// </summary>
		public bool HasScrollBar { get; private set; }

        /// <summary>
        /// Used to indicate when a button was pressed.
        /// </summary>
        private bool buttonPressed;

        /// <summary>
        /// Used to indicate the level number of the pressed button.
        /// </summary>
        private int selectedLevelNumber;

		public override void Construct(MultiformTransmissionData args)
		{
            // We have to set this value to false so that it doesn't automatically start out as
            // true when we reconstruct this multiform, which would cause the player to automatically
            // go back into the last level they selected.
            buttonPressed = false;

			HasScrollBar = false;

            int BUTTON_WIDTH  = Assets.LevelSelect.Images.ClearedLevelButton.Width;
            int BUTTON_HEIGHT = Assets.LevelSelect.Images.ClearedLevelButton.Height;

			int xOffset = INITIAL_LEVEL_SELECT_X_OFFSET;
			int yOffset = INITIAL_LEVEL_SELECT_Y_OFFSET;

			int rowNum          = LoadedLevelManager.SequentialLevels.Length / BUTTONS_PER_ROW,
				totalWidth      = (BUTTON_WIDTH + LEVEL_SELECT_BUTTON_GAP_X - 2) * BUTTONS_PER_ROW,
				totalHeight     = rowNum * BUTTON_HEIGHT + (rowNum - 1) * LEVEL_SELECT_BUTTON_GAP_Y + 200,
				scrollBarHeight = DisplayManager.WindowResolution.Height - 2 * INITIAL_LEVEL_SELECT_Y_OFFSET;

            LevelSelectScrollBar scrollBar = null;

			// We only add a scrollbar if there is not enough room onscreen to fit everything. This occurs
			// when the total vertical space taken up by the level select buttons is greater than that which
			// would've been taken up by the scrollbar.
			if (totalHeight > scrollBarHeight)
			{
				var topLeft = new Vector2(xOffset + totalWidth + 20, yOffset - 10);
				scrollBar = new LevelSelectScrollBar(topLeft, scrollBarHeight, totalHeight, 12, true);
				RegisterForm(ScrollBarFormName, scrollBar);
				HasScrollBar = true;
			}

            // Construct all the buttons.
            int xIndex, yIndex;
            LevelSelectButton button;
            for (int i = 0; i < LoadedLevelManager.SequentialLevels.Length; i++)
            {
                xIndex = i % BUTTONS_PER_ROW;
                yIndex = i / BUTTONS_PER_ROW;
                button = new LevelSelectButton(
                    new RectCollider(
                        xOffset + xIndex * (BUTTON_WIDTH + LEVEL_SELECT_BUTTON_GAP_X), 
                        yOffset + yIndex * (BUTTON_HEIGHT + LEVEL_SELECT_BUTTON_GAP_Y),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT),
                    scrollBar,
                    i);
                RegisterForm(button);
            }

            RegisterForm(BackButtonFormName,
                new GUIButton(BackButtonInfo, BackButtonBottomLeft, PositionType.BottomLeft));

            RegisterForm(new ClickParticleSpawnerForm());

            FadeIn(20, Color.White, Update_Main, Render_Main);
		}

        internal void ButtonPressed(int number)
        {
            buttonPressed = true;
            selectedLevelNumber = number;
        }

		public void Update_Main()
		{
            if (HasScrollBar)
            {
                UpdateForm(ScrollBarFormName);
            }

            UpdateFormsExcept(ScrollBarFormName);

            if (buttonPressed)
            {
                var data = new MultiformTransmissionData(MultiformName);
                var LevelNameInfo = new LevelNameInfo(selectedLevelNumber);
                data.SetAttr<LevelNameInfo>("LevelNameInfo", LevelNameInfo);

                FadeOutAndClose(
                    20, Color.White, LevelLoadMultiform.MultiformName, 
                    data, true, () => { UpdateForms(); }, Render_Main);
            }

            else if (GetForm<GUIButton>(BackButtonFormName).IsReleased(MouseButtons.Left))
            {
                FadeOutAndClose(
                    20, Color.White, MainMenu.MainMenuMultiform.MultiformName, 
                    new MultiformTransmissionData(MultiformName), true, 
                    () => { UpdateForms(); }, Render_Main);
            }
		}

		public void Render_Main()
		{
            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            RenderForms();

            DisplayManager.ClearSpriteBatchProperties();
		}

	}
}
