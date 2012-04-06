using Lervik.Minifier.Core;
using NUnit.Framework;

namespace Lervik.Minifier.Test
{
    [TestFixture]
    class ShouldNotMinifyTests
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual("\r\n", 
                Minify.Complete("\r\n"));
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual("<pre tag=\"hmm\"> and more stuff like that asdhasdhadh adasdasd\r\n\r\n asdad \r\n</pr", 
                Minify.Complete("<pre tag=\"hmm\"> and more stuff like that asdhasdhadh adasdasd\r\n\r\n asdad \r\n</pr"));
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual(" ]", 
                Minify.Complete(" ]"));
        }

        [Test]
        public void Test4()
        {
            Assert.AreEqual("\r\n", 
                Minify.Complete("\r\n"));
        }

        [Test]
        public void Test5()
        {
            Assert.AreEqual("<pre tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</pre>", 
                Minify.Complete("<pre tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</pre>"));
        }

        [Test]
        public void Test6()
        {
            Assert.AreEqual("<textarea tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</textarea>", 
                Minify.Complete("<textarea tag=\"hmm\"> and more stuff like that \tasdhasdhadh adasdasd\r\n\r\n asdad \r\n</textarea>"));
        }

        [Test]
        public void Test7()
        {
            Assert.AreEqual("\" rel=stylesheet type=\"text/css\"/><script src=\"", 
                Minify.Complete("\" rel=\"stylesheet\" type=\"text/css\"/>\r\n    <script src=\""));
        }

        [Test]
        public void Test8()
        {
            Assert.AreEqual(",\r\nconvert_urls : false,\r\n\r\n// Drop lists for link/image/media/template dialogs\r\ntemplate_external_list_url : \"lists/template_list.js\",\r\nexternal_link_list_url : \"lists/link_list.js\",\r\nexternal_image_list_url : \"lists/image_list.js\",\r\nmedia_external_list_url : \"lists/media_list.js\"\r\n\r\n});\r\n</script>", 
                Minify.Complete(",\r\nconvert_urls : false,\r\n\r\n// Drop lists for link/image/media/template dialogs\r\ntemplate_external_list_url : \"lists/template_list.js\",\r\nexternal_link_list_url : \"lists/link_list.js\",\r\nexternal_image_list_url : \"lists/image_list.js\",\r\nmedia_external_list_url : \"lists/media_list.js\"\r\n\r\n});\r\n</script>"));
        }

        [Test]
        public void Test9()
        {
            Assert.AreEqual(" /> hasdmamsd </div>", 
                Minify.Complete(" /> hasdmamsd </div>"));
        }
    }
}
