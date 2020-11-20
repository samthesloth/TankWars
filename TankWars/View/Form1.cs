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
        /// Handler for the controller's OnUpdate event. Invalidates itself so drawingPanel redraws
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(invalidator);
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
        /// </summary>
        private void SetWorld()
        {
            world = controller.GetWorld();
            drawingPanel.SetWorld(controller.GetWorld());
        }

        /// <summary>
        /// Called when Start is clicked. Disables form components, enables KeyPreview, and calls controller's connect method
        /// </summary>
        private void StartClick(object sender, EventArgs e)
        {
            // Disable the form controls
            startButton.Enabled = false;
            nameText.Enabled = false;
            hostText.Enabled = false;
            // Enable the global form to capture key presses
            KeyPreview = true;
            // "connect" to the "server"
            controller.Connect(hostText.Text, nameText.Text);
        }
    }
}