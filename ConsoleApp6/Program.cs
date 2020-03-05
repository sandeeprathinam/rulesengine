using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApp6
{
    class Program
    {
        static void Main(string[] args)
        {
           
            checkthedata();
            Console.WriteLine("Check Completed");
        }

        public static void checkthedata()
        {
            List<SignalDataModel> SignalDataModel = JsonConvert.DeserializeObject<List<SignalDataModel>>(File.ReadAllText(@"C:\Users\Rathinams\Downloads\raw_data.json"));
            List<UserDefinedRules> Rules = JsonConvert.DeserializeObject<List<UserDefinedRules>>(File.ReadAllText(@"C:\Users\Rathinams\Downloads\Rules.json"));
            foreach (var item in SignalDataModel)
            {
                var rulestobeapplied = (from s in Rules where s.SignalType == item.signal select s).ToList();
                ConvertedDM convertedDM = new ConvertedDM();
                if (item.value_type == "String")
                {

                    convertedDM.signal = item.signal;
                    convertedDM.svalue = item.value;
                    convertedDM.value_type = item.value_type;

                }
                else if(item.value_type == "Double")
                {

                    convertedDM.signal = item.signal;
                    convertedDM.dvalue = Convert.ToDouble(item.value);
                    convertedDM.value_type = item.value_type;
                }
                else
                {

                    convertedDM.signal = item.signal;
                    convertedDM.dtvalue = Convert.ToDateTime(item.value);
                    convertedDM.value_type = item.value_type;
                }
                foreach(var item2 in rulestobeapplied)
                {
                    if (item2.TargetValue == "CurrentDateTime")
                    {
                        item2.TargetValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                        Rule rule = new Rule(item2.MemberName, item2.Operator, item2.TargetValue, item.value_type);
                        Func<ConvertedDM, bool> compiledRule = CompileRule<ConvertedDM>(rule);
                        bool result = compiledRule(convertedDM);
                        if(result)
                        {
                            Console.WriteLine("Value Passed");
                        }
                    
                }
            }
        }

        public static Func<T, bool> CompileRule<T>(Rule r)
        {
            var paramUser = Expression.Parameter(typeof(ConvertedDM));
            Expression expr = BuildExpr<T>(r, paramUser);
            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile();
        }

        static Expression BuildExpr<T>(Rule r, ParameterExpression param)
        {
            var left = MemberExpression.Property(param, r.MemberName);
            var tProp = Type.GetType("System." + r.ValueType);
            ExpressionType tBinary;
            if (ExpressionType.TryParse(r.Operator, out tBinary))
            {
                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tProp));
                return Expression.MakeBinary(tBinary, left, right);
            }
            else
            {
                var method = tProp.GetMethod(r.Operator);
                var tParam = method.GetParameters()[0].ParameterType;
                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tParam));
                return Expression.Call(left, method, right);
            }
        }
    }
}
