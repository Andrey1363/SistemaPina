using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Lotes : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoLote"] != null && (bool)ViewState["EditandoLote"]; }
            set { ViewState["EditandoLote"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdLoteEditando"] as string; }
            set { ViewState["IdLoteEditando"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
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

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarLoteParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT FincaId, Nombre, Codigo FROM Lotes WHERE LoteId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlFinca.SelectedValue = reader["FincaId"].ToString();
                    txtNombre.Text = reader["Nombre"].ToString();
                    txtCodigo.Text = reader["Codigo"].ToString();
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
        // CARGAR LOTES
        // ==========================================
        private void CargarLotes()
        {
            if (Session["EmpresaId"] == null || Session["EmpresaId"].ToString() == "")
            {
                lblMensaje.Text = "Error: No hay empresa asociada a este usuario.";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT l.LoteId, f.Nombre AS NombreFinca, l.Nombre, l.Codigo " +
                                  "FROM Lotes l " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "ORDER BY f.Nombre, l.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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

        // ==========================================
        // GUARDAR / ACTUALIZAR LOTE
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string codigo = txtCodigo.Text.Trim();
            string fincaId = ddlFinca.SelectedValue;

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

                if (Editando)
                {
                    // ==========================================
                    // ACTUALIZAR LOTE
                    // ==========================================
                    string consulta = "UPDATE Lotes SET FincaId = @fincaId, Nombre = @nombre, Codigo = @codigo WHERE LoteId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Lote actualizado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar lote";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVO LOTE
                    // ==========================================
                    string consulta = "INSERT INTO Lotes (FincaId, Nombre, Codigo) " +
                                      "VALUES (@fincaId, @nombre, @codigo)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Lote guardado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombre.Text = "";
                txtCodigo.Text = "";
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

        // ==========================================
        // ELIMINAR / EDITAR (RowCommand)
        // ==========================================
        protected void gvLotes_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string loteId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarLoteParaEditar(loteId);
                Editando = true;
                IdEditando = loteId;
                btnGuardar.Text = "✅ Actualizar Lote";
            }
            else if (e.CommandName == "Eliminar")
            {
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