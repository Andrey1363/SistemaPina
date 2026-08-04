using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Labores : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoLabor"] != null && (bool)ViewState["EditandoLabor"]; }
            set { ViewState["EditandoLabor"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdLaborEditando"] as string; }
            set { ViewState["IdLaborEditando"] = value; }
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
                CargarLabores();
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

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarLaborParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT l.*, b.LoteId, lo.FincaId 
                    FROM Labores l 
                    INNER JOIN Bloques b ON l.BloqueId = b.BloqueId 
                    INNER JOIN Lotes lo ON b.LoteId = lo.LoteId 
                    WHERE l.LaborId = @laborId";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@laborId", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string fincaId = reader["FincaId"].ToString();
                    string loteId = reader["LoteId"].ToString();
                    string bloqueId = reader["BloqueId"].ToString();

                    // Seleccionar finca
                    if (ddlFinca.Items.FindByValue(fincaId) != null)
                    {
                        ddlFinca.SelectedValue = fincaId;
                    }

                    // Cargar y seleccionar lote
                    CargarLotes();
                    if (ddlLote.Items.FindByValue(loteId) != null)
                    {
                        ddlLote.SelectedValue = loteId;
                    }

                    // Cargar y seleccionar bloque
                    CargarBloques();
                    if (ddlBloque.Items.FindByValue(bloqueId) != null)
                    {
                        ddlBloque.SelectedValue = bloqueId;
                    }

                    // Cargar el resto de los datos
                    ddlTipoLabor.SelectedValue = reader["TipoLabor"].ToString();
                    txtFechaLabor.Text = Convert.ToDateTime(reader["FechaLabor"]).ToString("yyyy-MM-dd");
                    txtResponsable.Text = reader["Responsable"].ToString();
                    txtObservaciones.Text = reader["Observaciones"].ToString();
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
        // EVENTOS DE CAMBIO
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
        // CARGAR LABORES
        // ==========================================
        private void CargarLabores()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT l.LaborId, f.Nombre AS NombreFinca, " +
                                  "lo.Nombre AS NombreLote, b.Nombre AS NombreBloque, " +
                                  "l.TipoLabor, l.FechaLabor, l.Responsable, l.Observaciones " +
                                  "FROM Labores l " +
                                  "INNER JOIN Bloques b ON l.BloqueId = b.BloqueId " +
                                  "INNER JOIN Lotes lo ON b.LoteId = lo.LoteId " +
                                  "INNER JOIN Fincas f ON lo.FincaId = f.FincaId " +
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "ORDER BY l.FechaLabor DESC";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvLabores.DataSource = dt;
                gvLabores.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR LABOR
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string bloqueId = ddlBloque.SelectedValue;
            string tipoLabor = ddlTipoLabor.SelectedValue;
            string fecha = txtFechaLabor.Text.Trim();
            string responsable = txtResponsable.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            if (bloqueId == "-- Seleccione un bloque --" || fecha == "")
            {
                lblMensaje.Text = "Bloque y fecha son obligatorios.";
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
                    // ACTUALIZAR LABOR
                    // ==========================================
                    string consulta = "UPDATE Labores SET BloqueId = @bloqueId, TipoLabor = @tipoLabor, " +
                                      "FechaLabor = @fecha, Responsable = @responsable, Observaciones = @observaciones " +
                                      "WHERE LaborId = @id";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@bloqueId", bloqueId);
                    cmd.Parameters.AddWithValue("@tipoLabor", tipoLabor);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@responsable", string.IsNullOrEmpty(responsable) ? (object)DBNull.Value : responsable);
                    cmd.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(observaciones) ? (object)DBNull.Value : observaciones);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Labor actualizada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar labor";
                    btnCancelarEdicion.Visible = false;
                    lblTituloFormulario.Text = "Registrar labor diaria";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA LABOR
                    // ==========================================
                    string consulta = "INSERT INTO Labores (BloqueId, TipoLabor, FechaLabor, Responsable, Observaciones) " +
                                      "VALUES (@bloqueId, @tipoLabor, @fecha, @responsable, @observaciones)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@bloqueId", bloqueId);
                    cmd.Parameters.AddWithValue("@tipoLabor", tipoLabor);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@responsable", string.IsNullOrEmpty(responsable) ? (object)DBNull.Value : responsable);
                    cmd.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(observaciones) ? (object)DBNull.Value : observaciones);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Labor registrada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtFechaLabor.Text = "";
                txtResponsable.Text = "";
                txtObservaciones.Text = "";
                lblMensaje.Visible = true;

                CargarLabores();
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
        // ELIMINAR / EDITAR (RowCommand)
        // ==========================================
        protected void gvLabores_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string laborId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarLaborParaEditar(laborId);
                Editando = true;
                IdEditando = laborId;
                btnGuardar.Text = "✅ Actualizar Labor";
                btnCancelarEdicion.Visible = true;
                lblTituloFormulario.Text = "Editando labor #" + laborId;
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Labores WHERE LaborId = @laborId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@laborId", laborId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Labor eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarLabores();
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
        // CANCELAR EDICIÓN
        // ==========================================
        protected void btnCancelarEdicion_Click(object sender, EventArgs e)
        {
            Editando = false;
            IdEditando = null;
            btnGuardar.Text = "Guardar labor";
            btnCancelarEdicion.Visible = false;
            lblTituloFormulario.Text = "Registrar labor diaria";
            txtFechaLabor.Text = "";
            txtResponsable.Text = "";
            txtObservaciones.Text = "";
            lblMensaje.Visible = false;
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