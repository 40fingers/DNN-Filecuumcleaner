using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Common.Utilities;

namespace FortyFingers.FilecuumCleaner.Library
{
    /// <summary>
    /// Utility class containing several commonly used procedures by 40FINGERS
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Set scriptTimeout in seconds, for possibly lengthy operations
        /// </summary>
        /// <param name="timeout"></param>
        public static void SetScriptTimeout(int timeout)
        {
            HttpContext.Current.Server.ScriptTimeout = timeout;
        }


        /// <summary>
        /// Concatenates the values in a List Of Strings, sepratated by the supplied separator.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator">Any string</param>
        /// <returns></returns>
        public static string ToString(this List<string> list, string separator)
        {
            var retval = "";
            if (list != null && list.Count > 0)
            {
                foreach (var value in list)
                {
                    if (retval.Length > 0) retval += separator;
                    retval += value;
                }
            }

            return retval;
        }

        #region Controls Stuff

        public static int TextAsInt(this TextBox txt)
        {
            int i = Null.NullInteger;
            int.TryParse(txt.Text.Trim(), out i);
            return i;
        }

        public static void SelectValue(this DropDownList ddl, string value)
        {
            if (ddl.Items == null || ddl.Items.Count == 0) return;

            var item = ddl.Items.FindByValue(value);

            if (item != null)
                ddl.SelectedIndex = ddl.Items.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ddl"></param>
        /// <returns>Return the selected value as Integer. Returns Null.NullInteger if it can't be parsed into an Integer.</returns>
        public static int SelectedValueInt(this DropDownList ddl)
        {
            var sel = ddl.SelectedValue;
            int retval;

            if(!int.TryParse(sel, out retval))
                retval = Null.NullInteger;

            return retval;
        }

        #endregion

        #region XML Stuff

        public static string InnerText(this XmlElement elm, string xpath)
        {
            var innerText = "";
            var node = elm.SelectSingleNode(xpath);

            if(node != null)
            {
                innerText = node.InnerText;
            }

            return innerText;
        }

        public static int InnerTextInt(this XmlElement elm, string xpath)
        {
            int retval=Null.NullInteger;
            var innerText = "";
            var node = elm.SelectSingleNode(xpath);

            if(node != null)
            {
                innerText = node.InnerText;
            }

            int.TryParse(innerText, out retval);

            return retval;
        }

        public static Guid InnerTextGuid(this XmlElement elm, string xpath)
        {
            Guid retval=Guid.Empty;
            var innerText = "";
            var node = elm.SelectSingleNode(xpath);

            if(node != null)
            {
                innerText = node.InnerText;
            }

            try
            {
                retval = new Guid(innerText);
            }
            catch
            {
                // do nothing
            }

            return retval;
        }

        public static XmlNode SelectSingleNodeOrCreate(this XmlDocument xmlDoc, string xpath)
        {
            string currentXPath = xpath;
            // see if the element exists
            var parentNode = xmlDoc.SelectSingleNodeOrCreate(xpath.Substring(0, xpath.LastIndexOf('/')));
            var node = xmlDoc.SelectSingleNode(xpath);
            if (node == null)
            {
                // create the element if needed
                node = xmlDoc.CreateElement(xpath.Substring(xpath.LastIndexOf('/') + 1));
                parentNode.AppendChild(node);
            }
            return node;
        }

        #endregion

        //        Private Function GetNodeValueSafely(pXPath As String) As String
        //    Dim retval As String = ""
        //    Dim _node As XmlNode = _doc.SelectSingleNode(pXPath)

        //    If Not _node Is Nothing Then
        //        retval = _node.InnerText
        //    End If

        //    Return retval
        //End Function

    }
}