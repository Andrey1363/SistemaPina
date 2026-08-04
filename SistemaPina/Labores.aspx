<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Labores.aspx.cs" Inherits="SistemaPina.Labores" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Labores - Sistema de Gestión de Piña</title>
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
                <h2>Bitácora de Labores</h2>
                <p>Registrá las actividades diarias realizadas en cada bloque.</p>

                <!-- Campo oculto para guardar el ID cuando se edita -->
                <asp:HiddenField ID="hfLaborId" runat="server" Value="0" />

                <!-- Formulario registro/edición de labor -->
                <div class="formulario-box">
                    <!-- El título cambia según si es nuevo o edición -->
                    <h3><asp:Label ID="lblTituloFormulario" runat="server" Text="Registrar labor diaria"></asp:Label></h3>

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
                        <label>Tipo de labor</label>
                        <asp:DropDownList ID="ddlTipoLabor" runat="server" CssClass="ddl">
                            <asp:ListItem Value="Deshierba">Deshierba</asp:ListItem>
                            <asp:ListItem Value="Chapea">Chapea</asp:ListItem>
                            <asp:ListItem Value="Abono">Abono</asp:ListItem>
                            <asp:ListItem Value="Otro">Otro</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de labor</label>
                        <asp:TextBox ID="txtFechaLabor" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Responsable (opcional)</label>
                        <asp:TextBox ID="txtResponsable" runat="server" placeholder="Nombre del encargado de la labor"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3"
                            placeholder="Ej: 4 horas de trabajo, observaciones adicionales..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar labor" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnCancelarEdicion" runat="server" Text="Cancelar" CssClass="btn-eliminar" OnClick="btnCancelarEdicion_Click" Visible="false" />
                </div>

                <!-- Tabla de labores -->
                <div class="tabla-box">
                    <h3>Labores registradas</h3>
                    <asp:GridView ID="gvLabores" runat="server" CssClass="tabla" AutoGenerateColumns="false"
    OnRowCommand="gvLabores_RowCommand" DataKeyNames="LaborId">
    <Columns>
        <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
        <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
        <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
        <asp:BoundField DataField="TipoLabor" HeaderText="Labor" />
        <asp:BoundField DataField="FechaLabor" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
        <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" />
        
        
        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnEditar" runat="server" Text="Editar"
                    CommandName="Editar"
                    CommandArgument='<%# Eval("LaborId") %>'
                    CssClass="btn-editar" />
                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                    CommandName="Eliminar"
                    CommandArgument='<%# Eval("LaborId") %>'
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