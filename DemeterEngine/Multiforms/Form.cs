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
 * Date of Creation: 6/17/2015
 * 
 * Description
 * ===========
 * A Form is anything that can be added to a Multiform, can be updated over time, rendered,
 * and can have Effectors attached to it.
 */

#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using DemeterEngine.Effectors;
using DemeterEngine.Extensions;
using DemeterEngine.Utils;

#endregion

namespace DemeterEngine.Multiforms
{
    public class Form : ChronometricObject
    {

        /// <summary>
        /// The parent multiform this form is attached to.
        /// </summary>
        public Multiform Parent { get; private set; }

        /// <summary>
        /// The list of all anonymous effectors currently attached to this form.
        /// </summary>
        private List<Effector> anonymousEffectors = new List<Effector>();

        /// <summary>
        /// The dictionary of all named effectors currently attached to this form.
        /// </summary>
        private Dictionary<string, Effector> namedEffectors = new Dictionary<string, Effector>();

        /// <summary>
        /// The container of attributes attached to this form.
        /// 
        /// We unfortunately can't just inherit from AttributeContainer because that would mess
        /// with the inheritance chain (can't inherit from AttributeContainer and EventListener
        /// simultaneously).
        /// </summary>
        public AttributeContainer Attributes { get; protected set; }

        /// <summary>
        /// Whether or not we have begun updating this form. The value of this property is false
        /// between the time the instance is initialized and the first time it's update method
        /// is called.
        /// </summary>
        public bool Begun { get; private set; }

        public Form(bool keepTime = false, int initialFrame = 0, double initialTime = 0)
            : base(keepTime, initialFrame, initialTime)
        {
            Begun = false;
            Attributes = new AttributeContainer();
        }

        /// <summary>
        /// Set this form's parent.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(Multiform parent)
        {
            if (this.Parent != null)
                throw new ArgumentException("This form has already been added to a multiform.");
            this.Parent = parent;
			PostConstruct();
        }

		/// <summary>
		/// Any construction operations to perform on this form after it's parent has been set.
		/// </summary>
		public virtual void PostConstruct() { }

        /// <summary>
        /// Add an anonymous effector to this Form.
        /// </summary>
        /// <param name="effector"></param>
        public void AddEffector(Effector effector)
        {
            if (effector.form == null)
                effector.AttachTo(this);
            anonymousEffectors.Add(effector);
        }

        /// <summary>
        /// Add a named effector to this Form.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="effector"></param>
        public void AddEffector(string name, Effector effector)
        {
            if (effector.form == null)
                effector.AttachTo(this);
            namedEffectors[name] = effector;
        }

        /// <summary>
        /// Return a named effector.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Effector GetEffector(string name)
        {
            return namedEffectors[name];
        }

        /// <summary>
        /// Get a list of all active effectors attached to this Form.
        /// </summary>
        /// <returns></returns>
        public Effector[] GetEffectors()
        {
            return anonymousEffectors.ToArray().Concat(namedEffectors.Values.ToArray());
        }

        /// <summary>
        /// Clear the effectors attached to this Form.
        /// </summary>
        public void ClearEffectors()
        {
            anonymousEffectors.Clear();
            namedEffectors.Clear();
        }

        /// <summary>
        /// Send information to another (named) form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="data"></param>
        protected void SendInfoTo(string formName, FormTransmissionData data)
        {
            data.Sender = this;
            Parent.GetForm(formName).ReceiveData(data);
        }

        /// <summary>
        /// Receive information from another form.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void ReceiveData(FormTransmissionData data) { }

        /// <summary>
        /// Send a request to another (named) form requesting information.
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="formName"></param>
        /// <param name="requestName"></param>
        protected void RequestInfoFrom(string senderName, string formName, string requestName)
        {
            Parent.GetForm(formName).ReceiveRequest(senderName, requestName);
        }

        /// <summary>
        /// Receive a request from another (named) form requesting information.
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="requestName"></param>
        protected virtual void ReceiveRequest(string senderName, string requestName) { }

        /// <summary>
        /// Update the Form.
        /// </summary>
        public virtual void Update()
        {
            Begun = true;
            base.UpdateTime();
            UpdateEffectors();
        }

        /// <summary>
        /// Update the effectors attached to this Form.
        /// </summary>
        private void UpdateEffectors()
        {
            foreach (var effector in anonymousEffectors)
                effector.Update();
            anonymousEffectors.RemoveAll(e => e.dead);

            var dead = new List<string>();
            foreach (var effector in namedEffectors)
            {
                effector.Value.Update();
                if (effector.Value.dead)
                    dead.Add(effector.Key);
            }

            foreach (var name in dead)
                namedEffectors.Remove(name);
        }

        /// <summary>
        /// Render the Form.
        /// </summary>
        public virtual void Render()
        {

        }

    }
}
