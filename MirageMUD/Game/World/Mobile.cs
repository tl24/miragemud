using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;
using System.Security.Principal;
using Mirage.Game.Command;
using Mirage.Game.World.MobAI;

namespace Mirage.Game.World
{
    /// <summary>
    /// Non-player chars, monsters, mobs, etc
    /// </summary>
    public class Mobile : Living, IDisposable
    {
        private IPrincipal _principal;
        // mob uri's can be duplicates so give us an ID to distinguish us
        private Guid _id;
        private Queue<IMobileCommand> _commands;
        private IList<AIProgram> _programs;
        private MobTemplate _template;

        public Mobile(MobTemplate template)
            : base()
        {
            _id = Guid.NewGuid();
            _commands = new Queue<IMobileCommand>();
            _programs = new List<AIProgram>();
        }

        public void ProcessInput()
        {
            foreach(AIProgram prog in Programs) {
                prog.GenerateInput();
            }
            if (_commands.Count > 0) {
                IMobileCommand command = _commands.Dequeue();
                command.Execute(this);
            }
        }

        public override void Write(IMessage message)
        {
            foreach(AIProgram prog in Programs) {
                AIMessageResult result = prog.HandleMessage(message);
                if (result == AIMessageResult.MessageHandledStop)
                    break;
            }
        }

        public override void Write(object sender, IMessage message)
        {
            Write(message);
        }

        public override IPrincipal Principal
        {
            get {
                if (_principal != null)
                    return _principal;

                if (!string.IsNullOrEmpty(this.Uri))
                {
                    _principal = new MobPrincipal(this.Uri);
                }
                else
                {
                    return new MobPrincipal("");
                }
                return _principal;
            }
        }

        public Guid ID
        {
            get { return this._id; }
        }

        public Queue<IMobileCommand> Commands
        {
            get { return this._commands; }
        }

        public IList<AIProgram> Programs
        {
            get { return this._programs; }
        }

        public MobTemplate Template
        {
            get { return _template; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Commands.Clear();
            Programs.Clear();
            Template.Mobiles.Remove(this);
        }

        #endregion
    }

    public interface IMobileCommand
    {
        void Execute(Mobile actor);
    }

    public struct MobileStringCommand : IMobileCommand
    {
        private string _command;

        public MobileStringCommand(string command)
        {
            _command = command;
        }


        #region IMobileCommand Members

        public void Execute(Mobile actor)
        {
            Interpreter.ExecuteCommand(actor, _command);
        }

        #endregion
    }

    public struct MobileTypedCommand : IMobileCommand
    {
        private string _commandName;
        private object[] _arguments;

        public MobileTypedCommand(string commandName, object[] arguments)
        {
            _commandName = commandName;
            _arguments = arguments;
        }


        #region IMobileCommand Members

        public void Execute(Mobile actor)
        {
            MethodInvoker.Interpret(actor, _commandName, _arguments);
        }

        #endregion
    }

}
