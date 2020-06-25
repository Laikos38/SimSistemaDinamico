using SimulacionMontecarlo;
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
using GeneradorDeNumerosAleatorios;

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

            Simulator simulator = new Simulator();
            simulator.clientes = this.getAvionesEstacionados();

            Generator generator = new Generator();

            double rndInesI = generator.NextRnd();
            double tiempoInesI;
            if (rndInesI < 0.2)
            {
                tiempoInesI = 320.7;
            }
            else if (rndInesI < 0.5)
            {
                tiempoInesI = 367.5;
            }
            else tiempoInesI = 417.2;

            StateRow anterior = new StateRow()
            {
                tiempoProximaLlegada = proxAvion,
                pista = new Pista() { state = "Libre", colaEET = new Queue<Avion>(), colaEEV = new Queue<Avion>() },
                evento = "Inicializacion",
                reloj = 0,
                iterationNum = 0,
                rndInestable = rndInesI,
                tiempoInestabilidad = tiempoInesI
            };


            /*
            this.txtMaxTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEETTime).ToString();
            this.txtMaxTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEEVTime).ToString();
            this.txtAvgTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEETTime).ToString();
            this.txtAvgTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEEVTime).ToString();
            this.txtPorcAyDInstant.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].porcAvionesAyDInst).ToString();
            */

            int filas = 0;
            for (int i = 0; i < quantity; i++)
            {
                StateRow actual = simulator.NextStateRow(anterior, i);

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
                }

                if (i >= from && i< to)
                {
                    this.dgvResults.Rows.Add();
                    this.dgvResults.Rows[filas].Cells[0].Value = actual.iterationNum;
                    this.dgvResults.Rows[filas].Cells[1].Value = actual.evento;
                    this.dgvResults.Rows[filas].Cells[2].Value = truncar(actual.reloj);
                    this.dgvResults.Rows[filas].Cells[3].Value = diferenteDeCero(actual.rndLlegada);
                    this.dgvResults.Rows[filas].Cells[4].Value = diferenteDeCero(actual.tiempoEntreLlegadas);
                    this.dgvResults.Rows[filas].Cells[5].Value = diferenteDeCero(actual.tiempoProximaLlegada);
                    this.dgvResults.Rows[filas].Cells[6].Value = diferenteDeCero(actual.rndAterrizaje);
                    this.dgvResults.Rows[filas].Cells[7].Value = diferenteDeCero(actual.tiempoAterrizaje);
                    this.dgvResults.Rows[filas].Cells[8].Value = diferenteDeCero(actual.tiempoFinAterrizaje);
                    this.dgvResults.Rows[filas].Cells[9].Value = diferenteDeCero(actual.rndPermanencia);
                    this.dgvResults.Rows[filas].Cells[10].Value = diferenteDeCero(actual.tiempoDePermanencia);
                    this.dgvResults.Rows[filas].Cells[11].Value = diferenteDeCero(actual.tiempoFinPermanencia);
                    this.dgvResults.Rows[filas].Cells[12].Value = diferenteDeCero(actual.rndDespegue);
                    this.dgvResults.Rows[filas].Cells[13].Value = diferenteDeCero(actual.tiempoDeDespegue);
                    this.dgvResults.Rows[filas].Cells[14].Value = diferenteDeCero(actual.tiempoFinDeDespegue);
                    this.dgvResults.Rows[filas].Cells[15].Value = actual.pista.state;
                    this.dgvResults.Rows[filas].Cells[16].Value = actual.pista.colaEET.Count;
                    this.dgvResults.Rows[filas].Cells[17].Value = actual.pista.colaEEV.Count;
                    this.dgvResults.Rows[filas].Cells[18].Value = truncar(actual.porcAvionesAyDInst);
                    this.dgvResults.Rows[filas].Cells[19].Value = truncar(actual.maxEETTime);
                    this.dgvResults.Rows[filas].Cells[20].Value = truncar(actual.avgEETTime);
                    this.dgvResults.Rows[filas].Cells[21].Value = truncar(actual.maxEEVTime);
                    this.dgvResults.Rows[filas].Cells[22].Value = truncar(actual.avgEEVTime);
                    filas += 1;
                }

                anterior = actual;

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
