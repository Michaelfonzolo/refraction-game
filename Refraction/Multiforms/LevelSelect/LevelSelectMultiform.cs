using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Multiforms;

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class LevelSelectMultiform : Multiform
	{

		public override void Construct(MultiformTransmissionData args)
		{
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
