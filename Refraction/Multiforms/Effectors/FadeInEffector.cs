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

using DemeterEngine.Effectors;

#endregion

namespace Refraction_V2.Multiforms.Effectors
{
    public class FadeInEffector : Effector
    {

        /// <summary>
        /// The duration of the fade-in.
        /// </summary>
        public int Duration { get; private set; }

        public FadeInEffector(int frameDuration)
            : base()
        {
            Duration = frameDuration;
        }

        /// <summary>
        /// The function that calculates the alpha at a given time.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private float AlphaFunction(int frame)
        {
            return frame / (float)Duration;
        }

        public override void Update()
        {
            base.Update();

            var form = (ITransitionalForm)Form;
            form.SetAlpha(255 * AlphaFunction(LocalFrame));

            if (LocalFrame >= Duration)
            {
                form.SetAlpha(255f);
                Kill();
            }
        }
    }
}
