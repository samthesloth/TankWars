using System;
using System.Drawing;
using System.Windows.Forms;

namespace TankWars
{
    public partial class Form1 : Form
    {
        //Controller and world objects
        private GameController controller;
        private World world;

        //Drawing panel where game will be displayed
        private DrawingPanel drawingPanel;

        //Button to start the game
        private Button startButton;

        //Label and text box for putting in player name
        private Label nameLabel;
        private TextBox nameText;

        //Label and text box for putting in host name
        private Label hostLabel;
        private TextBox hostText;

        //Constants to be used for window management
        private const int viewSize = 900;
        private const int menuSize = 40;

        /// <summary>
        /// Main form application. Sets controller, world, and clientsize. Then adds events for controller and finally adds form components
        /// </summary>
        public Form1(GameController controller)
        {
            //Initialize and set controller to parameter and world to controller's world
            InitializeComponent();
            this.controller = controller;
            this.world = controller.GetWorld();

            //Sets clientsize for form
            ClientSize = new Size(viewSize, viewSize + menuSize);

            //Adds listeners to events for controller-view handshake
            controller.OnUpdate += OnFrame;
            controller.IDLoaded += SetID;
            controller.WorldLoaded += SetWorld;

            // Place and add the start button
            startButton = new Button();
            startButton.Location = new Point(245, 5);
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
            nameText.MaxLength = 16;
            nameText.Location = new Point(50, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);

            // Place and add the host label
            hostLabel = new Label();
            hostLabel.Text = "Host:";
            hostLabel.Location = new Point(125, 10);
            hostLabel.Size = new Size(40, 15);
            this.Controls.Add(hostLabel);

            // Place and add the host textbox
            hostText = new TextBox();
            hostText.Text = "";
            hostText.Location = new Point(165, 5);
            hostText.Size = new Size(70, 15);
            this.Controls.Add(hostText);

            // Place and add the drawing panel with event when beam gets fired
            drawingPanel = new DrawingPanel(world);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            drawingPanel.SetViewSize(viewSize);
            this.Controls.Add(drawingPanel);
            controller.BeamFired += drawingPanel.DrawBeam;

            //Set up control events for keys and mouse inputs
            this.KeyDown += Moved;
            drawingPanel.MouseDown += Shoot;
            drawingPanel.MouseUp += StopShoot;
            drawingPanel.MouseMove += Aim;
            this.KeyUp += StopMove;
        }

        /// <summary>
        /// Handler for the controller's OnUpdate event. Invalidates itself so drawingPanel redraws
        /// </summary>
        private void OnFrame()
        { 
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            try
            {
                MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
                this.Invoke(invalidator);
            }
            catch { }

            //If world is set and walls are loaded, call Send
            if (world != null && world.WallsLoaded)
            {
                controller.Send();
            }
        }

        /// <summary>
        /// Method to be called when key is pushed to set controller's movement booleans to true
        /// </summary>
        private void Moved(Object sender, KeyEventArgs e)
        {
            lock (sender)
            {
                switch (e.KeyData)
                {
                    case (Keys.W):
                        controller.W = true;
                        break;
                    case (Keys.A):
                        controller.A = true;
                        break;
                    case (Keys.D):
                        controller.D = true;
                        break;
                    case (Keys.S):
                        controller.S = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Method to be called when key is done being pushed to set controller's movement booleans to false
        /// </summary>
        private void StopMove(Object sender, KeyEventArgs e)
        {
            lock (sender)
            {
                switch (e.KeyData)
                {
                    case (Keys.W):
                        controller.W = false;
                        break;
                    case (Keys.A):
                        controller.A = false;
                        break;
                    case (Keys.D):
                        controller.D = false;
                        break;
                    case (Keys.S):
                        controller.S = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Method to be called when mouse is clicked to change controller's mouse bools to true
        /// </summary>
        private void Shoot(Object sender, MouseEventArgs e)
        {
            switch(e.Button)
            {
                case (MouseButtons.Left):
                    controller.LMB = true;
                    break;
                case (MouseButtons.Right):
                    controller.RMB = true;
                    break;
            }
        }

        /// <summary>
        /// Method to be called when mouse button is released to change controller's mouse bools to false
        /// </summary>
        private void StopShoot(Object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case (MouseButtons.Left):
                    controller.LMB = false;
                    break;
                case (MouseButtons.Right):
                    controller.RMB = false;
                    break;
            }
        }

        /// <summary>
        /// Method to be called when mouse moves. Calls controller's aiming method with normalized vector2D of mouse location
        /// </summary>
        private void Aim(Object sender, MouseEventArgs e)
        {
            Vector2D aim = new Vector2D(e.X - 450, e.Y - 450);
            aim.Normalize();
            controller.Aiming(aim);
        }

        /// <summary>
        /// Handler for the controller's IDLoaded event. Call's drawingPanel's SetID so we know where to center camera
        /// </summary>
        private void SetID()
        {
            drawingPanel.SetID(controller.GetPlayerID());
        }

        /// <summary>
        /// Handler for the controller's WorldLoaded event. Sets this form and drawingPanel's world to the loaded world
        /// Also enables KeyPreview
        /// </summary>
        private void SetWorld()
        {
            world = controller.GetWorld();
            drawingPanel.SetWorld(controller.GetWorld());

            // Enable the global form to capture key presses
            KeyPreview = true;
        }

        /// <summary>
        /// Called when Start is clicked. Disables form components and calls controller's connect method
        /// </summary>
        private void StartClick(object sender, EventArgs e)
        {
            //If host and name are nonempty
            if (hostText.TextLength > 0 && nameText.TextLength > 0)
            {
                // Disable the form controls
                startButton.Enabled = false;
                startButton.TabStop = false;
                nameText.Enabled = false;
                hostText.Enabled = false;
                //Set focus to drawing panel
                this.Focus();
                // "connect" to the "server"
                controller.Connect(hostText.Text, nameText.Text);
            }
        }

        /// <summary>
        /// When form is closing, close socket
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.Close();
        }
    }
}