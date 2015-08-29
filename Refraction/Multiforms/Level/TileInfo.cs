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
 * The TileInfo class is exactly what it sounds like, a class encapsulating info
 * regarding a specific tile. It is a subclass of AttributeContainer, meaning
 * attributes can be dynamically attached to it, which is why it acts as a common
 * interface for information regarding different tile types.
 */

#endregion

#region Using Statements

using DemeterEngine.Utils;

using Microsoft.Xna.Framework;

using Refraction_V2.Multiforms.Level.Tiles;

using System;

#endregion

namespace Refraction_V2.Multiforms.Level
{
	public class TileInfo : AttributeContainer
	{

		/// <summary>
		/// The type of tile we are representing.
		/// </summary>
		public TileType Type { get; private set; }

		/// <summary>
		/// Whether or not this tile is open. An open tile is one the user can 
		/// overwrite with a tile from their inventory.
		/// </summary>
		public bool Open { get; private set; }

		public TileInfo(TileType type, bool open)
		{
			Type = type;
			Open = open;
		}

		/// <summary>
		/// Convert the tile info to an actual level tile form instance.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public LevelTile ToLevelTile(Vector2 position)
		{
			switch (Type)
			{
				case TileType.Empty:
					return new Empty(position, Open);
				case TileType.Outputter:
					return new Outputter(
						position,
						GetAttr<Directions>(LevelInfo.DIRECTION_ATTRIBUTE),
						GetAttr<LaserColours>(LevelInfo.COLOUR_ATTRIBUTE)
						);
				case TileType.Receiver:
					return new Receiver(
						position,
						GetAttr<Directions>(LevelInfo.DIRECTION_ATTRIBUTE),
						GetAttr<LaserColours>(LevelInfo.COLOUR_ATTRIBUTE)
						);
				case TileType.RF_UxL_UL:
					return new RF_UxL_UL(position, Open);
				case TileType.RF_UxR_UR:
					return new RF_UxR_UR(position, Open);
				case TileType.RF_DxL_DL:
					return new RF_DxL_DL(position, Open);
				case TileType.RF_DxR_DR:
					return new RF_DxR_DR(position, Open);
				case TileType.RF_ULxUR_U:
					return new RF_ULxUR_U(position, Open);
				case TileType.RF_DLxDR_D:
					return new RF_DLxDR_D(position, Open);
				case TileType.RF_ULxDL_L:
					return new RF_ULxDL_L(position, Open);
				case TileType.RF_URxDR_R:
					return new RF_URxDR_R(position, Open);
                case TileType.RF_U_L_and_U_R_pass_UL:
                    return new RF_U_L_and_U_R_pass_UL(position, Open);
                case TileType.RF_U_L_and_U_R_pass_UR:
                    return new RF_U_L_and_U_R_pass_UR(position, Open);
                case TileType.RF_UL_DL_and_UR_DR_pass_L:
                    return new RF_UL_DL_and_UR_DR_pass_L(position, Open);
                case TileType.RF_UL_UR_and_DL_DR_pass_U:
                    return new RF_UL_UR_and_DL_DR_pass_U(position, Open);
				default:
					throw new ArgumentException("Invalid TileType.");
			}
		}
	}
}
