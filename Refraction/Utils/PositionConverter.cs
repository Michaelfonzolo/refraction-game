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

using DemeterEngine.Collision;

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Refraction_V2.Utils
{
    public static class PositionConverter
    {

        public static Vector2 ToType(
            Vector2 position, double width, double height, PositionType fromType, PositionType toType)
        {
            var rect = ToRectangle(position, width, height, fromType);
            switch (toType)
            {
                case PositionType.TopLeft:
                    return rect.TopLeft;
                case PositionType.TopRight:
                    return rect.TopRight;
                case PositionType.BottomLeft:
                    return rect.BottomLeft;
                case PositionType.BottomRight:
                    return rect.BottomRight;
                case PositionType.Center:
                    return rect.Center;
                case PositionType.TopCenter:
                    return rect.TopCenter;
                case PositionType.BottomCenter:
                    return rect.BottomCenter;
                case PositionType.LeftCenter:
                    return rect.LeftCenter;
                case PositionType.RightCenter:
                    return rect.RightCenter;
                default:
                    throw new ArgumentException("Invalid PositionType supplied.");
            }
        }

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
