using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core
{
    public static class Minify
    {
        private static readonly Regex RegexRemoveWhitespace = new Regex(">[\r\n][ \r\n\t]*<", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexRemoveWhitespace2 = new Regex(">[ \r\n\t]+<", RegexOptions.Multiline | RegexOptions.Compiled);

        public static string Complete(string html)
        {
            return MinifyComplete(html);
        }

        public static string Quick(string html)
        {
            html = RegexRemoveWhitespace.Replace(html, "><");
            html = RegexRemoveWhitespace2.Replace(html, "> <");
            return html;
        }

        private static readonly Regex RegexRemoveEndAttritbuteWhitespace = new Regex("\" />", RegexOptions.Compiled);
        private static readonly Regex RegexRemoveWhitespaces = new Regex("[ \r\n\t]+$", RegexOptions.Multiline | RegexOptions.Compiled);

        private static string MinifyComplete(string html)
        {
            // Change <a hmm="x" /> down to <a hmm="x"/>
            html = RegexRemoveEndAttritbuteWhitespace.Replace(html, "\"/>");

            var sb = new StringBuilder();
            bool done = false;
            int lastEnd = 0;

            while (!done)
            {
                var index = html.IndexOf("<script", lastEnd, StringComparison.InvariantCultureIgnoreCase);

                if (index != -1)
                    index = html.IndexOf(">", index, StringComparison.InvariantCultureIgnoreCase);


                // check if we have a script-end 
                if (index == -1)
                {
                    var endIndex = html.IndexOf("</script>", lastEnd, StringComparison.InvariantCultureIgnoreCase);

                    if (endIndex != -1)
                    {

                        sb.Append(html.Substring(lastEnd, endIndex - lastEnd));
                        if (lastEnd == endIndex) break;
                        lastEnd = endIndex;
                    }
                    else
                    {
                        done = true;
                        break;
                    }
                    continue;
                }

                // Append whatever before script-tag
                var x = html.Substring(lastEnd, index - lastEnd);
                //x = RegexRemoveWhitespaces.Replace(x, string.Empty);
                GetMinifiedHtml(sb, x);

                lastEnd = html.IndexOf("</script>", index, StringComparison.InvariantCultureIgnoreCase);

                if (lastEnd == -1)
                {
                    // Append rest of script and return
                    // TODO Minify javascript
                    sb.Append(MinifyJs(html.Substring(index, html.Length - index)));
                    return sb.ToString();
                }

                // Append rest of script
                // TODO Minify javascript
                sb.Append(MinifyJs(html.Substring(index, lastEnd - index)));
            }

            GetMinifiedHtml(sb, html.Substring(lastEnd, html.Length - lastEnd));

            return sb.ToString();
        }

        private static string MinifyJs(string p)
        {
            var index = p.IndexOf(">") + 1;
            var scriptTags = p.Substring(0, index);
            var minifier = new JsMinify();
            var bundle = new BundleResponse
            {
                Content = p.Substring(index, p.Length - index)
            };
            minifier.Process(bundle);
            return bundle.Content.StartsWith("/* Minification failed.") ? p : ">" + bundle.Content;
        }

        private static void GetMinifiedHtml(StringBuilder sb, string html)
        {
            if (string.IsNullOrEmpty(html) || !ContainsHtmlTags(html))
            {
                sb.Append(html);
                return;
            }

            var done = false;
            var offset = 0;

            do
            {
                var nextOffset = html.ClosestIndexOf(offset, "<pre", "<textarea", "<style");
                if (nextOffset != -1)
                    nextOffset = html.IndexOf(">", nextOffset, StringComparison.InvariantCultureIgnoreCase);

                var cutOffOffset = html.ClosestIndexOf(offset, "</pre>", "</textarea>", "</style>");

                if (cutOffOffset != -1 && (nextOffset > cutOffOffset || (nextOffset == -1 && cutOffOffset != offset )) && cutOffOffset != offset)
                {
                    // skipping until end of invalid tag
                    sb.Append(html.Substring(offset, cutOffOffset - offset));
                    Debug.WriteLine("Skipping1: " + html.Substring(offset, cutOffOffset - offset));
                    offset = cutOffOffset;
                }
                else if (nextOffset > offset)    
                {
                    // html before invalid tags
                    sb.Append(MinifyPatch(html.Substring(offset, nextOffset - offset)));
                    offset = nextOffset;

                    var endOffset = html.IndexOf("<", nextOffset + 1);

                    if (endOffset == -1)
                    {
                        sb.Append(html.Substring(offset, html.Length - offset));
                        Debug.WriteLine("Skipping2: " + html.Substring(offset, html.Length - offset));
                        done = true;
                    }
                    else
                    {
                        sb.Append(html.Substring(offset, endOffset - offset));
                        Debug.WriteLine("Skipping3: " + html.Substring(offset, endOffset - offset));
                        offset = endOffset;
                    }
                }
                else if (nextOffset == -1)       
                {
                    // no invalid tags
                    sb.Append(MinifyPatch(html.Substring(offset, html.Length - offset)));
                    done = true;
                }
                else                        
                {
                    // skipping invalid tag
                    var endOffset = html.IndexOf("<", nextOffset + 1);

                    if (endOffset == -1)
                    {
                        sb.Append(html.Substring(offset, html.Length - offset));
                        Debug.WriteLine("Skipping2: " + html.Substring(offset, html.Length - offset));
                        done = true;
                    }
                    else
                    {
                        sb.Append(html.Substring(offset, endOffset - offset));
                        Debug.WriteLine("Skipping3: " + html.Substring(offset, endOffset - offset));
                        offset = endOffset;
                    }
                }
            } while (!done);
        }

        private static readonly Regex WhitespaceThatStartWithReturn = new Regex("\r[ \r\n\t]*", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex CommentsThatAreNotIEScriptTags = new Regex(@"<!--(?!\[)[\w\W]{1}((.|\n)+?)-->", RegexOptions.Compiled);
        private static readonly Regex MultipleSpaces = new Regex("[ ]+", RegexOptions.Compiled);
        private static readonly Regex BrTag = new Regex("<br/>", RegexOptions.Compiled);
        private static readonly Regex TagAttributes = new Regex("([a-zA-Z0-]+=)\"([a-zA-Z0-9-_.:]+)\"([ >])", RegexOptions.Compiled);
        private static readonly Regex TagAttributesWithEnding = new Regex("([a-zA-Z-]+=)\"([a-zA-Z0-9-_.:]+)\"/>", RegexOptions.Compiled);

        private static string MinifyPatch(string patch)
        {
            Debug.WriteLine("Patching: " + patch);
            patch = WhitespaceThatStartWithReturn.Replace(patch, string.Empty);
            patch = CommentsThatAreNotIEScriptTags.Replace(patch, string.Empty);
            patch = MultipleSpaces.Replace(patch, " ");
            patch = BrTag.Replace(patch, "<br>");
            patch = TagAttributes.Replace(patch, "$1$2$3");
            patch = TagAttributesWithEnding.Replace(patch, "$1$2 />");
            Debug.WriteLine("Patched: " + patch);
            return patch;
        }

        private static int ClosestIndexOf(this string input, int offset, params string[] values)
        {
            var result = int.MaxValue;

            foreach (var item in values)
            {
                var r = input.IndexOf(item, offset, StringComparison.InvariantCultureIgnoreCase);

                if (r != -1 && r < result)
                    result = r;
            }

            if (result != int.MaxValue)
                return result;

            return -1;
        }

        private static bool ContainsHtmlTags(string html)
        {
            return html.Contains("<");
        }

        //public static string Basic(string content)
        //{
        //    return Minifier(content);
        //}

        //private static string Minifier(string html, bool complete = false)
        //{
        //    Debug.WriteLine("Minifying 3");
        //    if (string.IsNullOrEmpty(html))
        //        return string.Empty;

        //    //html = MinifyGeneralHtml(html);

        //    //if (complete)
        //    //{
        //    //    html = MinifyInlineJavaScript(html);
        //    //    html = MinifyInlineCss(html);
        //    //    //html = MinfyHtmlElements(html);
        //    //}

        //    return html;
        //}

        //private static string MinifyGeneralHtml(string html)
        //{
        //    html = Regex.Replace(html, ">[ \r\n\t]+<", "><", RegexOptions.Multiline | RegexOptions.Compiled);
        //    html = Regex.Replace(html, " />", "/>", RegexOptions.Compiled);

        //    // <!--[ \r\n\t]+(?i)\b(?!function|var |{|})((.|\n)+?)-->
        //    html = Regex.Replace(html, @"<!--(?!\[)[\w\W]{1}((.|\n)+?)-->", "", RegexOptions.Compiled);

        //    return html;
        //}

        //private static string MinifyQuick(string html)
        //{
        //    var sb = new StringBuilder();
        //    bool done = false;
        //    int lastEnd = 0;

        //    while (!done)
        //    {
        //        var index = html.IndexOf("<pre", lastEnd, StringComparison.InvariantCultureIgnoreCase);

        //        if (index == -1)
        //        {
        //            done = true;
        //            break;
        //        }

        //        var x = RemoveWhitespace(html.Substring(lastEnd, html.Length - index));
        //        sb.Append(x);

        //        lastEnd = html.IndexOf("</pre>", lastEnd, StringComparison.InvariantCultureIgnoreCase);

        //        if (lastEnd == -1)
        //        {
        //            return html;
        //        }
        //    }

        //    var y = RemoveWhitespace(html.Substring(lastEnd, html.Length - lastEnd));
        //    sb.Append(y);

        //    return sb.ToString();
        //}

        //private static string RemoveWhitespace(string patch)
        //{
        //    patch = Regex.Replace(patch, "\r[ \r\n\t]*", "", RegexOptions.Multiline | RegexOptions.Compiled);
        //    patch = Regex.Replace(patch, "[ ]+", " ", RegexOptions.Compiled);
        //    return patch;
        //}

        //private static string MinfyHtmlElements(string html)
        //{
        //    var matches = Regex.Matches(html, @"<((.|\n)+?)>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    var sb = new StringBuilder();

        //    int lastEnd = 0;

        //    foreach (Match match in matches)
        //    {
        //        if (Regex.IsMatch(match.Groups[1].Value, @"[ \t]+"))
        //        {
        //            var matchText = Regex.Replace(match.Groups[1].Value, @"n[ \t]+", " ", RegexOptions.Multiline | RegexOptions.Compiled);
        //            matchText = string.Format("<{0}>", matchText);

        //            if (match.Groups[0].Index > 0)
        //                sb.Append(html.Substring(lastEnd, match.Groups[0].Index - lastEnd));

        //            sb.Append(matchText);
        //            lastEnd = match.Groups[0].Index + match.Groups[0].Length;
        //        }
        //    }

        //    sb.Append(html.Substring(lastEnd, html.Length - lastEnd));
        //    return sb.ToString();
        //}

        //private static string MinifyInlineCss(string html)
        //{
        //    //var matches = Regex.Matches(html, @"<style((.|\n)+?)<\/style>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //    //int offset = 0;
        //    //foreach (Match match in matches)
        //    //{
        //    //    var matchText = Regex.Replace(match.Groups[1].Value, @";[ \r\n\t]+", ";", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = Regex.Replace(matchText, @"{[ \r\n\t]+", "{", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = Regex.Replace(matchText, @"\n[ \r\n\t]+", "\n", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = Regex.Replace(matchText, @":[ \t]+", ":", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = string.Format("<style{0}</style>", matchText);

        //    //    int startLength = match.Groups[0].Index + match.Groups[0].Length + offset;
        //    //    html = html.Substring(0, match.Groups[0].Index + offset) +
        //    //        matchText +
        //    //        html.Substring(startLength, html.Length - startLength);

        //    //    offset += matchText.Length - match.Groups[0].Value.Length;
        //    //}

        //    //return html;

        //    var matches = Regex.Matches(html, @"<style((.|\n)+?)<\/style>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    var sb = new StringBuilder();

        //    int lastEnd = 0;

        //    foreach (Match match in matches)
        //    {
        //        var matchText = Regex.Replace(match.Groups[1].Value, @";[ \r\n\t]+", ";", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = Regex.Replace(matchText, @"{[ \r\n\t]+", "{", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = Regex.Replace(matchText, @"\n[ \r\n\t]+", "\n", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = Regex.Replace(matchText, @":[ \t]+", ":", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = string.Format("<style{0}</style>", matchText);

        //        if (match.Groups[0].Index > 0)
        //            sb.Append(html.Substring(lastEnd, match.Groups[0].Index - lastEnd));

        //        sb.Append(matchText);
        //        lastEnd = match.Groups[0].Index + match.Groups[0].Length;
        //    }

        //    sb.Append(html.Substring(lastEnd, html.Length - lastEnd));
        //    return sb.ToString();
        //}

        //private static string MinifyInlineJavaScript(string html)
        //{
        //    //var matches = Regex.Matches(html, @"<script((.|\n)+?)<\/script>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //    //int offset = 0;
        //    //foreach (Match match in matches)
        //    //{
        //    //    var matchText = Regex.Replace(match.Groups[1].Value, @";[ \r\n\t]+", ";", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = Regex.Replace(matchText, @"{[ \r\n\t]+", "{", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = Regex.Replace(matchText, @"\n[ \r\n\t]+", "\n", RegexOptions.Multiline | RegexOptions.Compiled);
        //    //    matchText = string.Format("<script{0}</script>", matchText);

        //    //    int startLength = match.Groups[0].Index + match.Groups[0].Length + offset;
        //    //    html = html.Substring(0, match.Groups[0].Index + offset) +
        //    //        matchText +
        //    //        html.Substring(startLength, html.Length - startLength);

        //    //    offset += matchText.Length - match.Groups[0].Value.Length;
        //    //}

        //    //return html;

        //    var matches = Regex.Matches(html, @"<script((.|\n)+?)<\/script>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    var sb = new StringBuilder();

        //    int lastEnd = 0;

        //    foreach (Match match in matches)
        //    {
        //        var matchText = Regex.Replace(match.Groups[1].Value, @";[ \r\n\t]+", ";", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = Regex.Replace(matchText, @"{[ \r\n\t]+", "{", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = Regex.Replace(matchText, @"\n[ \r\n\t]+", "\n", RegexOptions.Multiline | RegexOptions.Compiled);
        //        matchText = string.Format("<script{0}</script>", matchText);

        //        if (match.Groups[0].Index > 0)
        //            sb.Append(html.Substring(lastEnd, match.Groups[0].Index - lastEnd));

        //        sb.Append(matchText);
        //        lastEnd = match.Groups[0].Index + match.Groups[0].Length;
        //    }

        //    sb.Append(html.Substring(lastEnd, html.Length - lastEnd));
        //    return sb.ToString();
        //}
    }
}
