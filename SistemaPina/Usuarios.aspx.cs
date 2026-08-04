//using System;
//using MySql.Data.MySqlClient;
//using ClassLibrary1;
//using System.Data;

//namespace SistemaPina
//{
//    public partial class Usuarios : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Session["UsuarioId"] == null)
//            {
//                Response.Redirect("Login.aspx");
//                return;
//            }

//            string rol = Session["Rol"].ToString();

//            // ==========================================
//            // MOSTRAR USUARIO Y EMPRESA
//            // ==========================================
//            lblNombreUsuario.Text = Session["Nombre"].ToString();

//            if (Session["NombreEmpresa"] != null && Session["NombreEmpresa"].ToString() != "")
//            {
//                lblEmpresa.Text = Session["NombreEmpresa"].ToString();
//                lblEmpresa.Visible = true;
//            }
//            else
//            {
//                lblEmpresa.Visible = false;
//            }

//            // Solo Admin y SuperAdmin pueden entrar aquí
//            if (rol == "Encargado")
//            {
//                Response.Redirect("Dashboard.aspx");
//                return;
//            }

//            if (!IsPostBack)
//            {
//                ConfigurarVistaPorRol(rol);
//                CargarUsuarios(rol);

//                if (rol == "SuperAdmin")
//                {
//                    CargarEmpresas();
//                    CargarDropdownEmpresas();
//                }
//            }
//        }

//        // Configura la vista según el rol
//        private void ConfigurarVistaPorRol(string rol)
//        {
//            if (rol == "SuperAdmin")
//            {
//                // SuperAdmin ve todo
//                panelEmpresas.Visible = true;
//                panelSelectorEmpresa.Visible = true;
//                panelUsuarios.Visible = true;
//                lblDescripcion.Text = "Administrá las empresas y usuarios del sistema.";
//                lblSubtituloUsuarios.Text = "Todos los usuarios del sistema";

//                // SuperAdmin puede crear Admin y Encargado
//                ddlRol.Items.Add("Admin");
//                ddlRol.Items.Add("Encargado");
//            }
//            else if (rol == "Admin")
//            {
//                // Admin solo ve su empresa
//                panelEmpresas.Visible = false;
//                panelSelectorEmpresa.Visible = false;
//                panelUsuarios.Visible = true;
//                lblDescripcion.Text = "Administrá los encargados de tu empresa.";
//                lblSubtituloUsuarios.Text = "Encargados de tu empresa";

//                // Admin solo puede crear Encargados
//                ddlRol.Items.Add("Encargado");
//            }
//        }

//        // Carga el dropdown de empresas para SuperAdmin
//        private void CargarDropdownEmpresas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT EmpresaId, NombreEmpresa FROM Empresas WHERE Activa = 1 ORDER BY NombreEmpresa";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlEmpresa.DataSource = dt;
//                ddlEmpresa.DataTextField = "NombreEmpresa";
//                ddlEmpresa.DataValueField = "EmpresaId";
//                ddlEmpresa.DataBind();
//                ddlEmpresa.Items.Insert(0, "-- Seleccione una empresa --");
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // Carga la tabla de empresas para SuperAdmin
//        private void CargarEmpresas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT e.EmpresaId, e.NombreEmpresa, e.Descripcion, " +
//                                  "COUNT(u.UsuarioId) AS TotalUsuarios " +
//                                  "FROM Empresas e " +
//                                  "LEFT JOIN Usuarios u ON e.EmpresaId = u.EmpresaId " +
//                                  "GROUP BY e.EmpresaId, e.NombreEmpresa, e.Descripcion " +
//                                  "ORDER BY e.NombreEmpresa";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                gvEmpresas.DataSource = dt;
//                gvEmpresas.DataBind();
//            }
//            catch (Exception ex)
//            {
//                lblMensajeEmpresa.Text = "Error: " + ex.Message;
//                lblMensajeEmpresa.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // Carga la tabla de usuarios según el rol
//        private void CargarUsuarios(string rol)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                string consulta = "";

//                if (rol == "SuperAdmin")
//                {
//                    // SuperAdmin ve todos los usuarios menos él mismo
//                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, 'Sistema') AS NombreEmpresa, " +
//                               "u.Nombre, u.Usuario, u.Rol, " +
//                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
//                               "FROM Usuarios u " +
//                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
//                               "WHERE u.Rol != 'SuperAdmin' " +
//                               "ORDER BY e.NombreEmpresa, u.Rol, u.Nombre";
//                }
//                else if (rol == "Admin")
//                {
//                    // Admin solo ve los encargados de su empresa
//                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, '') AS NombreEmpresa, " +
//                               "u.Nombre, u.Usuario, u.Rol, " +
//                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
//                               "FROM Usuarios u " +
//                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
//                               "WHERE u.EmpresaId = @empresaId AND u.Rol = 'Encargado' " +
//                               "ORDER BY u.Nombre";
//                }

//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

//                if (rol == "Admin")
//                    cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                gvUsuarios.DataSource = dt;
//                gvUsuarios.DataBind();
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // Guarda una nueva empresa
//        protected void btnGuardarEmpresa_Click(object sender, EventArgs e)
//        {
//            string nombre = txtNombreEmpresa.Text.Trim();
//            string descripcion = txtDescripcionEmpresa.Text.Trim();

//            if (nombre == "")
//            {
//                lblMensajeEmpresa.Text = "El nombre de la empresa es obligatorio.";
//                lblMensajeEmpresa.CssClass = "mensaje-error";
//                lblMensajeEmpresa.Visible = true;
//                return;
//            }

//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "INSERT INTO Empresas (NombreEmpresa, Descripcion) VALUES (@nombre, @descripcion)";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@nombre", nombre);
//                cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(descripcion) ? (object)DBNull.Value : descripcion);
//                cmd.ExecuteNonQuery();

//                txtNombreEmpresa.Text = "";
//                txtDescripcionEmpresa.Text = "";

//                lblMensajeEmpresa.Text = "✅ Empresa registrada correctamente.";
//                lblMensajeEmpresa.CssClass = "mensaje-exito";
//                lblMensajeEmpresa.Visible = true;

//                CargarEmpresas();
//                CargarDropdownEmpresas();
//            }
//            catch (Exception ex)
//            {
//                lblMensajeEmpresa.Text = "Error: " + ex.Message;
//                lblMensajeEmpresa.CssClass = "mensaje-error";
//                lblMensajeEmpresa.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // Guarda un nuevo usuario
//        protected void btnGuardar_Click(object sender, EventArgs e)
//        {
//            string nombre = txtNombre.Text.Trim();
//            string usuario = txtUsuario.Text.Trim();
//            string contrasena = txtContrasena.Text.Trim();
//            string rol = ddlRol.SelectedValue;
//            string rolActual = Session["Rol"].ToString();

//            if (nombre == "" || usuario == "" || contrasena == "")
//            {
//                lblMensaje.Text = "Todos los campos son obligatorios.";
//                lblMensaje.CssClass = "mensaje-error";
//                lblMensaje.Visible = true;
//                return;
//            }

//            // Determinar la empresa
//            string empresaId = "";
//            if (rolActual == "SuperAdmin")
//            {
//                if (ddlEmpresa.SelectedValue == "-- Seleccione una empresa --")
//                {
//                    lblMensaje.Text = "Seleccioná una empresa.";
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                    return;
//                }
//                empresaId = ddlEmpresa.SelectedValue;
//            }
//            else
//            {
//                empresaId = Session["EmpresaId"].ToString();
//            }

//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                // Verificar que el usuario no exista
//                string verificar = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario";
//                MySqlCommand cmdVerificar = new MySqlCommand(verificar, conexion.CONECTAR);
//                cmdVerificar.Parameters.AddWithValue("@usuario", usuario);
//                int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

//                if (existe > 0)
//                {
//                    lblMensaje.Text = "Ese nombre de usuario ya existe.";
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                    return;
//                }

//                string consulta = "INSERT INTO Usuarios (Nombre, Usuario, Contrasena, Rol, EmpresaId) " +
//                                  "VALUES (@nombre, @usuario, SHA2(@contrasena, 256), @rol, @empresaId)";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@nombre", nombre);
//                cmd.Parameters.AddWithValue("@usuario", usuario);
//                cmd.Parameters.AddWithValue("@contrasena", contrasena);
//                cmd.Parameters.AddWithValue("@rol", rol);
//                cmd.Parameters.AddWithValue("@empresaId", empresaId);
//                cmd.ExecuteNonQuery();

//                txtNombre.Text = "";
//                txtUsuario.Text = "";
//                txtContrasena.Text = "";

//                lblMensaje.Text = "✅ Usuario creado correctamente.";
//                lblMensaje.CssClass = "mensaje-exito";
//                lblMensaje.Visible = true;

//                CargarUsuarios(rolActual);
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.CssClass = "mensaje-error";
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // Maneja botones de la tabla de usuarios
//        protected void gvUsuarios_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
//        {
//            int index = Convert.ToInt32(e.CommandArgument);
//            string usuarioId = gvUsuarios.DataKeys[index].Value.ToString();
//            string rolActual = Session["Rol"].ToString();

//            if (e.CommandName == "Toggleactivo")
//            {
//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();

//                    // Cambiar el estado activo/inactivo
//                    string consulta = "UPDATE Usuarios SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END WHERE UsuarioId = @usuarioId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Estado del usuario actualizado.";
//                    lblMensaje.CssClass = "mensaje-exito";
//                    lblMensaje.Visible = true;

//                    CargarUsuarios(rolActual);
//                }
//                catch (Exception ex)
//                {
//                    lblMensaje.Text = "Error: " + ex.Message;
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//            else if (e.CommandName == "Eliminar")
//            {
//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();
//                    string consulta = "DELETE FROM Usuarios WHERE UsuarioId = @usuarioId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Usuario eliminado correctamente.";
//                    lblMensaje.CssClass = "mensaje-exito";
//                    lblMensaje.Visible = true;

//                    CargarUsuarios(rolActual);
//                }
//                catch (Exception ex)
//                {
//                    lblMensaje.Text = "Error: " + ex.Message;
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//        }

//        // Maneja botones de la tabla de empresas
//        protected void gvEmpresas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
//        {
//            if (e.CommandName == "Eliminar")
//            {
//                int index = Convert.ToInt32(e.CommandArgument);
//                string empresaId = gvEmpresas.DataKeys[index].Value.ToString();

//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();
//                    string consulta = "DELETE FROM Empresas WHERE EmpresaId = @empresaId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
//                    cmd.ExecuteNonQuery();

//                    lblMensajeEmpresa.Text = "✅ Empresa eliminada correctamente.";
//                    lblMensajeEmpresa.CssClass = "mensaje-exito";
//                    lblMensajeEmpresa.Visible = true;

//                    CargarEmpresas();
//                    CargarDropdownEmpresas();
//                }
//                catch (Exception ex)
//                {
//                    lblMensajeEmpresa.Text = "Error al eliminar: " + ex.Message;
//                    lblMensajeEmpresa.CssClass = "mensaje-error";
//                    lblMensajeEmpresa.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//        }

//        // Cerrar sesión
//        protected void btnCerrarSesion_Click(object sender, EventArgs e)
//        {
//            Session.Clear();
//            Session.Abandon();
//            Response.Redirect("Login.aspx");
//        }
//    }
//}

//using System;
//using MySql.Data.MySqlClient;
//using ClassLibrary1;
//using System.Data;

//namespace SistemaPina
//{
//    public partial class Usuarios : System.Web.UI.Page
//    {
//        // ==========================================
//        // VARIABLES DE ESTADO PARA EDITAR
//        // ==========================================
//        private bool Editando
//        {
//            get { return ViewState["EditandoUsuario"] != null && (bool)ViewState["EditandoUsuario"]; }
//            set { ViewState["EditandoUsuario"] = value; }
//        }

//        private string IdEditando
//        {
//            get { return ViewState["IdUsuarioEditando"] as string; }
//            set { ViewState["IdUsuarioEditando"] = value; }
//        }

//        // ==========================================
//        // DECLARACIÓN DE CONTROLES
//        // ==========================================
//        protected global::System.Web.UI.WebControls.Panel panelEmpresas;
//        protected global::System.Web.UI.WebControls.Panel panelSelectorEmpresa;
//        protected global::System.Web.UI.WebControls.Panel panelUsuarios;

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Session["UsuarioId"] == null)
//            {
//                Response.Redirect("Login.aspx");
//                return;
//            }

//            string rol = Session["Rol"].ToString();

//            // ==========================================
//            // MOSTRAR USUARIO Y EMPRESA
//            // ==========================================
//            lblNombreUsuario.Text = Session["Nombre"].ToString();

//            if (Session["NombreEmpresa"] != null && Session["NombreEmpresa"].ToString() != "")
//            {
//                //lblEmpresa.Text = Session["NombreEmpresa"].ToString();
//                //lblEmpresa.Visible = true;
//            }
//            else
//            {
//                //lblEmpresa.Visible = false;
//            }

//            // Solo Admin y SuperAdmin pueden entrar aquí
//            if (rol == "Encargado")
//            {
//                Response.Redirect("Dashboard.aspx");
//                return;
//            }

//            if (!IsPostBack)
//            {
//                ConfigurarVistaPorRol(rol);
//                CargarUsuarios(rol);

//                if (rol == "SuperAdmin")
//                {
//                    CargarEmpresas();
//                    CargarDropdownEmpresas();
//                }
//            }
//        }

//        // ==========================================
//        // CONFIGURAR VISTA SEGÚN ROL
//        // ==========================================
//        private void ConfigurarVistaPorRol(string rol)
//        {
//            if (rol == "SuperAdmin")
//            {
//                // SuperAdmin ve y gestiona empresas
//                panelEmpresas.Visible = true;
//                panelSelectorEmpresa.Visible = true;
//                lblDescripcion.Text = "Administrá las empresas y usuarios del sistema.";
//                lblSubtituloUsuarios.Text = "Todos los usuarios del sistema";
//                ddlRol.Items.Clear();
//                ddlRol.Items.Add("Admin");
//                ddlRol.Items.Add("Encargado");
//            }
//            else if (rol == "Admin")
//            {
//                // Admin solo ve su empresa
//                panelEmpresas.Visible = false;
//                panelSelectorEmpresa.Visible = false;
//                lblDescripcion.Text = "Administrá los encargados de tu empresa.";
//                lblSubtituloUsuarios.Text = "Encargados de tu empresa";
//                ddlRol.Items.Clear();
//                ddlRol.Items.Add("Encargado");
//            }
//        }

//        // ==========================================
//        // CARGAR DROPDOWN DE EMPRESAS
//        // ==========================================
//        private void CargarDropdownEmpresas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT EmpresaId, NombreEmpresa FROM Empresas WHERE Activa = 1 ORDER BY NombreEmpresa";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlEmpresa.DataSource = dt;
//                ddlEmpresa.DataTextField = "NombreEmpresa";
//                ddlEmpresa.DataValueField = "EmpresaId";
//                ddlEmpresa.DataBind();
//                ddlEmpresa.Items.Insert(0, "-- Seleccione una empresa --");
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // ==========================================
//        // CARGAR EMPRESAS
//        // ==========================================
//        private void CargarEmpresas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT e.EmpresaId, e.NombreEmpresa, e.Descripcion, " +
//                                  "COUNT(u.UsuarioId) AS TotalUsuarios " +
//                                  "FROM Empresas e " +
//                                  "LEFT JOIN Usuarios u ON e.EmpresaId = u.EmpresaId " +
//                                  "GROUP BY e.EmpresaId, e.NombreEmpresa, e.Descripcion " +
//                                  "ORDER BY e.NombreEmpresa";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                gvEmpresas.DataSource = dt;
//                gvEmpresas.DataBind();
//            }
//            catch (Exception ex)
//            {
//                lblMensajeEmpresa.Text = "Error: " + ex.Message;
//                lblMensajeEmpresa.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // ==========================================
//        // CARGAR USUARIOS
//        // ==========================================
//        private void CargarUsuarios(string rol)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                string consulta = "";

//                if (rol == "SuperAdmin")
//                {
//                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, 'Sistema') AS NombreEmpresa, " +
//                               "u.Nombre, u.Usuario, u.Rol, " +
//                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
//                               "FROM Usuarios u " +
//                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
//                               "WHERE u.Rol != 'SuperAdmin' " +
//                               "ORDER BY e.NombreEmpresa, u.Rol, u.Nombre";
//                }
//                else if (rol == "Admin")
//                {
//                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, '') AS NombreEmpresa, " +
//                               "u.Nombre, u.Usuario, u.Rol, " +
//                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
//                               "FROM Usuarios u " +
//                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
//                               "WHERE u.EmpresaId = @empresaId AND u.Rol = 'Encargado' " +
//                               "ORDER BY u.Nombre";
//                }

//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

//                if (rol == "Admin")
//                    cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                gvUsuarios.DataSource = dt;
//                gvUsuarios.DataBind();
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // ==========================================
//        // CARGAR DATOS PARA EDITAR USUARIO
//        // ==========================================
//        private void CargarUsuarioParaEditar(string id)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT Nombre, Usuario, Rol, EmpresaId FROM Usuarios WHERE UsuarioId = @id";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@id", id);
//                MySqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    txtNombre.Text = reader["Nombre"].ToString();
//                    txtUsuario.Text = reader["Usuario"].ToString();
//                    ddlRol.SelectedValue = reader["Rol"].ToString();

//                    if (Session["Rol"].ToString() == "SuperAdmin")
//                    {
//                        ddlEmpresa.SelectedValue = reader["EmpresaId"].ToString();
//                    }
//                }
//                reader.Close();
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error al cargar datos: " + ex.Message;
//                lblMensaje.Visible = true;
//            }
//            finally
//            {
//                conexion.CERRAR_CONEXION();
//            }
//        }

//        // ==========================================
//        // CANCELAR EDICIÓN
//        // ==========================================
//        protected void btnCancelarEdicion_Click(object sender, EventArgs e)
//        {
//            Editando = false;
//            IdEditando = null;
//            btnGuardar.Text = "Guardar usuario";
//            btnCancelarEdicion.Visible = false;
//            lblContrasenaAyuda.Visible = false;
//            txtNombre.Text = "";
//            txtUsuario.Text = "";
//            txtContrasena.Text = "";
//            lblMensaje.Visible = false;
//            CargarUsuarios(Session["Rol"].ToString());
//        }

//        // ==========================================
//        // GUARDAR EMPRESA
//        // ==========================================
//        protected void btnGuardarEmpresa_Click(object sender, EventArgs e)
//        {
//            string nombre = txtNombreEmpresa.Text.Trim();
//            string descripcion = txtDescripcionEmpresa.Text.Trim();

//            if (nombre == "")
//            {
//                lblMensajeEmpresa.Text = "El nombre de la empresa es obligatorio.";
//                lblMensajeEmpresa.CssClass = "mensaje-error";
//                lblMensajeEmpresa.Visible = true;
//                return;
//            }

//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "INSERT INTO Empresas (NombreEmpresa, Descripcion) VALUES (@nombre, @descripcion)";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@nombre", nombre);
//                cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(descripcion) ? (object)DBNull.Value : descripcion);
//                cmd.ExecuteNonQuery();

//                txtNombreEmpresa.Text = "";
//                txtDescripcionEmpresa.Text = "";

//                lblMensajeEmpresa.Text = "✅ Empresa registrada correctamente.";
//                lblMensajeEmpresa.CssClass = "mensaje-exito";
//                lblMensajeEmpresa.Visible = true;

//                CargarEmpresas();
//                CargarDropdownEmpresas();
//            }
//            catch (Exception ex)
//            {
//                lblMensajeEmpresa.Text = "Error: " + ex.Message;
//                lblMensajeEmpresa.CssClass = "mensaje-error";
//                lblMensajeEmpresa.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // ==========================================
//        // GUARDAR / ACTUALIZAR USUARIO
//        // ==========================================
//        protected void btnGuardar_Click(object sender, EventArgs e)
//        {
//            string nombre = txtNombre.Text.Trim();
//            string usuario = txtUsuario.Text.Trim();
//            string contrasena = txtContrasena.Text.Trim();
//            string rol = ddlRol.SelectedValue;
//            string rolActual = Session["Rol"].ToString();

//            if (nombre == "" || usuario == "")
//            {
//                lblMensaje.Text = "Nombre y usuario son obligatorios.";
//                lblMensaje.CssClass = "mensaje-error";
//                lblMensaje.Visible = true;
//                return;
//            }

//            // Si es nuevo usuario y no tiene contraseña, error
//            if (!Editando && contrasena == "")
//            {
//                lblMensaje.Text = "La contraseña es obligatoria para nuevos usuarios.";
//                lblMensaje.CssClass = "mensaje-error";
//                lblMensaje.Visible = true;
//                return;
//            }

//            string empresaId = "";
//            if (rolActual == "SuperAdmin")
//            {
//                if (ddlEmpresa.SelectedValue == "-- Seleccione una empresa --")
//                {
//                    lblMensaje.Text = "Seleccioná una empresa.";
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                    return;
//                }
//                empresaId = ddlEmpresa.SelectedValue;
//            }
//            else
//            {
//                empresaId = Session["EmpresaId"].ToString();
//            }

//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                if (Editando)
//                {
//                    // ==========================================
//                    // ACTUALIZAR USUARIO
//                    // ==========================================
//                    string consulta = "";
//                    MySqlCommand cmd;

//                    if (contrasena != "")
//                    {
//                        consulta = "UPDATE Usuarios SET Nombre = @nombre, Usuario = @usuario, Contrasena = SHA2(@contrasena, 256), " +
//                                   "Rol = @rol, EmpresaId = @empresaId WHERE UsuarioId = @id";
//                        cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
//                    }
//                    else
//                    {
//                        consulta = "UPDATE Usuarios SET Nombre = @nombre, Usuario = @usuario, " +
//                                   "Rol = @rol, EmpresaId = @empresaId WHERE UsuarioId = @id";
//                        cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    }

//                    cmd.Parameters.AddWithValue("@nombre", nombre);
//                    cmd.Parameters.AddWithValue("@usuario", usuario);
//                    cmd.Parameters.AddWithValue("@rol", rol);
//                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
//                    cmd.Parameters.AddWithValue("@id", IdEditando);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Usuario actualizado correctamente.";
//                    lblMensaje.CssClass = "mensaje-exito";

//                    Editando = false;
//                    IdEditando = null;
//                    btnGuardar.Text = "Guardar usuario";
//                }
//                else
//                {
//                    // ==========================================
//                    // INSERTAR NUEVO USUARIO
//                    // ==========================================
//                    string verificar = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario";
//                    MySqlCommand cmdVerificar = new MySqlCommand(verificar, conexion.CONECTAR);
//                    cmdVerificar.Parameters.AddWithValue("@usuario", usuario);
//                    int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

//                    if (existe > 0)
//                    {
//                        lblMensaje.Text = "Ese nombre de usuario ya existe.";
//                        lblMensaje.CssClass = "mensaje-error";
//                        lblMensaje.Visible = true;
//                        return;
//                    }

//                    string consulta = "INSERT INTO Usuarios (Nombre, Usuario, Contrasena, Rol, EmpresaId) " +
//                                      "VALUES (@nombre, @usuario, SHA2(@contrasena, 256), @rol, @empresaId)";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@nombre", nombre);
//                    cmd.Parameters.AddWithValue("@usuario", usuario);
//                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
//                    cmd.Parameters.AddWithValue("@rol", rol);
//                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Usuario creado correctamente.";
//                    lblMensaje.CssClass = "mensaje-exito";
//                }

//                txtNombre.Text = "";
//                txtUsuario.Text = "";
//                txtContrasena.Text = "";
//                lblMensaje.Visible = true;

//                CargarUsuarios(rolActual);
//            }
//            catch (Exception ex)
//            {
//                lblMensaje.Text = "Error: " + ex.Message;
//                lblMensaje.CssClass = "mensaje-error";
//                lblMensaje.Visible = true;
//            }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        // ==========================================
//        // ROWCOMMAND (Toggleactivo, Eliminar)
//        // ==========================================
//        protected void gvUsuarios_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
//        {
//            int index = Convert.ToInt32(e.CommandArgument);
//            string usuarioId = gvUsuarios.DataKeys[index].Value.ToString();
//            string rolActual = Session["Rol"].ToString();

//            if (e.CommandName == "Toggleactivo")
//            {
//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();

//                    string consulta = "UPDATE Usuarios SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END WHERE UsuarioId = @usuarioId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Estado del usuario actualizado.";
//                    lblMensaje.CssClass = "mensaje-exito";
//                    lblMensaje.Visible = true;

//                    CargarUsuarios(rolActual);
//                }
//                catch (Exception ex)
//                {
//                    lblMensaje.Text = "Error: " + ex.Message;
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//            else if (e.CommandName == "Eliminar")
//            {
//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();
//                    string consulta = "DELETE FROM Usuarios WHERE UsuarioId = @usuarioId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
//                    cmd.ExecuteNonQuery();

//                    lblMensaje.Text = "✅ Usuario eliminado correctamente.";
//                    lblMensaje.CssClass = "mensaje-exito";
//                    lblMensaje.Visible = true;

//                    CargarUsuarios(rolActual);
//                }
//                catch (Exception ex)
//                {
//                    lblMensaje.Text = "Error: " + ex.Message;
//                    lblMensaje.CssClass = "mensaje-error";
//                    lblMensaje.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//        }

//        // ==========================================
//        // ELIMINAR EMPRESA
//        // ==========================================
//        protected void gvEmpresas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
//        {
//            if (e.CommandName == "Eliminar")
//            {
//                int index = Convert.ToInt32(e.CommandArgument);
//                string empresaId = gvEmpresas.DataKeys[index].Value.ToString();

//                CLASS_CONEXION conexion = new CLASS_CONEXION();
//                try
//                {
//                    conexion.ABRIR_CONEXION();
//                    string consulta = "DELETE FROM Empresas WHERE EmpresaId = @empresaId";
//                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
//                    cmd.ExecuteNonQuery();

//                    lblMensajeEmpresa.Text = "✅ Empresa eliminada correctamente.";
//                    lblMensajeEmpresa.CssClass = "mensaje-exito";
//                    lblMensajeEmpresa.Visible = true;

//                    CargarEmpresas();
//                    CargarDropdownEmpresas();
//                }
//                catch (Exception ex)
//                {
//                    lblMensajeEmpresa.Text = "Error al eliminar: " + ex.Message;
//                    lblMensajeEmpresa.CssClass = "mensaje-error";
//                    lblMensajeEmpresa.Visible = true;
//                }
//                finally { conexion.CERRAR_CONEXION(); }
//            }
//        }

//        // ==========================================
//        // CERRAR SESIÓN
//        // ==========================================
//        protected void btnCerrarSesion_Click(object sender, EventArgs e)
//        {
//            Session.Clear();
//            Session.Abandon();
//            Response.Redirect("Login.aspx");
//        }
//    }
//}

using System;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;

namespace SistemaPina
{
    public partial class Usuarios : System.Web.UI.Page
    {
        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR USUARIO
        // ==========================================
        private bool Editando
        {
            get { return ViewState["EditandoUsuario"] != null && (bool)ViewState["EditandoUsuario"]; }
            set { ViewState["EditandoUsuario"] = value; }
        }

        private string IdEditando
        {
            get { return ViewState["IdUsuarioEditando"] as string; }
            set { ViewState["IdUsuarioEditando"] = value; }
        }

        // ==========================================
        // VARIABLES DE ESTADO PARA EDITAR EMPRESA
        // ==========================================
        private bool EditandoEmpresa
        {
            get { return ViewState["EditandoEmpresa"] != null && (bool)ViewState["EditandoEmpresa"]; }
            set { ViewState["EditandoEmpresa"] = value; }
        }

        private string IdEmpresaEditando
        {
            get { return ViewState["IdEmpresaEditando"] as string; }
            set { ViewState["IdEmpresaEditando"] = value; }
        }

        // ==========================================
        // DECLARACIÓN DE CONTROLES
        // ==========================================
        protected global::System.Web.UI.WebControls.Panel panelEmpresas;
        protected global::System.Web.UI.WebControls.Panel panelSelectorEmpresa;
        protected global::System.Web.UI.WebControls.Panel panelUsuarios;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string rol = Session["Rol"].ToString();

            // ==========================================
            // MOSTRAR USUARIO Y EMPRESA
            // ==========================================
            lblNombreUsuario.Text = Session["Nombre"].ToString();

            // Solo Admin y SuperAdmin pueden entrar aquí
            if (rol == "Encargado")
            {
                Response.Redirect("Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ConfigurarVistaPorRol(rol);
                CargarUsuarios(rol);

                if (rol == "SuperAdmin")
                {
                    CargarEmpresas();
                    CargarDropdownEmpresas();
                }
            }
        }

        // ==========================================
        // CONFIGURAR VISTA SEGÚN ROL
        // ==========================================
        private void ConfigurarVistaPorRol(string rol)
        {
            if (rol == "SuperAdmin")
            {
                panelEmpresas.Visible = true;
                panelSelectorEmpresa.Visible = true;
                lblDescripcion.Text = "Administrá las empresas y usuarios del sistema.";
                lblSubtituloUsuarios.Text = "Todos los usuarios del sistema";
                ddlRol.Items.Clear();
                ddlRol.Items.Add("Admin");
                ddlRol.Items.Add("Encargado");
            }
            else if (rol == "Admin")
            {
                panelEmpresas.Visible = false;
                panelSelectorEmpresa.Visible = false;
                lblDescripcion.Text = "Administrá los encargados de tu empresa.";
                lblSubtituloUsuarios.Text = "Encargados de tu empresa";
                ddlRol.Items.Clear();
                ddlRol.Items.Add("Encargado");
            }
        }

        // ==========================================
        // CARGAR DROPDOWN DE EMPRESAS
        // ==========================================
        private void CargarDropdownEmpresas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT EmpresaId, NombreEmpresa FROM Empresas WHERE Activa = 1 ORDER BY NombreEmpresa";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlEmpresa.DataSource = dt;
                ddlEmpresa.DataTextField = "NombreEmpresa";
                ddlEmpresa.DataValueField = "EmpresaId";
                ddlEmpresa.DataBind();
                ddlEmpresa.Items.Insert(0, "-- Seleccione una empresa --");
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // CARGAR EMPRESAS
        // ==========================================
        private void CargarEmpresas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT e.EmpresaId, e.NombreEmpresa, e.Descripcion, " +
                                  "COUNT(u.UsuarioId) AS TotalUsuarios " +
                                  "FROM Empresas e " +
                                  "LEFT JOIN Usuarios u ON e.EmpresaId = u.EmpresaId " +
                                  "GROUP BY e.EmpresaId, e.NombreEmpresa, e.Descripcion " +
                                  "ORDER BY e.NombreEmpresa";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvEmpresas.DataSource = dt;
                gvEmpresas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensajeEmpresa.Text = "Error: " + ex.Message;
                lblMensajeEmpresa.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // CARGAR USUARIOS
        // ==========================================
        private void CargarUsuarios(string rol)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = "";

                if (rol == "SuperAdmin")
                {
                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, 'Sistema') AS NombreEmpresa, " +
                               "u.Nombre, u.Usuario, u.Rol, " +
                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
                               "FROM Usuarios u " +
                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
                               "WHERE u.Rol != 'SuperAdmin' " +
                               "ORDER BY e.NombreEmpresa, u.Rol, u.Nombre";
                }
                else if (rol == "Admin")
                {
                    consulta = "SELECT u.UsuarioId, IFNULL(e.NombreEmpresa, '') AS NombreEmpresa, " +
                               "u.Nombre, u.Usuario, u.Rol, " +
                               "CASE WHEN u.Activo = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado " +
                               "FROM Usuarios u " +
                               "LEFT JOIN Empresas e ON u.EmpresaId = e.EmpresaId " +
                               "WHERE u.EmpresaId = @empresaId AND u.Rol = 'Encargado' " +
                               "ORDER BY u.Nombre";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);

                if (rol == "Admin")
                    cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvUsuarios.DataSource = dt;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR EMPRESA
        // ==========================================
        private void CargarEmpresaParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT NombreEmpresa, Descripcion FROM Empresas WHERE EmpresaId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtNombreEmpresa.Text = reader["NombreEmpresa"].ToString();
                    txtDescripcionEmpresa.Text = reader["Descripcion"].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMensajeEmpresa.Text = "Error al cargar datos: " + ex.Message;
                lblMensajeEmpresa.Visible = true;
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        // ==========================================
        // CARGAR DATOS PARA EDITAR USUARIO
        // ==========================================
        private void CargarUsuarioParaEditar(string id)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = "SELECT Nombre, Usuario, Rol, EmpresaId FROM Usuarios WHERE UsuarioId = @id";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtNombre.Text = reader["Nombre"].ToString();
                    txtUsuario.Text = reader["Usuario"].ToString();
                    ddlRol.SelectedValue = reader["Rol"].ToString();

                    if (Session["Rol"].ToString() == "SuperAdmin")
                    {
                        ddlEmpresa.SelectedValue = reader["EmpresaId"].ToString();
                    }
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
        // CANCELAR EDICIÓN DE EMPRESA
        // ==========================================
        protected void btnCancelarEdicionEmpresa_Click(object sender, EventArgs e)
        {
            EditandoEmpresa = false;
            IdEmpresaEditando = null;
            btnGuardarEmpresa.Text = "Guardar empresa";
            btnCancelarEdicionEmpresa.Visible = false;
            //lblTituloEmpresa.Text = "Agregar nueva empresa";
            txtNombreEmpresa.Text = "";
            txtDescripcionEmpresa.Text = "";
            lblMensajeEmpresa.Visible = false;
            CargarEmpresas();
        }

        // ==========================================
        // CANCELAR EDICIÓN DE USUARIO
        // ==========================================
        protected void btnCancelarEdicion_Click(object sender, EventArgs e)
        {
            Editando = false;
            IdEditando = null;
            btnGuardar.Text = "Guardar usuario";
            btnCancelarEdicion.Visible = false;
            lblContrasenaAyuda.Visible = false;
            txtNombre.Text = "";
            txtUsuario.Text = "";
            txtContrasena.Text = "";
            lblMensaje.Visible = false;
            CargarUsuarios(Session["Rol"].ToString());
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR EMPRESA
        // ==========================================
        protected void btnGuardarEmpresa_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreEmpresa.Text.Trim();
            string descripcion = txtDescripcionEmpresa.Text.Trim();

            if (nombre == "")
            {
                lblMensajeEmpresa.Text = "El nombre de la empresa es obligatorio.";
                lblMensajeEmpresa.CssClass = "mensaje-error";
                lblMensajeEmpresa.Visible = true;
                return;
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                if (EditandoEmpresa)
                {
                    string consulta = "UPDATE Empresas SET NombreEmpresa = @nombre, Descripcion = @descripcion WHERE EmpresaId = @id";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@id", IdEmpresaEditando);
                    cmd.ExecuteNonQuery();

                    lblMensajeEmpresa.Text = "✅ Empresa actualizada correctamente.";
                    lblMensajeEmpresa.CssClass = "mensaje-exito";

                    EditandoEmpresa = false;
                    IdEmpresaEditando = null;
                    btnGuardarEmpresa.Text = "Guardar empresa";
                    btnCancelarEdicionEmpresa.Visible = false;
                    //lblTituloEmpresa.Text = "Agregar nueva empresa";
                }
                else
                {
                    string consulta = "INSERT INTO Empresas (NombreEmpresa, Descripcion) VALUES (@nombre, @descripcion)";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(descripcion) ? (object)DBNull.Value : descripcion);
                    cmd.ExecuteNonQuery();

                    lblMensajeEmpresa.Text = "✅ Empresa registrada correctamente.";
                    lblMensajeEmpresa.CssClass = "mensaje-exito";
                }

                txtNombreEmpresa.Text = "";
                txtDescripcionEmpresa.Text = "";
                lblMensajeEmpresa.Visible = true;

                CargarEmpresas();
                CargarDropdownEmpresas();
            }
            catch (Exception ex)
            {
                lblMensajeEmpresa.Text = "Error: " + ex.Message;
                lblMensajeEmpresa.CssClass = "mensaje-error";
                lblMensajeEmpresa.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // GUARDAR / ACTUALIZAR USUARIO
        // ==========================================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();
            string rol = ddlRol.SelectedValue;
            string rolActual = Session["Rol"].ToString();

            if (nombre == "" || usuario == "")
            {
                lblMensaje.Text = "Nombre y usuario son obligatorios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            if (!Editando && contrasena == "")
            {
                lblMensaje.Text = "La contraseña es obligatoria para nuevos usuarios.";
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
                return;
            }

            string empresaId = "";
            if (rolActual == "SuperAdmin")
            {
                if (ddlEmpresa.SelectedValue == "-- Seleccione una empresa --")
                {
                    lblMensaje.Text = "Seleccioná una empresa.";
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Visible = true;
                    return;
                }
                empresaId = ddlEmpresa.SelectedValue;
            }
            else
            {
                empresaId = Session["EmpresaId"].ToString();
            }

            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                if (Editando)
                {
                    string consulta = "";
                    MySqlCommand cmd;

                    if (contrasena != "")
                    {
                        consulta = "UPDATE Usuarios SET Nombre = @nombre, Usuario = @usuario, Contrasena = SHA2(@contrasena, 256), " +
                                   "Rol = @rol, EmpresaId = @empresaId WHERE UsuarioId = @id";
                        cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    }
                    else
                    {
                        consulta = "UPDATE Usuarios SET Nombre = @nombre, Usuario = @usuario, " +
                                   "Rol = @rol, EmpresaId = @empresaId WHERE UsuarioId = @id";
                        cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    }

                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@rol", rol);
                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
                    cmd.Parameters.AddWithValue("@id", IdEditando);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Usuario actualizado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";

                    Editando = false;
                    IdEditando = null;
                    btnGuardar.Text = "Guardar usuario";
                }
                else
                {
                    string verificar = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario";
                    MySqlCommand cmdVerificar = new MySqlCommand(verificar, conexion.CONECTAR);
                    cmdVerificar.Parameters.AddWithValue("@usuario", usuario);
                    int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                    if (existe > 0)
                    {
                        lblMensaje.Text = "Ese nombre de usuario ya existe.";
                        lblMensaje.CssClass = "mensaje-error";
                        lblMensaje.Visible = true;
                        return;
                    }

                    string consulta = "INSERT INTO Usuarios (Nombre, Usuario, Contrasena, Rol, EmpresaId) " +
                                      "VALUES (@nombre, @usuario, SHA2(@contrasena, 256), @rol, @empresaId)";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);
                    cmd.Parameters.AddWithValue("@rol", rol);
                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Usuario creado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }

                txtNombre.Text = "";
                txtUsuario.Text = "";
                txtContrasena.Text = "";
                lblMensaje.Visible = true;
                CargarUsuarios(rolActual);
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Visible = true;
            }
            finally { conexion.CERRAR_CONEXION(); }
        }

        // ==========================================
        // ROWCOMMAND EMPRESAS
        // ==========================================
        protected void gvEmpresas_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string empresaId = e.CommandArgument.ToString();

            if (e.CommandName == "EditarEmpresa")
            {
                CargarEmpresaParaEditar(empresaId);
                EditandoEmpresa = true;
                IdEmpresaEditando = empresaId;
                btnGuardarEmpresa.Text = "✅ Actualizar Empresa";
                btnCancelarEdicionEmpresa.Visible = true;
                //lblTituloEmpresa.Text = "Editar empresa";
            }
            else if (e.CommandName == "EliminarEmpresa")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Empresas WHERE EmpresaId = @empresaId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@empresaId", empresaId);
                    cmd.ExecuteNonQuery();

                    lblMensajeEmpresa.Text = "✅ Empresa eliminada correctamente.";
                    lblMensajeEmpresa.CssClass = "mensaje-exito";
                    lblMensajeEmpresa.Visible = true;

                    CargarEmpresas();
                    CargarDropdownEmpresas();
                }
                catch (Exception ex)
                {
                    lblMensajeEmpresa.Text = "Error al eliminar: " + ex.Message;
                    lblMensajeEmpresa.CssClass = "mensaje-error";
                    lblMensajeEmpresa.Visible = true;
                }
                finally { conexion.CERRAR_CONEXION(); }
            }
        }

        // ==========================================
        // ROWCOMMAND USUARIOS
        // ==========================================
        protected void gvUsuarios_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string usuarioId = e.CommandArgument.ToString();
            string rolActual = Session["Rol"].ToString();

            if (e.CommandName == "Editar")
            {
                CargarUsuarioParaEditar(usuarioId);
                Editando = true;
                IdEditando = usuarioId;
                btnGuardar.Text = "✅ Actualizar Usuario";
                btnCancelarEdicion.Visible = true;
                lblContrasenaAyuda.Visible = true;
            }
            else if (e.CommandName == "Eliminar")
            {
                CLASS_CONEXION conexion = new CLASS_CONEXION();
                try
                {
                    conexion.ABRIR_CONEXION();
                    string consulta = "DELETE FROM Usuarios WHERE UsuarioId = @usuarioId";
                    MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.ExecuteNonQuery();

                    lblMensaje.Text = "✅ Usuario eliminado correctamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                    lblMensaje.Visible = true;

                    CargarUsuarios(rolActual);
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
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