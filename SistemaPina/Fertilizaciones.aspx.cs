using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Fertilizaciones : System.Web.UI.Page
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
                CargarGruposForza();
                CargarFertilizacionesPlantacion();
                CargarFertilizacionesFruta();
            }
        }

        // ─────────────────────────────────────────
        // CARGA DE DROPDOWNS
        // ─────────────────────────────────────────

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

        private void CargarGruposForza()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT g.GrupoForzaId, CONCAT(f.Nombre, ' - ', g.Nombre) AS Descripcion " +
                                  "FROM GruposForza g " +
                                  "INNER JOIN Fincas f ON g.FincaId = f.FincaId " +
                                  "ORDER BY f.Nombre, g.Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlGrupoForza.DataSource = dt;
                ddlGrupoForza.DataTextField = "Descripcion";
                ddlGrupoForza.DataValueField = "GrupoForzaId";
                ddlGrupoForza.DataBind();
                ddlGrupoForza.Items.Insert(0, "-- Seleccione un grupo --");
            }
            catch (Exception ex)
            {
                lblMensajeFruta.Text = "Error: " + ex.Message;
                lblMensajeFruta.Visible = true;
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

        // Seleccionar todos los bloques
        protected void btnMarcarTodos_Click(object sender, EventArgs e)
        {
            foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
            {
                item.Selected = true;
            }
        }

        // Deseleccionar todos los bloques
        protected void btnDesmarcarTodos_Click(object sender, EventArgs e)
        {
            foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
            {
                item.Selected = false;
            }
        }

        // ─────────────────────────────────────────
        // CARGAR TABLAS
        // ─────────────────────────────────────────

        private void CargarFertilizacionesPlantacion()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT fp.FertilizacionId, fi.Nombre AS NombreFinca, " +
                                  "l.Nombre AS NombreLote, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, " +
                                  "fp.NumeroCiclo, fp.TipoFertilizante, fp.FechaAplicacion " +
                                  "FROM FertilizacionesPlantacion fp " +
                                  "INNER JOIN FertilizacionPlantacionBloques fb ON fp.FertilizacionId = fb.FertilizacionId " +
                                  "INNER JOIN Bloques b ON fb.BloqueId = b.BloqueId " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "INNER JOIN Fincas fi ON l.FincaId = fi.FincaId " +
                                  "GROUP BY fp.FertilizacionId, fi.Nombre, l.Nombre, fp.NumeroCiclo, fp.TipoFertilizante, fp.FechaAplicacion " +
                                  "ORDER BY fp.FechaAplicacion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvPlantacion.DataSource = dt;
                gvPlantacion.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        private void CargarFertilizacionesFruta()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT ff.FertilizacionFrutaId, fi.Nombre AS NombreFinca, " +
                                  "g.Nombre AS NombreGrupo, ff.NumeroCiclo, " +
                                  "ff.TipoFertilizante, ff.FechaAplicacion " +
                                  "FROM FertilizacionesFruta ff " +
                                  "INNER JOIN GruposForza g ON ff.GrupoForzaId = g.GrupoForzaId " +
                                  "INNER JOIN Fincas fi ON g.FincaId = fi.FincaId " +
                                  "ORDER BY ff.FechaAplicacion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvFruta.DataSource = dt;
                gvFruta.DataBind();
            }
            catch (Exception ex)
            {
                lblMensajeFruta.Text = "Error: " + ex.Message;
                lblMensajeFruta.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ─────────────────────────────────────────
        // GUARDAR
        // ─────────────────────────────────────────

        protected void btnGuardarPlantacion_Click(object sender, EventArgs e)
        {
            string tipo = txtTipoFertilizante.Text.Trim();
            string fecha = txtFechaAplicacion.Text.Trim();
            string ciclo = ddlCicloPlantacion.SelectedValue;
            string observaciones = txtObservaciones.Text.Trim();

            bool hayBloqueSeleccionado = false;
            foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
            {
                if (item.Selected) { hayBloqueSeleccionado = true; break; }
            }

            if (!hayBloqueSeleccionado || tipo == "" || fecha == "")
            {
                lblMensaje.Text = "Seleccioná al menos un bloque, tipo de fertilizante y fecha son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                // Insertar el registro principal
                string consulta = "INSERT INTO FertilizacionesPlantacion (NumeroCiclo, TipoFertilizante, FechaAplicacion, Observaciones) " +
                                  "VALUES (@ciclo, @tipo, @fecha, @observaciones)";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);
                cmd.ExecuteNonQuery();

                long fertilizacionId = cmd.LastInsertedId;

                // Insertar cada bloque seleccionado
                foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                {
                    if (item.Selected)
                    {
                        string insertBloque = "INSERT INTO FertilizacionPlantacionBloques (FertilizacionId, BloqueId) VALUES (@fertId, @bloqueId)";
                        MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                        cmdBloque.Parameters.AddWithValue("@fertId", fertilizacionId);
                        cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                        cmdBloque.ExecuteNonQuery();
                    }
                }

                txtTipoFertilizante.Text = "";
                txtFechaAplicacion.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Ciclo de plantación registrado correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarFertilizacionesPlantacion();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        protected void btnGuardarFruta_Click(object sender, EventArgs e)
        {
            string grupoId = ddlGrupoForza.SelectedValue;
            string tipo = txtTipoFertilizanteFruta.Text.Trim();
            string fecha = txtFechaAplicacionFruta.Text.Trim();
            string ciclo = ddlCicloFruta.SelectedValue;
            string observaciones = txtObservacionesFruta.Text.Trim();

            if (grupoId == "-- Seleccione un grupo --" || tipo == "" || fecha == "")
            {
                lblMensajeFruta.Text = "Grupo de Forza, tipo de fertilizante y fecha son obligatorios.";
                lblMensajeFruta.CssClass = "mensaje-error";
                lblMensajeFruta.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO FertilizacionesFruta (GrupoForzaId, NumeroCiclo, TipoFertilizante, FechaAplicacion, Observaciones) " +
                                  "VALUES (@grupoId, @ciclo, @tipo, @fecha, @observaciones)";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@grupoId", grupoId);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);
                cmd.ExecuteNonQuery();

                txtTipoFertilizanteFruta.Text = "";
                txtFechaAplicacionFruta.Text = "";
                txtObservacionesFruta.Text = "";

                lblMensajeFruta.Text = "✅ Ciclo de fruta registrado correctamente.";
                lblMensajeFruta.CssClass = "mensaje-exito";
                lblMensajeFruta.Visible = true;

                CargarFertilizacionesFruta();
            }
            catch (Exception ex)
            {
                lblMensajeFruta.Text = "Error al guardar: " + ex.Message;
                lblMensajeFruta.CssClass = "mensaje-error";
                lblMensajeFruta.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ─────────────────────────────────────────
        // ELIMINAR
        // ─────────────────────────────────────────

        protected void gvPlantacion_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string fertId = gvPlantacion.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    // Primero eliminar los bloques asociados
                    string eliminarBloques = "DELETE FROM FertilizacionPlantacionBloques WHERE FertilizacionId = @fertId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@fertId", fertId);
                    cmdBloques.ExecuteNonQuery();

                    // Luego eliminar el registro principal
                    string eliminar = "DELETE FROM FertilizacionesPlantacion WHERE FertilizacionId = @fertId";
                    MySqlCommand cmd = new MySqlCommand(eliminar, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fertId", fertId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Registro eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarFertilizacionesPlantacion();
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

        protected void gvFruta_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string fertId = gvFruta.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    string eliminar = "DELETE FROM FertilizacionesFruta WHERE FertilizacionFrutaId = @fertId";
                    MySqlCommand cmd = new MySqlCommand(eliminar, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fertId", fertId);
                    cmd.ExecuteNonQuery();

                    lblMensajeFruta.Text = "✅ Registro eliminado correctamente.";
                    lblMensajeFruta.CssClass = "mensaje-exito";
                    lblMensajeFruta.Visible = true;

                    CargarFertilizacionesFruta();
                }
                catch (Exception ex)
                {
                    lblMensajeFruta.Text = "Error: " + ex.Message;
                    lblMensajeFruta.CssClass = "mensaje-error";
                    lblMensajeFruta.Visible = true;
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