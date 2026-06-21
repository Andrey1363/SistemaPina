using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class GruposForza : System.Web.UI.Page
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
                CargarGrupos();
            }
        }

        // Carga las fincas en el dropdown
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

        // Carga los bloques de la finca seleccionada como checkboxes
        private void CargarBloques()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // Trae todos los bloques de la finca seleccionada
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

                // Llenar los checkboxes con los bloques
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

        // Cuando cambia la finca actualiza los bloques
        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        // Carga todos los grupos con sus bloques
        private void CargarGrupos()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // GROUP_CONCAT une los nombres de los bloques en una sola celda
                string consulta = "SELECT g.GrupoForzaId, f.Nombre AS NombreFinca, g.Nombre, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques " +
                                  "FROM GruposForza g " +
                                  "INNER JOIN Fincas f ON g.FincaId = f.FincaId " +
                                  "LEFT JOIN GrupoForzaBloques gb ON g.GrupoForzaId = gb.GrupoForzaId " +
                                  "LEFT JOIN Bloques b ON gb.BloqueId = b.BloqueId " +
                                  "GROUP BY g.GrupoForzaId, f.Nombre, g.Nombre " +
                                  "ORDER BY f.Nombre, g.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvGrupos.DataSource = dt;
                gvGrupos.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar grupos: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Guarda el grupo con sus bloques seleccionados
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

                // Insertar el grupo de forza
                string consulta = "INSERT INTO GruposForza (FincaId, Nombre) VALUES (@fincaId, @nombre)";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@fincaId", fincaId);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.ExecuteNonQuery();

                // Obtener el ID del grupo recién creado
                long grupoId = cmd.LastInsertedId;

                // Insertar cada bloque seleccionado en la tabla intermedia
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

                txtNombre.Text = "";

                lblMensaje.Text = "✅ Grupo de forza guardado correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
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

        // Elimina un grupo de forza
        protected void gvGrupos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string grupoId = gvGrupos.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    // Primero eliminar los bloques asociados
                    string eliminarBloques = "DELETE FROM GrupoForzaBloques WHERE GrupoForzaId = @grupoId";
                    MySqlCommand cmdBloques = new MySqlCommand(eliminarBloques, conexion.CONECTAR);
                    cmdBloques.Parameters.AddWithValue("@grupoId", grupoId);
                    cmdBloques.ExecuteNonQuery();

                    // Luego eliminar el grupo
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

        // Cerrar sesión
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}