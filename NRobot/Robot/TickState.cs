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
using NRobot.Engine;

namespace NRobot.Robot 
{
	using Robot = NRobot.Engine.Robot;

	public class TickState : RobotState 
	{
		public string Name 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.Name;
			}
		}

		internal int moveSpeed;
		public int MoveSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return moveSpeed;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				moveSpeed = value;
				if (moveSpeed > robot.MaxMoveSpeed) moveSpeed = robot.MaxMoveSpeed;
				else if (moveSpeed < -robot.MaxMoveSpeed) moveSpeed = -robot.MaxMoveSpeed;
				if (moveSpeed == 0) moveDuration = 0;
				else if (moveDuration == 0) moveDuration = 1;
			}
		}
		internal int moveDuration;
		public int MoveDuration 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return moveDuration;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				moveDuration = value;
				if (moveDuration == 0) moveSpeed = 0;
				else if (moveSpeed == 0) moveSpeed = robot.MaxMoveSpeed;
			}
		}
		internal int turnSpeed;
		public int TurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return turnSpeed;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				turnSpeed = value;
				if (turnSpeed > robot.MaxTurnSpeed) turnSpeed = robot.MaxTurnSpeed;
				else if (turnSpeed < -robot.MaxTurnSpeed) turnSpeed = -robot.MaxTurnSpeed;
				if (turnSpeed == 0) turnDuration = 0;
				else if (turnDuration == 0) turnDuration = 1;
			}
		}
		internal int turnDuration;
		public int TurnDuration 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return turnDuration;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				turnDuration = value;
				if (turnDuration == 0) turnSpeed = 0;
				else if (turnSpeed == 0) turnSpeed = robot.MaxTurnSpeed;
			}
		}
		internal int gunTurnSpeed;
		public int GunTurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return gunTurnSpeed;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				gunTurnSpeed = value;
				if (gunTurnSpeed > robot.MaxTurretTurnSpeed) gunTurnSpeed = robot.MaxTurretTurnSpeed;
				else if (gunTurnSpeed < -robot.MaxTurretTurnSpeed) gunTurnSpeed = -robot.MaxTurretTurnSpeed;
				if (gunTurnSpeed == 0) gunTurnDuration = 0;
				else if (gunTurnDuration == 0) gunTurnDuration = 1;
			}
		}
		internal int gunTurnDuration;
		public int GunTurnDuration 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return gunTurnDuration;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				gunTurnDuration = value;
				if (gunTurnDuration == 0) gunTurnSpeed = 0;
				else if (gunTurnSpeed == 0) gunTurnSpeed = robot.MaxTurretTurnSpeed;
			}
		}
		internal int cameraTurnSpeed;
		public int CameraTurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return cameraTurnSpeed;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				cameraTurnSpeed = value;
				if (cameraTurnSpeed > robot.MaxTurretTurnSpeed) cameraTurnSpeed = robot.MaxTurretTurnSpeed;
				else if (cameraTurnSpeed < -robot.MaxTurretTurnSpeed) cameraTurnSpeed = -robot.MaxTurretTurnSpeed;
				if (cameraTurnSpeed == 0) cameraTurnDuration = 0;
				else if (cameraTurnDuration == 0) cameraTurnDuration = 1;
			}
		}
		internal int cameraTurnDuration;
		public int CameraTurnDuration 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return cameraTurnDuration;
			}
			set 
			{
				if (!IsActive) throw new ApplicationException("Cannot set values on an inactive state");
				cameraTurnDuration = value;
				if (cameraTurnDuration == 0) cameraTurnSpeed = 0;
				else if (cameraTurnSpeed == 0) cameraTurnSpeed = robot.MaxTurretTurnSpeed;
			}
		}

		public int TurretTurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				if (gunTurnSpeed != cameraTurnSpeed) throw new ApplicationException("Gun and camera speeds not equal");
				return gunTurnSpeed;
			}
			set 
			{
				GunTurnSpeed = value;
				CameraTurnSpeed = value;
			}
		}
		public int TurretTurnDuration 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				if (gunTurnDuration != cameraTurnDuration) throw new ApplicationException("Gun and camera durations not equal");
				return gunTurnDuration;
			}
			set 
			{
				GunTurnDuration = value;
				CameraTurnDuration = value;
			}
		}

		internal bool fired = false;
		public void Fire() 
		{
			if (!IsActive) throw new ApplicationException("Cannot fire from an inactive state");
			fired = true;
		}

		internal decimal newX;
		internal decimal newY;

		public int BotShotsPermitted 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.ShotsPermitted - robot.Bullets.Count;
			}
		}
		public int ShotsPermitted 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return min(Team.ShotsPermitted, BotShotsPermitted);
			}
		}
		public int DamageThisTick 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.prevHealth - robot.health;
			}
		}
		public int ActualDistanceMoved 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.actualDistanceMoved;
			}
		}
		public int ShotDelay 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.currentShotDelay;
			}
		}
		internal ArrayList visibleBullets = new ArrayList();
		public ArrayList VisibleBullets 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return visibleBullets;
			}
		}
		internal ArrayList impactsThisTick = new ArrayList();
		/// <summary>Currently doesn't give exact point of impact, just the position
		/// at the time of the tick. So don't rely too much on the details of the
		/// BulletInfo object yet.</summary>
		public ArrayList ImpactsThisTick 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return impactsThisTick;
			}
		}

		internal TickState(Robot robot) : base(robot) 
		{
			moveSpeed = robot.moveSpeed;
			moveDuration = robot.moveDuration;
			turnSpeed = robot.turnSpeed;
			turnDuration = robot.turnDuration;
			gunTurnSpeed = robot.gunTurnSpeed;
			gunTurnDuration = robot.gunTurnDuration;
			cameraTurnSpeed = robot.cameraTurnSpeed;
			cameraTurnDuration = robot.cameraTurnDuration;

			// Find all the visible bullets and make them available
			foreach (Bullet bullet in robot.GameState.bullets) 
			{
				BulletInfo bi = new BulletInfo(this, bullet);
				if (bi.angleFromSelf >= -eighth && bi.angleFromSelf <= eighth) 
				{
					visibleBullets.Add(bi);
				}
			}

			// Find all the impacts this tick and make those available too
			foreach (Bullet bullet in robot.ImpactsThisTick) 
			{
				impactsThisTick.Add(new BulletInfo(this, bullet));
			}
		}
		private static int min(int a, int b) 
		{
			return a < b ? a : b;
		}
		private const int eighth = NRMath.FullCircle / 8;
	}
}
