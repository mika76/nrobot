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

namespace NRobot.Robot 
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class RobotInfoAttribute : Attribute, IComparable 
	{
		public RobotInfoAttribute(int index) 
		{
			this.index = index;
		}
		private int index;
		public int Index {get {return index;}}
		private string name;
		public string Name {get {return name;} set {name = value;}}
		private int bodyColor = int.MinValue;
		public int BodyColor {get {return bodyColor;} set {bodyColor = value;}}
		private int turretColor = int.MinValue;
		public int TurretColor {get {return turretColor;} set {turretColor = value;}}
		private int gunColor = int.MinValue;
		public int GunColor {get {return gunColor;} set {gunColor = value;}}
		private int wheelColor = int.MinValue;
		public int WheelColor {get {return wheelColor;} set {wheelColor = value;}}
		private int cameraColor = int.MinValue;
		public int CameraColor {get {return cameraColor;} set {cameraColor = value;}}
		private int lensColor = int.MinValue;
		public int LensColor {get {return lensColor;} set {lensColor = value;}}
		private int bulletColor = int.MinValue;
		public int BulletColor {get {return bulletColor;} set {bulletColor = value;}}

	
		// Tradeoffs in favour of offense
		private TradeOff bulletDamage;
		public TradeOff BulletDamage {get {return bulletDamage;} set {bulletDamage = value;}}
		private TradeOff shotDelay;
		public TradeOff ShotDelay {get {return shotDelay;} set {shotDelay = value;}}
		private TradeOff bulletSpeed;
		public TradeOff BulletSpeed {get {return bulletSpeed;} set {bulletSpeed = value;}}
		private TradeOff shotsPermitted;
		public TradeOff ShotsPermitted {get {return shotsPermitted;} set {shotsPermitted = value;}}

		// Tradeoffs in favor of defense
		private TradeOff maxMoveSpeed;
		public TradeOff MaxMoveSpeed {get {return maxMoveSpeed;} set {maxMoveSpeed = value;}}
		private TradeOff maxTurnSpeed;
		public TradeOff MaxTurnSpeed {get {return maxTurnSpeed;} set {maxTurnSpeed = value;}}
		private TradeOff maxTurretTurnSpeed;
		public TradeOff MaxTurretTurnSpeed {get {return maxTurretTurnSpeed;} set {maxTurretTurnSpeed = value;}}
		private TradeOff size;
		public TradeOff Size {get {return size;} set {size = value;}}

		public int CompareTo(object obj)
		{
			RobotInfoAttribute ria = obj as RobotInfoAttribute;
			return index.CompareTo(ria.index);
		}
	}
}
