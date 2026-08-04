<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="SistemaPina.Reportes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reportes - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
    <!-- Chart.js para los gráficos -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.0/chart.umd.min.js"></script>
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
                <h2>Reportes</h2>
                <p>Consultá y analizá la información del sistema por período y ubicación.</p>

                <!-- ================================================
                     SECCIÓN 1: REPORTE DE PLAGAS Y ENFERMEDADES
                     ================================================ -->
                <div class="seccion-titulo">
                    <h3>Reporte de Plagas y Enfermedades</h3>
                    <p>Historial fitosanitario por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaPlagas" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaPlagas_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLotePlagas" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarPlagas" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarPlagas_Click" />
                </div>

                <asp:Panel ID="panelResultadoPlagas" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Plagas y Enfermedades</h3>
                        <asp:GridView ID="gvReportePlagas" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="Nombre" HeaderText="Plaga/Enfermedad" />
                                <asp:BoundField DataField="NivelAfectacion" HeaderText="Nivel" />
                                <asp:BoundField DataField="FechaDeteccion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <!-- Gráfico de plagas -->
                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Frecuencia de Plagas y Enfermedades</h3>
                        <canvas id="graficoPlagas" height="100"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosPlagas" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 2: REPORTE DE LABORES
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3>Reporte de Labores</h3>
                    <p>Actividades realizadas por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaLabores" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaLabores_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLoteLabores" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarLabores" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarLabores_Click" />
                </div>

                <asp:Panel ID="panelResultadoLabores" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Labores</h3>
                        <asp:GridView ID="gvReporteLabores" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="TipoLabor" HeaderText="Labor" />
                                <asp:BoundField DataField="FechaLabor" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                                <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <!-- Gráfico de labores -->
                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Tipos de Labores Realizadas</h3>
                        <canvas id="graficoLabores" height="60"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosLabores" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 3: RENDIMIENTO POR GRUPO DE FORZA
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3>Rendimiento por Grupo de Forza</h3>
                    <p>Kilos cosechados por grupo</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaRendimiento" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <asp:Button ID="btnGenerarRendimiento" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarRendimiento_Click" />
                </div>

                <asp:Panel ID="panelResultadoRendimiento" runat="server" Visible="false">
    <div class="tabla-box">
        <h3>Resultados — Rendimiento</h3>
        <asp:GridView ID="GridView1" runat="server" CssClass="tabla" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
                <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
                <asp:BoundField DataField="TotalPlantas" HeaderText="Total plantas" />
                <asp:BoundField DataField="FechaCosecha" HeaderText="Fecha cosecha" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="KilosCosechados" HeaderText="Kilos cosechados" />
                <asp:BoundField DataField="KgPorPlanta" HeaderText="Kg por planta" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="tabla-box" style="margin-top:20px">
        <h3>Rendimiento (Kg por planta) por Grupo de Forza</h3>
        <canvas id="graficoRendimiento" height="100"></canvas>
    </div>
    <asp:HiddenField ID="HiddenField1" runat="server" />

    <!-- Script del gráfico DENTRO del panel -->
    <script>
        (function () {
            var hf = document.getElementById('<%= hfDatosRendimiento.ClientID %>');
            if (!hf || !hf.value) return;

            var datos3 = JSON.parse(hf.value);
            var numericos = datos3.valores.map(function (v) { return parseFloat(v); });
            var suma = numericos.reduce(function (a, b) { return a + b; }, 0);
            var promedio = suma / numericos.length;
            var lineaPromedio = numericos.map(function () { return promedio; });

            new Chart(document.getElementById('graficoRendimiento'), {
                type: 'bar',
                data: {
                    labels: datos3.labels,
                    datasets: [
                        {
                            label: 'Kg por planta',
                            data: numericos,
                            backgroundColor: 'rgba(46, 125, 50, 0.7)',
                            borderColor: '#2e7d32',
                            borderWidth: 1,
                            borderRadius: 6,
                            order: 2
                        },
                        {
                            label: 'Promedio',
                            data: lineaPromedio,
                            type: 'line',
                            borderColor: '#c62828',
                            borderWidth: 2,
                            pointRadius: 0,
                            fill: false,
                            order: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: { position: 'top' },
                        title: {
                            display: true,
                            text: 'Rendimiento por Grupo de Forza',
                            font: { size: 14 }
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 5,
                            ticks: { stepSize: 0.5 },
                            title: { display: true, text: 'Kg por planta' }
                        },
                        x: { grid: { display: false } }
                    }
                }
            });
        })();
    </script>
</asp:Panel>

                        
                 <asp:GridView ID="gvReporteRendimiento" runat="server" CssClass="tabla" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
        <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
        <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
        <asp:BoundField DataField="FechaCosecha" HeaderText="Fecha cosecha" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:BoundField DataField="KilosCosechados" HeaderText="Kilos cosechados" />
    </Columns>
</asp:GridView>
                    </div>

                    <!-- Gráfico de rendimiento -->
                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Kilos Cosechados por Grupo de Forza</h3>
                        <canvas id="graficoRendimiento" height="100"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosRendimiento" runat="server" />
                </asp:Panel>

            </div>
        </div>

    </form>

    <!-- Script para generar los gráficos con Chart.js -->
    <script>
        // ─────────────────────────────────────────
        // GRÁFICO 1: PLAGAS Y ENFERMEDADES
        // Barras horizontales con color rojo
        // ─────────────────────────────────────────
        var datosPlagas = document.getElementById('<%= hfDatosPlagas.ClientID %>').value;
        if (datosPlagas) {
            var datos = JSON.parse(datosPlagas);
            new Chart(document.getElementById('graficoPlagas'), {
                type: 'bar',
                data: {
                    labels: datos.labels,
                    datasets: [{
                        label: 'Total reportes',
                        data: datos.valores,
                        backgroundColor: datos.labels.map((_, i) =>
                            `rgba(198, 40, 40, ${0.5 + (i * 0.1)})`
                        ),
                        borderColor: '#c62828',
                        borderWidth: 1,
                        borderRadius: 6
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    plugins: {
                        legend: { display: false },
                        title: {
                            display: true,
                            text: 'Frecuencia de reportes',
                            font: { size: 14 }
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true,
                            ticks: { stepSize: 1 },
                            grid: { color: 'rgba(0,0,0,0.05)' }
                        },
                        y: {
                            grid: { display: false }
                        }
                    }
                }
            });
        }

        // ─────────────────────────────────────────
        // GRÁFICO 2: LABORES
        // Dona moderna con leyenda
        // ─────────────────────────────────────────
        var datosLabores = document.getElementById('<%= hfDatosLabores.ClientID %>').value;
    if (datosLabores) {
        var datos2 = JSON.parse(datosLabores);
        new Chart(document.getElementById('graficoLabores'), {
            type: 'doughnut',
            data: {
                labels: datos2.labels,
                datasets: [{
                    data: datos2.valores,
                    backgroundColor: [
                        '#2e7d32',
                        '#1565c0',
                        '#e65100',
                        '#6a1b9a',
                        '#00695c',
                        '#f9a825'
                    ],
                    borderWidth: 2,
                    borderColor: '#fff',
                    hoverOffset: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                cutout: '65%',
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            padding: 20,
                            font: { size: 13 }
                        }
                    },
                    title: {
                        display: true,
                        text: 'Distribución de labores',
                        font: { size: 14 }
                    }
                }
            }
        });
    }

    // ─────────────────────────────────────────
    // GRÁFICO 3: RENDIMIENTO
    // Barras con línea de promedio
    // ─────────────────────────────────────────
        var datosRendimiento = document.getElementById('<%= hfDatosRendimiento.ClientID %>').value;
        
        if (datosRendimiento) {
            var datos3 = JSON.parse(datosRendimiento);

            // Calcular el promedio
            var suma = datos3.valores.reduce((a, b) => a + parseFloat(b), 0);
            var promedio = suma / datos3.valores.length;
            var lineaPromedio = datos3.valores.map(() => promedio);

            new Chart(document.getElementById('graficoRendimiento'), {
                type: 'bar',
                data: {
                    labels: datos3.labels,
                    datasets: [
                        {
                            label: 'Kg por planta',
                            data: datos3.valores,
                            backgroundColor: 'rgba(46, 125, 50, 0.7)',
                            borderColor: '#2e7d32',
                            borderWidth: 1,
                            borderRadius: 6,
                            order: 2
                        },
                        {
                            label: 'Promedio',
                            data: lineaPromedio,
                            type: 'line',
                            borderColor: '#c62828',
                            borderWidth: 2,
                            borderDash: [6, 4],
                            pointRadius: 0,
                            fill: false,
                            order: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                            labels: { font: { size: 13 } }
                        },
                        title: {
                            display: true,
                            text: 'Rendimiento (Kg por planta) por Grupo de Forza',
                            font: { size: 14 }
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 5,
                            ticks: {
                                stepSize: 0.5
                            },
                            grid: { color: 'rgba(0,0,0,0.05)' },
                            title: {
                                display: true,
                                text: 'Kilogramos'
                            }
                        },
                        x: {
                            grid: { display: false }
                        }
                    }
                }
            });
        }
    </script>

</body>
</html>--%>

<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="SistemaPina.Reportes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reportes - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
    <!-- Chart.js para los gráficos -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.0/chart.umd.min.js"></script>
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
                    <li><a href="Reportes.aspx">📊 Reportes</a></li>
                    <asp:Panel ID="panelUsuarios" runat="server">
                        <li><a href="Usuarios.aspx">Usuarios</a></li>
                    </asp:Panel>
                </ul>
            </div>

            <div class="contenido">
                <h2>📊 Reportes</h2>
                <p>Consultá y analizá la información del sistema por período y ubicación.</p>

                <!-- ================================================
                     SECCIÓN 1: REPORTE DE PLAGAS Y ENFERMEDADES
                     ================================================ -->
                <div class="seccion-titulo">
                    <h3>Reporte de Plagas y Enfermedades</h3>
                    <p>Historial fitosanitario por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaPlagas" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaPlagas_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLotePlagas" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarPlagas" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarPlagas_Click" />
                </div>

                <asp:Panel ID="panelResultadoPlagas" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Plagas y Enfermedades</h3>
                        <asp:GridView ID="gvReportePlagas" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="Nombre" HeaderText="Plaga/Enfermedad" />
                                <asp:BoundField DataField="NivelAfectacion" HeaderText="Nivel" />
                                <asp:BoundField DataField="FechaDeteccion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Frecuencia de Plagas y Enfermedades</h3>
                        <canvas id="graficoPlagas" height="100"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosPlagas" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 2: REPORTE DE LABORES
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3>Reporte de Labores</h3>
                    <p>Actividades realizadas por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaLabores" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaLabores_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLoteLabores" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarLabores" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarLabores_Click" />
                </div>

                <asp:Panel ID="panelResultadoLabores" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Labores</h3>
                        <asp:GridView ID="gvReporteLabores" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="TipoLabor" HeaderText="Labor" />
                                <asp:BoundField DataField="FechaLabor" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                                <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Tipos de Labores Realizadas</h3>
                        <canvas id="graficoLabores" height="60"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosLabores" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 3: RENDIMIENTO POR GRUPO DE FORZA
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3>Rendimiento por Grupo de Forza</h3>
                    <p>Kilos cosechados por grupo</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaRendimiento" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <asp:Button ID="btnGenerarRendimiento" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarRendimiento_Click" />
                </div>

                <asp:Panel ID="panelResultadoRendimiento" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Rendimiento</h3>
                        <asp:GridView ID="gvReporteRendimiento" runat="server" CssClass="tabla" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
                                <asp:BoundField DataField="Bloques" HeaderText="Bloques" />
                                <asp:BoundField DataField="TotalPlantas" HeaderText="Total plantas" />
                                <asp:BoundField DataField="FechaCosecha" HeaderText="Fecha cosecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="KilosCosechados" HeaderText="Kilos cosechados" />
                                <asp:BoundField DataField="KgPorPlanta" HeaderText="Kg por planta" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Rendimiento (Kg por planta) por Grupo de Forza</h3>
                        <canvas id="graficoRendimiento" height="100"></canvas>
                    </div>
                    <asp:HiddenField ID="hfDatosRendimiento" runat="server" />
                </asp:Panel>

            </div>
        </div>

    </form>

    <!-- ========================================== -->
    <!-- SCRIPTS DE GRÁFICOS (CORREGIDOS) -->
    <!-- ========================================== -->
    <script>
        // ─────────────────────────────────────────
        // GRÁFICO 1: PLAGAS Y ENFERMEDADES
        // ─────────────────────────────────────────
        var datosPlagas = document.getElementById('<%= hfDatosPlagas.ClientID %>').value;
        if (datosPlagas) {
            var datos = JSON.parse(datosPlagas);
            new Chart(document.getElementById('graficoPlagas'), {
                type: 'bar',
                data: {
                    labels: datos.labels,
                    datasets: [{
                        label: 'Total reportes',
                        data: datos.valores,
                        backgroundColor: 'rgba(198, 40, 40, 0.7)',
                        borderColor: '#c62828',
                        borderWidth: 1,
                        borderRadius: 6
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    plugins: {
                        legend: { display: false },
                        title: {
                            display: true,
                            text: 'Frecuencia de reportes',
                            font: { size: 14 }
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true,
                            ticks: { stepSize: 1 },
                            grid: { color: 'rgba(0,0,0,0.05)' }
                        },
                        y: {
                            grid: { display: false }
                        }
                    }
                }
            });
        }

        // ─────────────────────────────────────────
        // GRÁFICO 2: LABORES
        // ─────────────────────────────────────────
        var datosLabores = document.getElementById('<%= hfDatosLabores.ClientID %>').value;
        if (datosLabores) {
            var datos2 = JSON.parse(datosLabores);
            new Chart(document.getElementById('graficoLabores'), {
                type: 'doughnut',
                data: {
                    labels: datos2.labels,
                    datasets: [{
                        data: datos2.valores,
                        backgroundColor: [
                            '#2e7d32',
                            '#1565c0',
                            '#e65100',
                            '#6a1b9a',
                            '#00695c',
                            '#f9a825'
                        ],
                        borderWidth: 2,
                        borderColor: '#fff',
                        hoverOffset: 8
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    cutout: '65%',
                    plugins: {
                        legend: {
                            position: 'right',
                            labels: {
                                padding: 20,
                                font: { size: 13 }
                            }
                        },
                        title: {
                            display: true,
                            text: 'Distribución de labores',
                            font: { size: 14 }
                        }
                    }
                }
            });
        }

        // ─────────────────────────────────────────
        // GRÁFICO 3: RENDIMIENTO (CORREGIDO)
        // ─────────────────────────────────────────
        var datosRendimiento = document.getElementById('<%= hfDatosRendimiento.ClientID %>').value;
        if (datosRendimiento) {
            var datos3 = JSON.parse(datosRendimiento);

            // Convertir valores a números (strings → numbers)
            var valoresNumericos = datos3.valores.map(function (v) { return parseFloat(v); });

            // Calcular el promedio de kg por planta (NO de kilos totales)
            var suma = valoresNumericos.reduce(function (a, b) { return a + b; }, 0);
            var promedio = suma / valoresNumericos.length;
            var lineaPromedio = valoresNumericos.map(function () { return promedio; });

            new Chart(document.getElementById('graficoRendimiento'), {
                type: 'bar',
                data: {
                    labels: datos3.labels,
                    datasets: [
                        {
                            label: 'Kg por planta',
                            data: valoresNumericos,
                            backgroundColor: 'rgba(46, 125, 50, 0.7)',
                            borderColor: '#2e7d32',
                            borderWidth: 1,
                            borderRadius: 6,
                            order: 2
                        },
                        {
                            label: 'Promedio',
                            data: lineaPromedio,
                            type: 'line',
                            borderColor: '#c62828',
                            borderWidth: 2,
                            borderDash: [6, 4],
                            pointRadius: 0,
                            fill: false,
                            order: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                            labels: { font: { size: 13 } }
                        },
                        title: {
                            display: true,
                            text: 'Rendimiento (Kg por planta) por Grupo de Forza',
                            font: { size: 14 }
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 5,
                            ticks: { stepSize: 0.5 },
                            grid: { color: 'rgba(0,0,0,0.05)' },
                            title: {
                                display: true,
                                text: 'Kilogramos por planta'
                            }
                        },
                        x: {
                            grid: { display: false }
                        }
                    }
                }
            });
        }
    </script>

</body>
</html>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="SistemaPina.Reportes" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Reportes - Sistema de Gestión de Piña</title>
    <link rel="stylesheet" href="Estilos.css" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.0/chart.umd.min.js"></script>
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
                    <li><a href="Reportes.aspx"> Reportes</a></li>
                    <asp:Panel ID="panelUsuarios" runat="server">
                        <li><a href="Usuarios.aspx">Usuarios</a></li>
                    </asp:Panel>
                </ul>
            </div>

            <div class="contenido">
                <h2> Reportes</h2>
                <p>Consultá y analizá la información del sistema por período y ubicación.</p>

                <!-- ================================================
                     SECCIÓN 1: REPORTE DE PLAGAS Y ENFERMEDADES
                     ================================================ -->
                <div class="seccion-titulo">
                    <h3> Reporte de Plagas y Enfermedades</h3>
                    <p>Historial fitosanitario por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaPlagas" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaPlagas_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLotePlagas" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta1" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarPlagas" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarPlagas_Click" />
                </div>

                <asp:Panel ID="panelResultadoPlagas" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Plagas y Enfermedades</h3>
                        <asp:GridView ID="gvReportePlagas" runat="server" CssClass="tabla" AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="Nombre" HeaderText="Plaga/Enfermedad" />
                                <asp:BoundField DataField="NivelAfectacion" HeaderText="Nivel" />
                                <asp:BoundField DataField="FechaDeteccion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Frecuencia de Plagas y Enfermedades</h3>
                        <div style="max-height:400px;">
                            <canvas id="graficoPlagas" height="200"></canvas>
                        </div>
                    </div>
                    <asp:HiddenField ID="hfDatosPlagas" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 2: REPORTE DE LABORES
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3> Reporte de Labores</h3>
                    <p>Actividades realizadas por lote y período</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaLabores" runat="server" CssClass="ddl" AutoPostBack="true" OnSelectedIndexChanged="ddlFincaLabores_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="campo-form">
                        <label>Lote (opcional)</label>
                        <asp:DropDownList ID="ddlLoteLabores" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <div class="reporte-fechas">
                        <div class="campo-form">
                            <label>Fecha desde</label>
                            <asp:TextBox ID="txtFechaDesde2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="campo-form">
                            <label>Fecha hasta</label>
                            <asp:TextBox ID="txtFechaHasta2" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnGenerarLabores" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarLabores_Click" />
                </div>

                <asp:Panel ID="panelResultadoLabores" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Labores</h3>
                        <asp:GridView ID="gvReporteLabores" runat="server" CssClass="tabla" AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreLote" HeaderText="Lote" />
                                <asp:BoundField DataField="NombreBloque" HeaderText="Bloque" />
                                <asp:BoundField DataField="TipoLabor" HeaderText="Labor" />
                                <asp:BoundField DataField="FechaLabor" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="Responsable" HeaderText="Responsable" />
                                <asp:BoundField DataField="Observaciones" HeaderText="Observaciones" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="tabla-box" style="margin-top:20px">
                        <h3>Tipos de Labores Realizadas</h3>
                        <div style="max-height:400px;">
                            <canvas id="graficoLabores" height="200"></canvas>
                        </div>
                    </div>
                    <asp:HiddenField ID="hfDatosLabores" runat="server" />
                </asp:Panel>

                <!-- ================================================
                     SECCIÓN 3: RENDIMIENTO POR GRUPO DE FORZA
                     ================================================ -->
                <div class="seccion-titulo" style="margin-top:30px">
                    <h3> Rendimiento por Grupo de Forza</h3>
                    <p>Kilos cosechados por grupo</p>
                </div>

                <div class="formulario-box">
                    <div class="campo-form">
                        <label>Finca</label>
                        <asp:DropDownList ID="ddlFincaRendimiento" runat="server" CssClass="ddl"></asp:DropDownList>
                    </div>
                    <asp:Button ID="btnGenerarRendimiento" runat="server" Text="Generar reporte" CssClass="btn-guardar" OnClick="btnGenerarRendimiento_Click" />
                </div>

                <asp:Panel ID="panelResultadoRendimiento" runat="server" Visible="false">
                    <div class="tabla-box">
                        <h3>Resultados — Rendimiento</h3>
                        <asp:GridView ID="gvReporteRendimiento" runat="server" CssClass="tabla" AutoGenerateColumns="false" AllowPaging="true" PageSize="10">
                            <Columns>
                                <asp:BoundField DataField="NombreFinca" HeaderText="Finca" />
                                <asp:BoundField DataField="NombreGrupo" HeaderText="Grupo Forza" />
                                <asp:BoundField DataField="FechaCosecha" HeaderText="Fecha cosecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="KilosCosechados" HeaderText="Kilos cosechados" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="TotalPlantas" HeaderText="Total plantas" DataFormatString="{0:N0}" />
                                <asp:BoundField DataField="KgPorPlanta" HeaderText="Kg por planta" DataFormatString="{0:N2}" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>

            </div>
        </div>

    </form>

    <!-- ========================================== -->
    <!-- SCRIPTS DE GRÁFICOS (SOLO PLAGAS Y LABORES) -->
    <!-- ========================================== -->
    <script>
        // ─────────────────────────────────────────
        // GRÁFICO 1: PLAGAS Y ENFERMEDADES
        // ─────────────────────────────────────────
        function crearGraficoPlagas() {
            var datosPlagas = document.getElementById('<%= hfDatosPlagas.ClientID %>').value;
            if (datosPlagas && datosPlagas !== '{"labels":[],"valores":[]}') {
                try {
                    var datos = JSON.parse(datosPlagas);
                    var ctx = document.getElementById('graficoPlagas').getContext('2d');
                    new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: datos.labels,
                            datasets: [{
                                label: 'Total reportes',
                                data: datos.valores,
                                backgroundColor: 'rgba(198, 40, 40, 0.7)',
                                borderColor: '#c62828',
                                borderWidth: 1,
                                borderRadius: 6
                            }]
                        },
                        options: {
                            indexAxis: 'y',
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: { display: false },
                                title: {
                                    display: true,
                                    text: 'Frecuencia de reportes',
                                    font: { size: 14, weight: 'bold' }
                                }
                            },
                            scales: {
                                x: {
                                    beginAtZero: true,
                                    ticks: { stepSize: 1 },
                                    grid: { color: 'rgba(0,0,0,0.05)' }
                                },
                                y: {
                                    grid: { display: false }
                                }
                            }
                        }
                    });
                } catch (e) {
                    console.error('Error al crear gráfico de plagas:', e);
                }
            }
        }

        // ─────────────────────────────────────────
        // GRÁFICO 2: LABORES
        // ─────────────────────────────────────────
        function crearGraficoLabores() {
            var datosLabores = document.getElementById('<%= hfDatosLabores.ClientID %>').value;
            if (datosLabores && datosLabores !== '{"labels":[],"valores":[]}') {
                try {
                    var datos = JSON.parse(datosLabores);
                    var colores = [
                        '#2e7d32', '#1565c0', '#e65100', '#6a1b9a',
                        '#00695c', '#f9a825', '#c62828', '#4a148c'
                    ];

                    var ctx = document.getElementById('graficoLabores').getContext('2d');
                    new Chart(ctx, {
                        type: 'doughnut',
                        data: {
                            labels: datos.labels,
                            datasets: [{
                                data: datos.valores,
                                backgroundColor: colores.slice(0, datos.labels.length),
                                borderWidth: 2,
                                borderColor: '#fff',
                                hoverOffset: 8
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            cutout: '65%',
                            plugins: {
                                legend: {
                                    position: 'right',
                                    labels: {
                                        padding: 20,
                                        font: { size: 13 }
                                    }
                                },
                                title: {
                                    display: true,
                                    text: 'Distribución de labores',
                                    font: { size: 14, weight: 'bold' }
                                }
                            }
                        }
                    });
                } catch (e) {
                    console.error('Error al crear gráfico de labores:', e);
                }
            }
        }

        // ─────────────────────────────────────────
        // INICIALIZAR GRÁFICOS AL CARGAR LA PÁGINA
        // ─────────────────────────────────────────
        document.addEventListener('DOMContentLoaded', function () {
            crearGraficoPlagas();
            crearGraficoLabores();
        });
    </script>

</body>
</html>