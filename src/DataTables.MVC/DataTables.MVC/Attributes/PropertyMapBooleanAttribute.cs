using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Attributes
{
    public class PropertyMapBooleanAttribute : Attribute
    {
        protected string _TrueValue;
        public virtual string TrueValue
        {
            get { return _TrueValue; }
        }

        protected string _FalseValue;
        public virtual string FalseValue
        {
            get { return _FalseValue; }
        }

        //public PropertyMapBooleanAttribute(Type targetPropertyType, string targetPropertyName
        //    , string trueValue, string falseValue)
        //    : base(targetPropertyType, targetPropertyName)
        //{
        //    this._FalseValue = falseValue;
        //    this._TrueValue = trueValue;
        //}

        public PropertyMapBooleanAttribute(string trueValue, string falseValue)
        {
            this._FalseValue = falseValue;
            this._TrueValue = trueValue;
        }

    }
}
