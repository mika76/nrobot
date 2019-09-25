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
using System.Collections;
using System.Reflection;
using NRobot.Robot;

namespace NRobot.Engine 
{

	[Serializable]
	public class Bullet 
	{

		// Make the constructor internal so this can't be instantiated from
		// elsewhere
		internal Bullet(Robot robot, GameRules rules) 
		{
			this.robot = robot;
			this.x = robot.X + NRMath.Sin(robot.GunDirection) * rules.RobotRadius;
			this.y = robot.Y + NRMath.Cos(robot.GunDirection) * rules.RobotRadius;
			this.direction = robot.GunDirection;
		}

		[NonSerialized]
		private object idObject;
		public object IdObject {get {if (idObject == null) idObject = new object(); return idObject;}}

		internal int direction;

		internal decimal x;
		public int X {get {return (int) x;}}
		internal decimal y;
		public int Y {get {return (int) y;}}
		private Robot robot;
		public Robot Robot {get {return robot;}}
		public Team Team {get {return robot.Team;}}
		public Game Game {get {return robot.Game;}}
	}
}