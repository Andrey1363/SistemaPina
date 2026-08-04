<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Plagas.aspx.cs" Inherits="SistemaPina.Plagas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Plagas - Sistema de Gestión de Piña</title>
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
                <h2>Control de Plagas</h2>
                <p>Registrá incidencias de plagas seleccionando uno o varios bloques afectados.</p>

                <!-- Alerta de brotes -->
                <asp:Panel ID="panelAlerta" runat="server" Visible="false">
                    <div class="alerta-brote">
                        ⚠️ <strong>Alerta:</strong>
                        <asp:Label ID="lblAlerta" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <!-- Formulario registro de plaga -->
                <div class="formulario-box">
                    <h3>Registrar incidencia de plaga</h3>

                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFinca_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Lote</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlLote_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Bloques afectados (podés seleccionar varios)</label>
                        <asp:CheckBoxList ID="cblBloques" runat="server" CssClass="check-list"></asp:CheckBoxList>
                    </div>

                    <div class="campo-form">
                        <label>Nombre de la plaga</label>
                        <asp:TextBox ID="txtNombrePlaga" runat="server" placeholder="Ej: Picudo, Cochinilla"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Nivel de afectación</label>
                        <asp:DropDownList ID="ddlNivel" runat="server" CssClass="ddl">
                            <asp:ListItem Value="Leve">Leve</asp:ListItem>
                            <asp:ListItem Value="Moderado">Moderado</asp:ListItem>
                            <asp:ListItem Value="Severo">Severo</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de detección</label>
                        <asp:TextBox ID="txtFechaDeteccion" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar registro" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Panel de recomendación -->
                <asp:Panel ID="panelRecomendacion" runat="server" Visible="false">
                    <div class="formulario-box recomendacion-box">
                        <h3>Generar recomendación de aplicación</h3>
                        <p class="recomendacion-info">
                            Plaga: <strong><asp:Label ID="lblPlagaRecomendacion" runat="server"></asp:Label></strong> |
                            Bloques: <strong><asp:Label ID="lblBloquesRecomendacion" runat="server"></asp:Label></strong>
                        </p>

                        <asp:HiddenField ID="hfPlagaId" runat="server" />

                        <div class="campo-form">
                            <label>Producto recomendado</label>
                            <asp:TextBox ID="txtProductoRec" runat="server" placeholder="Ej: Solver 50EC"></asp:TextBox>
                        </div>

                        <div class="campo-form">
                            <label>Dosis/Ha Kg-Lts</label>
                            <asp:TextBox ID="txtDosisRec" runat="server" placeholder="Ej: 3.5 LTS/HA"></asp:TextBox>
                        </div>

                        <div class="campo-form">
                            <label>Fecha de aplicación</label>
                            <asp:TextBox ID="txtFechaRec" runat="server" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="campo-form">
                            <label>Observaciones (opcional)</label>
                            <asp:TextBox ID="txtObservacionesRec" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                        </div>

                        <asp:Label ID="lblMensajeRec" runat="server" Text="" Visible="false"></asp:Label>

                        <asp:Button ID="btnGuardarRec" runat="server" Text="Guardar recomendación" CssClass="btn-guardar" OnClick="btnGuardarRec_Click" />
                        <asp:Button ID="btnCancelarRec" runat="server" Text="Cancelar" CssClass="btn-eliminar" OnClick="btnCancelarRec_Click" />
                    </div>
                </asp:Panel>

                <!-- Tabla registros de plagas -->
                <div class="tabla-box">
                    <h3>Registros de plagas</h3>
                        <asp:GridView ID="gvPlagas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                    OnRowCommand="gvPlagas_RowCommand" DataKeyNames="PlagaId">
                    <Columns>
                        <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                        <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                        <asp:BoundField DataField="NombrePlaga" HeaderText="Plaga" />
                        <asp:BoundField DataField="NivelAfectacion" HeaderText="Nivel" />
                        <asp:BoundField DataField="FechaDeteccion" HeaderText="Detección" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Bloques" HeaderText="Bloques afectados" />
        
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                    CommandName="Editar"
                                    CommandArgument='<%# Eval("PlagaId") %>'
                                    CssClass="btn-editar" />
                                <asp:Button ID="btnRecomendar" runat="server" Text="Recomendar"
                                    CommandName="Recomendar"
                                    CommandArgument='<%# Eval("PlagaId") %>'
                                    CssClass="btn-recomendar" />
                                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                    CommandName="Eliminar"
                                    CommandArgument='<%# Eval("PlagaId") %>'
                                    CssClass="btn-eliminar" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </div>

                <!-- Tabla recomendaciones -->
                <div class="tabla-box" style="margin-top:20px">
                    <h3>Recomendaciones generadas</h3>
                     <asp:GridView ID="gvRecomendaciones" runat="server" CssClass="tabla" AutoGenerateColumns="false"
    DataKeyNames="RecomendacionId" OnRowCommand="gvRecomendaciones_RowCommand">
    <Columns>
        <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
        <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
        <asp:BoundField DataField="NombrePlaga" HeaderText="Plaga" />
        <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
        <asp:BoundField DataField="Producto" HeaderText="Producto" />
        <asp:BoundField DataField="Dosis" HeaderText="Dosis" />
        <asp:BoundField DataField="FechaAplicacion" HeaderText="Fecha aplicación" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" />
        
        <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnEditarRec" runat="server" Text="Editar"
                    CommandName="EditarRec"
                    CommandArgument='<%# Eval("RecomendacionId") %>'
                    CssClass="btn-editar" />
                <asp:Button ID="btnEliminarRec" runat="server" Text="Eliminar"
                    CommandName="Eliminar"
                    CommandArgument='<%# Eval("RecomendacionId") %>'
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