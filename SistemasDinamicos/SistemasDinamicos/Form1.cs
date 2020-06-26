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
                tiempoInesI = 320.7000;
            }
            else if (rndInesI < 0.5)
            {
                tiempoInesI = 367.5000;
            }
            else tiempoInesI = 417.2000;

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

            // Se imprimen por única vez las columnas fijas del vector de estado
            #region columnas fijas
            this.dgvResults.ColumnCount = 27;

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

            this.dgvResults.Columns[15].HeaderText = "RND";
            this.dgvResults.Columns[16].HeaderText = "T. sig. interrupción";
            this.dgvResults.Columns[17].HeaderText = "T. fin purga";
            this.dgvResults.Columns[15].DefaultCellStyle.BackColor = Color.LightGreen;
            this.dgvResults.Columns[16].DefaultCellStyle.BackColor = Color.LightGreen;
            this.dgvResults.Columns[17].DefaultCellStyle.BackColor = Color.LightGreen;
            this.dgvResults.Columns[15].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[16].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[17].SortMode = DataGridViewColumnSortMode.NotSortable;

            this.dgvResults.Columns[18].HeaderText = "Estado pista";
            this.dgvResults.Columns[19].HeaderText = "Cola EET";
            this.dgvResults.Columns[20].HeaderText = "Cola EEV";
            this.dgvResults.Columns[21].HeaderText = "T. restante suspensión";
            this.dgvResults.Columns[18].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[19].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[20].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[21].SortMode = DataGridViewColumnSortMode.NotSortable;


            this.dgvResults.Columns[22].HeaderText = "% aviones sin espera";
            this.dgvResults.Columns[23].HeaderText = "Máx. T. EET";
            this.dgvResults.Columns[24].HeaderText = "Prom. T. EET";
            this.dgvResults.Columns[25].HeaderText = "Máx. T. EEV";
            this.dgvResults.Columns[26].HeaderText = "Prom. T. EEV";
            this.dgvResults.Columns[22].DefaultCellStyle.BackColor = Color.MediumAquamarine;
            this.dgvResults.Columns[23].DefaultCellStyle.BackColor = Color.DarkSalmon;
            this.dgvResults.Columns[24].DefaultCellStyle.BackColor = Color.DarkSalmon;
            this.dgvResults.Columns[25].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            this.dgvResults.Columns[26].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            this.dgvResults.Columns[22].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[23].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[24].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[25].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvResults.Columns[26].SortMode = DataGridViewColumnSortMode.NotSortable;
            #endregion columnas fijas

            if (from == 0)
            {
                this.dgvResults.Rows.Add();
                this.dgvResults.Rows[0].Cells[0].Value = anterior.iterationNum;
                this.dgvResults.Rows[0].Cells[1].Value = anterior.evento;
                this.dgvResults.Rows[0].Cells[2].Value = truncar(anterior.reloj);
                this.dgvResults.Rows[0].Cells[3].Value = diferenteDeCero(anterior.rndLlegada);
                this.dgvResults.Rows[0].Cells[4].Value = diferenteDeCero(anterior.tiempoEntreLlegadas);
                this.dgvResults.Rows[0].Cells[5].Value = diferenteDeCero(anterior.tiempoProximaLlegada);
                this.dgvResults.Rows[0].Cells[6].Value = diferenteDeCero(anterior.rndAterrizaje);
                this.dgvResults.Rows[0].Cells[7].Value = diferenteDeCero(anterior.tiempoAterrizaje);
                this.dgvResults.Rows[0].Cells[8].Value = diferenteDeCero(anterior.tiempoFinAterrizaje);
                this.dgvResults.Rows[0].Cells[9].Value = diferenteDeCero(anterior.rndPermanencia);
                this.dgvResults.Rows[0].Cells[10].Value = diferenteDeCero(anterior.tiempoDePermanencia);
                this.dgvResults.Rows[0].Cells[11].Value = diferenteDeCero(anterior.tiempoFinPermanencia);
                this.dgvResults.Rows[0].Cells[12].Value = diferenteDeCero(anterior.rndDespegue);
                this.dgvResults.Rows[0].Cells[13].Value = diferenteDeCero(anterior.tiempoDeDespegue);
                this.dgvResults.Rows[0].Cells[14].Value = diferenteDeCero(anterior.tiempoFinDeDespegue);
                this.dgvResults.Rows[0].Cells[15].Value = diferenteDeCero(anterior.rndInestable);
                this.dgvResults.Rows[0].Cells[16].Value = diferenteDeCero(anterior.tiempoInestabilidad);
                this.dgvResults.Rows[0].Cells[17].Value = diferenteDeCero(anterior.tiempoFinPurga);
                this.dgvResults.Rows[0].Cells[18].Value = anterior.pista.state;
                this.dgvResults.Rows[0].Cells[19].Value = anterior.pista.colaEET.Count;
                this.dgvResults.Rows[0].Cells[20].Value = anterior.pista.colaEEV.Count;
                this.dgvResults.Rows[0].Cells[21].Value = diferenteDeCero(anterior.pista.tiempoRestanteAoD);
                this.dgvResults.Rows[0].Cells[22].Value = truncar(anterior.porcAvionesAyDInst);
                this.dgvResults.Rows[0].Cells[23].Value = truncar(anterior.maxEETTime);
                this.dgvResults.Rows[0].Cells[24].Value = truncar(anterior.avgEETTime);
                this.dgvResults.Rows[0].Cells[25].Value = truncar(anterior.maxEEVTime);
                this.dgvResults.Rows[0].Cells[26].Value = truncar(anterior.avgEEVTime);

                int cont = 0;
                for (int j = 0; j < simulator.clientes.Count; j++)
                {
                    if (simulator.clientes[j].disabled)
                    {
                        this.dgvResults.Rows[0].Cells[27 + cont].Value = simulator.clientes[j].estado;
                        this.dgvResults.Rows[0].Cells[27 + cont + 1].Value = diferenteDeCero(simulator.clientes[j].tiempoPermanencia);
                    }
                    else
                    {
                        this.dgvResults.ColumnCount += 2;

                        this.dgvResults.Columns[27 + cont].HeaderText = "Estado cliente " + simulator.clientes[j].id.ToString();
                        this.dgvResults.Columns[27 + cont + 1].HeaderText = "T. permanencia " + simulator.clientes[j].id.ToString();
                        this.dgvResults.Columns[27 + cont].SortMode = DataGridViewColumnSortMode.NotSortable;
                        this.dgvResults.Columns[27 + cont + 1].SortMode = DataGridViewColumnSortMode.NotSortable;

                        this.dgvResults.Rows[0].Cells[27 + cont].Value = simulator.clientes[j].estado;
                        this.dgvResults.Rows[0].Cells[27 + cont + 1].Value = diferenteDeCero(simulator.clientes[j].tiempoPermanencia);

                        simulator.clientes[j].disabled = true;
                    }
                    cont += 2;
                }
            }

            int filas = 0;
            if (from == 0)
                filas = 1;
            var columnaInicial = 0;
            for (int i = 0; i < quantity; i++)
            {
                StateRow actual = simulator.NextStateRow(anterior, i);

                if (i >= from-1 && i < to)
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
                    this.dgvResults.Rows[filas].Cells[15].Value = diferenteDeCero(actual.rndInestable);
                    this.dgvResults.Rows[filas].Cells[16].Value = diferenteDeCero(actual.tiempoInestabilidad);
                    this.dgvResults.Rows[filas].Cells[17].Value = diferenteDeCero(actual.tiempoFinPurga);
                    this.dgvResults.Rows[filas].Cells[18].Value = actual.pista.state;
                    this.dgvResults.Rows[filas].Cells[19].Value = actual.pista.colaEET.Count;
                    this.dgvResults.Rows[filas].Cells[20].Value = actual.pista.colaEEV.Count;
                    this.dgvResults.Rows[filas].Cells[21].Value = diferenteDeCero(actual.pista.tiempoRestanteAoD);
                    this.dgvResults.Rows[filas].Cells[22].Value = truncar(actual.porcAvionesAyDInst);
                    this.dgvResults.Rows[filas].Cells[23].Value = truncar(actual.maxEETTime);
                    this.dgvResults.Rows[filas].Cells[24].Value = truncar(actual.avgEETTime);
                    this.dgvResults.Rows[filas].Cells[25].Value = truncar(actual.maxEEVTime);
                    this.dgvResults.Rows[filas].Cells[26].Value = truncar(actual.avgEEVTime);
                    
                    if(i == from)
                    {
                        for (int j=0; j<simulator.clientes.Count; j++)
                        {
                            if (simulator.clientes[j].estado != "")
                            {
                                columnaInicial = j;
                                break;
                            }
                        }
                    }

                    int cont = 0;
                    for (int j = columnaInicial; j < simulator.clientes.Count; j++)
                    {
                        if (simulator.clientes[j].disabled)
                        {
                            this.dgvResults.Rows[filas].Cells[27 + cont].Value = simulator.clientes[j].estado;
                            this.dgvResults.Rows[filas].Cells[27 + cont + 1].Value = diferenteDeCero(simulator.clientes[j].tiempoPermanencia);
                        }
                        else
                        {
                            this.dgvResults.ColumnCount += 2;

                            this.dgvResults.Columns[27 + cont].HeaderText = "Estado cliente " + simulator.clientes[j].id.ToString();
                            this.dgvResults.Columns[27 + cont + 1].HeaderText = "T. permanencia " + simulator.clientes[j].id.ToString();
                            this.dgvResults.Columns[27 + cont].SortMode = DataGridViewColumnSortMode.NotSortable;
                            this.dgvResults.Columns[27 + cont + 1].SortMode = DataGridViewColumnSortMode.NotSortable;

                            this.dgvResults.Rows[filas].Cells[27 + cont].Value = simulator.clientes[j].estado;
                            this.dgvResults.Rows[filas].Cells[27 + cont + 1].Value = diferenteDeCero(simulator.clientes[j].tiempoPermanencia);

                            simulator.clientes[j].disabled = true;
                        }
                        cont += 2;
                    }

                    filas += 1;
                }

                // Muestro estadísticas
                if (i == quantity - 1)
                {
                    this.txtMaxTimeEET.Text = truncar(actual.maxEETTime).ToString();
                    this.txtMaxTimeEEV.Text = truncar(actual.maxEEVTime).ToString();
                    this.txtAvgTimeEET.Text = truncar(actual.avgEETTime).ToString();
                    this.txtAvgTimeEEV.Text = truncar(actual.avgEEVTime).ToString();
                    this.txtPorcAyDInstant.Text = truncar(actual.porcAvionesAyDInst).ToString();
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
