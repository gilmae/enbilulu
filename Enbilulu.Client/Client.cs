using System;
using System.Collections.Generic;
using RestSharp;


namespace Enbilulu;

using Models;

public class Client
{
    public Uri Endpoint { get; set; }

    public Client()
    {
    }

    public Client(string host, int port = 16016)
    {
        Endpoint = new Uri($"{host}:{port}");
    }

    public Stream CreateStream(string stream)
    {
        var client = new RestClient(Endpoint);

        var request = new RestRequest("/streams/{stream}", Method.Post);
        request.AddUrlSegment("stream", stream);

        var response = client.Post<Stream>(request);

        return response;
    }

    public Stream PutRecord(string stream, dynamic data)
    {
        var client = new RestClient(Endpoint);

        var request = new RestRequest("/streams/{stream}/point", Method.Post);
        request.AddUrlSegment("stream", stream);
        request.AddJsonBody<object>(data as object);

        var response = client.Post<Stream>(request);

        return response;
    }

    public Stream GetStream(string stream)
    {
        var client = new RestClient(Endpoint);

        var request = new RestRequest("/streams/{stream}", Method.Get);
        request.AddUrlSegment("stream", stream);

        var response = client.Get<Stream>(request);

        return response;
        
    }

    public Section GetRecordsFrom(string stream, int start, int limit)
    {
        var client = new RestClient(Endpoint);

        var request = new RestRequest("/streams/{stream}/{start}/{limit}", Method.Get);
        request.AddUrlSegment("stream", stream);
        request.AddUrlSegment("start", start);
        request.AddUrlSegment("limit", limit);

        var response = client.Get<Section>(request);

        return response;
    }

    public IList<string> ListStreams()
    {
        var client = new RestClient(Endpoint);

        var request = new RestRequest("/streams", Method.Get);

        var response = client.Get<List<string>>(request);

        return response;
    }

}
