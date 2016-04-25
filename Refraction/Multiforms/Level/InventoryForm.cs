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

        #region Look and Feel Constants

        public const float INVENTORY_BUTTON_X_GAP = 30;
        public const float INVENTORY_BUTTON_Y_GAP = 10;

        #endregion

        /// <summary>
		/// The currently selected tile type or null.
		/// </summary>
        public TileType? CurrentlySelectedTile { get; internal set; }

		/// <summary>
		/// A bool determining whether or not the player can place the currently
		/// selected tile (if it is not null).
		/// </summary>
        public bool CanPlaceCurrentTile
        {
            get
            {
				return CurrentlySelectedTile.HasValue
					&& InventoryButtons[CurrentlySelectedTile.Value].Quantity != 0;
            }
        }

		/// <summary>
		/// The dictionary of inventory button forms.
		/// </summary>
        public Dictionary<TileType, InventoryButtonForm> InventoryButtons
            = new Dictionary<TileType, InventoryButtonForm>();

		/// <summary>
		/// The font used to display item quantities.
		/// </summary>
		public SpriteFont ItemQuantityFont { get; private set; }

        /// <summary>
        /// A flag indicating whether or not the tile selection has changed.
        /// </summary>
        public bool SelectionChanged { get; private set; }

        public InventoryForm(LevelInfo info)
            : base(true)
        {
			ItemQuantityFont = Assets.Level.Fonts.InventoryItem;

            try
            {
                CurrentlySelectedTile = info.Inventory.First(kvp => kvp.Value != 0).Key;
            }
            catch (InvalidOperationException)
            {
                CurrentlySelectedTile = null;
            }

            var invButtonWidth  = Assets.Level.Images.InventoryButton.Width + INVENTORY_BUTTON_X_GAP;
            var invButtonHeight = Assets.Level.Images.InventoryButton.Height + INVENTORY_BUTTON_Y_GAP;

            var inv_h = invButtonHeight * LevelInfo.InventoryTileOrder[0].Length;
			var y = (DisplayManager.WindowResolution.Height - inv_h - invButtonHeight) / 2; 

            var inv_w = invButtonWidth * LevelInfo.InventoryTileOrder.Length;
            var x = (DisplayManager.WindowResolution.Width
                     + Assets.Level.Images.EmptyTile.Width * info.BoardDimensions.X
                     + LevelInfo.BOARD_INVENTORY_GAP) / 2f
                  - invButtonWidth + INVENTORY_BUTTON_X_GAP;

            int i = 0, j = 0;
            foreach (var column in LevelInfo.InventoryTileOrder)
            {
                foreach (var tileType in column)
                {
					if (info.InactiveInventory.Contains(tileType))
					{
						continue;
					}
                    var position = new Vector2(
                        x + i * invButtonWidth, 
                        y + j * invButtonHeight);
                    var button = new InventoryButtonForm(tileType, position, info.Inventory[tileType]);
                    InventoryButtons.Add(tileType, button);
                    j++;
                }
                i++;
                j = 0;
            }
        }

        public void IncrementTileCount(TileType type)
        {
            InventoryButtons[type].IncrementQuantity();
        }

        public void DecrementTileCount(TileType type)
        {
            InventoryButtons[type].DecrementQuantity();
        }

		/// <summary>
		/// Decrement the quantity of the current tile.
		/// </summary>
        public void DecrementCurrentTileCount()
        {
            DecrementTileCount(CurrentlySelectedTile.Value);
        }

        public override void Update()
        {
            base.Update();

            SelectionChanged = false;

            TileType type;
            InventoryButtonForm button;
            foreach (var kvp in InventoryButtons)
            {
                type = kvp.Key;
                button = kvp.Value;

                button.Update();
                if (button.IsReleased(MouseButtons.Left))
                {
                    SelectionChanged = type != CurrentlySelectedTile;
                    CurrentlySelectedTile = type;
                }
            }
        }

        public override void Render()
        {
            foreach (var button in InventoryButtons.Values)
			{
				button.Render();
			}
        }
    }
}
