using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Attributes
{
    public sealed class DisplayOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public DisplayOrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}
