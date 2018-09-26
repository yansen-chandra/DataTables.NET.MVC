using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataTables.MVC.Fluent
{
    public class CollapsiblePanel : MvcComponent
    {
        public string ContentId { get; set; }
        public string Title { get; set; }
        public bool Expanded { get; set; }
        public string PanelClass { get; set; }
        public bool DisableCollapse { get; set; }

        public CollapsiblePanel()
        {
            this.ContentId = Guid.NewGuid().ToString();
            this.Title = "Title";
            this.Expanded = false;
             this.PanelClass = "panel-primary";
        }

        public CollapsiblePanel(HtmlHelper html, string contentId, string title, bool expanded = false)
        {
            _Html = html;
            this.ContentId = contentId;
            this.Title = title;
            this.Expanded = expanded;
            WriteContext();
        }

        public CollapsiblePanel Begin(HtmlHelper html, string contentId, string title, bool expanded = false)
        {
            return new CollapsiblePanel(html, contentId, title, expanded);
        }

        public override void WriteContext()
        {
            string header = DisableCollapse ?  
                    string.Format("<span>{0}</span>", Title)
                : string.Format("<a data-toggle='collapse' data-parent='#accordion' href='#{0}' class='collapse-toggle {2}'>{1}</a>"
                , ContentId, Title, Expanded ? "" : "collapsed");

            _Html.ViewContext.Writer.Write(string.Format(
                @"
<div class='panel {3}'>
    <div class='panel-heading'>
        <h4 class='panel-title'>
            {1}    
        </h4>
    </div>
    <div id='{0}' class='panel-collapse collapse {2}'>
        <div class='panel-body'>
            "
                , ContentId, header, Expanded ? "in" : "", PanelClass)
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
        public static CollapsiblePanel ContentId(this CollapsiblePanel comp, string contentId)
        {
            comp.ContentId = contentId;
            return comp;
        }

        public static CollapsiblePanel Title(this CollapsiblePanel comp, string title)
        {
            comp.Title = title;
            return comp;
        }

        public static CollapsiblePanel Expanded(this CollapsiblePanel comp, bool expanded)
        {
            comp.Expanded = expanded;
            return comp;
        }

        public static CollapsiblePanel PanelClass(this CollapsiblePanel comp, string panelClass)
        {
            comp.PanelClass = panelClass;
            return comp;
        }

        public static CollapsiblePanel DisableCollapse(this CollapsiblePanel comp, bool disableCollapse)
        {
            comp.DisableCollapse = disableCollapse;
            return comp;
        }


    }

}
