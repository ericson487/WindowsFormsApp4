using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            ConnectToDatabase();
            initTable();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string name = nameInput.Text;
            string timeIn = timeInput.Text;
            string id = idInput.Text;
            int a = Convert.ToInt32(id);
            insertUser(name, timeIn, a); 
            
            Form3 form = new Form3();
            form.Show();
            this.Hide();
        }
        

        private SQLiteConnection sqlConnection;

        private void ConnectToDatabase()
        {
            string connectionString = "Data Source=history.db;Version=3;";
            sqlConnection = new SQLiteConnection(connectionString);
            sqlConnection.Open();
        }
        private void initTable()
        {
            string query = "CREATE TABLE IF NOT EXISTS history(id INTEGER, name TEXT, time_in TEXT, time_out TEXT)";
            using (SQLiteCommand command = new SQLiteCommand(query, sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }
        private void insertUser(string name , string timeIn, int id){
            string query = "INSERT INTO history(id,name,time_in) VALUES (@id, @name, @timeIn)";
            using (SQLiteCommand command = new SQLiteCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@timeIn", timeIn);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form4 form = new Form4();
            form.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.Show();
            this.Hide();
        }

        private void timeInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
