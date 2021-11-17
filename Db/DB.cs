using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuler.Db
{
    internal class DB
    {
        private static DB? _instance = null;

        private DB() {}

        internal static DB Instance()
        {
            if (_instance == null)
            {
                _instance = new DB();
            }
            return _instance;
        }

        public void InitializeDb()
        {
            using (var connection = Connection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    create table if not exists character_stash (
                        id integer primary key,
                        name text,
                        image_src text,
                        update_date text
                    );
                ";

                command.ExecuteNonQuery();
            }
        }

        public static SqliteConnection Connection()
        {
            return new SqliteConnection("Data Source=character.db");
        }




    }
}
