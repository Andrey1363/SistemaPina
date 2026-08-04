<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cosechas.aspx.cs" Inherits="SistemaPina.Cosechas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Cosechas - Sistema de Gestión de Piña</title>
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
                <h2> Registro de Cosechas</h2>
                <p>Registrá las cosechas por grupo de forza.</p>

                <!-- Formulario para registrar cosecha -->
                <div class="formulario-box">
                    <h3>Registrar nueva cosecha</h3>

                    <div class="campo-form">
                        <label>Grupo de Forza a cosechar</label>
                        <asp:DropDownList ID="ddlGrupoForza" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de cosecha</label>
                        <asp:TextBox ID="txtFechaCosecha" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Kilos cosechados</label>
                        <asp:TextBox ID="txtKilos" runat="server" placeholder="Ej: 2500.50"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar cosecha" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de cosechas -->
                <div class="tabla-box">
                    <h3>Cosechas registradas</h3>
                       <asp:GridView ID="gvCosechas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                            OnRowCommand="gvCosechas_RowCommand" DataKeyNames="CosechaId">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
                                <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
                                <asp:BoundField DataField="FechaCosecha" HeaderText="Fecha cosecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="KilosCosechados" HeaderText="Kilos" />
        
                                <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                            CommandName="Editar"
                                            CommandArgument='<%# Eval("CosechaId") %>'
                                            CssClass="btn-editar" />
                                        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                            CommandName="Eliminar"
                                            CommandArgument='<%# Eval("CosechaId") %>'
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