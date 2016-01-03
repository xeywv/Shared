using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace XUtils
{
    /// <summary>
    /// Class contains functions for validatiing form controls.
    /// Errors in validation will appear as a tooltip on the control being tested.
    /// There is also a ShowToolTip function to display any custom validation 
    /// messages in the same form as this control provides.
    /// </summary>
    public static class Validation
    {
        #region Member Variables
        static private ToolTip m_cToolTip = new ToolTip();  // tool tip that is displayed        
        #endregion

        #region Static Contructor
        static Validation()
        {
            ToolTipDisplayTime = 4000;
            m_cToolTip.IsBalloon = true;
        }        
        #endregion

        #region Public Properties
        /// <summary>Sets display time for tool tip</summary>
        static public int ToolTipDisplayTime { get; set; }        
        #endregion

        #region Public Members
        /// <summary>
        /// Validate text in the control for
        ///     1. Can text be parsed to specified type
        ///     2. Control text is not blank if required
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="cType">Expected data type (e.g. typeof(string), typeof(int))</param>
        /// <param name="bRequired">If user must enter value, or can it be blank</param>
        /// <returns>If text is valid</returns>
        static public bool ValidateText(Control cControl, Type cType, bool bRequired)
        {
            string sValue = cControl.Text;
            string sError = "";

            // Error if required
            if ((sValue == "") && bRequired)
            {
                ShowToolTip("Invalid value", "Please enter a value.", cControl);
                return false;
            }

            // Type to parse it
            switch (cType.Name.ToLower())
            {
                case "string":
                    break;

                case "double":
                    double dTemp;
                    if (!double.TryParse(sValue, out dTemp))
                        sError = "Must be a decimal value.";
                    break;

                case "int32":
                    Int32 iTemp;
                    if (!Int32.TryParse(sValue, out iTemp))
                        sError = "Must be a integer value.";
                    break;

                case "uint32":
                    UInt32 uiTemp;
                    if (!UInt32.TryParse(sValue, out uiTemp))
                        sError = "Must be a unsigned integer value.";
                    break;

                default:
                    sError = string.Format("Invalid type {0}", cType.Name);
                    throw new ApplicationException(sError);
            }

            if (sError != "")
            {
                ShowToolTip("Invalid value", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate text in the control for
        ///     1. Can text be parsed to specified type
        ///     2. Control text is not blank if required
        ///     3. Control text does not exceed maximum length
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="cType">Expected data type (e.g. typeof(string), typeof(int))</param>
        /// <param name="bRequired">If user must enter value, or can it be blank</param>
        /// <param name="iMaxLength">Maximum text length</param>
        /// <returns>If text is valid</returns>
        static public bool ValidateText(Control cControl, Type cType, bool bRequired, int iMaxLength)
        {
            string sValue = cControl.Text;

            // Error if required
            if ( (sValue == "") && bRequired )
            {
                ShowToolTip ( "Invalid value", "Please enter a value.", cControl );
                return false;
            }

            // Test it is correct length
            if ( sValue.Length > iMaxLength )
            {
                string sError = string.Format("Must be less than {0} characters.", iMaxLength);
                ShowToolTip ( "Invalid value", sError, cControl );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate text in the control for
        ///     1. Can text be parsed to specified type
        ///     2. Control text is not blank if required
        ///     3. Control text is a number within the specified range
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="cType">Expected data type but should be number (e.g. typeof(int), typeof(double))</param>
        /// <param name="bRequired">If user must enter value, or can it be blank</param>
        /// <param name="dMin">Minimum allowed value</param>
        /// <param name="dMax">Maximum allowed value</param>
        /// <returns>If text is valid</returns>
        static public bool ValidateText(Control cControl, Type cType, bool bRequired, double dMin, double dMax)
        {
            string sValue = cControl.Text;

            // Do standard validation
            if (!ValidateText(cControl, cType, bRequired))
                return false;

            // Check the value is in range
            double dValue = double.Parse(sValue);
            if ((dValue < dMin) || (dValue > dMax))
            {
                string sError = string.Format("Must be in the range {0} to {1}", dMin, dMax);
                ShowToolTip("Invalid range", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if item is selected in a list cotrol.
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must select an item</param>
        /// <returns>If item selected</returns>
        static public bool ValidateListControl(ListControl cControl, bool bRequired)
        {
            // Error if required
            if (bRequired && (cControl.SelectedIndex == -1))
            {
                string sError = string.Format("Please select an item from the list");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if one, or more, items are selected in a list view.
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must select an item</param>
        /// <param name="bSingleItemOnly">If user is can only select a single item</param>
        /// <returns>If validation passes</returns>
        public static bool ValidateListView(ListView cControl, bool bRequired)
        {
            return ValidateListView(cControl, true, false);
        }
        public static bool ValidateListView(ListView cControl, bool bRequired, bool bSingleItemOnly)
        {
            // Error if required
            if (bRequired && (cControl.SelectedItems.Count == 0))
            {
                string sError = string.Format("Please select an item from the list");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            // Error if single item only
            if (bSingleItemOnly && (cControl.SelectedItems.Count != 1))
            {
                string sError = string.Format("Only select one item in the list");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if one, or more, items are selected in a list are checked
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must check an item</param>
        /// <param name="bSingleItemOnly">If user is can only check a single item</param>
        /// <returns>If validation passes</returns>
        public static bool ValidateCheckedListBox(CheckedListBox cControl, bool bRequired)
        {
            return ValidateCheckedListBox(cControl, true, false);
        }
        public static bool ValidateCheckedListBox(CheckedListBox cControl, bool bRequired, bool bSingleItemOnly)
        {
            // Error if required
            if (bRequired && (cControl.CheckedItems.Count == 0))
            {
                string sError = string.Format("Please check an item from the list");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            // Error if single item only
            if (bSingleItemOnly && (cControl.CheckedItems.Count != 1))
            {
                string sError = string.Format("Only check one item in the list");
                ShowToolTip("Too many", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if item is selected in a tree view cotrol.
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must select an item</param>
        /// <returns>If item selected</returns>
        public static bool ValidateTreeView(TreeView cControl, bool bRequired)
        {
            // Error if required
            if (bRequired && (cControl.SelectedNode == null))
            {
                string sError = string.Format("Please select an item.");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate if item is selected in a combobox.
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must select an item</param>
        /// <returns>If item selected</returns>
        public static bool ValidateComboBox(ComboBox cControl, bool bRequired)
        {
            // Error if required
            if (bRequired && (cControl.SelectedIndex == -1))
            {
                string sError = string.Format("Please select an item.");
                ShowToolTip("Not selected", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate text in the control
        ///     1. Is a valid email address
        ///     2. Control text is not blank if required
        ///     3. Control text does not exceed maximum length
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must enter value, or can it be blank</param>
        /// <param name="iMaxLength">Maximum text length</param>
        /// <returns>If text is valid</returns>
        public static bool ValidateEmailAddress(Control cControl, bool bRequired, int iMaxLength)
        {
            // Test if required, and length
            if (!ValidateText(cControl, typeof(string), bRequired, iMaxLength))
                return false;

            // If got to this stage and is empty then must be allowed (so end)
            if (string.IsNullOrEmpty(cControl.Text))
                return true;

            //regular expression pattern for valid email
            //addresses, allows for the following domains:
            //com,edu,info,gov,int,mil,net,org,biz,name,museum,coop,aero,pro,tv
            string pattern = @"^[-a-zA-Z0-9_][-.a-zA-Z0-9_]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.
            (com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";
            //Regular expression object
            Regex check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);

            // Validate the address
            if (!check.IsMatch(cControl.Text))
            {
                string sError = string.Format("Email address is not in correct format.");
                ShowToolTip("Incorrect format", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate text in the control 
        ///     1. Is a valid phone number
        ///     2. Control text is not blank if required
        ///     3. Control text does not exceed maximum length
        /// </summary>
        /// <param name="cControl">Control to validate</param>
        /// <param name="bRequired">If user must enter value, or can it be blank</param>
        /// <param name="iMaxLength">Maximum text length</param>
        /// <returns>If text is valid</returns>
        public static bool ValidatePhoneNumberAddress(Control cControl, bool bRequired, int iMaxLength)
        {
            if (!ValidateText(cControl, typeof(string), bRequired, iMaxLength))
                return false;

            // If got to this stage and is empty then must be allowed (so end)
            if (string.IsNullOrEmpty(cControl.Text))
                return true;

            // check each value is a digit (or + sign)
            bool isValid = true;
            for (int pos = 0; pos < cControl.Text.Length; pos++)
            {
                char c = cControl.Text.ToCharArray()[pos];
                isValid = isValid && (Char.IsDigit(c) || ((c == '+') && ((pos == 0) || (pos == 1))));
            }

            // Validate the address
            if (!isValid)
            {
                string sError = string.Format("Phone numbers must not contain spaces, -, or ( ) characters.");
                ShowToolTip("Incorrect format", sError, cControl);
                return false;
            }

            // Don't allow number of emergency services
            if ((cControl.Text == "999") || (cControl.Text == "911"))
            {
                string sError = string.Format("Specified number is invalid.");
                ShowToolTip("Invalid number", sError, cControl);
                return false;
            }

            return true;
        }

        /// <summary>Displays the tool tip</summary>
        /// 
        /// <param name="sTitle">Title</param>
        /// <param name="sText">Tool tip message</param>
        /// <param name="ctrl">associated control</param>
        static public void ShowToolTip(string sTitle, string sText, Control ctrl)
        {
            // In win 98 can end up with crash if call again when form for previous tooltip has been closed.
            // So to prevent this remove old tooltip, and create a new one.
            if (m_cToolTip != null)
                m_cToolTip.Dispose();
            m_cToolTip = new ToolTip();
            m_cToolTip.IsBalloon = true;

            m_cToolTip.ToolTipTitle = sTitle;

            // There is a bug with the ToolTip.Show method, as the first time it is 
            // displayed it is not positioned at the correct control, so need to 
            // display it twice to get it to work correctly
            m_cToolTip.Active = false;
            m_cToolTip.Show("", ctrl);
            m_cToolTip.Active = true;

            // Now it will appear in the correct postion
            m_cToolTip.Show(sText, ctrl, ToolTipDisplayTime);
            m_cToolTip.Tag = ctrl;
        } 
        #endregion
    }
}
