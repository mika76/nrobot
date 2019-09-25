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
using System.IO;
using NRobot.Robot;

namespace NRobot.Engine 
{
	[Serializable]
	internal class GameRules
	{
		internal int TeamSize = 1;

		internal int StartHealth = 100;

		internal int RobotRadius = 1000;

		internal int BulletSpeed = 500;

		internal int BulletDamage = 5;

		internal int ShotDelay = 5;

		internal int ArenaWidth = 30000;

		internal int ArenaHeight = 20000;

		internal int MaxMoveSpeed = 100;

		internal int MaxTurnSpeed = 10;

		internal int MaxTurretTurnSpeed = 15;

		private int teamShotsPermitted = 0;
		internal int TeamShotsPermitted 
		{
			get 
			{
				if (teamShotsPermitted == 0) 
				{
					// This is a function which ranges from BotShotsPermitted * TeamSize
					// (when TeamSize = 1) to BotShotsPermitted * TeamSize / 2
					// (as TeamSize approaches infinity).
					return (int) (BotShotsPermitted * TeamSize * ((TeamSize + 19.0m)/(TeamSize + 9.0m)) / 2);				} 
				else 
				{
					return teamShotsPermitted;
				}
			}
			set {teamShotsPermitted = value;}
		}

		internal int BotShotsPermitted = 5;
	}
}