//using System;
//using System.Text;
//using MySql.Data.MySqlClient;
//using ClassLibrary1;
//using System.Data;

//namespace SistemaPina
//{
//    public partial class Reportes : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Session["UsuarioId"] == null)
//            {
//                Response.Redirect("Login.aspx");
//                return;
//            }
//            // SuperAdmin no tiene acceso a este módulo
//            if (Session["Rol"].ToString() == "SuperAdmin")
//            {
//                Response.Redirect("Usuarios.aspx");
//                return;
//            }

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

//            if (Session["Rol"].ToString() != "Admin")
//            {
//                panelUsuarios.Visible = false;
//            }

//            if (!IsPostBack)
//            {
//                CargarFincas();
//            }
//        }

//        private void CargarFincas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT FincaId, Nombre FROM Fincas WHERE EmpresaId = @empresaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                ddlFincaPlagas.DataSource = dt;
//                ddlFincaPlagas.DataTextField = "Nombre";
//                ddlFincaPlagas.DataValueField = "FincaId";
//                ddlFincaPlagas.DataBind();
//                ddlFincaPlagas.Items.Insert(0, "-- Todas las fincas --");

//                ddlFincaLabores.DataSource = dt;
//                ddlFincaLabores.DataTextField = "Nombre";
//                ddlFincaLabores.DataValueField = "FincaId";
//                ddlFincaLabores.DataBind();
//                ddlFincaLabores.Items.Insert(0, "-- Todas las fincas --");

//                ddlFincaRendimiento.DataSource = dt;
//                ddlFincaRendimiento.DataTextField = "Nombre";
//                ddlFincaRendimiento.DataValueField = "FincaId";
//                ddlFincaRendimiento.DataBind();
//                ddlFincaRendimiento.Items.Insert(0, "-- Todas las fincas --");

//                CargarLotesPlagas();
//                CargarLotesLabores();
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        private void CargarLotesPlagas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = ddlFincaPlagas.SelectedValue == "-- Todas las fincas --"
//                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
//                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlLotePlagas.DataSource = dt;
//                ddlLotePlagas.DataTextField = "Nombre";
//                ddlLotePlagas.DataValueField = "LoteId";
//                ddlLotePlagas.DataBind();
//                ddlLotePlagas.Items.Insert(0, "-- Todos los lotes --");
//            }
//            catch { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        private void CargarLotesLabores()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = ddlFincaLabores.SelectedValue == "-- Todas las fincas --"
//                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
//                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlLoteLabores.DataSource = dt;
//                ddlLoteLabores.DataTextField = "Nombre";
//                ddlLoteLabores.DataValueField = "LoteId";
//                ddlLoteLabores.DataBind();
//                ddlLoteLabores.Items.Insert(0, "-- Todos los lotes --");
//            }
//            catch { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void ddlFincaPlagas_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            CargarLotesPlagas();
//        }

//        protected void ddlFincaLabores_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            CargarLotesLabores();
//        }

//        protected void btnGenerarPlagas_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("'Plaga' AS Tipo, p.NombrePlaga AS Nombre, p.NivelAfectacion, p.FechaDeteccion ");
//                sb.Append("FROM Plagas p ");
//                sb.Append("LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId ");
//                sb.Append("LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId ");
//                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    sb.Append("AND p.FechaDeteccion >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    sb.Append("AND p.FechaDeteccion <= @fechaHasta ");
//                sb.Append("UNION ALL ");
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("'Enfermedad' AS Tipo, en.NombreEnfermedad AS Nombre, en.NivelAfectacion, en.FechaDeteccion ");
//                sb.Append("FROM Enfermedades en ");
//                sb.Append("LEFT JOIN EnfermedadBloques eb ON en.EnfermedadId = eb.EnfermedadId ");
//                sb.Append("LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId ");
//                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    sb.Append("AND en.FechaDeteccion >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    sb.Append("AND en.FechaDeteccion <= @fechaHasta ");
//                sb.Append("ORDER BY FechaDeteccion DESC");

//                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    cmd.Parameters.AddWithValue("@loteId", ddlLotePlagas.SelectedValue);
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde1.Text);
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta1.Text);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReportePlagas.DataSource = dt;
//                gvReportePlagas.DataBind();
//                panelResultadoPlagas.Visible = true;

//                var conteo = new System.Collections.Generic.Dictionary<string, int>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    string nombre = row["Nombre"].ToString();
//                    if (conteo.ContainsKey(nombre)) conteo[nombre]++;
//                    else conteo[nombre] = 1;
//                }

//                var labels = new System.Collections.Generic.List<string>();
//                var valores = new System.Collections.Generic.List<int>();
//                foreach (var item in conteo)
//                {
//                    labels.Add(item.Key);
//                    valores.Add(item.Value);
//                }

//                hfDatosPlagas.Value = $"{{\"labels\":[{string.Join(",", labels.ConvertAll(l => $"\"{l}\""))}],\"valores\":[{string.Join(",", valores)}]}}";
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnGenerarLabores_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("la.TipoLabor, la.FechaLabor, la.Responsable, la.Observaciones ");
//                sb.Append("FROM Labores la ");
//                sb.Append("INNER JOIN Bloques b ON la.BloqueId = b.BloqueId ");
//                sb.Append("INNER JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("INNER JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
//                    sb.Append("AND la.FechaLabor >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
//                    sb.Append("AND la.FechaLabor <= @fechaHasta ");
//                sb.Append("ORDER BY la.FechaLabor DESC");

//                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
//                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
//                    cmd.Parameters.AddWithValue("@loteId", ddlLoteLabores.SelectedValue);
//                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
//                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde2.Text);
//                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
//                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta2.Text);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReporteLabores.DataSource = dt;
//                gvReporteLabores.DataBind();
//                panelResultadoLabores.Visible = true;

//                var conteo = new System.Collections.Generic.Dictionary<string, int>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    string tipo = row["TipoLabor"].ToString();
//                    if (conteo.ContainsKey(tipo)) conteo[tipo]++;
//                    else conteo[tipo] = 1;
//                }

//                var labels = new System.Collections.Generic.List<string>();
//                var valores = new System.Collections.Generic.List<int>();
//                foreach (var item in conteo)
//                {
//                    labels.Add(item.Key);
//                    valores.Add(item.Value);
//                }

//                hfDatosLabores.Value = $"{{\"labels\":[{string.Join(",", labels.ConvertAll(l => $"\"{l}\""))}],\"valores\":[{string.Join(",", valores)}]}}";
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnGenerarRendimiento_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("SELECT f.Nombre AS NombreFinca, g.Nombre AS NombreGrupo, ");
//                sb.Append("GROUP_CONCAT(DISTINCT b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques, ");
//                sb.Append("c.FechaCosecha, c.KilosCosechados ");
//                sb.Append("FROM Cosechas c ");
//                sb.Append("INNER JOIN GruposForza g ON c.GrupoForzaId = g.GrupoForzaId ");
//                sb.Append("INNER JOIN Fincas f ON g.FincaId = f.FincaId ");
//                sb.Append("LEFT JOIN GrupoForzaBloques gb ON g.GrupoForzaId = gb.GrupoForzaId ");
//                sb.Append("LEFT JOIN Bloques b ON gb.BloqueId = b.BloqueId ");
//                sb.Append("GROUP BY f.Nombre, g.Nombre, c.FechaCosecha, c.KilosCosechados ");
//                sb.Append("ORDER BY c.FechaCosecha DESC");

//                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
//                    sb.Insert(sb.ToString().IndexOf("GROUP BY"), "AND f.FincaId = @fincaId ");

//                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
//                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaRendimiento.SelectedValue);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReporteRendimiento.DataSource = dt;
//                gvReporteRendimiento.DataBind();
//                panelResultadoRendimiento.Visible = true;

//                var labels = new System.Collections.Generic.List<string>();
//                var valores = new System.Collections.Generic.List<string>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    labels.Add(row["NombreGrupo"].ToString());
//                    valores.Add(row["KilosCosechados"].ToString());
//                }

//                hfDatosRendimiento.Value = $"{{\"labels\":[{string.Join(",", labels.ConvertAll(l => $"\"{l}\""))}],\"valores\":[{string.Join(",", valores)}]}}";
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnCerrarSesion_Click(object sender, EventArgs e)
//        {
//            Session.Clear();
//            Session.Abandon();
//            Response.Redirect("Login.aspx");
//        }
//    }
//}

//using System;
//using System.Text;
//using MySql.Data.MySqlClient;
//using ClassLibrary1;
//using System.Data;
//using System.Collections.Generic;
//using System.Text.Json;  // 👈 AGREGAR ESTA LÍNEA

//namespace SistemaPina
//{
//    public partial class Reportes : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (Session["UsuarioId"] == null)
//            {
//                Response.Redirect("Login.aspx");
//                return;
//            }
//            // SuperAdmin no tiene acceso a este módulo
//            if (Session["Rol"].ToString() == "SuperAdmin")
//            {
//                Response.Redirect("Usuarios.aspx");
//                return;
//            }

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

//            if (Session["Rol"].ToString() != "Admin")
//            {
//                panelUsuarios.Visible = false;
//            }

//            if (!IsPostBack)
//            {
//                CargarFincas();
//            }
//        }

//        private void CargarFincas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = "SELECT FincaId, Nombre FROM Fincas WHERE EmpresaId = @empresaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                ddlFincaPlagas.DataSource = dt;
//                ddlFincaPlagas.DataTextField = "Nombre";
//                ddlFincaPlagas.DataValueField = "FincaId";
//                ddlFincaPlagas.DataBind();
//                ddlFincaPlagas.Items.Insert(0, "-- Todas las fincas --");

//                ddlFincaLabores.DataSource = dt;
//                ddlFincaLabores.DataTextField = "Nombre";
//                ddlFincaLabores.DataValueField = "FincaId";
//                ddlFincaLabores.DataBind();
//                ddlFincaLabores.Items.Insert(0, "-- Todas las fincas --");

//                ddlFincaRendimiento.DataSource = dt;
//                ddlFincaRendimiento.DataTextField = "Nombre";
//                ddlFincaRendimiento.DataValueField = "FincaId";
//                ddlFincaRendimiento.DataBind();
//                ddlFincaRendimiento.Items.Insert(0, "-- Todas las fincas --");

//                CargarLotesPlagas();
//                CargarLotesLabores();
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        private void CargarLotesPlagas()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = ddlFincaPlagas.SelectedValue == "-- Todas las fincas --"
//                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
//                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlLotePlagas.DataSource = dt;
//                ddlLotePlagas.DataTextField = "Nombre";
//                ddlLotePlagas.DataValueField = "LoteId";
//                ddlLotePlagas.DataBind();
//                ddlLotePlagas.Items.Insert(0, "-- Todos los lotes --");
//            }
//            catch { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        private void CargarLotesLabores()
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();
//                string consulta = ddlFincaLabores.SelectedValue == "-- Todas las fincas --"
//                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
//                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                ddlLoteLabores.DataSource = dt;
//                ddlLoteLabores.DataTextField = "Nombre";
//                ddlLoteLabores.DataValueField = "LoteId";
//                ddlLoteLabores.DataBind();
//                ddlLoteLabores.Items.Insert(0, "-- Todos los lotes --");
//            }
//            catch { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void ddlFincaPlagas_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            CargarLotesPlagas();
//        }

//        protected void ddlFincaLabores_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            CargarLotesLabores();
//        }

//        protected void btnGenerarPlagas_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("'Plaga' AS Tipo, p.NombrePlaga AS Nombre, p.NivelAfectacion, p.FechaDeteccion ");
//                sb.Append("FROM Plagas p ");
//                sb.Append("LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId ");
//                sb.Append("LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId ");
//                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    sb.Append("AND p.FechaDeteccion >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    sb.Append("AND p.FechaDeteccion <= @fechaHasta ");
//                sb.Append("UNION ALL ");
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("'Enfermedad' AS Tipo, en.NombreEnfermedad AS Nombre, en.NivelAfectacion, en.FechaDeteccion ");
//                sb.Append("FROM Enfermedades en ");
//                sb.Append("LEFT JOIN EnfermedadBloques eb ON en.EnfermedadId = eb.EnfermedadId ");
//                sb.Append("LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId ");
//                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    sb.Append("AND en.FechaDeteccion >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    sb.Append("AND en.FechaDeteccion <= @fechaHasta ");
//                sb.Append("ORDER BY FechaDeteccion DESC");

//                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
//                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
//                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
//                    cmd.Parameters.AddWithValue("@loteId", ddlLotePlagas.SelectedValue);
//                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
//                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde1.Text);
//                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
//                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta1.Text);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReportePlagas.DataSource = dt;
//                gvReportePlagas.DataBind();
//                panelResultadoPlagas.Visible = true;

//                var conteo = new Dictionary<string, int>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    string nombre = row["Nombre"].ToString();
//                    if (conteo.ContainsKey(nombre)) conteo[nombre]++;
//                    else conteo[nombre] = 1;
//                }

//                var labels = new List<string>();
//                var valores = new List<int>();
//                foreach (var item in conteo)
//                {
//                    labels.Add(item.Key);
//                    valores.Add(item.Value);
//                }

//                hfDatosPlagas.Value = JsonSerializer.Serialize(new { labels, valores });
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnGenerarLabores_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                StringBuilder sb = new StringBuilder();
//                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
//                sb.Append("la.TipoLabor, la.FechaLabor, la.Responsable, la.Observaciones ");
//                sb.Append("FROM Labores la ");
//                sb.Append("INNER JOIN Bloques b ON la.BloqueId = b.BloqueId ");
//                sb.Append("INNER JOIN Lotes l ON b.LoteId = l.LoteId ");
//                sb.Append("INNER JOIN Fincas f ON l.FincaId = f.FincaId ");
//                sb.Append("WHERE 1=1 ");
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    sb.Append("AND f.FincaId = @fincaId ");
//                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
//                    sb.Append("AND l.LoteId = @loteId ");
//                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
//                    sb.Append("AND la.FechaLabor >= @fechaDesde ");
//                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
//                    sb.Append("AND la.FechaLabor <= @fechaHasta ");
//                sb.Append("ORDER BY la.FechaLabor DESC");

//                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
//                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
//                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
//                    cmd.Parameters.AddWithValue("@loteId", ddlLoteLabores.SelectedValue);
//                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
//                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde2.Text);
//                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
//                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta2.Text);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReporteLabores.DataSource = dt;
//                gvReporteLabores.DataBind();
//                panelResultadoLabores.Visible = true;

//                var conteo = new Dictionary<string, int>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    string tipo = row["TipoLabor"].ToString();
//                    if (conteo.ContainsKey(tipo)) conteo[tipo]++;
//                    else conteo[tipo] = 1;
//                }

//                var labels = new List<string>();
//                var valores = new List<int>();
//                foreach (var item in conteo)
//                {
//                    labels.Add(item.Key);
//                    valores.Add(item.Value);
//                }

//                hfDatosLabores.Value = JsonSerializer.Serialize(new { labels, valores });
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnGenerarRendimiento_Click(object sender, EventArgs e)
//        {
//            CLASS_CONEXION conexion = new CLASS_CONEXION();
//            try
//            {
//                conexion.ABRIR_CONEXION();

//                string consulta = @"
//                    SELECT f.Nombre AS NombreFinca, g.Nombre AS NombreGrupo, 
//                           GROUP_CONCAT(DISTINCT b.Nombre ORDER BY b.Nombre SEPARATOR ', ') AS Bloques,
//                           (SELECT SUM(s.CantidadPlantas) 
//                            FROM Siembras s 
//                            INNER JOIN GrupoForzaBloques gb2 ON s.BloqueId = gb2.BloqueId 
//                            WHERE gb2.GrupoForzaId = g.GrupoForzaId) AS TotalPlantas,
//                           c.FechaCosecha, c.KilosCosechados,
//                           ROUND(c.KilosCosechados / 
//                               (SELECT SUM(s.CantidadPlantas) 
//                                FROM Siembras s 
//                                INNER JOIN GrupoForzaBloques gb2 ON s.BloqueId = gb2.BloqueId 
//                                WHERE gb2.GrupoForzaId = g.GrupoForzaId), 2) AS KgPorPlanta
//                    FROM Cosechas c
//                    INNER JOIN GruposForza g ON c.GrupoForzaId = g.GrupoForzaId
//                    INNER JOIN Fincas f ON g.FincaId = f.FincaId
//                    LEFT JOIN GrupoForzaBloques gb ON g.GrupoForzaId = gb.GrupoForzaId
//                    LEFT JOIN Bloques b ON gb.BloqueId = b.BloqueId
//                    WHERE f.EmpresaId = @empresaId 
//                    AND c.GrupoForzaId IS NOT NULL";

//                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
//                    consulta += " AND f.FincaId = @fincaId";

//                consulta += " GROUP BY f.Nombre, g.Nombre, c.FechaCosecha, c.KilosCosechados";

//                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
//                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
//                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
//                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaRendimiento.SelectedValue);

//                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvReporteRendimiento.DataSource = dt;
//                gvReporteRendimiento.DataBind();
//                panelResultadoRendimiento.Visible = true;

//                var labels = new List<string>();
//                var valores = new List<decimal>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    labels.Add(row["NombreGrupo"].ToString());
//                    decimal kgPorPlanta = row["KgPorPlanta"] != DBNull.Value ? Convert.ToDecimal(row["KgPorPlanta"]) : 0;
//                    valores.Add(kgPorPlanta);
//                }

//                hfDatosRendimiento.Value = JsonSerializer.Serialize(new { labels, valores });
//            }
//            catch (Exception ex) { }
//            finally { conexion.CERRAR_CONEXION(); }
//        }

//        protected void btnCerrarSesion_Click(object sender, EventArgs e)
//        {
//            Session.Clear();
//            Session.Abandon();
//            Response.Redirect("Login.aspx");
//        }
//    }
//}

using System;
using System.Text;
using MySql.Data.MySqlClient;
using ClassLibrary1;
using System.Data;
using System.Collections.Generic;
using System.Text.Json;

namespace SistemaPina
{
    public partial class Reportes : System.Web.UI.Page
    {
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

                ddlFincaPlagas.DataSource = dt;
                ddlFincaPlagas.DataTextField = "Nombre";
                ddlFincaPlagas.DataValueField = "FincaId";
                ddlFincaPlagas.DataBind();
                ddlFincaPlagas.Items.Insert(0, "-- Todas las fincas --");

                ddlFincaLabores.DataSource = dt;
                ddlFincaLabores.DataTextField = "Nombre";
                ddlFincaLabores.DataValueField = "FincaId";
                ddlFincaLabores.DataBind();
                ddlFincaLabores.Items.Insert(0, "-- Todas las fincas --");

                ddlFincaRendimiento.DataSource = dt;
                ddlFincaRendimiento.DataTextField = "Nombre";
                ddlFincaRendimiento.DataValueField = "FincaId";
                ddlFincaRendimiento.DataBind();
                ddlFincaRendimiento.Items.Insert(0, "-- Todas las fincas --");

                CargarLotesPlagas();
                CargarLotesLabores();
            }
            catch (Exception ex)
            {
                // Manejar error
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        private void CargarLotesPlagas()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = ddlFincaPlagas.SelectedValue == "-- Todas las fincas --"
                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLotePlagas.DataSource = dt;
                ddlLotePlagas.DataTextField = "Nombre";
                ddlLotePlagas.DataValueField = "LoteId";
                ddlLotePlagas.DataBind();
                ddlLotePlagas.Items.Insert(0, "-- Todos los lotes --");
            }
            catch
            {
                // Manejar error
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        private void CargarLotesLabores()
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();
                string consulta = ddlFincaLabores.SelectedValue == "-- Todas las fincas --"
                    ? "SELECT LoteId, Nombre FROM Lotes ORDER BY Nombre"
                    : "SELECT LoteId, Nombre FROM Lotes WHERE FincaId = @fincaId ORDER BY Nombre";
                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlLoteLabores.DataSource = dt;
                ddlLoteLabores.DataTextField = "Nombre";
                ddlLoteLabores.DataValueField = "LoteId";
                ddlLoteLabores.DataBind();
                ddlLoteLabores.Items.Insert(0, "-- Todos los lotes --");
            }
            catch
            {
                // Manejar error
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        protected void ddlFincaPlagas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotesPlagas();
        }

        protected void ddlFincaLabores_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLotesLabores();
        }

        protected void btnGenerarPlagas_Click(object sender, EventArgs e)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
                sb.Append("'Plaga' AS Tipo, p.NombrePlaga AS Nombre, p.NivelAfectacion, p.FechaDeteccion ");
                sb.Append("FROM Plagas p ");
                sb.Append("LEFT JOIN PlagaBloques pb ON p.PlagaId = pb.PlagaId ");
                sb.Append("LEFT JOIN Bloques b ON pb.BloqueId = b.BloqueId ");
                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
                sb.Append("WHERE 1=1 ");
                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
                    sb.Append("AND f.FincaId = @fincaId ");
                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
                    sb.Append("AND l.LoteId = @loteId ");
                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
                    sb.Append("AND p.FechaDeteccion >= @fechaDesde ");
                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
                    sb.Append("AND p.FechaDeteccion <= @fechaHasta ");
                sb.Append("UNION ALL ");
                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
                sb.Append("'Enfermedad' AS Tipo, en.NombreEnfermedad AS Nombre, en.NivelAfectacion, en.FechaDeteccion ");
                sb.Append("FROM Enfermedades en ");
                sb.Append("LEFT JOIN EnfermedadBloques eb ON en.EnfermedadId = eb.EnfermedadId ");
                sb.Append("LEFT JOIN Bloques b ON eb.BloqueId = b.BloqueId ");
                sb.Append("LEFT JOIN Lotes l ON b.LoteId = l.LoteId ");
                sb.Append("LEFT JOIN Fincas f ON l.FincaId = f.FincaId ");
                sb.Append("WHERE 1=1 ");
                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
                    sb.Append("AND f.FincaId = @fincaId ");
                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
                    sb.Append("AND l.LoteId = @loteId ");
                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
                    sb.Append("AND en.FechaDeteccion >= @fechaDesde ");
                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
                    sb.Append("AND en.FechaDeteccion <= @fechaHasta ");
                sb.Append("ORDER BY FechaDeteccion DESC");

                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
                if (ddlFincaPlagas.SelectedValue != "-- Todas las fincas --")
                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaPlagas.SelectedValue);
                if (ddlLotePlagas.SelectedValue != "-- Todos los lotes --")
                    cmd.Parameters.AddWithValue("@loteId", ddlLotePlagas.SelectedValue);
                if (!string.IsNullOrEmpty(txtFechaDesde1.Text))
                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde1.Text);
                if (!string.IsNullOrEmpty(txtFechaHasta1.Text))
                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta1.Text);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReportePlagas.DataSource = dt;
                gvReportePlagas.DataBind();
                panelResultadoPlagas.Visible = true;

                var conteo = new Dictionary<string, int>();
                foreach (DataRow row in dt.Rows)
                {
                    string nombre = row["Nombre"].ToString();
                    if (conteo.ContainsKey(nombre))
                        conteo[nombre]++;
                    else
                        conteo[nombre] = 1;
                }

                var labels = new List<string>();
                var valores = new List<int>();
                foreach (var item in conteo)
                {
                    labels.Add(item.Key);
                    valores.Add(item.Value);
                }

                hfDatosPlagas.Value = JsonSerializer.Serialize(new { labels, valores });
            }
            catch (Exception ex)
            {
                // Manejar error
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        protected void btnGenerarLabores_Click(object sender, EventArgs e)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT f.Nombre AS NombreFinca, l.Nombre AS NombreLote, b.Nombre AS NombreBloque, ");
                sb.Append("la.TipoLabor, la.FechaLabor, la.Responsable, la.Observaciones ");
                sb.Append("FROM Labores la ");
                sb.Append("INNER JOIN Bloques b ON la.BloqueId = b.BloqueId ");
                sb.Append("INNER JOIN Lotes l ON b.LoteId = l.LoteId ");
                sb.Append("INNER JOIN Fincas f ON l.FincaId = f.FincaId ");
                sb.Append("WHERE 1=1 ");
                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
                    sb.Append("AND f.FincaId = @fincaId ");
                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
                    sb.Append("AND l.LoteId = @loteId ");
                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
                    sb.Append("AND la.FechaLabor >= @fechaDesde ");
                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
                    sb.Append("AND la.FechaLabor <= @fechaHasta ");
                sb.Append("ORDER BY la.FechaLabor DESC");

                MySqlCommand cmd = new MySqlCommand(sb.ToString(), conexion.CONECTAR);
                if (ddlFincaLabores.SelectedValue != "-- Todas las fincas --")
                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaLabores.SelectedValue);
                if (ddlLoteLabores.SelectedValue != "-- Todos los lotes --")
                    cmd.Parameters.AddWithValue("@loteId", ddlLoteLabores.SelectedValue);
                if (!string.IsNullOrEmpty(txtFechaDesde2.Text))
                    cmd.Parameters.AddWithValue("@fechaDesde", txtFechaDesde2.Text);
                if (!string.IsNullOrEmpty(txtFechaHasta2.Text))
                    cmd.Parameters.AddWithValue("@fechaHasta", txtFechaHasta2.Text);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReporteLabores.DataSource = dt;
                gvReporteLabores.DataBind();
                panelResultadoLabores.Visible = true;

                var conteo = new Dictionary<string, int>();
                foreach (DataRow row in dt.Rows)
                {
                    string tipo = row["TipoLabor"].ToString();
                    if (conteo.ContainsKey(tipo))
                        conteo[tipo]++;
                    else
                        conteo[tipo] = 1;
                }

                var labels = new List<string>();
                var valores = new List<int>();
                foreach (var item in conteo)
                {
                    labels.Add(item.Key);
                    valores.Add(item.Value);
                }

                hfDatosLabores.Value = JsonSerializer.Serialize(new { labels, valores });
            }
            catch (Exception ex)
            {
                // Manejar error
            }
            finally
            {
                conexion.CERRAR_CONEXION();
            }
        }

        protected void btnGenerarRendimiento_Click(object sender, EventArgs e)
        {
            CLASS_CONEXION conexion = new CLASS_CONEXION();
            try
            {
                conexion.ABRIR_CONEXION();

                string consulta = @"
                    SELECT 
                        f.Nombre AS NombreFinca, 
                        g.Nombre AS NombreGrupo, 
                        c.FechaCosecha, 
                        c.KilosCosechados,
                        COALESCE(
                            (SELECT SUM(s.CantidadPlantas) 
                             FROM Siembras s 
                             INNER JOIN GrupoForzaBloques gb2 ON s.BloqueId = gb2.BloqueId 
                             WHERE gb2.GrupoForzaId = g.GrupoForzaId), 0) AS TotalPlantas,
                        ROUND(
                            c.KilosCosechados / NULLIF(
                                (SELECT SUM(s.CantidadPlantas) 
                                 FROM Siembras s 
                                 INNER JOIN GrupoForzaBloques gb2 ON s.BloqueId = gb2.BloqueId 
                                 WHERE gb2.GrupoForzaId = g.GrupoForzaId), 0), 2) AS KgPorPlanta
                    FROM Cosechas c
                    INNER JOIN GruposForza g ON c.GrupoForzaId = g.GrupoForzaId
                    INNER JOIN Fincas f ON g.FincaId = f.FincaId
                    WHERE f.EmpresaId = @empresaId 
                    AND c.GrupoForzaId IS NOT NULL";

                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
                    consulta += " AND f.FincaId = @fincaId";

                consulta += " ORDER BY g.Nombre, c.FechaCosecha";

                MySqlCommand cmd = new MySqlCommand(consulta, conexion.CONECTAR);
                cmd.Parameters.AddWithValue("@empresaId", Session["EmpresaId"].ToString());
                if (ddlFincaRendimiento.SelectedValue != "-- Todas las fincas --")
                    cmd.Parameters.AddWithValue("@fincaId", ddlFincaRendimiento.SelectedValue);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReporteRendimiento.DataSource = dt;
                gvReporteRendimiento.DataBind();
                panelResultadoRendimiento.Visible = true;
            }
            catch (Exception ex)
            {
                // Manejar error
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