using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Attributes
{
    public class PropertyMapDatetimeAttribute : Attribute
    {
        protected string _Format;
        public virtual string Format
        {
            get { return _Format; }
        }

        //public PropertyMapDatetimeAttribute(Type targetPropertyType, string targetPropertyName
        //    , string format)
        //    : base(targetPropertyType, targetPropertyName)
        //{
        //    this._Format = format;
        //}

        public PropertyMapDatetimeAttribute(string format)
        {
            this._Format = format;
        }

    }
}
