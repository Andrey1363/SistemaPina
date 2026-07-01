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
                CargarRecomendaciones();
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
                cblBloques.DataSource = dt;
                cblBloques.DataTextField = "Nombre";
                cblBloques.DataValueField = "BloqueId";
                cblBloques.DataBind();
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

        // Carga todos los registros de enfermedades con finca, lote y bloques
        private void CargarEnfermedades()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT e.EnfermedadId, e.NombreEnfermedad, e.NivelAfectacion, " +
                                  "e.FechaDeteccion, " +
                                  "f.Nombre AS NombreFinca, " +
                                  "l.Nombre AS NombreLote, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques " +
                                  "FROM Enfermedades e " +
                                  "LEFT JOIN EnfermedadBloques eb ON e.EnfermedadId = eb.EnfermedadId " +
                                  "LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId " +
                                  "LEFT JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "LEFT JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "GROUP BY e.EnfermedadId, e.NombreEnfermedad, e.NivelAfectacion, e.FechaDeteccion, f.Nombre, l.Nombre " +
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

        // Carga todas las recomendaciones de enfermedades
        private void CargarRecomendaciones()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT r.RecomendacionId, e.NombreEnfermedad, " +
                                  "f.Nombre AS NombreFinca, " +
                                  "l.Nombre AS NombreLote, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, " +
                                  "r.Producto, r.Dosis, r.FechaAplicacion, r.Observaciones " +
                                  "FROM Recomendaciones r " +
                                  "INNER JOIN Enfermedades e ON r.EnfermedadId = e.EnfermedadId " +
                                  "LEFT JOIN EnfermedadBloques eb ON e.EnfermedadId = eb.EnfermedadId " +
                                  "LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId " +
                                  "LEFT JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "LEFT JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "GROUP BY r.RecomendacionId, e.NombreEnfermedad, f.Nombre, l.Nombre, r.Producto, r.Dosis, r.FechaAplicacion, r.Observaciones " +
                                  "ORDER BY r.FechaAplicacion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvRecomendaciones.DataSource = dt;
                gvRecomendaciones.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Guarda un nuevo registro de enfermedad con varios bloques
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreEnfermedad.Text.Trim();
            string nivel = ddlNivel.SelectedValue;
            string fechaDeteccion = txtFechaDeteccion.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            bool hayBloqueSeleccionado = false;
            foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
            {
                if (item.Selected) { hayBloqueSeleccionado = true; break; }
            }

            if (!hayBloqueSeleccionado || nombre == "" || fechaDeteccion == "")
            {
                lblMensaje.Text = "Seleccioná al menos un bloque, nombre de enfermedad y fecha son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Enfermedades (NombreEnfermedad, NivelAfectacion, FechaDeteccion, Observaciones) " +
                                  "VALUES (@nombre, @nivel, @fechaDeteccion, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@fechaDeteccion", fechaDeteccion);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);
                cmd.ExecuteNonQuery();

                long enfermedadId = cmd.LastInsertedId;

                foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                {
                    if (item.Selected)
                    {
                        string insertBloque = "INSERT INTO EnfermedadBloques (EnfermedadId, BloqueId) VALUES (@enfermedadId, @bloqueId)";
                        MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                        cmdBloque.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                        cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                        cmdBloque.ExecuteNonQuery();
                    }
                }

                txtNombreEnfermedad.Text = "";
                txtFechaDeteccion.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Enfermedad registrada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarEnfermedades();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Maneja botones de la tabla
        protected void gvEnfermedades_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            string enfermedadId = gvEnfermedades.DataKeys[index].Value.ToString();

            if (e.CommandName == "Recomendar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "SELECT e.NombreEnfermedad, " +
                                      "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques " +
                                      "FROM Enfermedades e " +
                                      "LEFT JOIN EnfermedadBloques eb ON e.EnfermedadId = eb.EnfermedadId " +
                                      "LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId " +
                                      "WHERE e.EnfermedadId = @enfermedadId " +
                                      "GROUP BY e.NombreEnfermedad";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblEnfermedadRecomendacion.Text = reader["NombreEnfermedad"].ToString();
                        lblBloquesRecomendacion.Text = reader["Bloques"].ToString();
                        hfEnfermedadId.Value = enfermedadId;
                        txtFechaRec.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        panelRecomendacion.Visible = true;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
                    lblMensaje.Visible = true;
                }
                finally { conexion.CERRAR_CONEXION(); }
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    string eliminarBloques = "DELETE FROM EnfermedadBloques WHERE EnfermedadId = @enfermedadId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                    cmdBloques.ExecuteNonQuery();

                    string eliminarEnfermedad = "DELETE FROM Enfermedades WHERE EnfermedadId = @enfermedadId";
                    MySqlCommand cmdEnfermedad = new MySqlCommand(eliminarEnfermedad, conexion.CONECTAR);
                    cmdEnfermedad.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                    cmdEnfermedad.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Registro eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarEnfermedades();
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

        // Guarda la recomendación de enfermedad
        protected void btnGuardarRec_Click(object sender, EventArgs e)
        {
            string producto = txtProductoRec.Text.Trim();
            string dosis = txtDosisRec.Text.Trim();
            string fecha = txtFechaRec.Text.Trim();
            string observaciones = txtObservacionesRec.Text.Trim();
            string enfermedadId = hfEnfermedadId.Value;

            if (producto == "" || dosis == "" || fecha == "")
            {
                lblMensajeRec.Text = "Producto, dosis y fecha son obligatorios.";
                lblMensajeRec.CssClass = "mensaje-error";
                lblMensajeRec.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Recomendaciones (EnfermedadId, Producto, Dosis, FechaAplicacion, Observaciones) " +
                                  "VALUES (@enfermedadId, @producto, @dosis, @fecha, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@enfermedadId", enfermedadId);
                cmd.Parameters.AddWithValue("@producto", producto);
                cmd.Parameters.AddWithValue("@dosis", dosis);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(observaciones) ? (object)DBNull.Value : observaciones);
                cmd.ExecuteNonQuery();

                txtProductoRec.Text = "";
                txtDosisRec.Text = "";
                txtObservacionesRec.Text = "";

                lblMensajeRec.Text = "✅ Recomendación guardada correctamente.";
                lblMensajeRec.CssClass = "mensaje-exito";
                lblMensajeRec.Visible = true;

                panelRecomendacion.Visible = false;
                CargarEnfermedades();
                CargarRecomendaciones();
            }
            catch (Exception ex)
            {
                lblMensajeRec.Text = "Error al guardar: " + ex.Message;
                lblMensajeRec.CssClass = "mensaje-error";
                lblMensajeRec.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // Cancela la recomendación
        protected void btnCancelarRec_Click(object sender, EventArgs e)
        {
            panelRecomendacion.Visible = false;
            txtProductoRec.Text = "";
            txtDosisRec.Text = "";
            txtObservacionesRec.Text = "";
        }

        // Elimina una recomendación
        protected void gvRecomendaciones_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string recId = gvRecomendaciones.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Recomendaciones WHERE RecomendacionId = @recId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@recId", recId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Recomendación eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarRecomendaciones();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
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