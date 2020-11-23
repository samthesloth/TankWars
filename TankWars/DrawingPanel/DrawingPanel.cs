// Authors: Nicholas Vaskelis and Sam Peters

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TankWars
{
    public class DrawingPanel : Panel
    {
        // References to world, player ID, and view size in order to draw and center camera
        private World theWorld;
        private int PlayerID;
        private int ViewSize;

        //Font and string format
        private Font font;
        private StringFormat sf;

        //Images
        Bitmap WallImage;
        Bitmap BlueTank;
        Bitmap BlueTurret;
        Bitmap DarkTank;
        Bitmap DarkTurret;
        Bitmap GreenTank;
        Bitmap GreenTurret;
        Bitmap LightGreenTank;
        Bitmap LightGreenTurret;
        Bitmap OrangeTank;
        Bitmap OrangeTurret;
        Bitmap PurpleTank;
        Bitmap PurpleTurret;
        Bitmap RedTank;
        Bitmap RedTurret;
        Bitmap YellowTank;
        Bitmap YellowTurret;
        Bitmap Background;
        Bitmap BlueShot;
        Bitmap GrayShot;
        Bitmap GreenShot;
        Bitmap WhiteShot;
        Bitmap BrownShot;
        Bitmap PurpleShot;
        Bitmap RedShot;
        Bitmap YellowShot;

        //Dictionary to hold beams and tanks and their amount of frames since their animation started
        Dictionary<Beam, int> beamsTimer;
        Dictionary<Tank, int> tankTimer;

        /// <summary>
        /// Constructor that  sets double buffered and the world
        /// Also sets up images and font
        /// </summary>
        public DrawingPanel(World w)
        {
            //DoubleBuffered to reduce tearing
            DoubleBuffered = true;
            //Sets world, although this will be changed later in SetWorld
            theWorld = w;

            //Initializes beams and tanks
            beamsTimer = new Dictionary<Beam, int>();
            tankTimer = new Dictionary<Tank, int>();

            //Initializes images and font
            WallImage = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\WallSprite.png");
            BlueTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\BlueTank.png");
            BlueTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\BlueTurret.png");
            DarkTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\DarkTank.png");
            DarkTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\DarkTurret.png");
            GreenTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\GreenTank.png");
            GreenTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\GreenTurret.png");
            LightGreenTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\LightGreenTank.png");
            LightGreenTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\LightGreenTurret.png");
            OrangeTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\OrangeTank.png");
            OrangeTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\OrangeTurret.png");
            PurpleTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\PurpleTank.png");
            PurpleTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\PurpleTurret.png");
            RedTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\RedTank.png");
            RedTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\RedTurret.png");
            YellowTank = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\YellowTank.png");
            YellowTurret = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\YellowTurret.png");
            Background = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\Background.png");
            BlueShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-blue.png");
            GrayShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot_grey.png");
            GreenShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-yellow.png");
            WhiteShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-white.png");
            BrownShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-brown.png");
            PurpleShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-violet.png");
            RedShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot_red_new.png");
            YellowShot = (Bitmap)Image.FromFile(@"..\..\..\Resources\Images\shot-yellow.png");

            //Set up stringformat and font
            sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            font = new Font("Franklin Gothic", 15);
        }

        /// <summary>
        /// Sets PlayerID to ID so the camera can center around the player
        /// </summary>
        public void SetID(int ID)
        {
            PlayerID = ID;
        }

        /// <summary>
        /// Sets ViewSize to size so camera can be centered
        /// </summary>
        public void SetViewSize(int size)
        {
            ViewSize = size;
        }

        /// <summary>
        /// Sets world since constructor is called early. Used for drawing and centering
        /// </summary>
        public void SetWorld(World w)
        {
            theWorld = w;
        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws body of tanks
        /// </summary>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            //Gets the tank object and sets width and height. Sets antialias
            Tank t = o as Tank;
            int width = 60;
            int height = 60;

            // Creates rectangle to draw tank and draws with image
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            e.Graphics.DrawImage(GetTankColor(t), r);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws HP, name, and score of tanks
        /// </summary>
        private void InfoDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            SolidBrush brush = new SolidBrush(Color.White);
            using (brush) {
                //Draw name and score
                e.Graphics.DrawString(t.name + ": " + t.score, font, brush, 0, 30, sf);
                //Draw healthbar according to hp left
                switch (t.hitPoints)
                {
                    case (3):
                        brush.Color = Color.Green;
                        e.Graphics.FillRectangle(brush, new Rectangle(-25, -50, 50, 10));
                        break;
                    case (2):
                        brush.Color = Color.Orange;
                        e.Graphics.FillRectangle(brush, new Rectangle(-17, -50, 33, 10));
                        break;
                    case (1):
                        brush.Color = Color.Red;
                        e.Graphics.FillRectangle(brush, new Rectangle(-8, -50, 17, 10));
                        break;
                }
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws top turret of tank
        /// </summary>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            //Gets the tank object and sets width and height. Sets antialias
            Tank t = o as Tank;
            int width = 60;
            int height = 60;

            // Creates rectangle to draw and then draws image
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            e.Graphics.DrawImage(GetTurretColor(t), r);
        }

        /// <summary>
        /// Returns the image of the turret color for the given turret using ID
        /// </summary>
        private Bitmap GetTurretColor(Tank t)
        {
            switch (t.ID % 8)
            {
                case (0):
                    return BlueTurret;
                case (1):
                    return DarkTurret;
                case (2):
                    return GreenTurret;
                case (3):
                    return LightGreenTurret;
                case (4):
                    return OrangeTurret;
                case (5):
                    return PurpleTurret;
                case (6):
                    return RedTurret;
                case (7):
                    return YellowTurret;
                default:
                    return BlueTurret;
            }
        }

        /// <summary>
        /// Returns the image of the turret color for the given turret using ID
        /// </summary>
        private Bitmap GetTankColor(Tank t)
        {
            switch (t.ID % 8)
            {
                case (0):
                    return BlueTank;
                case (1):
                    return DarkTank;
                case (2):
                    return GreenTank;
                case (3):
                    return LightGreenTank;
                case (4):
                    return OrangeTank;
                case (5):
                    return PurpleTank;
                case (6):
                    return RedTank;
                case (7):
                    return YellowTank;
                default:
                    return BlueTank;
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws projectile objects
        /// </summary>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            //Gets the tank object and sets width and height. Sets antialias
            Projectile p = o as Projectile;
            int width = 30;
            int height = 30;

            // Creates rectangle for drawing and then draws image
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            e.Graphics.DrawImage(GetProjectileColor(p), r);
        }

        /// <summary>
        /// Returns the image of the projectile color for the given turret using owner's ID
        /// </summary>
        private Bitmap GetProjectileColor(Projectile p)
        {
            switch (p.owner % 8)
            {
                case (0):
                    return BlueShot;
                case (1):
                    return BrownShot;
                case (2):
                    return GreenShot;
                case (3):
                    return WhiteShot;
                case (4):
                    return GrayShot;
                case (5):
                    return PurpleShot;
                case (6):
                    return RedShot;
                case (7):
                    return YellowShot;
                default:
                    return BlueShot;
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws powerup collectibles throughout gameworld
        /// </summary>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            //Get powerup and set width, height, and antialias
            Powerup p = o as Powerup;
            int width = 15;
            int height = 15;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Creates rectangle for drawing and then draws ellipse/circle
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            {
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.FillEllipse(redBrush, r);
            }
        }

        /// <summary>
        /// Adds beam to beamsTimer dictionary so countdown for animation can be started
        /// </summary>
        public void DrawBeam(Beam b)
        {
            beamsTimer.Add(b, 0);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws beams using frames in beamsTimer
        /// </summary>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            //Gets the tank object and sets width and height of beam using timer
            Beam b = o as Beam;
            int width = 10 - (beamsTimer[b] / 6);
            int height = 5000;

            // Creates new rectangle for beam and then draws rectangle
            Rectangle r = new Rectangle(-width, -height, width, height);
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red)) {
                e.Graphics.FillRectangle(redBrush, r);
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws tank epxlosions using tankTimer
        /// </summary>
        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            //Gets tank. If world no longer has tank, remove tank from tankTimer and return
            Tank t = o as Tank;
            Tank temp = new Tank();
            if (!theWorld.GetTank(t.ID, out temp))
            {
                tankTimer.Remove(t);
                return;
            }

            //Gets the tank object and sets width and height for explosion using tankTimer
            int frames = tankTimer[t];
            int radius;
            if (frames <= 50)
                radius = frames  * 2;
            else if (frames <= 150)
                radius = 150 - frames;
            else
                radius = 0;

            //Creates rectangle and then draws ellipse/circle of explosion
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            {
                Rectangle r = new Rectangle(-(radius / 2), -(radius / 2), radius, radius);

                e.Graphics.FillEllipse(redBrush, r);
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// Draws walls
        /// </summary>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            //Gets wall and distance between the two points
            Wall w = o as Wall;
            int height, width;
            int distance = (int)Math.Sqrt(Math.Pow(w.p1.GetX() - w.p2.GetX(), 2) + Math.Pow(w.p1.GetY() - w.p2.GetY(), 2)) + 50;

            //If X's are same, then wall is vertical
            if (w.p2.GetX() == w.p1.GetX())
            {
                width = 50;
                height = distance;
            }
            //Otherwise, wall is horizontal
            else
            {
                height = 50;
                width = distance;
            }

            //Texture brush with image to draw walls
            TextureBrush textureBrush = new TextureBrush(WallImage, System.Drawing.Drawing2D.WrapMode.Tile);
            textureBrush.ScaleTransform(0.8f, 0.8f);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Uses texture brush to create new rectangle and fill rectangle with image
            using (textureBrush)
            {
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                e.Graphics.FillRectangle(textureBrush, r);
            }
        }

        /// <summary>
        /// This method is invoked when the DrawingPanel needs to be re-drawn
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            //Makes sure world is null, since it is null while loading game
            if (theWorld != null)
            {
                //Locks world since it accesses info from it
                lock (theWorld)
                {
                    //Get player's tank to center view
                    Tank player;
                    if (theWorld.GetTank(PlayerID, out player))
                    {
                            double playerX = player.location.GetX();
                            double playerY = player.location.GetY();

                            double ratio = (double)ViewSize / (double)theWorld.GetSize();
                            int halfSizeScaled = (int)(theWorld.GetSize() / 2.0 * ratio);

                            double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.GetSize(), playerX) + halfSizeScaled;
                            double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.GetSize(), playerY) + halfSizeScaled;

                            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);
                    }

                    //Draw background
                    e.Graphics.DrawImage(Background, new Rectangle(0, 0, theWorld.GetSize(), theWorld.GetSize()));

                    // Draw the tanks
                    foreach (Tank tank in theWorld.GetTanks())
                    {
                        //Draw tank, turret, and info if tank is alive and ingame
                        if (!tank.died && !tankTimer.ContainsKey(tank) && tank.hitPoints > 0)
                        {
                            DrawObjectWithTransform(e, tank, theWorld.GetSize(), tank.location.GetX(), tank.location.GetY(), tank.orientation.ToAngle(), TankDrawer);
                            DrawObjectWithTransform(e, tank, theWorld.GetSize(), tank.location.GetX(), tank.location.GetY(), tank.aiming.ToAngle(), TurretDrawer);
                            DrawObjectWithTransform(e, tank, theWorld.GetSize(), tank.location.GetX(), tank.location.GetY(), 0, InfoDrawer);
                        }
                        //Otherwise, add tank to tank timer, increment tank timer, or remove tank from tank timer if it is alive
                        else
                        {
                            //If tank is a live again, remove from tank timer
                            if(tank.hitPoints > 0)
                            {
                                tankTimer.Remove(tank);
                                break;
                            }
                            //If tank is already in tank timer, decrement tank timer or remove tank from timer
                            if (tankTimer.ContainsKey(tank))
                            {
                                if (tankTimer[tank] < 200)
                                {
                                    tankTimer[tank]++;
                                }
                                else
                                {
                                    tankTimer.Remove(tank);
                                }
                            }
                            //Add tank to timer if not in timer yet
                            else
                            {
                                tankTimer.Add(tank, 0);
                            }
                        }
                    }

                    // Draw the walls
                    foreach (Wall w in theWorld.GetWalls())
                    {
                        Vector2D location = GetWallLocation(w);
                        DrawObjectWithTransform(e, w, theWorld.GetSize(), location.GetX(), location.GetY(), 0, WallDrawer);
                    }

                    //Draw beams if existed for less than 60 frames
                    List<Beam> beams = new List<Beam>(beamsTimer.Keys);
                    foreach (Beam b in beams)
                    {
                        if(beamsTimer[b] > 60)
                        {
                            beamsTimer.Remove(b);
                        }
                        //Increments beamsTimer and draws beams
                        else
                        {
                            beamsTimer[b]++;
                            DrawObjectWithTransform(e, b, theWorld.GetSize(), b.origin.GetX(), b.origin.GetY(), b.direction.ToAngle(), BeamDrawer); ;
                        }
                    }

                    // Draw the powerups if not dead
                    foreach (Powerup pow in theWorld.GetPowerups())
                    {
                        if (!pow.died)
                        {
                            DrawObjectWithTransform(e, pow, theWorld.GetSize(), pow.location.GetX(), pow.location.GetY(), 0, PowerupDrawer);
                        }
                    }

                    //Draw projectiles if not dead
                    foreach (Projectile p in theWorld.GetProjectiles())
                    {
                        if (!p.died)
                        {
                            DrawObjectWithTransform(e, p, theWorld.GetSize(), p.location.GetX(), p.location.GetY(), p.orientation.ToAngle(), ProjectileDrawer);
                        }
                    }

                    //Draw tank explosions with tanks in tankTimer
                    List<Tank> tanks = new List<Tank>(tankTimer.Keys);
                    foreach (Tank t in tanks)
                    {
                        DrawObjectWithTransform(e, t, theWorld.GetSize(), t.location.GetX(), t.location.GetY(), 0, ExplosionDrawer);
                    }
                }
            }
            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

        /// <summary>
        /// Gets middle point of wall
        /// </summary>
        private Vector2D GetWallLocation(Wall w)
        {
            return new Vector2D((w.p1.GetX() + w.p2.GetX()) / 2, (w.p1.GetY() + w.p2.GetY()) / 2);
        }
    }
}