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
	public class Team 
	{

		// Make the constructor internal so this can't be instantiated from
		// elsewhere
		internal Team(GameState gameState, string dllPath) 
		{
			this.gameState = gameState;
			this.dllPath = dllPath;
		}

		internal string dllPath;

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

		[NonSerialized]
		private object idObject;
		public object IdObject {get {if (idObject == null) idObject = new object(); return idObject;}}

		private GameState gameState;
		internal GameState GameState {get {return gameState;}}

		public Game Game {get {return gameState.game;}}

		internal string name;

		// Can be changed up until game start
		public string Name 
		{
			get {return name;}
			set 
			{
				if (gameState.started) throw new ApplicationException("Cannot change team name after game start");
				name = value;
			}
		}

		internal string ownerEmail;

		// Can be changed up until game start
		public string OwnerEmail 
		{
			get {return ownerEmail;}
			set 
			{
				if (gameState.started) throw new ApplicationException("Cannot change owner name after game start");
				ownerEmail = value;
			}
		}

		internal ArrayList robots;

		public IList Robots 
		{
			get 
			{
				if (!gameState.started) throw new ApplicationException("Cannot get robot list until game start");
				return robots;
			}
		}

		internal ArrayList aliveBots;

		public IList AliveBots 
		{
			get 
			{
				if (!gameState.started) throw new ApplicationException("Cannot get alive bot list until game start");
				return aliveBots;
			}
		}

		internal ArrayList bullets;

		public IList Bullets 
		{
			get 
			{
				if (!gameState.started) throw new ApplicationException("Cannot get bullet list until game start");
				return bullets;
			}
		}

		public int RobotsAlive 
		{
			get 
			{
				return gameState.started ? aliveBots.Count : gameState.rules.TeamSize;
			}
		}

		public int TotalHealth 
		{
			get 
			{
				if (!gameState.started) return gameState.rules.TeamSize * gameState.rules.StartHealth;

				int result = 0;
				foreach (Robot robot in aliveBots) 
				{
					result += robot.health;
				}
				return result;
			}
		}
	}
}