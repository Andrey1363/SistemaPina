<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bloques.aspx.cs" Inherits="SistemaPina.Bloques" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Bloques - Sistema de Gestión de Piña</title>
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
                    <li><a href="Cosechas.aspx"> Cosechas</a></li>
                    <li><a href="GruposForza.aspx"> Grupos de Forza</a></li>
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
                <h2> Gestión de Bloques</h2>
                <p>Registrá y administrá los bloques dentro de cada lote.</p>

                <!-- Formulario para agregar bloque -->
                <div class="formulario-box">
                    <h3>Agregar nuevo bloque</h3>

                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFinca_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Lote al que pertenece</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Nombre del bloque</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Ej: Bloque 1"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Área (hectáreas)</label>
                        <asp:TextBox ID="txtArea" runat="server" placeholder="Ej: 5.5"></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar bloque" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de bloques -->
                <div class="tabla-box">
                    <h3>Bloques registrados</h3>
                    <asp:GridView ID="gvBloques" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvBloques_RowCommand" DataKeyNames="BloqueId">
                        <Columns>
                            <asp:BoundField DataField="BloqueId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                            <asp:BoundField DataField="Nombre" HeaderText="Bloque" />
                            <asp:BoundField DataField="AreaHectareas" HeaderText="Área (ha)" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>