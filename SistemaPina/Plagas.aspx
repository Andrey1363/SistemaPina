<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Plagas.aspx.cs" Inherits="SistemaPina.Plagas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Plagas - Sistema de Gestión de Piña</title>
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
                <h2> Control de Plagas</h2>
                <p>Registrá y controlá las incidencias de plagas por bloque.</p>

                <!-- Alerta de brotes -->
                <asp:Panel ID="panelAlerta" runat="server" Visible="false">
                    <div class="alerta-brote">
                        ⚠️ <strong>Alerta:</strong>
                        <asp:Label ID="lblAlerta" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <!-- Formulario para registrar plaga -->
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
                        <label>Bloque</label>
                        <asp:DropDownList ID="ddlBloque" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Nombre de la plaga</label>
                        <asp:TextBox ID="txtNombrePlaga" runat="server" placeholder="Ej: Cochinilla, Gusano Soldado"></asp:TextBox>
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
                        <label>Producto aplicado (opcional)</label>
                        <asp:TextBox ID="txtProducto" runat="server" placeholder="Ej: Shenzi, Actara"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Dosis/Ha Kg-Lts (opcional)</label>
                        <asp:TextBox ID="txtDosis" runat="server" placeholder="Ej: 2.5"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de control (opcional)</label>
                        <asp:TextBox ID="txtFechaControl" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar registro" CssClass="btn-guardar" OnClick="btnGuardar_Click" />
                </div>

                <!-- Contador por bloque -->
                <div class="tabla-box">
                    <h3>📊 Resumen de plagas por bloque</h3>
                    <asp:GridView ID="gvResumen" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                            <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                            <asp:BoundField DataField="NombrePlaga" HeaderText="Plaga" />
                            <asp:BoundField DataField="TotalReportes" HeaderText="Total reportes" />
                            <asp:BoundField DataField="UltimaDeteccion" HeaderText="Última detección" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- Tabla de registros -->
                <div class="tabla-box" style="margin-top:20px">
                    <h3>Registros de plagas</h3>
                    <asp:GridView ID="gvPlagas" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvPlagas_RowCommand" DataKeyNames="PlagaId">
                        <Columns>
                            <asp:BoundField DataField="PlagaId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                            <asp:BoundField DataField="NombrePlaga" HeaderText="Plaga" />
                            <asp:BoundField DataField="NivelAfectacion" HeaderText="Nivel" />
                            <asp:BoundField DataField="FechaDeteccion" HeaderText="Detección" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="ProductoAplicado" HeaderText="Producto" />
                            <asp:BoundField DataField="FechaControl" HeaderText="Control" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>