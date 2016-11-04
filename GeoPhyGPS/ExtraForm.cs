using System;
using System.Windows.Forms;

namespace Geophy
{
    public partial class CustomMessageForm : Form
    {
        public CustomMessageForm()
        {
            InitializeComponent();
        }       
       
        public CustomMessageForm(string title, string description, string leftButt, string rightButt)
        {
            InitializeComponent();
            this.Text = title;
            this.lblMessage.Text = description;
            this.btnAut.Text = leftButt;
            this.btnMan.Text = rightButt;
           
            
        }
        

        /// <summary>
        /// Your custom message box helper.
        /// </summary>
        public static class CustomMessageBox
        {
            public static DialogResult Show(string title, string description, string leftButt, string rightButt)
            {
                // using construct ensures the resources are freed when form is closed
                using (var form = new CustomMessageForm(title, description, leftButt, rightButt))
                {
                    
                    form.btnAut.DialogResult = DialogResult.Yes;
                    form.btnMan.DialogResult = DialogResult.No;
                    return form.ShowDialog();
                }
                
            }
         
        }

        private void btnAut_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMan_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
