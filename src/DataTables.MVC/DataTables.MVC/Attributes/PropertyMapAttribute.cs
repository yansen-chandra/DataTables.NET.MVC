using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Attributes
{
    /// <summary>
    /// Property attribute used to determine destination attribute to be mapped at entity conversion 
    /// Used by EntityDataHelper.Copy function
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PropertyMapAttribute : Attribute
    {
        protected string _TargetPropertyName;
        public virtual string TargetPropertyName
        {
            get { return _TargetPropertyName; }
        }

        protected Type _TargetPropertyType;
        public virtual Type TargetPropertyType
        {
            get { return _TargetPropertyType; }
        }

        /// <summary>
        /// Property attribute used to determine destination attribute to be mapped at entity conversion 
        /// Used by EntityDataHelper.Copy function
        /// </summary>
        /// <param name="targetPropertyType">A target class type to be mapped</param>
        /// <param name="targetPropertyName">A target property name to be mapped</param>
        public PropertyMapAttribute(Type targetPropertyType, string targetPropertyName)
        {
            this._TargetPropertyType = targetPropertyType;
            this._TargetPropertyName = targetPropertyName;
        }
    }
}
