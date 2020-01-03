using System;
using System.Net;
using Newtonsoft.Json;
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
            request.AddParameter("stream", stream);

            var response = client.ExecuteAsPost<Stream>(request, "POST");

            return response.Data;
        }

        public Stream PutRecord(string stream, dynamic data)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/points/{stream}", Method.POST);
            request.AddParameter("stream", stream);
            request.AddJsonBody(data);

            var response = client.ExecuteAsPost<Stream>(request, "POST");

            return response.Data;
        }

        public Stream GetStream(string stream, dynamic data)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/stream/{stream}", Method.GET);
            request.AddParameter("stream", stream);
            request.AddJsonBody(data);

            var response = client.ExecuteAsPost<Stream>(request, "GET");

            return response.Data;
        }

        public Section GetRecordsAfter(string stream, int start, int limit)
        {
            var client = new RestClient(endpoint);

            var request = new RestRequest("/points/{stream}/{start}/{limit}", Method.GET);
            request.AddParameter("stream", stream);
            request.AddParameter("start", start);
            request.AddParameter("limit", limit);

            var response = client.ExecuteAsPost<Section>(request, "GET");

            return response.Data;
        }

    }
}
