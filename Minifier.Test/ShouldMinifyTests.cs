using Lervik.Minifier.Core;
using NUnit.Framework;

namespace Lervik.Minifier.Test
{
    [TestFixture]
    public class ShouldMinifyTests
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual("<h2>Sorry, an error occurred while processing your request.</h2>", 
                Minify.Complete("\r\n<h2>\r\n    Sorry, an error occurred while processing your request.\r\n</h2>"));
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual("</h2><p>To learn more about ASP.NET MVC visit <a href=\"http://asp.net/mvc\" title=\"ASP.NET MVC Website\">http://asp.net/mvc</a>.</p>", 
                Minify.Complete("</h2>\r\n<p>\r\n    To learn more about ASP.NET MVC visit <a href=\"http://asp.net/mvc\" title=\"ASP.NET MVC Website\">http://asp.net/mvc</a>.\r\n</p>\r\n"));
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual("<h2>", 
                Minify.Complete("\r\n<h2>"));
        }

        [Test]
        public void Test4()
        {
            Assert.AreEqual("<h2>About</h2><p>Put content here.</p>", 
                Minify.Complete("\r\n<h2>About</h2>\r\n<p>\r\n     Put content here.\r\n</p>\r\n"));
        }

        [Test]
        public void Test5()
        {
            Assert.AreEqual("Welcome <strong>", 
                Minify.Complete("Welcome <strong>"));
        }

        [Test]
        public void Test6()
        {
            Assert.AreEqual("</strong>![ ", 
                Minify.Complete("</strong>!\r\n    [ "));
        }

        [Test]
        public void Test7()
        {
            Assert.AreEqual("<!DOCTYPE html><html><head> <meta charset=utf-8 /><title>", 
                Minify.Complete("<!DOCTYPE html>\r\n<html>\r\n<head> \r\n    <meta charset=\"utf-8\" />\r\n    <title>"));
        }

        [Test]
        public void Test8()
        {
            Assert.AreEqual("</title><link href=\"", 
                Minify.Complete("</title>\r\n    <link href=\""));
        }

        [Test]
        public void Test9()
        {
            Assert.AreEqual("<style rel=stylesheet type=\"text/css\"/>\r\n    <script src=\"", 
                Minify.Complete("<style rel=\"stylesheet\" type=\"text/css\" />\r\n    <script src=\""));
        }

        [Test]
        public void Test10()
        {
            Assert.AreEqual("\" type=\"text/javascript\"></script></head><body><div class=page><header> <div id=title><h1>My MVC Application</h1></div><div id=logindisplay>", 
                Minify.Complete("\" type=\"text/javascript\"></script>\r\n</head>\r\n<body>\r\n    <div class=\"page\">\r\n        <header>     \r\n            <div id=\"title\">\r\n                <h1>My MVC Application</h1>\r\n            </div>\r\n            <div id=\"logindisplay\">\r\n                "));
        }

        [Test]
        public void Test11()
        {
            Assert.AreEqual("</div><nav><ul id=menu><li>", 
                Minify.Complete("\r\n            </div>\r\n            <nav>\r\n                <ul id=\"menu\">\r\n                    <li>"));
        }

        [Test]
        public void Test12()
        {
            Assert.AreEqual("</li><li>", 
                Minify.Complete("</li>\r\n                    <li>"));
        }

        [Test]
        public void Test13()
        {
            Assert.AreEqual("</section><footer></footer></div></body></html>", 
                Minify.Complete("\r\n        </section>\r\n        <footer>\r\n        </footer>\r\n    </div>\r\n</body>\r\n</html>"));
        }

        [Test]
        public void Test14()
        {
            Assert.AreEqual("</section><footer></footer><pre tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</pre></div></body></html>", 
                Minify.Complete("\r\n        </section>\r\n        <footer>\r\n        </footer>\r\n    <pre tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</pre></div>\r\n</body>\r\n</html>"));
        }

        [Test]
        public void Test15()
        {
            Assert.AreEqual("</section><footer></footer><textarea tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</textarea></div></body></html>", 
                Minify.Complete("\r\n        </section>\r\n        <footer>\r\n        </footer>\r\n    <textarea tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</textarea></div>\r\n</body>\r\n</html>"));
        }

        [Test]
        public void Test16()
        {
            Assert.AreEqual("<div class=test>hmm</div><script type=\"test\">var x=y</script><div class=test>hmm</div><script type=\"test\">var x=y</script><div class=test>hmm</div><script type=\"test\">var x=y</script>", 
                Minify.Complete("<div class=\"test\">hmm</div>\r\n   <script type=\"test\">var x = y;</script>\r\n    <div class=\"test\">hmm</div>\r\n   <script type=\"test\">var x = y;</script>\r\n    <div class=\"test\">hmm</div>\r\n   <script type=\"test\">var x = y;</script>"));
        }

        [Test]
        public void Test17()
        {
            Assert.AreEqual("<div class=test>hmm</div><script type=\"test\">var x=y,postComment=function(){$(\"#comment-form\").validate();$(\"#comment-form\").valid()}", 
                Minify.Complete("<div class=test>hmm</div><script type=\"test\">var x = y;\r\n   var postComment = function () {\r\n$(\"#comment-form\").validate();\r\n\r\nif ($(\"#comment-form\").valid()) {\r\n}\r\n  };"));
        }

        [Test]
        public void Test18()
        {
            Assert.AreEqual("x=y /> hasdmamsd </div>", 
                Minify.Complete("x=\"y\" /> hasdmamsd </div>"));
        }
    }
}
