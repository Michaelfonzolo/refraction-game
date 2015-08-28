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
 * The main level multiform.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.LevelComplete;
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Utils;
using System;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class LevelMultiform : RefractionGameMultiform
    {

        #region Form Info

        /// <summary>
		/// The name of the multiform.
		/// </summary>
		public const string MultiformName = "Level";

		/// <summary>
		/// The name of the board form instance.
		/// </summary>
		public const string BoardFormName = "Board";

		/// <summary>
		/// The name of the inventory form instance.
		/// </summary>
		public const string InventoryFormName = "Inventory";

        /// <summary>
        /// The name of the back button form instance.
        /// </summary>
        public const string BackButtonFormName = "BackButton";

        /// <summary>
        /// The GUIButtonInfo for the back button.
        /// </summary>
        public static readonly GUIButtonInfo BackButtonInfo = new GUIButtonInfo(
            "BACK", Assets.Level.Images.BackButton, Assets.Shared.Fonts.GUIButtonFont_Small);

        /// <summary>
        /// The bottom left of the back button.
        /// </summary>
        public static readonly Vector2 BackButtonBottomLeft = new Vector2(
            10,
            DisplayManager.WindowHeight - 10
            );

        /// <summary>
        /// The messages displayed when the game is paused.
        /// </summary>
        public static readonly string[] PauseScreenMessages = new[] 
        {
            "PAUSED",
            "PRESS ESCAPE TO RESUME"
        };

        /// <summary>
        /// The centers of each message displayed when the game is paused.
        /// </summary>
        public static readonly Vector2[] PauseScreenMessageCenters = new[] 
        {
            new Vector2(DisplayManager.WindowWidth / 2f, DisplayManager.WindowHeight / 2f - 25),
            new Vector2(DisplayManager.WindowWidth / 2f, DisplayManager.WindowHeight / 2f + 25)
        };

        /// <summary>
        /// The colour of the text displayed when the game is paused.
        /// </summary>
        public static readonly Color PauseTextColour = Color.White;

        #endregion

        /// <summary>
		/// The LevelNameInfo sent in from the previous multiform.
		/// </summary>
		public LevelNameInfo LevelNameInfo { get; private set; }

        /// <summary>
        /// Whether or not the level is paused.
        /// </summary>
        public bool Paused { get; private set; }

        public override void Construct(MultiformTransmissionData args)
        {
            LevelNameInfo = args == null ? new LevelNameInfo(0) : args.GetAttr<LevelNameInfo>("LevelNameInfo");
            var levelInfo = args.GetAttr<LevelInfo>("LevelInfo");

            RegisterForm(BoardFormName, new BoardForm(levelInfo));
            RegisterForm(InventoryFormName, new InventoryForm(levelInfo));
            RegisterForm(BackButtonFormName, new GUIButton(
                BackButtonInfo, BackButtonBottomLeft, PositionType.BottomLeft));

            FadeIn(20, Color.White, Update_Main, Render_Main);
        }

        public void Update_Main()
        {
            base.UpdateTime();

            if (KeyboardInput.IsReleased(Keys.Escape))
            {
                Paused ^= true;
            }

            if (!Paused)
            {
                UpdateForms();

                var form = GetForm<BoardForm>(BoardFormName);
                if (form.LevelComplete)
                {
                    if (LevelNameInfo.Sequential)
                        LoadedLevelManager.CompletedLevels.Add(LevelNameInfo.LevelNumber.Value);

                    var data = new MultiformTransmissionData(MultiformName);
                    data.SetAttr<LevelNameInfo>("LevelNameInfo", LevelNameInfo);

                    FadeOutAndClose(
                        20, Color.White, LevelCompleteMultiform.MultiformName, 
                        data, true, () => { UpdateForms(); }, Render_Main);
                    
                }
                else if (GetForm<GUIButton>(BackButtonFormName).IsReleased(MouseButtons.Left))
                {
                    FadeOutAndClose(
                        20, Color.White, LevelSelectMultiform.MultiformName,
                        null, true, () => { UpdateForms(); }, Render_Main);
                }
            }
        }

        public void Render_Main()
        {
            RenderForms(BoardFormName, InventoryFormName, BackButtonFormName);
            
            if (Paused)
            {
                Render_PauseOverlay();
            }
        }

        private void Render_PauseOverlay()
        {
            var dummyTexture = new Texture2D(DisplayManager.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Black * 0.75f });

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            DisplayManager.Draw(
                dummyTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 
                new Vector2(DisplayManager.WindowWidth, DisplayManager.WindowHeight), 
                SpriteEffects.None, 0f);

            DisplayManager.ClearSpriteBatchProperties();

            for (int i = 0; i < PauseScreenMessages.Length; i++)
            {
                RenderPauseMessage(PauseScreenMessages[i], PauseScreenMessageCenters[i]);
            }
        }

        private void RenderPauseMessage(string message, Vector2 center)
        {
            var dimensions = Assets.Level.Fonts.PauseText.MeasureString(message);
            var topleft = PositionConverter.ToTopLeft(center, dimensions.X, dimensions.Y, PositionType.Center);
            DisplayManager.DrawString(Assets.Level.Fonts.PauseText, message, topleft, PauseTextColour);
        }

    }
}
