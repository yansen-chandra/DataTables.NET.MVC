using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using DataTables.MVC.Extensions;

namespace DataTables.MVC.Fluent
{
    public class ExtendedViewData
    {
        public struct Keys
        {
            public const string Inline = "inline";
            public const string LabelLength = "labellength";
            public const string InputLength = "inputlength";
            public const string Label = "label";
            public const string HideLabel = "hideLabel";
            public const string HelpText = "helptext";
            public const string HelpTooltip = "helptooltip";
            public const string InfoTooltip = "infotooltip";
            public const string HelpUrl = "helpurl";
            public const string HelpUrlLabel = "helpurllabel";
            public const string HelpUrlClass = "helpurlclass";
            public const string DisableValidation = "disableValidation";
            public const string Disabled = "disabled";
            public const string ParentIndex = "parentindex";
        }
        
        private HtmlHelper _Html;
        public ExtendedViewData(HtmlHelper html)
        {
            this._Html = html;
        }

        public int LabelLength()
        {
            return _Html.GetViewData<int>(Keys.LabelLength, 4);
        }

        public int InputLength()
        {
            return _Html.GetViewData<int>(Keys.InputLength, 8);
        }

        public string Label()
        {
            return _Html.GetViewData<string>(Keys.Label);
        }

        public bool HideLabel()
        {
            return _Html.GetViewData<bool>(Keys.HideLabel);
        }

        public string HelpText()
        {
            return _Html.GetViewData<string>(Keys.HelpText);
        }

        public string HelpTooltip()
        {
            return _Html.GetViewData<string>(Keys.HelpTooltip);
        }

        public string InfoTooltip()
        {
            return _Html.GetViewData<string>(Keys.InfoTooltip);
        }

        public string HelpUrl()
        {
            return _Html.GetViewData<string>(Keys.HelpUrl);
        }

        public string HelpUrlLabel()
        {
            return _Html.GetViewData<string>(Keys.HelpUrlLabel);
        }

        public string HelpUrlClass()
        {
            return _Html.GetViewData<string>(Keys.HelpUrlClass);
        }

        public bool DisableValidation()
        {
            return _Html.GetViewData<bool>(Keys.DisableValidation);
        }

        public bool Disabled()
        {
            return _Html.GetViewData<bool>(Keys.Disabled);
        }

        public bool Inline()
        {
            return _Html.GetViewData<bool>(Keys.Inline, true);
        }

        public int ParentIndex()
        {
            return _Html.GetViewData<int>(Keys.ParentIndex);
        }

    }

    public class ExtendedViewData<TModel> : ExtendedViewData
    {
        private HtmlHelper<TModel> _Html;
        public ExtendedViewData(HtmlHelper<TModel> html)
            : base(html)
        {
            this._Html = html;
        }

        public new int LabelLength()
        {
            return _Html.GetViewData<int, TModel>(Keys.LabelLength, base.LabelLength());
        }

        public new int InputLength()
        {
            return _Html.GetViewData<int, TModel>(Keys.InputLength, base.InputLength());
        }

        public new string Label()
        {
            return _Html.GetViewData<string, TModel>(Keys.Label, base.Label());
        }

        public new bool HideLabel()
        {
            return _Html.GetViewData<bool, TModel>(Keys.HideLabel, base.HideLabel());
        }

        public new string HelpText()
        {
            return _Html.GetViewData<string, TModel>(Keys.HelpText, base.HelpText());
        }

        public new string HelpTooltip()
        {
            return _Html.GetViewData<string, TModel>(Keys.HelpTooltip, base.HelpTooltip());
        }

        public new string InfoTooltip()
        {
            return _Html.GetViewData<string, TModel>(Keys.InfoTooltip, base.InfoTooltip());
        }

        public new string HelpUrl()
        {
            return _Html.GetViewData<string, TModel>(Keys.HelpUrl, base.HelpUrl());
        }

        public new string HelpUrlLabel()
        {
            return _Html.GetViewData<string, TModel>(Keys.HelpUrlLabel, base.HelpUrlLabel());
        }

        public new string HelpUrlClass()
        {
            return _Html.GetViewData<string, TModel>(Keys.HelpUrlClass, base.HelpUrlClass());
        }

        public new bool DisableValidation()
        {
            return _Html.GetViewData<bool, TModel>(Keys.DisableValidation, base.DisableValidation());
        }

        public new bool Disabled()
        {
            return _Html.GetViewData<bool, TModel>(Keys.Disabled, base.Disabled());
        }

        public new bool Inline()
        {
            return _Html.GetViewData<bool, TModel>(Keys.Inline, base.Inline());// && !_Html.ExtendedViewData().HideLabel();
        }

        public new int ParentIndex()
        {
            return _Html.GetViewData<int, TModel>(Keys.ParentIndex, base.ParentIndex());
        }

    }

}
