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
 * Date of Creation: 6/12/2015
 * 
 * Description
 * ===========
 * The MultiformManager is a manager object that stores all multiforms in the game,
 * and initializes them, constructs, them, updates them, transitions between them,
 * transfers data between them, and destroys them accordingly. There can only be 
 * one active multiform at a time.
 * 
 * The MultiformManager also stores a dictionary of global forms not attached to
 * any multiform. 
 */

#endregion

#region Using Statements

using System;
using System.Collections.Generic;

#endregion

namespace DemeterEngine.Multiforms
{

    public class MultiformManager
    {

        /// <summary>
        /// The dictionary of registered multiforms.
        /// </summary>
        private Dictionary<string, Multiform> registeredMultiforms =
            new Dictionary<string, Multiform>();

        /// <summary>
        /// The list of currently active multiforms.
        /// </summary>
        private List<string> currentlyActive = new List<string>();

        /*
         * PostUpdateEvents are things the manager IS TOLD to do during the first foreach loop in Update
         * (which updates the Multiforms), but HAS to do AFTERWARDS. The reason they have to be done after
         * the main foreach loop is because otherwise, they would cause a ConcurrentModificationException
         * to be thrown (since they are modifiying the currentlyActive list).
         */

        private abstract class PostUpdateEvent
        {
            public abstract void Perform(
                Dictionary<string, Multiform> registered,
                List<string> current);
        }

        /// <summary>
        /// The event that corresponds to constructing a registered multiform.
        /// </summary>
        private class ConstructEvent : PostUpdateEvent
        {
            string name;
            MultiformTransmissionData data;
            public ConstructEvent(string name, MultiformTransmissionData data)
            {
                this.name = name;
                this.data = data;
            }
            public override void Perform(
                Dictionary<string, Multiform> registered,
                List<string> current)
            {
                var multiform = registered[name];
                current.Add(name);
                multiform.Construct(data);
            }
        }

        /// <summary>
        /// The event that corresponds to closing an active multiform.
        /// </summary>
        private class CloseEvent : PostUpdateEvent
        {
            string name;
            public CloseEvent(string name) { this.name = name; }
            public override void Perform(
                Dictionary<string, Multiform> registered,
                List<string> current)
            {
                current.Remove(name);
            }
        }

        /// <summary>
        /// The event that corresponds to bringing a multiform to the front of the update list.
        /// </summary>
        private class BringToFrontEvent : PostUpdateEvent
        {
            string name;
            public BringToFrontEvent(string name) { this.name = name; }
            public override void Perform(
                Dictionary<string, Multiform> registered,
                List<string> current)
            {
                current.Remove(name);
                current.Insert(0, name);
            }
        }

        /// <summary>
        /// The event that corresponds to sending a multiform to the back of the update list.
        /// </summary>
        private class SendToBackEvent : PostUpdateEvent
        {
            string name;
            public SendToBackEvent(string name) { this.name = name; }
            public override void Perform(
                Dictionary<string, Multiform> registered,
                List<string> current)
            {
                current.Remove(name);
                current.Add(name);
            }
        }

        /* 
         * We have to store the list of multiforms to construct and close in their own lists because 
         * if we tried to construct/close them immediately we would cause a ConcurrentModificationException 
         * to be thrown, since the only location multiform's can be closed or constructed is from within
         * a multiform being updated by the foreach loop in Update.
         */

        // The list of multiforms to construct in the next call to Update.
        private List<PostUpdateEvent> postUpdateEvents = new List<PostUpdateEvent>();

        public MultiformManager() { }

        /// <summary>
        /// Register a multiform by a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multiform"></param>
        public void RegisterMultiform(string name, Multiform multiform)
        {
            registeredMultiforms[name] = multiform;
            multiform.SetManager(this);
        }

        /// <summary>
        /// Revoke the registered multiform by the given name. This means it is removed from the dictionary
        /// of registered multiforms entirely, and the manager stops tracking it.
        /// </summary>
        /// <param name="name"></param>
        public void RevokeMultiform(string name)
        {
            registeredMultiforms[name].SetManager(null);
            registeredMultiforms.Remove(name);
        }

        /// <summary>
        /// Construct the registered multiform with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Construct(string name)
        {
            Construct(name, null);
        }

		public Multiform GetMultiform(string name)
		{
			return registeredMultiforms[name];
		}

		public T GetMultiform<T>(string name) where T : Multiform
		{
			try
			{
				return (T)registeredMultiforms[name];
			}
			catch (KeyNotFoundException)
			{
				throw new MultiformException(
					String.Format("No registered multiform with name '{0}' was found.", name));
			}
		}

		public Multiform GetActiveMultiform(string name)
		{
			if (!currentlyActive.Contains(name))
				throw new MultiformException(
					String.Format("Multiform with name '{0}' is not active.", name));
			return GetMultiform(name);
		}

		public T GetActiveMultiform<T>(string name) where T : Multiform
		{
			if (!currentlyActive.Contains(name))
				throw new MultiformException(
					String.Format("Multiform with name '{0}' is not active.", name));
			return GetMultiform<T>(name);
		}

        /// <summary>
        /// Construct the registered multiform with the given name. This overload sends in
        /// the given data into the multiform's Construct method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void Construct(string name, MultiformTransmissionData data)
        {
            if (!registeredMultiforms.ContainsKey(name))
                throw new MultiformException(
                    String.Format(
                        "No multiform has been registered with the name \"{0}\"." +
                        "\nCannot construct an unregistered multiform.", name)
                        );
            if (data != null)
                data.Sender = registeredMultiforms[data.SenderName];
            postUpdateEvents.Add(new ConstructEvent(name, data));
        }

        /// <summary>
        /// Close the registered multiform with the given name.
        /// </summary>
        /// <param name="name"></param>
        public void Close(string name)
        {
            if (!currentlyActive.Contains(name))
                throw new MultiformException(
                    String.Format(
                        "There is no currently active multiform with the name \"{0}\"." +
                        "\nCannot close an inactive multiform.", name)
                        );
            postUpdateEvents.Add(new CloseEvent(name));
        }

		public void Close(Multiform multiform)
		{
			foreach (var name in currentlyActive)
				if (multiform == registeredMultiforms[name])
					Close(name);
		}

        private MultiformException _UpdateOrderException(string name)
        {
            return new MultiformException(
                    String.Format(
                        "There is no currently active multiform with the name \"{0}\"." +
                        "\nCannot change the update order of an inactive multiform.", name)
                        );
        }

        /// <summary>
        /// Bring the active multiform with the given name to the front of the update list. Multiforms
        /// that are closer to the front of the update list get updated first and rendered last, whereas
        /// multiforms closer to the back get updated last and rendered first. The reason for this is 
        /// because multiforms that are closer to the front should be rendered on top of (i.e. in front of)
        /// the multiforms closer to the bottom, which is the opposite to the order they are updated in.
        /// </summary>
        /// <param name="name"></param>
        public void BringToFront(string name)
        {
            if (!currentlyActive.Contains(name))
                throw _UpdateOrderException(name);
            postUpdateEvents.Add(new BringToFrontEvent(name));
        }

        /// <summary>
        /// Send the active multiform with the given name to the back of the update list.
        /// </summary>
        /// <param name="name"></param>
        public void SendToBack(string name)
        {
            if (!currentlyActive.Contains(name))
                throw _UpdateOrderException(name);
            postUpdateEvents.Add(new SendToBackEvent(name));
        }

        /// <summary>
        /// Update all the multiforms.
        /// </summary>
        /// <param name="serviceLocator"></param>
        public void Update()
        {
            Multiform m;
            foreach (var name in currentlyActive)
            {
                m = registeredMultiforms[name];
                if (m.Updater == null)
                    throw new MultiformException(
                        String.Format("The multiform with the name \"{0}\" has no updater.", name));
                 m.Updater();
            }

            foreach (var evt in postUpdateEvents)
                evt.Perform(registeredMultiforms, currentlyActive);
            postUpdateEvents.Clear();
        }

        /// <summary>
        /// Render all the currently multiforms.
        /// </summary>
        /// <param name="serviceLocator"></param>
        public void Render()
        {
            Multiform m;
            // Iterate through the currently active multiforms in reverse order so as to render
            // them in reverse order. This makes multiforms at the front get rendered last, so
            // as to appear on top of multiforms closer to the back.
            for (int i = currentlyActive.Count - 1; i >= 0; i--)
            {
                m = registeredMultiforms[currentlyActive[i]];
                if (m.Renderer == null)
                    throw new MultiformException(
                        String.Format("The multiform with the name \"{0}\" has no renderer.", currentlyActive[i])
                        );
                m.Renderer();
            }
        }

    }
}
