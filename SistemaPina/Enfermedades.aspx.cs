using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Enfermedades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            lblNombreUsuario.Text = "👤 " + Session["Nombre"].ToString();

            if (Session["Rol"].ToString() != "Admin")
            {
                panelUsuarios.Visible = false;
            }

            if (!IsPostBack)
            {
                CargarFincas();
                CargarEnfermedades();
                CargarResumen();
            }
        }

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
                ddlFinca.DataSource = dt;
                ddlFinca.DataTextField = "Nombre";
                ddlFinca.DataValueField = "FincaId";
                ddlFinca.DataBind();
                ddlFinca.Items.Insert(0, "-- Seleccione una finca --");
                CargarLotes();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        private void CargarLotes()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@fincaId", ddlFinca.SelectedValue);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLote.DataSource = dt;
                ddlLote.DataTextField = "Nombre";
                ddlLote.DataValueField = "LoteId";
                ddlLote.DataBind();
                ddlLote.Items.Insert(0, "-- Seleccione un lote --");
                CargarBloques();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        private void CargarBloques()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT BloqueId, Nombre FROM Bloques WHERE LoteId = @loteId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@loteId", ddlLote.SelectedValue);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlBloque.DataSource = dt;
                ddlBloque.DataTextField = "Nombre";
                ddlBloque.DataValueField = "BloqueId";
                ddlBloque.DataBind();
                ddlBloque.Items.Insert(0, "-- Seleccione un bloque --");
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotes();
        }

        protected void ddlLote_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        // Carga el resumen de enfermedades por bloque
        private void CargarResumen()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, " +
                                  "b.Nombre AS NombreBloque, e.NombreEnfermedad, " +
                                  "COUNT(*) AS TotalReportes, MAX(e.FechaDeteccion) AS UltimaDeteccion " +
                                  "FROM Enfermedades e " +
                                  "INNER JOIN Bloques b ON e.BloqueId = b.BloqueId " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "GROUP BY f.Nombre, l.Nombre, b.Nombre, e.NombreEnfermedad " +
                                  "ORDER BY TotalReportes DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvResumen.DataSource = dt;
                gvResumen.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Carga todos los registros de enfermedades
        private void CargarEnfermedades()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT e.EnfermedadId, b.Nombre AS NombreBloque, e.NombreEnfermedad, " +
                                  "e.NivelAfectacion, e.FechaDeteccion, e.ProductoAplicado, e.FechaControl " +
                                  "FROM Enfermedades e " +
                                  "INNER JOIN Bloques b ON e.BloqueId = b.BloqueId " +
                                  "ORDER BY e.FechaDeteccion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvEnfermedades.DataSource = dt;
                gvEnfermedades.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Guarda un nuevo registro de enfermedad
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string bloqueId = ddlBloque.SelectedValue;
            string nombre = txtNombreEnfermedad.Text.Trim();
            string nivel = ddlNivel.SelectedValue;
            string fechaDeteccion = txtFechaDeteccion.Text.Trim();
            string producto = txtProducto.Text.Trim();
            string dosis = txtDosis.Text.Trim();
            string fechaControl = txtFechaControl.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            if (bloqueId == "-- Seleccione un bloque --" || nombre == "" || fechaDeteccion == "")
            {
                lblMensaje.Text = "Bloque, nombre de enfermedad y fecha son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Enfermedades (BloqueId, NombreEnfermedad, NivelAfectacion, FechaDeteccion, ProductoAplicado, Dosis, FechaControl, Observaciones) " +
                                  "VALUES (@bloqueId, @nombre, @nivel, @fechaDeteccion, @producto, @dosis, @fechaControl, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@bloqueId", bloqueId);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@fechaDeteccion", fechaDeteccion);
                cmd.Parameters.AddWithValue("@producto", string.IsNullOrEmpty(producto) ? (object)DBNull.Value : producto);
                cmd.Parameters.AddWithValue("@dosis", string.IsNullOrEmpty(dosis) ? (object)DBNull.Value : dosis);
                cmd.Parameters.AddWithValue("@fechaControl", string.IsNullOrEmpty(fechaControl) ? (object)DBNull.Value : fechaControl);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);
                cmd.ExecuteNonQuery();

                txtNombreEnfermedad.Text = "";
                txtFechaDeteccion.Text = "";
                txtProducto.Text = "";
                txtDosis.Text = "";
                txtFechaControl.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Enfermedad registrada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarEnfermedades();
                CargarResumen();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Elimina un registro de enfermedad
        protected void gvEnfermedades_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string enfermedadId = gvEnfermedades.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Enfermedades WHERE EnfermedadId = @enfermedadId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Registro eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarEnfermedades();
                    CargarResumen();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al eliminar: " + ex.Message;
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Visible = true;
                }
                finally { conexion.CERRAR_CONEXION(); }
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