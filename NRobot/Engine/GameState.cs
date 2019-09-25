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

	/// <summary>Represents a game, with a particular set of rules</summary>
	// This class provides the main entry point used by a GUI to create and
	// manage a new game. It also provides the bulk of the implementation of
	// the game itself.
	[Serializable]
	internal class GameState 
	{
		internal ArrayList sayMessages = new ArrayList();

		internal bool started = false;

		internal ArrayList teams = new ArrayList();

		internal ArrayList robots = null;

		internal ArrayList aliveBots = null;

		internal ArrayList bullets = null;

		internal bool over = false;

		internal Team winner = null;

		// This has a value if you're on the main side of the appdomain barrier,
		// but not if you're on the arena side.
		[NonSerialized]
		internal Game game = null;

		// This has a value if you're on the arena side of the appdomain barrier,
		// but not on the other side.
		[NonSerialized]
		internal GameArena arena = null;

		private static readonly bool inArenaDomain = AppDomain.CurrentDomain.FriendlyName == "NRobotArena";

		/// <summary>Get the rules of this game, regardless of which side of the barrier we're on.</summary>
		internal GameRules rules 
		{
			get 
			{
				return inArenaDomain ? arena.rules : game.rules;
			}
		}
	}
}