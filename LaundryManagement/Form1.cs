using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Forms;

namespace LaundryManagement
{
    public partial class Form1 : Form
    {
        private Dictionary<string, System.Windows.Forms.Timer> washerTimers; // Temporizadores para cada lavadora
        private Dictionary<string, System.Windows.Forms.Timer> dryerTimers; // Temporizadores para cada secadora
        private Dictionary<string, DateTime> startWashTimes; // Tiempos de inicio para cada lavadora
        private Dictionary<string, DateTime> startDryTimes; // Tiempos de inicio para cada secadora
        private Dictionary<string, string> washerDepartments; // Departamentos asignados a cada lavadora
        private Dictionary<string, string> dryerDepartments; // Departamentos asignados a cada secadora
        private string selectedWasher; // Lavadora seleccionada
        private string selectedDryer; // Secadora seleccionada
        private List<string> history;
        private Dictionary<string, bool> washerStatus; // Estado de cada lavadora (disponible u ocupada)
        private Dictionary<string, bool> dryerStatus; // Estado de cada secadora (disponible u ocupada)

        public Form1()
        {
            InitializeComponent();

            // Inicializar el estado de las lavadoras
            washerStatus = new Dictionary<string, bool>
            {
                { "Lavadora 1", true },
                { "Lavadora 2", true },
                { "Lavadora 3", true }
            };

            // Inicializar el estado de las secadoras
            dryerStatus = new Dictionary<string, bool>
            {
                { "Secadora 1", true },
                { "Secadora 2", true },
                { "Secadora 3", true }
            };

            // Llenar los ComboBox con las lavadoras y secadoras
            comboBox2.Items.Add("Lavadora 1");
            comboBox2.Items.Add("Lavadora 2");
            comboBox2.Items.Add("Lavadora 3");

            comboBox1.Items.Add("Secadora 1");
            comboBox1.Items.Add("Secadora 2");
            comboBox1.Items.Add("Secadora 3");

            // Inicializar los temporizadores y los tiempos de inicio para cada lavadora y secadora
            washerTimers = new Dictionary<string, System.Windows.Forms.Timer>();
            dryerTimers = new Dictionary<string, System.Windows.Forms.Timer>();
            startWashTimes = new Dictionary<string, DateTime>();
            startDryTimes = new Dictionary<string, DateTime>();
            washerDepartments = new Dictionary<string, string>();
            dryerDepartments = new Dictionary<string, string>();

            // Crear temporizadores para cada lavadora y secadora
            CreateWasherTimers();
            CreateDryerTimers();

            // Inicializar el historial
            history = new List<string>();

            // Inicializar los ListBox para mostrar el historial
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }

        // Crear y configurar los temporizadores para cada lavadora
        private void CreateWasherTimers()
        {
            foreach (var washer in washerStatus.Keys)
            {
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000; // 1 segundo
                timer.Tick += (sender, e) => WasherTimer_Tick(washer);
                washerTimers[washer] = timer;
            }
        }

        // Crear y configurar los temporizadores para cada secadora
        private void CreateDryerTimers()
        {
            foreach (var dryer in dryerStatus.Keys)
            {
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000; // 1 segundo
                timer.Tick += (sender, e) => DryerTimer_Tick(dryer);
                dryerTimers[dryer] = timer;
            }
        }

        // Evento del timer para el ciclo de lavado
        private void WasherTimer_Tick(string washer)
        {
            int washDuration = 1; // Duración en minutos

            if (startWashTimes.ContainsKey(washer) && (DateTime.Now - startWashTimes[washer]).TotalMinutes >= washDuration)
            {
                washerTimers[washer].Stop();
                DateTime endWashTime = DateTime.Now;
                string departmentForWasher = washerDepartments[washer];

                // Reproducir el sonido de alerta
                System.Media.SystemSounds.Exclamation.Play();

                string finishedMessage = $"El departamento {departmentForWasher} ha terminado el ciclo de lavado en {washer}.\n" +
                                         $"Inicio: {startWashTimes[washer]:HH:mm:ss} - Fin: {endWashTime:HH:mm:ss}";
                MessageBox.Show(finishedMessage);

                history.Add(finishedMessage);
                listBox1.Items.Add(finishedMessage);
                washerStatus[washer] = true;
                UpdateWasherComboBox();
            }
        }

        // Evento del timer para el ciclo de secado
        private void DryerTimer_Tick(string dryer)
        {
            int dryDuration = 1; // Duración en minutos

            if (startDryTimes.ContainsKey(dryer) && (DateTime.Now - startDryTimes[dryer]).TotalMinutes >= dryDuration)
            {
                dryerTimers[dryer].Stop();
                DateTime endDryTime = DateTime.Now;
                string departmentForDryer = dryerDepartments[dryer];

                // Reproducir el sonido de alerta
                System.Media.SystemSounds.Exclamation.Play();

                string finishedMessage = $"El departamento {departmentForDryer} ha terminado el ciclo de secado en {dryer}.\n" +
                                         $"Inicio: {startDryTimes[dryer]:HH:mm:ss} - Fin: {endDryTime:HH:mm:ss}";
                MessageBox.Show(finishedMessage);

                history.Add(finishedMessage);
                listBox2.Items.Add(finishedMessage);
                dryerStatus[dryer] = true;
                UpdateDryerComboBox();
            }
        }

        // Iniciar ciclo de lavado
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Por favor ingresa un departamento y selecciona una lavadora.");
                return;
            }

            string selectedDepartment = textBox1.Text.Trim();
            selectedWasher = comboBox2.SelectedItem.ToString();

            if (!washerStatus[selectedWasher])
            {
                MessageBox.Show($"La {selectedWasher} está en uso. Por favor, elige otra lavadora.");
                return;
            }

            washerDepartments[selectedWasher] = selectedDepartment;
            startWashTimes[selectedWasher] = DateTime.Now;
            washerTimers[selectedWasher].Start();
            string startMessage = $"El departamento {selectedDepartment} ha iniciado el ciclo de lavado en {selectedWasher} a las {startWashTimes[selectedWasher]:HH:mm:ss}.";
            MessageBox.Show(startMessage);

            history.Add(startMessage);
            listBox1.Items.Add(startMessage);
            washerStatus[selectedWasher] = false;
            UpdateWasherComboBox();
        }

        // Iniciar ciclo de secado
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Por favor ingresa un departamento y selecciona una secadora.");
                return;
            }

            string selectedDepartment = textBox2.Text.Trim();
            selectedDryer = comboBox1.SelectedItem.ToString();

            if (!dryerStatus[selectedDryer])
            {
                MessageBox.Show($"La {selectedDryer} está en uso. Por favor, elige otra secadora.");
                return;
            }

            dryerDepartments[selectedDryer] = selectedDepartment;
            startDryTimes[selectedDryer] = DateTime.Now;
            dryerTimers[selectedDryer].Start();
            string startMessage = $"El departamento {selectedDepartment} ha iniciado el ciclo de secado en {selectedDryer} a las {startDryTimes[selectedDryer]:HH:mm:ss}.";
            MessageBox.Show(startMessage);

            history.Add(startMessage);
            listBox2.Items.Add(startMessage);
            dryerStatus[selectedDryer] = false;
            UpdateDryerComboBox();
        }

        // Actualizar el ComboBox de lavadoras
        private void UpdateWasherComboBox()
        {
            comboBox2.Items.Clear();
            foreach (var washer in washerStatus)
            {
                if (washer.Value)
                {
                    comboBox2.Items.Add(washer.Key);
                }
            }
        }

        // Actualizar el ComboBox de secadoras
        private void UpdateDryerComboBox()
        {
            comboBox1.Items.Clear();
            foreach (var dryer in dryerStatus)
            {
                if (dryer.Value)
                {
                    comboBox1.Items.Add(dryer.Key);
                }
            }
        }

        // Eliminar historial de lavado
        private void button2_Click(object sender, EventArgs e)
        {
            history.Clear();
            listBox1.Items.Clear();
            MessageBox.Show("Historial de lavado eliminado.");
        }

        // Eliminar historial de secado
        private void button4_Click(object sender, EventArgs e)
        {
            history.Clear();
            listBox2.Items.Clear();
            MessageBox.Show("Historial de secado eliminado.");
        }

        // Métodos adicionales del formulario (mantener vacíos si no se usan)
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
        private void timer2_Tick(object sender, EventArgs e) { }
        private void timer3_Tick(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Código para ejecutar al cargar el formulario
        }
    }
}
