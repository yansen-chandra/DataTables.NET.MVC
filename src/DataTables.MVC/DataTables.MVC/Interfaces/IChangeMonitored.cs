using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Interfaces
{
    public class ChangedProperty
    {
        public PropertyInfo Property { get; set; }
        public Object BeforeValue { get; set; }
        public Object AfterValue { get; set; }

        public ChangedProperty()
        {

        }

        public ChangedProperty(PropertyInfo prop, Object beforeVal, Object afterVal)
        {
            Property = prop;
            BeforeValue = beforeVal;
            AfterValue = afterVal;
        }
    }
    public interface IChangeMonitored
    {
        bool IsDirty { get; set; }
        List<ChangedProperty> ChangedProperties { get; set; }
    }
}
