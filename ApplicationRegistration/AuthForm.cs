using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationRegistration
{
    public partial class AuthForm : Form
    {
        private string userLogin;
        public AuthForm()
        {
            InitializeComponent();
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            string login = loginBox.Text;
            string password = passBox.Text;
            try
            {
                int role = AuthenticateUser(login, password);
                if (role != 0)
                {
                    if (role == 1)
                    {
                        StudentRequestForm studentRequestForm = new StudentRequestForm(login);
                        studentRequestForm.Show();
                    }
                    if (role == 2)
                    {
                        AllRequestsForm allRequestsForm = new AllRequestsForm(login);
                        allRequestsForm.Show();
                    }
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка аутентификации: " + ex.Message);
            }
        }

        private int AuthenticateUser(string login, string password)
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("SELECT role_id FROM users WHERE login = @login AND pass = @pass", conn))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@pass", password);
                    var result = command.ExecuteScalar();
                    return (int)(result ?? 0);
                }
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show();
            this.Hide();
        }
    }
}
