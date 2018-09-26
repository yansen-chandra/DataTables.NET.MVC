using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataTables.MVC.Fluent
{
    public class ShowmorePanel : MvcComponent
    {
        public string ShowMoreText;
        public string ShowLessText;
        public string ButtonCss;
        public string ContentId;
        public string ContentCss;
        //protected bool _expanded;

        public ShowmorePanel()
        {
            ContentId = Guid.NewGuid().ToString();
            ShowLessText = "Show less";
            ShowMoreText = "Show more";
            ButtonCss = "";
            ContentCss = "";
        }

        public ShowmorePanel(HtmlHelper helper, string contentId//, bool expanded = false
            , string showMoreText = "Show more", string showLessText = "Show less", string contentCss = "", string buttonCss = "")
        {
            _Html = helper;
            ContentId = contentId;
            ShowLessText = showLessText;
            ShowMoreText = showMoreText;
            ButtonCss = buttonCss;
            ContentCss = contentCss;
            //_expanded = expanded;
            WriteContext();
        }

        public ShowmorePanel Begin(HtmlHelper html, string contentId
            , string showMoreText = "Show more", string showLessText = "Show less", string contentCss = "", string buttonCss = "")
        {
            return new ShowmorePanel(html, contentId, showMoreText, showLessText, contentCss, buttonCss);
        }

        public override void WriteContext()
        {
            _Html.ViewContext.Writer.Write(string.Format(
                "<div id=\"{0}\" role=\"contentinfo\" class=\"{1}\">", ContentId, ContentCss
            ));
        }

        public override void Dispose()
        {
            _Html.ViewContext.Writer.Write("</div>");

            /*<a data-toggle="showmore" data-target="#contentmore" data-less-text="Show less" data-more-text="Show more" ></a>*/
            //TagBuilder expandButton = new TagBuilder("a");
            //expandButton.AddCssClass(ButtonCss);
            //expandButton.Attributes.Add("data-toggle", "showmore");
            //expandButton.Attributes.Add("data-target", "#" + ContentId);
            //expandButton.Attributes.Add("data-less-text", ShowLessText);
            //expandButton.Attributes.Add("data-more-text", ShowMoreText);
            //_Html.ViewContext.Writer.Write(expandButton.ToString());
            
            _Html.ViewContext.Writer.Write(_Html.DTC().ShowmoreLink(ContentId, ShowMoreText, ShowLessText, ContentCss, ButtonCss).ToString());
        }

    }
    public static partial class MVCControlExtension
    {
        public static ShowmorePanel ContentId(this ShowmorePanel comp, string contentId)
        {
            comp.ContentId = contentId;
            return comp;
        }

        public static ShowmorePanel ShowMoreText(this ShowmorePanel comp, string text)
        {
            comp.ShowMoreText = text;
            return comp;
        }

        public static ShowmorePanel ShowLessText(this ShowmorePanel comp, string text)
        {
            comp.ShowLessText = text;
            return comp;
        }

        public static ShowmorePanel ButtonCss(this ShowmorePanel comp, string text)
        {
            comp.ButtonCss = text;
            return comp;
        }
        
        public static ShowmorePanel ContentCss(this ShowmorePanel comp, string text)
        {
            comp.ContentCss = text;
            return comp;
        }
    }

}
