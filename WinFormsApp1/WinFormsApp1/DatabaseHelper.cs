using System;
using System.Data.SQLite;
using System.IO;

namespace CreditApp
{
    public static class DatabaseHelper
    {
        public static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credit.db");

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                using var conn = new SQLiteConnection($"Data Source={DbPath}");
                conn.Open();

                string sql = @"
                    CREATE TABLE Clients (
                        ClientID INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Phone TEXT NOT NULL,
                        Passport TEXT
                    );

                    CREATE TABLE Applications (
                        AppID INTEGER PRIMARY KEY AUTOINCREMENT,
                        ClientID INTEGER NOT NULL,
                        Amount REAL NOT NULL,
                        TermMonths INTEGER NOT NULL,
                        Purpose TEXT,
                        Status TEXT DEFAULT 'Новая',
                        CreatedDate TEXT NOT NULL,
                        FOREIGN KEY (ClientID) REFERENCES Clients(ClientID)
                    );

                    INSERT INTO Clients (FullName, Phone, Passport) VALUES 
                    ('Иванов Иван Иванович', '+79991234567', '4510 123456'),
                    ('Петрова Мария Сергеевна', '+79997654321', '4511 654321'),
                    ('ООО Ромашка', '+74951234567', 'ИНН 1234567890');

                    INSERT INTO Applications (ClientID, Amount, TermMonths, Purpose, Status, CreatedDate) VALUES 
                    (1, 500000, 36, 'Ремонт квартиры', 'Новая', '2026-04-01'),
                    (2, 1500000, 60, 'Покупка автомобиля', 'Новая', '2026-04-05'),
                    (3, 3000000, 120, 'Развитие бизнеса', 'Одобрена', '2026-03-20'),
                    (1, 200000, 24, 'Бытовая техника', 'Новая', date('now'));
                ";

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}