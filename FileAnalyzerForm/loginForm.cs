using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileAnalyzerForm
{
    public partial class loginForm: Form
    {
        private loginForm lF;
        private Methods m;
        public loginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lF = new loginForm();
            m = new Methods(lF);
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            string result = m.login(username, password, this);  // 'this' ile formu geçiriyoruz
        }
    }
}
