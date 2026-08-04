using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Plagas : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR PLAGA
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoPlaga"] != null && (bool)ViewState["EditandoPlaga"]; }
            set { ViewState["EditandoPlaga"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdPlagaEditando"] as string; }
            set { ViewState["IdPlagaEditando"] = value; }
        }

        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR RECOMENDACIÓN
        // ==========================================
        private bool EditandoRec
        {
            get { return ViewState["EditandoRec"] != null && (bool)ViewState["EditandoRec"]; }
            set { ViewState["EditandoRec"] = value; }
        }

        private string IdRecEditando
        {
            get { return ViewState["IdRecEditando"] as string; }
            set { ViewState["IdRecEditando"] = value; }
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
                CargarPlagas();
                CargarRecomendaciones();
                VerificarAlertas();
            }
        }

        // ==========================================
        // CARGAR FINCAS
        // ==========================================
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

        // ==========================================
        // CARGAR LOTES
        // ==========================================
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

        // ==========================================
        // CARGAR BLOQUES
        // ==========================================
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

        // ==========================================
        // EVENTOS DE CAMBIO EN DROPDOWNS
        // ==========================================
        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotes();
        }

        protected void ddlLote_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR PLAGA
        // ==========================================
        private void CargarPlagaParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                // Obtener datos de la plaga
                string consulta = "SELECT NombrePlaga, NivelAfectacion, FechaDeteccion, Observaciones FROM Plagas WHERE PlagaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtNombrePlaga.Text = reader["NombrePlaga"].ToString();
                    ddlNivel.SelectedValue = reader["NivelAfectacion"].ToString();
                    txtFechaDeteccion.Text = Convert.ToDateTime(reader["FechaDeteccion"]).ToString("yyyy-MM-dd");
                    txtObservaciones.Text = reader["Observaciones"].ToString();
                }
                reader.Close();

                // Marcar los bloques seleccionados
                string consultaBloques = "SELECT BloqueId FROM PlagaBloques WHERE PlagaId = @id";
                MySqlCommand cmdBloques = new MySqlCommand(consultaBloques, conexion.CONECTAR);
                cmdBloques.Parameters.AddWithValue("@id", id);
                MySqlDataReader readerBloques = cmdBloques.ExecuteReader();

                while (readerBloques.Read())
                {
                    string bloqueId = readerBloques["BloqueId"].ToString();
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Value == bloqueId)
                        {
                            item.Selected = true;
                        }
                    }
                }
                readerBloques.Close();
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
        // CARGAR DATOS PARA EDITAR RECOMENDACIÓN
        // ==========================================
        private void CargarRecomendacionParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT Producto, Dosis, FechaAplicacion, Observaciones FROM Recomendaciones WHERE RecomendacionId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtProductoRec.Text = reader["Producto"].ToString();
                    txtDosisRec.Text = reader["Dosis"].ToString();
                    txtFechaRec.Text = Convert.ToDateTime(reader["FechaAplicacion"]).ToString("yyyy-MM-dd");
                    txtObservacionesRec.Text = reader["Observaciones"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMensajeRec.Text = "Error al cargar datos: " + ex.Message;
                lblMensajeRec.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // ==========================================
        // VERIFICAR ALERTAS (SOLO EMPRESA)
        // ==========================================
        private void VerificarAlertas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT p.NombrePlaga, COUNT(*) AS Total 
                    FROM Plagas p
                    LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId
                    LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId
                    LEFT JOIN Lotes l ON b.LoteId = l.LoteId
                    LEFT JOIN Fincas f ON l.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId
                      AND p.FechaDeteccion >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                    GROUP BY p.NombrePlaga
                    HAVING COUNT(*) > 3";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                MySqlDataReader reader = cmd.ExecuteReader();

                string alertas = "";
                while (reader.Read())
                {
                    alertas += $"La plaga '{reader["NombrePlaga"]}' se reportó {reader["Total"]} veces en los últimos 7 días. ";
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

        // ==========================================
        // CARGAR PLAGAS (SOLO EMPRESA)
        // ==========================================
        private void CargarPlagas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT p.PlagaId, p.NombrePlaga, p.NivelAfectacion, 
                           p.FechaDeteccion, f.Nombre AS NombreFinca, l.Nombre AS NombreLote, 
                           GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques
                    FROM Plagas p
                    LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId
                    LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId
                    LEFT JOIN Lotes l ON b.LoteId = l.LoteId
                    LEFT JOIN Fincas f ON l.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId
                    GROUP BY p.PlagaId, p.NombrePlaga, p.NivelAfectacion, p.FechaDeteccion, f.Nombre, l.Nombre
                    ORDER BY p.FechaDeteccion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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

        // ==========================================
        // CARGAR RECOMENDACIONES (SOLO EMPRESA)
        // ==========================================
        private void CargarRecomendaciones()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT r.RecomendacionId, p.NombrePlaga, 
                           f.Nombre AS NombreFinca, 
                           l.Nombre AS NombreLote, 
                           GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, 
                           r.Producto, r.Dosis, r.FechaAplicacion, r.Observaciones
                    FROM Recomendaciones r
                    INNER JOIN Plagas p ON r.PlagaId = p.PlagaId
                    LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId
                    LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId
                    LEFT JOIN Lotes l ON b.LoteId = l.LoteId
                    LEFT JOIN Fincas f ON l.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId
                    GROUP BY r.RecomendacionId, p.NombrePlaga, f.Nombre, l.Nombre, 
                             r.Producto, r.Dosis, r.FechaAplicacion, r.Observaciones
                    ORDER BY r.FechaAplicacion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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

        // ==========================================
        // GUARDAR / ACTUALIZAR PLAGA
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombrePlaga.Text.Trim();
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
                lblMensaje.Text = "Seleccioná al menos un bloque, nombre de plaga y fecha son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                if (Editando)
                {
                    // ==========================================
                    // ACTUALIZAR PLAGA
                    // ==========================================
                    string consulta = "UPDATE Plagas SET NombrePlaga = @nombre, NivelAfectacion = @nivel, " +
                                      "FechaDeteccion = @fechaDeteccion, Observaciones = @observaciones " +
                                      "WHERE PlagaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@fechaDeteccion", fechaDeteccion);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    // Eliminar bloques antiguos
                    string eliminarBloques = "DELETE FROM PlagaBloques WHERE PlagaId = @id";
                    MySqlCommand cmdEliminar = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdEliminar.Parameters.AddWithValue("@id", IdEditando);
                    cmdEliminar.ExecuteNonQuery();

                    // Insertar bloques seleccionados
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO PlagaBloques (PlagaId, BloqueId) VALUES (@plagaId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@plagaId", IdEditando);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Plaga actualizada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar plaga";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA PLAGA
                    // ==========================================
                    string consulta = "INSERT INTO Plagas (NombrePlaga, NivelAfectacion, FechaDeteccion, Observaciones) " +
                                      "VALUES (@nombre, @nivel, @fechaDeteccion, @observaciones)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@fechaDeteccion", fechaDeteccion);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.ExecuteNonQuery();

                    long plagaId = cmd.LastInsertedId;

                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO PlagaBloques (PlagaId, BloqueId) VALUES (@plagaId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@plagaId", plagaId);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Plaga registrada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombrePlaga.Text = "";
                txtFechaDeteccion.Text = "";
                txtObservaciones.Text = "";
                lblMensaje.Visible = true;
                CargarPlagas();
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

        // ==========================================
        // GUARDAR / ACTUALIZAR RECOMENDACIÓN
        // ==========================================
        protected void btnGuardarRec_Click(object sender, EventArgs e)
        {
            string producto = txtProductoRec.Text.Trim();
            string dosis = txtDosisRec.Text.Trim();
            string fecha = txtFechaRec.Text.Trim();
            string observaciones = txtObservacionesRec.Text.Trim();
            string plagaId = hfPlagaId.Value;

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

                if (EditandoRec)
                {
                    // ==========================================
                    // ACTUALIZAR RECOMENDACIÓN
                    // ==========================================
                    string consulta = "UPDATE Recomendaciones SET Producto = @producto, Dosis = @dosis, " +
                                      "FechaAplicacion = @fecha, Observaciones = @observaciones " +
                                      "WHERE RecomendacionId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@dosis", dosis);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdRecEditando);
                    cmd.ExecuteNonQuery();

                    lblMensajeRec.Text = "✅ Recomendación actualizada correctamente.";
                    lblMensajeRec.CssClass = "mensaje-exito";

                    EditandoRec = false;
                    IdRecEditando = null;
                    btnGuardarRec.Text = "Guardar recomendación";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA RECOMENDACIÓN
                    // ==========================================
                    string consulta = "INSERT INTO Recomendaciones (PlagaId, Producto, Dosis, FechaAplicacion, Observaciones) " +
                                      "VALUES (@plagaId, @producto, @dosis, @fecha, @observaciones)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@plagaId", plagaId);
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@dosis", dosis);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.ExecuteNonQuery();

                    lblMensajeRec.Text = "✅ Recomendación guardada correctamente.";
                    lblMensajeRec.CssClass = "mensaje-exito";
                }

                txtProductoRec.Text = "";
                txtDosisRec.Text = "";
                txtObservacionesRec.Text = "";
                lblMensajeRec.Visible = true;

                panelRecomendacion.Visible = false;
                CargarPlagas();
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

        // ==========================================
        // CANCELAR RECOMENDACIÓN
        // ==========================================
        protected void btnCancelarRec_Click(object sender, EventArgs e)
        {
            panelRecomendacion.Visible = false;
            txtProductoRec.Text = "";
            txtDosisRec.Text = "";
            txtObservacionesRec.Text = "";
        }

        // ==========================================
        // COMANDOS DE TABLA PLAGAS (Recomendar, Editar, Eliminar)
        // ==========================================
        protected void gvPlagas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string plagaId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarPlagaParaEditar(plagaId);
                Editando = true;
                IdEditando = plagaId;
                btnGuardar.Text = "✅ Actualizar Plaga";
            }
            else if (e.CommandName == "Recomendar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "SELECT p.NombrePlaga, " +
                                      "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques " +
                                      "FROM Plagas p " +
                                      "LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId " +
                                      "LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId " +
                                      "WHERE p.PlagaId = @plagaId " +
                                      "GROUP BY p.NombrePlaga";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@plagaId", plagaId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblPlagaRecomendacion.Text = reader["NombrePlaga"].ToString();
                        lblBloquesRecomendacion.Text = reader["Bloques"].ToString();
                        hfPlagaId.Value = plagaId;
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

                    string eliminarBloques = "DELETE FROM PlagaBloques WHERE PlagaId = @plagaId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@plagaId", plagaId);
                    cmdBloques.ExecuteNonQuery();

                    string eliminarPlaga = "DELETE FROM Plagas WHERE PlagaId = @plagaId";
                    MySqlCommand cmdPlaga = new MySqlCommand(eliminarPlaga, conexion.CONECTAR);
                    cmdPlaga.Parameters.AddWithValue("@plagaId", plagaId);
                    cmdPlaga.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Registro eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarPlagas();
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

        // ==========================================
        // COMANDOS DE TABLA RECOMENDACIONES (Editar, Eliminar)
        // ==========================================
        protected void gvRecomendaciones_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string recId = e.CommandArgument.ToString();

            if (e.CommandName == "EditarRec")
            {
                CargarRecomendacionParaEditar(recId);
                EditandoRec = true;
                IdRecEditando = recId;
                btnGuardarRec.Text = "✅ Actualizar Recomendación";
            }
            else if (e.CommandName == "Eliminar")
            {
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

        // ==========================================
        // CERRAR SESIÓN
        // ==========================================
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}