using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Collision;
using Microsoft.Xna.Framework;

namespace Refraction_V2.Utils
{
    public enum PositionType
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
        TopCenter,
        BottomCenter,
        LeftCenter,
        RightCenter,
    }

    public static class PositionConverter
    {

        public static RectCollider ToRectangle(
            Vector2 position, double width, double height, PositionType positionType)
        {
            var x = position.X;
            var y = position.Y;
            

            switch (positionType)
            {
                case PositionType.TopLeft:
                    return new RectCollider(x, y, width, height);
                case PositionType.TopRight:
                    return new RectCollider(x - width, y, width, height);
                case PositionType.BottomLeft:
                    return new RectCollider(x, y - height, width, height);
                case PositionType.BottomRight:
                    return new RectCollider(x - width, y - height, width, height);
                case PositionType.Center:
                    return new RectCollider(x - width / 2f, y - height / 2f, width, height);
                case PositionType.TopCenter:
                    return new RectCollider(x - width / 2f, y, width, height);
                case PositionType.BottomCenter:
                    return new RectCollider(x - width / 2f, y - height, width, height);
                case PositionType.LeftCenter:
                    return new RectCollider(x, y - height / 2f, width, height);
                case PositionType.RightCenter:
                    return new RectCollider(x + width, y - height / 2f, width, height);
                default:
                    throw new ArgumentException("Invalid PositionType supplied.");
            }
        }

        public static Vector2 ToTopLeft(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).TopLeft;
        }

        public static Vector2 ToTopRight(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).TopRight;
        }

        public static Vector2 ToBottomLeft(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).BottomLeft;
        }

        public static Vector2 ToBottomRight(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).BottomRight;
        }

        public static Vector2 ToCenter(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).Center;
        }

        public static Vector2 ToTopCenter(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).TopCenter;
        }

        public static Vector2 ToBottomCenter(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).BottomCenter;
        }

        public static Vector2 ToLeftCenter(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).LeftCenter;
        }

        public static Vector2 ToRightCenter(
            Vector2 position, double width, double height, PositionType positionType)
        {
            return ToRectangle(position, width, height, positionType).RightCenter;
        }

    }
}
