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
    public partial class RegistrationForm : Form
    {
        private string studentFullName;
        private string parentFullName;
        private string passportData;
        private string snils;
        private string phoneNumber;
        private string email;
        private string password;
        private string institutionName;
        private int attestatAverage;
        private int diplomAverage;
        private string filePath;
        private string selectedEducationLevel;
        private string login;
        public RegistrationForm()
        {
            InitializeComponent();
            LoadEducationLevels();
            LoadSubjects();
            egePanel.Visible = false;
            attestatPanel.Visible = false;
            diplomPanel.Visible = false;
            specialtiesCheckedListBox.ItemCheck += specialtiesCheckedListBox_ItemCheck;
            phoneNumberBox.KeyPress += TextBox_KeyPress_DigitsOnly;
            snilsBox.KeyPress += TextBox_KeyPress_DigitsOnly;
        }

        private void TextBox_KeyPress_DigitsOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
        private void LoadEducationLevels()
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var command = new NpgsqlCommand("SELECT title FROM education_level", conn);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        educationLevelBox.Items.Add(title);
                    }
                }
            }
        }
        private void LoadSubjects()
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var command = new NpgsqlCommand("SELECT title FROM subjects", conn);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        subjectsBox.Items.Add(title);
                    }
                }
            }
        }
        private bool AreTextBoxesFilled()
        {
            if (string.IsNullOrWhiteSpace(fullNameStudentBox.Text) ||
                string.IsNullOrWhiteSpace(fullNameParentBox.Text) ||
                string.IsNullOrWhiteSpace(passportBox.Text) ||
                string.IsNullOrWhiteSpace(snilsBox.Text) ||
                string.IsNullOrWhiteSpace(phoneNumberBox.Text) ||
                string.IsNullOrWhiteSpace(emailBox.Text) ||
                string.IsNullOrWhiteSpace(passwordBox.Text) ||
                string.IsNullOrWhiteSpace(institutionNameBox.Text))
            {
                return false; 
            }

            return true; 
        }
        private void educationLevelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedEducationLevel = educationLevelBox.SelectedItem.ToString();
            string selectedLevel = educationLevelBox.SelectedItem.ToString();
            specialtiesCheckedListBox.Items.Clear();
            switch (selectedLevel)
            {
                case "СПО":
                    attestatPanel.Visible = true;
                    egePanel.Visible = false;
                    diplomPanel.Visible = false;
                    break;
                case "Бакалавриат":
                    attestatPanel.Visible = false;
                    egePanel.Visible = true;
                    diplomPanel.Visible = false;
                    break;
                case "Специалитет":
                    attestatPanel.Visible = false;
                    egePanel.Visible = true;
                    diplomPanel.Visible = false;
                    break;
                case "Магистратура":
                    attestatPanel.Visible = false;
                    egePanel.Visible = false;
                    diplomPanel.Visible = true;
                    break;
            }
            switch (selectedLevel)
            {
                case "СПО":
                    specialtiesCheckedListBox.Items.Add("11.01.01 Монтажник радиоэлектронной аппаратуры и приборов");
                    specialtiesCheckedListBox.Items.Add("08.02.01 Строительство и эксплуатация зданий и сооружений");
                    specialtiesCheckedListBox.Items.Add("08.02.05 Строительство и эксплуатация автомобильных дорог и аэродромов");
                    specialtiesCheckedListBox.Items.Add("08.02.08 Монтаж и эксплуатация оборудования и систем газоснабжения");
                    specialtiesCheckedListBox.Items.Add("08.02.13 Монтаж и эксплуатация внутренних сантехнических устройств, кондиционирования воздуха и вентиляции");
                    specialtiesCheckedListBox.Items.Add("09.02.01 Компьютерные системы и комплексы");
                    break;
                case "Бакалавриат":
                    specialtiesCheckedListBox.Items.Add("07.03.01 Архитектура");
                    specialtiesCheckedListBox.Items.Add("07.03.02 Реконструкция и реставрация архитектурного наследия");
                    specialtiesCheckedListBox.Items.Add("07.03.03 Дизайн архитектурной среды");
                    specialtiesCheckedListBox.Items.Add("07.03.04 Градостроительство");
                    break;
                case "Специалитет":
                    specialtiesCheckedListBox.Items.Add("10.05.02 Информационная безопасность телекоммуникационных систем");
                    specialtiesCheckedListBox.Items.Add("11.05.01 Радиоэлектронные системы и комплексы");
                    specialtiesCheckedListBox.Items.Add("24.05.02 Проектирование авиационных и ракетных двигателей");
                    specialtiesCheckedListBox.Items.Add("24.05.07 Самолето- и вертолетостроение");

                    break;
                case "Магистратура":
                    specialtiesCheckedListBox.Items.Add("07.04.01 Архитектура");
                    specialtiesCheckedListBox.Items.Add("07.04.02 Реконструкция и реставрация архитектурного наследия");
                    specialtiesCheckedListBox.Items.Add("07.04.03 Дизайн архитектурной среды");
                    break;
                default:
                    break;
            }
        }
        private void specialtiesCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked && specialtiesCheckedListBox.CheckedItems.Count >= 5)
            {
                MessageBox.Show("Максимальное количество выбранных специальностей - 5.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.NewValue = CheckState.Unchecked;
            }
        }
        private void egeScore_Click(object sender, EventArgs e)
        {
            if (subjectsBox.SelectedItem != null)
            {
                string selectedSubject = subjectsBox.SelectedItem.ToString();
                int egePoints = (int)egeBox.Value;
                if (egePoints <= 100)
                {
                    AddSubjectToGrid(selectedSubject, egePoints);
                }
                else
                {
                    MessageBox.Show("Количество очков не может превышать 100.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите предмет.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddSubjectToGrid(string subject, int points)
        {
            foreach (DataGridViewRow row in subjectsDataGrid.Rows)
            {
                if (row.Cells["Subject"].Value != null && row.Cells["Subject"].Value.ToString() == subject)
                {
                    MessageBox.Show("Предмет уже добавлен.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (subjectsDataGrid.Rows.Count > 4)
            {
                MessageBox.Show("Вы уже добавили максимальное количество предметов (4).", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            subjectsDataGrid.Rows.Add(subject, points);
        }

        private void attestatScore_Click(object sender, EventArgs e)
        {
            int attestatPoints = (int)attestatBox.Value;

            if (attestatPoints > 5)
            {
                MessageBox.Show("Средний балл аттестата не может быть больше 5.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            attestatAverage = attestatPoints;
            MessageBox.Show("Средний балл аттестата сохранен.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void diplomScore_Click(object sender, EventArgs e)
        {
            int diplomPoints = (int)diplomBox.Value;

            if (diplomPoints > 5)
            {
                MessageBox.Show("Средний балл диплома не может быть больше 5.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            diplomAverage = diplomPoints;
            MessageBox.Show("Средний балл диплома сохранен.", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите скан документа об образовании";
            openFileDialog.Filter = "Файлы PDF (*.pdf)|*.pdf";
            openFileDialog.Multiselect = false; 
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
        }
        private bool IsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsPasswordValid(string password)
        {
            return password.Length >= 8;
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber.All(char.IsDigit);
        }

        private bool AreSubjectsSelected()
        {
            if (selectedEducationLevel == "Бакалавриат" || selectedEducationLevel == "Специалитет")
            {
                return subjectsDataGrid.Rows.Count > 1;
            }
            return true;
        }

        private bool IsDocumentUploaded()
        {
            return !string.IsNullOrWhiteSpace(filePath);
        }

        private bool AreSpecialtiesSelected()
        {
            if (selectedEducationLevel == "СПО" || selectedEducationLevel == "Магистратура")
            {
                return specialtiesCheckedListBox.CheckedItems.Count > 0;
            }
            return true;
        }
        private void createRequestButton_Click(object sender, EventArgs e)
        {
            if (!AreTextBoxesFilled())
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsEmailValid(emailBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный адрес электронной почты.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsPasswordValid(passwordBox.Text))
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsPhoneNumberValid(phoneNumberBox.Text))
            {
                MessageBox.Show("Номер телефона должен содержать только цифры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!AreSubjectsSelected())
            {
                MessageBox.Show("Пожалуйста, выберите уровень образования", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsDocumentUploaded())
            {
                MessageBox.Show("Пожалуйста, загрузите скан документа об образовании.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!AreSpecialtiesSelected())
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы одну специальность.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            studentFullName = fullNameStudentBox.Text;
            parentFullName = fullNameParentBox.Text;
            passportData = passportBox.Text;
            snils = snilsBox.Text;
            phoneNumber = phoneNumberBox.Text;
            email = emailBox.Text;
            password = passwordBox.Text;
            institutionName = institutionNameBox.Text;
            login = loginBox.Text;

            string selectedEducationLevel = educationLevelBox.SelectedItem.ToString();
            List<string> selectedSpecialties = new List<string>();
            foreach (var item in specialtiesCheckedListBox.CheckedItems)
            {
                selectedSpecialties.Add(item.ToString());
            }

            int? attestationAverage = null;
            int? diplomaAverage = null;
            switch (selectedEducationLevel)
            {
                case "СПО":
                    attestationAverage = (int)attestatBox.Value;
                    break;
                case "Магистратура":
                    diplomaAverage = (int)diplomBox.Value;
                    break;
                default:
                    break;
            }

            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO users (login, pass, role_id) VALUES (@login, @pass, @roleId)", conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@roleId", 1);
                    cmd.ExecuteNonQuery();
                }
                var selectUserId = new NpgsqlCommand("SELECT currval('users_id_seq')", conn);
                int userId = Convert.ToInt32(selectUserId.ExecuteScalar());
                var insertCommand = new NpgsqlCommand("INSERT INTO request (education_level_id, full_name, passport_data, snils, email, phone, parent_full_name, institution_name, " +
                    "attestation_average, diploma_average, document_scan_path, request_status_id, request_start, user_id) VALUES (@educationLevelId, @fullName, @passportData, @snils, @email, " +
                    "@phone, @parentFullName, @institutionName, @attestationAverage, @diplomaAverage, @documentScanPath, @requestStatusId, @requestStart, @user_id)", conn);
                insertCommand.Parameters.AddWithValue("@educationLevelId", GetEducationLevelId(selectedEducationLevel));
                insertCommand.Parameters.AddWithValue("@fullName", studentFullName);
                insertCommand.Parameters.AddWithValue("@passportData", passportData);
                insertCommand.Parameters.AddWithValue("@snils", snils);
                insertCommand.Parameters.AddWithValue("@email", email);
                insertCommand.Parameters.AddWithValue("@phone", phoneNumber);
                insertCommand.Parameters.AddWithValue("@parentFullName", parentFullName);
                insertCommand.Parameters.AddWithValue("@institutionName", institutionName);
                insertCommand.Parameters.AddWithValue("@attestationAverage", (object)attestationAverage ?? DBNull.Value);
                insertCommand.Parameters.AddWithValue("@diplomaAverage", (object)diplomaAverage ?? DBNull.Value);
                insertCommand.Parameters.AddWithValue("@documentScanPath", filePath);
                insertCommand.Parameters.AddWithValue("@requestStatusId", 1);
                insertCommand.Parameters.AddWithValue("@requestStart", DateTime.Now);
                insertCommand.Parameters.AddWithValue("@user_id", userId);
                insertCommand.ExecuteNonQuery();

                var selectCommand = new NpgsqlCommand("SELECT currval('request_id_seq')", conn);
                int requestId = Convert.ToInt32(selectCommand.ExecuteScalar());
                if (selectedEducationLevel == "Бакалавриат" || selectedEducationLevel == "Специалитет")
                {
                    int rowCount = subjectsDataGrid.Rows.Count;
                    for (int i = 0; i < rowCount - 1; i++)
                    {
                        DataGridViewRow row = subjectsDataGrid.Rows[i];
                        string subject = row.Cells["Subject"].Value.ToString();
                        subject = Convert.ToString(GetSubjectId(subject));
                        int score = Convert.ToInt32(row.Cells["Score"].Value);

                        var insertSubjectCommand = new NpgsqlCommand("INSERT INTO request_subjects (request_id, subject_id, score) VALUES (@requestId, @subjectId, @score)", conn);
                        insertSubjectCommand.Parameters.AddWithValue("@requestId", requestId);
                        insertSubjectCommand.Parameters.AddWithValue("@subjectId", Convert.ToInt32(subject));
                        insertSubjectCommand.Parameters.AddWithValue("@score", score);
                        insertSubjectCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    foreach (var specialty in specialtiesCheckedListBox.CheckedItems)
                    {
                        int educationAreaId = GetEducationAreaId(specialty.ToString());

                        using (var cmd = new NpgsqlCommand("INSERT INTO request_education_area (request_id, education_area_id) VALUES (@requestId, @educationAreaId)", conn))
                        {
                            cmd.Parameters.AddWithValue("@requestId", requestId);
                            cmd.Parameters.AddWithValue("@educationAreaId", educationAreaId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Заявка успешно создана. Чтобы войти в личный кабинет используйте указанный логин и пароль при регистрации.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AuthForm authForm = new AuthForm();
                authForm.Show();
                this.Hide();
            }
        }
        private int GetEducationLevelId(string educationLevelTitle)
        {
            switch (educationLevelTitle)
            {
                case "СПО":
                    return 1;
                case "Бакалавриат":
                    return 2;
                case "Специалитет":
                    return 3;
                case "Магистратура":
                    return 4;
            }
            return 0;
        }
        private int GetEducationAreaId(string specialty)
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            int educationAreaId = -1; 

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT id FROM education_area WHERE title = @specialty";
                using (var command = new NpgsqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@specialty", specialty);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        educationAreaId = Convert.ToInt32(result);
                    }
                }
            }

            return educationAreaId;
        }
        private int GetSubjectId(string subject)
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=qwerty123;Database=RequestsDB;";
            int subjectId = 0;

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT id FROM subjects WHERE title = @subject";
                using (var command = new NpgsqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@subject", subject);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        subjectId = Convert.ToInt32(result);
                    }
                }
            }

            return subjectId;
        }
            
    }
}
