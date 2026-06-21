using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Cosechas : System.Web.UI.Page
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
                CargarGruposForza();
                CargarCosechas();
            }
        }

        // Carga los grupos de forza en el dropdown
        private void CargarGruposForza()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT g.GrupoForzaId, " +
                                  "CONCAT(f.Nombre, ' - ', g.Nombre) AS Descripcion " +
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
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Carga todas las cosechas registradas
        private void CargarCosechas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "SELECT c.CosechaId, f.Nombre AS NombreFinca, g.Nombre AS NombreGrupo, " +
                                  "GROUP_CONCAT(b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, " +
                                  "c.FechaCosecha, c.KilosCosechados " +
                                  "FROM Cosechas c " +
                                  "INNER JOIN GruposForza g ON c.GrupoForzaId = g.GrupoForzaId " +
                                  "INNER JOIN Fincas f ON g.FincaId = f.FincaId " +
                                  "LEFT JOIN GrupoForzaBloques gb ON g.GrupoForzaId = gb.GrupoForzaId " +
                                  "LEFT JOIN Bloques b ON gb.BloqueId = b.BloqueId " +
                                  "GROUP BY c.CosechaId, f.Nombre, g.Nombre, c.FechaCosecha, c.KilosCosechados " +
                                  "ORDER BY c.FechaCosecha DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCosechas.DataSource = dt;
                gvCosechas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar cosechas: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Guarda una nueva cosecha
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string grupoId = ddlGrupoForza.SelectedValue;
            string fecha = txtFechaCosecha.Text.Trim();
            string kilos = txtKilos.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            if (grupoId == "-- Seleccione un grupo --" || fecha == "" || kilos == "")
            {
                lblMensaje.Text = "Grupo, fecha y kilos son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Cosechas (GrupoForzaId, FechaCosecha, KilosCosechados, Observaciones) " +
                                  "VALUES (@grupoId, @fecha, @kilos, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@grupoId", grupoId);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@kilos", kilos);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);
                cmd.ExecuteNonQuery();

                txtFechaCosecha.Text = "";
                txtKilos.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Cosecha registrada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarCosechas();
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

        // Elimina una cosecha
        protected void gvCosechas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string cosechaId = gvCosechas.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "DELETE FROM Cosechas WHERE CosechaId = @cosechaId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@cosechaId", cosechaId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Cosecha eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarCosechas();
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