using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms
{
    public interface ITransitionalForm
    {

        Vector2 GetPosition(PositionType positionType = PositionType.TopLeft);

        void SetAlpha(float alpha);

        void SetPosition(Vector2 vec, PositionType positionType);

    }
}
