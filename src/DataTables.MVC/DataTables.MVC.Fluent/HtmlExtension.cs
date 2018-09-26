using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Razor;

namespace DataTables.MVC.Fluent
{
    public static class HtmlExtension
    {
        public static ExtendedViewData ExtendedViewData(this HtmlHelper html)
        {
            return new ExtendedViewData(html);
        }

        public static ExtendedViewData<TModel> ExtendedViewData<TModel>(this HtmlHelper<TModel> html)
        {
            return new ExtendedViewData<TModel>(html);
        }

        public static T GetViewData<T>(this HtmlHelper html, string key, T defaultValue = default(T))
        {
            T val = defaultValue;
            if (html.ViewData.ContainsKey(key))
            {
                val = (T)(html.ViewData[key]);
            }
            return val;
        }

        public static T GetViewData<T, TModel>(this HtmlHelper<TModel> html, string key, T defaultValue = default(T))
        {
            T val = defaultValue;
            if (html.ViewData.ContainsKey(key))
            {
                val = (T)(html.ViewData[key]);
            }
            return val;
        }

        public static MvcHtmlString ExtHiddenText<TModel, TValue>(this HtmlHelper<TModel> html
            , Expression<Func<TModel, TValue>> expression, object additionalAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var propertyName = html.NameFor(expression).ToString();
            var propertyValue = metadata.Model ?? "";

            var htmlAttributes = html.GetUnobtrusiveValidationAttributes(html.ViewData.ModelMetadata.PropertyName, html.ViewData.ModelMetadata);
            var addDict = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalAttributes);
            addDict.ToList().ForEach(x => htmlAttributes.SetAttribute(x.Key, x.Value));

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttribute("type", "text");
            tagBuilder.MergeAttribute("name", propertyName);
            tagBuilder.MergeAttribute("value", string.IsNullOrEmpty(metadata.DisplayFormatString) ? Convert.ToString(propertyValue) : String.Format(metadata.DisplayFormatString, propertyValue));
            tagBuilder.GenerateId(propertyName);
            tagBuilder.MergeAttributes(htmlAttributes);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ExtValidationSummary(this HtmlHelper html
            , bool excludePropertyError, string transactionId = null
            , string message = null)
        {
            if (message == null)
                message = "You have the following errors to resolve :";

            var valSummary = html.ValidationSummary(excludePropertyError, message, new { @class = "alert alert-danger" });
            String htmlResult = string.Empty;
            if (valSummary != null)
            {
                htmlResult = valSummary.ToString();
            }
            if (!string.IsNullOrEmpty(transactionId))
            {
                string formName = "form_id";
                TagBuilder formId = new TagBuilder("input");
                formId.GenerateId(formName);
                formId.Attributes.Add("name", formName);
                formId.Attributes.Add("type", "hidden");
                formId.Attributes.Add("value", transactionId);

                htmlResult += formId.ToString();
            }

            return new MvcHtmlString(htmlResult);

        }

        public static void SetAttribute(this IDictionary<string, object> attr, string key, object value)
        {
            if (!attr.ContainsKey(key))
            {
                attr.Add(key, value);
            }
            else
            {
                attr[key] = value;
            }
        }


        public static ComponentFactory DTC(this HtmlHelper html)
        {
            return new ComponentFactory(html);
        }

        public static DTC<TModel> DTC<TModel>(this HtmlHelper<TModel> html)
            where TModel : class
        {
            return new DTC<TModel>(html);
        }

    }
}
