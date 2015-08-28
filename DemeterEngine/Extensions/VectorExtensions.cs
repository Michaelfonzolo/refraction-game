using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DemeterEngine.Maths;

namespace DemeterEngine.Extensions
{
    public static class VectorExtensions
    {

        public static Vector2 ToNormalized(this Vector2 vec)
        {
            return VectorUtils.ToNormalized(vec);
        }

        public static Vector2 LeftNormal(this Vector2 vec)
        {
            return new Vector2(vec.Y, -vec.X);
        }

        public static Vector2 RightNormal(this Vector2 vec)
        {
            return new Vector2(-vec.Y, vec.X);
        }

    }
}
