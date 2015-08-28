using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Refraction_V2.Multiforms.Level;

namespace Refraction_V2.Multiforms.LevelLoad
{
    public class LevelLoadMultiform : RefractionGameMultiform
    {

        public const string MultiformName = "LevelLoad";

        public const int MESSAGE_LINE_GAP = 20;

        public const int ERROR_WARNING_GAP = 50;

        public const int MAIN_ERROR_MESSAGE_OFFSET_Y = 100;

        public const string FATAL_ERROR_MESSAGE = "A fatal error occurred when loading the level:";

        public const string NON_FATAL_ERROR_MESSAGE = "The following non-fatal error(s) occurred when loading the level:";

        public const string ADDITIONAL_NON_FATAL_ERROR_MESSAGE = "The following non-fatal error(s) occurred as well:";

        public const string PRESS_TO_COPY_MESSAGE = "Press CTRL + C to copy these messages, or click to continue.";

        public const string MESSAGES_COPIED_MESSAGE = "Messages copied! Click to continue.";

        public static readonly Microsoft.Xna.Framework.Input.Keys CTRL = Microsoft.Xna.Framework.Input.Keys.LeftControl;

        public static readonly Microsoft.Xna.Framework.Input.Keys C = Microsoft.Xna.Framework.Input.Keys.C;

        public const string PressToCopyMessageFormName = "PressToCopyMessage";

        public LevelNameInfo LevelNameInfo { get; private set; }

        public LevelInfo LevelInfo { get; private set; }

        public List<string> WarningMessages = new List<string>();

        private bool Error = false;

        public string ErrorMessage { get; private set; }

        public MultiformTransmissionData TransmissionData { get; private set; }

        public override void Construct(MultiformTransmissionData args)
        {
            WarningMessages.Clear();
            LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");

            try
            {
                LevelInfo = new LevelInfo(LevelNameInfo.LevelName);
            }
            catch (LevelLoadException ex)
            {
                ErrorMessage = ex.Message;
                Error = true;
                LevelInfo = null;
            }

            if (LevelInfo.WarningMessages.Count > 0)
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
            var levelName = LevelNameInfo.Sequential ? (LevelNameInfo.LevelNumber + 1).ToString()
                                                     : LevelNameInfo.LevelName;
            var textFont = Assets.Level.Fonts.PauseText;
            var errorFont = Assets.Shared.Fonts.Error;

            var center = DisplayManager.WindowResolution.Center;

            var textHeight = GetTextHeight(textFont, errorFont);
            var currentCenter = center - new Vector2(0, (DisplayManager.WindowHeight - textHeight) / 2f);
            if (Error)
            {
                RegisterMessage(FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                RegisterMessage(ErrorMessage, errorFont, ERROR_WARNING_GAP, ref currentCenter);
                
                if (WarningMessages.Count > 0)
                {
                    RegisterMessage(ADDITIONAL_NON_FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                    foreach (var message in WarningMessages)
                    {
                        RegisterMessage(message, errorFont, MESSAGE_LINE_GAP, ref currentCenter);
                    }
                    // Add a little extra padding.
                    currentCenter += new Vector2(0, ERROR_WARNING_GAP - MESSAGE_LINE_GAP);
                }
            }
            else if (WarningMessages.Count > 0)
            {
                RegisterMessage(NON_FATAL_ERROR_MESSAGE, textFont, MESSAGE_LINE_GAP, ref currentCenter);
                foreach (var message in WarningMessages)
                {
                    RegisterMessage(message, errorFont, MESSAGE_LINE_GAP, ref currentCenter);
                }
                currentCenter += new Vector2(0, ERROR_WARNING_GAP - MESSAGE_LINE_GAP);
            }

            if (Error || WarningMessages.Count > 0)
            {
                RegisterMessage(
                    PRESS_TO_COPY_MESSAGE, textFont, 0, ref currentCenter, name: PressToCopyMessageFormName);
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
            if (Error)
            {
                textHeight += textFont.MeasureString(FATAL_ERROR_MESSAGE).Y;
                textHeight += errorFont.MeasureString(ErrorMessage).Y;
                textHeight += MESSAGE_LINE_GAP + ERROR_WARNING_GAP;
                
                if (WarningMessages.Count > 0)
                {
                    textHeight += textFont.MeasureString(ADDITIONAL_NON_FATAL_ERROR_MESSAGE).Y;
                    textHeight += MESSAGE_LINE_GAP;
                    foreach (var message in WarningMessages)
                    {
                        textHeight += errorFont.MeasureString(message).Y;
                        textHeight += MESSAGE_LINE_GAP;
                    }
                    textHeight += ERROR_WARNING_GAP - MESSAGE_LINE_GAP;
                }
            }
            else if (WarningMessages.Count > 0)
            {
                textHeight += textFont.MeasureString(NON_FATAL_ERROR_MESSAGE).Y;
                textHeight += MESSAGE_LINE_GAP;
                foreach (var message in WarningMessages)
                {
                    textHeight += errorFont.MeasureString(message).Y;
                    textHeight += MESSAGE_LINE_GAP;
                }
                textHeight += ERROR_WARNING_GAP - MESSAGE_LINE_GAP;
            }

            if (Error || WarningMessages.Count > 0)
            {
                textHeight += textFont.MeasureString(PRESS_TO_COPY_MESSAGE).Y;
            }

            return textHeight;
        }

        private void RegisterMessage(string message, SpriteFont font, int gap, ref Vector2 center, string name = null)
        {
            var form = new SimpleTextForm(message, font, center, true);
            if (name != null)
            {
                RegisterForm(name, form);
            }
            else
            {
                RegisterForm(form);
            }
            center += new Vector2(0, font.MeasureString(message).Y + gap);
        }

        public void Update_Main()
        {
            UpdateForms();

            if (WarningMessages.Count == 0 && !Error)
            {
                ExitTo(LevelMultiform.MultiformName, TransmissionData);
            }
            else
            {
                if (KeyboardInput.IsPressed(C) && KeyboardInput.IsReleased(CTRL))
                {
                    var messages = new List<string>(WarningMessages);
                    if (Error)
                    {
                        messages.Insert(0, ErrorMessage);
                    }
                    Clipboard.SetText(String.Join("\n", messages));
                    GetForm<SimpleTextForm>(PressToCopyMessageFormName).SetText(MESSAGES_COPIED_MESSAGE);
                }
                if (MouseInput.IsReleased(DemeterEngine.Input.MouseButtons.Left))
                {
                    if (Error)
                    {
                        ExitTo(LevelSelect.LevelSelectMultiform.MultiformName);
                    }
                    else
                    {
                        ExitTo(LevelMultiform.MultiformName, TransmissionData);
                    }
                }
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
        }

    }
}
