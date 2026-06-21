<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GruposForza.aspx.cs" Inherits="SistemaPina.GruposForza" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Grupos de Forza - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.less" />
</head>
<body>

    <form id="form1" runat="server">

        <!-- Barra superior -->
        <div class="topbar">
            <div class="topbar-titulo">🍍 Sistema de Gestión de Piña</div>
            <div class="topbar-usuario">
                <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn-cerrar" OnClick="btnCerrarSesion_Click" />
            </div>
        </div>

        <div class="contenedor-principal">

            <!-- Menú lateral -->
            <div class="menu-lateral">
                <ul>
                    <li><a href="Fincas.aspx"> Fincas</a></li>
                    <li><a href="Lotes.aspx"> Lotes</a></li>
                    <li><a href="Bloques.aspx"> Bloques</a></li>
                    <li><a href="Siembras.aspx"> Siembras</a></li>
                    <li><a href="GruposForza.aspx"> Grupos de Forza</a></li>
                    <li><a href="Cosechas.aspx"> Cosechas</a></li>
                    <li><a href="Plagas.aspx"> Plagas</a></li>
                    <li><a href="Enfermedades.aspx"> Enfermedades</a></li>
                    <li><a href="Fertilizaciones.aspx"> Fertilizaciones</a></li>
                    <li><a href="Labores.aspx"> Labores</a></li>
                    <li><a href="Reportes.aspx"> Reportes</a></li>
                    <asp:Panel ID="panelUsuarios" runat="server">
                        <li><a href="Usuarios.aspx"> Usuarios</a></li>
                    </asp:Panel>
                </ul>
            </div>

            <!-- Contenido principal -->
            <div class="contenido">
                <h2> Grupos de Forza</h2>
                <p>Creá grupos de forza y asignales los bloques correspondientes.</p>

                <!-- Formulario para crear grupo -->
                <div class="formulario-box">
                    <h3>Crear nuevo grupo de forza</h3>

                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFinca_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Nombre del grupo</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Ej: GF1-2026"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Bloques que pertenecen a este grupo</label>
                        <!-- CheckBoxList permite seleccionar varios bloques a la vez -->
                        <asp:CheckBoxList ID="cblBloques" runat="server" CssClass="check-list"></asp:CheckBoxList>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar grupo" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de grupos registrados -->
                <div class="tabla-box">
                    <h3>Grupos de Forza registrados</h3>
                    <asp:GridView ID="gvGrupos" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvGrupos_RowCommand" DataKeyNames="GrupoForzaId">
                        <Columns>
                            <asp:BoundField DataField="GrupoForzaId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="Nombre" HeaderText="Grupo" />
                            <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>
