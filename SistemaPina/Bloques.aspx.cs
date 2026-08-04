using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Bloques : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoBloque"] != null && (bool)ViewState["EditandoBloque"]; }
            set { ViewState["EditandoBloque"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdBloqueEditando"] as string; }
            set { ViewState["IdBloqueEditando"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
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
                CargarBloques();
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
            finally
            {
                conexion.CERRAR_CONEXION();
            }
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

                string fincaId = ddlFinca.SelectedValue;

                string consulta = "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@fincaId", fincaId);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlLote.DataSource = dt;
                ddlLote.DataTextField = "Nombre";
                ddlLote.DataValueField = "LoteId";
                ddlLote.DataBind();
                ddlLote.Items.Insert(0, "-- Seleccione un lote --");
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarBloqueParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT LoteId, Nombre, AreaHectareas FROM Bloques WHERE BloqueId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlLote.SelectedValue = reader["LoteId"].ToString();
                    txtNombre.Text = reader["Nombre"].ToString();
                    txtArea.Text = reader["AreaHectareas"].ToString();
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

        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotes();
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
                string consulta = "SELECT b.BloqueId, f.Nombre AS NombreFinca, l.Nombre AS NombreLote, " +
                                  "b.Nombre, b.AreaHectareas " +
                                  "FROM Bloques b " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "ORDER BY f.Nombre, l.Nombre, b.Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvBloques.DataSource = dt;
                gvBloques.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR BLOQUE
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string area = txtArea.Text.Trim();
            string loteId = ddlLote.SelectedValue;

            if (loteId == "-- Seleccione un lote --" || nombre == "" || area == "")
            {
                lblMensaje.Text = "Todos los campos son obligatorios.";
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
                    // ACTUALIZAR BLOQUE
                    // ==========================================
                    string consulta = "UPDATE Bloques SET LoteId = @loteId, Nombre = @nombre, AreaHectareas = @area WHERE BloqueId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@loteId", loteId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@area", area);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Bloque actualizado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar bloque";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVO BLOQUE
                    // ==========================================
                    string consulta = "INSERT INTO Bloques (LoteId, Nombre, AreaHectareas) " +
                                      "VALUES (@loteId, @nombre, @area)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@loteId", loteId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@area", area);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Bloque guardado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombre.Text = "";
                txtArea.Text = "";
                lblMensaje.Visible = true;
                CargarBloques();
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

        // ==========================================
        // ELIMINAR / EDITAR (RowCommand)
        // ==========================================
        protected void gvBloques_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string bloqueId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarBloqueParaEditar(bloqueId);
                Editando = true;
                IdEditando = bloqueId;
                btnGuardar.Text = "✅ Actualizar Bloque";
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "DELETE FROM Bloques WHERE BloqueId = @bloqueId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@bloqueId", bloqueId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Bloque eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarBloques();
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

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}