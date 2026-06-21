using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Plagas : System.Web.UI.Page
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
                CargarPlagas();
                CargarResumen();
                VerificarAlertas();
            }
        }

        // Carga las fincas en el dropdown
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

        // Verifica si hay brotes de plagas
        // RF-15: alerta cuando la misma plaga se reporta más de 3 veces en 7 días
        private void VerificarAlertas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT b.Nombre AS Bloque, p.NombrePlaga, COUNT(*) AS Total " +
                                  "FROM Plagas p " +
                                  "INNER JOIN Bloques b ON p.BloqueId = b.BloqueId " +
                                  "WHERE p.FechaDeteccion >= DATE_SUB(CURDATE(), INTERVAL 7 DAY) " +
                                  "GROUP BY p.BloqueId, p.NombrePlaga " +
                                  "HAVING COUNT(*) > 3";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataReader reader = cmd.ExecuteReader();

                string alertas = "";
                while (reader.Read())
                {
                    alertas += $"La plaga '{reader["NombrePlaga"]}' se reportó {reader["Total"]} veces en el Bloque '{reader["Bloque"]}' en los últimos 7 días. ";
                }
                reader.Close();

                if (alertas != "")
                {
                    lblAlerta.Text = alertas;
                    panelAlerta.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Carga el resumen de plagas por bloque
        private void CargarResumen()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, " +
                                  "b.Nombre AS NombreBloque, p.NombrePlaga, " +
                                  "COUNT(*) AS TotalReportes, MAX(p.FechaDeteccion) AS UltimaDeteccion " +
                                  "FROM Plagas p " +
                                  "INNER JOIN Bloques b ON p.BloqueId = b.BloqueId " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "GROUP BY f.Nombre, l.Nombre, b.Nombre, p.NombrePlaga " +
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

        // Carga todos los registros de plagas
        private void CargarPlagas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT p.PlagaId, b.Nombre AS NombreBloque, p.NombrePlaga, " +
                                  "p.NivelAfectacion, p.FechaDeteccion, p.ProductoAplicado, p.FechaControl " +
                                  "FROM Plagas p " +
                                  "INNER JOIN Bloques b ON p.BloqueId = b.BloqueId " +
                                  "ORDER BY p.FechaDeteccion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvPlagas.DataSource = dt;
                gvPlagas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Guarda un nuevo registro de plaga
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string bloqueId = ddlBloque.SelectedValue;
            string nombre = txtNombrePlaga.Text.Trim();
            string nivel = ddlNivel.SelectedValue;
            string fechaDeteccion = txtFechaDeteccion.Text.Trim();
            string producto = txtProducto.Text.Trim();
            string dosis = txtDosis.Text.Trim();
            string fechaControl = txtFechaControl.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            if (bloqueId == "-- Seleccione un bloque --" || nombre == "" || fechaDeteccion == "")
            {
                lblMensaje.Text = "Bloque, nombre de plaga y fecha son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Plagas (BloqueId, NombrePlaga, NivelAfectacion, FechaDeteccion, ProductoAplicado, Dosis, FechaControl, Observaciones) " +
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

                txtNombrePlaga.Text = "";
                txtFechaDeteccion.Text = "";
                txtProducto.Text = "";
                txtDosis.Text = "";
                txtFechaControl.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Plaga registrada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarPlagas();
                CargarResumen();
                VerificarAlertas();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Elimina un registro de plaga
        protected void gvPlagas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string plagaId = gvPlagas.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Plagas WHERE PlagaId = @plagaId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@plagaId", plagaId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Registro eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarPlagas();
                    CargarResumen();
                    VerificarAlertas();
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