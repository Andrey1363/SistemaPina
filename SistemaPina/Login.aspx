<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaPina.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.less" />
    
</head>
<body>
    <div class="login-contenedor">
    <form id="form1" runat="server">
        <div class="login-box">

            <!-- Ícono y título -->
            <div class="icono">🍍</div>
            <h1>Sistema de Gestión de Piña</h1>
            <p>Ingresá tus credenciales para continuar</p>

            <!-- Usuario -->
            <div class="campo">
                <label>Usuario</label>
                <asp:TextBox ID="txtUsuario" runat="server" placeholder="Escriba su usuario"></asp:TextBox>
            </div>

            <!-- Contraseña -->
            <div class="campo">
                <label>Contraseña</label>
                <asp:TextBox ID="txtContrasena" runat="server" placeholder="Escriba su contraseña" TextMode="Password"></asp:TextBox>
            </div>

            <!-- Botón ingresar -->
            <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="btn-ingresar" OnClick="btnIngresar_Click" />

            <!-- Mensaje de error (empieza oculto) -->
            <asp:Label ID="lblError" runat="server" Text="" CssClass="mensaje-error" Visible="false"></asp:Label>

        </div>
    </form>
</body>
</html>
