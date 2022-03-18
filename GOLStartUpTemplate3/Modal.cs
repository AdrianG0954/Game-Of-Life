using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate3
{
    public partial class Modal : Form
    {
        public Modal()
        {
            InitializeComponent();
        }

        public int TimeInterval
        {
            get
            {
                return (int)numericUpDown1.Value;
            }

            set
            {
                numericUpDown1.Value = value;
            }
            
        }

        public int WidthCell
        {
            get
            {
                return (int)numericUpDown2.Value;
            }
            set
            {
                numericUpDown2.Value = value;
            }
        }

        public int HeightCell
        {
            get
            {
                return (int)numericUpDown3.Value;
            }
            set
            {
                numericUpDown3.Value = value;
            }
        }
    }
}
