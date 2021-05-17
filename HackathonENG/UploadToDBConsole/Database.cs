using System.Data.SqlClient;

namespace UploadToDBConsole
{
    public class Database
    {
        public SqlConnection Connection;
        public string InstanceName;
        public string DatabaseName;

        //SqlConnection cn;

        public Database()
        {
            Connection = new SqlConnection();
            Connection.ConnectionString = "Data Source=revitdetective.cyldsxwh3dzr.us-east-2.rds.amazonaws.com;Initial Catalog=VERSION01;User ID=admin;Password=hackathon_ENG_2020";
            InstanceName = "revitdetective";

        }

    }
}
