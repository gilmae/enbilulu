using System;
using CommandLine;

namespace Enbilulu
{
    //public abstract class BaseOptions
    //{
    //    [Option('w', "working-folder", Required=false, Default = "")]
    //    public string WorkingDir { get; set; }

    //    [Option('s', "stream", Required=true, Default = "")]
    //    public string Stream { get; set; }

    //    [Option('v', "verbose", Required=false, Default = false)]
    //    public bool Verbose { get; set; }

    //}

    //[Verb("Create")]
    //public class CreateStreamOptions : BaseOptions
    //{
        
    //}

    //[Verb("Get")]
    //public class GetStreamOptions : BaseOptions
    //{

    //}

    //[Verb("Put")]
    //public class PutRecordOptions : BaseOptions
    //{
    //    [Option('d', "data", Required = false, Default = "")]
    //    public string Data { get; set; }

    //    [Option('f', "file", Required = false, Default = "")]
    //    public string DataFile { get; set; }
    //}

    public class Options
    {
        [Option('p', "port", Required =false, Default = 367)]
        public int Port { get; set; }

        [Option('h', "host", Required = false, Default = "localhost")]
        public string Host { get; set; }


        [Option('d', "data-folder", Required = false, Default = "")]
        public string DataFolder { get; set; }

    }
}
