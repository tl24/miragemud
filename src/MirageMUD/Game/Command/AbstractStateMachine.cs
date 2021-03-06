using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mirage.Game.Communication;
using Mirage.Game.IO.Net;
using Mirage.Core.Messaging;
using Mirage.Core.IO.Net;

namespace Mirage.Game.Command
{

    /// <summary>
    /// Base class for a State Machine.  The state machine guides
    /// the Player/Session through a range of states.  Prompting for
    /// necessary items when needed.
    /// </summary>
    public abstract class AbstractStateMachine : ILoginInputHandler
    {
        /// <summary>
        /// Holds context items for the State Machine
        /// </summary>
        private HybridDictionary _properties;

        /// <summary>
        /// The current state for the state machine.  SubClasses set
        /// this by calling Require
        /// </summary>
        private ValidateDelegate _currentState;

        /// <summary>
        /// Delegate type for the validation routines.  Passed as
        /// an argument to Require.
        /// </summary>
        /// <param name="input">The user's current input string</param>
        public delegate void ValidateDelegate(object input);

        /// <summary>
        /// Creates an instance with the given client.  The client
        /// object is used to send prompts for information.
        /// </summary>
        /// <param name="client">The current client</param>
        public AbstractStateMachine(IClient<ClientPlayerState> client)
        {
            _properties = new HybridDictionary();
            this.Client = client;
        }

        /// <summary>
        /// Gets a value from the State Machine context, cast to
        /// the correct type.
        /// </summary>
        /// <typeparam name="T">The desired type of the object</typeparam>
        /// <param name="name">The name of the parameter to retrieve</param>
        /// <returns>The specified value</returns>
        public T GetValue<T>(string name)
        {
            if (_properties.Contains(name))
            {
                return (T)_properties[name];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets a value in the state machine context.  The value
        /// is converted to the specified type before being stored
        /// </summary>
        /// <typeparam name="T">The desired type of the object</typeparam>
        /// <param name="name">The parameter key</param>
        /// <param name="value">The new value</param>
        public void SetValue<T>(string name, object value)
        {
            _properties[name] = Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Returns true if the state machine context contains the
        /// given value.
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>true if the parameter has been set</returns>
        public bool Contains(string name)
        {
            return _properties.Contains(name);
        }

        /// <summary>
        /// Removes the given parameter from the context.
        /// </summary>
        /// <param name="name">true if the parameter has been set</param>
        public void Remove(string name)
        {
            if (Contains(name))
                _properties.Remove(name);
        }

        /// <summary>
        /// Removes the specified name(s) from the state machine context
        /// </summary>
        /// <param name="names">a list of names to remove</param>
        public void Remove(params string[] names)
        {
            foreach (string name in names)
                Remove(name);
        }

        /// <summary>
        /// Removes all parameters from the context
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }

        /// <summary>
        /// This flag is set when the state machine should enter into the final state
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// The client object that this state machine is managing
        /// </summary>
        public IClient<ClientPlayerState> Client { get; set; }

        /// <summary>
        /// Called when a given parameter is not set.  The user will be prompted with
        /// the given prompt and the nextStep delegate will be used to validate the 
        /// resulting input from the client.
        /// </summary>
        /// <param name="messageName">The name of the prompt message</param>
        /// <param name="prompt">The prompt text</param>
        /// <param name="nextStep">The next step in the state machine</param>
        public void Require(string messageName, string prompt, ValidateDelegate nextStep)
        {
            Require(new StringMessage(MessageType.Prompt, messageName, prompt), nextStep);
        }

        /// <summary>
        /// Called when a given parameter is not set.  The user will be prompted with
        /// the given prompt and the nextStep delegate will be used to validate the 
        /// resulting input from the client.
        /// </summary>
        /// <param name="prompt">The prompt</param>
        /// <param name="nextStep">The next step in the state machine</param>
        public void Require(IMessage prompt, ValidateDelegate nextStep)
        {
            Client.Write(prompt);
            _currentState = nextStep;
        }

        public void Require(IEnumerable<IMessage> messages, ValidateDelegate nextStep)
        {
            foreach (IMessage m in messages)
            {
                Client.Write(m);
            }
            _currentState = nextStep;
        }

        /// <summary>
        /// Called to handle incoming input and determine the next state transition.
        /// The validation step will be called and then the next state will be determined
        /// and run.
        /// </summary>
        /// <param name="input">client input</param>
        public void HandleInput(object input)
        {
            if (_currentState == null)
            {
                InitialState();
            }
            else
            {
                _currentState(input);
            }

            if (!Finished)
            {
                DetermineNextState();
            }
            if (Finished)
            {
                FinalState();
                Clear();
            }
        }

        /// <summary>
        /// Evaluate the state machine logic and determine the next step.  Clients
        /// should implement with a series of boolean checks and Require statements.
        /// </summary>
        /// <remarks>
        /// Example code:
        /// <code>
        /// if (!Contains&lt;string&gt;("somevalue")) 
        ///     Require("Prompt.SomeValue", "Enter Some Value:", new ValidateDelegate(this.ValidateSomeValue));
        /// else if (!Contains&lt;string&gt;("othervalue"))
        ///     Require("Prompt.OtherValue", "Enter Other Value:", new ValidateDelegate(this.OtherValue));
        /// else
        ///     Finished = true;
        /// </code>
        /// </remarks>
        protected abstract void DetermineNextState();

        /// <summary>
        /// This will be called as the initial state
        /// </summary>
        protected abstract void InitialState();

        /// <summary>
        /// This will be called as the final step
        /// </summary>
        protected abstract void FinalState();
    }
}
