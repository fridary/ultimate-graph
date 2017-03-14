using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace conf
{
    public class RegEx
    {
        #region RegEx

        public static string MakeReg(string text, string expression)
        {
            string result = null;
            try
            {
                RegexOptions regexOptions = new RegexOptions();
                regexOptions |= RegexOptions.IgnoreCase;
                regexOptions |= RegexOptions.CultureInvariant;
                regexOptions |= RegexOptions.Multiline;
                regexOptions |= RegexOptions.Singleline;
                //regexOptions |= RegexOptions.IgnorePatternWhitespace;

                Regex regex = new Regex(expression, regexOptions);
                MatchCollection matchCollection = regex.Matches(text);

                for (int i = 0; i < matchCollection.Count; i++)
                    result += matchCollection[i];
            }
            catch (Exception ex)
            {
                ErrorBox.Show("error with name!\r\n" + ex.Message, Application.ProductName, ErrorBoxButtons.OkCancel, ErrorBoxIcon.Error);
            }

            return result;
        }

        #endregion
    }
}
