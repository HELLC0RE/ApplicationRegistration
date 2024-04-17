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
using System.Windows.Forms.VisualStyles;

namespace ApplicationRegistration
{
    public partial class EditRequest : Form
    {
        private string userLogin;
        private int requestId;
        public EditRequest(string login, int requestId)
        {
            InitializeComponent();
            this.userLogin = login;
            this.requestId = requestId;
            LoadStatuses();
        }
        private void LoadStatuses()
        {
            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    var command = new NpgsqlCommand("SELECT title FROM request_status", conn);
                    using (var reader = command.ExecuteReader())
                    {
                        comboBox1.Items.Clear();
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статусов: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newStatus = comboBox1.SelectedItem.ToString();
            string newComment = textBox1.Text;
            int newExecutor = FindExecutorId(userLogin); 

            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string getStatusIdQuery = "SELECT id FROM request_status WHERE title = @status";
                    int statusId;
                    using (var getStatusIdCommand = new NpgsqlCommand(getStatusIdQuery, conn))
                    {
                        getStatusIdCommand.Parameters.AddWithValue("@status", newStatus);
                        object result = getStatusIdCommand.ExecuteScalar();
                        if (result != null)
                        {
                            statusId = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("Статус с указанным названием не найден.");
                            return;
                        }
                    }
                    string updateCommentQuery;
                    if (string.IsNullOrEmpty(newComment))
                    {
                        updateCommentQuery = "INSERT INTO comment(request_id, comment) VALUES (@requestId, @comment)";
                    }
                    else
                    {
                        updateCommentQuery = "UPDATE comment SET comment = @comment WHERE request_id = @requestId";
                    }

                    using (var updateCommentCommand = new NpgsqlCommand(updateCommentQuery, conn))
                    {
                        updateCommentCommand.Parameters.AddWithValue("@requestId", requestId);
                        updateCommentCommand.Parameters.AddWithValue("@comment", newComment);

                        int commentRowsAffected = updateCommentCommand.ExecuteNonQuery();
                    }

                    using (var updateCommentCommand = new NpgsqlCommand(updateCommentQuery, conn))
                    {
                        updateCommentCommand.Parameters.AddWithValue("@comment", newComment);
                        updateCommentCommand.Parameters.AddWithValue("@requestId", requestId);

                        int commentRowsAffected = updateCommentCommand.ExecuteNonQuery();
                    }
                    string updateQuery = "UPDATE request SET request_status_id = @statusId, executor_id = @executor, request_end = NOW() WHERE id = @requestId";
                    using (var command = new NpgsqlCommand(updateQuery, conn))
                    {
                        command.Parameters.AddWithValue("@statusId", statusId);
                        command.Parameters.AddWithValue("@executor", newExecutor);
                        command.Parameters.AddWithValue("@requestId", requestId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Изменения успешно сохранены.");
                        }
                        else
                        {
                            MessageBox.Show("Изменения не были сохранены.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения изменений: " + ex.Message);
            }
            this.Close();
        }
        private int FindExecutorId(string login)
        {
            int executorId = -1; 

            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    string query = "SELECT id FROM users WHERE login = @login";
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            executorId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска идентификатора сотрудника: " + ex.Message);
            }

            return executorId;
        }
    }
}
