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

	public class RobotState 
	{

		internal RobotTeam team;
		public RobotTeam Team {get {return team;}}

		public int Radius 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.Radius;
			}
		}
		public int MaxMoveSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.MaxMoveSpeed;
			}
		}
		public int MaxTurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.MaxTurnSpeed;
			}
		}
		public int MaxTurretTurnSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.MaxTurretTurnSpeed;
			}
		}
		public int BulletSpeed 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.BulletSpeed;
			}
		}
		public int TeamSize 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.Team.GameState.rules.TeamSize;
			}
		}

		internal int distanceToWall;
		public int DistanceToWall 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return distanceToWall;
			}
		}
		internal int angleToWall;
		public int AngleToWall 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return angleToWall;
			}
		}
		internal Robot robot;
		public int Health 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.Health;
			}
		}
		internal ArrayList visibleBots;
		public IList VisibleBots 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return visibleBots;
			}
		}
		internal ArrayList teams;
		public IList Teams 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return teams;
			}
		}

		public int CameraAngle 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.cameraAngle;
			}
		}
		public int GunAngle 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				return robot.gunAngle;
			}
		}
		public int TurretAngle 
		{
			get 
			{
				if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
				if (robot.cameraAngle != robot.gunAngle) throw new ApplicationException("Camera and gun at different angles");
				return robot.cameraAngle;
			}
		}

		public object IdObject {get {return robot.IdObject;}}

		internal bool isActive;
		public bool IsActive {get {return isActive;}}

		public void Say(string message) 
		{
			robot.GameState.arena.callSay(robot, message);
		}

		internal Hashtable botsById = new Hashtable();
		public OtherBot GetRobotById(object idObject) 
		{
			if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
			return (OtherBot) botsById[idObject];
		}

		internal Hashtable teamsById = new Hashtable();
		public RobotTeam GetTeamById(object idObject) 
		{
			if (!IsActive) throw new ApplicationException("Cannot get information out of an inactive state");
			return (RobotTeam) teamsById[idObject];
		}

		internal RobotState(Robot robot) 
		{
			this.robot = robot;
			visibleBots = new ArrayList();
			teams = new ArrayList();
			foreach (Team team in robot.GameState.teams) 
			{
				// RobotTeam constructor automatically adds any bots that are visible
				// into our visibleBots list
				RobotTeam rt = new RobotTeam(this, team);
				teams.Add(rt);
				teamsById[team.IdObject] = rt;
				if (team == robot.Team) this.team = rt;
			}
			visibleBots.Sort();

			// Lastly, figure out distance and angle to wall...
			int direction = robot.CameraDirection;
			if (direction % NRMath.Right == 0) 
			{
				angleToWall = 0;
				if (direction == 0) 
				{
					distanceToWall = robot.Y;
				} 
				else if (direction == NRMath.Right) 
				{
					distanceToWall = robot.GameState.rules.ArenaWidth - robot.X;
				} 
				else if (direction == NRMath.HalfCircle) 
				{
					distanceToWall = robot.GameState.rules.ArenaHeight - robot.Y;
				} 
				else if (direction == NRMath.Right3) 
				{
					distanceToWall = robot.X;
				} 
				else 
				{
					throw new ApplicationException("Internal error: invalid angle");
				}
			} 
			else 
			{
				decimal vNorm = (direction < NRMath.Right ||
					direction > NRMath.Right3) ? robot.Y :
					robot.GameState.rules.ArenaHeight - robot.Y;
				decimal hNorm = (direction > NRMath.HalfCircle) ? robot.X :
					robot.GameState.rules.ArenaWidth - robot.X;
				decimal vDist = abs(vNorm / NRMath.Cos(direction));
				decimal hDist = abs(hNorm / NRMath.Sin(direction));
				if (vDist < hDist) 
				{
					distanceToWall = (int) vDist;
					angleToWall = ((direction + NRMath.Right) % NRMath.HalfCircle) - NRMath.Right;
				} 
				else 
				{
					distanceToWall = (int) hDist;
					angleToWall = (direction % NRMath.HalfCircle) - NRMath.Right;
				}
			}
		}
		private decimal abs(decimal n) 
		{
			return n > 0 ? n : -n;
		}
	}
}
