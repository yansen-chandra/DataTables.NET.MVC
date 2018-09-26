using System.Web.Mvc;

namespace DataTables.MVC.Fluent
{
    public abstract class MvcComponent : IMvcComponent
    {
        protected HtmlHelper _Html;
        public HtmlHelper Html
        {
            get { return _Html; }
        }

        public void SetHelper(HtmlHelper html)
        {
            _Html = html;
        }

        public abstract void WriteContext();

        public abstract void Dispose();
    }

}
