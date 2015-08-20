using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Multiforms;

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class LevelSelectMultiform : Multiform
	{

		/// <summary>
		/// The x offset of the first level select button from the top of the screen.
		/// </summary>
		public const int INITIAL_LEVEL_SELECT_X_OFFSET = 80;

		/// <summary>
		/// The y offset of the first level select button from the top of the screen.
		/// </summary>
		public const int INITIAL_LEVEL_SELECT_Y_OFFSET = 80;

		/// <summary>
		/// The width of a level select button.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_WIDTH = 50;

		/// <summary>
		/// The height of a level select button.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_HEIGHT = 50;

		/// <summary>
		/// The gap between horizontally adjacent level select buttons.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_GAP_X = 30;

		/// <summary>
		/// The gap between vertically adjacent level select buttons.
		/// </summary>
		public const int LEVEL_SELECT_BUTTON_GAP_Y = 30;

		/// <summary>
		/// The number of level select buttons per row.
		/// </summary>
		public const int BUTTONS_PER_ROW = 10;

		public List<SelectLevelButton> LevelSelectButtons = new List<SelectLevelButton>();

		public override void Construct(MultiformTransmissionData args)
		{
			int xOffset = INITIAL_LEVEL_SELECT_X_OFFSET;
			int yOffset = INITIAL_LEVEL_SELECT_Y_OFFSET;

			int rowNum = (int)(LoadedLevelManager.SequentialLevels.Length / BUTTONS_PER_ROW),
				totalHeight = rowNum * LEVEL_SELECT_BUTTON_HEIGHT + (rowNum - 1) * LEVEL_SELECT_BUTTON_GAP_X,
				scrollBarHeight = DisplayManager.WindowResolution.Height - 2 * INITIAL_LEVEL_SELECT_Y_OFFSET;

			// We only add a scrollbar if there is not enough room onscreen to fit everything. This occurs
			// when the total vertical space taken up by the level select buttons is greater than that which
			// would've been taken up by the scrollbar (scrollbar height = thumb height + shaft height).
			if (totalHeight > scrollBarHeight)
			{
				int scrollBarThumbHeight = scrollBarHeight * scrollBarHeight / totalHeight;
			}

			SetUpdater(Update_Main);
			SetRenderer(Render_Main);
		}

		public void Update_Main()
		{

		}

		public void Render_Main()
		{

		}

	}
}
