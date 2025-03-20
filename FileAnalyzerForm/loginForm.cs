using System;
using System.Windows.Forms;

namespace FileAnalyzerForm
{
    public partial class loginForm: Form
    {
        public loginForm()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Methods sınıfının bir örneğini oluşturuyoruz
            Methods methods = new Methods(this);

            // Login metodunu çağırıyoruz
            string result = methods.Login(username, password, this);

            if (result == "Giriş başarılı")
            {
                this.DialogResult = DialogResult.OK; // Giriş başarılıysa formu kapat
            }
        }


    }
}