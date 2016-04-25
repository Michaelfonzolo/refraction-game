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

using Refraction_V2.Multiforms.Level.Tiles;

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class BoardForm : Form
    {

        private static readonly int TILE_SIDE_LENGTH = Assets.Level.Images.EmptyTile.Width;

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
        public bool BoardChanged { get; private set; }

        /// <summary>
        /// A flag that indicates when a tile has been added to the board.
        /// </summary>
        public bool TileAdded { get; private set; }

        /// <summary>
        /// A flag that indicates when a tile has been removed from the board.
        /// </summary>
        public bool TileRemoved { get; private set; }

        public BoardForm(LevelInfo levelInfo)
            : base(true)
        {
            LevelInfo = levelInfo;
            Dimensions = levelInfo.BoardDimensions;

            // Find the x and y offsets of the topleft corner of the board.
            var boardY = GetBoardY(Dimensions);
            var boardX = GetBoardX(Dimensions);

            BoardCollider = new RectCollider(
                boardX, boardY, 
                TILE_SIDE_LENGTH * Dimensions.X, 
                TILE_SIDE_LENGTH * Dimensions.Y
                );
            Board = new LevelTile[Dimensions.Y, Dimensions.X];

            // Create all the LevelTiles.
            int i = 0;
            foreach (var tileInfo in levelInfo.Board)
            {
                AddTile(boardX, boardY, i, Dimensions, tileInfo);
                i++;
            }

            Lasers = new List<Laser>();

            BoardChanged = true;
        }

        public bool IsReceiverActivated(LaserColours colour)
        {
            foreach (var pos in LevelInfo.Receivers)
            {
                var tile = (LevelEndController)Board[pos.Y, pos.X];
                if (tile.InputColour == colour && tile.Activated)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the x coordinate of the top left corner of the board.
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private float GetBoardX(Point dim)
        {
            var windowWidth = DisplayManager.WindowWidth;
            var tileWidth = TILE_SIDE_LENGTH;
            var invButtonWidth = Assets.Level.Images.InventoryButton.Width;
            var gap = LevelInfo.BOARD_INVENTORY_GAP;

            var X_times_2 = windowWidth - (tileWidth * dim.X + 2 * invButtonWidth + gap);
            return X_times_2 / 2f;
        }

        /// <summary>
        /// Get the y coordinate of the top left corner of the board.
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        private float GetBoardY(Point dim)
        {
            return (DisplayManager.WindowHeight - TILE_SIDE_LENGTH * dim.Y) / 2f;
        }

        /// <summary>
        /// Add a tile to the board.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        /// <param name="dim"></param>
        /// <param name="tileInfo"></param>
        private void AddTile(float x, float y, int index, Point dim, TileInfo tileInfo)
        {
            var xIndex = index % dim.X;
            var yIndex = index / dim.X;

            var tile = tileInfo.ToLevelTile(GetTilePosition(x, y, xIndex, yIndex, dim));
            Board[yIndex, xIndex] = tile;
        }

        /// <summary>
        /// Get the top left corner of a tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
        private Vector2 GetTilePosition(float x, float y, float xIndex, float yIndex, Point dim)
        {
            var vx = x + xIndex * TILE_SIDE_LENGTH;

            // We do (dim.Y - yIndex - 1) instead of just yIndex so that the board gets flipped
            // on the y-axis.
            var vy = y + (dim.Y - yIndex - 1) * TILE_SIDE_LENGTH;
            return new Vector2(vx, vy);
        }

        private Point GetTileCollidingWithMouse(Vector2 mpos)
        {
            var xIndex = (int)Math.Floor((mpos.X - BoardCollider.X) / TILE_SIDE_LENGTH);
            var yIndex = (int)Math.Floor((mpos.Y - BoardCollider.Y) / TILE_SIDE_LENGTH);

            // We again have to flip the yIndex vertically, since the window coordinate space
            // (the coordinate space the mouse position corresponds to) is not the same as the
            // board coordinate space.
            return new Point(xIndex, Dimensions.Y - yIndex - 1);
        }

        public override void Update()
        {
            base.Update();

            TileAdded = false;
            TileRemoved = false;

            foreach (var tile in Board)
                tile.Update();

            // Figure out which tile the mouse is colliding with, and then update the board's
            // properties accordingly.

            var mpos = MouseInput.MouseVector;

            // Get the indices of the tile the mouse is colliding with.
            var index = GetTileCollidingWithMouse(mpos);
            int xIndex = index.X;
            var yIndex = index.Y;

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
                        PerformBoardAction(button, xIndex, yIndex);
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
                iy = i / Dimensions.X;

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
        /// Perform the appropriate board action (setting a tile, removing a tile, or
        /// choosing a tile) at the given tile coordinates depending on the mouse button released.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="xIndex"></param>
        /// <param name="yIndex"></param>
        private void PerformBoardAction(MouseButtons button, int xIndex, int yIndex)
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
                inventory.IncrementTileCount(refractor);
			}

            // We have to reset the alpha value of the tile's hover sprite since otherwise
            // the tile's alpha value would reset and it would look weird.
            var prevAlpha = prevTile.HoverSpriteAlpha;
            var currentlySelected = inventory.CurrentlySelectedTile.Value;
            Board[yIndex, xIndex] = new TileInfo(
                currentlySelected, 
                true)
                .ToLevelTile(Board[yIndex, xIndex].Position);
            Board[yIndex, xIndex].HoverSpriteAlpha = prevAlpha;

            inventory.DecrementCurrentTileCount();

            BoardChanged = true;
            TileAdded = true;
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

            inventory.IncrementTileCount(tileType);

            var prevAlpha = refractorTile.HoverSpriteAlpha;
            Board[yIndex, xIndex] = new Empty(Board[yIndex, xIndex].Position, true);
            Board[yIndex, xIndex].HoverSpriteAlpha = prevAlpha;

            BoardChanged = true;
            TileRemoved = true;
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
            foreach (var receiverPosition in LevelInfo.Receivers)
            {
                var receiver = (LevelEndController)Board[receiverPosition.Y, receiverPosition.X];

                // We have to reset the Activated property to false because we retrace the lasers
                // each time the board gets updated, which in turn will reactivate the Activated
                // property anyways if a laser reaches it's associated receiver.
                //
                // If we didn't do this, then a player could beat a level by simply activating all
                // receivers sequentially, instead of all at once.
                receiver.Activated = false;
            }

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
            }
            LevelComplete = levelComplete;

            BoardChanged = false;
        }

        public override void Render()
        {
            base.Render();

            foreach (var tile in Board)
                tile.Render();

            foreach (var laser in Lasers)
                laser.Render();
        }

    }
}
