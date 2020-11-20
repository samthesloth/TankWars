using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;

namespace TankWars
{
    public partial class Form1 : Form
    {
        GameController controller;
        World world;
        DrawingPanel drawingPanel;
        Button startButton;
        Label nameLabel;
        TextBox nameText;
        Label hostLabel;
        TextBox hostText;

        private const int viewSize = 900;
        private const int menuSize = 40;


        public Form1(GameController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.world = controller.GetWorld();

            ClientSize = new Size(viewSize, viewSize + menuSize);

            controller.OnUpdate += OnFrame;
            controller.IDLoaded += SetID;
            controller.WorldLoaded += SetWorld;

            // Place and add the start button
            startButton = new Button();
            startButton.Location = new Point(230, 5);
            startButton.Size = new Size(70, 20);
            startButton.Text = "Start";
            startButton.Click += StartClick;
            this.Controls.Add(startButton);

            // Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 10);
            nameLabel.Size = new Size(40, 15);
            this.Controls.Add(nameLabel);

            // Place and add the name textbox
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(50, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // Place and add the host label
            hostLabel = new Label();
            hostLabel.Text = "Host:";
            hostLabel.Location = new Point(120, 10);
            hostLabel.Size = new Size(40, 15);
            this.Controls.Add(hostLabel);

            // Place and add the host textbox
            hostText = new TextBox();
            hostText.Text = "localhost";
            hostText.Location = new Point(160, 5);
            hostText.Size = new Size(70, 15);
            this.Controls.Add(hostText);

            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(world);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            drawingPanel.SetViewSize(viewSize);
            this.Controls.Add(drawingPanel);
        }


        /// <summary>
        /// Handler for the controller's OnUpdate event
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(invalidator);
        }

        private void SetID()
        {
            drawingPanel.SetID(controller.GetPlayerID());
        }

        private void SetWorld()
        {
            drawingPanel.SetWorld(controller.GetWorld());
        }

        /// <summary>
        /// When
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, EventArgs e)
        {
            // Disable the form controls
            startButton.Enabled = false;
            nameText.Enabled = false;
            // Enable the global form to capture key presses
            KeyPreview = true;
            // "connect" to the "server"
            controller.Connect(hostText.Text, nameText.Text);
        }


    }
}
