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

  public class RobotTeam {
    public string OwnerEmail {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return team.OwnerEmail;
      }
    }
    public string Name {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return team.Name;
      }
    }
    public IList Robots {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return otherBots;
      }
    }
    internal ArrayList otherBots;
    public int ShotsPermitted {
      get {
        if (!robotState.IsActive) throw new ApplicationException("Cannot get information from an inactive state");
        return team.GameState.rules.TeamShotsPermitted - team.Bullets.Count;
      }
    }
    public object IdObject {
      get {return team.IdObject;}
    }
    internal RobotState robotState;
    internal Team team;
    internal RobotTeam(RobotState robotState, Team team) {
      this.robotState = robotState;
      this.team = team;
      otherBots = new ArrayList();
      foreach (Robot bot in team.Robots) {
        OtherBot ob;
        if (bot != robotState.robot && bot.Health > 0) {
          decimal xDist = bot.x - robotState.robot.x;
          decimal yDist = bot.y - robotState.robot.y;
          int angleFromSelf = NRMath.AngOff(NRMath.Atan2(xDist, yDist) - robotState.robot.CameraDirection);
          if (angleFromSelf >= -eighth && angleFromSelf <= eighth) {
            int distance = (int) Math.Sqrt((double) (xDist * xDist + yDist * yDist));
            ob = new VisibleBot(robotState, bot, this, angleFromSelf, distance);
            robotState.visibleBots.Add(ob);
          } else {
            ob = new OtherBot(robotState, bot, this);
          }
        } else {
          ob = new OtherBot(robotState, bot, this);
        }
        otherBots.Add(ob);
        robotState.botsById[bot.IdObject] = ob;
      }
    }
    private const int eighth = NRMath.FullCircle / 8;
  }
}
