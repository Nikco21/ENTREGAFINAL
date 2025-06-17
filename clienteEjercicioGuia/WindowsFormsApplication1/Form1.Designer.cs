namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
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
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.RichTextBox();
            this.chatInputTextBox = new System.Windows.Forms.TextBox();
            this.sendChatButton = new System.Windows.Forms.Button();
            this.usertextBox = new System.Windows.Forms.TextBox();
            this.passwordtextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.eliminarButton = new System.Windows.Forms.Button();
            this.ModificarPerfilButton = new System.Windows.Forms.Button();
            this.nametextBox = new System.Windows.Forms.TextBox();
            this.cambiarFondoButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(13, 641);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(199, 38);
            this.button1.TabIndex = 4;
            this.button1.Text = "conectar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(798, 632);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(188, 47);
            this.button3.TabIndex = 10;
            this.button3.Text = "desconectar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(292, 179);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(411, 416);
            this.chatBox.TabIndex = 32;
            this.chatBox.Text = "";
            // 
            // chatInputTextBox
            // 
            this.chatInputTextBox.Location = new System.Drawing.Point(235, 614);
            this.chatInputTextBox.Name = "chatInputTextBox";
            this.chatInputTextBox.Size = new System.Drawing.Size(532, 22);
            this.chatInputTextBox.TabIndex = 33;
            // 
            // sendChatButton
            // 
            this.sendChatButton.Location = new System.Drawing.Point(437, 654);
            this.sendChatButton.Name = "sendChatButton";
            this.sendChatButton.Size = new System.Drawing.Size(127, 26);
            this.sendChatButton.TabIndex = 34;
            this.sendChatButton.Text = "Send";
            this.sendChatButton.UseVisualStyleBackColor = true;
            this.sendChatButton.Click += new System.EventHandler(this.sendChatButton_Click);
            // 
            // usertextBox
            // 
            this.usertextBox.Location = new System.Drawing.Point(310, 35);
            this.usertextBox.Name = "usertextBox";
            this.usertextBox.Size = new System.Drawing.Size(119, 22);
            this.usertextBox.TabIndex = 35;
            this.usertextBox.Text = "USER";
            // 
            // passwordtextBox
            // 
            this.passwordtextBox.Location = new System.Drawing.Point(568, 35);
            this.passwordtextBox.Name = "passwordtextBox";
            this.passwordtextBox.Size = new System.Drawing.Size(119, 22);
            this.passwordtextBox.TabIndex = 36;
            this.passwordtextBox.Text = "PASSWORD";
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(127, 80);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(151, 23);
            this.loginButton.TabIndex = 37;
            this.loginButton.Text = "LOGIN";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // registerButton
            // 
            this.registerButton.Location = new System.Drawing.Point(413, 80);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(151, 23);
            this.registerButton.TabIndex = 38;
            this.registerButton.Text = "REGISTER";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
            // 
            // eliminarButton
            // 
            this.eliminarButton.Location = new System.Drawing.Point(704, 80);
            this.eliminarButton.Name = "eliminarButton";
            this.eliminarButton.Size = new System.Drawing.Size(151, 23);
            this.eliminarButton.TabIndex = 39;
            this.eliminarButton.Text = "DELETE";
            this.eliminarButton.UseVisualStyleBackColor = true;
            this.eliminarButton.Click += new System.EventHandler(this.eliminarButton_Click);
            // 
            // ModificarPerfilButton
            // 
            this.ModificarPerfilButton.Location = new System.Drawing.Point(35, 216);
            this.ModificarPerfilButton.Name = "ModificarPerfilButton";
            this.ModificarPerfilButton.Size = new System.Drawing.Size(177, 26);
            this.ModificarPerfilButton.TabIndex = 41;
            this.ModificarPerfilButton.Text = "Change name";
            this.ModificarPerfilButton.UseVisualStyleBackColor = true;
            this.ModificarPerfilButton.Click += new System.EventHandler(this.ModificarPerfilButton_Click);
            // 
            // nametextBox
            // 
            this.nametextBox.Location = new System.Drawing.Point(80, 179);
            this.nametextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nametextBox.Name = "nametextBox";
            this.nametextBox.Size = new System.Drawing.Size(93, 22);
            this.nametextBox.TabIndex = 42;
            this.nametextBox.Text = "Name";
            // 
            // cambiarFondoButton
            // 
            this.cambiarFondoButton.Location = new System.Drawing.Point(771, 179);
            this.cambiarFondoButton.Name = "cambiarFondoButton";
            this.cambiarFondoButton.Size = new System.Drawing.Size(177, 26);
            this.cambiarFondoButton.TabIndex = 43;
            this.cambiarFondoButton.Text = "Change wallpaper";
            this.cambiarFondoButton.UseVisualStyleBackColor = true;
            this.cambiarFondoButton.Click += new System.EventHandler(this.cambiarFondoButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 692);
            this.Controls.Add(this.cambiarFondoButton);
            this.Controls.Add(this.nametextBox);
            this.Controls.Add(this.ModificarPerfilButton);
            this.Controls.Add(this.eliminarButton);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.passwordtextBox);
            this.Controls.Add(this.usertextBox);
            this.Controls.Add(this.sendChatButton);
            this.Controls.Add(this.chatInputTextBox);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox chatBox;
        private System.Windows.Forms.TextBox chatInputTextBox;
        private System.Windows.Forms.Button sendChatButton;
        private System.Windows.Forms.TextBox usertextBox;
        private System.Windows.Forms.TextBox passwordtextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button eliminarButton;
        private System.Windows.Forms.Button ModificarPerfilButton;
        private System.Windows.Forms.TextBox nametextBox;
        private System.Windows.Forms.Button cambiarFondoButton;
    }
}

