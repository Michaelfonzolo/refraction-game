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
 * The LevelInfo class is responsible for representing level metadata. These
 * properties include the board dimensions, the initial tile arrangement, and
 * the player's inventory. The class is also responsible for loading levels from
 * the .levelfile xml format.
 */

#endregion

#region Using Statements

using DemeterEngine.Extensions;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#endregion

namespace Refraction_V2.Multiforms.Level
{

    public class LevelInfo
    {

        /// <summary>
        /// The length of the gap between the board and the inventory.
        /// </summary>
        public const int BOARD_INVENTORY_GAP = 30;

        /// <summary>
        /// The magic number indicating when the player has an infinite number
        /// of a certain tile in their inventory.
        /// </summary>
        public const int INFINITE_ITEMS_IN_INVENTORY = -1;

        public string LevelName { get; private set; }

        /// <summary>
        /// The dimensions of the board. This includes everything that is considered a "tile",
        /// be it open or closed.
        /// </summary>
        public Point BoardDimensions { get; private set; }

        /// <summary>
        /// The board itself. The information stored here is simply information regarding the
        /// tiles at each position in the board. They are converted to LevelTile objects in
        /// BoardForm which have an actual RectCollider and a Sprite.
        /// </summary>
        public TileInfo[,] Board { get; private set; }

        /// <summary>
        /// The positions of the laser outputters in the level.
        /// </summary>
        public Point[] Outputters { get; private set; }

        /// <summary>
        /// The positions of the laser receivers in the level.
        /// </summary>
        public Point[] Receivers { get; private set; }

        /// <summary>
        /// The amount of each tile the player starts out with. Note that -1 indicates
        /// an infinite number of tiles.
        /// </summary>
        public Dictionary<TileType, int> Inventory = new Dictionary<TileType, int>()
        {
            {TileType.RF_UxL_UL, 0},
            {TileType.RF_UxR_UR, 0},
            {TileType.RF_DxR_DR, 0},
            {TileType.RF_DxL_DL, 0},
            {TileType.RF_ULxUR_U, 0},
            {TileType.RF_ULxDL_L, 0},
            {TileType.RF_URxDR_R, 0},
            {TileType.RF_DLxDR_D, 0},
            {TileType.RF_U_L_and_U_R_pass_UL, 0},
            {TileType.RF_U_L_and_U_R_pass_UR, 0},
            {TileType.RF_UL_DL_and_UR_DR_pass_L, 0},
            {TileType.RF_UL_UR_and_DL_DR_pass_U, 0}
        };

        /// <summary>
        /// The order in which tiles in the player's inventory are drawn. Each row represents
        /// a column, as the inventory is a 2xN array of tiles.
        /// </summary>
        public static TileType[][] InventoryTileOrder = new[] 
        {
            new[] {TileType.RF_UxL_UL, TileType.RF_UxR_UR, TileType.RF_DxR_DR, TileType.RF_DxL_DL},
            new[] {TileType.RF_ULxUR_U, TileType.RF_ULxDL_L, TileType.RF_URxDR_R, TileType.RF_DLxDR_D},
            new[] {TileType.RF_U_L_and_U_R_pass_UL, TileType.RF_U_L_and_U_R_pass_UR, TileType.RF_UL_DL_and_UR_DR_pass_L, TileType.RF_UL_UR_and_DL_DR_pass_U}
        };

		public List<TileType> InactiveInventory = new List<TileType>();

        /// <summary>
        /// The number of the tutorial to load.
        /// </summary>
        public int? TutorialNumber { get; private set; }

        /// <summary>
        /// A list of all warning messages regarding non-fatal issues that occurred whilst
        /// loading the level file.
        /// </summary>
        public HashSet<string> WarningMessages = new HashSet<string>();

        /// <summary>
        /// The exception thrown when attempting to load the level, if level loading failed.
        /// </summary>
        public LevelLoadException Exception { get; private set; }

        public LevelInfo(string levelName, bool mock = false)
        {
            LevelName = levelName;
            try 
            {
                if (mock)
                {
                    LoadMock();
                }
                else
                {
                    LoadLevel();
                }
                Exception = null;
            }
            catch (LevelLoadException exception)
            {
                Exception = exception;
            }
        }

		/// <summary>
		/// Load a mock level.
		/// </summary>
        private void LoadMock()
        {
            BoardDimensions = new Point(4, 7);
            Board = new TileInfo[7, 4];

            Board[0, 0] = new TileInfo(TileType.Outputter, false);
            Board[0, 0].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
			Board[0, 0].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Red);

            Board[0, 1] = new TileInfo(TileType.Outputter, false);
			Board[0, 1].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
			Board[0, 1].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Blue);

            Board[0, 2] = new TileInfo(TileType.Outputter, false);
			Board[0, 2].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[0, 2].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Green);

            Board[0, 3] = new TileInfo(TileType.Outputter, false);
			Board[0, 3].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[0, 3].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Purple);

            Board[1, 0] = new TileInfo(TileType.Empty, true);
            Board[1, 1] = new TileInfo(TileType.Empty, true);
            Board[1, 2] = new TileInfo(TileType.Empty, true);
            Board[1, 3] = new TileInfo(TileType.Empty, true);

            Board[2, 0] = new TileInfo(TileType.Empty, true);
            Board[2, 1] = new TileInfo(TileType.Empty, true);
            Board[2, 2] = new TileInfo(TileType.Empty, true);
            Board[2, 3] = new TileInfo(TileType.Empty, true);

            Board[3, 0] = new TileInfo(TileType.Empty, true);
            Board[3, 1] = new TileInfo(TileType.Empty, true);
            Board[3, 2] = new TileInfo(TileType.Empty, true);
            Board[3, 3] = new TileInfo(TileType.Empty, true);

            Board[4, 0] = new TileInfo(TileType.RF_ULxUR_U, false);
            Board[4, 1] = new TileInfo(TileType.RF_ULxUR_U, false);
            Board[4, 2] = new TileInfo(TileType.RF_ULxUR_U, false);
            Board[4, 3] = new TileInfo(TileType.RF_ULxUR_U, false);

            Board[5, 0] = new TileInfo(TileType.Empty, true);
            Board[5, 1] = new TileInfo(TileType.Empty, true);
            Board[5, 2] = new TileInfo(TileType.Empty, true);
            Board[5, 3] = new TileInfo(TileType.Empty, true);

            Board[6, 0] = new TileInfo(TileType.Receiver, false);
            Board[6, 0].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[6, 0].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Purple);

            Board[6, 1] = new TileInfo(TileType.Receiver, false);
            Board[6, 1].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[6, 1].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Red);

            Board[6, 2] = new TileInfo(TileType.Receiver, false);
            Board[6, 2].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[6, 2].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Green);

            Board[6, 3] = new TileInfo(TileType.Receiver, false);
            Board[6, 3].SetAttr<Directions>(DIRECTION_ATTRIBUTE, Directions.Up);
            Board[6, 3].SetAttr<LaserColours>(COLOUR_ATTRIBUTE, LaserColours.Blue);

            Outputters = new Point[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0) };
            Receivers  = new Point[] { new Point(0, 6), new Point(1, 6), new Point(2, 6), new Point(3, 6) };

            foreach (var row in InventoryTileOrder) 
				foreach (var tileType in row)
					Inventory[tileType] = -1;
        }

		#region Parsing Errors

		private void Error_UnexpectedRootNodes()
		{
			throw new LevelLoadException("Expected an XmlDeclaration and exactly one root element.");
		}

        private void Error_NoRootChildren()
        {
            throw new LevelLoadException("Root node has no children. Requires at least one child.");
        }

		private void Error_UnexpectedRootNodeName(string name)
		{
			throw new LevelLoadException(
				String.Format("Expected root element to have name \"Level\"; got \"{0}\" instead.", name));
		}

		private void Error_UnexpectedRootChildrenCount()
		{
			throw new LevelLoadException("Expected \"Level\" element to contain 2 or 3 children.");
		}

		private void Error_UnexpectedRootChildren()
		{
			throw new LevelLoadException(
				"Expected one \"Board\" element and one \"Inventory\" element in \"Level\" root.");
		}

		private void Error_InconsistentBoardRowWidths()
		{
			throw new LevelLoadException("Inconsistent board row widths.");
		}

		private void Error_IndeterminateBoardWidth()
		{
			throw new LevelLoadException("Could not determine board width.");
		}

		private void Error_InvalidBoardTileElement(string name)
		{
			throw new LevelLoadException(String.Format("Invalid tile element {0} encountered.", name));
		}

		private void Error_NoTypeAttribute()
		{
			throw new LevelLoadException("All tile elements expected to have a \"Type\" attribute.");
		}

		private void Error_NoColourOrDirectionAttribute(string name)
		{
			throw new LevelLoadException(
				String.Format(
					"All {0} elements expected to have a \"Colour\" " +
					"attribute and a \"Direction\" attribute.", name
					)
				);
		}

		private void Error_InvalidAttributeValue(string attribute, string value)
		{
			throw new LevelLoadException(String.Format("Invalid {0} {1}.", attribute, value));
		}

		private void Error_UnexpectedInventoryElement()
		{
			throw new LevelLoadException(
				"Expected only \"Tile\" and \"InactivateRemaining\" elements in \"Inventory\".");
		}

		private void Error_NoCountAttribute()
		{
			throw new LevelLoadException(
				"All \"Tile\" elements in \"Inventory\" are expected to have a \"Count\" attribute.");
		}

        private void Error_NoBoardElement()
        {
            throw new LevelLoadException("No \"Board\" element was supplied.");
        }

		private const string WARNING_COULDNT_PARSE_OPEN = "Couldn't parse \"Open\" attribute's value \"{0}\".";

		private const string WARNING_COULDNT_PARSE_COUNT = "Couldn't parse \"Count\" attribute's value \"{0}\".";

        private const string WARNING_COULDNT_PARSE_TUTORIAL = "Couldn't parse \"LoadTutorial\"'s text \"{0}\" as an integer.";

        private const string WARNING_EMPTY_INVENTORY = "No inventory was supplied, so it will be empty.";

		private void Warn_CouldntParseOpenAttribute(string received)
		{
			WarningMessages.Add(String.Format(WARNING_COULDNT_PARSE_OPEN, received));
		}

		private void Warn_CouldntParseCountAttribute(string received)
		{
			WarningMessages.Add(String.Format(WARNING_COULDNT_PARSE_COUNT, received));
		}

        private void Warn_CouldntParseTutorialNumber(string received)
        {
            WarningMessages.Add(String.Format(WARNING_COULDNT_PARSE_TUTORIAL, received));
        }

        private void Warn_EmptyInventory()
        {
            WarningMessages.Add(WARNING_EMPTY_INVENTORY);
        }

		#endregion

		#region XmlElement

		public const string ROOT_ELEMENT                 = "Level";
		public const string BOARD_ELEMENT                = "Board";
		public const string INVENTORY_ELEMENT            = "Inventory";
		public const string ROW_ELEMENT                  = "Row";
		public const string EMPTY_ROW_ELEMENT            = "EmptyRow";
		public const string EMPTY_ELEMENT                = "Empty";
		public const string OUTPUTTER_ELEMENT            = "Outputter";
		public const string RECEIVER_ELEMENT             = "Receiver";
		public const string TILE_ELEMENT                 = "Tile";
		public const string DEACTIVATE_REMAINING_ELEMENT = "DeactivateRemaining";
        public const string LOAD_TUTORIAL_ELEMENT        = "LoadTutorial";

        #endregion

        #region XmlAttribute Names

        public const string OPEN_ATTRIBUTE      = "Open";
		public const string TYPE_ATTRIBUTE      = "Type";
		public const string DIRECTION_ATTRIBUTE = "Direction";
		public const string COLOUR_ATTRIBUTE    = "Colour";
		public const string COUNT_ATTRIBUTE     = "Count";

        #endregion

        #region Xml Document Loading Constants

        public const string INFINITE_COUNT_ATTRIBUTE_VAL = "infinite";

		public const int INDETERMINATE_ROW_WIDTH = -1;

		#endregion

		/// <summary>
		/// Load the level info from a .levelfile xml document.
		/// </summary>
		private void LoadLevel()
        {
			var doc = new XmlDocument();
			doc.Load(LevelName);

            var roots = doc.ChildNodes;
			if (roots.Count != 2)
				Error_UnexpectedRootNodes();

			// We grab roots[1] in order to skip the XmlDeclaration.
            var root = roots[1] as XmlElement;
			if (root.Name != ROOT_ELEMENT)
				Error_UnexpectedRootNodeName(root.Name);

			// There should only be a Board element and an Inventory element.
            // There can also be an additional LoadTutorial node.
			if (root.ChildNodes.Count != 2 && root.ChildNodes.Count != 3)
				Error_UnexpectedRootChildrenCount();

            bool boardLoaded = false, inventoryLoaded = false, tutorialAdded = false;

            foreach (var child in root.ChildElements())
            {
				if (child.Name == BOARD_ELEMENT && !boardLoaded)
				{
					LoadBoard(child);
					boardLoaded = true;
				}
				else if (child.Name == INVENTORY_ELEMENT && !inventoryLoaded)
				{
					LoadInventory(child);
					inventoryLoaded = true;
				}
                else if (child.Name == LOAD_TUTORIAL_ELEMENT && !tutorialAdded)
                {
                    LoadTutorial(child);
                    tutorialAdded = true;
                }
				else
					Error_UnexpectedRootChildren();
            }

            if (!boardLoaded)
            {
                Error_NoBoardElement();
            }
            if (!inventoryLoaded)
            {
                Warn_EmptyInventory();
            }
        }

		/// <summary>
		/// This struct represents an index that describes where and how to
		/// insert a row of empty tiles into the board. The index indicates
		/// where in the board it is inserted and the open property determines
		/// whether all the tiles are open or closed.
		/// 
		/// The reason this exists is due to how the Board element is parsed.
		/// It is parsed sequentially, and each element can either be a parent
		/// element with as many children elements as there are tiles in a row,
		/// or it could be a terminal element like "<EmptyRow/>"
		/// 
		/// If we encounter a row element that has children, then the number of
		/// children in the row is the board's width. Otherwise, since we don't
		/// know the width of the board, when we encounter a terminal row element
		/// we have to store a record of encountering it and then insert it when
		/// we've gone through everything.
		/// 
		/// If by the end we can't determine the width of the board, an error is
		/// thrown.
		/// </summary>
        private struct EmptyRowIndexStruct
        {
            public int Index { get; set; }
            public bool Open { get; set; }
        }

        private void LoadBoard(XmlElement boardElement)
        {
			int dimX = INDETERMINATE_ROW_WIDTH, dimY = boardElement.ChildNodes.Count;

            var rows = new List<List<TileInfo>>();
            var currentRow = new List<TileInfo>();

            var emptyRowIndices = new List<EmptyRowIndexStruct>();

            foreach (var child in boardElement.ChildElements())
            {
                if (child.Name == ROW_ELEMENT)
                {
                    var len = child.ChildNodes.Count;
					if (dimX == INDETERMINATE_ROW_WIDTH)
						dimX = len;
					else if (dimX != len)
						Error_InconsistentBoardRowWidths();
                    LoadRow(child, currentRow);
                }
                else if (child.Name == EMPTY_ROW_ELEMENT)
                {
                    bool rowOpen = AttemptReadOpenAttribute(child);
					// If we haven't yet determined the width of the board, we
					// have to insert it afterwards.
                    if (dimX == INDETERMINATE_ROW_WIDTH)
                    {
						emptyRowIndices.Add(
							new EmptyRowIndexStruct
							{
								Index = rows.Count - 1,
								Open = rowOpen
							});
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < dimX; i++)
                            currentRow.Add(new TileInfo(TileType.Empty, rowOpen));
                    }
                }
                rows.Add(currentRow);
                currentRow = new List<TileInfo>();
            }

			// If we could not determine the width of the board. This occurs when
			// the board is entirely composed of terminal row elements (like "<EmptyRow/>").
			if (dimX == INDETERMINATE_ROW_WIDTH)
				Error_IndeterminateBoardWidth();

			// Reinsert the empty row indices.
            foreach (var index in emptyRowIndices)
            {
                var newRow = new List<TileInfo>();
                for (int i = 0; i < dimX; i++)
                    newRow.Add(new TileInfo(TileType.Empty, index.Open));
                rows.Insert(index.Index, newRow);
            }

			BoardDimensions = new Point(dimX, dimY);
            Board = new TileInfo[dimY, dimX];
			var outputters = new List<Point>();
			var receivers = new List<Point>();

            int j = 0;
            foreach (var row in rows)
            {
                foreach (var info in row)
                {
                    Board[j / dimX, j % dimX] = info;

					if (info.Type == TileType.Outputter)
						outputters.Add(new Point(j % dimX, j / dimX));

					else if (info.Type == TileType.Receiver)
						receivers.Add(new Point(j % dimX, j / dimX));

                    j++;
                }
            }

			Outputters = outputters.ToArray();
			Receivers = receivers.ToArray();
        }

		/// <summary>
		/// Load a "<Row>" element. A row element is composed of X number of
		/// child tile elements, where X is the width of the board. The tile
		/// elements can be "<Empty>", "<Tile>", "<Outputter>", or "<Receiver>"
		/// elements.
		/// </summary>
		/// <param name="rowElement"></param>
		/// <param name="currentRow"></param>
        private void LoadRow(XmlElement rowElement, List<TileInfo> currentRow)
        {
            foreach (var child in rowElement.ChildElements())
            {
                switch (child.Name)
                {
                    case EMPTY_ELEMENT:
                        currentRow.Add(ReadEmptyTileElement(child));
                        break;
                    case TILE_ELEMENT:
                        currentRow.Add(ReadTileElement(child));
                        break;
                    default:
						Error_InvalidBoardTileElement(child.Name);
						break;
                }
            }
        }

		/// <summary>
		/// Attempt to read the "open" attribute from a tile element. Many
		/// tile elements can have an open attribute which determines whether
		/// the tile is open or closed on the board. By default every tile
		/// with an optional open attribute is open.
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
        private bool AttemptReadOpenAttribute(XmlElement tile)
        {
            bool open = true;
            if (tile.HasAttribute(OPEN_ATTRIBUTE))
            {
                try
                {
                    open = Convert.ToBoolean(tile.GetAttribute(OPEN_ATTRIBUTE));
                }
                catch (FormatException)
                {
					// Add a warning to display after loading the level.
					Warn_CouldntParseOpenAttribute(tile.GetAttribute(OPEN_ATTRIBUTE));
                }
            }
            return open;
        }

		/// <summary>
		/// Read an empty tile element and return a TileInfo object representing it.
		/// </summary>
        private TileInfo ReadEmptyTileElement(XmlElement tile)
        {
            return new TileInfo(TileType.Empty, AttemptReadOpenAttribute(tile));
        }

		/// <summary>
		/// Read a tile element. In this case, a tile element is any of "<Tile>",
		/// "<Outputter>", or "<Receiver>", where the string "type" parameter
		/// determines which of the three it is.
		/// </summary>
        private TileInfo ReadTileElement(XmlElement tile)
        {
            var tileName = tile.GetAttribute(TYPE_ATTRIBUTE);

            TileType tileType;
            var success = Enum.TryParse<TileType>(tileName, out tileType);
            if (!success)
            {
                Error_InvalidAttributeValue("tile type", tile.GetAttribute(TYPE_ATTRIBUTE));
            }

            // All non-empty tiles initially specified in the level file are closed to
            // prevent the player overwriting them.
            var tileInfo = new TileInfo(tileType, false);

            foreach (var attr in tile.Attributes.Cast<XmlAttribute>())
            {
                switch(attr.Name)
                {
                    case DIRECTION_ATTRIBUTE:
                        if (!Directions.DirectionsByName.ContainsKey(attr.InnerText))
                            Error_InvalidAttributeValue("direction", attr.InnerText);

                        var direction = Directions.DirectionsByName[attr.InnerText];
                        tileInfo.SetAttr<Directions>(attr.Name, direction);
                        break;
                    case COLOUR_ATTRIBUTE:
                        if (!LaserColours.ColoursByName.ContainsKey(attr.InnerText))
					        Error_InvalidAttributeValue("colour", attr.InnerText);

				        var colour = LaserColours.ColoursByName[attr.InnerText];
                        tileInfo.SetAttr<LaserColours>(attr.Name, colour);
                        break;
                    default:
                        break;
                }
            }

            // Check to ensure a tile of a specific type has the required
            // attributes.
            switch (tileType)
            {
                case TileType.Outputter:
                case TileType.Receiver:
                    if (!tileInfo.HasAttr<LaserColours>(COLOUR_ATTRIBUTE) ||
                        !tileInfo.HasAttr<Directions>(DIRECTION_ATTRIBUTE))
                        Error_NoColourOrDirectionAttribute(tileName.ToLower());
                    break;
                case TileType.OpenReceiver:
                case TileType.OpenReceiver_pass_L:
                case TileType.OpenReceiver_pass_U:
                case TileType.OpenReceiver_pass_UL:
                case TileType.OpenReceiver_pass_UR:
                    if (!tileInfo.HasAttr<LaserColours>(COLOUR_ATTRIBUTE))
                        Error_NoColourOrDirectionAttribute(tileName.ToLower());
                    break;
                default:
                    break;
            }

            return tileInfo;
        }

		/// <summary>
		/// Load the player's inventory.
		/// </summary>
		/// <param name="inventoryElement"></param>
        private void LoadInventory(XmlElement inventoryElement)
        {
			// We keep a list of seen tile types so that if the user chooses to include an
			// <InactivateRemaining/> element at the end of the inventory element, we can
			// decide which tiles we haven't seen and add them to the list of inactive tile
			// types.
			var seenTiles = new List<TileType>();
			bool inactivateRemaining = false;

			foreach (var child in inventoryElement.ChildNodes.Cast<XmlElement>())
			{
				if (child.Name == TILE_ELEMENT)
				{
					if (!child.HasAttribute(TYPE_ATTRIBUTE))
						Error_NoTypeAttribute();

					TileType tileType;
					bool validTileType = Enum.TryParse<TileType>(child.GetAttribute(TYPE_ATTRIBUTE), out tileType);
					if (!validTileType)
						Error_InvalidAttributeValue("tile type", child.GetAttribute(TYPE_ATTRIBUTE));

					if (!child.HasAttribute(COUNT_ATTRIBUTE))
						Error_NoCountAttribute();

					int count = 0;
					var countString = child.GetAttribute(COUNT_ATTRIBUTE);
					if (countString.ToLower() == INFINITE_COUNT_ATTRIBUTE_VAL)
						count = INFINITE_ITEMS_IN_INVENTORY;
					else
					{
						try
						{
							count = Convert.ToInt32(child.GetAttribute(COUNT_ATTRIBUTE));
						}
						catch (FormatException)
						{
							Warn_CouldntParseCountAttribute(child.GetAttribute(COUNT_ATTRIBUTE));
						}
					}
					Inventory[tileType] = count;
					seenTiles.Add(tileType);
				}
				else if (child.Name == DEACTIVATE_REMAINING_ELEMENT)
				{
					inactivateRemaining = true;
					break;
				}
				else
					Error_UnexpectedInventoryElement();
			}

			if (!inactivateRemaining)
				return;

			foreach (var row in InventoryTileOrder)
			{
				foreach (var tile in row)
				{
					if (seenTiles.Contains(tile))
						continue;
					InactiveInventory.Add(tile);
				}
			}
        }

        private void LoadTutorial(XmlElement element)
        {
            try
            {
                TutorialNumber = Convert.ToInt32(element.InnerText);
            }
            catch (FormatException)
            {
                TutorialNumber = null;
                Warn_CouldntParseTutorialNumber(element.InnerText);
            }
        }

    }
}
