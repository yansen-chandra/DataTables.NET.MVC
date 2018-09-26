using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using System.Web.Routing;
using System.IO;
using DataTables.MVC.Extensions;


namespace DataTables.MVC.Fluent
{
    public enum TabOrientation
    {
        Horizontal = 0,
        Vertical
    }
    public class ComponentFactory : IDisposable
    {
        private HtmlHelper _Html;
        private IMvcComponent _Component;
        public ComponentFactory(HtmlHelper html)
        {
            this._Html = html;
        }
        public T Begin<T>(T component)
            where T : IMvcComponent
        {
            _Component = component;
            _Component.SetHelper(_Html);
            _Component.WriteContext();
            return component;
        }

        public void Dispose()
        {
            if (_Component != null)
            {
                _Component.Dispose();
            }
        }

        public CollapsiblePanel BeginCollapsiblePanel(string contentId, string title, bool expanded = false)
        {
            return new CollapsiblePanel(_Html, contentId, title, expanded);
        }

        public ShowmorePanel BeginShowmorePanel(string contentId, string showMoreText = "Show more", string showLessText = "Show less", string contentCss = "", string buttonCss = "")
        {
            return new ShowmorePanel(_Html, contentId, showMoreText, showLessText, contentCss, buttonCss);
        }

        public MvcHtmlString Carousel<T>(IEnumerable<T> data, string containerId
            , Func<T, int, HelperResult> itemTemplate
            , int interval = 5, int activeIndex = 0
            , bool showIndicator = false, bool showArrow = false
            )
        {

            /*
<div id="featuredCourses" class="carousel slide" data-ride="carousel">
  <!-- Indicators -->
  <!-- Wrapper for slides -->
  <div class="carousel-inner" role="listbox">
      @for (int i = 0; i < Model.Data.Count(); i++ )
      {
          @RenderItemTemplate(Model.Data.ToList()[i], i == 0)
      }
  </div>
</div>             
             */
            if (data.Count() == 0)
                return new MvcHtmlString("");

            TagBuilder main = new TagBuilder("div");
            main.AddCssClass("carousel slide");
            main.Attributes.Add("id", containerId);
            main.Attributes.Add("data-ride", "carousel");
            main.Attributes.Add("data-interval", (interval * 1000).ToString());

            TagBuilder inner = new TagBuilder("div");
            inner.AddCssClass("carousel-inner");
            inner.Attributes.Add("role", "listbox");

            if (showIndicator)
            {
                TagBuilder indicator = new TagBuilder("ol");
                indicator.AddCssClass("carousel-indicators");
                for (int i = 0; i < data.Count(); i++)
                {
                    indicator.InnerHtml += string.Format("<li data-target=\"#{0}\" data-slide-to=\"{1}\" class=\"{2}\"></li>"
                        , containerId, i, i == activeIndex ? "active" : "");

                }
                main.InnerHtml += indicator.ToString();
            }

            for (int i = 0; i < data.Count(); i++)
            {
                /*
                    string activeClass = active ? "active" : "";
                    <div class="item @activeClass">
                        <div>
                            @Html.Partial("_CourseCard", course, new ViewDataDictionary {{ "Title", title }})
                        </div>
                        <div class="carousel-caption">
                        </div>
                    </div>
                 */
                TagBuilder itemTag = new TagBuilder("div");
                itemTag.AddCssClass("item");
                if (i == activeIndex)
                {
                    itemTag.AddCssClass("active");
                }
                itemTag.InnerHtml = itemTemplate(data.ToList()[i], i).ToString();
                inner.InnerHtml += itemTag.ToString();
            }
            main.InnerHtml += inner.ToString();

            if (showArrow)
            {
                TagBuilder leftArrow = new TagBuilder("a");
                leftArrow.AddCssClass("left carousel-control");
                leftArrow.Attributes.Add("href", "#" + containerId);
                leftArrow.Attributes.Add("role", "button");
                leftArrow.Attributes.Add("data-slide", "prev");
                leftArrow.InnerHtml = @"
                    <span class=""glyphicon glyphicon-chevron-left"" aria-hidden=""true""></span>
                    <span class=""sr-only"">Previous</span>
                ";

                TagBuilder rightArrow = new TagBuilder("a");
                rightArrow.AddCssClass("right carousel-control");
                rightArrow.Attributes.Add("href", "#" + containerId);
                rightArrow.Attributes.Add("role", "button");
                rightArrow.Attributes.Add("data-slide", "next");
                rightArrow.InnerHtml = @"
                    <span class=""glyphicon glyphicon-chevron-right"" aria-hidden=""true""></span>
                    <span class=""sr-only"">Next</span>
                ";

                main.InnerHtml += leftArrow.ToString() + rightArrow.ToString();

            }

            return new MvcHtmlString(main.ToString());

        }

        public MvcHtmlString ShowmoreLink(string contentId
            , string showMoreText = "Show more", string showLessText = "Show less"
            , string contentCss = "", string buttonCss = "")
        {
            TagBuilder expandButton = new TagBuilder("a");
            expandButton.AddCssClass(buttonCss);
            expandButton.Attributes.Add("data-toggle", "showmore");
            expandButton.Attributes.Add("data-target", "#" + contentId);
            expandButton.Attributes.Add("data-less-text", showLessText);
            expandButton.Attributes.Add("data-more-text", showMoreText);
            return new MvcHtmlString(expandButton.ToString());

        }

        public MvcHtmlString Modal(string containerId, string title, string triggerText, Func<HelperResult> content
            , string triggerCss = "btn", string triggerTag = "button", string additionalModalCss = "")
        {
            return Modal(containerId, title, triggerText, content().ToString(), triggerCss, triggerTag, false, additionalModalCss);
        }

        public MvcHtmlString Modal(string containerId, string title
            , string triggerText, string content, string triggerCss = "btn"
            , string triggerTag = "button", bool showCloseButton = false, string additionalModalCss = "")
        {
            TagBuilder trigger = new TagBuilder(triggerTag);
            trigger.AddCssClass(triggerCss);
            trigger.Attributes.Add("data-toggle", "modal");
            trigger.Attributes.Add("data-target", "#" + containerId);
            trigger.Attributes.Add("onclick", "return false;");
            trigger.InnerHtml = triggerText;

            string footer = !showCloseButton ? "" : @"
              <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">Close</button>
              </div>
            ";

            TagBuilder main = new TagBuilder("div");
            main.AddCssClass("modal fade");
            main.Attributes.Add("role", "dialog");
            main.Attributes.Add("id", containerId);
            main.InnerHtml = string.Format(@"
              <div class=""modal-dialog {3}"">
                <!-- Modal content-->
                <div class=""modal-content"">
                  <div class=""modal-header"">
                    <button type=""button"" class=""close"" data-dismiss=""modal"">&times;</button>
                    <h4 class=""modal-title"">{0}</h4>
                  </div>
                  <div class=""modal-body"">
                    {1}
                  </div>
                    {2}
                </div>
              </div>
            ", title, content, footer, additionalModalCss);

            return new MvcHtmlString(trigger.ToString() + main.ToString());

        }

        public MvcHtmlString PopoverLink(string linkText, string title
            , Func<HelperResult> content, string placement = "right", string cssClass = "")
        {
            return PopoverLink(linkText, title, content().ToString(), placement, cssClass);
        }

        public MvcHtmlString PopoverLink(string linkText, string title
            , string content, string placement = "right", string cssClass = "")
        {
            TagBuilder tag = new TagBuilder("a");
            tag.Attributes.Add("href", "javascript:void(0);");
            tag.Attributes.Add("data-toggle", "popover");
            tag.Attributes.Add("title", title);
            tag.Attributes.Add("data-content", content);
            tag.Attributes.Add("data-placement", placement);
            if (!string.IsNullOrEmpty(cssClass))
            {
                tag.AddCssClass(cssClass);
            }
            tag.InnerHtml = linkText;
            return new MvcHtmlString(tag.ToString());
        }

        public MvcHtmlString TooltipIcon(string content, string placement = "bottom"
            , string contentCssClass = "default text-left", string iconClass = "", string iconColorClass = "danger")
        {
            if (string.IsNullOrEmpty(iconClass))
            {
                iconClass = "fa-info-circle";
            }
            string linkText = string.Format("<span class='fa {0} fa-lg fa-fw color-{1}'></span>", iconClass, iconColorClass);
            return TooltipLink(Guid.NewGuid().ToString(), linkText, "#", content, placement, contentCssClass);
        }

        public MvcHtmlString TooltipLink(string id, string linkText, string url
            , Func<HelperResult> content, string placement = "bottom", string cssClass = "")
        {
            return TooltipLink(id, linkText, url, content().ToString(), placement, cssClass);
        }

        public MvcHtmlString TooltipLink(string id, string linkText, string url
            , string content, string placement = "bottom", string cssClass = "")
        {
            TagBuilder tag = new TagBuilder("a");
            tag.Attributes.Add("href", "javascript:void(0);");
            tag.Attributes.Add("data-toggle", "tooltip");
            tag.Attributes.Add("title", content);
            tag.Attributes.Add("data-placement", placement);
            if (!string.IsNullOrEmpty(cssClass))
            {
                tag.AddCssClass(cssClass);
            }
            tag.InnerHtml = linkText;
            return new MvcHtmlString(tag.ToString());
        }

        public MvcHtmlString Crumbs(Dictionary<string, string> items
            , int selected = 0, string cssBrand = "default", string cssClass = "")
        {
            TagBuilder main = new TagBuilder("div");
            main.AddCssClass("btn-group btn-breadcrumb");
            main.AddCssClass(cssBrand);
            main.AddCssClass(cssClass);
            for (int i = 0; i < items.Count; i++)
            {
                var item = items.ElementAt(i);
                TagBuilder linkTag = new TagBuilder("a");
                linkTag.AddCssClass("btn btn-" + cssBrand);
                if (i == selected)
                    linkTag.AddCssClass("current");
                if (!string.IsNullOrWhiteSpace(item.Value))
                    linkTag.Attributes.Add("href", item.Value);
                linkTag.InnerHtml = item.Key;
                main.InnerHtml += linkTag.ToString();
            }
            return new MvcHtmlString(main.ToString());
        }

        public MvcHtmlString Crumbs(List<string> items
            , int selected = 0, string cssBrand = "default", string cssClass = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            items.ForEach(x => dict.Add(x, string.Empty));
            return Crumbs(dict, selected, cssBrand, cssClass);
        }

        public MvcHtmlString Crumbs(List<string> items
            , string selectedString = "", string cssBrand = "default", string cssClass = "")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            int selected = -1;
            items.ForEach(x => { if (!dict.ContainsKey(x)) { dict.Add(x, string.Empty); } });
            for (int i = 0; i < items.Count; i++)
            {
                if (selectedString == items[i])
                {
                    selected = i;
                    break;
                }
            }
            return Crumbs(dict, selected, cssBrand, cssClass);
        }

        public MvcHtmlString Collapsible(string title
            , string contentId, Func<HelperResult> content, bool expanded = false)
        {

            TagBuilder main = new TagBuilder("div");
            main.AddCssClass("panel panel-default");

            TagBuilder header = new TagBuilder("div");
            header.AddCssClass("panel-heading");
            header.InnerHtml = string.Format(@"
                <h4 class='panel-title'>
                    <a data-toggle='collapse' data-parent='#accordion' href='#{0}'>{1}</a>
                </h4>
            ", contentId, title);

            TagBuilder contentTag = new TagBuilder("div");
            contentTag.AddCssClass("panel-collapse collapse");
            if (expanded)
                contentTag.AddCssClass("in");
            contentTag.Attributes.Add("id", contentId);
            contentTag.InnerHtml = string.Format(@"
            <div class='panel-body'>
                {0}
            </div>
            ", content.ToString());

            main.InnerHtml = header.ToString() + contentTag.ToString();

            return new MvcHtmlString(main.ToString());

        }

        public MvcHtmlString ProgressCircle(string id, int value, int min = 0, int max = 100
            , Func<HelperResult> contentTemplate = null, bool showText = true, string borderColor = "#eeeeee", string color = "#000000"
            , string style = "", int width = 15)
        {
            return ProgressCircle(id, value, min, max, contentTemplate == null ? null : contentTemplate().ToString().SanitizeEndLine()
                , showText, borderColor, color, style, width);
        }

        public MvcHtmlString ProgressCircle(string id, int value
            , string contentTemplate, bool showText = true, string style = "", int width = 15)
        {
            return ProgressCircle(id, value, 0, 100, contentTemplate, showText, style: style, width: width);
        }

        private MvcHtmlString ProgressCircle(string id, int value, int min = 0, int max = 100
            , string contentTemplate = null, bool showText = true, string borderColor = "#eeeeee", string color = "#000000"
            , string style = "", int width = 15)
        {

            /*
        $("#progress").shieldProgressBar({
            min: 0,
            max: 100,
            value: 70,
            layout: "circular",
            layoutOptions: {
                circular: {
                    borderColor: "#EEEEEE",
                    borderWidth: 1,
                    width: 10,
                    color: "blue",
                    backgroundColor: "transparent"
                }
            },
            text: {
                enabled: true,
                template: '<span style="font-size:20px; color: #1E98E4">711231</span><p style="font-size:20px; color: #1E98E4">hrs</p>'
            },
        }).swidget();
             
             */
            StringBuilder sb = new StringBuilder();

            TagBuilder main = new TagBuilder("div");
            main.Attributes.Add("id", id);
            if (!string.IsNullOrEmpty(style))
                main.Attributes.Add("style", style);

            string template = contentTemplate ?? "<span>{0:n1}%</span>";
            TagBuilder jsTag = new TagBuilder("script");
            jsTag.InnerHtml += "$(document).ready(function () {";
            jsTag.InnerHtml += "        $(\"#" + id + "\").shieldProgressBar({";
            jsTag.InnerHtml += "min: " + min.ToString();
            jsTag.InnerHtml += ", max: " + max.ToString();
            jsTag.InnerHtml += ", value: " + value.ToString();

            jsTag.InnerHtml += ", layout: \"circular\"";
            jsTag.InnerHtml += ", layoutOptions: {";
            jsTag.InnerHtml += "    circular: {";
            jsTag.InnerHtml += "        borderColor: \"" + borderColor + "\",";
            jsTag.InnerHtml += "        borderWidth: 1,";
            jsTag.InnerHtml += "        width: " + width.ToString() + ",";
            jsTag.InnerHtml += "        color: \"" + color + "\",";
            jsTag.InnerHtml += "        backgroundColor: \"transparent\"";
            jsTag.InnerHtml += "    }";
            jsTag.InnerHtml += "},";
            jsTag.InnerHtml += "text: {";
            jsTag.InnerHtml += "enabled: " + showText.ToString().ToLower();
            jsTag.InnerHtml += ", template: \"" + template + "\"";
            jsTag.InnerHtml += "},";
            jsTag.InnerHtml += "}).swidget();";
            jsTag.InnerHtml += "});";

            return new MvcHtmlString(main.ToString() + jsTag.ToString());

        }

        public MvcHtmlString TextBlock(string text, string styleClass = "")
        {
            if (string.IsNullOrWhiteSpace(text))
                return new MvcHtmlString("");

            TagBuilder span = new TagBuilder("span");
            if (string.IsNullOrEmpty(styleClass))
            {
                styleClass = "small text-muted";
            }
            span.AddCssClass(styleClass);
            span.InnerHtml = text;

            return new MvcHtmlString(span.ToString());
        }

        public MvcHtmlString HelpLink(string helpUrl = "", string helpLabel = ""
            , string iconCss = "glyphicon glyphicon-info-sign color-info"
            , string cssClass = "", string container = "", string containerCss = "", string popupParams = null)
        {
            if (String.IsNullOrEmpty(helpUrl))
                helpUrl = _Html.ExtendedViewData().HelpUrl();

            if (String.IsNullOrEmpty(helpUrl))
                return new MvcHtmlString("");

            if (String.IsNullOrEmpty(helpLabel))
                helpLabel = _Html.ExtendedViewData().HelpUrlLabel();

            if (String.IsNullOrEmpty(cssClass))
                cssClass = _Html.ExtendedViewData().HelpUrlClass();
            if (popupParams == null)
            {
                popupParams = "height=' + screen.height + ',width=' + screen.width + ',resizable=yes,scrollbars=yes,toolbar=no,menubar=no,location=no";
            }

            string openScript =
                string.Format("window.open('{0}','popup','{1}');return false;", helpUrl, popupParams);
            //string.Format("javascript:openWindow('{0}');", helpUrl);
            //string.Format("javascript:var newwindow = window.open('', '', 'height=600,width=800,resizable=no,scrollbars=yes,status=no').location = '{0}'; if (window.focus) {{ newwindow.focus(); }}", helpUrl);
            TagBuilder linkTag = new TagBuilder("a");
            linkTag.AddCssClass(cssClass);
            linkTag.Attributes.Add("href", "#");
            linkTag.Attributes.Add("onclick", openScript);
            linkTag.Attributes.Add("target", "_blank");
            linkTag.InnerHtml += helpLabel;

            TagBuilder iconTag = new TagBuilder("span");
            iconTag.AddCssClass(iconCss);
            if (!string.IsNullOrEmpty(iconCss))
                linkTag.InnerHtml += iconTag.ToString();


            if (!string.IsNullOrEmpty(container))
            {
                TagBuilder containerTag = new TagBuilder(container);
                containerTag.AddCssClass(containerCss);
                containerTag.InnerHtml = linkTag.ToString();
                return new MvcHtmlString(containerTag.ToString());
            }

            return new MvcHtmlString(linkTag.ToString());
        }

        public MvcHtmlString Alert(string content, bool dismisible = true, string cssClass = ""
            )
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new MvcHtmlString("");
            }

            TagBuilder main = new TagBuilder("div");
            main.AddCssClass("alert");
            main.AddCssClass("alert-success");
            if (dismisible)
            {
                main.AddCssClass("alert-dismissible");
                main.InnerHtml += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">×</button>";
            }
            main.AddCssClass(cssClass);
            main.InnerHtml += content;
            return new MvcHtmlString(main.ToString());
        }

        public MvcHtmlString Tabs(string id
            , Dictionary<string, string> partialSteps, int activeIndex = 0, bool isPills = false
            , string tabCss = "", string tabPanelCss = "", bool isJsActivated = false)
        {
            TagBuilder mainDiv = new TagBuilder("div");
            mainDiv.GenerateId(id);

            TagBuilder tabLink = new TagBuilder("ul");
            tabLink.AddCssClass("nav");
            tabLink.AddCssClass(isPills ? "nav-pills" : "nav-tabs");
            tabLink.AddCssClass(tabCss);

            TagBuilder tabPanel = new TagBuilder("div");
            tabPanel.AddCssClass("tab-content");
            tabPanel.AddCssClass(tabPanelCss);


            int i = 0;
            if (activeIndex < 0)
                activeIndex = 0;
            if (activeIndex >= partialSteps.Count)
                activeIndex = partialSteps.Count - 1;
            foreach (var partialStep in partialSteps)
            {
                string tabPanelId = string.Format("{0}_{1}", id, partialStep.Key);

                TagBuilder tabContent = new TagBuilder("div");
                tabContent.GenerateId(tabPanelId);
                tabContent.AddCssClass("tab-pane fade");
                tabContent.InnerHtml += _Html.Partial(partialStep.Value, _Html.ViewData.Model).ToString();

                string toggle = isJsActivated ? "" : "data-toggle=\"tab\"";
                TagBuilder tabItem = new TagBuilder("li");
                tabItem.InnerHtml = string.Format("<a {2} href=\"#{0}\">{1}</a>"
                    , tabContent.Attributes["id"], partialStep.Key, toggle);

                if (i == activeIndex)
                {
                    tabItem.AddCssClass("active");
                    tabContent.AddCssClass("tab-pane fade in active");
                }
                tabLink.InnerHtml += tabItem.ToString();
                tabPanel.InnerHtml += tabContent.ToString();
                i++;
            }

            mainDiv.InnerHtml += tabLink.ToString();
            mainDiv.InnerHtml += tabPanel.ToString();

            return new MvcHtmlString(mainDiv.ToString());
        }

        public MvcHtmlString Tabs(string id
            , Dictionary<string, Func<HelperResult>> tabTemplates, int activeIndex = 0, bool isPills = false
            , string tabCss = "", string tabPanelCss = "", TabOrientation orientation = TabOrientation.Horizontal
            , Func<string, HelperResult> headerTemplate = null, bool hideHeader = false
            )
        {
            TagBuilder mainDiv = new TagBuilder("div");
            mainDiv.GenerateId(id);

            TagBuilder tabLink = new TagBuilder("ul");
            tabLink.AddCssClass("nav");
            tabLink.AddCssClass(isPills ? "nav-pills" : "nav-tabs");
            tabLink.AddCssClass(tabCss);
            if (hideHeader)
                tabLink.AddCssClass("hidden");

            TagBuilder tabPanel = new TagBuilder("div");
            tabPanel.AddCssClass("tab-content");
            tabPanel.AddCssClass(tabPanelCss);


            int i = 0;
            if (activeIndex < 0)
                activeIndex = 0;
            if (activeIndex >= tabTemplates.Count)
                activeIndex = tabTemplates.Count - 1;
            foreach (var tabTemplate in tabTemplates)
            {
                string tabPanelId = string.Format("{0}_{1}", id, tabTemplate.Key);

                TagBuilder tabContent = new TagBuilder("div");
                tabContent.GenerateId(tabPanelId);
                tabContent.AddCssClass("tab-pane fade");
                tabContent.InnerHtml += tabTemplate.Value().ToString();

                TagBuilder tabItem = new TagBuilder("li");
                tabItem.InnerHtml = string.Format("<a data-toggle=\"tab\" href=\"#{0}\">{1}</a>"
                    , tabContent.Attributes["id"], headerTemplate == null ? tabTemplate.Key : headerTemplate(tabTemplate.Key).ToString());

                if (i == activeIndex)
                {
                    tabItem.AddCssClass("active");
                    tabContent.AddCssClass("tab-pane fade in active");
                }
                tabLink.InnerHtml += tabItem.ToString();
                tabPanel.InnerHtml += tabContent.ToString();
                i++;
            }

            if (orientation == TabOrientation.Vertical)
            {
                mainDiv.AddCssClass("row");
                tabLink.AddCssClass("tabs-left");

                TagBuilder leftDiv = new TagBuilder("div");
                leftDiv.AddCssClass("col-sm-3");
                leftDiv.InnerHtml = tabLink.ToString();

                TagBuilder rightDiv = new TagBuilder("div");
                rightDiv.AddCssClass("col-sm-9");
                rightDiv.InnerHtml = tabPanel.ToString();

                mainDiv.InnerHtml += leftDiv.ToString();
                mainDiv.InnerHtml += rightDiv.ToString();
            }
            else
            {
                mainDiv.InnerHtml += tabLink.ToString();
                mainDiv.InnerHtml += tabPanel.ToString();
            }

            return new MvcHtmlString(mainDiv.ToString());
        }

        public MvcHtmlString Tabs(string id
            , Dictionary<string, Func<HelperResult>> tabTemplates, string activeTab = null, bool isPills = false
            , string tabCss = "", string tabPanelCss = "", TabOrientation orientation = TabOrientation.Horizontal
            , Func<string, HelperResult> headerTemplate = null, bool hideHeader = false
            )
        {
            int activeIndex = 0;
            for (int i = 0; i < tabTemplates.Count; i++)
            {
                if (activeTab == tabTemplates.Keys.ToList()[i])
                {
                    activeIndex = i;
                    break;
                }
            }
            return Tabs(id, tabTemplates, activeIndex, isPills, tabCss, tabPanelCss, orientation, headerTemplate, hideHeader);
        }

        public MvcHtmlString FileIconFromName(string fileName, string additionalCss = "")
        {
            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass("fa fa-fw");
            icon.AddCssClass(additionalCss);
            string ext = Path.GetExtension(fileName);
            icon.AddCssClass(GetFAIcon(ext));

            return new MvcHtmlString(icon.ToString());
        }

        /// <summary>
        /// Get FA file icon by file extension or content type
        /// </summary>
        /// <param name="name">File Extension or Content Type</param>
        /// <returns>fa icon class</returns>
        private string GetFAIcon(string name)
        {
            //Need to add in more mime content type 
            string extIcon = "fa-file-o";
            switch (name.ToLower())
            {
                case ".txt":
                    extIcon = "fa-file-text-o";
                    break;
                case ".xls":
                case ".xlsx":
                case "application/vnd.ms-excel":
                    extIcon = "fa-file-excel-o";
                    break;
                case ".pdf":
                case "application/pdf":
                    extIcon = "fa-file-pdf-o";
                    break;
                case ".zip":
                    extIcon = "fa-file-zip-o";
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                    extIcon = "fa-file-image-o";
                    break;
                case ".doc":
                case ".docx":
                case "application/msword":
                    extIcon = "fa-file-word-o";
                    break;
            }
            return extIcon;
        }
        #region From View Data Value

        //public string ContentClass()
        //{
        //    string res = !_Html.ExtendedViewData().Inline() ? "" : "row";
        //    return res;
        //}

        //public string LabelClass()
        //{
        //    string res = !_Html.ExtendedViewData().Inline() ? ""
        //        : string.Format("col-sm-{0}", _Html.ExtendedViewData().LabelLength());
        //    return res;
        //}

        //public string InputClass()
        //{
        //    string res = !_Html.ExtendedViewData().Inline() ? ""
        //        : string.Format("col-sm-{0}", _Html.ExtendedViewData().InputLength());
        //    return res;
        //}

        public string ContentClass(bool inline)
        {
            string res = !inline ? "" : "row";
            return res;
        }

        public string LabelClass(bool inline, int labelLength)
        {
            string res = !inline ? ""
                : string.Format("col-sm-{0}", labelLength);
            return res;
        }

        public string InputClass(bool inline, int inputLength)
        {
            string res = !inline ? ""
                : string.Format("col-sm-{0}", inputLength);
            return res;
        }
        #endregion

        public MvcHtmlString Title(string text, bool lineAfter = false)
        {
            TagBuilder titleTag = new TagBuilder("div");
            titleTag.AddCssClass("h3");
            titleTag.InnerHtml += text;
            return new MvcHtmlString(titleTag.ToString() + (lineAfter ? LineTag() : ""));
        }
        public MvcHtmlString Title(MvcHtmlString htmlString, bool lineAfter = false)
        {
            return Title(htmlString.ToString(), lineAfter);
        }
        public MvcHtmlString Title(Func<HelperResult> contentTemplate, bool lineAfter = false)
        {
            return Title(contentTemplate().ToString());
        }

        public MvcHtmlString SubTitle(string text, string description, bool lineAfter = false, bool uppercase = false)
        {
            TagBuilder titleTag = new TagBuilder("div");
            titleTag.AddCssClass("h4");
            if (uppercase)
            {
                titleTag.AddCssClass("text-uppercase");
            }
            titleTag.InnerHtml += text;
            string result = titleTag.ToString();
            if (!string.IsNullOrEmpty(description))
            {
                TagBuilder descTag = new TagBuilder("p");
                descTag.InnerHtml = description;
                result += descTag.ToString();
            }
            return new MvcHtmlString(result + (lineAfter ? LineTag() : ""));
        }
        public MvcHtmlString SubTitle(string text, bool lineAfter = false, bool uppercase = false)
        {
            return SubTitle(text, null, lineAfter, uppercase);
        }
        public MvcHtmlString SubTitle(MvcHtmlString htmlString, string description = null, bool lineAfter = false)
        {
            return SubTitle(htmlString.ToString(), description, lineAfter);
        }
        public MvcHtmlString SubTitle(Func<HelperResult> contentTemplate, string description = null, bool lineAfter = false)
        {
            return SubTitle(contentTemplate().ToString(), description, lineAfter);
        }

        private string LineTag(string additionalClass = null)
        {
            TagBuilder line = new TagBuilder("hr");
            line.AddCssClass("compact");
            if (!string.IsNullOrEmpty(additionalClass))
            {
                line.AddCssClass(additionalClass);
            }
            return line.ToString(TagRenderMode.SelfClosing);
        }

        public MvcHtmlString HSeparator(string additionalClass = null)
        {
            return new MvcHtmlString(LineTag(additionalClass));
        }

        public TableBuilder<TModel> HtmlTable<TModel>(IEnumerable<TModel> dataSource)
            where TModel : class
        {
            return new TableBuilder<TModel>(_Html).DataSource(dataSource);
        }

    }
    public class DTC<TModel> : ComponentFactory
        where TModel : class
    {
        private HtmlHelper<TModel> _Html;

        public DTC(HtmlHelper<TModel> html)
            : base(html)
        {
            this._Html = html;
        }

        //public TableBuilder<TModel> TableForModel(IEnumerable<TModel> dataSource)
        //{
        //    return new TableBuilder<TModel>(_Html).DataSource(dataSource);
        //}


        //public MvcHtmlString DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression, object additionalViewData)
        //{
        //    return DisplayFor(expression, null, additionalViewData);
        //}

        //public MvcHtmlString DisplayFor<TValue>(Expression<Func<TModel, TValue>> expression
        //    , string templateName = null, object additionalViewData = null
        //    , string separator = ":", string labelClass = "", string fieldClass = "", bool hideIfNull = false
        //    , bool compact = false
        //    //, bool inline = false, bool hideLabel = false, string label = ""
        //    )
        //{

        //    #region View Data Properties

        //    bool inline = _Html.ExtendedViewData().Inline();
        //    bool hideLabel = _Html.ExtendedViewData().HideLabel();
        //    string customLabel = _Html.ExtendedViewData().Label();
        //    int labelLength = _Html.ExtendedViewData().LabelLength();
        //    int inputLength = _Html.ExtendedViewData().InputLength();
        //    RouteValueDictionary routedict = new RouteValueDictionary(additionalViewData);
        //    if (additionalViewData != null)
        //    {
        //        foreach (KeyValuePair<string, object> kvp in routedict)
        //        {
        //            //_Html.ViewData[kvp.Key] = kvp.Value;

        //            if (kvp.Key == ExtendedViewData.Keys.Inline)
        //            {
        //                inline = Convert.ToBoolean(kvp.Value);
        //            }
        //            if (kvp.Key == ExtendedViewData.Keys.HideLabel)
        //            {
        //                hideLabel = Convert.ToBoolean(kvp.Value);
        //            }
        //            if (kvp.Key == ExtendedViewData.Keys.Label)
        //            {
        //                customLabel = Convert.ToString(kvp.Value);
        //            }
        //            if (kvp.Key == ExtendedViewData.Keys.LabelLength)
        //            {
        //                labelLength = Convert.ToInt32(kvp.Value);
        //            }
        //            if (kvp.Key == ExtendedViewData.Keys.InputLength)
        //            {
        //                inputLength = Convert.ToInt32(kvp.Value);
        //            }
        //        }
        //    }

        //    #endregion
        //    if (hideIfNull)
        //    {
        //        var value = ModelMetadata.FromLambdaExpression(expression, _Html.ViewData).Model;
        //        if (value == null || Convert.ToString(value) == string.Empty)
        //            return new MvcHtmlString("");
        //    }

        //    #region Display Label

        //    TagBuilder separatorTag = new TagBuilder("span");
        //    if (inline && !compact)
        //        separatorTag.AddCssClass("pull-right");
        //    separatorTag.AddCssClass("separator");
        //    separatorTag.InnerHtml = separator;

        //    string containerTag = "div";
        //    string labelFormat = inline ? "<p>{0}{1}</p>" : "{0}{1}";
        //    string fieldFormat = "<p>{0}</p>";
        //    if (compact)
        //    {
        //        containerTag = "span";
        //        labelFormat = "<strong>{0} {1} </strong>";
        //        fieldFormat = "<span>{0}</span>";
        //    }

        //    TagBuilder labelTag = new TagBuilder(containerTag);
        //    labelTag.AddCssClass("display-label");
        //    if (!string.IsNullOrEmpty(labelClass))
        //    {
        //        labelTag.AddCssClass(labelClass);
        //    }
        //    if (!compact)
        //        labelTag.AddCssClass(_Html.Bootstrap().LabelClass(inline, labelLength));
        //    labelTag.InnerHtml += string.Format(labelFormat
        //        , _Html.ExtDisplayNameFor(expression, customLabel).ToString()
        //        , separatorTag.ToString()
        //        );
        //    #endregion

        //    #region Value Label

        //    TagBuilder fieldTag = new TagBuilder(containerTag);
        //    fieldTag.AddCssClass("display-field");
        //    if (!string.IsNullOrEmpty(fieldClass))
        //    {
        //        fieldTag.AddCssClass(fieldClass);
        //    }
        //    if (!compact)
        //        fieldTag.AddCssClass(_Html.Bootstrap().InputClass(inline, inputLength));
        //    string fieldHtml = _Html.DisplayFor(expression, templateName, additionalViewData).ToString();
        //    if (fieldHtml.Contains("<"))
        //        fieldFormat = "{0}";
        //    fieldTag.InnerHtml += string.Format(fieldFormat, fieldHtml);
        //    //fieldTag.InnerHtml = fieldTag.InnerHtml.Replace("&lt;br/&gt;", "<br/>");

        //    #endregion

        //    TagBuilder mainTag = new TagBuilder(containerTag);
        //    if (!compact)
        //        mainTag.AddCssClass(_Html.Bootstrap().ContentClass(inline));

        //    //if (!_Html.ExtendedViewData().HideLabel())
        //    if (!hideLabel)
        //        mainTag.InnerHtml += labelTag.ToString();
        //    mainTag.InnerHtml += fieldTag.ToString();

        //    return new MvcHtmlString(mainTag.ToString());
        //}

        //public MvcHtmlString EditorTemplateFor(Func<TModel, HelperResult> renderEditor, MvcHtmlString validationError, string label = ""
        //    , bool hideDescription = false)
        //{
        //    #region Display Label
        //    bool disabled = _Html.ExtendedViewData().Disabled();
        //    string customLabel = string.IsNullOrWhiteSpace(label) ? _Html.ExtendedViewData().Label() : label;
        //    TagBuilder labelTag = new TagBuilder("div");
        //    labelTag.AddCssClass(_Html.Bootstrap().LabelClass(_Html.ExtendedViewData().Inline(), _Html.ExtendedViewData().LabelLength()));
        //    if (!_Html.ExtendedViewData().HideLabel())
        //    {
        //        string tooltipsHtml = string.Empty;
        //        string tooltip = _Html.ExtendedViewData().HelpTooltip();
        //        if (!string.IsNullOrEmpty(tooltip))
        //            tooltipsHtml += _Html.Bootstrap().TooltipIcon(tooltip);
        //        string infotooltip = _Html.ExtendedViewData().InfoTooltip();
        //        if (!string.IsNullOrEmpty(infotooltip))
        //            tooltipsHtml += _Html.Bootstrap().TooltipIcon(infotooltip, iconClass: "fa-info-circle", iconColorClass: "info");

        //        labelTag.InnerHtml += string.IsNullOrWhiteSpace(customLabel)
        //            ? _Html.ExtLabelFor(m => m, new { @class = "control-label" }, disabled, tooltipsHtml)
        //            : _Html.ExtLabelFor(m => m, customLabel, new { @class = "control-label" }, disabled, tooltipsHtml)
        //            ;
        //        labelTag.InnerHtml += _Html.Bootstrap().HelpLink(container: "div");

        //    }

        //    #endregion

        //    #region Editor

        //    TagBuilder editorTag = new TagBuilder("div");
        //    editorTag.AddCssClass(_Html.Bootstrap().InputClass(_Html.ExtendedViewData().Inline(), _Html.ExtendedViewData().InputLength()));
        //    editorTag.AddCssClass("controls");
        //    editorTag.InnerHtml = renderEditor(_Html.ViewData.Model).ToString();
        //    if (!hideDescription)
        //    {
        //        if (!string.IsNullOrEmpty(_Html.ViewData.ModelMetadata.Description))
        //        {
        //            editorTag.InnerHtml += TextBlock(_Html.ViewData.ModelMetadata.Description).ToString();
        //        }
        //        if (!string.IsNullOrEmpty(_Html.ExtendedViewData().HelpText()))
        //        {
        //            editorTag.InnerHtml += TextBlock(_Html.ExtendedViewData().HelpText()).ToString();
        //        }
        //    }

        //    #endregion

        //    TagBuilder mainTag = new TagBuilder("div");
        //    mainTag.AddCssClass(_Html.Bootstrap().ContentClass(_Html.ExtendedViewData().Inline()));
        //    mainTag.AddCssClass(string.Format("form-group{0}", validationError));

        //    //show if !hidelabel or inline
        //    if (!(_Html.ExtendedViewData().HideLabel() && !_Html.ExtendedViewData().Inline()))
        //        mainTag.InnerHtml += labelTag.ToString();
        //    mainTag.InnerHtml += editorTag.ToString();

        //    return new MvcHtmlString(mainTag.ToString());
        //}



    }
}

