<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Siembras.aspx.cs" Inherits="SistemaPina.Siembras" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Siembras - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
</head>
<body>

    <form id="form1" runat="server">
        <%-- Barra superior --%>
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
                <h2> Gestión de Siembras</h2>
                <p>Registrá y consultá las siembras por bloque.</p>

                <!-- Formulario para agregar siembra -->
                <div class="formulario-box">
                    <h3>Registrar nueva siembra</h3>

                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFinca_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Lote</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlLote_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Bloque</label>
                        <asp:DropDownList ID="ddlBloque" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de siembra</label>
                        <asp:TextBox ID="txtFechaSiembra" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Cantidad de plantas</label>
                        <asp:TextBox ID="txtCantidadPlantas" runat="server" placeholder="Ej: 1500"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Tipo de etapa</label>
                        <asp:DropDownList ID="ddlTipoEtapa" runat="server" CssClass="ddl">
                            <asp:ListItem Value="Plantacion">Plantación</asp:ListItem>
                            <asp:ListItem Value="Fruta">Fruta</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar siembra" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Tabla de siembras -->
                <div class="tabla-box">
                    <h3>Siembras registradas</h3>
                    <asp:GridView ID="gvSiembras" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvSiembras_RowCommand" DataKeyNames="SiembraId">
                        <Columns>
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                            <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                            <asp:BoundField DataField="FechaSiembra" HeaderText="Fecha siembra" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="EdadDias" HeaderText="Edad (días)" />
                            <asp:BoundField DataField="CantidadPlantas" HeaderText="Plantas" />
                            <asp:BoundField DataField="TipoEtapa" HeaderText="Etapa" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
        
                            <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("SiembraId") %>'
                                        CssClass="btn-editar" />
                                    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("SiembraId") %>'
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
