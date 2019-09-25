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

namespace NRobot.Robot {
  using Robot = NRobot.Engine.Robot;

  public class OtherBot {
    public string Name {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return otherBot.Name;
      }
    }
    public RobotTeam Team {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return otherTeam;
      }
    }
    public int Health {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return otherBot.Health;
      }
    }

    public IRobot GetRobot(RobotState state) {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
      if (state != robotState) throw new ApplicationException("Must use current state object to find robot implementations");
      if (state.robot.Team != otherBot.Team) throw new ApplicationException("Cannot get implementation of another team's bot");
      return otherBot.iRobot;
    }
    public object IdObject {get {return otherBot.IdObject;}}
    internal RobotState robotState;
    internal Robot otherBot;
    internal RobotTeam otherTeam;
    internal OtherBot(RobotState robotState, Robot otherBot, RobotTeam otherTeam) {
      this.robotState = robotState;
      this.otherBot = otherBot;
      this.otherTeam = otherTeam;
    }
  }
}

