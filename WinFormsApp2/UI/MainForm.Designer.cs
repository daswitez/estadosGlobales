namespace WinFormsApp2.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox grpP1;
        private System.Windows.Forms.GroupBox grpP2;
        private System.Windows.Forms.GroupBox grpP3;

        private System.Windows.Forms.Label lblP1EstadoActual;
        private System.Windows.Forms.Label lblP1Snapshot;
        private System.Windows.Forms.Label lblP1Transito;

        private System.Windows.Forms.Label lblP2EstadoActual;
        private System.Windows.Forms.Label lblP2Snapshot;
        private System.Windows.Forms.Label lblP2Transito;

        private System.Windows.Forms.Label lblP3EstadoActual;
        private System.Windows.Forms.Label lblP3Snapshot;
        private System.Windows.Forms.Label lblP3Transito;

        private System.Windows.Forms.Button btnIniciarSnapshot;
        private System.Windows.Forms.Button btnEnviarP3P2;
        private System.Windows.Forms.Button btnEntregar;
        private System.Windows.Forms.Button btnEscenario;
        private System.Windows.Forms.Button btnReiniciar;

        private System.Windows.Forms.ListBox lstEventos;
        private System.Windows.Forms.ListBox lstCola;
        private System.Windows.Forms.Label lblEventos;
        private System.Windows.Forms.Label lblCola;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.TextBox txtResumen;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpP1 = new System.Windows.Forms.GroupBox();
            this.lblP1EstadoActual = new System.Windows.Forms.Label();
            this.lblP1Snapshot = new System.Windows.Forms.Label();
            this.lblP1Transito = new System.Windows.Forms.Label();

            this.grpP2 = new System.Windows.Forms.GroupBox();
            this.lblP2EstadoActual = new System.Windows.Forms.Label();
            this.lblP2Snapshot = new System.Windows.Forms.Label();
            this.lblP2Transito = new System.Windows.Forms.Label();

            this.grpP3 = new System.Windows.Forms.GroupBox();
            this.lblP3EstadoActual = new System.Windows.Forms.Label();
            this.lblP3Snapshot = new System.Windows.Forms.Label();
            this.lblP3Transito = new System.Windows.Forms.Label();

            this.btnIniciarSnapshot = new System.Windows.Forms.Button();
            this.btnEnviarP3P2 = new System.Windows.Forms.Button();
            this.btnEntregar = new System.Windows.Forms.Button();
            this.btnEscenario = new System.Windows.Forms.Button();
            this.btnReiniciar = new System.Windows.Forms.Button();

            this.lstEventos = new System.Windows.Forms.ListBox();
            this.lstCola = new System.Windows.Forms.ListBox();
            this.lblEventos = new System.Windows.Forms.Label();
            this.lblCola = new System.Windows.Forms.Label();
            this.lblResumen = new System.Windows.Forms.Label();
            this.txtResumen = new System.Windows.Forms.TextBox();

            this.SuspendLayout();

            // grpP1
            this.grpP1.Controls.Add(this.lblP1EstadoActual);
            this.grpP1.Controls.Add(this.lblP1Snapshot);
            this.grpP1.Controls.Add(this.lblP1Transito);
            this.grpP1.Location = new System.Drawing.Point(20, 20);
            this.grpP1.Name = "grpP1";
            this.grpP1.Size = new System.Drawing.Size(350, 120);
            this.grpP1.TabIndex = 0;
            this.grpP1.TabStop = false;
            this.grpP1.Text = "Proceso P1";

            // lblP1EstadoActual
            this.lblP1EstadoActual.AutoSize = true;
            this.lblP1EstadoActual.Location = new System.Drawing.Point(15, 28);
            this.lblP1EstadoActual.Name = "lblP1EstadoActual";
            this.lblP1EstadoActual.Size = new System.Drawing.Size(83, 15);
            this.lblP1EstadoActual.TabIndex = 0;
            this.lblP1EstadoActual.Text = "Estado actual:";

            // lblP1Snapshot
            this.lblP1Snapshot.AutoSize = true;
            this.lblP1Snapshot.Location = new System.Drawing.Point(15, 53);
            this.lblP1Snapshot.Name = "lblP1Snapshot";
            this.lblP1Snapshot.Size = new System.Drawing.Size(58, 15);
            this.lblP1Snapshot.TabIndex = 1;
            this.lblP1Snapshot.Text = "Snapshot:";

            // lblP1Transito
            this.lblP1Transito.AutoSize = true;
            this.lblP1Transito.Location = new System.Drawing.Point(15, 78);
            this.lblP1Transito.Name = "lblP1Transito";
            this.lblP1Transito.Size = new System.Drawing.Size(66, 15);
            this.lblP1Transito.TabIndex = 2;
            this.lblP1Transito.Text = "En tránsito:";

            // grpP2
            this.grpP2.Controls.Add(this.lblP2EstadoActual);
            this.grpP2.Controls.Add(this.lblP2Snapshot);
            this.grpP2.Controls.Add(this.lblP2Transito);
            this.grpP2.Location = new System.Drawing.Point(400, 20);
            this.grpP2.Name = "grpP2";
            this.grpP2.Size = new System.Drawing.Size(350, 120);
            this.grpP2.TabIndex = 1;
            this.grpP2.TabStop = false;
            this.grpP2.Text = "Proceso P2";

            // lblP2EstadoActual
            this.lblP2EstadoActual.AutoSize = true;
            this.lblP2EstadoActual.Location = new System.Drawing.Point(15, 28);
            this.lblP2EstadoActual.Name = "lblP2EstadoActual";
            this.lblP2EstadoActual.Size = new System.Drawing.Size(83, 15);
            this.lblP2EstadoActual.TabIndex = 0;
            this.lblP2EstadoActual.Text = "Estado actual:";

            // lblP2Snapshot
            this.lblP2Snapshot.AutoSize = true;
            this.lblP2Snapshot.Location = new System.Drawing.Point(15, 53);
            this.lblP2Snapshot.Name = "lblP2Snapshot";
            this.lblP2Snapshot.Size = new System.Drawing.Size(58, 15);
            this.lblP2Snapshot.TabIndex = 1;
            this.lblP2Snapshot.Text = "Snapshot:";

            // lblP2Transito
            this.lblP2Transito.AutoSize = true;
            this.lblP2Transito.Location = new System.Drawing.Point(15, 78);
            this.lblP2Transito.Name = "lblP2Transito";
            this.lblP2Transito.Size = new System.Drawing.Size(66, 15);
            this.lblP2Transito.TabIndex = 2;
            this.lblP2Transito.Text = "En tránsito:";

            // grpP3
            this.grpP3.Controls.Add(this.lblP3EstadoActual);
            this.grpP3.Controls.Add(this.lblP3Snapshot);
            this.grpP3.Controls.Add(this.lblP3Transito);
            this.grpP3.Location = new System.Drawing.Point(780, 20);
            this.grpP3.Name = "grpP3";
            this.grpP3.Size = new System.Drawing.Size(350, 120);
            this.grpP3.TabIndex = 2;
            this.grpP3.TabStop = false;
            this.grpP3.Text = "Proceso P3";

            // lblP3EstadoActual
            this.lblP3EstadoActual.AutoSize = true;
            this.lblP3EstadoActual.Location = new System.Drawing.Point(15, 28);
            this.lblP3EstadoActual.Name = "lblP3EstadoActual";
            this.lblP3EstadoActual.Size = new System.Drawing.Size(83, 15);
            this.lblP3EstadoActual.TabIndex = 0;
            this.lblP3EstadoActual.Text = "Estado actual:";

            // lblP3Snapshot
            this.lblP3Snapshot.AutoSize = true;
            this.lblP3Snapshot.Location = new System.Drawing.Point(15, 53);
            this.lblP3Snapshot.Name = "lblP3Snapshot";
            this.lblP3Snapshot.Size = new System.Drawing.Size(58, 15);
            this.lblP3Snapshot.TabIndex = 1;
            this.lblP3Snapshot.Text = "Snapshot:";

            // lblP3Transito
            this.lblP3Transito.AutoSize = true;
            this.lblP3Transito.Location = new System.Drawing.Point(15, 78);
            this.lblP3Transito.Name = "lblP3Transito";
            this.lblP3Transito.Size = new System.Drawing.Size(66, 15);
            this.lblP3Transito.TabIndex = 2;
            this.lblP3Transito.Text = "En tránsito:";

            // btnIniciarSnapshot
            this.btnIniciarSnapshot.Location = new System.Drawing.Point(20, 170);
            this.btnIniciarSnapshot.Name = "btnIniciarSnapshot";
            this.btnIniciarSnapshot.Size = new System.Drawing.Size(180, 35);
            this.btnIniciarSnapshot.TabIndex = 3;
            this.btnIniciarSnapshot.Text = "Iniciar Snapshot en P1";
            this.btnIniciarSnapshot.UseVisualStyleBackColor = true;
            this.btnIniciarSnapshot.Click += new System.EventHandler(this.btnIniciarSnapshot_Click);

            // btnEnviarP3P2
            this.btnEnviarP3P2.Location = new System.Drawing.Point(220, 170);
            this.btnEnviarP3P2.Name = "btnEnviarP3P2";
            this.btnEnviarP3P2.Size = new System.Drawing.Size(190, 35);
            this.btnEnviarP3P2.TabIndex = 4;
            this.btnEnviarP3P2.Text = "Enviar mensaje P3 -> P2";
            this.btnEnviarP3P2.UseVisualStyleBackColor = true;
            this.btnEnviarP3P2.Click += new System.EventHandler(this.btnEnviarP3P2_Click);

            // btnEntregar
            this.btnEntregar.Location = new System.Drawing.Point(430, 170);
            this.btnEntregar.Name = "btnEntregar";
            this.btnEntregar.Size = new System.Drawing.Size(150, 35);
            this.btnEntregar.TabIndex = 5;
            this.btnEntregar.Text = "Entregar siguiente";
            this.btnEntregar.UseVisualStyleBackColor = true;
            this.btnEntregar.Click += new System.EventHandler(this.btnEntregar_Click);

            // btnEscenario
            this.btnEscenario.Location = new System.Drawing.Point(600, 170);
            this.btnEscenario.Name = "btnEscenario";
            this.btnEscenario.Size = new System.Drawing.Size(160, 35);
            this.btnEscenario.TabIndex = 6;
            this.btnEscenario.Text = "Ejecutar escenario";
            this.btnEscenario.UseVisualStyleBackColor = true;
            this.btnEscenario.Click += new System.EventHandler(this.btnEscenario_Click);

            // btnReiniciar
            this.btnReiniciar.Location = new System.Drawing.Point(780, 170);
            this.btnReiniciar.Name = "btnReiniciar";
            this.btnReiniciar.Size = new System.Drawing.Size(120, 35);
            this.btnReiniciar.TabIndex = 7;
            this.btnReiniciar.Text = "Reiniciar";
            this.btnReiniciar.UseVisualStyleBackColor = true;
            this.btnReiniciar.Click += new System.EventHandler(this.btnReiniciar_Click);

            // lblCola
            this.lblCola.AutoSize = true;
            this.lblCola.Location = new System.Drawing.Point(20, 230);
            this.lblCola.Name = "lblCola";
            this.lblCola.Size = new System.Drawing.Size(103, 15);
            this.lblCola.TabIndex = 8;
            this.lblCola.Text = "Cola de mensajes";

            // lstCola
            this.lstCola.FormattingEnabled = true;
            this.lstCola.ItemHeight = 15;
            this.lstCola.Location = new System.Drawing.Point(20, 255);
            this.lstCola.Name = "lstCola";
            this.lstCola.Size = new System.Drawing.Size(350, 184);
            this.lstCola.TabIndex = 9;

            // lblEventos
            this.lblEventos.AutoSize = true;
            this.lblEventos.Location = new System.Drawing.Point(400, 230);
            this.lblEventos.Name = "lblEventos";
            this.lblEventos.Size = new System.Drawing.Size(86, 15);
            this.lblEventos.TabIndex = 10;
            this.lblEventos.Text = "Log de eventos";

            // lstEventos
            this.lstEventos.FormattingEnabled = true;
            this.lstEventos.HorizontalScrollbar = true;
            this.lstEventos.ItemHeight = 15;
            this.lstEventos.Location = new System.Drawing.Point(400, 255);
            this.lstEventos.Name = "lstEventos";
            this.lstEventos.Size = new System.Drawing.Size(730, 274);
            this.lstEventos.TabIndex = 11;

            // lblResumen
            this.lblResumen.AutoSize = true;
            this.lblResumen.Location = new System.Drawing.Point(20, 460);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(179, 15);
            this.lblResumen.TabIndex = 12;
            this.lblResumen.Text = "Resumen del snapshot global";

            // txtResumen
            this.txtResumen.Location = new System.Drawing.Point(20, 485);
            this.txtResumen.Multiline = true;
            this.txtResumen.Name = "txtResumen";
            this.txtResumen.ReadOnly = true;
            this.txtResumen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResumen.Size = new System.Drawing.Size(350, 150);
            this.txtResumen.TabIndex = 13;

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 680);
            this.Controls.Add(this.txtResumen);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.lstEventos);
            this.Controls.Add(this.lblEventos);
            this.Controls.Add(this.lstCola);
            this.Controls.Add(this.lblCola);
            this.Controls.Add(this.btnReiniciar);
            this.Controls.Add(this.btnEscenario);
            this.Controls.Add(this.btnEntregar);
            this.Controls.Add(this.btnEnviarP3P2);
            this.Controls.Add(this.btnIniciarSnapshot);
            this.Controls.Add(this.grpP3);
            this.Controls.Add(this.grpP2);
            this.Controls.Add(this.grpP1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simulador de Snapshot - Chandy-Lamport";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}