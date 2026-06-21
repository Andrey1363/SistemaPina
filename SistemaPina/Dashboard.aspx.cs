using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;

namespace SistemaPina
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que el usuario haya iniciado sesión
            // Si no hay sesión activa, redirigir al Login
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Mostrar el nombre del usuario en la barra superior
            lblNombreUsuario.Text = "👤 " + Session["Nombre"].ToString();

            // Ocultar menú de usuarios si no es Admin
            if (Session["Rol"].ToString() != "Admin")
            {
                panelUsuarios.Visible = false;
            }

            // Cargar los contadores solo la primera vez que carga la página
            if (!IsPostBack)
            {
                CargarContadores();
            }
        }

        
        // Método que carga los contadores generales del dashboard
        private void CargarContadores()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();

            try
            {
                conexion.ABRIR_CONEXION();

                // Contar total de fincas registradas
                MySqlCommand cmdFincas = new MySqlCommand(
                    "SELECT COUNT(*) FROM Fincas", conexion.CONECTAR);
                lblTotalFincas.Text = cmdFincas.ExecuteScalar().ToString();

                // Contar grupos de forza registrados
                MySqlCommand cmdGrupos = new MySqlCommand(
                    "SELECT COUNT(*) FROM GruposForza", conexion.CONECTAR);
                lblTotalPlagas.Text = cmdGrupos.ExecuteScalar().ToString();

                // Contar cosechas realizadas
                MySqlCommand cmdCosechas = new MySqlCommand(
                    "SELECT COUNT(*) FROM Cosechas", conexion.CONECTAR);
                lblTotalFertilizaciones.Text = cmdCosechas.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // Si hay error dejamos los contadores en 0
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // Método para cerrar sesión
        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Eliminar todos los datos de la sesión
            Session.Clear();
            Session.Abandon();

            // Redirigir al Login
            Response.Redirect("Login.aspx");
        }
    }
}