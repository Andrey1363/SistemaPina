<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Fincas.aspx.cs" Inherits="SistemaPina.Fincas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Fincas - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
</head>
<body>

    <form id="form1" runat="server">

        <!-- Barra superior -->
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
                <h2> Gestión de Fincas</h2>
                <p>Registrá, consultá y administrá las fincas del sistema.</p>

                <!-- Formulario para agregar finca -->
                <div class="formulario-box">
                    <h3>Agregar nueva finca</h3>

                    <div class="campo-form">
                        <label>Nombre de la finca</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Ej: Finca La Esperanza"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Ubicación</label>
                        <asp:TextBox ID="txtUbicacion" runat="server" placeholder="Ej: San Carlos, Alajuela"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Área total (hectáreas)</label>
                        <asp:TextBox ID="txtArea" runat="server" placeholder="Ej: 25.5"></asp:TextBox>
                    </div>

                    <!-- Mensaje de error o éxito -->
                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar finca" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de fincas registradas -->
                <div class="tabla-box">
                    <h3>Fincas registradas</h3>
                    <asp:GridView ID="gvFincas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvFincas_RowCommand" DataKeyNames="FincaId">
                        <Columns>
                            
                            <asp:BoundField DataField="NombreEmpresa" HeaderText="Empresa" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Ubicacion" HeaderText="Ubicación" />
                            <asp:BoundField DataField="AreaTotal" HeaderText="Área (ha)" />
                            
                            <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                              <asp:Button ID="btnEditar" runat="server" Text="Editar" 
                                CommandName="Editar" CommandArgument='<%# Eval("FincaId") %>' 
                                CssClass="btn-editar" />
                              <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" 
                                CommandName="Eliminar" CommandArgument='<%# Eval("FincaId") %>' 
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
