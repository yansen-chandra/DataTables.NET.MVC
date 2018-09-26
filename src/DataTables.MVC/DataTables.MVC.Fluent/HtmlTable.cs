using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataTables.MVC.Fluent
{
    public class HtmlTable<TModel> : MvcComponent
        where TModel : class
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TableClass { get; set; }

        public HtmlTable()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Title = "Title";
             this.TableClass = "panel-primary";
        }

        public HtmlTable(HtmlHelper html, string id, string title)
        {
            _Html = html;
            this.Id = id;
            this.Title = title;
            WriteContext();
        }

        public HtmlTable<TModel> Begin(HtmlHelper html, string id, string title)
        {
            return new HtmlTable<TModel>(html, id, title);
        }

        public override void WriteContext()
        {
            _Html.ViewContext.Writer.Write(
                new TableBuilder<TModel>(_Html).ToHtml()
            );
        }

        public override void Dispose()
        {
            _Html.ViewContext.Writer.Write(@"
        </div>
    </div>
</div>");
        }

    }

    public static partial class MVCControlExtension
    {
        public static HtmlTable<TModel> Id<TModel>(this HtmlTable<TModel> comp, string id)
            where TModel : class
        {
            comp.Id = id;
            return comp;
        }


    }

}
