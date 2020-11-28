using MySql.Data.MySqlClient;

namespace ConnectionDB
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "192.168.0.130";
            int port = 3306;
            string database = "server_chats";
            string username = "Andre Linder";
            string password = "gus.ar.1628652470";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }

    }
}
