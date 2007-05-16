using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Shoop.IO;
using Shoop.Communication;

namespace Shoop.Command
{
    public abstract class AbstractStateMachine
    {
        private HybridDictionary _properties;
        private ValidateValue _nextState;
        private bool _finished;
        private IClient _client;

        public delegate void ValidateValue(string input);

        public AbstractStateMachine(IClient client)
        {
            _properties = new HybridDictionary();
            this._client = client;
        }

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

        public void SetValue<T>(string name, object value)
        {
            _properties[name] = Convert.ChangeType(value, typeof(T));
        }

        public bool Contains(string name)
        {
            return _properties.Contains(name);
        }

        public void Remove(string name)
        {
            if (Contains(name))
                _properties.Remove(name);
        }

        public void Remove(params string[] names)
        {
            foreach (string name in names)
                Remove(name);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }

        public IClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        public void Require(string messageName, string prompt, ValidateValue nextStep)
        {
            Require(new StringMessage(MessageType.Prompt, messageName, prompt), nextStep);
        }

        public void Require(Message prompt, ValidateValue nextStep)
        {
            _client.Write(prompt);
            _nextState = nextStep;
        }

        public void HandleInput(string input)
        {
            if (_nextState == null)
            {
                InitialState();
            }
            else
            {
                _nextState(input);
            }

            if (!_finished)
            {
                Validate();
            }
            if (_finished)
            {
                FinalState();
                Clear();
            }
        }

        protected abstract void Validate();

        protected abstract void InitialState();

        protected abstract void FinalState();
    }
}
