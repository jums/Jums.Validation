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
    /// Validates that the target CheckBox control is checked.
    /// </summary>
    /// <remarks>
    /// Unlike standard Web Forms validators, this has dynamic display, focus on error and "error" css class by default.
    /// </remarks>
    [ToolboxData("<{0}:RequireCheckedValidator runat=server></{0}:RequireCheckedValidator>")]
    public class RequireCheckedValidator : BaseValidator
    {
        public RequireCheckedValidator() : base()
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
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "HandyCheckIfChecked");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "JSScript", javascriptValidationFunction);
            }
            
        }

        protected string CreateJavaScript()
        {
            return @"
<script type=""text/javascript"">
    function HandyCheckIfChecked(validator){
        var checkbox = document.getElementById(validator.controltovalidate);
        return checkbox.checked;
    }
</script>";
        }

        protected override bool ControlPropertiesValid()
        {
            Control ctrl = this.FindControl(ControlToValidate) as CheckBox;
            return (ctrl != null);
        }

        protected override bool EvaluateIsValid()
        {
            return this.CheckIfItemIsChecked();
        }

        protected bool CheckIfItemIsChecked()
        {
            CheckBox checkbox = ((CheckBox) this.FindControl(this.ControlToValidate));
            return checkbox.Checked;
        }
    }
}
