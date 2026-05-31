using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class CLASS_CONEXION
    {
        private string CONEXION = "Server= localhost; database= db_pina ; user = root; password= Arcearce1";
        public MySqlConnection CONECTAR = new MySqlConnection();
        public void ABRIR_CONEXION()
        {
            try
            {
                CONECTAR.ConnectionString = CONEXION;
                CONECTAR.Open();
                System.Diagnostics.Debug.WriteLine("Conexion existosa");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public void CERRAR_CONEXION()
        {
            try
            {
                if (CONECTAR.State == ConnectionState.Open)
                {
                    CONECTAR.Close();
                    System.Diagnostics.Debug.WriteLine("Fin de la conexion");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}