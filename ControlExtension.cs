using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared
{
    public static class ControlExtension
    {
        /// <summary>Get all child control (including children of children) of specific type</summary>
        public static IEnumerable<T> GetAllChildControls<T>(this Control ctrl)
        {
            return ctrl.GetAllChildControls().OfType<T>();
        }

        /// <summary>Get all child control (including children of children)</summary>
        public static IEnumerable<Control> GetAllChildControls(this Control ctrl)
        {
            List<Control> result = new List<Control>();
            result.Add(ctrl);

            if (ctrl.HasChildren)
            {
                foreach (var child in ctrl.Controls.OfType<Control>())
                    result.AddRange(child.GetAllChildControls());
            }

            return result;
        }
    }
}
