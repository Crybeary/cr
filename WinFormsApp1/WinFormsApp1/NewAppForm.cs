using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace CreditApp
{
    public class NewAppForm : Form
    {
        private ComboBox cmbClient;
        private TextBox txtAmount, txtTerm, txtPurpose;

        public NewAppForm()
        {
            InitializeComponent();
            LoadClients();
        }

        private void InitializeComponent()
        {
            Label lblClient = new Label { Text = "Клиент:", Location = new Point(20, 25), AutoSize = true };
            cmbClient = new ComboBox { Location = new Point(120, 22), Size = new Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblAmount = new Label { Text = "Сумма (руб):", Location = new Point(20, 65), AutoSize = true };
            txtAmount = new TextBox { Location = new Point(120, 62), Size = new Size(150, 23) };

            Label lblTerm = new Label { Text = "Срок (мес):", Location = new Point(20, 105), AutoSize = true };
            txtTerm = new TextBox { Location = new Point(120, 102), Size = new Size(80, 23) };

            Label lblPurpose = new Label { Text = "Цель:", Location = new Point(20, 145), AutoSize = true };
            txtPurpose = new TextBox { Location = new Point(120, 142), Size = new Size(250, 23) };

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(120, 190), Size = new Size(100, 35) };
            btnSave.Click += BtnSave_Click;

            Button btnCancel = new Button { Text = "Отмена", Location = new Point(240, 190), Size = new Size(100, 35) };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblClient, cmbClient, lblAmount, txtAmount, lblTerm, txtTerm, lblPurpose, txtPurpose, btnSave, btnCancel });

            Text = "Новая заявка";
            ClientSize = new Size(400, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadClients()
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseHelper.DbPath}");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT ClientID, FullName FROM Clients", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                cmbClient.Items.Add(new ClientItem { Id = reader.GetInt32(0), Name = reader.GetString(1) });

            cmbClient.DisplayMember = "Name";
            if (cmbClient.Items.Count > 0) cmbClient.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbClient.SelectedItem == null) { MessageBox.Show("Выберите клиента!"); return; }
            if (!decimal.TryParse(txtAmount.Text, out decimal amt) || amt <= 0) { MessageBox.Show("Неверная сумма!"); return; }
            if (!int.TryParse(txtTerm.Text, out int trm) || trm < 1) { MessageBox.Show("Неверный срок!"); return; }

            var client = (ClientItem)cmbClient.SelectedItem;
            using var conn = new SQLiteConnection($"Data Source={DatabaseHelper.DbPath}");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Applications (ClientID, Amount, TermMonths, Purpose, CreatedDate) VALUES (@cid, @amt, @trm, @pur, @dt)", conn);
            cmd.Parameters.AddWithValue("@cid", client.Id);
            cmd.Parameters.AddWithValue("@amt", amt);
            cmd.Parameters.AddWithValue("@trm", trm);
            cmd.Parameters.AddWithValue("@pur", txtPurpose.Text);
            cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();

            MessageBox.Show("Заявка сохранена!");
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class ClientItem { public int Id { get; set; } public string Name { get; set; } }
}