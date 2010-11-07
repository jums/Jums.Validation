using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jums.Validation.HandyValidators
{
    /// <summary>
    /// Validates that the target text input controls text length is within  given limits.
    /// </summary>
    /// <remarks>
    /// Unlike standard Web Forms validators, this has dynamic display, focus on error and "error" css class by default.
    /// </remarks>
    [ToolboxData("<{0}:LengthValidator runat=server></{0}:LengthValidator>")]
    public class LengthValidator : BaseValidator
    {
        /// <summary>
        /// Minimum value string length. 0 = no limit.
        /// </summary>
        [DefaultValue(0)]
        [Category("Handy")]
        public int Min {
            get { return (int)(this.ViewState["Min"] ?? 0); }
            set { this.ViewState["Min"] = value; }
        }
        
        /// <summary>
        /// Maximum vlaue string length. 0 = no limit.
        /// </summary>
        [DefaultValue(0)]
        [Category("Handy")]
        public int Max
        {
            get { return (int)(this.ViewState["Max"] ?? 0); }
            set { this.ViewState["Max"] = value; }
        }

        public LengthValidator() : base()
        {
            this.CssClass = "error";
            this.Display = ValidatorDisplay.Dynamic;
            this.SetFocusOnError = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.EnableClientScript && this.DetermineRenderUplevel())
            {
                string javascriptValidationFunction = CreateJavaScript();
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "HandyCheckLength");
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "minLength", this.Min.ToString());
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "maxLength", this.Max.ToString());
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "JSScript", javascriptValidationFunction);
            }
        }

        protected string CreateJavaScript()
        {
            return @"
<script type=""text/javascript"">
    function HandyCheckLength(validator){
        var value = ValidatorTrim(ValidatorGetValue(validator.controltovalidate));
        if (value.length == 0) return true;
        if (validator.minLength > 0 && value.length < validator.minLength) return false;
        if (validator.maxLength > 0 && value.length > validator.maxLength) return false;
        return true;
    }
</script>";
        }

        protected override bool ControlPropertiesValid()
        {
            IEditableTextControl ctrl = FindControl(ControlToValidate) as IEditableTextControl;
            return (ctrl != null);
        }

        protected override bool EvaluateIsValid()
        {
            return this.CheckInputLength();
        }

        protected bool CheckInputLength()
        {
            IEditableTextControl ctrl = ((IEditableTextControl)this.FindControl(this.ControlToValidate));
            string value = (ctrl.Text ?? "").Trim();

            if (this.Min > 0)
            {
                if (value.Length < this.Min)
                {
                    return false;
                }
            }

            if (this.Max > 0)
            {
                if (value.Length > this.Max)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
