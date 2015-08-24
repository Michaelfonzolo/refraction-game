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
 */

#endregion

#region Using Statements

using DemeterEngine;
using Refraction_V2.Multiforms.Level;
using Refraction_V2.Multiforms.LevelComplete;
using Refraction_V2.Multiforms.LevelSelect;
using Refraction_V2.Multiforms.MainMenu;

#endregion

namespace Refraction_V2
{
    public class RefractionGameKernel : GameKernel
    {

        public RefractionGameKernel(string setupFileName) : base(setupFileName) { }

		public override void InitializeGame()
		{
			base.InitializeGame();
			UserConfig.Load();
		}

        public override void LoadMultiforms()
        {
            
            MultiformManager.RegisterMultiform(MainMenuMultiform.MultiformName,      new MainMenuMultiform());
            MultiformManager.RegisterMultiform(LevelSelectMultiform.MultiformName,   new LevelSelectMultiform());
            MultiformManager.RegisterMultiform(LevelMultiform.MultiformName,         new LevelMultiform());
            MultiformManager.RegisterMultiform(LevelCompleteMultiform.MultiformName, new LevelCompleteMultiform());

            MultiformManager.Construct(MainMenuMultiform.MultiformName);
        }

    }
}
