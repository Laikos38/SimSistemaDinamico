namespace SistemasDinamicos
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtParkingTime3 = new System.Windows.Forms.TextBox();
            this.txtParkingTime2 = new System.Windows.Forms.TextBox();
            this.txtParkingTime1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbParkedPlanes = new System.Windows.Forms.ComboBox();
            this.txtFirstPlaneArrival = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPorcAyDInstant = new System.Windows.Forms.TextBox();
            this.txtMaxTimeEEV = new System.Windows.Forms.TextBox();
            this.txtAvgTimeEEV = new System.Windows.Forms.TextBox();
            this.txtAvgTimeEET = new System.Windows.Forms.TextBox();
            this.txtMaxTimeEET = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvResults
            // 
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(6, 19);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(1321, 372);
            this.dgvResults.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnSimulate);
            this.groupBox1.Controls.Add(this.txtTo);
            this.groupBox1.Controls.Add(this.txtFrom);
            this.groupBox1.Controls.Add(this.txtQuantity);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1002, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 160);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parámetros de simulación";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(181, 126);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Limpiar";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSimulate
            // 
            this.btnSimulate.Location = new System.Drawing.Point(262, 126);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(75, 23);
            this.btnSimulate.TabIndex = 6;
            this.btnSimulate.Text = "Simular";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Click += new System.EventHandler(this.BtnSimulate_Click);
            // 
            // txtTo
            // 
            this.txtTo.Enabled = false;
            this.txtTo.Location = new System.Drawing.Point(129, 92);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(100, 20);
            this.txtTo.TabIndex = 5;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(129, 58);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(100, 20);
            this.txtFrom.TabIndex = 4;
            this.txtFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(129, 23);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(100, 20);
            this.txtQuantity.TabIndex = 3;
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Hasta:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Desde:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cantidad simulaciones:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtParkingTime3);
            this.groupBox2.Controls.Add(this.txtParkingTime2);
            this.groupBox2.Controls.Add(this.txtParkingTime1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.cmbParkedPlanes);
            this.groupBox2.Controls.Add(this.txtFirstPlaneArrival);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(561, 160);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parámetros dominio";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(372, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Aeronave 3:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(372, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Aeronave 2:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(372, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Aeronave 1:";
            // 
            // txtParkingTime3
            // 
            this.txtParkingTime3.Enabled = false;
            this.txtParkingTime3.Location = new System.Drawing.Point(448, 104);
            this.txtParkingTime3.Name = "txtParkingTime3";
            this.txtParkingTime3.Size = new System.Drawing.Size(76, 20);
            this.txtParkingTime3.TabIndex = 17;
            this.txtParkingTime3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // txtParkingTime2
            // 
            this.txtParkingTime2.Enabled = false;
            this.txtParkingTime2.Location = new System.Drawing.Point(448, 78);
            this.txtParkingTime2.Name = "txtParkingTime2";
            this.txtParkingTime2.Size = new System.Drawing.Size(76, 20);
            this.txtParkingTime2.TabIndex = 16;
            this.txtParkingTime2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // txtParkingTime1
            // 
            this.txtParkingTime1.Enabled = false;
            this.txtParkingTime1.Location = new System.Drawing.Point(448, 50);
            this.txtParkingTime1.Name = "txtParkingTime1";
            this.txtParkingTime1.Size = new System.Drawing.Size(76, 20);
            this.txtParkingTime1.TabIndex = 15;
            this.txtParkingTime1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(372, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Tiempos de permanencia:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(171, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Cantidad aeronaves estacionadas:";
            // 
            // cmbParkedPlanes
            // 
            this.cmbParkedPlanes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParkedPlanes.FormattingEnabled = true;
            this.cmbParkedPlanes.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cmbParkedPlanes.Location = new System.Drawing.Point(198, 89);
            this.cmbParkedPlanes.Name = "cmbParkedPlanes";
            this.cmbParkedPlanes.Size = new System.Drawing.Size(82, 21);
            this.cmbParkedPlanes.TabIndex = 12;
            this.cmbParkedPlanes.SelectedIndexChanged += new System.EventHandler(this.cmbParkedPlanes_SelectedIndexChanged);
            // 
            // txtFirstPlaneArrival
            // 
            this.txtFirstPlaneArrival.Location = new System.Drawing.Point(162, 38);
            this.txtFirstPlaneArrival.Name = "txtFirstPlaneArrival";
            this.txtFirstPlaneArrival.Size = new System.Drawing.Size(121, 20);
            this.txtFirstPlaneArrival.TabIndex = 9;
            this.txtFirstPlaneArrival.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AllowPositiveIntegerNumbers);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Llegada próxima aeronave:";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dgvResults);
            this.groupBox3.Location = new System.Drawing.Point(12, 178);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1333, 397);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Resultados";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtPorcAyDInstant);
            this.groupBox4.Controls.Add(this.txtMaxTimeEEV);
            this.groupBox4.Controls.Add(this.txtAvgTimeEEV);
            this.groupBox4.Controls.Add(this.txtAvgTimeEET);
            this.groupBox4.Controls.Add(this.txtMaxTimeEET);
            this.groupBox4.Location = new System.Drawing.Point(580, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(407, 160);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Estadísticas";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(94, 111);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(239, 13);
            this.label14.TabIndex = 9;
            this.label14.Text = "Porcentaje de aterrizajes y despegues inmediatos";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(213, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(182, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "Tiempo de espera promedio en vuelo";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 66);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(179, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Tiempo de espera promedio en tierra";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(213, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(174, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Tiempo de espera máximo en vuelo";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(171, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Tiempo de espera máximo en tierra";
            // 
            // txtPorcAyDInstant
            // 
            this.txtPorcAyDInstant.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtPorcAyDInstant.Enabled = false;
            this.txtPorcAyDInstant.Location = new System.Drawing.Point(120, 129);
            this.txtPorcAyDInstant.Name = "txtPorcAyDInstant";
            this.txtPorcAyDInstant.Size = new System.Drawing.Size(168, 20);
            this.txtPorcAyDInstant.TabIndex = 4;
            // 
            // txtMaxTimeEEV
            // 
            this.txtMaxTimeEEV.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtMaxTimeEEV.Enabled = false;
            this.txtMaxTimeEEV.Location = new System.Drawing.Point(216, 41);
            this.txtMaxTimeEEV.Name = "txtMaxTimeEEV";
            this.txtMaxTimeEEV.Size = new System.Drawing.Size(171, 20);
            this.txtMaxTimeEEV.TabIndex = 1;
            // 
            // txtAvgTimeEEV
            // 
            this.txtAvgTimeEEV.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtAvgTimeEEV.Enabled = false;
            this.txtAvgTimeEEV.Location = new System.Drawing.Point(216, 85);
            this.txtAvgTimeEEV.Name = "txtAvgTimeEEV";
            this.txtAvgTimeEEV.Size = new System.Drawing.Size(171, 20);
            this.txtAvgTimeEEV.TabIndex = 3;
            // 
            // txtAvgTimeEET
            // 
            this.txtAvgTimeEET.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtAvgTimeEET.Enabled = false;
            this.txtAvgTimeEET.Location = new System.Drawing.Point(19, 84);
            this.txtAvgTimeEET.Name = "txtAvgTimeEET";
            this.txtAvgTimeEET.Size = new System.Drawing.Size(168, 20);
            this.txtAvgTimeEET.TabIndex = 2;
            // 
            // txtMaxTimeEET
            // 
            this.txtMaxTimeEET.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtMaxTimeEET.Enabled = false;
            this.txtMaxTimeEET.Location = new System.Drawing.Point(19, 40);
            this.txtMaxTimeEET.Name = "txtMaxTimeEET";
            this.txtMaxTimeEET.Size = new System.Drawing.Size(168, 20);
            this.txtMaxTimeEET.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1357, 587);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(1364, 626);
            this.Name = "Form1";
            this.Text = "Alan Britos";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtParkingTime3;
        private System.Windows.Forms.TextBox txtParkingTime2;
        private System.Windows.Forms.TextBox txtParkingTime1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbParkedPlanes;
        private System.Windows.Forms.TextBox txtFirstPlaneArrival;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPorcAyDInstant;
        private System.Windows.Forms.TextBox txtMaxTimeEEV;
        private System.Windows.Forms.TextBox txtAvgTimeEEV;
        private System.Windows.Forms.TextBox txtAvgTimeEET;
        private System.Windows.Forms.TextBox txtMaxTimeEET;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
    }
}

