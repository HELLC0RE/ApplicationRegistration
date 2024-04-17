using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationRegistration
{
    public partial class Stats : Form
    {
        public Stats()
        {
            InitializeComponent();
            UpdateCompletedRequestsLabel();
            UpdateAverageProcessingTimeLabel();
        }
        private void UpdateCompletedRequestsLabel()
        {
            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM request WHERE request_status_id = 3"; 
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        int completedCount = Convert.ToInt32(command.ExecuteScalar());
                        label3.Text = $"Количество выполненных заявок: {completedCount}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении количества выполненных заявок: " + ex.Message);
            }
        }
        private void UpdateAverageProcessingTimeLabel()
        {
            try
            {
                string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT AVG(EXTRACT(EPOCH FROM (request_end - request_start))) FROM request WHERE request_end IS NOT NULL"; 
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            double averageTimeInSeconds = Convert.ToDouble(result);
                            TimeSpan averageTimeSpan = TimeSpan.FromSeconds(averageTimeInSeconds);
                            label4.Text = $"Среднее время выполнения заявки: {averageTimeSpan.TotalHours:F2} часов";
                        }
                        else
                        {
                            label4.Text = "Среднее время выполнения заявки: Неизвестно";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении среднего времени выполнения заявок: " + ex.Message);
            }
        }
    }
}
