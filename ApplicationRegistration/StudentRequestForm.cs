using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationRegistration
{
    public partial class StudentRequestForm : Form
    {
        private string userLogin;
        private PrintDocument printDocument = new PrintDocument();
        public StudentRequestForm(string login)
        {
            InitializeComponent();
            this.userLogin = login;
            LoadRequestsByLogin(userLogin);
            label2.Text = userLogin;
        }
        private void LoadRequestsByLogin(string login)
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"SELECT request.id, 
                                        education_level.title AS education_level, 
                                        request_status.title AS request_status, 
                                        request.request_start, 
                                        request.request_end,
                                        COALESCE(comment.comment, '') AS comment
                                  FROM request
                                  INNER JOIN users ON request.user_id = users.id
                                  INNER JOIN education_level ON request.education_level_id = education_level.id
                                  INNER JOIN request_status ON request.request_status_id = request_status.id
                                  LEFT JOIN comment ON request.id = comment.request_id
                                  WHERE  users.login = @login", conn);
                command.Parameters.AddWithValue("@login", login);

                var adapter = new NpgsqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridViewRequests.DataSource = dataTable;
                dataGridViewRequests.Columns["id"].HeaderText = "ID";
                dataGridViewRequests.Columns["education_level"].HeaderText = "Уровень образования";
                dataGridViewRequests.Columns["request_status"].HeaderText = "Статус заявки";
                dataGridViewRequests.Columns["request_start"].HeaderText = "Дата начала заявки";
                dataGridViewRequests.Columns["request_end"].HeaderText = "Дата окончания заявки";
                dataGridViewRequests.Columns["comment"].HeaderText = "Комментарий";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridViewRequests.SelectedRows.Count > 0)
            {
                int selectedRowID = (int)dataGridViewRequests.SelectedRows[0].Cells["id"].Value;
                string login = userLogin;

                printDocument.PrintPage += (s, ev) =>
                {
                    int startX = 10;
                    int startY = 10;
                    int offset = 20;
                    Graphics g = ev.Graphics;

                    string fullName = GetStudentFullName(selectedRowID, login);
                    List<string> educationAreas = GetEducationAreas(selectedRowID, login);

                    // Measure string width
                    SizeF textSize = g.MeasureString("ЗАЯВЛЕНИЕ", new Font("Arial", 14, FontStyle.Bold));
                    int headerX = (int)((ev.PageBounds.Width - textSize.Width) / 2); // Calculate X position for centering

                    // Draw document header
                    g.DrawString("ЗАЯВЛЕНИЕ", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, headerX, startY);
                    startY += offset * 2;

                    // Draw student's full name
                    g.DrawString($"Я, {fullName}, подал документы в ВУЗ", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, startX, startY);
                    startY += offset * 2;

                    // Draw education areas
                    g.DrawString("На направления подготовки:", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, startX, startY);
                    startY += offset;
                    foreach (string area in educationAreas)
                    {
                        g.DrawString(area, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, startX, startY);
                        startY += offset;
                    }

                    // Draw date
                    startY += offset * 2;
                    g.DrawString(DateTime.Now.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, startX, startY);
                };

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            else
            {
                MessageBox.Show("Выберите заявление для печати.");
            }
        }
        private string GetStudentFullName(int requestId, string login)
        {
            string fullName = "";
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"SELECT full_name 
                                          FROM users 
                                          INNER JOIN request ON request.user_id = users.id
                                          WHERE request.id = @requestId AND users.login = @login", conn);
                command.Parameters.AddWithValue("@requestId", requestId);
                command.Parameters.AddWithValue("@login", login);

                fullName = command.ExecuteScalar()?.ToString();
            }

            return fullName;
        }

        private List<string> GetEducationAreas(int requestId, string login)
        {
            List<string> educationAreas = new List<string>();
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"SELECT education_area.title 
                                          FROM request 
                                          INNER JOIN education_level ON request.education_level_id = education_level.id
                                          INNER JOIN education_area ON education_level.id = education_area.id
                                          INNER JOIN users ON request.user_id = users.id
                                          WHERE request.id = @requestId AND users.login = @login", conn);
                command.Parameters.AddWithValue("@requestId", requestId);
                command.Parameters.AddWithValue("@login", login);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        educationAreas.Add(reader["title"].ToString());
                    }
                }
            }

            return educationAreas;
        }

    }
}
