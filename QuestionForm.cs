using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared
{
    public partial class QuestionForm : Form
    {
        public QuestionForm()
        {
            InitializeComponent();
        }

        public string Caption { set { lbCaption.Text = value; } }

        public bool Required { get; set; }

        public string Result { get { return textBox1.Text; } }

        public new bool ShowDialog()
        {
            return base.ShowDialog() == System.Windows.Forms.DialogResult.OK;
        }
    }
}
