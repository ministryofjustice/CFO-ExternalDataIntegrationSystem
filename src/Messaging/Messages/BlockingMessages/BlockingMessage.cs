﻿using Messaging.Queues;

namespace Messaging.Messages.BlockingMessages
{
    public abstract class BlockingMessage : Message
    {
        public TBlockingQueue routingKey { get; set; }
        public string fileName = string.Empty; //To handle processing of multiple files. 
    }
}