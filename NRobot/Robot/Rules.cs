#region Copyright and License
///////////////////////////////////////////////////////////////////////////////
// NRobot - Autonomous robot fighting game
// Copyright (c) 2004 Stuart Ballard
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
// For more information about NRobot, please contact nrobot-list@nongnu.org or
// write to Stuart Ballard at NetReach Inc, 124 S Maple Street, Ambler,
// PA  19002  USA.
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using NRobot.Engine;

namespace NRobot.Robot {
  using Robot = NRobot.Engine.Robot;

  public class Rules {
    internal Game game;
    internal Rules(Game game) {
      this.game = game;
    }
    public int TeamSize {get {return game.TeamSize;}}
    public int StartHealth {get {return game.StartHealth;}}
    public int RobotRadius {get {return game.RobotRadius;}}
    public int BulletSpeed {get {return game.BulletSpeed;}}
    public int BulletDamage {get {return game.BulletDamage;}}
    public int ShotDelay {get {return game.ShotDelay;}}
    public int MaxMoveSpeed {get {return game.MaxMoveSpeed;}}
    public int MaxTurnSpeed {get {return game.MaxTurnSpeed;}}
    public int MaxTurretTurnSpeed {get {return game.MaxTurretTurnSpeed;}}
    public int TeamShotsPermitted {get {return game.TeamShotsPermitted;}}
    public int BotShotsPermitted {get {return game.BotShotsPermitted;}}
  }
}
