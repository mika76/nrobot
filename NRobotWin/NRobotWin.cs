#region Copyright and License
///////////////////////////////////////////////////////////////////////////////
// NRobot - Autonomous robot fighting game
// Copyright (c) 2004,2005 Stuart Ballard
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// The GNU General Public License should be located in the file COPYING.
//
// For more information about NRobot, please contact nrobot-list@gna.org or
// write to Stuart Ballard at NetReach Inc, 124 S Maple Street, Ambler,
// PA  19002  USA.
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;using System.Drawing;using System.Drawing.Drawing2D;using System.Collections;using System.ComponentModel;using System.Windows.Forms;using NRobot.Engine;using NRobot.Render;
namespace NRobot.Win{	using Robot = NRobot.Engine.Robot;	/// <summary>	/// Windows client for NRobot.	/// </summary>	public class NRobotWin : System.Windows.Forms.Form	{		private System.Windows.Forms.Timer timer1;		private System.Windows.Forms.PictureBox arena;		private System.Windows.Forms.TextBox msgLog;		private System.Windows.Forms.Label scoreboard;		private System.ComponentModel.IContainer components;		public NRobotWin(string[] args)		{			//			// Required for Windows Form Designer support			//			InitializeComponent();			StartGame(args);		}		/// <summary>		/// Clean up any resources being used.		/// </summary>		protected override void Dispose( bool disposing )		{			if( disposing )			{				if (components != null) 				{					components.Dispose();				}			}			base.Dispose( disposing );		}		private Game game;		public void StartGame(string[] args) 		{			game = Game.Create();			game.Say += new Sayer(sayHandler);			game.InitFromCommandLineArgs(args);			game.Start();			Repaint();			timer1.Start();		}		private void sayHandler(Robot robot, string message) 		{			msgLog.AppendText((robot == null ? "GAME" : robot.Name) + ": " + message + "\r\n");		}		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.arena = new System.Windows.Forms.PictureBox();
			this.msgLog = new System.Windows.Forms.TextBox();
			this.scoreboard = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// arena
			// 
			this.arena.Location = new System.Drawing.Point(8, 8);
			this.arena.Name = "arena";
			this.arena.Size = new System.Drawing.Size(704, 440);
			this.arena.TabIndex = 0;
			this.arena.TabStop = false;
			// 
			// msgLog
			// 
			this.msgLog.Location = new System.Drawing.Point(8, 456);
			this.msgLog.Multiline = true;
			this.msgLog.Name = "msgLog";
			this.msgLog.ReadOnly = true;
			this.msgLog.Size = new System.Drawing.Size(928, 128);
			this.msgLog.TabIndex = 1;
			this.msgLog.Text = "";
			// 
			// scoreboard
			// 
			this.scoreboard.Location = new System.Drawing.Point(720, 0);
			this.scoreboard.Name = "scoreboard";
			this.scoreboard.Size = new System.Drawing.Size(216, 448);
			this.scoreboard.TabIndex = 2;
			// 
			// NRobotWin
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(944, 589);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.scoreboard,
																		  this.msgLog,
																		  this.arena});
			this.Name = "NRobotWin";
			this.Text = "NRobotWin";
			this.ResumeLayout(false);

		}
		#endregion		/// <summary>		/// The main entry point for the application.		/// </summary>		[STAThread]		static void Main(string[] args) 		{			Application.Run(new NRobotWin(args));		}		private void timer1_Tick(object sender, System.EventArgs e)		{			if (!game.Over) 			{				game.Tick();				Repaint();			}		}		private void Repaint() 		{			if (pic == null) 			{				pic = new Bitmap(arena.Width, arena.Height);				buffer = new Bitmap(arena.Width, arena.Height);			}			Graphics g = Graphics.FromImage(buffer);			g.Clear(SystemColors.Control);			Image temp = pic;			arena.Image = pic = buffer;			buffer = pic;			Renderer renderer = new Renderer(game, new WinCanvas(pic, g));			renderer.Render();			string scores = "";			foreach (Team team in game.Teams) 			{				scores += team.Name + ": " + team.TotalHealth + " health, " + plural(team.AliveBots.Count, "robot") + " alive\n";				foreach (Robot robot in team.AliveBots) 				{					scores += "  " + robot.Name + ": " + robot.Health + "\n";				}				scores += "\n";			}			scoreboard.Text = scores;		}		Image pic;		Image buffer;		private string plural(int count, string name) 		{			return count + " " + name + (count == 1 ? "" : "s");		}	}	public class WinCanvas : ICanvas 	{		private Graphics g;		private Image img;		public WinCanvas(Image img, Graphics g) 		{			this.img = img;			this.g = g;		}		public int Width {get {return img.Width;}}		public int Height {get {return img.Height;}}		public void DrawRectangle(object color, int x, int y, int width, int height) 		{			g.FillRectangle(brush(color), x, y, width, height);		}		public void DrawCircle(object color, int startx, int starty, int diameter) 		{			g.FillEllipse(brush(color), startx, starty, diameter, diameter);		}		public void DrawLine(object color, int x1, int y1, int x2, int y2, int thickness) 		{			Pen lpen = pen(color);			lpen.Width = thickness;			g.DrawLine(lpen, x1, y1, x2, y2);		}		public object GetColor(NRColor color) 		{			return color;		}		private static Hashtable brushes = new Hashtable();		private static Hashtable pens = new Hashtable();		private static Brush brush(object color) 		{			Brush brush = (Brush) brushes[color];			if (brush == null) 			{				NRColor col = (NRColor) color;				brush = new SolidBrush(Color.FromArgb(col.Red, col.Green, col.Blue));				brushes[color] = brush;			}			return brush;		}		private static Pen pen(object color) 		{			Pen pen = (Pen) pens[color];			if (pen == null) 			{				NRColor col = (NRColor) color;				pen = new Pen(Color.FromArgb(col.Red, col.Green, col.Blue));				pen.StartCap = LineCap.Round;				pen.EndCap = LineCap.Round;				pens[color] = pen;			}			return pen;		}	}}