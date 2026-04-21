using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CreditApp
{
    public class ReportForm : Form
    {
        private DateTimePicker dtpFrom, dtpTo;
        private DataGridView dgv;
        private Label lblCount;
        private List<App> apps;

        public ReportForm()
        {
            InitializeComponent();
            dtpFrom.Value = DateTime.Now.AddDays(-30);
            dtpTo.Value = DateTime.Now;
            LoadData();
        }

        private void InitializeComponent()
        {
            Label lblFrom = new Label { Text = "С:", Location = new Point(15, 20), AutoSize = true };
            dtpFrom = new DateTimePicker { Location = new Point(35, 17), Size = new Size(100, 23), Format = DateTimePickerFormat.Short };
            Label lblTo = new Label { Text = "По:", Location = new Point(145, 20), AutoSize = true };
            dtpTo = new DateTimePicker { Location = new Point(170, 17), Size = new Size(100, 23), Format = DateTimePickerFormat.Short };

            Button btnLoad = new Button { Text = "Загрузить", Location = new Point(290, 15), Size = new Size(90, 28) };
            btnLoad.Click += (s, e) => LoadData();

            dgv = new DataGridView
            {
                Location = new Point(15, 55),
                Size = new Size(550, 280),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.Columns.Add("Id", "№");
            dgv.Columns.Add("Client", "Клиент");
            dgv.Columns.Add("Amount", "Сумма");
            dgv.Columns.Add("Term", "Срок");
            dgv.Columns.Add("Purpose", "Цель");
            dgv.Columns.Add("Status", "Статус");
            dgv.Columns.Add("Date", "Дата");

            lblCount = new Label { Text = "Найдено: 0", Location = new Point(15, 350), AutoSize = true };

            Button btnExport = new Button { Text = "📄 Экспорт XML", Location = new Point(330, 345), Size = new Size(110, 35) };
            btnExport.Click += BtnExport_Click;

            Button btnClose = new Button { Text = "Закрыть", Location = new Point(455, 345), Size = new Size(110, 35) };
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblFrom, dtpFrom, lblTo, dtpTo, btnLoad, dgv, lblCount, btnExport, btnClose });

            Text = "Отчёт по заявкам";
            ClientSize = new Size(580, 400);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadData()
        {
            apps = new List<App>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseHelper.DbPath}");
            conn.Open();
            using var cmd = new SQLiteCommand(@"SELECT a.AppID, c.FullName, a.Amount, a.TermMonths, a.Purpose, a.Status, a.CreatedDate
                FROM Applications a JOIN Clients c ON a.ClientID = c.ClientID
                WHERE date(a.CreatedDate) BETWEEN date(@from) AND date(@to) ORDER BY a.CreatedDate DESC", conn);
            cmd.Parameters.AddWithValue("@from", dtpFrom.Value.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@to", dtpTo.Value.ToString("yyyy-MM-dd"));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                apps.Add(new App { Id = reader.GetInt32(0), Client = reader.GetString(1), Amount = reader.GetDecimal(2), Term = reader.GetInt32(3), Purpose = reader.IsDBNull(4) ? "" : reader.GetString(4), Status = reader.GetString(5), Date = reader.GetString(6) });

            dgv.Rows.Clear();
            foreach (var a in apps)
                dgv.Rows.Add(a.Id, a.Client, a.Amount.ToString("N0") + " ₽", a.Term + " мес", a.Purpose, a.Status, a.Date);
            lblCount.Text = $"Найдено: {apps.Count} | Сумма: {apps.Sum(a => a.Amount):N0} ₽";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (apps.Count == 0) { MessageBox.Show("Нет данных!"); return; }
            using SaveFileDialog sfd = new SaveFileDialog { Filter = "XML|*.xml", FileName = $"Заявки_{DateTime.Now:yyyyMMdd}.xml" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                new XmlSerializer(typeof(List<App>)).Serialize(File.Create(sfd.FileName), apps);
                MessageBox.Show($"Сохранено:\n{sfd.FileName}");
            }
        }
    }

    [Serializable] public class App { public int Id { get; set; } public string Client { get; set; } public decimal Amount { get; set; } public int Term { get; set; } public string Purpose { get; set; } public string Status { get; set; } public string Date { get; set; } }
}