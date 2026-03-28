using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsApp2.Models;
using WinFormsApp2.Services;

namespace WinFormsApp2.UI
{
    public partial class MainForm : Form
    {
        private readonly SnapshotSimulator _simulator;

        private readonly Dictionary<string, PointF> _nodePositions = new();
        private AnimatedMessage? _currentAnimation;
        private readonly HashSet<string> _highlightedTransitChannels = new();

        public MainForm()
        {
            InitializeComponent();

            _simulator = new SnapshotSimulator();

            ConfigurarNodos();

            animationTimer.Interval = 30;
            animationTimer.Tick += animationTimer_Tick;

            panelCanvas.Paint += panelCanvas_Paint;
            panelCanvas.Resize += panelCanvas_Resize;

            ActualizarUI();
        }

        private void ConfigurarNodos()
        {
            _nodePositions["P1"] = new PointF(140, 110);
            _nodePositions["P2"] = new PointF(620, 110);
            _nodePositions["P3"] = new PointF(380, 290);
        }

        private void btnIniciarSnapshot_Click(object sender, EventArgs e)
        {
            _simulator.IniciarSnapshotEnP1();
            ActualizarCanalesResaltados();
            ActualizarUI();
        }

        private void btnEnviarP3P2_Click(object sender, EventArgs e)
        {
            _simulator.EnviarMensajeNormalP3aP2();
            ActualizarUI();
        }

        private void btnEventoInternoP3_Click(object sender, EventArgs e)
        {
            _simulator.EjecutarEventoInternoEnP3();
            ActualizarUI();
        }

        private void btnEntregar_Click(object sender, EventArgs e)
        {
            IniciarAnimacionSiguienteMensaje();
        }

        private void btnEscenario_Click(object sender, EventArgs e)
        {
            animationTimer.Stop();
            _currentAnimation = null;

            _simulator.EjecutarEscenarioDidactico();
            ActualizarCanalesResaltados();
            ActualizarUI();

            MessageBox.Show(
                "Se ejecutó el escenario didáctico completo.\n\n" +
                "Observa especialmente:\n" +
                "- P3: Estado actual vs estado capturado\n" +
                "- Canal P3->P2 resaltado por mensaje en tránsito\n" +
                "- Resumen global del snapshot",
                "Escenario automático",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            animationTimer.Stop();
            _currentAnimation = null;
            _highlightedTransitChannels.Clear();

            _simulator.Reiniciar();
            ActualizarUI();
        }

        private void IniciarAnimacionSiguienteMensaje()
        {
            if (_currentAnimation != null)
            {
                return;
            }

            Mensaje? siguiente = _simulator.ObtenerSiguienteMensajePendiente();

            if (siguiente == null)
            {
                _simulator.LogEventos.Add("No hay mensajes pendientes en la cola.");
                ActualizarUI();
                return;
            }

            PointF start = _nodePositions[siguiente.Origen];
            PointF end = _nodePositions[siguiente.Destino];

            _currentAnimation = new AnimatedMessage
            {
                Mensaje = siguiente,
                Start = start,
                End = end,
                Progress = 0f
            };

            animationTimer.Start();
        }

        private void animationTimer_Tick(object? sender, EventArgs e)
        {
            if (_currentAnimation == null)
            {
                animationTimer.Stop();
                return;
            }

            _currentAnimation.Progress += 0.04f;

            if (_currentAnimation.Progress >= 1f)
            {
                _currentAnimation.Progress = 1f;
                animationTimer.Stop();

                _simulator.EntregarSiguienteMensaje();
                _currentAnimation = null;

                ActualizarCanalesResaltados();
                ActualizarUI();
                return;
            }

            panelCanvas.Invalidate();
        }

        private void ActualizarCanalesResaltados()
        {
            _highlightedTransitChannels.Clear();

            foreach (var proceso in _simulator.Procesos.Values)
            {
                foreach (var entrada in proceso.MensajesEnTransitoPorCanal)
                {
                    if (entrada.Value.Count > 0)
                    {
                        _highlightedTransitChannels.Add(entrada.Key);
                    }
                }
            }
        }

        private void ActualizarUI()
        {
            var p1 = _simulator.Procesos["P1"];
            var p2 = _simulator.Procesos["P2"];
            var p3 = _simulator.Procesos["P3"];

            lblP1EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p1)}";
            lblP1Snapshot.Text = $"Capturado: {(p1.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP1Transito.Text = $"En tránsito: {p1.ObtenerMensajesEnTransitoTexto()}";

            lblP2EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p2)}";
            lblP2Snapshot.Text = $"Capturado: {(p2.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP2Transito.Text = $"En tránsito: {p2.ObtenerMensajesEnTransitoTexto()}";

            lblP3EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p3)}";
            lblP3Snapshot.Text = $"Capturado: {(p3.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP3Transito.Text = $"En tránsito: {p3.ObtenerMensajesEnTransitoTexto()}";

            lstCola.Items.Clear();
            foreach (string item in _simulator.ObtenerColaMensajesTexto())
            {
                lstCola.Items.Add(item);
            }

            lstEventos.Items.Clear();
            foreach (string log in _simulator.LogEventos)
            {
                lstEventos.Items.Add(log);
            }

            txtResumen.Text = _simulator.ObtenerResumenSnapshotGlobal();

            panelCanvas.Invalidate();
        }

        private string FormatearEstadoConDelta(Proceso proceso)
        {
            if (!proceso.EstadoGuardado.HasValue)
            {
                return proceso.EstadoLocalActual.ToString();
            }

            int delta = proceso.EstadoLocalActual - proceso.EstadoGuardado.Value;

            if (delta == 0)
            {
                return $"{proceso.EstadoLocalActual} (Δ=0)";
            }

            string deltaTexto = delta > 0 ? $"+{delta}" : delta.ToString();
            return $"{proceso.EstadoLocalActual} (Δ={deltaTexto})";
        }

        private void panelCanvas_Resize(object? sender, EventArgs e)
        {
            panelCanvas.Invalidate();
        }

        private void panelCanvas_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DibujarCanal(g, "P1", "P2");
            DibujarCanal(g, "P1", "P3");
            DibujarCanal(g, "P3", "P2");

            DibujarNodo(g, "P1");
            DibujarNodo(g, "P2");
            DibujarNodo(g, "P3");

            if (_currentAnimation != null)
            {
                DibujarMensajeAnimado(g, _currentAnimation);
            }
        }

        private void DibujarCanal(Graphics g, string origen, string destino)
        {
            string canal = $"{origen}->{destino}";
            PointF start = _nodePositions[origen];
            PointF end = _nodePositions[destino];

            bool enTransito = _highlightedTransitChannels.Contains(canal);

            Color color = enTransito ? Color.Crimson : Color.Gray;
            float width = enTransito ? 4f : 2f;

            using Pen pen = new Pen(color, width);
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);

            PointF startEdge = CalcularPuntoEnBorde(start, end, 48);
            PointF endEdge = CalcularPuntoEnBorde(end, start, 48);

            g.DrawLine(pen, startEdge, endEdge);

            float midX = (startEdge.X + endEdge.X) / 2f;
            float midY = (startEdge.Y + endEdge.Y) / 2f;

            using Font font = new Font("Segoe UI", 9, FontStyle.Bold);
            using Brush brush = new SolidBrush(color);

            g.DrawString(canal, font, brush, midX - 34, midY - 22);

            if (enTransito)
            {
                g.DrawString("En tránsito", font, brush, midX - 38, midY - 4);
            }
        }

        private void DibujarNodo(Graphics g, string nombre)
        {
            var proceso = _simulator.Procesos[nombre];
            PointF center = _nodePositions[nombre];

            RectangleF rect = new RectangleF(center.X - 52, center.Y - 52, 104, 104);

            bool capturado = proceso.SnapshotGuardado;
            bool divergente = proceso.EstadoGuardado.HasValue && proceso.EstadoLocalActual != proceso.EstadoGuardado.Value;

            Color fill;
            Color border;

            if (!capturado)
            {
                fill = Color.FromArgb(180, 230, 230, 230);
                border = Color.DimGray;
            }
            else if (divergente)
            {
                fill = Color.FromArgb(200, 255, 236, 179);
                border = Color.DarkOrange;
            }
            else
            {
                fill = Color.FromArgb(180, 230, 255, 180);
                border = Color.ForestGreen;
            }

            using Brush fillBrush = new SolidBrush(fill);
            using Pen borderPen = new Pen(border, 3);

            g.FillEllipse(fillBrush, rect);
            g.DrawEllipse(borderPen, rect);

            using Font titleFont = new Font("Segoe UI", 11, FontStyle.Bold);
            using Font textFont = new Font("Segoe UI", 8, FontStyle.Regular);
            using Font badgeFont = new Font("Segoe UI", 7, FontStyle.Bold);
            using Brush textBrush = new SolidBrush(Color.Black);
            using Brush badgeBrush = new SolidBrush(border);

            string snapshotTexto = proceso.EstadoGuardado?.ToString() ?? "-";

            g.DrawString(nombre, titleFont, textBrush, center.X - 14, center.Y - 32);
            g.DrawString($"Actual: {proceso.EstadoLocalActual}", textFont, textBrush, center.X - 34, center.Y - 8);
            g.DrawString($"Capturado: {snapshotTexto}", textFont, textBrush, center.X - 42, center.Y + 13);

            if (divergente)
            {
                g.DrawString("ACTUAL ≠ CAPTURADO", badgeFont, badgeBrush, center.X - 50, center.Y + 32);
            }
        }

        private void DibujarMensajeAnimado(Graphics g, AnimatedMessage animated)
        {
            PointF pos = animated.GetCurrentPosition();

            Color color = animated.Mensaje.Tipo == TipoMensaje.Marker ? Color.RoyalBlue : Color.DarkOrange;
            string texto = animated.Mensaje.Tipo == TipoMensaje.Marker ? "MK" : animated.Mensaje.Id;

            RectangleF rect = new RectangleF(pos.X - 18, pos.Y - 12, 36, 24);

            using Brush fill = new SolidBrush(color);
            using Pen border = new Pen(Color.Black, 1.5f);
            using Brush textBrush = new SolidBrush(Color.White);
            using Font font = new Font("Segoe UI", 8, FontStyle.Bold);
            using StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.FillEllipse(fill, rect);
            g.DrawEllipse(border, rect);
            g.DrawString(texto, font, textBrush, rect, sf);
        }

        private PointF CalcularPuntoEnBorde(PointF desde, PointF hacia, float radio)
        {
            float dx = hacia.X - desde.X;
            float dy = hacia.Y - desde.Y;
            float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distancia == 0)
            {
                return desde;
            }

            float ux = dx / distancia;
            float uy = dy / distancia;

            return new PointF(
                desde.X + ux * radio,
                desde.Y + uy * radio
            );
        }
    }
}