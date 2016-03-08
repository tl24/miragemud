using System.Collections.Generic;
using Mirage.Game.Command;
using Mirage.Game.Communication;
using Mirage.Game.IO.Net;
using Mirage.Game.World;
using Mirage.Core.IO.Net;
using Mirage.Core.Messaging;

namespace NUnitTests
{
    public class MockClient : IClient<ClientPlayerState>
    {
        public MockClient()
        {
            ClientState = new ClientPlayerState();
            ClientState.State = ConnectedState.Playing;
            IsOpen = true;
            Messages = new List<IMessage>();
        }

        public bool OutputWritten { get; set; }

        public bool CommandRead { get; set; }

        public bool IsOpen { get; set; }

        public string Address
        {
            get { return this.GetType().FullName; }
        }

        public List<IMessage> Messages { get; private set; }

        public ClientPlayerState ClientState { get; private set; }

        public void Initialize()
        {
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void ProcessInput()
        {
            CommandRead = true;
        }

        public void Write(IMessage message)
        {
            Messages.Add(message);
            OutputWritten = true;
        }

    }
}
