using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Siembras : System.Web.UI.Page
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
                CargarSiembras();
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

        // Carga los lotes según la finca seleccionada
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
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Carga los bloques según el lote seleccionado
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
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Cuando cambia la finca actualiza los lotes
        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotes();
        }

        // Cuando cambia el lote actualiza los bloques
        protected void ddlLote_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBloques();
        }

        // Carga todas las siembras en la tabla
        // Calcula automáticamente la edad en días
        private void CargarSiembras()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // DATEDIFF calcula la diferencia en días entre hoy y la fecha de siembra
                string consulta = "SELECT s.SiembraId, f.Nombre AS NombreFinca, l.Nombre AS NombreLote, " +
                                  "b.Nombre AS NombreBloque, s.FechaSiembra, " +
                                  "DATEDIFF(CURDATE(), s.FechaSiembra) AS EdadDias, " +
                                  "s.CantidadPlantas, s.TipoEtapa, s.Estado " +
                                  "FROM Siembras s " +
                                  "INNER JOIN Bloques b ON s.BloqueId = b.BloqueId " +
                                  "INNER JOIN Lotes l ON b.LoteId = l.LoteId " +
                                  "INNER JOIN Fincas f ON l.FincaId = f.FincaId " +
                                  "ORDER BY s.FechaSiembra DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSiembras.DataSource = dt;
                gvSiembras.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar siembras: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Guarda una nueva siembra
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string bloqueId = ddlBloque.SelectedValue;
            string fecha = txtFechaSiembra.Text.Trim();
            string plantas = txtCantidadPlantas.Text.Trim();
            string etapa = ddlTipoEtapa.SelectedValue;
            string observaciones = txtObservaciones.Text.Trim();

            if (bloqueId == "-- Seleccione un bloque --" || fecha == "" || plantas == "")
            {
                lblMensaje.Text = "Bloque, fecha y cantidad de plantas son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "INSERT INTO Siembras (BloqueId, FechaSiembra, CantidadPlantas, TipoEtapa, Observaciones) " +
                                  "VALUES (@bloqueId, @fecha, @plantas, @etapa, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@bloqueId", bloqueId);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@plantas", plantas);
                cmd.Parameters.AddWithValue("@etapa", etapa);
                cmd.Parameters.AddWithValue("@observaciones", observaciones);

                cmd.ExecuteNonQuery();

                txtFechaSiembra.Text = "";
                txtCantidadPlantas.Text = "";
                txtObservaciones.Text = "";

                lblMensaje.Text = "✅ Siembra registrada correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
                lblMensaje.Visible = true;

                CargarSiembras();
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

        // Elimina una siembra
        protected void gvSiembras_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string siembraId = gvSiembras.DataKeys[index].Value.ToString();

                CLASS_CONEXION conexion = new CLASS_CONEXION();

                try
                {
                    conexion.ABRIR_CONEXION();

                    string consulta = "DELETE FROM Siembras WHERE SiembraId = @siembraId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@siembraId", siembraId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Siembra eliminada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarSiembras();
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