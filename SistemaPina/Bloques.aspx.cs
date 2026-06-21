using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Bloques : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
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
                CargarBloques();
            }
        }

        // Carga las fincas en el primer dropdown
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

                // Cargar los lotes de la primera finca
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

        // Se ejecuta cuando el usuario cambia la finca seleccionada
        // Actualiza automáticamente los lotes del dropdown
        protected void ddlFinca_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotes();
        }

        // Carga todos los bloques en la tabla
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
                                  "ORDER BY f.Nombre, l.Nombre, b.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBloques.DataSource = dt;
                gvBloques.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar bloques: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Guarda un nuevo bloque
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

                string consulta = "INSERT INTO Bloques (LoteId, Nombre, AreaHectareas) " +
                                  "VALUES (@loteId, @nombre, @area)";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@loteId", loteId);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@area", area);

                cmd.ExecuteNonQuery();

                txtNombre.Text = "";
                txtArea.Text = "";

                lblMensaje.Text = "✅ Bloque guardado correctamente.";
                lblMensaje.CssClass = "mensaje-exito";
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

        // Elimina un bloque
        protected void gvBloques_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string bloqueId = gvBloques.DataKeys[index].Value.ToString();

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

        // Cerrar sesión
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}