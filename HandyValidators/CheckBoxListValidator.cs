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
    /// Validates that the target CheckBoxList control has proper amoutn of checked items.
    /// </summary>
    /// <remarks>
    /// Unlike standard Web Forms validators, this has dynamic display, focus on error and "error" css class by default.
    /// </remarks>
    [ToolboxData("<{0}:CheckBoxListValidator runat=server></{0}:CheckBoxListValidator>")]
    public class CheckBoxListValidator : BaseValidator
    {
        /// <summary>
        /// Minimum amount of items checked.
        /// </summary>
        [DefaultValue(0)]
        [Category("Handy")]
        public int Min
        {
            get { return (int)(this.ViewState["Min"] ?? 0); }
            set { this.ViewState["Min"] = value; }
        }

        /// <summary>
        /// Maximum amount of items checked.
        /// </summary>
        [DefaultValue(0)]
        [Category("Handy")]
        public int Max
        {
            get { return (int)(this.ViewState["Max"] ?? 0); }
            set { this.ViewState["Max"] = value; }
        }

        public CheckBoxListValidator() : base()
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
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "evaluationfunction", "HandyCheckIfListChecked");
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "minimum", this.Min.ToString());
                Page.ClientScript.RegisterExpandoAttribute(this.ClientID, "maximum", this.Max.ToString());
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "JSScript", javascriptValidationFunction);
            }
            
        }

        protected string CreateJavaScript()
        {
            return @"
<script type=""text/javascript"">
    function HandyCheckIfListChecked(validator){
        var checkBoxList = document.getElementById(validator.controltovalidate);
        var checkBoxes = checkBoxList.getElementsByTagName(""input"");
        var amount = 0;

        for(var i=0; i<checkBoxes.length; i++){
            if(checkBoxes.item(i).checked){
                amount++;
            }
        }

        if (validator.minimum > 0 && amount < validator.minimum) return false;
        if (validator.maximum > 0 && amount > validator.maximum) return false;
        return true;
    }
</script>";

        }

        protected override bool ControlPropertiesValid()
        {
            Control ctrl = this.FindControl(ControlToValidate) as CheckBoxList;
            return (ctrl != null);
        }

        protected override bool EvaluateIsValid()
        {
            return this.CheckIfItemIsChecked();
        }

        protected bool CheckIfItemIsChecked()
        {
            CheckBoxList checkboxList = ((CheckBoxList)this.FindControl(this.ControlToValidate));
            int amount = CountCheckedItems(checkboxList);

            if (amount < this.Min)
            {
                return false;
            }

            if (this.Max > 0)
            {
                if (amount > this.Max)
                {
                    return false;
                }
            }

            return true;
        }

        private static int CountCheckedItems(CheckBoxList checkboxList)
        {
            int amount = 0;

            foreach (ListItem listItem in checkboxList.Items)
            {
                if (listItem.Selected == true)
                {
                    amount++;
                }
            }

            return amount;
        }
    }
}
