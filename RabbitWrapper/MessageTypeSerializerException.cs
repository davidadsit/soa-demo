using System;

namespace RabbitWrapper
{
    public class MessageTypeSerializerException : Exception
    {
        public MessageTypeSerializerException(string message, string typeName) : base(string.Format("[{0}]: {1}", typeName, message))
        {
            
        }
    }
}