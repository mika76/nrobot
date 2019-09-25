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
using NRobot.Robot;

[assembly:RobotClass("NRobot.SampleBots.Follower.FollowerBot")]
namespace NRobot.SampleBots.Follower {

  [OwnerEmail("nrobot-list@gna.org")]
  [TeamInfo(Name="FollowerTeam", BodyColor=0xaaccff,
    // Follower pursues an entirely offensive strategy. All offensive parameters are maxed out at the expense of all the
    // defensive ones.
    BulletDamage=TradeOff.Better, BulletSpeed=TradeOff.Better, ShotsPermitted=TradeOff.Better, ShotDelay=TradeOff.Better,
    MaxMoveSpeed=TradeOff.Worse, MaxTurnSpeed=TradeOff.Worse, MaxTurretTurnSpeed=TradeOff.Worse, Size=TradeOff.Worse)]
  [RobotInfo(1, Name="FollowerBot")]
  public class FollowerBot : IRobot {
    public void Start(StartState state) {
      // Nothing much to do: this bot is stateless so there's nothing to
      // initialize...
    }

    private int abs(int n) {
      return n > 0 ? n : -n;
    }

    // The follower algorithm is to spin clockwise until at least one bot is
    // visible, then aim to point exactly towards that bot at all times,
    // maintaining a distance of exactly 6*RobotRadius.
    public void Tick(TickState state) {

      // If an enemy bot is in firing range, shoot it
      if (state.VisibleBots.Count > 0) {
        VisibleBot vb = (VisibleBot) state.VisibleBots[0];
        if (vb.Team != state.Team &&
            abs(vb.AngleFromSelf) * vb.Distance < 250 * vb.Radius) {
          state.Fire();
        }
      }

      // See if any enemy bots at all are visible; if so find the first one.
      VisibleBot firstEnemy = null;
      foreach (VisibleBot vb in state.VisibleBots) {
        if (vb.Team != state.Team) {
          firstEnemy = vb;
          break;
        }
      }

      // If there is an enemy bot visible, try to aim exactly towards it and
      // maintain a distance of exactly 6*RobotRadius.
      if (firstEnemy != null) {

        // These will be auto-capped at MaxTurnSpeed and MaxMoveSpeed if
        // they're out of range.
        state.TurnSpeed = firstEnemy.AngleFromSelf;
        state.MoveSpeed = firstEnemy.Distance - 6 * firstEnemy.Radius;

      // If no enemies are in range, spin clockwise at top speed to try to
      // find some.
      } else {
        state.TurnSpeed = state.MaxTurnSpeed;
      }
    }
  }
}
