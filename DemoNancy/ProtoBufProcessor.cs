using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Responses.Negotiation;
using ProtoBuf;

namespace DemoNancy
{
    public class ProtoBufProcessor : IResponseProcessor
    {
        public const string KeyApplicationXProtobuf = "application/x-protobuf";

        public ProcessorMatch CanProcess(MediaRange
        requestedMediaRange, dynamic model, NancyContext
        context)
        {
            if (requestedMediaRange.Matches(new MediaRange
            (KeyApplicationXProtobuf)))
                return new ProcessorMatch
                {
                    ModelResult =
                        MatchResult.DontCare,
                    RequestedContentTypeResult
                        = MatchResult.ExactMatch
                };
            if (requestedMediaRange.Subtype.ToString()
            .EndsWith("protobuf"))
                return new ProcessorMatch
                {
                    ModelResult =
                        MatchResult.DontCare,
                    RequestedContentTypeResult
                        = MatchResult.NonExactMatch
                };
            return new ProcessorMatch
            {
                ModelResult =
                    MatchResult.DontCare,
                RequestedContentTypeResult =
                    MatchResult.NoMatch
            };
        }
        public Response Process(MediaRange requestedMediaRange,
        dynamic model, NancyContext context)
        {
            return new Response
            {
                Contents = stream => Serializer.Serialize(stream,
                model),
                ContentType = KeyApplicationXProtobuf
            };
        }
        public IEnumerable<Tuple<string, MediaRange>>
        ExtensionMappings => new[] { new Tuple<string,
            MediaRange>(".protobuf", new MediaRange(KeyApplicationXProtobuf)) };
    }
}