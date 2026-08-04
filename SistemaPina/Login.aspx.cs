
using System;
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
            // Obtener lo que escribió el usuario
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            // Validar que no estén vacíos
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

                // ==========================================
                // CONSULTA CON NOMBRE DE EMPRESA
                // ==========================================
                string consulta = @"
                    SELECT u.UsuarioId, u.Nombre, u.Rol, u.EmpresaId, 
                           e.NombreEmpresa 
                    FROM Usuarios u
                    LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId
                    WHERE u.Usuario = @usuario 
                    AND u.Contrasena = SHA2(@contrasena, 256) 
                    AND u.Activo = 1";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // ==========================================
                    // GUARDAR DATOS EN SESIÓN
                    // ==========================================
                    Session["UsuarioId"] = reader["UsuarioId"].ToString();
                    Session["Nombre"] = reader["Nombre"].ToString();
                    Session["Rol"] = reader["Rol"].ToString();

                    // Guardar EmpresaId (puede ser NULL para SuperAdmin)
                    if (reader["EmpresaId"] != DBNull.Value)
                    {
                        Session["EmpresaId"] = reader["EmpresaId"].ToString();
                        Session["NombreEmpresa"] = reader["NombreEmpresa"].ToString();  // 👈 NUEVO
                    }
                    else
                    {
                        Session["EmpresaId"] = "";
                        Session["NombreEmpresa"] = "Sin empresa asignada";  // 👈 NUEVO
                    }

                    reader.Close();
                    conexion.CERRAR_CONEXION();

                    // Redirigir según el rol
                    if (Session["Rol"].ToString() == "SuperAdmin")
                    {
                        Response.Redirect("Usuarios.aspx");
                    }
                    else
                    {
                        Response.Redirect("Fincas.aspx");
                    }
                }
                else
                {
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