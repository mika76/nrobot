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

	public class VisibleBot : OtherBot, IComparable 
	{
		public int AngleFromSelf 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return angleFromSelf;
			}
		}
		public int Distance 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return distance;
			}
		}
		// Relative to the opposite of our own angle, ie, if it's pointing at
		// us then Direction == -AngleFromSelf.
		public int Direction 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return direction;
			}
		}
		public int GunDirection 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return gunDirection;
			}
		}
		public int CameraDirection 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return cameraDirection;
			}
		}
		public int Radius 
		{
			get 
			{
				if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
				return otherBot.Radius;
			}
		}


		internal int angleFromSelf;
		internal int distance;
		internal int direction;
		internal int cameraDirection;
		internal int gunDirection;

		internal VisibleBot(RobotState robotState, Robot otherBot, RobotTeam otherTeam, int angleFromSelf, int distance)
			: base(robotState, otherBot, otherTeam) 
		{
			direction = NRMath.AngOff(otherBot.Direction + NRMath.HalfCircle -
				robotState.robot.CameraDirection);
			gunDirection = NRMath.AngOff(otherBot.GunDirection + NRMath.HalfCircle -
				robotState.robot.CameraDirection);
			cameraDirection = NRMath.AngOff(otherBot.CameraDirection +
				NRMath.HalfCircle -
				robotState.robot.CameraDirection);
			this.angleFromSelf = angleFromSelf;
			this.distance = distance;
		}

		private int abs(int x) 
		{
			return x >= 0 ? x : -x;
		}
		int IComparable.CompareTo(object o) 
		{
			VisibleBot vb = o as VisibleBot;
			int result = abs(angleFromSelf).CompareTo(abs(vb.angleFromSelf));
			if (result == 0) result = distance.CompareTo(abs(vb.distance));
			return result;
		}
	}
}