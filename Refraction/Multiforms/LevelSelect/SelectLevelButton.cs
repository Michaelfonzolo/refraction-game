using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class SelectLevelButton : ButtonForm
	{

		public const int BUTTON_WIDTH = 50;

		public const int BUTTON_HEIGHT = 50;

		public const string SpriteName = "LevelSelect_LevelButton";

		public const string FontName = "LevelSelect_LevelNoFont";

		public Sprite ButtonSprite { get; private set; }

		public SpriteFont LevelNoFont { get; private set; }

		public int LevelNo { get; private set; }

		public Vector2 TextPosition { get; private set; }

		public SelectLevelButton(RectCollider collider, int levelNo)
			: base(collider)
		{
			LevelNo = levelNo;

			ButtonSprite = ArtManager.Sprite(SpriteName);
			LevelNoFont  = ArtManager.Font(FontName);

			ButtonSprite.Position = collider.TopLeft;
			var textDimensions = LevelNoFont.MeasureString(LevelNo.ToString());
			TextPosition = (new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT) - textDimensions) / 2f + collider.TopLeft;
		}

		public override void Render()
		{
			ButtonSprite.Render();
			DisplayManager.DrawString(LevelNoFont, LevelNo.ToString(), TextPosition, Color.White);
		}
	}
}
