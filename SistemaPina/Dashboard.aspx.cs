//using System;
//using MySql.Data.MySqlClient;
//using ClassLibrary1;

//namespace SistemaPina
//{
//    public partial class Dashboard : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            // Verificar que el usuario haya iniciado sesión
//            // Si no hay sesión activa, redirigir al Login
//            if (Session["UsuarioId"] == null)
//            {
//                Response.Redirect("Login.aspx");
//                return;
//            }

//            // Mostrar el nombre del usuario en la barra superior
//            lblNombreUsuario.Text = "👤 " + Session["Nombre"].ToString();

//            // Ocultar menú de usuarios si no es Admin
//            if (Session["Rol"].ToString() != "Admin")
//            {
//                panelUsuarios.Visible = false;
//            }

//            // Cargar los contadores solo la primera vez que carga la página
//            if (!IsPostBack)
//            {
//                CargarContadores();
//            }
//        }


//        // Método que carga los contadores generales del dashboard
//        private void CargarContadores()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();

//            try
//            {
//                conexion.ABRIR_CONEXION();

//                // Contar total de fincas registradas
//                MySqlCommand cmdFincas = new MySqlCommand(
//                    "SELECT COUNT(*) FROM Fincas", conexion.CONECTAR);
//                lblTotalFincas.Text = cmdFincas.ExecuteScalar().ToString();

//                // Contar grupos de forza registrados
//                MySqlCommand cmdGrupos = new MySqlCommand(
//                    "SELECT COUNT(*) FROM GruposForza", conexion.CONECTAR);
//                lblTotalPlagas.Text = cmdGrupos.ExecuteScalar().ToString();

//                // Contar cosechas realizadas
//                MySqlCommand cmdCosechas = new MySqlCommand(
//                    "SELECT COUNT(*) FROM Cosechas", conexion.CONECTAR);
//                lblTotalFertilizaciones.Text = cmdCosechas.ExecuteScalar().ToString();
//            }
//            catch (Exception ex)
//            {
//                // Si hay error dejamos los contadores en 0
//            }
//            finally
//            {
//                conexion.CERRAR_CONEXION();
//            }
//        }

//        // Método para cerrar sesión
//        protected void btnCerrarSesion_Click(object sender, EventArgs e)
//        {
//            // Eliminar todos los datos de la sesión
//            Session.Clear();
//            Session.Abandon();

//            // Redirigir al Login
//            Response.Redirect("Login.aspx");
//        }
//    }
//}
using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;

namespace SistemaPina
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["Rol"].ToString();
            lblNombreUsuario.Text = "👤 " + Session["Nombre"].ToString();

            if (rol == "SuperAdmin")
            {
                // SuperAdmin solo ve su menú
                panelMenuNormal.Visible = false;
                panelMenuSuperAdmin.Visible = true;
                panelUsuarios.Visible = false;
            }
            else if (rol == "Encargado")
            {
                // Encargado no ve usuarios
                panelUsuarios.Visible = false;
                panelMenuSuperAdmin.Visible = false;
            }
            else
            {
                // Admin ve todo menos SuperAdmin
                panelMenuSuperAdmin.Visible = false;
                panelUsuarios.Visible = true;
            }

            if (!IsPostBack)
            {
                CargarContadores();
            }
        }

        private void CargarContadores()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string rol = Session["Rol"].ToString();
                string empresaId = Session["EmpresaId"].ToString();

                // Si es SuperAdmin ve todo, si no filtra por empresa
                string filtro = rol == "SuperAdmin" ? "" : "WHERE f.EmpresaId = @empresaId";
                string filtroGF = rol == "SuperAdmin" ? "" : "WHERE g.FincaId IN (SELECT FincaId FROM Fincas WHERE EmpresaId = @empresaId)";
                string filtroC = rol == "SuperAdmin" ? "" : "WHERE c.GrupoForzaId IN (SELECT GrupoForzaId FROM GruposForza g INNER JOIN Fincas f ON g.FincaId = f.FincaId WHERE f.EmpresaId = @empresaId)";

                // Contar fincas
                MySqlCommand cmdFincas = new MySqlCommand(
                    $"SELECT COUNT(*) FROM Fincas f {filtro}", conexion.CONECTAR);
                if (rol != "SuperAdmin")
                    cmdFincas.Parameters.AddWithValue("@empresaId", empresaId);
                lblTotalFincas.Text = cmdFincas.ExecuteScalar().ToString();

                // Contar grupos de forza
                MySqlCommand cmdGrupos = new MySqlCommand(
                    $"SELECT COUNT(*) FROM GruposForza g {filtroGF}", conexion.CONECTAR);
                if (rol != "SuperAdmin")
                    cmdGrupos.Parameters.AddWithValue("@empresaId", empresaId);
                lblTotalPlagas.Text = cmdGrupos.ExecuteScalar().ToString();

                // Contar cosechas
                MySqlCommand cmdCosechas = new MySqlCommand(
                    $"SELECT COUNT(*) FROM Cosechas c {filtroC}", conexion.CONECTAR);
                if (rol != "SuperAdmin")
                    cmdCosechas.Parameters.AddWithValue("@empresaId", empresaId);
                lblTotalFertilizaciones.Text = cmdCosechas.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // Error silencioso
            }
            finally
            {
                conexion.CERRAR_CONEXION();
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