using System;
using System.Collections.Generic;
using RestSharp;


namespace libEnbilulu
{
    using Models;

    public class Client
    {
        private Uri endpoint;

        public Client(string host, int port = 16016)
        {
            endpoint = new Uri($"{host}:{port}");
        }

        public Stream CreateStream(string stream)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/streams/{stream}", Method.POST);
            request.AddUrlSegment("stream", stream);

            var response = client.Post<Stream>(request);

            return response.Data;
        }

        public Stream PutRecord(string stream, dynamic data)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/points/{stream}", Method.POST);
            request.AddUrlSegment("stream", stream);
            request.AddJsonBody(data);

            var response = client.Post<Stream>(request);

            return response.Data;
        }

        public Stream GetStream(string stream)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/streams/{stream}", Method.GET);
            request.AddUrlSegment("stream", stream);

            var response = client.Get<Stream>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }
            return null;
        }

        public Section GetRecordsAfter(string stream, int start, int limit)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/points/{stream}/{start}/{limit}", Method.GET);
            request.AddUrlSegment("stream", stream);
            request.AddUrlSegment("start", start);
            request.AddUrlSegment("limit", limit);

            var response = client.Get<Section>(request);

            return response.Data;
        }
    }
}
