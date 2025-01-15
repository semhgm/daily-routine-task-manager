using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Dailyroutine1
{
    public partial class frmSignIn : Form
    {
        public frmSignIn()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            frmRegisterr frmRegisterr = new frmRegisterr();
            frmRegisterr.ShowDialog();
            this.Hide();
        }
        private void txtUserName_Leave(object sender, EventArgs e)
        {

        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {

        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string usernName = txtUserName.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(usernName) || string.IsNullOrEmpty(password))
            {
                errorProvider1.SetError(txtUserName, "Gerekli alanlar boş");
                errorProvider1.SetError(txtPassword, "Gerekli alanlar boş");
                txtUserName.Focus();
                return;
            }

            // Hata temizliği
            errorProvider1.Clear();

            // Bağlantı dizesi
            string connectionString = @"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;";

            // Sorguyu parametreli şekilde hazırlayın
            string query = "SELECT user_id FROM tblUser WHERE email = @Email AND password = @Password";

            try
            {
                // using bloğu içinde SqlConnection
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // Bağlantıyı aç

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Parametreleri tanımlayın
                        cmd.Parameters.AddWithValue("@Email", usernName);
                        cmd.Parameters.AddWithValue("@Password", password);

                        // SqlDataReader ile sorgu sonucu
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Kullanıcı doğrulandı, ID'yi al
                                string loggedInUserID = reader["user_id"].ToString();

                                // Ana formu aç ve kullanıcı ID'sini aktar
                                frmMain frmMain = new frmMain();
                                frmMain.CurrentUserID = loggedInUserID;
                                frmMain.Show();
                                this.Hide();
                            }
                            else
                            {
                                // Kullanıcı bulunamadı
                                MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
