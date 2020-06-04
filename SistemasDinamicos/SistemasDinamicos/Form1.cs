using SimulacionMontecarlo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
                return;
            }

            int quantity = Convert.ToInt32(this.txtQuantity.Text);
            int from = Convert.ToInt32(this.txtFrom.Text);
            int to = from + 100;
            if (to > quantity)
                to = quantity;
            this.txtTo.Text = to.ToString();
            double proxAvion = Convert.ToDouble(this.txtFirstPlaneArrival.Text);

            StateRow initialize = new StateRow() {
                clientes = this.getAvionesEstacionados(),
                tiempoProximaLlegada = proxAvion,
                pista = new Pista() { libre = true, colaEET = new Queue<Avion>(), colaEEV = new Queue<Avion>() },
                evento = "Inicializacion",
                reloj = 0,
                iterationNum = 0
            };

            Simulator simulator = new Simulator();
            IList<StateRow> filasAMostrar = simulator.simulate(quantity, from, initialize);

            int clientCounter = 0;
            for (int i=0; i < filasAMostrar.Count; i++)
            {
                string estadoPista = filasAMostrar[i].pista.libre ? "Libre" : "Ocupada";

                // Manejo de columnas
                if (i==0)
                {
                    this.dgvResults.ColumnCount = 18;

                    this.dgvResults.Columns[0].HeaderText = "n°";
                    this.dgvResults.Columns[1].HeaderText = "Evento";
                    this.dgvResults.Columns[2].HeaderText = "Reloj";
                    this.dgvResults.Columns[3].HeaderText = "RND";
                    this.dgvResults.Columns[4].HeaderText = "T. entre llegadas";
                    this.dgvResults.Columns[5].HeaderText = "T. prox. llegada";
                    this.dgvResults.Columns[6].HeaderText = "RND";
                    this.dgvResults.Columns[7].HeaderText = "T. aterrizaje";
                    this.dgvResults.Columns[8].HeaderText = "T. fin aterrizaje";
                    this.dgvResults.Columns[9].HeaderText = "SUM RND";
                    this.dgvResults.Columns[10].HeaderText = "T. permanencia";
                    this.dgvResults.Columns[11].HeaderText = "T. fin permanencia";
                    this.dgvResults.Columns[12].HeaderText = "RND";
                    this.dgvResults.Columns[13].HeaderText = "T. despegue";
                    this.dgvResults.Columns[14].HeaderText = "T. fin despegue";
                    this.dgvResults.Columns[15].HeaderText = "Estado pista";
                    this.dgvResults.Columns[16].HeaderText = "Cola EET";
                    this.dgvResults.Columns[17].HeaderText = "Cola EEV";
                }
                
                // Manejo de filas
                //object[] dataFila = new object[] {
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
                    filasAMostrar[i].pista.colaEETnum,
                    filasAMostrar[i].pista.colaEEVnum
                };

                for(int j = 0; j < filasAMostrar[i].clientes.Count; j++)
                {
                    if ((this.dgvResults.Columns.Count - 18 + clientCounter) != filasAMostrar[i].clientes.Count * 2)
                    {
                        this.dgvResults.ColumnCount += 1;
                        this.dgvResults.Columns[this.dgvResults.ColumnCount - 1].HeaderText = "Estado cliente " + filasAMostrar[i].clientes[j].id.ToString();
                        this.dgvResults.ColumnCount += 1;
                        this.dgvResults.Columns[this.dgvResults.ColumnCount - 1].HeaderText = "T. permanencia " + filasAMostrar[i].clientes[j].id.ToString();
                        clientCounter += 2;
                    }

                    dataFila.Add(filasAMostrar[i].clientes[j].estado);
                    dataFila.Add(filasAMostrar[i].clientes[j].tiempoPermanencia);


                }

                this.dgvResults.Rows.Add(dataFila.ToArray());
            }
        }

        private object diferenteDeCero(double value)
        {
            if (value != 0)
                return (Math.Truncate(value * 10000) / 10000);
            else
                return "";
        }

        private bool validateInputs()
        {
            return true;
        }

        private void cmbParkedPlanes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbParkedPlanes.SelectedIndex == 0)
            {
                this.txtParkingTime1.Enabled = false;
                this.txtParkingTime2.Enabled = false;
                this.txtParkingTime3.Enabled = false;
            }
            if (this.cmbParkedPlanes.SelectedIndex == 1)
            {
                this.txtParkingTime1.Enabled = true;
                this.txtParkingTime2.Enabled = false;
                this.txtParkingTime3.Enabled = false;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 2)
            {
                this.txtParkingTime1.Enabled = true;
                this.txtParkingTime2.Enabled = true;
                this.txtParkingTime3.Enabled = false;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 3)
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
                return result;
            }
            if (this.cmbParkedPlanes.SelectedIndex == 1)
            {
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP" });
                return result;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 2)
            {
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP" });
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP" });
                return result;
            }
            else if (this.cmbParkedPlanes.SelectedIndex == 3)
            {
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP" });
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP" });
                result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime3.Text), estado = "EP" });
                return result;
            }
            return result;
        }
    }
}
