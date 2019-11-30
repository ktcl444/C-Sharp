using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchJSFloat.Lib
{
    public class JsVarHelper
    {
        public static string GetVarExp(string strInput)
        {
            Regex reg = new Regex(@"^\d+(\.\d+)?");
            Match match;
            string strCurrentMatch = string.Empty;
            match = reg.Match(strInput);
            if (match.Success)
            {
                strCurrentMatch = match.Value;
            }
            else
            {
                reg = new Regex(@"^(?<var>[\$\w]+)");
                if (strInput.StartsWith("("))
                {
                    int intEndPos = GetBracketEndPos(strInput, '(');
                    strCurrentMatch = strInput.Substring(0, intEndPos + 1);

                    if (strInput.Length > intEndPos + 1 && strInput[intEndPos + 1] == '.')
                    {
                        GetVar(reg, ref strCurrentMatch, strInput);
                    }
                }
                else
                {
                    match = reg.Match(strInput);
                    if (match.Success)
                    {
                        strCurrentMatch = match.Value;
                        GetVar(reg, ref strCurrentMatch, strInput);
                    }
                }
            }

            return strCurrentMatch;
        }

        private static void GetVar(Regex reg, ref string strCurrentMatch, string strInput)
        {
            while (true)
            {
                if (string.IsNullOrEmpty(strCurrentMatch))
                {
                    break;
                }
                int pos1 = strInput.IndexOf(strCurrentMatch) + strCurrentMatch.Length;
                string strRemain = strInput.Substring(pos1);
                if (strRemain.Length > 0)
                {
                    switch (strRemain[0])
                    {
                        case '(':
                            int intEndPos = GetBracketEndPos(strRemain, '(');
                            strCurrentMatch += strRemain.Substring(0, intEndPos + 1);
                            break;
                        case '[':
                            intEndPos = GetBracketEndPos(strRemain, '[');
                            strCurrentMatch += strRemain.Substring(0, intEndPos + 1);
                            break;
                        case '.':
                            Match match = reg.Match(strRemain.Substring(1));
                            strCurrentMatch += '.' + match.Groups["var"].Value;
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public static int GetBracketEndPos(string strInput, char chr)
        {
            Stack<char> stack = new Stack<char>();
            int intEndPos = 0;
            for (int i = 0; i < strInput.Length; i++)
            {
                if (strInput[i] == chr)
                {
                    stack.Push(chr);
                }
                else if (strInput[i] == (chr == '(' ? ')' : ']'))
                {
                    if (stack.Count > 0)
                    {
                        stack.Pop();
                        if (stack.Count == 0)
                        {
                            intEndPos = i;
                            break;
                        }
                    }
                }
            }

            return intEndPos;
        }
    }
}
