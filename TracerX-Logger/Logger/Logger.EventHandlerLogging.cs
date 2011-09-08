using System;
using System.Threading;
using System.ComponentModel;

namespace TracerX 
{
    public partial class Logger
    {
        /// <summary>
        /// Methods and configuration for logging via the <see cref="Logger.MessageCreated"/> event, which users
        /// write their own handlers for.
        /// </summary>
        internal static class EventHandlerLogging
        {
            // Returns true if the event was cancelled.
            internal static bool LogMsg(Logger logger, ThreadData threadData, TraceLevel msgLevel, string msg, bool methodEntered, bool methodExiting)
            {
                bool result = false;

                if (MessageCreated != null)
                {
                    try
                    {
                        var args = new MessageCreatedEventArgs
                        {
                            Indentation = threadData.EventHandlerState.StackDepth,
                            Method = threadData.EventHandlerState.CurrentMethod,
                            ThreadNumber = threadData.TracerXID,
                            Message = msg,
                            TraceLevel = msgLevel,
                            ThreadName = threadData.Name,
                            LoggerName = logger.Name,
                            MethodEntered = methodEntered,
                            MethodExiting = methodExiting
                        };

                        MessageCreated(logger, args);
                        result = args.Cancel;
                    }
                    catch { }
                }

                return result;
            }
        }

        /// <summary>
        /// Event args for the <see cref="MessageCreated"/> event, which allows you to cancel the
        /// log message or perform custom processing. 
        /// </summary>
        [Serializable]
        public class MessageCreatedEventArgs : CancelEventArgs
        {
            /// <summary>
            /// The message text passed to Logger.Info(), Logger.Debug(), etc.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// The TraceLevel associated with the message. 
            /// </summary>
            public TraceLevel TraceLevel { get; set; }

            /// <summary>
            /// Basically equal to the stack depth.
            /// </summary>
            public int Indentation { get; set; }

            /// <summary>
            /// The ID number assigned by TracerX to the thread that logged the message.  This is not the same as the thread's ManagedThreadID.
            /// </summary>
            public int ThreadNumber { get; set; }

            /// <summary>
            /// The name of the thread that logged the message.
            /// </summary>
            public string ThreadName { get; set; }

            /// <summary>
            /// The name of the method that logged the message, determined by your calls to InfoCall(), DebugCall(), etc.
            /// </summary>
            public string Method { get; set; }

            /// <summary>
            /// The name of the Logger instance that logged the message.
            /// </summary>
            public string LoggerName { get; set; }

            /// <summary>
            /// True if this is a "method entered" message.
            /// </summary>
            public bool MethodEntered { get; set; }

            /// <summary>
            /// True if this is a "method exiting" message.
            /// </summary>
            public bool MethodExiting { get; set; }
        }
    }
}
