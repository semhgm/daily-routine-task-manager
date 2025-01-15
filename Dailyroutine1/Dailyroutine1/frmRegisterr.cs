using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dailyroutine1
{
    public partial class frmRegisterr : Form
    {
        public frmRegisterr()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtName_Leave(object sender, EventArgs e)
        {

        }

        private void txtUserName_Leave(object sender, EventArgs e)
        {

        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {

            frmSignIn frmSignIn = new frmSignIn();
            frmSignIn.ShowDialog();
            this.Hide();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan alınan bilgiler
            string nameSurname = txtName.Text;
            string userName = txtUserName.Text;
            string password = txtPassword.Text;

            // Form alanlarının boş olup olmadığını kontrol et
            if (string.IsNullOrEmpty(nameSurname) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bağlantı dizesi
            string connectionString = @"Data Source=SEMIH\SQLEXPRESS;Initial Catalog=DailyRoutineDb1;Integrated Security=True;";

            // SQL sorgusu (Parametreli)
            string query = "INSERT INTO tblUser (name_surname, email, password) VALUES (@NameSurname, @UserName, @Password)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // Bağlantıyı aç

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Parametreleri ekle
                        cmd.Parameters.AddWithValue("@NameSurname", nameSurname);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Password", password);

                        // Sorguyu çalıştır
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); // Formu kapat
                            frmSignIn frmSignIn = new frmSignIn();
                            frmSignIn.Show();
                        }
                        else
                        {
                            MessageBox.Show("Kayıt başarısız oldu. Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Benzersiz kullanıcı adı hatasını ele al
                if (ex.Number == 2627) // Unique constraint violation
                {
                    MessageBox.Show("Bu kullanıcı adı zaten mevcut. Lütfen başka bir kullanıcı adı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
    }
}
