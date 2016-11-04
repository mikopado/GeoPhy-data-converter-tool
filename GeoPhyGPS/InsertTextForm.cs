using System.Windows.Forms;

namespace Geophy
{
    public partial class InsertTextForm : Form
    {
        private string inputData;
        DialogResult result;

        public string InputData
        {
            get { return inputData; }
            set
            {
                //if(!string.IsNullOrEmpty(value))
                    inputData = value;
            }
        }

        public InsertTextForm()
        {
            InitializeComponent();
        }
        public InsertTextForm(string title, string description)
        {
            InitializeComponent();
            this.Text = title;
            this.lblDescription.Text = description;
            

        }
       
        public new DialogResult Show()
        {            
            ShowDialog();
            return result;
            
        }

        

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {

            for (int i = 0; i < 4; i++)
            {
                if (e.KeyCode == Keys.Enter)
                {

                    InputData = txtInput.Text;
                    result = DialogResult.OK;
                    this.Close();

                }
            }
           
            
            
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {                
                InputData = txtInput.Text;
                result = DialogResult.OK;
                e.Handled = true;
                this.Close();
            }
        }

    }
}
