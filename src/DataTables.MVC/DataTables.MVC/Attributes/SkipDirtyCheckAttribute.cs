using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTables.MVC.Attributes
{
    /// <summary>
    /// Property attribute used to skip dirty check at dirty checking 
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SkipDirtyCheckAttribute : Attribute
    {
        public SkipDirtyCheckAttribute()
        {

        }
    }
}
