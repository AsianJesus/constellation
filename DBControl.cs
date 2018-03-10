using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CanSat_Desktop
{
    class DBControl
    {
        private MySqlConnection connection;
        private MySqlCommand command;
        string serv, user, passw, db;
        public DBControl(string server, string uid, string password, string database)
        {
            serv = server;
            user = uid;
            passw = password;
            db = database;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = serv;
            stringBuilder.UserID = user;
            stringBuilder.Password = passw;
            stringBuilder.Database = db;
            connection = new MySqlConnection(stringBuilder.GetConnectionString(false));
        }
        public DBControl(DBControl db)
        {
            this.serv = db.serv;
            this.user = db.user;
            this.passw = db.passw;
            this.db = db.db;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = serv;
            stringBuilder.Password = passw;
            stringBuilder.UserID = user;
            stringBuilder.Database = this.db;
            connection = new MySqlConnection(stringBuilder.GetConnectionString(false));
        }
        ~DBControl()
        { 
            command.Cancel();
            connection.Close();
        }
        public bool Open()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Close()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void TryAdd(string key)
        {
            if (command.Parameters.IndexOf(key) == -1)
                command.Parameters.Add(new MySqlParameter(key,new object()));
        }
        public void SetParameters(List<string> keys)
        {
            foreach(string key in keys)
            {
                TryAdd(key);
            }
        }
        public void SetVariables<TVal>(Dictionary<string,TVal> parameters)
        {
            foreach (KeyValuePair<string, TVal> v in parameters)
            {
                TryAdd(v.Key);
                command.Parameters[v.Key].Value = v.Value;
            }
        }
        public bool ExetuceNonQuery(MySqlCommand com)
        {
            MySqlCommand buffer = command;
            Command = com;
            bool result = ExecuteNonQuery();
            command = buffer;
            return result;
        }
        public bool ExecuteNonQuery() {
            if (command == null)
                return false; ;
            if (!IsOpen())
                Open();
            this.command.Prepare();
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {

                return false;
            }
        }
        public List<List<string>> ExetuceQuery(MySqlCommand com)
        {
            MySqlCommand buffer = command;
            Command = com;
            var result = ExecuteQuery();
            command = buffer;
            return result;
        }
        public List<List<string>> ExecuteQuery()
        {
            if (command == null)
                return null;
            if (!IsOpen())
                Open();
            this.command.Prepare();
            MySqlDataReader dataReader = command.ExecuteReader();
            List<List<string>> result = null;
            try
            {
                result = new List<List<string>>(dataReader.FieldCount);
                for (int i = 0; i < dataReader.FieldCount; i++)
                    result.Add(new List<string>());
                while (dataReader.Read())
                {
                    for(int i = 0;i < dataReader.FieldCount; i++)
                    {
                        result[i].Add(dataReader[i].ToString());
                    }
                }
            }
            finally
            {
                dataReader.Close();
            }
            return result;
        }
        public bool IsOpen()
        {
            return connection != null && connection.State == System.Data.ConnectionState.Open;
        }
        public MySqlCommand Command
        {
            set
            {
                this.command = value;                
                this.command.Connection = connection;
            }
        }
    }
}
