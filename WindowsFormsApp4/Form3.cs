using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Entity.Core.Common.CommandTrees;

namespace WindowsFormsApp4
{
    public partial class Form3 : Form
    {
        private SQLiteConnection sqlConnection;

        public Form3()
        {
            InitializeComponent();
            userList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConnectToDatabase();
            searchInput.TextChanged += new EventHandler(search);
            loadUsers();
            delete.Click += new EventHandler(deleteSearchResults);

        }

        private void search(object sender, EventArgs e)
        {
            string text = searchInput.Text;
            searchUser(text);
        }

        private void searchUser(string user)
        {
            string query = "SELECT * FROM history WHERE name LIKE @user OR time_in LIKE @user OR id LIKE @user";
            using (SQLiteCommand command = new SQLiteCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@user", "%" + user + "%");
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    userList.DataSource = dt;
                }
            }
        }
        private void deleteSearchResults(object sender, EventArgs e)
        {
            string user = searchInput.Text;  // Use the current search input text

            string deleteQuery = "DELETE FROM history WHERE name LIKE @user OR time_in LIKE @user OR id LIKE @user";

            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, sqlConnection))
            {
                deleteCommand.Parameters.AddWithValue("@user", "%" + user + "%");

                int rowsAffected = deleteCommand.ExecuteNonQuery();

             }

            userList.DataSource = null;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
            this.Hide();
        }

        private void ConnectToDatabase()
        {
            string connectionString = "Data Source=history.db;Version=3;";
            sqlConnection = new SQLiteConnection(connectionString);
            sqlConnection.Open();
        }

        private void loadUsers()
        {
            string query = "SELECT * FROM history";
            using (SQLiteCommand command = new SQLiteCommand(query, sqlConnection))
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                userList.DataSource = dt;
            }
            userList.Columns["id"].ReadOnly = true;
            userList.Columns["time_in"].ReadOnly = true;
            userList.Columns["name"].ReadOnly = true;
        }
        private void deleteChanges()
        {
            try
            {
                // Ensure DataTable is valid and bound to the DataGridView
                DataTable dataTable = userList.DataSource as DataTable;
                if (dataTable == null)
                {
                    MessageBox.Show("Data source is not set or is not a DataTable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for changes
                DataTable changes = dataTable.GetChanges();
                if (changes == null)
                {
                    // No changes to save
                    return;
                }

                using (SQLiteTransaction transaction = sqlConnection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(sqlConnection))
                    {
                        foreach (DataRow row in changes.Rows)
                        {
                            switch (row.RowState)
                            {
                                case DataRowState.Modified:
                                    command.CommandText = @"UPDATE history 
                                                            SET name = @name, time_in = @time_in, time_out = @time_out 
                                                            WHERE id = @id";
                                    command.Parameters.AddWithValue("@id", row["id"]);
                                    command.Parameters.AddWithValue("@name", row["name"]);
                                    command.Parameters.AddWithValue("@time_in", row["time_in"]);
                                    command.Parameters.AddWithValue("@time_out", row["time_out"]);
                                    command.ExecuteNonQuery();
                                    break;

                                case DataRowState.Added:
                                    command.CommandText = @"INSERT INTO history (name, time_in, time_out) 
                                                            VALUES (@name, @time_in, @time_out)";
                                    command.Parameters.AddWithValue("@name", row["name"]);
                                    command.Parameters.AddWithValue("@time_in", row["time_in"]);
                                    command.Parameters.AddWithValue("@time_out", row["time_out"]);
                                    command.ExecuteNonQuery();
                                    break;

                                case DataRowState.Deleted:
                                    command.CommandText = @"DELETE FROM history WHERE id = @id";
                                    command.Parameters.AddWithValue("@id", row["id", DataRowVersion.Original]);
                                    command.ExecuteNonQuery();
                                    break;
                            }
                            command.Parameters.Clear();
                        }
                    }
                    transaction.Commit();
                }

                // Accept the changes in the DataTable after successful update
                dataTable.AcceptChanges();

                // Reload the data to refresh the DataGridView
                loadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        private void saveChanges()
        {
            try
            {
                // Ensure DataTable is valid and bound to the DataGridView
                DataTable dataTable = userList.DataSource as DataTable;
                if (dataTable == null)
                {
                    MessageBox.Show("Data source is not set or is not a DataTable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check for changes
                DataTable changes = dataTable.GetChanges();
                if (changes == null)
                {
                    // No changes to save
                    return;
                }

                using (SQLiteTransaction transaction = sqlConnection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(sqlConnection))
                    {
                        foreach (DataRow row in changes.Rows)
                        {
                            switch (row.RowState)
                            {
                                case DataRowState.Modified:
                                    command.CommandText = @"UPDATE history 
                                                            SET name = @name, time_in = @time_in, time_out = @time_out 
                                                            WHERE id = @id";
                                    command.Parameters.AddWithValue("@id", row["id"]);
                                    command.Parameters.AddWithValue("@name", row["name"]);
                                    command.Parameters.AddWithValue("@time_in", row["time_in"]);
                                    command.Parameters.AddWithValue("@time_out", row["time_out"]);
                                    command.ExecuteNonQuery();
                                    break;

                                case DataRowState.Added:
                                    command.CommandText = @"INSERT INTO history (name, time_in, time_out) 
                                                            VALUES (@name, @time_in, @time_out)";
                                    command.Parameters.AddWithValue("@name", row["name"]);
                                    command.Parameters.AddWithValue("@time_in", row["time_in"]);
                                    command.Parameters.AddWithValue("@time_out", row["time_out"]);
                                    command.ExecuteNonQuery();
                                    break;

                                case DataRowState.Deleted:
                                    command.CommandText = @"DELETE FROM history WHERE id = @id";
                                    command.Parameters.AddWithValue("@id", row["id", DataRowVersion.Original]);
                                    command.ExecuteNonQuery();
                                    break;
                            }
                            command.Parameters.Clear();
                        }
                    }
                    transaction.Commit();
                }

                // Accept the changes in the DataTable after successful update
                dataTable.AcceptChanges();

                // Reload the data to refresh the DataGridView
                loadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveChanges();
            Form2 form = new Form2();
            form.Show();
            this.Hide();
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Any additional initialization code can go here
        }

        private void save_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        private void delete_Click(object sender, EventArgs e)
        {

        }
    }
}