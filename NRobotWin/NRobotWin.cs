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

using System;
namespace NRobot.Win
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
		#endregion