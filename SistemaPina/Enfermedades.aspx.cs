using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Enfermedades : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoEnfermedad"] != null && (bool)ViewState["EditandoEnfermedad"]; }
            set { ViewState["EditandoEnfermedad"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdEnfermedadEditando"] as string; }
            set { ViewState["IdEnfermedadEditando"] = value; }
        }

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
                CargarEnfermedades();
                CargarRecomendaciones();
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
        // CARGAR DATOS PARA EDITAR ENFERMEDAD
        // ==========================================
        private void CargarEnfermedadParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                // Obtener datos de la enfermedad
                string consulta = "SELECT NombreEnfermedad, NivelAfectacion, FechaDeteccion, Observaciones FROM Enfermedades WHERE EnfermedadId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtNombreEnfermedad.Text = reader["NombreEnfermedad"].ToString();
                    ddlNivel.SelectedValue = reader["NivelAfectacion"].ToString();
                    txtFechaDeteccion.Text = Convert.ToDateTime(reader["FechaDeteccion"]).ToString("yyyy-MM-dd");
                    txtObservaciones.Text = reader["Observaciones"].ToString();
                }
                reader.Close();

                // Marcar los bloques seleccionados
                string consultaBloques = "SELECT BloqueId FROM EnfermedadBloques WHERE EnfermedadId = @id";
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
        // CARGAR ENFERMEDADES (SOLO EMPRESA)
        // ==========================================
        private void CargarEnfermedades()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT e.EnfermedadId, e.NombreEnfermedad, e.NivelAfectacion, 
                           e.FechaDeteccion, f.Nombre AS NombreFinca, l.Nombre AS NombreLote, 
                           GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques
                    FROM Enfermedades e
                    LEFT JOIN EnfermedadBloques eb ON e.EnfermedadId = eb.EnfermedadId
                    LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId
                    LEFT JOIN Lotes l ON b.LoteId = l.LoteId
                    LEFT JOIN Fincas f ON l.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId
                    GROUP BY e.EnfermedadId, e.NombreEnfermedad, e.NivelAfectacion, e.FechaDeteccion, f.Nombre, l.Nombre
                    ORDER BY e.FechaDeteccion DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvEnfermedades.DataSource = dt;
                gvEnfermedades.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar enfermedades: " + ex.Message;
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
                    SELECT r.RecomendacionId, e.NombreEnfermedad, 
                           f.Nombre AS NombreFinca, 
                           l.Nombre AS NombreLote, 
                           GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, 
                           r.Producto, r.Dosis, r.FechaAplicacion, r.Observaciones
                    FROM Recomendaciones r
                    INNER JOIN Enfermedades e ON r.EnfermedadId = e.EnfermedadId
                    LEFT JOIN EnfermedadBloques eb ON e.EnfermedadId = eb.EnfermedadId
                    LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId
                    LEFT JOIN Lotes l ON b.LoteId = l.LoteId
                    LEFT JOIN Fincas f ON l.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId
                    GROUP BY r.RecomendacionId, e.NombreEnfermedad, f.Nombre, l.Nombre, 
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
                lblMensaje.Text = "Error al cargar recomendaciones: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR ENFERMEDAD
        // ==========================================
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

                if (Editando)
                {
                    // ==========================================
                    // ACTUALIZAR ENFERMEDAD
                    // ==========================================
                    string consulta = "UPDATE Enfermedades SET NombreEnfermedad = @nombre, NivelAfectacion = @nivel, " +
                                      "FechaDeteccion = @fechaDeteccion, Observaciones = @observaciones " +
                                      "WHERE EnfermedadId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@fechaDeteccion", fechaDeteccion);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    // Eliminar bloques antiguos
                    string eliminarBloques = "DELETE FROM EnfermedadBloques WHERE EnfermedadId = @id";
                    MySqlCommand cmdEliminar = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdEliminar.Parameters.AddWithValue("@id", IdEditando);
                    cmdEliminar.ExecuteNonQuery();

                    // Insertar bloques seleccionados
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO EnfermedadBloques (EnfermedadId, BloqueId) VALUES (@enfermedadId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@enfermedadId", IdEditando);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Enfermedad actualizada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar enfermedad";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA ENFERMEDAD
                    // ==========================================
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

                    lblMensaje.Text = "✅ Enfermedad registrada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombreEnfermedad.Text = "";
                txtFechaDeteccion.Text = "";
                txtObservaciones.Text = "";
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

        // ==========================================
        // COMANDOS DE TABLA ENFERMEDADES (Recomendar, Editar, Eliminar)
        // ==========================================
        protected void gvEnfermedades_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string enfermedadId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarEnfermedadParaEditar(enfermedadId);
                Editando = true;
                IdEditando = enfermedadId;
                btnGuardar.Text = "✅ Actualizar Enfermedad";
            }
            else if (e.CommandName == "Recomendar")
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

        // ==========================================
        // GUARDAR / ACTUALIZAR RECOMENDACIÓN
        // ==========================================
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
                    string consulta = "INSERT INTO Recomendaciones (EnfermedadId, Producto, Dosis, FechaAplicacion, Observaciones) " +
                                      "VALUES (@enfermedadId, @producto, @dosis, @fecha, @observaciones)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@enfermedadId", enfermedadId);
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