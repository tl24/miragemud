using Mirage.Game.Communication;

namespace Mirage.Game.World.MobAI
{

    public enum AIMessageResult
    {
        /// <summary>
        /// Indicates the message was not handled and
        /// control should pass to the next handler
        /// </summary>
        MessageNotHandled,

        /// <summary>
        /// Indicates the message was handled, but other
        /// programs should still get a chance to handle the message
        /// </summary>
        MessageHandledContinue,

        /// <summary>
        /// Indicates the message was handled and no other handlers should
        /// process the message
        /// </summary>
        MessageHandledStop
    }

    /// <summary>
    /// Base class for AI for mobs
    /// </summary>
    public class AIProgram
    {
        private Mobile _mob;
        public AIProgram(Mobile mob)
        {
            _mob = mob;
        }

        /// <summary>
        /// The mobile for which the program is acting for
        /// </summary>
        public Mobile Mob
        {
            get { return this._mob; }        
        }

        /// <summary>
        /// This method will be called at the beginning of ProcessInput and will
        /// allow the program to generate commands that are not triggered by incomming messages
        /// </summary>
        public virtual void GenerateInput() {
        }

        /// <summary>
        /// Called when the mobile receives a message.  The result returned indicates how the message
        /// was handling and if further processing of the message should continue
        /// </summary>
        /// <param name="message">message to process</param>
        /// <returns>processing result</returns>
        public virtual AIMessageResult HandleMessage(IMessage message)
        {
            return AIMessageResult.MessageNotHandled;
        }
    }
}
