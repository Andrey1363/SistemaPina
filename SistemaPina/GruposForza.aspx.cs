using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class GruposForza : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoGrupo"] != null && (bool)ViewState["EditandoGrupo"]; }
            set { ViewState["EditandoGrupo"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdGrupoEditando"] as string; }
            set { ViewState["IdGrupoEditando"] = value; }
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
                CargarGrupos();
            }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarGrupoParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                // Obtener datos del grupo
                string consulta = "SELECT FincaId, Nombre FROM GruposForza WHERE GrupoForzaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlFinca.SelectedValue = reader["FincaId"].ToString();
                    txtNombre.Text = reader["Nombre"].ToString();
                }
                reader.Close();

                // Cargar los bloques seleccionados
                CargarBloques();

                // Marcar los bloques que pertenecen a este grupo
                string consultaBloques = "SELECT BloqueId FROM GrupoForzaBloques WHERE GrupoForzaId = @id";
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

                CargarBloques();
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
        // CARGAR BLOQUES
        // ==========================================
        private void CargarBloques()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT b.BloqueId, CONCAT(l.Nombre, ' - ', b.Nombre) AS Descripcion " +
                                  "FROM Bloques b " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "WHERE l.FincaId = @fincaId " +
                                  "ORDER BY l.Nombre, b.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@fincaId", ddlFinca.SelectedValue);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cblBloques.DataSource = dt;
                cblBloques.DataTextField = "Descripcion";
                cblBloques.DataValueField = "BloqueId";
                cblBloques.DataBind();
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
        // EVENTOS DE CAMBIO
        // ==========================================
        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        // ==========================================
        // CARGAR GRUPOS
        // ==========================================
        private void CargarGrupos()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT g.GrupoForzaId, f.Nombre AS NombreFinca, g.Nombre, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques " +
                                  "FROM GruposForza g " +
                                  "INNER JOIN Fincas f ON g.FincaId = f.FincaId " +
                                  "LEFT JOIN GrupoForzaBloques gb ON g.GrupoForzaId = gb.GrupoForzaId " +
                                  "LEFT JOIN Bloques b ON gb.BloqueId = b.BloqueId " +
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "GROUP BY g.GrupoForzaId, f.Nombre, g.Nombre " +
                                  "ORDER BY f.Nombre, g.Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvGrupos.DataSource = dt;
                gvGrupos.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR GRUPO
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string fincaId = ddlFinca.SelectedValue;

            if (fincaId == "-- Seleccione una finca --" || nombre == "")
            {
                lblMensaje.Text = "Seleccioná una finca y escribí el nombre del grupo.";
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
                    // ACTUALIZAR GRUPO
                    // ==========================================
                    string consulta = "UPDATE GruposForza SET FincaId = @fincaId, Nombre = @nombre WHERE GrupoForzaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    // Eliminar bloques antiguos
                    string eliminarBloques = "DELETE FROM GrupoForzaBloques WHERE GrupoForzaId = @id";
                    MySqlCommand cmdEliminar = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdEliminar.Parameters.AddWithValue("@id", IdEditando);
                    cmdEliminar.ExecuteNonQuery();

                    // Insertar bloques seleccionados
                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO GrupoForzaBloques (GrupoForzaId, BloqueId) " +
                                                 "VALUES (@grupoId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@grupoId", IdEditando);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Grupo actualizado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar grupo";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVO GRUPO
                    // ==========================================
                    string consulta = "INSERT INTO GruposForza (FincaId, Nombre) VALUES (@fincaId, @nombre)";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@fincaId", fincaId);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.ExecuteNonQuery();

                    long grupoId = cmd.LastInsertedId;

                    foreach (System.Web.UI.WebControls.ListItem item in cblBloques.Items)
                    {
                        if (item.Selected)
                        {
                            string insertBloque = "INSERT INTO GrupoForzaBloques (GrupoForzaId, BloqueId) " +
                                                 "VALUES (@grupoId, @bloqueId)";
                            MySqlCommand cmdBloque = new MySqlCommand(insertBloque, conexion.CONECTAR);
                            cmdBloque.Parameters.AddWithValue("@grupoId", grupoId);
                            cmdBloque.Parameters.AddWithValue("@bloqueId", item.Value);
                            cmdBloque.ExecuteNonQuery();
                        }
                    }

                    lblMensaje.Text = "✅ Grupo de forza guardado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombre.Text = "";
                lblMensaje.Visible = true;
                CargarGrupos();
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
        protected void gvGrupos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string grupoId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarGrupoParaEditar(grupoId);
                Editando = true;
                IdEditando = grupoId;
                btnGuardar.Text = "✅ Actualizar Grupo";
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string eliminarBloques = "DELETE FROM GrupoForzaBloques WHERE GrupoForzaId = @grupoId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@grupoId", grupoId);
                    cmdBloques.ExecuteNonQuery();

                    string eliminarGrupo = "DELETE FROM GruposForza WHERE GrupoForzaId = @grupoId";
                    MySqlCommand cmdGrupo = new MySqlCommand(eliminarGrupo, conexion.CONECTAR);
                    cmdGrupo.Parameters.AddWithValue("@grupoId", grupoId);
                    cmdGrupo.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Grupo eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarGrupos();
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