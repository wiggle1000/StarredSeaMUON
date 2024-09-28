using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace StarredSeaMUON.Database
{
    internal class DBHelper : IDisposable
    {
        public SqliteConnection connection;

        public DBHelper()
        {
            connection = new SqliteConnection("Data Source=data.db");
            connection.Open();
        }
        public int ExecuteNonQuery(SqliteCommandBuilder cmd)
        {
            return ((SqliteCommand)cmd).ExecuteNonQuery();
        }
        public SqliteDataReader ExecuteReader(SqliteCommandBuilder cmd)
        {
            return ((SqliteCommand)cmd).ExecuteReader();
        }

        public bool CheckForUser(string userName)
        {
            SqliteDataReader r = ExecuteReader(new SqliteCommandBuilder(connection, "SELECT * from users WHERE name=@name")
                .WithTextParam("name", userName));
            return r.HasRows;
        }
        public SqliteDataReader GetUserReader(long userID)
        {
            return ExecuteReader(new SqliteCommandBuilder(connection, "SELECT * from users WHERE id=@id")
                .WithIntParam("id", userID));
        }
        public SqliteDataReader GetRoomReader(long roomID)
        {
            return ExecuteReader(new SqliteCommandBuilder(connection, "SELECT * from rooms WHERE id=@id")
                .WithIntParam("id", roomID));
        }
        public long GetUserID(string userName)
        {
            SqliteDataReader r = ExecuteReader(new SqliteCommandBuilder(connection, "SELECT * from users WHERE name=@name")
                .WithTextParam("name", userName));
            if(r.Read())
            {
                return (long)r.GetValue("id");
            }
            return -1;
        }
        public bool CheckLogin(string userName, string pass)
        {
            SqliteDataReader r = ExecuteReader(new SqliteCommandBuilder(connection, "SELECT * from users WHERE name=@name")
                .WithTextParam("name", userName));
            if (r.Read())
            {
                string passHash = (string)r.GetValue("passhash");
                byte[] savedHashBytes = Convert.FromBase64String(passHash);
                byte[] salt = new byte[32];
                Array.Copy(savedHashBytes, salt, 32);
                byte[] theseBytes = PasswordHelper.doHash(pass, salt);
                for (int i = 0; i < theseBytes.Length; i++)
                {
                    if (theseBytes[i] != savedHashBytes[i + 32])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public void CreateUser(string userName, string email, string pass)
        {
            string passHash = PasswordHelper.hashPassword(pass);
            ExecuteNonQuery(new SqliteCommandBuilder(connection, "INSERT INTO users ('name', 'email', 'passhash', 'created', 'perms') VALUES(@name, @email, @ph, @created, '');")
                .WithTextParam("name", userName).WithTextParam("email", email).WithTextParam("ph", passHash).WithTimeParam("created", DateTime.Now));
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
