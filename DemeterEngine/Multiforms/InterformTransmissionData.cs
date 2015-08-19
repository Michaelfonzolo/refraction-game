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
 * A piece of data that can be sent between arbitrary objects. This is used
 * by forms and multiforms which don't usually have direct access to each other,
 * but are rather different branches of the same parent node. 
 * 
 * For example, two forms attached to the same multiform don't necessarily directly 
 * interact with each other, but they are "children" of the same "parent" object 
 * (the multiform).
 */

#endregion

#region Using Statements

using DemeterEngine.Utils;

#endregion

namespace DemeterEngine.Multiforms
{
    public class InterformTransmissionData<T> : AttributeContainer
    {

        /// <summary>
        /// The name of the form/multiform that sent this data packet.
        /// </summary>
        public string SenderName { get; internal set; }

		/// <summary>
		/// The name of the transmission. This is used to distinct transmission types
		/// from the same sender.
		/// </summary>
        public string TransmissionName { get; internal set; }

		/// <summary>
		/// The actual sender object.
		/// </summary>
        public T Sender { get; internal set; }

        public InterformTransmissionData() : this("", "", default(T)) { }

        public InterformTransmissionData(string sender, string transmission, T t)
        {
            SenderName = sender;
            TransmissionName = transmission;
            Sender = t;
        }

    }
}
