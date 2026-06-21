<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaPina.Dashboard" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.less" />
</head>
<body>

    <form id="form1" runat="server">

        <!-- Barra superior -->
        <div class="topbar">
            <div class="topbar-titulo">🍍 Control de producción de piña</div>
            <div class="topbar-usuario">
                <!-- Mostrar el nombre del usuario que inició sesión -->
                <asp:Label ID="lblNombreUsuario" runat="server"></asp:Label>
                <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar sesión" CssClass="btn-cerrar" OnClick="btnCerrarSesion_Click" />
            </div>
        </div>

        <div class="contenedor-principal">

            <!-- Menú lateral izquierdo -->
            <div class="menu-lateral">
                <ul>
                    <li><a href="Fincas.aspx"> Fincas</a></li>
                    <li><a href="Lotes.aspx"> Lotes</a></li>
                    <li><a href="Bloques.aspx"> Bloques</a></li>
                    <li><a href="Siembras.aspx"> Siembras</a></li>
                    <li><a href="Cosechas.aspx"> Cosechas</a></li>
                    <li><a href="Plagas.aspx"> Plagas</a></li>
                    <li><a href="Enfermedades.aspx"> Enfermedades</a></li>
                    <li><a href="Fertilizaciones.aspx"> Fertilizaciones</a></li>
                    <li><a href="Labores.aspx"> Labores</a></li>
                    <li><a href="Reportes.aspx"> Reportes</a></li>
                    <!-- Solo visible para Admin -->
                    <asp:Panel ID="panelUsuarios" runat="server">
                        <li><a href="Usuarios.aspx"> Usuarios</a></li>
                    </asp:Panel>
                </ul>
            </div>

            
            <div class="contenido">
                <h2>Bienvenido al sistema</h2>
                <p>Seleccioná una opción del menú para comenzar.</p>

                <!-- Tarjetas de resumen -->
               <div class="tarjetas">
    <div class="tarjeta">
        <h3> Fincas</h3>
        <asp:Label ID="lblTotalFincas" runat="server" Text="0"></asp:Label>
        <p>registradas</p>
    </div>
    
    <div class="tarjeta">
        <h3> Grupos de Forza</h3>
        <asp:Label ID="lblTotalPlagas" runat="server" Text="0"></asp:Label>
        <p>registrados</p>
    </div>
    <div class="tarjeta">
        <h3> Cosechas</h3>
        <asp:Label ID="lblTotalFertilizaciones" runat="server" Text="0"></asp:Label>
        <p>realizadas</p>
    </div>
</div>
            </div>

        </div>

    </form>
</body>
</html>
