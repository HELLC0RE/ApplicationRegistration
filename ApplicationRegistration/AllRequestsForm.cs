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
    public partial class AllRequestsForm : Form
    {
        private string userLogin;
        private DataTable originalDataTable;
        public AllRequestsForm(string login)
        {
            InitializeComponent();
            this.userLogin = login;
            LoadRequests();
        }
        private void LoadRequests()
        {
            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    var command = new NpgsqlCommand(@"SELECT 
                request.id,
                request.full_name, 
                request.phone, 
                request.institution_name, 
                CASE
                    WHEN request.education_level_id = 1 THEN request.attestation_average::text
                    WHEN request.education_level_id = 4 THEN request.diploma_average::text
                    ELSE subjects.title || ': ' || request_subjects.score
                END AS education_info,
                request_status.title AS request_status,
                DATE(request.request_start) AS request_start,
                DATE(request.request_end) AS request_end,
                comment.comment AS Комментарий,
                request.executor_id AS Ответственный
                FROM 
                request
                LEFT JOIN 
                request_subjects ON request.id = request_subjects.request_id
                LEFT JOIN
                subjects ON request_subjects.subject_id = subjects.id
                LEFT JOIN
                request_status ON request.request_status_id = request_status.id
                LEFT JOIN
                comment ON request.id = comment.request_id
                WHERE
                request.education_level_id IN (1, 2, 3, 4)", conn);

                    var adapter = new NpgsqlDataAdapter(command);
                    originalDataTable = new DataTable();
                    adapter.Fill(originalDataTable);
                    dataGridViewRequests.DataSource = originalDataTable;
                    dataGridViewRequests.Columns["full_name"].HeaderText = "ФИО";
                    dataGridViewRequests.Columns["phone"].HeaderText = "Телефон";
                    dataGridViewRequests.Columns["institution_name"].HeaderText = "Место образования";
                    dataGridViewRequests.Columns["education_info"].HeaderText = "Предмет и баллы";
                    dataGridViewRequests.Columns["request_status"].HeaderText = "Статус заявки";
                    dataGridViewRequests.Columns["request_start"].HeaderText = "Дата начала обработки";
                    dataGridViewRequests.Columns["request_end"].HeaderText = "Дата окончания обработки";
                    dataGridViewRequests.Columns["Комментарий"].HeaderText = "Комментарий";
                    dataGridViewRequests.Columns["Ответственный"].HeaderText = "Ответственный";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }
        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            FilterRequestsByDate(dateTimePicker.Value);
        }

        private void FilterRequestsByDate(DateTime selectedDate)
        {
            string formattedDate = selectedDate.ToString("yyyy-MM-dd");
            (dataGridViewRequests.DataSource as DataTable).DefaultView.RowFilter = $"request_start = '{formattedDate}'";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int requestId = GetSelectedRequestId();
            if (requestId > 0)
            {
                EditRequest editForm = new EditRequest(userLogin, requestId);
                editForm.FormClosed += (s, args) => { ReloadRequests(); };
                editForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите заявку для редактирования.");
            }
        }
        private int GetSelectedRequestId()
        {
            if (dataGridViewRequests.SelectedRows.Count > 0)
            {
                return Convert.ToInt32(dataGridViewRequests.SelectedRows[0].Cells["id"].Value);
            }
            else
            {
                return 0;
            }
        }
        private void ReloadRequests()
        {
            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    var command = new NpgsqlCommand(@"SELECT 
                request.id,
                request.full_name, 
                request.phone, 
                request.institution_name, 
                CASE
                    WHEN request.education_level_id = 1 THEN request.attestation_average::text
                    WHEN request.education_level_id = 4 THEN request.diploma_average::text
                    ELSE subjects.title || ': ' || request_subjects.score
                END AS education_info,
                request_status.title AS request_status,
                DATE(request.request_start) AS request_start,
                DATE(request.request_end) AS request_end,
                comment.comment AS Комментарий,
                request.executor_id AS Ответственный
                FROM 
                request
                LEFT JOIN 
                request_subjects ON request.id = request_subjects.request_id
                LEFT JOIN
                subjects ON request_subjects.subject_id = subjects.id
                LEFT JOIN
                request_status ON request.request_status_id = request_status.id
                LEFT JOIN
                comment ON request.id = comment.request_id
                WHERE
                request.education_level_id IN (1, 2, 3, 4)", conn);

                    var adapter = new NpgsqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridViewRequests.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                dataGridViewRequests.DataSource = originalDataTable;
            }
            else
            {
                DataView dv = new DataView(originalDataTable);
                dv.RowFilter = $"id = '{searchText}'"; 
                dataGridViewRequests.DataSource = dv;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
