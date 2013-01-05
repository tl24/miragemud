using System;
using System.Collections.Generic;
using System.Text;
using Mirage.Game.Communication;
using Mirage.Core.Messaging;

namespace MirageGUI.Code
{
    public delegate ProcessStatus ResponseHandler(Message response);

    /// <summary>
    /// Status of the processing of the event
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// Event successfully processed, stop further handlers from processing
        /// </summary>
        SuccessAbort,
        /// <summary>
        /// Event successfully processed, but let other handlers process as well
        /// </summary>
        SuccessContinue,
        /// <summary>
        /// No handlers have processed this event yet
        /// </summary>
        NotProcessed
    }

    public interface IResponseHandler
    {
        ProcessStatus HandleResponse(Message response);
    }
}
