<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Fertilizaciones.aspx.cs" Inherits="SistemaPina.Fertilizaciones" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Fertilizaciones - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.less" />
</head>
<body>

    <form id="form1" runat="server">

        <div class="topbar">
            <div class="topbar-titulo">🍍 Sistema de Gestión de Piña</div>
            <div class="topbar-usuario">
                <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
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
                <h2>Fertilizaciones</h2>
                <p>Registrá las aplicaciones de fertilizantes por ciclo.</p>

                <!-- ================================================
                     SECCIÓN 1: FERTILIZACIÓN EN PLANTACIÓN
                     ================================================ -->
                <div class="seccion-titulo">
                    <h3>🌱 Fertilización en Plantación</h3>
                    <p>13 ciclos cada 15 días por bloque</p>
                </div>

                <div class="formulario-box">
                    <h3>Registrar ciclo de plantación</h3>

                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFinca" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFinca_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Lote</label>
                        <asp:DropDownList ID="ddlLote" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlLote_SelectedIndexChanged"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Bloques a fertilizar</label>
                        <!-- Botón para marcar todos los bloques -->
                        <asp:Button ID="btnMarcarTodos" runat="server" Text="Seleccionar todos" CssClass="btn-secundario" OnClick="btnMarcarTodos_Click" />
                        <asp:Button ID="btnDesmarcarTodos" runat="server" Text="Deseleccionar todos" CssClass="btn-secundario" OnClick="btnDesmarcarTodos_Click" />
                        <asp:CheckBoxList ID="cblBloques" runat="server" CssClass="check-list"></asp:CheckBoxList>
                    </div>

                    <div class="campo-form">
                        <label>Número de ciclo</label>
                        <asp:DropDownList ID="ddlCicloPlantacion" runat="server" CssClass="ddl">
                            <asp:ListItem Value="1">Ciclo 1</asp:ListItem>
                            <asp:ListItem Value="2">Ciclo 2</asp:ListItem>
                            <asp:ListItem Value="3">Ciclo 3</asp:ListItem>
                            <asp:ListItem Value="4">Ciclo 4</asp:ListItem>
                            <asp:ListItem Value="5">Ciclo 5</asp:ListItem>
                            <asp:ListItem Value="6">Ciclo 6</asp:ListItem>
                            <asp:ListItem Value="7">Ciclo 7</asp:ListItem>
                            <asp:ListItem Value="8">Ciclo 8</asp:ListItem>
                            <asp:ListItem Value="9">Ciclo 9</asp:ListItem>
                            <asp:ListItem Value="10">Ciclo 10</asp:ListItem>
                            <asp:ListItem Value="11">Ciclo 11</asp:ListItem>
                            <asp:ListItem Value="12">Ciclo 12</asp:ListItem>
                            <asp:ListItem Value="13">Ciclo 13</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Tipo de fertilizante</label>
                        <asp:TextBox ID="txtTipoFertilizante" runat="server" placeholder="Ej: Urea, NPK, Foliar"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de aplicación</label>
                        <asp:TextBox ID="txtFechaAplicacion" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservaciones" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardarPlantacion" runat="server" Text="Guardar ciclo plantación" CssClass="btn-guardar" OnClick="btnGuardarPlantacion_Click" />
                </div>

                <!-- Tabla fertilizaciones plantación -->
                <div class="tabla-box">
                    <h3>Ciclos de plantación registrados</h3>
                    <asp:GridView ID="gvPlantacion" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvPlantacion_RowCommand" DataKeyNames="FertilizacionId">
                        <Columns>
                            <asp:BoundField DataField="FertilizacionId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                            <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
                            <asp:BoundField DataField="NumeroCiclo" HeaderText="Ciclo" />
                            <asp:BoundField DataField="TipoFertilizante" HeaderText="Fertilizante" />
                            <asp:BoundField DataField="FechaAplicacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- ================================================
                     SECCIÓN 2: FERTILIZACIÓN EN FRUTA
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3>🍍 Fertilización en Fruta</h3>
                    <p>10 ciclos cada 10 días por Grupo de Forza</p>
                </div>

                <div class="formulario-box">
                    <h3>Registrar ciclo de fruta</h3>

                    <div class="campo-form">
                        <label>Grupo de Forza</label>
                        <asp:DropDownList ID="ddlGrupoForza" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Número de ciclo</label>
                        <asp:DropDownList ID="ddlCicloFruta" runat="server" CssClass="ddl">
                            <asp:ListItem Value="1">Ciclo 1</asp:ListItem>
                            <asp:ListItem Value="2">Ciclo 2</asp:ListItem>
                            <asp:ListItem Value="3">Ciclo 3</asp:ListItem>
                            <asp:ListItem Value="4">Ciclo 4</asp:ListItem>
                            <asp:ListItem Value="5">Ciclo 5</asp:ListItem>
                            <asp:ListItem Value="6">Ciclo 6</asp:ListItem>
                            <asp:ListItem Value="7">Ciclo 7</asp:ListItem>
                            <asp:ListItem Value="8">Ciclo 8</asp:ListItem>
                            <asp:ListItem Value="9">Ciclo 9</asp:ListItem>
                            <asp:ListItem Value="10">Ciclo 10</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="campo-form">
                        <label>Tipo de fertilizante</label>
                        <asp:TextBox ID="txtTipoFertilizanteFruta" runat="server" placeholder="Ej: Urea, NPK, Foliar"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Fecha de aplicación</label>
                        <asp:TextBox ID="txtFechaAplicacionFruta" runat="server" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="campo-form">
                        <label>Observaciones (opcional)</label>
                        <asp:TextBox ID="txtObservacionesFruta" runat="server" TextMode="MultiLine" Rows="3" placeholder="Escriba observaciones..."></asp:TextBox>
                    </div>

                    <asp:Label ID="lblMensajeFruta" runat="server" Text="" Visible="false"></asp:Label>

                    <asp:Button ID="btnGuardarFruta" runat="server" Text="Guardar ciclo fruta" CssClass="btn-guardar" OnClick="btnGuardarFruta_Click" />
                </div>

                <!-- Tabla fertilizaciones fruta -->
                <div class="tabla-box">
                    <h3>Ciclos de fruta registrados</h3>
                    <asp:GridView ID="gvFruta" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        OnRowCommand="gvFruta_RowCommand" DataKeyNames="FertilizacionFrutaId">
                        <Columns>
                            <asp:BoundField DataField="FertilizacionFrutaId" HeaderText="ID" />
                            <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                            <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
                            <asp:BoundField DataField="NumeroCiclo" HeaderText="Ciclo" />
                            <asp:BoundField DataField="TipoFertilizante" HeaderText="Fertilizante" />
                            <asp:BoundField DataField="FechaAplicacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:ButtonField ButtonType="Button" CommandName="Eliminar" Text="Eliminar" ControlStyle-CssClass="btn-eliminar" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </div>

    </form>
</body>
</html>
