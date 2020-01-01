//using System;
//namespace Enbilulu
//{
//    public class CommandLineHandler
//    {
//        public CommandLineHandler()
//        {
//        }

//        public static int HandlePutRecord(PutRecordOptions options)
//        {

//            return 0;
//        }

//        public static int HandleCreateStream(CreateStreamOptions options)
//        {
//            var db = new Db(options.WorkingDir);
//            db.CreateStream(options.Stream);

//            if (options.Verbose)
//            {
//                Console.Write($"Created stream {options.Stream}");
//            }

//            return 0;
//        }

//        public static string HandleGetStream(GetStreamOptions options)
//        {
//            var db = new Db(options.WorkingDir);
//            var stream = db.GetStream(options.Stream);

//            if (options.Verbose)
//            {
//                Console.WriteLine($"Stream {options.Stream}:");
//                Console.WriteLine($"\t Num Points:\t{stream.Last_Point}");
//                Console.WriteLine($"\t Last Point:\t{stream.Last_Point}");
//            }

//            return 0;
//        }
//    }
//}
