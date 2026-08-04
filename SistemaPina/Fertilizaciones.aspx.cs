using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Fertilizaciones : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA PLANTACIÓN
        // ==========================================
        private bool EditandoPlantacion
        {
            get { return ViewState["EditandoPlantacion"] != null && (bool)ViewState["EditandoPlantacion"]; }
            set { ViewState["EditandoPlantacion"] = value; }
        }

        private string IdPlantacionEditando
        {
            get { return ViewState["IdPlantacionEditando"] as string; }
            set { ViewState["IdPlantacionEditando"] = value; }
        }

        // ==========================================
        // VARIABLES DE ESTADO PARA FRUTA
        // ==========================================
        private bool EditandoFruta
        {
            get { return ViewState["EditandoFruta"] != null && (bool)ViewState["EditandoFruta"]; }
            set { ViewState["EditandoFruta"] = value; }
        }

        private string IdFrutaEditando
        {
            get { return ViewState["IdFrutaEditando"] as string; }
            set { ViewState["IdFrutaEditando"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            // SuperAdmin no tiene acceso a este módulo
            if (Session["Rol"].ToString() == "SuperAdmin")
            {
                Response.Redirect("Usuarios.aspx");
                return;
            }

            // ==========================================
            // MOSTRAR USUARIO Y EMPRESA
            // ==========================================
            lblNombreUsuario.Text = Session["Nombre"].ToString();

            if (Session["NombreEmpresa"] != null && Session["NombreEmpresa"].ToString() != "")
            {
                lblEmpresa.Text = Session["NombreEmpresa"].ToString();
                lblEmpresa.Visible = true;
            }
            else
            {
                lblEmpresa.Visible = false;
            }

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
                string consulta = "SELECT FincaId, Nombre FROM Fincas WHERE EmpresaId = @empresaId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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

                string consulta = @"
                    SELECT g.GrupoForzaId, CONCAT(f.Nombre, ' - ', g.Nombre) AS Descripcion 
                    FROM GruposForza g 
                    INNER JOIN Fincas f ON g.FincaId = f.FincaId 
                    WHERE f.EmpresaId = @empresaId
                    ORDER BY f.Nombre, g.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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
                lblMensajeFruta.Text = "Error al cargar grupos: " + ex.Message;
                lblMensajeFruta.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
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

        // ==========================================
        // CARGAR DATOS PARA EDITAR PLANTACIÓN
        // ==========================================
        private void CargarPlantacionParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT fp.NumeroCiclo, fp.TipoFertilizante, fp.FechaAplicacion, fp.Observaciones,
                           GROUP_CONCAT(fb.BloqueId SEPARATOR ',') AS BloquesIds
                    FROM FertilizacionesPlantacion fp
                    INNER JOIN FertilizacionPlantacionBloques fb ON fp.FertilizacionId = fb.FertilizacionId
                    WHERE fp.FertilizacionId = @id
                    GROUP BY fp.NumeroCiclo, fp.TipoFertilizante, fp.FechaAplicacion, fp.Observaciones";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlCicloPlantacion.SelectedValue = reader["NumeroCiclo"].ToString();
                    txtTipoFertilizante.Text = reader["TipoFertilizante"].ToString();
                    txtFechaAplicacion.Text = Convert.ToDateTime(reader["FechaAplicacion"]).ToString("yyyy-MM-dd");
                    txtObservaciones.Text = reader["Observaciones"].ToString();

                    string bloquesIds = reader["BloquesIds"].ToString();
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        item.Selected = bloquesIds.Contains(item.Value);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar datos: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR FRUTA
        // ==========================================
        private void CargarFrutaParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT GrupoForzaId, NumeroCiclo, TipoFertilizante, FechaAplicacion, Observaciones FROM FertilizacionesFruta WHERE FertilizacionFrutaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlGrupoForza.SelectedValue = reader["GrupoForzaId"].ToString();
                    ddlCicloFruta.SelectedValue = reader["NumeroCiclo"].ToString();
                    txtTipoFertilizanteFruta.Text = reader["TipoFertilizante"].ToString();
                    txtFechaAplicacionFruta.Text = Convert.ToDateTime(reader["FechaAplicacion"]).ToString("yyyy-MM-dd");
                    txtObservacionesFruta.Text = reader["Observaciones"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMensajeFruta.Text = "Error al cargar datos: " + ex.Message;
                lblMensajeFruta.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
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
                                  "WHERE fi.EmpresaId = @empresaId " +
                                  "GROUP BY fp.FertilizacionId, fi.Nombre, l.Nombre, fp.NumeroCiclo, fp.TipoFertilizante, fp.FechaAplicacion " +
                                  "ORDER BY fp.FechaAplicacion DESC";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
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
                                  "WHERE fi.EmpresaId = @empresaId " +
                                  "ORDER BY ff.FechaAplicacion DESC";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
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
        // GUARDAR / ACTUALIZAR PLANTACIÓN
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

                if (EditandoPlantacion)
                {
                    // ==========================================
                    // ACTUALIZAR PLANTACIÓN
                    // ==========================================
                    string consulta = "UPDATE FertilizacionesPlantacion SET NumeroCiclo = @ciclo, TipoFertilizante = @tipo, " +
                                      "FechaAplicacion = @fecha, Observaciones = @observaciones " +
                                      "WHERE FertilizacionId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@ciclo", ciclo);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdPlantacionEditando);
                    cmd.ExecuteNonQuery();

                    // Eliminar bloques antiguos
                    string eliminarBloques = "DELETE FROM FertilizacionPlantacionBloques WHERE FertilizacionId = @id";
                    MySqlCommand cmdEliminar = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdEliminar.Parameters.AddWithValue("@id", IdPlantacionEditando);
                    cmdEliminar.ExecuteNonQuery();

                    // Insertar bloques seleccionados
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO FertilizacionPlantacionBloques (FertilizacionId, BloqueId) VALUES (@fertId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@fertId", IdPlantacionEditando);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Ciclo de plantación actualizado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    EditandoPlantacion = false;
                    IdPlantacionEditando = null;
                    btnGuardarPlantacion.Text = "Guardar Plantación";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA PLANTACIÓN
                    // ==========================================
                    string consulta = "INSERT INTO FertilizacionesPlantacion (NumeroCiclo, TipoFertilizante, FechaAplicacion, Observaciones) " +
                                      "VALUES (@ciclo, @tipo, @fecha, @observaciones)";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@ciclo", ciclo);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.ExecuteNonQuery();

                    long fertilizacionId = cmd.LastInsertedId;

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

                    lblMensaje.Text = "✅ Ciclo de plantación registrado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtTipoFertilizante.Text = "";
                txtFechaAplicacion.Text = "";
                txtObservaciones.Text = "";
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

        // ─────────────────────────────────────────
        // GUARDAR / ACTUALIZAR FRUTA
        // ─────────────────────────────────────────

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

                if (EditandoFruta)
                {
                    // ==========================================
                    // ACTUALIZAR FRUTA
                    // ==========================================
                    string consulta = "UPDATE FertilizacionesFruta SET GrupoForzaId = @grupoId, NumeroCiclo = @ciclo, " +
                                      "TipoFertilizante = @tipo, FechaAplicacion = @fecha, Observaciones = @observaciones " +
                                      "WHERE FertilizacionFrutaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@grupoId", grupoId);
                    cmd.Parameters.AddWithValue("@ciclo", ciclo);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdFrutaEditando);
                    cmd.ExecuteNonQuery();

                    lblMensajeFruta.Text = "✅ Ciclo de fruta actualizado correctamente.";
                    lblMensajeFruta.CssClass = "mensaje-exito";

                    EditandoFruta = false;
                    IdFrutaEditando = null;
                    btnGuardarFruta.Text = "Guardar Fruta";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA FRUTA
                    // ==========================================
                    string consulta = "INSERT INTO FertilizacionesFruta (GrupoForzaId, NumeroCiclo, TipoFertilizante, FechaAplicacion, Observaciones) " +
                                      "VALUES (@grupoId, @ciclo, @tipo, @fecha, @observaciones)";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@grupoId", grupoId);
                    cmd.Parameters.AddWithValue("@ciclo", ciclo);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.ExecuteNonQuery();

                    lblMensajeFruta.Text = "✅ Ciclo de fruta registrado correctamente.";
                    lblMensajeFruta.CssClass = "mensaje-exito";
                }

                txtTipoFertilizanteFruta.Text = "";
                txtFechaAplicacionFruta.Text = "";
                txtObservacionesFruta.Text = "";
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
        // ELIMINAR / EDITAR PLANTACIÓN
        // ─────────────────────────────────────────

        protected void gvPlantacion_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string fertId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarPlantacionParaEditar(fertId);
                EditandoPlantacion = true;
                IdPlantacionEditando = fertId;
                btnGuardarPlantacion.Text = "✅ Actualizar Plantación";
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    string eliminarBloques = "DELETE FROM FertilizacionPlantacionBloques WHERE FertilizacionId = @fertId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@fertId", fertId);
                    cmdBloques.ExecuteNonQuery();

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

        // ─────────────────────────────────────────
        // ELIMINAR / EDITAR FRUTA
        // ─────────────────────────────────────────

        protected void gvFruta_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string fertId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarFrutaParaEditar(fertId);
                EditandoFruta = true;
                IdFrutaEditando = fertId;
                btnGuardarFruta.Text = "✅ Actualizar Fruta";
            }
            else if (e.CommandName == "Eliminar")
            {
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