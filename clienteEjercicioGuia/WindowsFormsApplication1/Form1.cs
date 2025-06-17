using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        string usuarioActual = "";
        bool desconectado = false;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; //Necesario para que los elementos de los formularios puedan ser
            //accedidos desde threads diferentes a los que los crearon
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }


        private void AtenderServidor()
        {
            try
            {
                while (true)
                {
                    //Recibimos mensaje del servidor
                    byte[] msg2 = new byte[256];
                    int bytesRecibidos = server.Receive(msg2);

                    if (bytesRecibidos == 0)
                    {
                        Desconectar();
                        break;
                    }

                    string[] trozos = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('/');

                    if (trozos.Length < 2)
                    {
                        Console.WriteLine("Mensaje recibido mal formado.");
                        continue;
                    }

                    int codigo = 0;

                    try
                    {
                        codigo = Convert.ToInt32(trozos[0]);
                    }
                    catch
                    {
                        Console.WriteLine("Código no válido. Se ignora.");
                        continue;
                    }

                    string mensaje = trozos[1].Split('\0')[0].Trim();

                    if (usuarioActual == "" && codigo != 1 && codigo != 6)
                        continue;

                    switch (codigo)
                    {
                        case 1:  // respuesta a longitud
                            if (mensaje == "1")
                            {
                                //MessageBox.Show("Bienvenido");
                                chatBox.AppendText("Bienvenido, " + usuarioActual + Environment.NewLine);
                                usuarioActual = usertextBox.Text;
                            }
                            else
                                MessageBox.Show("No hemos encontrado su usuario o ha fallado la contraseña.");
                            break;
                        case 5:
                            //Mensaje de chat recibido
                            chatBox.AppendText(mensaje + Environment.NewLine);
                            break;
                        case 6:
                            Console.WriteLine("Mensaje recibido para registro: '" + mensaje + "'");
                            if (mensaje == "1")
                                MessageBox.Show("Se ha registrado correctamente");
                            else
                                MessageBox.Show("Usuario ya registrado anteriormente");
                            break;
                        case 7:
                            if (mensaje == "1")
                            {
                                MessageBox.Show("Usuario eliminado correctamente.");
                                usuarioActual = "";
                                this.BackColor = Color.Gray;
                                usertextBox.Clear();
                                passwordtextBox.Clear();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo eliminar el usuario (usuario incorrecto o inexistente).");
                            }
                            break;
                        /*case 8:
                            if (string.IsNullOrEmpty(usuarioActual))
                            {
                                MessageBox.Show("Debes iniciar sesión para ver los usuarios conectados.");
                                break;
                            }

                            conectadosBox.Clear();

                            string[] usuarios = mensaje.Split(',');
                            foreach (string usuario in usuarios)
                            {
                                conectadosBox.AppendText(usuario + Environment.NewLine);
                            }
                            break;*/
                        case 9:
                            if (mensaje == "1")
                            {
                                MessageBox.Show("Perfil modificado correctamente.");
                                usuarioActual = nametextBox.Text;
                            }
                            else
                            {
                                MessageBox.Show("Error al modificar el perfil. Asegúrate de que el nuevo nombre no esté en uso.");
                            }
                            break;
                        default:
                            Console.WriteLine("Código no reconocido: " + codigo);
                            break;
                    }
                }
            }
            catch (SocketException se)
            {
                Desconectar();
            }
            catch (Exception ex)
            {
                Desconectar();
            }
        }





        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9017);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (server != null && server.Connected)
            {
                MessageBox.Show("Ya estás conectado al servidor.");
                return;
            }

            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

                // Reiniciar el estado de desconexión
                desconectado = false;

                //pongo en marcha el thread que atenderá los mensajes del servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();
                button1.Enabled = false;
                button3.Enabled = true;

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Desconectar();
        }

        private void Desconectar()
        {
            if (desconectado)
                return;

            desconectado = true;

            try
            {
                if (server != null && server.Connected)
                {
                    string mensaje = "0/";
                    byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }

                chatBox.AppendText("Te has desconectado del servidor." + Environment.NewLine);
                usuarioActual = "";
                this.BackColor = Color.Gray;

                if (atender != null && atender.IsAlive)
                    atender.Abort(); // Considerar usar cancelación más limpia si lo deseas

                if (server != null)
                {
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                    button1.Enabled = true;
                    button3.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar conexión: " + ex.Message);
            }

        }

        private void sendChatButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor.");
                return;
            }

            if (string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("Debes iniciar sesión antes de enviar mensajes.");
                return;
            }

            if (string.IsNullOrWhiteSpace(chatInputTextBox.Text))
            {
                MessageBox.Show("No puedes enviar un mensaje vacío.");
                return;
            }

            string mensaje = "5/" + usuarioActual + "/" + chatInputTextBox.Text;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            chatInputTextBox.Clear();
            usertextBox.Clear();
            passwordtextBox.Clear();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor.");
                return;
            }

            if (!string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("Ya has iniciado sesión como '" + usuarioActual + "'. Por favor, desconéctate primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(usertextBox.Text) || string.IsNullOrWhiteSpace(passwordtextBox.Text))
            {
                MessageBox.Show("Introduce el nombre de usuario y la contraseña.");
                return;
            }

            string mensaje = "1/" + usertextBox.Text + "/" + passwordtextBox.Text;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor.");
                return;
            }


            if (!string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("Ya has iniciado sesión como '" + usuarioActual + "'. No puedes registrarte estando conectado.");
                return;
            }

            if (string.IsNullOrWhiteSpace(usertextBox.Text) || string.IsNullOrWhiteSpace(passwordtextBox.Text))
            {
                MessageBox.Show("Introduce el nombre de usuario y la contraseña.");
                return;
            }

            string mensaje = "6/" + usertextBox.Text + "/" + passwordtextBox.Text;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void eliminarButton_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No estás conectado al servidor.");
                return;
            }

            if (string.IsNullOrWhiteSpace(usertextBox.Text) || string.IsNullOrWhiteSpace(passwordtextBox.Text))
            {
                MessageBox.Show("Indica usuario y contraseña.");
                return;
            }

            if (string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("Debes iniciar sesión para poder eliminar tu usuario.");
                return;
            }

            if (usertextBox.Text.Trim() != usuarioActual)
            {
                MessageBox.Show("Solo puedes eliminar el usuario con el que estás conectado.");
                return;
            }

            string mensaje = "7/" + usertextBox.Text + "/" + passwordtextBox.Text;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void ModificarPerfilButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("Debes iniciar sesión primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nametextBox.Text))
            {
                MessageBox.Show("Debes introducir un nuevo nombre.");
                return;
            }

            string nuevoNombre = nametextBox.Text.Trim();
            string mensaje = "9/" + usuarioActual + "/" + nuevoNombre;
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void cambiarFondoButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Selecciona una imagen de fondo";
            openFileDialog.Filter = "Archivos JPG (*.jpg)|*.jpg";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string rutaImagen = openFileDialog.FileName;
                    this.BackgroundImage = Image.FromFile(rutaImagen);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
        }
    }
}
