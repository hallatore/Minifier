using System;
using System.Diagnostics;
using System.IO;

namespace Lervik.Minifier.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("dagbladet.no");
            RunTest("vg.no");
            RunTest("aftenposten.no");
            RunTest("nbc.com");
            RunTest("Microsoft.com");

            Console.ReadLine();
        }

        private static void RunTest(string filename)
        {
            var fr = File.OpenText(filename + ".htm");
            var html = fr.ReadToEnd();

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var minifiedContent = Core.Minify.Complete(html);
            stopwatch.Stop();

            Console.WriteLine("--- " + filename + " ----------------------------------");
            Console.WriteLine(string.Format("Elapsed: \t{0} ms", stopwatch.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Removed: \t{0:00.0} %   ({1:00.0} KB)", 100 - (100f / html.Length) * minifiedContent.Length, (html.Length - minifiedContent.Length) / 1000f));
            Console.WriteLine();
            Console.WriteLine(string.Format("Original: \t{0:00.0} KB", html.Length / 1000f));
            Console.WriteLine(string.Format("Minified: \t{0:00.0} KB", minifiedContent.Length / 1000f));
            Console.WriteLine();
            Console.WriteLine();

            File.WriteAllText("Result" + filename + ".htm", minifiedContent);
        }
    }
}
