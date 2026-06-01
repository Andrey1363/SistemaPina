using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Lotes : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                // Cargar las fincas en el dropdown
                CargarFincas();
                // Cargar la tabla de lotes
                CargarLotes();
            }
        }

        // Carga las fincas en el dropdown para seleccionar
        private void CargarFincas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT FincaId, Nombre FROM Fincas ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Conectar el dropdown con los datos
                ddlFinca.DataSource = dt;
                ddlFinca.DataTextField = "Nombre";   // lo que se muestra
                ddlFinca.DataValueField = "FincaId"; // el valor que se guarda
                ddlFinca.DataBind();

                // Opción por defecto
                ddlFinca.Items.Insert(0, "-- Seleccione una finca --");
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

        // Carga todos los lotes en la tabla
        private void CargarLotes()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // JOIN para mostrar el nombre de la finca en vez del ID
                string consulta = "SELECT l.LoteId, f.Nombre AS NombreFinca, l.Nombre, l.Codigo " +
                                  "FROM Lotes l " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "ORDER BY f.Nombre, l.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvLotes.DataSource = dt;
                gvLotes.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar lotes: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Guarda un nuevo lote
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string codigo = txtCodigo.Text.Trim();
            string fincaId = ddlFinca.SelectedValue;

            // Validar que seleccionó una finca y escribió un nombre
            if (fincaId == "-- Seleccione una finca --" || nombre == "")
            {
                lblMensaje.Text = "Seleccioná una finca y escribí el nombre del lote.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Lotes (FincaId, Nombre, Codigo) " +
                                  "VALUES (@fincaId, @nombre, @codigo)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@fincaId", fincaId);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@codigo", codigo);

                cmd.ExecuteNonQuery();

                txtNombre.Text = "";
                txtCodigo.Text = "";

                lblMensaje.Text = "✅ Lote guardado correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarLotes();
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

        // Elimina un lote
        protected void gvLotes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string loteId = gvLotes.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "DELETE FROM Lotes WHERE LoteId = @loteId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@loteId", loteId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Lote eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarLotes();
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