<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Lotes.aspx.cs" Inherits="SistemaPina.Lotes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Lotes - Sistema de Gestión de Piña</title>
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
                <h2> Gestión de Lotes</h2>
                <p>Registrá y administrá los lotes asociados a cada finca.</p>

                <!-- Formulario para agregar lote -->
                <div class="formulario-box">
                    <h3>Agregar nuevo lote</h3>

                    <div class="campo-form">
                        <label>Finca a la que pertenece</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Nombre del lote</label>
                        <asp:TextBox ID="txtNombre" runat="server" placeholder="Ej: Lote A"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Código identificador (opcional)</label>
                        <asp:TextBox ID="txtCodigo" runat="server" placeholder="Ej: L-001"></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar lote" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de lotes -->
                <div class="tabla-box">
                    <h3>Lotes registrados</h3>
                    <!-- Tabla de lotes -->
                    <div class="tabla-box">
                        <h3>Lotes registrados</h3>
                        <asp:GridView ID="gvLotes" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                            OnRowCommand="gvLotes_RowCommand" DataKeyNames="LoteId">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="Codigo" HeaderText="Código" />
            
                                
                                <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                            CommandName="Editar"
                                            CommandArgument='<%# Eval("LoteId") %>'
                                            CssClass="btn-editar" />
                                        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                            CommandName="Eliminar"
                                            CommandArgument='<%# Eval("LoteId") %>'
                                            CssClass="btn-eliminar" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

            </div>
        </div>

    </form>
</body>
</html>