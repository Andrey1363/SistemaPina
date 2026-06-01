using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Fincas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Mostrar nombre del usuario
            lblNombreUsuario.Text = "👤 " + Session["Nombre"].ToString();

            // Ocultar usuarios si no es Admin
            if (Session["Rol"].ToString() != "Admin")
            {
                panelUsuarios.Visible = false;
            }

            // Cargar la tabla solo la primera vez
            if (!IsPostBack)
            {
                CargarFincas();
            }
        }

        // Método que carga todas las fincas en la tabla
        private void CargarFincas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // Consulta para traer todas las fincas
                string consulta = "SELECT FincaId, Nombre, Ubicacion, AreaTotal FROM Fincas ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

                // Llenar un DataTable con los resultados
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Conectar el DataTable a la tabla visual
                gvFincas.DataSource = dt;
                gvFincas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar fincas: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Método que guarda una nueva finca
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // Obtener los valores del formulario
            string nombre = txtNombre.Text.Trim();
            string ubicacion = txtUbicacion.Text.Trim();
            string area = txtArea.Text.Trim();

            // Validar que no estén vacíos
            if (nombre == "" || area == "")
            {
                lblMensaje.Text = "El nombre y el área son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // Consulta para insertar la nueva finca
                string consulta = "INSERT INTO Fincas (UsuarioId, Nombre, Ubicacion, AreaTotal) " +
                                  "VALUES (@usuarioId, @nombre, @ubicacion, @area)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@usuarioId", Session["UsuarioId"].ToString());
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@ubicacion", ubicacion);
                cmd.Parameters.AddWithValue("@area", area);

                cmd.ExecuteNonQuery();

                // Limpiar los campos del formulario
                txtNombre.Text = "";
                txtUbicacion.Text = "";
                txtArea.Text = "";

                // Mostrar mensaje de éxito
                lblMensaje.Text = "✅ Finca guardada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                // Recargar la tabla
                CargarFincas();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Método para eliminar una finca
        protected void gvFincas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                // Obtener el ID de la finca a eliminar
                int index = Convert.ToInt32(e.CommandArgument);
                string fincaId = gvFincas.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "DELETE FROM Fincas WHERE FincaId = @fincaId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Finca eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarFincas();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al eliminar: " + ex.Message;
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Visible = true;
                }
                finally
                {
                    conexion.CERRAR_CONEXION();
                }
            }
        }

        // Cerrar sesión
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}