﻿using SimulacionMontecarlo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemasDinamicos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.cmbParkedPlanes.SelectedIndex = 0;
            this.txtTo.Enabled = false;
        }

        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            if (!validateInputs())
            {
                MessageBox.Show("Debe completar todos los campos antes de continuar", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.dgvResults.Rows.Clear();
            Avion.count = 0;

            int quantity = Convert.ToInt32(this.txtQuantity.Text);
            int from = Convert.ToInt32(this.txtFrom.Text);
            int to = from + 100;
            if (to > quantity)
                to = quantity;
            this.txtTo.Text = to.ToString();
            double proxAvion = Convert.ToDouble(this.txtFirstPlaneArrival.Text);

            StateRow initialize = new StateRow()
            {
                clientes = this.getAvionesEstacionados(),
                tiempoProximaLlegada = proxAvion,
                pista = new Pista() { libre = true, colaEET = new Queue<Avion>(), colaEEV = new Queue<Avion>() },
                evento = "Inicializacion",
                reloj = 0,
                iterationNum = 0
            };

            Simulator simulator = new Simulator();
            IList<StateRow> filasAMostrar = simulator.simulate(quantity, from, initialize);

            this.txtMaxTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEETTime).ToString();
            this.txtMaxTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEEVTime).ToString();
            this.txtAvgTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEETTime).ToString();
            this.txtAvgTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEEVTime).ToString();
            this.txtPorcAyDInstant.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].porcAvionesAyDInst).ToString();

            if (to != filasAMostrar.Last().iterationNum)
                filasAMostrar.Remove(filasAMostrar.Last());

            int columnaInicial = 0;
            int columnaFinal = 0;
            for (int a = 0; a < filasAMostrar[0].clientes.Count; a++)
            {
                if (!filasAMostrar[0].clientes[a].disabled)
                {
                    columnaInicial = filasAMostrar[0].clientes[a].id;
                    break;
                    //Console.WriteLine(columnaInicial);
                }
            }
            //columnaFinal = filasAMostrar.Last().clientes.Last().id;
            columnaFinal = filasAMostrar.Last().clientes.Count;

            for (int i = 0; i < filasAMostrar.Count; i++)
            {
                string estadoPista = filasAMostrar[i].pista.libre ? "Libre" : "Ocupada";

                // Manejo de columnas
                if (i == 0)
                {
                    this.dgvResults.ColumnCount = 23;

                    this.dgvResults.Columns[0].HeaderText = "n°";
                    this.dgvResults.Columns[1].HeaderText = "Evento";
                    this.dgvResults.Columns[2].HeaderText = "Reloj";
                    this.dgvResults.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvResults.Columns[3].HeaderText = "RND";
                    this.dgvResults.Columns[4].HeaderText = "T. entre llegadas";
                    this.dgvResults.Columns[5].HeaderText = "T. prox. llegada";
                    this.dgvResults.Columns[3].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvResults.Columns[4].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvResults.Columns[5].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvResults.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;


                    this.dgvResults.Columns[6].HeaderText = "RND";
                    this.dgvResults.Columns[7].HeaderText = "T. aterrizaje";
                    this.dgvResults.Columns[8].HeaderText = "T. fin aterrizaje";
                    this.dgvResults.Columns[6].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvResults.Columns[7].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvResults.Columns[8].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvResults.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;


                    this.dgvResults.Columns[9].HeaderText = "SUM RND";
                    this.dgvResults.Columns[10].HeaderText = "T. permanencia";
                    this.dgvResults.Columns[11].HeaderText = "T. fin permanencia";
                    this.dgvResults.Columns[9].DefaultCellStyle.BackColor = Color.Turquoise;
                    this.dgvResults.Columns[10].DefaultCellStyle.BackColor = Color.Turquoise;
                    this.dgvResults.Columns[11].DefaultCellStyle.BackColor = Color.Turquoise;
                    this.dgvResults.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[10].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[11].SortMode = DataGridViewColumnSortMode.NotSortable;


                    this.dgvResults.Columns[12].HeaderText = "RND";
                    this.dgvResults.Columns[13].HeaderText = "T. despegue";
                    this.dgvResults.Columns[14].HeaderText = "T. fin despegue";
                    this.dgvResults.Columns[12].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvResults.Columns[13].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvResults.Columns[14].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvResults.Columns[12].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[13].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[14].SortMode = DataGridViewColumnSortMode.NotSortable;


                    this.dgvResults.Columns[15].HeaderText = "Estado pista";
                    this.dgvResults.Columns[16].HeaderText = "Cola EET";
                    this.dgvResults.Columns[17].HeaderText = "Cola EEV";
                    this.dgvResults.Columns[15].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[16].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[17].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvResults.Columns[18].HeaderText = "% aviones sin espera";
                    this.dgvResults.Columns[19].HeaderText = "Máx. T. EET";
                    this.dgvResults.Columns[20].HeaderText = "Prom. T. EET";
                    this.dgvResults.Columns[21].HeaderText = "Máx. T. EEV";
                    this.dgvResults.Columns[22].HeaderText = "Prom. T. EEV";
                    this.dgvResults.Columns[18].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvResults.Columns[19].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvResults.Columns[20].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvResults.Columns[21].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvResults.Columns[22].DefaultCellStyle.BackColor = Color.LightSteelBlue;

                    this.dgvResults.Columns[18].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[19].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[20].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[21].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvResults.Columns[22].SortMode = DataGridViewColumnSortMode.NotSortable;

                    //this.dgvResults.ColumnCount += ((columnaFinal - columnaInicial) * 2) + 2;
                    this.dgvResults.ColumnCount = ((columnaFinal - columnaInicial) * 2) + 2;

                    int count = columnaInicial;
                    for (int j = 23; j <= ((columnaFinal - columnaInicial) * 2) + 23; j += 2)
                    {
                        this.dgvResults.Columns[j].HeaderText = "Estado cliente " + count.ToString();
                        this.dgvResults.Columns[j + 1].HeaderText = "T. permanencia " + count.ToString();
                        this.dgvResults.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
                        this.dgvResults.Columns[j+1].SortMode = DataGridViewColumnSortMode.NotSortable;
                        count++;
                    }
                }

                // Manejo de filas
                List<object> dataFila = new List<object>() {
                    diferenteDeCero(filasAMostrar[i].iterationNum),
                    filasAMostrar[i].evento,
                    diferenteDeCero(filasAMostrar[i].reloj),
                    diferenteDeCero(filasAMostrar[i].rndLlegada),
                    diferenteDeCero(filasAMostrar[i].tiempoEntreLlegadas),
                    diferenteDeCero(filasAMostrar[i].tiempoProximaLlegada),
                    diferenteDeCero(filasAMostrar[i].rndAterrizaje),
                    diferenteDeCero(filasAMostrar[i].tiempoAterrizaje),
                    diferenteDeCero(filasAMostrar[i].tiempoFinAterrizaje),
                    diferenteDeCero(filasAMostrar[i].rndPermanencia),
                    diferenteDeCero(filasAMostrar[i].tiempoDePermanencia),
                    diferenteDeCero(filasAMostrar[i].tiempoFinPermanencia),
                    diferenteDeCero(filasAMostrar[i].rndDespegue),
                    diferenteDeCero(filasAMostrar[i].tiempoDeDespegue),
                    diferenteDeCero(filasAMostrar[i].tiempoFinDeDespegue),
                    estadoPista,
                    truncar(filasAMostrar[i].pista.colaEETnum),
                    truncar(filasAMostrar[i].pista.colaEEVnum),
                    truncar(filasAMostrar[i].porcAvionesAyDInst),
                    truncar(filasAMostrar[i].maxEETTime),
                    truncar(filasAMostrar[i].avgEETTime),
                    truncar(filasAMostrar[i].maxEEVTime),
                    truncar(filasAMostrar[i].avgEEVTime)
                };

                //for (int k = columnaInicial - 1; k < (filasAMostrar[i].clientes.Last().id); k++)
                for (int k = columnaInicial - 1; k < (filasAMostrar[i].clientes.Count); k++)
                {
                    dataFila.Add(filasAMostrar[i].clientes[k].estado);
                    dataFila.Add(diferenteDeCero(filasAMostrar[i].clientes[k].tiempoPermanencia));
                }

                this.dgvResults.Rows.Add(dataFila.ToArray());
            }

            this.dgvResults.AllowUserToOrderColumns = false;
        }

        private object diferenteDeCero(double value)
        {
            if (value != 0)
                return (Math.Truncate(value * 10000) / 10000);
            else
                return "";
        }

        private double truncar(double value)
        {
            return (Math.Truncate(value * 10000) / 10000);
        }

        private bool validateInputs()
        {
            if (String.IsNullOrEmpty(this.txtFirstPlaneArrival.Text) || String.IsNullOrEmpty(this.txtFrom.Text) || String.IsNullOrEmpty(this.txtQuantity.Text))
            {
                return false;
            }

            if ((Convert.ToInt32(this.txtFirstPlaneArrival.Text) <= 0) || (Convert.ToInt32(this.txtQuantity.Text) <= 0) || (Convert.ToInt32(this.txtFrom.Text) < 0))
                return false;

            switch (this.cmbParkedPlanes.SelectedIndex)
            {
                case 0:
                    if (String.IsNullOrEmpty(this.txtParkingTime1.Text) || this.txtParkingTime1.Text == "0") return false;
                    break;
                case 1:
                    if (String.IsNullOrEmpty(this.txtParkingTime1.Text) || String.IsNullOrEmpty(this.txtParkingTime2.Text) || this.txtParkingTime1.Text == "0" || this.txtParkingTime2.Text == "0") return false;

                    break;
                case 2:
                    if (String.IsNullOrEmpty(this.txtParkingTime1.Text) || String.IsNullOrEmpty(this.txtParkingTime2.Text) || String.IsNullOrEmpty(this.txtParkingTime3.Text) || this.txtParkingTime1.Text == "0" || this.txtParkingTime2.Text == "0" || this.txtParkingTime3.Text == "0") return false;
                    break;
            }

            return true;
        }

        private void cmbParkedPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbParkedPlanes.SelectedIndex == 0)
            {
                this.txtParkingTime1.Enabled = true;
                this.txtParkingTime2.Enabled = false;
                this.txtParkingTime3.Enabled = false;
                this.txtParkingTime2.Text = "";
                this.txtParkingTime3.Text = "";
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 1)
            {
                this.txtParkingTime1.Enabled = true;
                this.txtParkingTime2.Enabled = true;
                this.txtParkingTime3.Enabled = false;
                this.txtParkingTime3.Text = "";
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 2)
            {
                this.txtParkingTime1.Enabled = true;
                this.txtParkingTime2.Enabled = true;
                this.txtParkingTime3.Enabled = true;
            }
        }

        private List<Avion> getAvionesEstacionados()
        {
            List<Avion> result = new List<Avion>();

            if (this.cmbParkedPlanes.SelectedIndex == 0)
            {
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
                return result;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 1)
            {
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP", disabled = false });
                return result;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 2)
            {
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP", disabled = false });
                Avion.count += 1;
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime3.Text), estado = "EP", disabled = false });
                return result;
            }
            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.SetProperty, null,
            dgvResults, new object[] { true });
            this.ResumeLayout();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.dgvResults.Rows.Clear();
            this.txtFirstPlaneArrival.Text = "";
            this.txtFrom.Text = "";
            this.txtParkingTime1.Text = "";
            this.txtParkingTime2.Text = "";
            this.txtParkingTime3.Text = "";
            this.txtQuantity.Text = "";
            this.txtTo.Text = "";
        }

        private void AllowPositiveIntegerNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void AllowPositiveDecimalNumbers(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }

        }
    }
}
