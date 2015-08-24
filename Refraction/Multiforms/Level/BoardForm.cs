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
 * The form representing the actual board the user interacts with.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.Level.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class BoardForm : Form
    {

        /// <summary>
        /// The name of the level currently loaded.
        /// </summary>
        public string LevelName { get; private set; }

        /// <summary>
        /// The dimensions of the board array.
        /// </summary>
        public Point Dimensions { get; private set; }

        /// <summary>
        /// The array of tiles.
        /// </summary>
        public LevelTile[,] Board { get; private set; }

        /// <summary>
        /// The collider representing the entire area the board takes up onscreen.
        /// </summary>
        public RectCollider BoardCollider { get; private set; }

        public LevelInfo LevelInfo { get; private set; }

        /// <summary>
        /// Whether or not we have completed the level.
        /// </summary>
        public bool LevelComplete { get; private set; }

        public List<Laser> Lasers { get; private set; }

        /// <summary>
        /// A flag that indicates to the form when to update the lasers.
        /// </summary>
        private bool BoardChanged = true;

        public BoardForm(LevelInfo levelInfo)
            : base(true)
        {
            LevelInfo = levelInfo;

            var dim = levelInfo.BoardDimensions;
            Dimensions = dim;

            // Find the x and y offsets of the topleft corner of the board.
            var y = (DisplayManager.WindowHeight - LevelInfo.TILE_SIDE_LENGTH * Dimensions.Y) / 2f;
            var x = (DisplayManager.WindowWidth
                - (LevelInfo.TILE_SIDE_LENGTH * dim.X
                   + 2 * LevelInfo.INVENTORY_BUTTON_WIDTH
                   + LevelInfo.BOARD_INVENTORY_GAP
                   )
               ) / 2f;

            BoardCollider = new RectCollider(x, y, LevelInfo.TILE_SIDE_LENGTH * dim.X, LevelInfo.TILE_SIDE_LENGTH * dim.Y);
            Board = new LevelTile[dim.Y, dim.X];

            // Create all the LevelTiles.
            int i = 0;
            foreach (var tileInfo in levelInfo.Board)
            {
                Board[(i / dim.X), (i % dim.X)] = tileInfo.ToLevelTile(
                    new Vector2(
                        x + (i % dim.X) * LevelInfo.TILE_SIDE_LENGTH,
                        y + (dim.Y - i / dim.X - 1) * LevelInfo.TILE_SIDE_LENGTH
                        )
                    );
                i++;
            }

            Lasers = new List<Laser>();
        }

        public override void Update()
        {
            base.Update();

            foreach (var tile in Board)
                tile.Update();

            // Figure out which tile the mouse is colliding with, and then update the board's
            // properties accordingly.

            var mpos = MouseInput.MouseVector;

            // Get the indices of the tile the mouse is colliding with.
            int xIndex = (int)Math.Floor((mpos.X - BoardCollider.X) / LevelInfo.TILE_SIDE_LENGTH);
            int yIndex = Dimensions.Y - (int)Math.Floor((mpos.Y - BoardCollider.Y) / LevelInfo.TILE_SIDE_LENGTH) - 1;

            if (0 <= xIndex && xIndex < Dimensions.X &&
                0 <= yIndex && yIndex < Dimensions.Y &&
                Board[yIndex, xIndex].Open)
            {
                Board[yIndex, xIndex].CollidingWithMouse = true;

                // If the mouse is pressed and it's colliding with a tile, set that tile's
                // MousePressed property to true. When the user releases their mouse while
                // on top of a tile, if that tile's MousePressed property is true, then the
                // user has "clicked" on the tile.
                foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
                {
                    if (MouseInput.IsClicked(button))
                        Board[yIndex, xIndex].MousePressed[button] = true;
                    else if (Board[yIndex, xIndex].MousePressed[button] && MouseInput.IsReleased(button))
                    {
                        switch (button)
                        {
                            case MouseButtons.Left:
                                SetTile(xIndex, yIndex);
                                break;
                            case MouseButtons.Right:
                                RemoveTile(xIndex, yIndex);
                                break;
                            case MouseButtons.Middle:
                                ChooseTile(xIndex, yIndex);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            // Reset the MousePressed and CollidingWithMouse fields of every 
            // tile the mouse isn't colliding with. This prevents the player 
            // from being able to click and drag to place tiles.
            int ix, iy;
            for (int i = 0; i < Board.Length; i++)
            {
                ix = i % Dimensions.X;
                iy = i / Dimensions.Y;
                if (ix == xIndex && iy == yIndex)
                    continue;
                Board[iy, ix].CollidingWithMouse = false;
                foreach (var button in Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>())
                    Board[iy, ix].MousePressed[button] = false;
            }

            if (BoardChanged)
                RetraceLasers();

            foreach (var laser in Lasers)
                laser.Update();
        }

        /// <summary>
        /// Set a tile at the given position on the board.
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        private void SetTile(int xIndex, int yIndex)
        {
            if (!Board[yIndex, xIndex].Open)
                return;

            var inventory = Parent.GetForm<InventoryForm>(LevelMultiform.InventoryFormName);
            if (!inventory.CanPlaceCurrentTile)
                return;

			var prevTile = Board[yIndex, xIndex];
			if (prevTile is RefractorTile)
			{
				var refractor = ((RefractorTile)prevTile).Type;
				if (inventory.InventoryQuantities[refractor] != LevelInfo.INFINITE_ITEMS_IN_INVENTORY)
					inventory.InventoryQuantities[refractor]++;
			}

            var currentlySelected = inventory.CurrentlySelectedTile.Value;
            Board[yIndex, xIndex] = new TileInfo(
                currentlySelected, 
                true)
                .ToLevelTile(Board[yIndex, xIndex].Position);

            inventory.DecrementCurrentTileCount();

            BoardChanged = true;
        }

        /// <summary>
        /// Remove the tile at the given position on the board (if allowed).
        /// </summary>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        private void RemoveTile(int xIndex, int yIndex)
        {
            var refractorTile = Board[yIndex, xIndex] as RefractorTile;

            // The only tiles the player can place and remove are refractors.
            if (refractorTile == null)
                return;

            var tileType = refractorTile.Type;
            var inventory = Parent.GetForm<InventoryForm>(LevelMultiform.InventoryFormName);

            if (inventory.InventoryQuantities[tileType] != LevelInfo.INFINITE_ITEMS_IN_INVENTORY)
                inventory.InventoryQuantities[tileType]++;

            Board[yIndex, xIndex] = new Empty(Board[yIndex, xIndex].Position, true);

            BoardChanged = true;
        }

		private void ChooseTile(int xIndex, int yIndex)
		{
			var refractorTile = Board[yIndex, xIndex] as RefractorTile;

			if (refractorTile == null || LevelInfo.InactiveInventory.Contains(refractorTile.Type))
				return;

			var inventory = Parent.GetForm<InventoryForm>(LevelMultiform.InventoryFormName);
			inventory.CurrentlySelectedTile = refractorTile.Type;
		}

        private void RetraceLasers()
        {
            // Clear the previous lasers.
            Lasers.Clear();

            // For each outputter, send out a laser and trace it's path.
            foreach (var outputterPosition in LevelInfo.Outputters)
            {
                var tile = (Outputter)Board[outputterPosition.Y, outputterPosition.X];
                var laser = new Laser(tile.OutputDirection, tile.OutputColour, LocalFrame);
                laser.TracePath(this, outputterPosition);
                Lasers.Add(laser);
            }

            // For each receiver, check if it's been activated.
            var levelComplete = true;
            foreach (var receiverPosition in LevelInfo.Receivers)
            {
                var receiver = (LevelEndController)Board[receiverPosition.Y, receiverPosition.X];
                levelComplete &= receiver.Activated;

                // We have to reset the Activated property to false because we retrace the lasers
                // each time the board gets updated, which in turn will reactivate the Activated
                // property anyways if a laser reaches it's associated receiver.
                //
                // If we didn't do this, then a player could beat a level by simply activating all
                // receivers sequentially, instead of all at once.
                receiver.Activated = false;
            }
            LevelComplete = levelComplete;

            BoardChanged = false;
        }

        public override void Render()
        {
            base.Render();

            foreach (var tile in Board)
                tile.Render();
            
            DisplayManager.SetSpriteBatchProperties(
                sortMode: SpriteSortMode.Texture, blendState: BlendState.Additive);

            foreach (var laser in Lasers)
                laser.Render();

            DisplayManager.ClearSpriteBatchProperties();
        }

    }
}
