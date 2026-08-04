using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Cosechas : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoCosecha"] != null && (bool)ViewState["EditandoCosecha"]; }
            set { ViewState["EditandoCosecha"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdCosechaEditando"] as string; }
            set { ViewState["IdCosechaEditando"] = value; }
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
                CargarGruposForza();
                CargarCosechas();
            }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR
        // ==========================================
        private void CargarCosechaParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT GrupoForzaId, FechaCosecha, KilosCosechados, Observaciones FROM Cosechas WHERE CosechaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ddlGrupoForza.SelectedValue = reader["GrupoForzaId"].ToString();
                    txtFechaCosecha.Text = Convert.ToDateTime(reader["FechaCosecha"]).ToString("yyyy-MM-dd");
                    txtKilos.Text = reader["KilosCosechados"].ToString();
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
        // CARGAR GRUPOS FORZA
        // ==========================================
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
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "ORDER BY f.Nombre, g.Nombre";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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
                lblMensaje.Text = "Error al cargar grupos: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // ==========================================
        // CARGAR COSECHAS
        // ==========================================
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
                                  "WHERE f.EmpresaId = @empresaId " +
                                  "GROUP BY c.CosechaId, f.Nombre, g.Nombre, c.FechaCosecha, c.KilosCosechados " +
                                  "ORDER BY c.FechaCosecha DESC";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

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

        // ==========================================
        // GUARDAR / ACTUALIZAR COSECHA
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string grupoId = ddlGrupoForza.SelectedValue;
            string fecha = txtFechaCosecha.Text.Trim();
            string kilos = txtKilos.Text.Trim();
            string observaciones = txtObservaciones.Text.Trim();

            // Validar grupo
            if (grupoId == "-- Seleccione un grupo --" || string.IsNullOrEmpty(grupoId))
            {
                lblMensaje.Text = "Seleccione un grupo de forza válido.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            // Validar fecha
            if (string.IsNullOrEmpty(fecha))
            {
                lblMensaje.Text = "La fecha de cosecha es obligatoria.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            // Validar kilos
            if (string.IsNullOrEmpty(kilos) || !decimal.TryParse(kilos, out decimal kilosDecimal) || kilosDecimal <= 0)
            {
                lblMensaje.Text = "Ingrese una cantidad de kilos válida (mayor a 0).";
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
                    // ACTUALIZAR COSECHA
                    // ==========================================
                    string consulta = "UPDATE Cosechas SET GrupoForzaId = @grupoId, FechaCosecha = @fecha, " +
                                      "KilosCosechados = @kilos, Observaciones = @observaciones " +
                                      "WHERE CosechaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@grupoId", grupoId);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@kilos", kilos);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Cosecha actualizada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar cosecha";
                }
                else
                {
                    // ==========================================
                    // INSERTAR NUEVA COSECHA
                    // ==========================================
                    string consulta = "INSERT INTO Cosechas (GrupoForzaId, FechaCosecha, KilosCosechados, Observaciones) " +
                                      "VALUES (@grupoId, @fecha, @kilos, @observaciones)";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@grupoId", grupoId);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@kilos", kilos);
                    cmd.Parameters.AddWithValue("@observaciones", observaciones);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Cosecha registrada correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtFechaCosecha.Text = "";
                txtKilos.Text = "";
                txtObservaciones.Text = "";
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

        // ==========================================
        // ELIMINAR / EDITAR (RowCommand)
        // ==========================================
        protected void gvCosechas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string cosechaId = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarCosechaParaEditar(cosechaId);
                Editando = true;
                IdEditando = cosechaId;
                btnGuardar.Text = "✅ Actualizar Cosecha";
            }
            else if (e.CommandName == "Eliminar")
            {
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