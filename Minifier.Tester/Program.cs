using System;
using System.Diagnostics;
using System.IO;

namespace Lervik.Minifier.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("Dagbladet");
            RunTest("Microsoft");
            RunTest("SelfEvaluation");
            RunTest("Admin");
            //RunTest("Dagbladet");
            //RunTest("Microsoft");
            //RunTest("SelfEvaluation");
            //Console.WriteLine(Core.Minify.Complete("Please enter your user name and password. <a href=\"/Account/Register\">Register</a> if you don't have an account."));

            Console.ReadLine();
        }

        private static void RunTest(string filename)
        {
            var fr = File.OpenText(filename + ".htm");
            var html = fr.ReadToEnd();

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var minifiedContent = Lervik.Minifier.Core.Minify.Complete(html);
            minifiedContent = Lervik.Minifier.Core.Minify.Quick(minifiedContent);
            stopwatch.Stop();

            Console.WriteLine("--- " + filename + " ----------------------------------");
            Console.WriteLine(string.Format("Elapsed: \t{0} ms", stopwatch.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Removed: \t{0:00.0} %   ({1:00.0} KB)", 100 - (100f / html.Length) * minifiedContent.Length, (html.Length - minifiedContent.Length) / 1000f));
            Console.WriteLine();
            Console.WriteLine(string.Format("Original: \t{0}", html.Length));
            Console.WriteLine(string.Format("Minified: \t{0}", minifiedContent.Length));
            Console.WriteLine();
            Console.WriteLine();

            File.WriteAllText("Result" + filename + ".htm", minifiedContent);
        }
    }
}
