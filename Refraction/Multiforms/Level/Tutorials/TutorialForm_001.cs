using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.Effectors;

namespace Refraction_V2.Multiforms.Level.Tutorials
{
    public class TutorialForm_001 : Form
    {

        BoardForm Board;
        InventoryForm Inventory;
        List<Form> TextForms;
        Action CurrentUpdater;

        float TOP_MESSAGE_Y_VALUE;

        public TutorialForm_001()
            : base(true) 
        {
            TextForms = new List<Form>();
        }

        /// <summary>
        /// Post-construct gets called after the form is added to a multiform
        /// (i.e. Parent != null).
        /// </summary>
        public override void PostConstruct()
        {
            base.PostConstruct();

            Board = Parent.GetForm<BoardForm>(LevelMultiform.BoardFormName);
            Inventory = Parent.GetForm<InventoryForm>(LevelMultiform.InventoryFormName);

            TOP_MESSAGE_Y_VALUE = (float)Board.BoardCollider.Y / 2f;

            // Set up the various sequential updaters.

            var Updater_001          = new DefaultUpdater();
            Updater_001.form         = this;
            Updater_001.InitialFrame = 70;
            Updater_001.Predicate    = () => Board.TileAdded;
            Updater_001.Messages     = new List<MessageInfo>() {
                new MessageInfo {
                    Frame    = 70,
                    Message  = "Click on one of the <<Image | 1>>'s to place a refractor.",
                    Position = new Vector2(DisplayManager.WindowWidth / 2f, TOP_MESSAGE_Y_VALUE),
                    Font     = Assets.Level.Fonts.TutorialMessage_Large,
                    Sprites  = new List<Sprite>() { new Sprite(Assets.Level.Images.EmptyTile) },
                }
            };

            var Updater_002          = new DefaultUpdater();
            Updater_002.form         = this;
            Updater_002.InitialFrame = 20;
            Updater_002.Predicate = () => Board.TileRemoved;
            Updater_002.Messages     = new List<MessageInfo>() {
                new MessageInfo {
                    Frame    = 20,
                    Message  = "You can right click to remove a refractor.",
                    Position = new Vector2(DisplayManager.WindowWidth / 2f, TOP_MESSAGE_Y_VALUE),
                    Font     = Assets.Level.Fonts.TutorialMessage_Large,
                    Sprites  = null
                }
            };

            var Updater_003          = new DefaultUpdater();
            Updater_003.form         = this;
            Updater_003.InitialFrame = 20;
            Updater_003.Predicate = () => Inventory.SelectionChanged;
            var y                    = ((RectCollider)Inventory.InventoryButtons[TileType.RF_UxR_UR].Collider).Center.Y;
            Updater_003.Messages     = new List<MessageInfo>() {
                new MessageInfo {
                    Frame    = 20,
                    Message  = "Select a different refractor.",
                    Position = new Vector2(DisplayManager.WindowWidth - 170, y),
                    Font     = Assets.Level.Fonts.TutorialMessage_Small,
                    Sprites  = null
                }
            };

            var Updater_004          = new DefaultUpdater();
            Updater_004.form         = this;
            Updater_004.InitialFrame = 20;
            Updater_004.Predicate    = () => Board.IsReceiverActivated(LaserColours.Red);
            var red_receiver         = new Sprite(Assets.Level.Images.Receiver);
            red_receiver.Tint        = LaserColours.Red.Color;
            Updater_004.Messages     = new List<MessageInfo>() {
                new MessageInfo {
                    Frame    = 20,
                    Message  = "Light up <<Image | 1>>",
                    Position = new Vector2(DisplayManager.WindowWidth / 2f, TOP_MESSAGE_Y_VALUE),
                    Font     = Assets.Level.Fonts.TutorialMessage_Large,
                    Sprites  = new List<Sprite>() { red_receiver }
                }
            };

            var Updater_005          = new DefaultUpdater();
            Updater_005.form         = this;
            Updater_005.InitialFrame = 20;
            Updater_005.Predicate    = () => Board.IsReceiverActivated(LaserColours.Blue);
            var blue_receiver        = new Sprite(Assets.Level.Images.Receiver);
            blue_receiver.Tint       = LaserColours.Blue.Color;
            Updater_005.Messages     = new List<MessageInfo>() {
                new MessageInfo {
                    Frame    = 20,
                    Message  = "Good! Now light up <<Image | 1>>",
                    Position = new Vector2(DisplayManager.WindowWidth / 2f, (float)Board.BoardCollider.Y / 3f),
                    Font     = Assets.Level.Fonts.TutorialMessage_Large,
                    Sprites  = new List<Sprite>() { blue_receiver }
                },
                new MessageInfo {
                    Frame    = 300,
                    Message  = "(Hint: Use a different refractor)",
                    Position = new Vector2(DisplayManager.WindowWidth / 2f, (float)Board.BoardCollider.Y / 3f * 2f),
                    Font     = Assets.Level.Fonts.TutorialMessage_Small,
                    Sprites  = null
                }
            };
            
            Updater_001.NextUpdater = Updater_002;
            Updater_002.NextUpdater = Updater_003;
            Updater_003.NextUpdater = Updater_004;
            Updater_004.NextUpdater = Updater_005;
            Updater_005.NextUpdaterAction = () => { };

            CurrentUpdater = Updater_001.Update;
        }

        private void RegisterTextForm(string text, Vector2 center, SpriteFont font, List<Sprite> sprites)
        {
            var newTextForm = new ComplexTextForm(text, center, font, Color.White, sprites);
            ((ITransitionalForm)newTextForm).SetAlpha(0);
            newTextForm.AddEffector(new FadeInEffector(15));

            TextForms.Add(newTextForm);
        }

        public override void Update()
        {
            base.Update();
            CurrentUpdater();
        }

        private struct MessageInfo
        {
            public int Frame { get; set; }
            public string Message { get; set; }
            public Vector2 Position { get; set; }
            public SpriteFont Font { get; set; }
            public List<Sprite> Sprites { get; set; }
        }

        private class DefaultUpdater
        {
            public TutorialForm_001 form;
            public int InitialFrame;
            public List<MessageInfo> Messages;
            public Func<bool> Predicate;
            public DefaultUpdater NextUpdater;
            public Action NextUpdaterAction;

            private bool transitioning = false;

            public void Update()
            {
                if (form.AtFrame(InitialFrame))
                {
                    form.TextForms.Clear();
                }

                foreach (var message in Messages)
                {
                    if (form.AtFrame(message.Frame))
                        form.RegisterTextForm(
                            message.Message, message.Position, 
                            message.Font, message.Sprites);
                }

                if (form.TextForms.Count > 0)
                {
                    foreach (var textForm in form.TextForms)
                    {
                        textForm.Update();
                    }

                    if (Predicate != null && Predicate() && !transitioning)
                    {
                        transitioning = true;

                        foreach (var textForm in form.TextForms)
                        {
                            textForm.AddEffector(new FadeOutEffector(15));
                        }
                        
                        form.ResetTime();

                        if (NextUpdater != null)
                        {
                            form.CurrentUpdater = NextUpdater.Update;
                        }
                        else if (NextUpdaterAction != null)
                        {
                            form.CurrentUpdater = NextUpdaterAction;
                        }
                    }
                }
            }
        }

        public override void Render()
        {
            if (TextForms.Count > 0)
            {
                DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

                foreach (var textForm in TextForms)
                {
                    textForm.Render();
                }

                DisplayManager.ClearSpriteBatchProperties();
            }
        }
    }
}
