using System;
using System.Web.Mvc;

namespace DataTables.MVC.Fluent
{
    public interface IMvcComponent : IDisposable
    {
        HtmlHelper Html { get; }
        void SetHelper(HtmlHelper helper);
        void WriteContext();
    }
}
