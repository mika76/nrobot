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
	public class Robot 
	{

		private static int counter = 0;

		// Make the constructor internal so this can't be instantiated from
		// elsewhere
		internal Robot(Team team, int x, int y, int direction) 
		{
			this.team = team;
			this.x = x;
			this.y = y;
			this.direction = direction;
			this.health = team.GameState.rules.StartHealth;
			this.id = ++counter;
		}

		private int id;

		[NonSerialized]
		internal IRobot iRobot;

		[NonSerialized]
		private object idObject;
		public object IdObject {get {if (idObject == null) idObject = new object(); return idObject;}}

		internal decimal x;
		public int X {get {return (int) x;}}
		internal decimal y;
		public int Y {get {return (int) y;}}
		internal int direction;
		public int Direction {get {return direction;}} // 0-999, 0 (and 1000 if it existed) are North
		internal int gunAngle = 0;
		public int GunDirection {get {return NRMath.Angle(direction + gunAngle);}}
		internal int cameraAngle = 0;
		public int CameraDirection {get {return NRMath.Angle(direction + cameraAngle);}}

		internal NRColor bodyColor;
		public NRColor BodyColor {get {return bodyColor;}}
		internal NRColor turretColor;
		public NRColor TurretColor {get {return turretColor;}}
		internal NRColor gunColor;
		public NRColor GunColor {get {return gunColor;}}
		internal NRColor wheelColor;
		public NRColor WheelColor {get {return wheelColor;}}
		internal NRColor cameraColor;
		public NRColor CameraColor {get {return cameraColor;}}
		internal NRColor lensColor;
		public NRColor LensColor {get {return lensColor;}}
		internal NRColor bulletColor;
		public NRColor BulletColor {get {return bulletColor;}}

		internal int moveSpeed;
		internal int moveDuration;
		internal int turnSpeed;
		internal int turnDuration;
		internal int gunTurnSpeed;
		internal int gunTurnDuration;
		internal int cameraTurnSpeed;
		internal int cameraTurnDuration;
		internal int prevHealth;
		internal int currentShotDelay;
		internal int actualDistanceMoved;

		internal ArrayList impactsThisTick = new ArrayList();

		// List of Bullet objects that impacted this robot this tick.
		public IList ImpactsThisTick {get {return impactsThisTick;}}

		internal string name = "";
		public string Name {get {return name;}}
		private Team team;
		public Team Team {get {return team;}}
		internal GameState GameState {get {return team.GameState;}}
		public Game Game {get {return GameState.game;}}
		internal ArrayList bullets = new ArrayList();
		public IList Bullets {get {return bullets;}}
		internal int health;
		public int Health {get {return health;}}

		// Tradeoffs in favour of offense
		internal int bulletDamage;
		public int BulletDamage {get {return bulletDamage;}}
		internal int shotDelay;
		public int ShotDelay {get {return shotDelay;}}
		internal int bulletSpeed;
		public int BulletSpeed {get {return bulletSpeed;}}
		internal int shotsPermitted;
		public int ShotsPermitted {get {return shotsPermitted;}}

		// Tradeoffs in favor of defense
		internal int maxMoveSpeed;
		public int MaxMoveSpeed {get {return maxMoveSpeed;}}
		internal int maxTurnSpeed;
		public int MaxTurnSpeed {get {return maxTurnSpeed;}}
		internal int maxTurretTurnSpeed;
		public int MaxTurretTurnSpeed {get {return maxTurretTurnSpeed;}}
		internal int radius;
		public int Radius {get {return radius;}}

		public override bool Equals(object obj)
		{
			Robot r = obj as Robot;
			return r != null && r.id == id;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode ();
		}


	}
}