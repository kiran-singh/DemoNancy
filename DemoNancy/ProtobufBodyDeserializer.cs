using System.IO;
using Nancy.ModelBinding;
using ProtoBuf;

namespace DemoNancy
{
    public class ProtobufBodyDeserializer : IBodyDeserializer
    {
        public bool CanDeserialize(string contentType)
        {
            return contentType == ProtoBufProcessor.KeyApplicationXProtobuf;
        }

        public bool CanDeserialize(string contentType, BindingContext context)
        {
            return contentType == ProtoBufProcessor.KeyApplicationXProtobuf;
        }

        public object Deserialize(string contentType, Stream
            bodyStream, BindingContext context)
        {
            return Serializer.NonGeneric.Deserialize
                (context.DestinationType, bodyStream);
        }
    }
}