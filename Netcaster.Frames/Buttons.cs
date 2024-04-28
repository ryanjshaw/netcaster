using Microsoft.Extensions.Primitives;

namespace Netcaster.Frames
{
    public class Button(string content, string action)
    {
        public string Content => content;
        public string Action => action;
    }

    public class FrameButton(string content, Frame destination)
        : Button(content, "post")
    {
        public Frame Destination => destination;
    }

    public class QueryButton : FrameButton
    {
        public QueryButton(string content, Frame destination, bool mergeQueryValues, params (object, object)[] queryValues)
            : base(content, destination)
        {
            _queryValues = (
                from qv in queryValues
                let key = qv.Item1.ToString()
                let value = new StringValues(qv.Item2.ToString())
                select (key, value)
            ).ToArray();

            MergeQueryValues = mergeQueryValues;
        }

        public QueryButton(string content, Frame destination, params (object, object)[] queryValues)
            : this(content, destination, false, queryValues) { }

        private (string, StringValues)[] _queryValues;
        public (string, StringValues)[] QueryValues => _queryValues;

        public bool MergeQueryValues { get; }
    }

    public class RouteButton(string content, Frame destination, object routeValues)
        : FrameButton(content, destination)
    {
        public object RouteValues => routeValues;
    }

}
