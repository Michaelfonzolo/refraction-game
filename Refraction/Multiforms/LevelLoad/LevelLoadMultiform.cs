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

/* 
 * Author: Michael Ala
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Refraction_V2.Multiforms.Effectors;
using Refraction_V2.Multiforms.Level;
using Refraction_V2.Multiforms.LevelSelect;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace Refraction_V2.Multiforms.LevelLoad
{

    /// <summary>
    /// The multiform that appears inbetween choosing a level on the level select menu
    /// and the level multiform itself. This multiform is only really visible if an error
    /// occurs in loading the level, in which case some user-friendly messages will be
    /// displayed. Otherwise, the level just gets loaded and LevelMultiform is constructed.
    /// </summary>
    public class LevelLoadMultiform : RefractionGameMultiform
    {

        #region Form Name Constants

        public const string MultiformName = "LevelLoad";

        /// <summary>
        /// The name of the message PRESS_TO_COPY_MESSAGE. The reason we need a name for this form
        /// specifically is because if the user presses CTRL + C, then 
        /// </summary>
        public const string PressToCopyMessageFormName = "PressToCopyMessage";

        #endregion

        #region Form Property Constants and Messages

        private const int MESSAGE_LINE_GAP = 50;

        private const int ERROR_WARNING_GAP = 100;

        /// <summary>
        /// The maximum number of warning messages displayed on screen at once. This is
        /// to prevent text overflow past the boundaries of the screen.
        /// </summary>
        private const int MAX_WARNING_MESSAGES_DISPLAYED = 3;

        private const string FATAL_ERROR_MESSAGE = "A fatal error occurred when loading the level:";

        private const string NON_FATAL_ERROR_MESSAGE = "The following non-fatal error(s) occurred when loading the level:";

        private const string ADDITIONAL_NON_FATAL_ERROR_MESSAGE = "The following non-fatal error(s) occurred as well:";

        private const string PRESS_TO_COPY_MESSAGE = "Press CTRL + C to copy these messages, or click to continue.";

        private const string MESSAGES_COPIED_MESSAGE = "Messages copied! Click to continue.";

        #endregion

        // These are just here because I didn't want to type their full
        // names in the Update method.
        public static readonly Microsoft.Xna.Framework.Input.Keys CTRL = Microsoft.Xna.Framework.Input.Keys.LeftControl;
        public static readonly Microsoft.Xna.Framework.Input.Keys C = Microsoft.Xna.Framework.Input.Keys.C;

        /// <summary>
        /// The LevelNameInfo object associated with the loaded level.
        /// </summary>
        public LevelNameInfo LevelNameInfo { get; private set; }

        /// <summary>
        /// The LevelInfo object describing the properties of the level.
        /// </summary>
        public LevelInfo LevelInfo { get; private set; }

        /// <summary>
        /// Any warning messages that occurred when loading the level.
        /// </summary>
        public List<string> WarningMessages = new List<string>();

        /// <summary>
        /// Whether or not an error occurred.
        /// </summary>
        private bool FatalError = false;

        /// <summary>
        /// The error message (if one occurred).
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The data to pass on to the LevelMultiform.
        /// </summary>
        public MultiformTransmissionData TransmissionData { get; private set; }

        /// <summary>
        /// The list of all registered TextForm objects. We keep a list of them so we can easily
        /// iterate through them and fade them in over time.
        /// </summary>
        public List<TextForm> RegisteredTextForms = new List<TextForm>();

        /// <summary>
        /// The time (frame number) when the user pressed CTRL + C to copy the warning/error
        /// messages. When this is set to any value other than -1, it activates an if (At(time))
        /// block in the main update method which fades out the PressToCopyMessage TextForm and
        /// fades in a new one displaying the MESSAGES_COPIED_MESSAGE.
        /// </summary>
        private int TimeOfMessagesCopied = -1;

        public override void Construct(MultiformTransmissionData args)
        {
            // Reinitialize variables that may have been carried over from the last
            // time the multiform was loaded.
            TimeOfMessagesCopied = -1;
            ResetTime();
            WarningMessages.Clear();
            RegisteredTextForms.Clear();

            LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");
            LevelInfo = new LevelInfo(LevelNameInfo.LevelName);

            FatalError = LevelInfo.Exception != null;
            if (FatalError)
            {
                ErrorMessage = LevelInfo.Exception.Message;
            }

            if (LevelInfo != null && LevelInfo.WarningMessages.Count > 0)
            {
                WarningMessages.AddRange(LevelInfo.WarningMessages);
            }

            TransmissionData = new MultiformTransmissionData(MultiformName);
            TransmissionData.SetAttr<LevelNameInfo>("LevelNameInfo", LevelNameInfo);
            TransmissionData.SetAttr<LevelInfo>("LevelInfo", LevelInfo);

            ConstructMessageForms();

            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
        }

        private void ConstructMessageForms()
        {
            var textFont = Assets.LevelLoad.Fonts.PlainMessage;
            var errorFont = Assets.LevelLoad.Fonts.Error;

            var center = DisplayManager.WindowResolution.Center;

            var textHeight = GetTextHeight(textFont, errorFont);
            var currentCenter = new Vector2(
                DisplayManager.WindowWidth / 2f, 
                (DisplayManager.WindowHeight - textHeight) / 2f);
            if (FatalError)
            {
                RegisterMessage(FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                RegisterMessage(ErrorMessage, errorFont, ERROR_WARNING_GAP, ref currentCenter);
                
                if (WarningMessages.Count > 0)
                {
                    RegisterMessage(ADDITIONAL_NON_FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                    RegisterWarningMessages(errorFont, ref currentCenter);
                }
            }
            else if (WarningMessages.Count > 0)
            {
                RegisterMessage(NON_FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                RegisterWarningMessages(errorFont, ref currentCenter);
            }

            if (FatalError || WarningMessages.Count > 0)
            {
                RegisterMessage(
                    PRESS_TO_COPY_MESSAGE, textFont, 0, ref currentCenter, name: PressToCopyMessageFormName);
            }
        }

        /// <summary>
        /// Register the warning messages.
        /// </summary>
        /// <param name="errorFont"></param>
        /// <param name="currentCenter"></param>
        private void RegisterWarningMessages(SpriteFont errorFont, ref Vector2 currentCenter)
        {
            for (int i = 0; i < Math.Min(MAX_WARNING_MESSAGES_DISPLAYED, WarningMessages.Count); i++)
            {
                RegisterMessage(WarningMessages[i], errorFont, MESSAGE_LINE_GAP, ref currentCenter);
            }

            if (WarningMessages.Count > MAX_WARNING_MESSAGES_DISPLAYED)
            {
                var msg = String.Format("({0} more...)", WarningMessages.Count - MAX_WARNING_MESSAGES_DISPLAYED);
                RegisterMessage(msg, errorFont, ERROR_WARNING_GAP, ref currentCenter);
            }
            else
            {
                // Add remaining padding.
                currentCenter += new Vector2(0, ERROR_WARNING_GAP - MESSAGE_LINE_GAP);
            }
        }

        /// <summary>
        /// Determine the height of all the text combined.
        /// </summary>
        /// <param name="textFont"></param>
        /// <param name="errorFont"></param>
        /// <returns></returns>
        private float GetTextHeight(SpriteFont textFont, SpriteFont errorFont)
        {
            var textHeight = 0f;
            if (FatalError)
            {
                textHeight += textFont.MeasureString(FATAL_ERROR_MESSAGE).Y;
                textHeight += errorFont.MeasureString(ErrorMessage).Y;
                textHeight += MESSAGE_LINE_GAP + ERROR_WARNING_GAP;
                
                if (WarningMessages.Count > 0)
                {
                    textHeight += textFont.MeasureString(ADDITIONAL_NON_FATAL_ERROR_MESSAGE).Y;
                    textHeight += MESSAGE_LINE_GAP;

                    for (int i = 0; i < Math.Min(MAX_WARNING_MESSAGES_DISPLAYED, WarningMessages.Count); i++ )
                    {
                        textHeight += errorFont.MeasureString(WarningMessages[i]).Y;
                        textHeight += MESSAGE_LINE_GAP;
                    }
                    textHeight += ERROR_WARNING_GAP - MESSAGE_LINE_GAP;
                }
            }
            else if (WarningMessages.Count > 0)
            {
                textHeight += textFont.MeasureString(NON_FATAL_ERROR_MESSAGE).Y;
                textHeight += MESSAGE_LINE_GAP;
                for (int i = 0; i < Math.Min(MAX_WARNING_MESSAGES_DISPLAYED, WarningMessages.Count); i++)
                {
                    textHeight += errorFont.MeasureString(WarningMessages[i]).Y;
                    textHeight += MESSAGE_LINE_GAP;
                }
                textHeight += ERROR_WARNING_GAP - MESSAGE_LINE_GAP;
            }

            if (FatalError || WarningMessages.Count > 0)
            {
                textHeight += textFont.MeasureString(PRESS_TO_COPY_MESSAGE).Y;
            }

            return textHeight;
        }

        private void RegisterMessage(string message, SpriteFont font, int gap, ref Vector2 center, string name = null)
        {
            var form = new TextForm(message, font, center, Color.TransparentBlack, true);
            if (name != null)
            {
                RegisterForm(name, form);
            }
            else
            {
                RegisterForm(form);
            }
            RegisteredTextForms.Add(form);

            center += new Vector2(0, font.MeasureString(message).Y + gap);
        }

        // Ambiguous name, I know.
        private void FadeTo(string multiformName)
        {
            FadeOutAndClose(
                20, Color.White, multiformName, TransmissionData, 
                true, () => { UpdateForms(); }, Render_Main);
        }

        public void Update_Main()
        {
            UpdateTime();
            UpdateForms();

            if (WarningMessages.Count == 0 && !FatalError)
            {
                FadeTo(LevelMultiform.MultiformName);
            }
            else
            {
                if ((KeyboardInput.IsPressed(C) && KeyboardInput.IsReleased(CTRL)) ||
                    (KeyboardInput.IsReleased(C) && KeyboardInput.IsPressed(CTRL)))
                {
                    var messages = new List<string>(WarningMessages);
                    if (FatalError)
                    {
                        messages.Insert(0, ErrorMessage);
                    }
                    Clipboard.SetText(String.Join("\n", messages));

                    GetForm(PressToCopyMessageFormName).AddEffector(new FadeOutEffector(15));

                    // Setting this adds a MESSAGES_COPIED_MESSAGE TextForm, since it activates
                    // the if (AtFrame(TimeOfMessagesCopied)) if statement below.
                    TimeOfMessagesCopied = LocalFrame + 20;
                }
                if (MouseInput.IsReleased(DemeterEngine.Input.MouseButtons.Left))
                {
                    if (FatalError)
                    {
                        FadeTo(LevelSelectMultiform.MultiformName);
                    }
                    else
                    {
                        FadeTo(LevelMultiform.MultiformName);
                    }
                }
            }

            for (int i = 0; i < RegisteredTextForms.Count; i++)
            {
                var textForm = RegisteredTextForms[i];
                if (AtFrame((int)(textForm.Position.Y / 5f)))
                {
                    textForm.AddEffector(new FadeInEffector(15));
                }
            }

            if (AtFrame(TimeOfMessagesCopied))
            {
                var pos = GetForm<TextForm>(PressToCopyMessageFormName).Position;
                var newForm = new TextForm(
                    MESSAGES_COPIED_MESSAGE, Assets.LevelLoad.Fonts.PlainMessage, pos, Color.TransparentBlack);
                newForm.AddEffector(new FadeInEffector(15));
                RegisterForm(newForm);
            }
        }

        public void Render_Main()
        {
            var dummyTexture = new Texture2D(DisplayManager.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            DisplayManager.Draw(
                dummyTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,
                new Vector2(DisplayManager.WindowWidth, DisplayManager.WindowHeight),
                SpriteEffects.None, 0f);

            RenderForms();
        }

    }
}
