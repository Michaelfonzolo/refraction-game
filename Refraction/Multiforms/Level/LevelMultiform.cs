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
 * The main level multiform.
 */

#endregion

#region Using Statements

using DemeterEngine.Multiforms;
using Refraction_V2.Multiforms.LevelComplete;
using System;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class LevelMultiform : Multiform
    {

		/// <summary>
		/// The name of the multiform.
		/// </summary>
		public const string MultiformName = "Level";

		/// <summary>
		/// The name of the board form instance.
		/// </summary>
		public const string BoardFormName = "Board";

		/// <summary>
		/// The name of the inventory form instance.
		/// </summary>
		public const string InventoryFormName = "Inventory";

		/// <summary>
		/// The LevelNameInfo sent in from the previous multiform.
		/// </summary>
		public LevelNameInfo LevelNameInfo { get; private set; }

        public override void Construct(MultiformTransmissionData args)
        {
            if (args == null)
			{
				LevelNameInfo = new LevelNameInfo(0);
			}
            else
			{
				LevelNameInfo = args.GetAttr<LevelNameInfo>("LevelNameInfo");
			}

            var levelInfo = new LevelInfo(LevelNameInfo.LevelName);
            RegisterForm(BoardFormName, new BoardForm(levelInfo));
            RegisterForm(InventoryFormName, new InventoryForm(levelInfo));

            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
        }

        public void Update_Main()
        {
            base.UpdateTime();

            UpdateForms();

            var form = GetForm<BoardForm>(BoardFormName);
            if (form.LevelComplete)
			{
				Console.WriteLine("Yay!");

				if (LevelNameInfo.Sequential)
					LoadedLevelManager.CompletedLevels.Add(LevelNameInfo.LevelNumber.Value);

				var data = new MultiformTransmissionData(MultiformName);
				data.SetAttr<LevelNameInfo>("LevelNameInfo", LevelNameInfo);

				Manager.Close(this);
				Manager.Construct(LevelCompleteMultiform.MultiformName, data);

				ClearForms();
			}
        }

        public void Render_Main()
        {
            RenderForms(BoardFormName, InventoryFormName);
        }

    }
}
