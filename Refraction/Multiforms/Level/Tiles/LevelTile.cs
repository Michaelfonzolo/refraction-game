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
 * A LevelTile represents a single tile on a board in a level. It is responsible for
 * drawing itself, and determining what to do to a laser entering it.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Graphics;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{

	// Note: Despite the similarity of this class to ButtonForm (both classes have
	// a CollidingWithMouse and a MousePressed field), the value of CollidingWithMouse
    // is determined by BoardForm (via spatial indexing), so it makes more sense not
	// to inherit from ButtonForm.

    public abstract class LevelTile : Form
    {

		/// <summary>
		/// Whether or not the mouse is colliding with this form.
		/// </summary>
        public bool CollidingWithMouse { get; internal set; }

		/// <summary>
		/// Whether or not the mouse is colliding with this tile and the mouse is pressed
		/// (MouseButtons.Left).
		/// </summary>
        public Dictionary<MouseButtons, bool> MousePressed { get; private set; }

		/// <summary>
		/// Whether or not the tile is open. An open tile is a tile that the player can
		/// overwrite with their own inventory.
		/// </summary>
        public bool Open { get; private set; }

		/// <summary>
		/// The topleft corner of this tile.
		/// </summary>
        public Vector2 Position { get; private set; }

		/// <summary>
		/// The center of this tile.
		/// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(
                    Position.X + EmptyTileSprite.Width / 2f,
                    Position.Y + EmptyTileSprite.Height / 2f
                    );
            }
        }

		/// <summary>
		/// The sprite representing an empty tile.
		/// </summary>
        public Sprite EmptyTileSprite { get; private set; }

		/// <summary>
		/// The sprite to draw atop the tile when CollidingWithMouse is true.
		/// </summary>
        public Sprite TileHoverSprite { get; private set; }

        public float HoverSpriteAlpha { get; set; }

        private const float HOVER_ALPHA_INCREMENT = 0.05f;

        private const float HOVER_ALPHA_MIN = 0f;

        private const float HOVER_ALPHA_MAX = 1f;

        private const int HOVER_ALPHA_MULTIPLIER = 100;

        public LevelTile(Vector2 position, bool open)
            : base(false)
        {
            Position = position;
            Open = open;
            HoverSpriteAlpha = 0f;

            EmptyTileSprite = new Sprite(Assets.Level.Images.EmptyTile);
            TileHoverSprite = new Sprite(Assets.Level.Images.TileHover);

            EmptyTileSprite.Position = position;
            TileHoverSprite.Position = position;

            MousePressed = new Dictionary<MouseButtons, bool>();
            foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
                MousePressed[button] = false;
        }

        public override void Update()
        {
            base.Update();

            if (CollidingWithMouse)
            {
                HoverSpriteAlpha += HOVER_ALPHA_INCREMENT;
                HoverSpriteAlpha = Math.Min(HoverSpriteAlpha, HOVER_ALPHA_MAX);
            }
            else
            {
                HoverSpriteAlpha -= HOVER_ALPHA_INCREMENT;
                HoverSpriteAlpha = Math.Max(HoverSpriteAlpha, HOVER_ALPHA_MIN);
            }
        }

        public override void Render()
        {
            if (Open)
            {
                EmptyTileSprite.Render();

                if (HoverSpriteAlpha != 0)
                {
                    DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);
                    TileHoverSprite.Alpha = HOVER_ALPHA_MULTIPLIER * HoverSpriteAlpha;
                    TileHoverSprite.Render();
                    DisplayManager.ClearSpriteBatchProperties();
                }
            }
        }

		/// <summary>
		/// Update the given laser. The given board is the BoardForm the laser was generated by,
		/// and the point is the position of this tile in the board's multidimensional array of
		/// LevelTiles.
		/// </summary>
        public abstract void UpdateLaser(BoardForm board, Laser laser, Point point);

    }
}
