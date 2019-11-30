using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace SearchJSFloat.Lib
{
    /// <summary>
    /// 输入的字符窜必须是存表达式
    /// </summary>
    public class ExpParser
    {
        public List<string> ListExp { get; private set; }
        public string Remain { get; private set; }
        private bool _blnCheckBracket = false;
        private string _FixLine = string.Empty;

        public ExpParser(string strExp)
        {
            this.ListExp = new List<string>();
            this.Remain = string.Empty;

            Parse(strExp);
        }

        public ExpParser(string strExp, bool blnCheckBracket, string strFixLine)
        {
            this.ListExp = new List<string>();
            this.Remain = string.Empty;
            _blnCheckBracket = blnCheckBracket;
            _FixLine = strFixLine;

            Parse(strExp);
        }

        private void Parse(string strInput)
        {
            string strRemain = strInput.Trim();
            while (true)
            {

                if (strRemain.Length < 1)
                {
                    break;
                }
                if (_blnCheckBracket)
                {
                    if (strRemain.StartsWith("("))
                    {
                        this.ListExp.Add(strRemain[0].ToString());
                        strRemain = strRemain.TrimStart(strRemain[0]).Trim();
                        if (strRemain.Length < 1)
                        {
                            break;
                        }
                    }
                }
                switch (strRemain[0])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        this.ListExp.Add(strRemain[0].ToString());
                        strRemain = strRemain.TrimStart(strRemain[0]).Trim();
                        break;
                }

                if (_blnCheckBracket)
                {
                    if (strRemain.StartsWith(")"))
                    {
                        this.ListExp.Add(strRemain[0].ToString());
                        strRemain = strRemain.TrimStart(strRemain[0]).Trim();
                        if (strRemain.Length < 1)
                        {
                            break;
                        }
                    }
                }

                string var = JsVarHelper.GetVarExp(strRemain);
                if (!string.IsNullOrEmpty(var))
                {
                    if (HasOperation(var))
                    {
                        bool blnObjectMethodCall = PreCheckMethodCall(var);//判断括号对象后是否还有调用方法
                        bool blnStartBracket = true;
                        if (var[0] != '(')
                        {
                            blnObjectMethodCall = true;
                            blnStartBracket = false;
                        }
                        string strTemp = var.Substring(var.IndexOf("("));
                        int intFirstBracketPos = JsVarHelper.GetBracketEndPos(strTemp,'(');
                        string strChildExp = strTemp;
                        string strPrevTemp = string.Empty;
                        if (intFirstBracketPos > 0)
                        {
                            strTemp = strTemp.Substring(0, intFirstBracketPos + 1); //这里应该是写成循环才是正确的
                            if (!HasOperation(strTemp))
                            {
                                int intTempPos = var.IndexOf(strTemp) + strTemp.Length + 1;
                                strChildExp = var.Substring(intTempPos);
                                strPrevTemp = var.Substring(0, intTempPos);
                            }
                        }                         
                        
                        ExpParser parser = new ExpParser(blnStartBracket ? var : strChildExp, true, blnObjectMethodCall ? var : string.Empty);
                        string strExpValue = strPrevTemp + parser.GetExpValue();
                        if (!string.IsNullOrEmpty(this._FixLine))
                        {
                            this._FixLine = this._FixLine.Replace(var, strExpValue);    //需要修复this._FixLine
                        }
                        this.ListExp.Add(strExpValue);
                    }
                    else
                    {
                        this.ListExp.Add(var);
                    }

                    int intRemainPos = strRemain.IndexOf(var) + var.Length;
                    if (strRemain.Length > intRemainPos)
                    {
                        strRemain = strRemain.Substring(intRemainPos).Trim();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(strRemain))
                    {
                        this.Remain = strRemain;
                    }
                    break;
                }
            }
        }

        private bool HasOperation(string var)
        {
            //系统中的特征判断
            Regex regSub = new Regex(@"\[\s*\w+\s*(\-|\+)\s*\w+");
            bool blnRegFlag = true;
            if (var.IndexOf("/,/") > -1)
            {
                string[] arr = var.Split(new string[] { "/,/" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].IndexOf("/") > -1)
                    {
                        blnRegFlag = true;
                        break;
                    }
                    else
                    {
                        blnRegFlag = false;
                    }
                }
            }
            bool blnNoteFlag = true;
            if (var.IndexOf("//") > -1)
            {
                string[] arr = var.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].IndexOf("/") > -1)
                    {
                        blnNoteFlag = true;
                        break;
                    }
                    else
                    {
                        blnNoteFlag = false;
                    }
                }
            }
            return ((var.IndexOf('+') > -1 || var.IndexOf('-') > -1) && !regSub.IsMatch(var)) || var.IndexOf('*') > -1 || (var.IndexOf('/') > -1 && blnRegFlag && blnNoteFlag);                    
        }

        public string GetExpValue(string strRemain)
        {
            if (!string.IsNullOrEmpty(strRemain))
            {
                return GetExpValue() + strRemain;
            }
            else
            {
                return GetExpValue();
            }
        }
        //返回本类的表达式值
        public string GetExpValue()
        {
            if (this.ListExp.Count == 1)
            {
                return this.ListExp[0];
            }
            return ExpValue(Toexpback(this.ListExp));
        }

        private bool PreCheckMethodCall(string strInput) //对象后还有调用方法如(1-5).toString()
        {
            int intPos = JsVarHelper.GetBracketEndPos(strInput,'(');
            if (strInput.Length > intPos + 1)
            {
                if (strInput[intPos + 1] == '.')
                {
                    return true;
                }
            }
            return false;
        }

        //将List类型的中缀表达式转为List类型的后缀表达式
        private List<string> Toexpback(List<string> listExp)
        {
            List<string> listExpBack = new List<string>();
            Stack stackOpers = new Stack();
            string strOper;
            //遍历List类型的中缀表达式
            foreach (string s in listExp)
            {
                //若为变量则添加到List类型的后缀表达式
                if (IsVar(s))
                {
                    listExpBack.Add(s);
                }
                else
                {
                    switch (s)
                    {
                        //为运算符
                        case "+":
                        case "-":
                        case "*":
                        case "/":
                            while (IsPop(s, stackOpers))
                            {
                                listExpBack.Add(stackOpers.Pop().ToString());
                            }
                            stackOpers.Push(s);
                            break;
                        //为开括号
                        case "(":
                            stackOpers.Push(s);
                            break;
                        //为闭括号
                        case ")":
                            while (stackOpers.Count != 0)
                            {
                                strOper = stackOpers.Pop().ToString();
                                if (strOper != "(")
                                {
                                    listExpBack.Add(strOper);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
            while (stackOpers.Count != 0)
            {
                listExpBack.Add(stackOpers.Pop().ToString());
            }
            return listExpBack;
        }

        private string Fix(string var1, string var2, string strOper)
        {
            return string.Format("calcDoubleFix({0}, {1}, '{2}')", var1, var2, strOper);
        }

        private string ExpValue(List<string> expback)
        {
            string var1 = string.Empty, var2 = string.Empty, strResult = string.Empty;
            Stack<string> stackExp = new Stack<string>();
            foreach (string strExp in expback)
            {
                if (IsVar(strExp))
                {
                    stackExp.Push(strExp);
                }
                else
                {
                    var2 = stackExp.Pop();
                    if (stackExp.Count > 0)
                    {
                        var1 = stackExp.Pop();
                        strResult = Fix(var1, var2, strExp);
                    }
                    else
                    {
                        strResult = strExp + var2;
                    }                    

                    _FixLine = Regex.Replace(_FixLine, GetRegEscString(var1) + @"\s*" + GetRegEscString(strExp) + @"\s*" + GetRegEscString(var2), strResult);

                    stackExp.Push(strResult);
                }
            }
            if (!string.IsNullOrEmpty(_FixLine))
            {
                return _FixLine;
            }
            return strResult;
        }

        private string GetRegEscString(string str)
        {
            return str.Replace("$", @"\$").Replace("(", @"\(").Replace(")", @"\)").Replace("*", @"\*").Replace("+", @"\+").Replace(".", @"\.").Replace("?", @"\?").Replace(@"/", @"\/").Replace(@"^", @"\^").Replace(@"|", @"\|").Replace(@"{", @"\{").Replace(@"}", @"\}").Replace(@"[", @"\[").Replace(@"]", @"\]").Replace("\"", @"\""");
        }

        //返回运算符的优先级
        private int GetPriority(string strOper)
        {
            switch (strOper)
            {
                case "*":
                    return 3;
                case "/":
                    return 4;
                case "+":
                    return 1;
                case "-":
                    return 2;
                default:
                    return 0;
            }
        }

        private bool IsPop(string strOper, Stack stackOper)
        {
            if (stackOper.Count == 0)
            {
                return false;
            }
            else
            {
                if (stackOper.Peek().ToString() == "(" || GetPriority(strOper) > GetPriority(stackOper.Peek().ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool IsVar(string str)
        {
            if (str == "+" || str == "-" || str == "*" || str == "/" || str == "(" || str == ")")
            {
                return false;
            }
            return true;
        }
    }
}
