using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Text.RegularExpressions;

namespace EncTest
{
    class Program
    {
        private static Encoding _eCp850 = Encoding.GetEncoding(850);
        private static Encoding _eUnicode = Encoding.UTF8;
        private static Encoding _eLatin1 = Encoding.GetEncoding("ISO-8859-1");

        public static string encode_cp850(string sText)
        {
            string sReturn;
            byte[] bSource;
            byte[] bTarget;

            bSource = _eUnicode.GetBytes(sText);
            bTarget = Encoding.Convert(_eUnicode, _eCp850, bSource);
            sReturn = _eLatin1.GetString(bTarget);

            return sReturn;
        }

        public static string decode_cp850(byte[] sTextAsBytea)
        {
            string sReturn;
            byte[] bSource = sTextAsBytea;
            byte[] bTarget;

            bTarget = Encoding.Convert(_eCp850, _eUnicode, bSource);
            sReturn = _eUnicode.GetString(bTarget);

            return sReturn;
        }

        static void Main(string[] args)
        {

            NpgsqlConnection conn;
            NpgsqlTransaction tran;
            NpgsqlCommand command;
            String sIn;

            conn = new NpgsqlConnection("Server=localhost;Port=5432;User Id=myuser;Password=mypassword;Database=mydatabase;Timeout=600;Pooling=false;ApplicationName=enctest");
            conn.Open();
            tran = conn.BeginTransaction();

            /////// Writing cp850
            NpgsqlParameter add1 = new NpgsqlParameter("add1", NpgsqlTypes.NpgsqlDbType.Text);
            command = new NpgsqlCommand("INSERT INTO myfile (add1) VALUES (:add1);", conn, tran);
            add1.Value = encode_cp850("Schwanthaler Straße 75a");

            command.Parameters.Add(add1);
            command.ExecuteNonQuery();

            /////// Reading cp850
            //Read the data as bytea so as not to automatically convert into UTF-16
            command = new NpgsqlCommand("select add1::bytea from myfile order by id desc limit 2;", conn, tran);
            NpgsqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    // We now have bytea rather than string, but we can decode it easily enough
                    byte[] bSource = (byte[])dr[i];
                    String reEncodedString = decode_cp850(bSource);
                    Console.Write("Value: {0} \n", reEncodedString);
                }
                Console.WriteLine();
            }

            tran.Commit();
            conn.Close();

            conn.Open();

            String keypress = Console.ReadLine().ToUpper();

        }        
    }
}
