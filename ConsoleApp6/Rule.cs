using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp6
{
    public class Rule
    {
        public string MemberName
        {
            get;
            set;
        }

        public string Operator
        {
            get;
            set;
        }

        public string TargetValue
        {
            get;
            set;
        }

        public string ValueType
        {
            get;
            set;
        }

        public Rule(string MemberName, string Operator, string TargetValue, string ValueType)
        {
            this.MemberName = MemberName;
            this.Operator = Operator;
            this.TargetValue = TargetValue;
            this.ValueType = ValueType;
        }
    }
}
