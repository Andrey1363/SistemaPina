using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using ClassLibrary1;

namespace SistemaPina
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No hace nada al cargar, solo espera que el usuario ingrese datos
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            
            if (usuario == "" || contrasena == "")
            {
                lblError.Text = "Por favor completá todos los campos.";
                lblError.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // Consulta que busca el usuario con esa contraseña encriptada
                string consulta = "SELECT UsuarioId, Nombre, Rol FROM Usuarios " +
                                  "WHERE Usuario = @usuario " +
                                  "AND Contrasena = SHA2(@contrasena, 256) " +
                                  "AND Activo = 1";

                // Crear el comando con la consulta
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

                // Agregar los parámetros
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                // Ejecutar y leer el resultado
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Usuario encontrado, guardar en Session
                    Session["UsuarioId"] = reader["UsuarioId"].ToString();
                    Session["Nombre"] = reader["Nombre"].ToString();
                    Session["Rol"] = reader["Rol"].ToString();

                    reader.Close();
                    conexion.CERRAR_CONEXION();

                    // Redirigir al Dashboard
                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    // Usuario no encontrado
                    reader.Close();
                    lblError.Text = "Usuario o contraseña incorrectos.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error de conexión: " + ex.Message;
                lblError.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }
    }
}