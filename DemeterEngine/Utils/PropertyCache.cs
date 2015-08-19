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
 * Date of Creation: 6/27/2015
 * 
 * Description
 * ===========
 * A PropertyCache<T> represents a single property of type T, along with a boolean
 * state called ``dirty``. This dirty state indicates whether or not the value needs
 * to be refreshed (recalculated). This is useful to store memoized results. 
 * 
 * For example, if you have a property that is expensive to calculate, and you are 
 * accessing it many times but it doesn't change very often (but it DOES still change), 
 * then storing it as a PropertyCache ensures it only gets recalculated when it is 
 * dirty, and not every time it is accessed.
 */

#endregion


namespace DemeterEngine.Utils
{
    public class PropertyCache<T>
    {
        /// <summary>
        /// The actual value stored.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// A setter that sets the internal Value to the given one, and
        /// the dirty state to false.
        /// </summary>
        public T Clean
        {
            set
            {
                Value = value;
                dirty = false;
            }
        }

        /// <summary>
        /// Whether or not the value stored is dirty (i.e. needs to be refreshed).
        /// </summary>
        public bool dirty { get; set; }

        public PropertyCache()
        {
            dirty = true;
        }

    }
}
