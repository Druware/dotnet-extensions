using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Druware.Extensions
{
    public static class HtmlStringExtension
    {
        public static String NormalizeHtml(this string inputData)
        {
            string result = inputData;

            // remove formatting html
            result = result.Replace("\n", "");
            result = result.Replace("\t", "");

            // line breaks
            result = result.Replace("<br>", "\n");
            result = result.Replace("<br/>", "\n");
            result = result.Replace("<br />", "\n");
            result = result.Replace("<BR>", "\n");
            result = result.Replace("<BR/>", "\n");
            result = result.Replace("<BR />", "\n");

            // bold tags
            result = result.Replace("<b>", "");
            result = result.Replace("</b>", "");
            result = result.Replace("<B>", "");
            result = result.Replace("</B>", "");

            // italics
            result = result.Replace("<i>", "");
            result = result.Replace("</i>", "");
            result = result.Replace("<I>", "");
            result = result.Replace("</I>", "");

            // deal with complex tags
            if (result.Contains("<font "))
            {
                result = result.Substring(0, result.IndexOf("<font")) +
                    result.Substring(1 + result.IndexOf(">", result.IndexOf("<font")));
            }
            result = result.Replace("</font>", "");
            if (result.Contains("<FONT "))
            {
                result = result.Substring(0, result.IndexOf("<FONT")) +
                    result.Substring(1 + result.IndexOf(">", result.IndexOf("<FONT")));
            }
            result = result.Replace("</FONT>", "");

            // replace various Html Entities

            result = result.Replace("&nbsp;", " ");
            result = result.Replace("&amp;", "&");
            result = result.Replace("&035;", "#");

            return result;
        }
    }

    public static class SqlStringExtension
    {
        public static String SqlSafeString(this string inputData)
        {
            String result = inputData;

            // Quote Replacement
            result = result.Replace("'", "''");

            return result.Trim();
        }
    }

}
