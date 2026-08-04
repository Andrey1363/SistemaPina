using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Fincas : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoFinca"] != null && (bool)ViewState["EditandoFinca"]; }
            set { ViewState["EditandoFinca"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdFincaEditando"] as string; }
            set { ViewState["IdFincaEditando"] = value; }
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

                string consulta = "SELECT f.FincaId, f.Nombre, f.Ubicacion, f.AreaTotal, " +
                                  "e.NombreEmpresa FROM Fincas f " +
                                  "LEFT JOIN Empresas e ON f.EmpresaId = e.EmpresaId " +
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "ORDER BY f.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvFincas.DataSource = dt;
                gvFincas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar fincas: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarFincaParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT Nombre, Ubicacion, AreaTotal FROM Fincas WHERE FincaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtNombre.Text = reader["Nombre"].ToString();
                    txtUbicacion.Text = reader["Ubicacion"].ToString();
                    txtArea.Text = reader["AreaTotal"].ToString();
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
        // GUARDAR / ACTUALIZAR FINCA
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string ubicacion = txtUbicacion.Text.Trim();
            string area = txtArea.Text.Trim();

            if (nombre == "" || area == "")
            {
                lblMensaje.Text = "El nombre y el área son obligatorios.";
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
                    // ACTUALIZAR FINCA EXISTENTE
                    // ==========================================
                    string consulta = "UPDATE Fincas SET Nombre = @nombre, Ubicacion = @ubicacion, AreaTotal = @area WHERE FincaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@ubicacion", ubicacion);
                    cmd.Parameters.AddWithValue("@area", area);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Finca actualizada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "💾 Guardar Finca";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA FINCA
                    // ==========================================
                    string consulta = "INSERT INTO Fincas (UsuarioId, EmpresaId, Nombre, Ubicacion, AreaTotal) " +
                                      "VALUES (@usuarioId, @empresaId, @nombre, @ubicacion, @area)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@usuarioId", Session["UsuarioId"].ToString());
                    cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@ubicacion", ubicacion);
                    cmd.Parameters.AddWithValue("@area", area);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Finca guardada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombre.Text = "";
                txtUbicacion.Text = "";
                txtArea.Text = "";
                lblMensaje.Visible = true;
                CargarFincas();
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
        protected void gvFincas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            // El ID llega directamente desde el CommandArgument del botón
            string fincaId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarFincaParaEditar(fincaId);
                Editando = true;
                IdEditando = fincaId;
                btnGuardar.Text = "✅ Actualizar Finca";
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Fincas WHERE FincaId = @fincaId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Finca eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarFincas();
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