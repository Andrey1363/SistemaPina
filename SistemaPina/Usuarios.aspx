<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="SistemaPina.Usuarios" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Usuarios - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
</head>
<body>

    <form id="form1" runat="server">

                      <div class="topbar">
    <div class="topbar-titulo">🍍 Sistema de Gestión de Piña</div>
    <div class="topbar-usuario">
        <div class="usuario-info">
            <div class="usuario-nombre">
                👤 <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
            </div>
            <div class="usuario-empresa">
                🍍 <asp:Label ID="lblEmpresa" runat="server"></asp:Label>
            </div>
        </div>
        <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn-cerrar" OnClick="btnCerrarSesion_Click" />
    </div>
</div>

        <div class="contenedor-principal">

            <div class="menu-lateral">
                <ul>
                    <li><a href="Fincas.aspx">Fincas</a></li>
                    <li><a href="Lotes.aspx">Lotes</a></li>
                    <li><a href="Bloques.aspx">Bloques</a></li>
                    <li><a href="Siembras.aspx">Siembras</a></li>
                    <li><a href="GruposForza.aspx">Grupos de Forza</a></li>
                    <li><a href="Cosechas.aspx">Cosechas</a></li>
                    <li><a href="Plagas.aspx">Plagas</a></li>
                    <li><a href="Enfermedades.aspx">Enfermedades</a></li>
                    <li><a href="Fertilizaciones.aspx">Fertilizaciones</a></li>
                    <li><a href="Labores.aspx">Labores</a></li>
                    <li><a href="Reportes.aspx">Reportes</a></li>
                    <asp:Panel ID="panelUsuarios" runat="server">
                        <li><a href="Usuarios.aspx">Usuarios</a></li>
                    </asp:Panel>
                </ul>
            </div>

            <div class="contenido">
                <h2>Gestión de Usuarios</h2>
                <asp:Label ID="lblDescripcion" runat="server"></asp:Label>

                <!-- Panel Solo para SuperAdmin: Gestión de Empresas -->
                <asp:Panel ID="panelEmpresas" runat="server" Visible="false">
                    <div class="seccion-titulo">
                        <h3>Empresas registradas</h3>
                        <p>Administrá las empresas del sistema</p>
                    </div>

                    <div class="formulario-box">
                        <h3>Agregar nueva empresa</h3>
                        <div class="campo-form">
                            <label>Nombre de la empresa</label>
                            <asp:TextBox ID="txtNombreEmpresa" runat="server" placeholder="Ej: Inversiones A y A LTDA"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Descripción (opcional)</label>
                            <asp:TextBox ID="txtDescripcionEmpresa" runat="server" placeholder="Descripción de la empresa"></asp:TextBox>
                        </div>
                        <asp:Label ID="lblMensajeEmpresa" runat="server" Text="" Visible="false"></asp:Label>
                        <asp:Button ID="btnGuardarEmpresa" runat="server" Text="Guardar empresa" CssClass="btn-guardar" OnClick="btnGuardarEmpresa_Click" />
                    </div>

                    <div class="tabla-box">
                        <h3>Empresas</h3>
                        <asp:GridView ID="gvEmpresas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                            OnRowCommand="gvEmpresas_RowCommand" DataKeyNames="EmpresaId">
                            <Columns>
                                <asp:BoundField DataField="EmpresaId" HeaderText="ID" />
                                <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="TotalUsuarios" HeaderText="Usuarios" />
                                <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>

                <!-- Formulario para crear usuario -->
                <div class="seccion-titulo" style="margin-top:20px">
                    <h3>Usuarios del sistema</h3>
                    <asp:Label ID="lblSubtituloUsuarios" runat="server"></asp:Label>
                </div>

                <div class="formulario-box">
                    <h3>Agregar nuevo usuario</h3>

                    <!-- Solo SuperAdmin ve el selector de empresa -->
                    <asp:Panel ID="panelSelectorEmpresa" runat="server" Visible="false">
                        <div class="campo-form">
                            <label>Empresa</label>
                            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="ddl"></asp:DropDownList>
                        </div>
                    </asp:Panel>

                    <div class="campo-form">
                        <label>Nombre completo</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Nombre del usuario"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Usuario (para el login)</label>
                        <asp:TextBox ID="txtUsuario" runat="server" placeholder="Ej: andrey123"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Contraseña</label>
                        <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Rol</label>
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar usuario" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de usuarios -->
                <div class="tabla-box">
                    <h3>Usuarios registrados</h3>
                    <asp:GridView ID="gvUsuarios" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="UsuarioId">
                        <Columns>
                            <asp:BoundField DataField="UsuarioId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Rol" HeaderText="Rol" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            <asp:ButtonField ButtonType="Button" CommandName="Toggleactivo" Text="Activar/Desactivar" ControlStyle-CssClass="btn-secundario" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>--%>

<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="SistemaPina.Usuarios" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Usuarios - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
</head>
<body>

    <form id="form1" runat="server">

        <!-- ========================================== -->
        <!-- BARRA SUPERIOR -->
        <!-- ========================================== -->
        <div class="topbar">
            <div class="topbar-titulo">Administración de Fincas y Usuarios</div>
            <div class="topbar-usuario">
                <div class="usuario-info">
                    <div class="usuario-nombre">
                        👤 <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
                    </div>
        
                </div>
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn-cerrar" OnClick="btnCerrarSesion_Click" />
            </div>
        </div>

        <div class="contenedor-principal">

            <!-- ========================================== -->
            <!-- MENU LATERAL (SOLO USUARIOS) -->
            <!-- ========================================== -->
            <div class="menu-lateral">
                <ul>
                    <li><a href="Usuarios.aspx">👥 Usuarios</a></li>
                </ul>
            </div>

            <!-- ========================================== -->
            <!-- CONTENIDO PRINCIPAL -->
            <!-- ========================================== -->
            <div class="contenido">
                <h2>Gestión de Usuarios</h2>
                <asp:Label ID="lblDescripcion" runat="server"></asp:Label>

                <!-- ========================================== -->
                <!-- PANEL EMPRESAS (SOLO SUPERADMIN) -->
                <!-- ========================================== -->
                <asp:Panel ID="panelEmpresas" runat="server" Visible="false">
                    <div class="seccion-titulo">
                        <h3>Empresas registradas</h3>
                        <p>Administrá las empresas del sistema</p>
                    </div>

                    <div class="formulario-box">
                        <h3>Agregar nueva empresa</h3>
                        <div class="campo-form">
                            <label>Nombre de la empresa</label>
                            <asp:TextBox ID="txtNombreEmpresa" runat="server" placeholder="Ej: Inversiones A y A LTDA"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Descripción (opcional)</label>
                            <asp:TextBox ID="txtDescripcionEmpresa" runat="server" placeholder="Descripción de la empresa"></asp:TextBox>
                        </div>
                        <asp:Label ID="lblMensajeEmpresa" runat="server" Text="" Visible="false"></asp:Label>
                        <asp:Button ID="btnGuardarEmpresa" runat="server" Text="Guardar empresa" CssClass="btn-guardar" OnClick="btnGuardarEmpresa_Click" />
                    </div>

                    <div class="tabla-box">
                        <h3>Empresas</h3>
                        <asp:GridView ID="gvEmpresas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                            OnRowCommand="gvEmpresas_RowCommand" DataKeyNames="EmpresaId">
                            <Columns>
                                <asp:BoundField DataField="EmpresaId" HeaderText="ID" />
                                <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="TotalUsuarios" HeaderText="Usuarios" />
                                <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>

                <!-- ========================================== -->
                <!-- FORMULARIO PARA CREAR/EDITAR USUARIO -->
                <!-- ========================================== -->
                <div class="seccion-titulo" style="margin-top:20px">
                    <h3>Usuarios del sistema</h3>
                    <asp:Label ID="lblSubtituloUsuarios" runat="server"></asp:Label>
                </div>

                <div class="formulario-box">
                    <h3 id="tituloFormulario">Agregar nuevo usuario</h3>

                    <!-- Solo SuperAdmin ve el selector de empresa -->
                    <asp:Panel ID="panelSelectorEmpresa" runat="server" Visible="false">
                        <div class="campo-form">
                            <label>Empresa</label>
                            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="ddl"></asp:DropDownList>
                        </div>
                    </asp:Panel>

                    <div class="campo-form">
                        <label>Nombre completo</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Nombre del usuario"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Usuario (para el login)</label>
                        <asp:TextBox ID="txtUsuario" runat="server" placeholder="Ej: andrey123"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Contraseña</label>
                        <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                        <!--  LABEL DE AYUDA PARA CONTRASEÑA -->
                        <asp:Label ID="lblContrasenaAyuda" runat="server" Text="(Dejar en blanco para no cambiar)" CssClass="ayuda" Visible="false"></asp:Label>
                    </div>

                    <div class="campo-form">
                        <label>Rol</label>
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <!--  BOTONES GUARDAR Y CANCELAR -->
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar usuario" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar" CssClass="btn-eliminar" OnClick="btnCancelarEdicion_Click" Visible="false" />
                </div>

                <!-- ========================================== -->
                <!-- TABLA DE USUARIOS -->
                <!-- ========================================== -->
                <div class="tabla-box">
                    <h3>Usuarios registrados</h3>
                    <asp:GridView ID="gvUsuarios" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="UsuarioId">
                        <Columns>
                            <asp:BoundField DataField="UsuarioId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Rol" HeaderText="Rol" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            <asp:ButtonField ButtonType="Button" CommandName="Toggleactivo" Text="Activar/Desactivar" ControlStyle-CssClass="btn-secundario" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="SistemaPina.Usuarios" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Usuarios - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
</head>
<body>

    <form id="form1" runat="server">

        <!-- ========================================== -->
        <!-- BARRA SUPERIOR -->
        <!-- ========================================== -->
        <div class="topbar">
            <div class="topbar-titulo">Administración de Fincas y Usuarios</div>
            <div class="topbar-usuario">
                <div class="usuario-info">
                    <div class="usuario-nombre">
                        👤 <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
                    </div>
                </div>
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn-cerrar" OnClick="btnCerrarSesion_Click" />
            </div>
        </div>

        <div class="contenedor-principal">

            <!-- ========================================== -->
            <!-- MENU LATERAL (SOLO USUARIOS) -->
            <!-- ========================================== -->
            <div class="menu-lateral">
                <ul>
                    <li><a href="Usuarios.aspx">👥 Usuarios</a></li>
                </ul>
            </div>

            <!-- ========================================== -->
            <!-- CONTENIDO PRINCIPAL -->
            <!-- ========================================== -->
            <div class="contenido">
                <h2>Gestión de Usuarios</h2>
                <asp:Label ID="lblDescripcion" runat="server"></asp:Label>

                <!-- ========================================== -->
                <!-- PANEL EMPRESAS (SOLO SUPERADMIN) -->
                <!-- ========================================== -->
                <asp:Panel ID="panelEmpresas" runat="server" Visible="false">
                    <div class="seccion-titulo">
                        <h3>Empresas registradas</h3>
                        <p>Administrá las empresas del sistema</p>
                    </div>

                    <div class="formulario-box">
                        <h3 id="tituloEmpresa">Agregar nueva empresa</h3>
                        <div class="campo-form">
                            <label>Nombre de la empresa</label>
                            <asp:TextBox ID="txtNombreEmpresa" runat="server" placeholder="Ej: Inversiones A y A LTDA"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Descripción (opcional)</label>
                            <asp:TextBox ID="txtDescripcionEmpresa" runat="server" placeholder="Descripción de la empresa"></asp:TextBox>
                        </div>
                        <asp:Label ID="lblMensajeEmpresa" runat="server" Text="" Visible="false"></asp:Label>
                        <asp:Button ID="btnGuardarEmpresa" runat="server" Text="Guardar empresa" CssClass="btn-guardar" OnClick="btnGuardarEmpresa_Click" />
                        <asp:Button ID="btnCancelarEdicionEmpresa" runat="server" Text="Cancelar" CssClass="btn-eliminar" OnClick="btnCancelarEdicionEmpresa_Click" Visible="false" />
                    </div>

                    <div class="tabla-box">
                        <h3>Empresas</h3>
                        <asp:GridView ID="gvEmpresas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                            OnRowCommand="gvEmpresas_RowCommand" DataKeyNames="EmpresaId">
                            <Columns>
                                <asp:BoundField DataField="EmpresaId" HeaderText="ID" />
                                <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="TotalUsuarios" HeaderText="Usuarios" />
                                
                                <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditarEmpresa" runat="server" Text="Editar"
                                            CommandName="EditarEmpresa"
                                            CommandArgument='<%# Eval("EmpresaId") %>'
                                            CssClass="btn-editar" />
                                        <asp:Button ID="btnEliminarEmpresa" runat="server" Text="Eliminar"
                                            CommandName="EliminarEmpresa"
                                            CommandArgument='<%# Eval("EmpresaId") %>'
                                            CssClass="btn-eliminar" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>

                <!-- ========================================== -->
                <!-- FORMULARIO PARA CREAR/EDITAR USUARIO -->
                <!-- ========================================== -->
                <div class="seccion-titulo" style="margin-top:20px">
                    <h3>Usuarios del sistema</h3>
                    <asp:Label ID="lblSubtituloUsuarios" runat="server"></asp:Label>
                </div>

                <div class="formulario-box">
                    <h3 id="tituloFormulario">Agregar nuevo usuario</h3>

                    <!-- Solo SuperAdmin ve el selector de empresa -->
                    <asp:Panel ID="panelSelectorEmpresa" runat="server" Visible="false">
                        <div class="campo-form">
                            <label>Empresa</label>
                            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="ddl"></asp:DropDownList>
                        </div>
                    </asp:Panel>

                    <div class="campo-form">
                        <label>Nombre completo</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Nombre del usuario"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Usuario (para el login)</label>
                        <asp:TextBox ID="txtUsuario" runat="server" placeholder="Ej: andrey123"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Contraseña</label>
                        <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                        <asp:Label ID="lblContrasenaAyuda" runat="server" Text="(Dejar en blanco para no cambiar)" CssClass="ayuda" Visible="false"></asp:Label>
                    </div>

                    <div class="campo-form">
                        <label>Rol</label>
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar usuario" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar" CssClass="btn-eliminar" OnClick="btnCancelarEdicion_Click" Visible="false" />
                </div>

                <!-- ========================================== -->
                <!-- TABLA DE USUARIOS CON EDITAR Y ELIMINAR -->
                <!-- ========================================== -->
                <div class="tabla-box">
                    <h3>Usuarios registrados</h3>
                    <asp:GridView ID="gvUsuarios" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="UsuarioId">
                        <Columns>
                            <asp:BoundField DataField="UsuarioId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Rol" HeaderText="Rol" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("UsuarioId") %>'
                                        CssClass="btn-editar" />
                                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("UsuarioId") %>'
                                        CssClass="btn-eliminar" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>