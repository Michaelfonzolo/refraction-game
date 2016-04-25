using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DemeterEngine;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms
{
    public class ComplexTextForm : Form, ITransitionalForm
    {

        public List<string> Messages = new List<string>();

        private enum ItemRenderType
        {
            Message, Sprite
        }

        private struct RenderOrderItem
        {
            public ItemRenderType type { get; set; }
            public string message { get; set; }
            public Sprite sprite { get; set; }
        }

        private List<RenderOrderItem> RenderOrder = new List<RenderOrderItem>();

        private Vector2 Offset;

        public Vector2 Dimensions { get; private set; }

        public SpriteFont Font { get; private set; }

        public Color Colour { get; private set; }

        public float Alpha { get; private set; }

        public ComplexTextForm(
            string msg, Vector2 center, SpriteFont font, Color colour, List<Sprite> sprites = null)
        {
            var matches = Regex.Matches(msg, @"<<Image \| \d+>>");
            var lastStart = 0;
            foreach (Match match in matches)
            {
                var end = match.Index;
                if (lastStart != end)
                {
                    RenderOrder.Add(
                        new RenderOrderItem 
                        {
                            type = ItemRenderType.Message,
                            message = msg.Substring(lastStart, end),
                            sprite = null
                        });
                }
                lastStart = end + match.Value.Length;

                var numSegment = match.Value.Substring(10);
                var num = numSegment.Substring(0, numSegment.Length - 2);
                var index = Convert.ToInt32(num) - 1;
                RenderOrder.Add(
                    new RenderOrderItem
                    {
                        type = ItemRenderType.Sprite,
                        sprite = sprites[index],
                        message = null
                    });
            }

            if (lastStart != msg.Length - 2)
            {
                RenderOrder.Add(
                    new RenderOrderItem
                    {
                        type = ItemRenderType.Message,
                        message = msg.Substring(lastStart),
                        sprite = null
                    }); 
            }

            Dimensions = Vector2.Zero;
            foreach (var item in RenderOrder)
            {
                switch (item.type)
                {
                    case ItemRenderType.Message:
                        Dimensions += font.MeasureString(item.message);
                        break;
                    case ItemRenderType.Sprite:
                        Dimensions += item.sprite.Bounds;
                        break;
                }
            }

            Offset = center - Dimensions / 2f;
            Font = font;
            Colour = colour;
        }

        public override void Render()
        {
            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            var colour = new Color(Colour, (int)Alpha);

            float xOffset = Offset.X, yOffset;
            Vector2 delta;
            foreach (var item in RenderOrder)
            {
                switch (item.type)
                {
                    case ItemRenderType.Message:
                        delta = Font.MeasureString(item.message);
                        yOffset = Offset.Y + (Dimensions.Y - delta.Y) / 2f;

                        DisplayManager.DrawString(Font, item.message, new Vector2(xOffset, yOffset), colour);

                        xOffset += delta.X;
                        break;
                    case ItemRenderType.Sprite:
                        delta = item.sprite.Bounds;
                        yOffset = Offset.Y + (Dimensions.Y - delta.Y) / 2f;

                        item.sprite.Alpha = Alpha;
                        item.sprite.Render(new Vector2(xOffset, yOffset));

                        xOffset += delta.X;
                        break;
                }
            }

            DisplayManager.ClearSpriteBatchProperties();
        }

        public void SetAlpha(float alpha)
        {
            Alpha = alpha;
        }

        public void SetPosition(Vector2 vec, PositionType positionType)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetPosition(PositionType positionType)
        {
            return Offset;
        }
    }
}
