using System;
using System.Drawing;
using System.Windows.Forms;

namespace CreditApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Button btnNew = new Button
            {
                Text = "📝 Новая заявка",
                Location = new Point(100, 50),
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 10)
            };
            btnNew.Click += (s, e) => new NewAppForm().ShowDialog();

            Button btnReport = new Button
            {
                Text = "📊 Отчёт и экспорт XML",
                Location = new Point(100, 120),
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 10)
            };
            btnReport.Click += (s, e) => new ReportForm().ShowDialog();

            Controls.Add(btnNew);
            Controls.Add(btnReport);

            Text = "Кредитование";
            ClientSize = new Size(400, 250);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}