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
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Multiforms.Effectors;

namespace Refraction_V2.Multiforms.LevelLoad
{
    public class LevelLoadMultiform : RefractionGameMultiform
    {

        public const string MultiformName = "LevelLoad";

        public const int MESSAGE_LINE_GAP = 50;

        public const int ERROR_WARNING_GAP = 100;

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

        public List<TextForm> RegisteredTextForms = new List<TextForm>();

        private int TimeOfMessagesCopied = -1;

        public override void Construct(MultiformTransmissionData args)
        {
            TimeOfMessagesCopied = -1;
            ResetTime();
            WarningMessages.Clear();
            RegisteredTextForms.Clear();

            LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");
            LevelInfo = new LevelInfo(LevelNameInfo.LevelName);

            Error = LevelInfo.Exception != null;
            if (Error)
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
            var levelName = LevelNameInfo.Sequential ? (LevelNameInfo.LevelNumber + 1).ToString()
                                                     : LevelNameInfo.LevelName;
            var textFont = Assets.LevelLoad.Fonts.PlainMessage;
            var errorFont = Assets.LevelLoad.Fonts.Error;

            var center = DisplayManager.WindowResolution.Center;

            var textHeight = GetTextHeight(textFont, errorFont);
            var currentCenter = new Vector2(
                DisplayManager.WindowWidth / 2f, 
                (DisplayManager.WindowHeight - textHeight) / 2f);
            if (Error)
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

            if (Error || WarningMessages.Count > 0)
            {
                RegisterMessage(
                    PRESS_TO_COPY_MESSAGE, textFont, 0, ref currentCenter, name: PressToCopyMessageFormName);
            }
        }

        private void RegisterWarningMessages(SpriteFont errorFont, ref Vector2 currentCenter)
        {
            for (int i = 0; i < Math.Min(3, WarningMessages.Count); i++)
            {
                RegisterMessage(WarningMessages[i], errorFont, MESSAGE_LINE_GAP, ref currentCenter);
            }

            if (WarningMessages.Count > 3)
            {
                var msg = String.Format("({0} more...)", WarningMessages.Count - 3);
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

            if (WarningMessages.Count == 0 && !Error)
            {
                FadeTo(LevelMultiform.MultiformName);
            }
            else
            {
                if ((KeyboardInput.IsPressed(C) && KeyboardInput.IsReleased(CTRL)) ||
                    (KeyboardInput.IsReleased(C) && KeyboardInput.IsPressed(CTRL)))
                {
                    var messages = new List<string>(WarningMessages);
                    if (Error)
                    {
                        messages.Insert(0, ErrorMessage);
                    }
                    Clipboard.SetText(String.Join("\n", messages));

                    GetForm(PressToCopyMessageFormName).AddEffector(new FadeOutEffector(15));
                    TimeOfMessagesCopied = LocalFrame + 20;
                }
                if (MouseInput.IsReleased(DemeterEngine.Input.MouseButtons.Left))
                {
                    if (Error)
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
