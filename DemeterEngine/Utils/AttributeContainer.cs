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
 * Date of Creation: 6/29/2015
 * 
 * Description
 * ===========
 * Represents an object that can store arbitrary attributes of arbitrary types, similar
 * to how you can just add attributes to any python object at will.
 */

#endregion

#region Using Statements

using System;
using System.Collections.Generic;

#endregion

namespace DemeterEngine.Utils
{
    public class AttributeContainer
    {

        /// <summary>
        /// The interior dictionary of attributes. The Value type is actually Dictionary<string, T>
        /// where T is an arbitrary type parameter. This is specified by the GetAttr<T>, SetAttr<T>,
        /// and TryGetValue<T> methods.
        /// </summary>
        private Dictionary<Type, object> attributes = new Dictionary<Type, object>();

        /// <summary>
        /// Retrieve an attribute by it's name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetAttr<T>(string name)
        {
            return ((Dictionary<string, T>)attributes[typeof(T)])[name];
        }

        /// <summary>
        /// Set an item as an attribute with the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void SetAttr<T>(string name, T val)
        {
            var t = typeof(T);

            object obj;

            if (attributes.TryGetValue(t, out obj))
            {
                // ``obj`` is never null, so we don't have to use ``as``, we can just cast.
                Dictionary<string, T> dictionary = (Dictionary<string, T>)obj;
                dictionary[name] = val;
            }
            else
                attributes[t] = new Dictionary<string, T>() { { name, val } };
        }

        /// <summary>
        /// Attempt to retrieve an attribute with the given name and store it to
        /// the out value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetAttr<T>(string name, out T val)
        {
            object obj;

            if (attributes.TryGetValue(typeof(T), out obj))
            {
                Dictionary<string, T> dictionary = (Dictionary<string, T>)obj;
                if (dictionary != null)
                {
                    val = dictionary[name];
                    return true;
                }
            }

            val = default(T);
            return false;
        }

    }
}
