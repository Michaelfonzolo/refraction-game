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
 * The form which represents the player's inventory. It records the quantities of each
 * refractor type the player has left to use, and stores all the buttons the player has
 * access to in a level.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class InventoryForm : Form
    {

		/// <summary>
		/// The currently selected tile type or null.
		/// </summary>
        public TileType? CurrentlySelectedTile { get; internal set; }

		/// <summary>
		/// The dictionary mapping tile types to their associated quantity left in
		/// the player's inventory.
		/// </summary>
        public Dictionary<TileType, int> InventoryQuantities { get; private set; }

		/// <summary>
		/// A bool determining whether or not the player can place the currently
		/// selected tile (if it is not null).
		/// </summary>
        public bool CanPlaceCurrentTile
        {
            get
            {
				return CurrentlySelectedTile.HasValue
					|| InventoryQuantities[CurrentlySelectedTile.Value] != 0;
            }
        }

		/// <summary>
		/// The list of inventory button forms.
		/// </summary>
        public List<InventoryButtonForm> InventoryButtons = new List<InventoryButtonForm>();

		/// <summary>
		/// The font used to display item quantities.
		/// </summary>
		public SpriteFont ItemQuantityFont { get; private set; }

        public InventoryForm(LevelInfo info)
            : base(true)
        {
			ItemQuantityFont = ArtManager.Font("InventoryItemFont");

            try
            {
                CurrentlySelectedTile = info.Inventory.First(kvp => kvp.Value != 0).Key;
            }
            catch (InvalidOperationException)
            {
                CurrentlySelectedTile = null;
            }
            InventoryQuantities = info.Inventory;

            var inv_h = LevelInfo.INVENTORY_BUTTON_HEIGHT * LevelInfo.InventoryTileOrder[0].Length;
			var y = (DisplayManager.WindowResolution.Height - inv_h - LevelInfo.INVENTORY_BUTTON_HEIGHT) / 2; 

            var inv_w = LevelInfo.INVENTORY_BUTTON_WIDTH * LevelInfo.InventoryTileOrder.Length;
            var x = (DisplayManager.WindowResolution.Width
                     + LevelInfo.TILE_SIDE_LENGTH * info.BoardDimensions.X
                     + LevelInfo.BOARD_INVENTORY_GAP) / 2f
                  - LevelInfo.INVENTORY_BUTTON_WIDTH;

            int i = 0, j = 0;
            foreach (var column in LevelInfo.InventoryTileOrder)
            {
                foreach (var tileType in column)
                {
					if (info.InactiveInventory.Contains(tileType))
					{
						// j++;
						continue;
					}
                    var collider = new RectCollider(
                        x + i * LevelInfo.INVENTORY_BUTTON_WIDTH, y + j * LevelInfo.INVENTORY_BUTTON_HEIGHT,
                        LevelInfo.INVENTORY_BUTTON_WIDTH, LevelInfo.INVENTORY_BUTTON_HEIGHT);
                    var button = new InventoryButtonForm(tileType, collider);
                    InventoryButtons.Add(button);
                    j++;
                }
                i++;
                j = 0;
            }
        }

		/// <summary>
		/// Decrement the quantity of the current tile.
		/// </summary>
        public void DecrementCurrentTileCount()
        {
            if (InventoryQuantities[CurrentlySelectedTile.Value] <= 0)
                return;
            InventoryQuantities[CurrentlySelectedTile.Value]--;
        }

        public override void Update()
        {
            base.Update();

            foreach (var button in InventoryButtons)
            {
                button.Update();
                if (button.IsReleased(MouseButtons.Left))
                    CurrentlySelectedTile = button.Type;
            }
        }

        public override void Render()
        {
            foreach (var button in InventoryButtons)
			{
				button.Render();

				var position = (button.Collider as RectCollider).TopLeft + new Vector2(
					LevelInfo.INVENTORY_BUTTON_WIDTH / 2 + 20, 
					LevelInfo.INVENTORY_BUTTON_HEIGHT / 2 - 8);
				var count = InventoryQuantities[button.Type];

				string text = count == LevelInfo.INFINITE_ITEMS_IN_INVENTORY
							  ? "Inf" : String.Format("x{0}", count);
				DisplayManager.DrawString(ItemQuantityFont, text, position, Color.White);
			}
        }
    }
}
