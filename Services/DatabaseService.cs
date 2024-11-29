using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DarwinPullaguariComplementario.Services
{
    public class DatabaseService
    {
        private readonly string connectionString = "Server=localhost;Database=DBUISRAEL;User=root;Password=;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
    }
