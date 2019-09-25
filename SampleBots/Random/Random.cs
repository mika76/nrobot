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

[assembly:RobotClass("NRobot.SampleBots.Random.RandomBot")]
namespace NRobot.SampleBots.Random {
  using Random = System.Random;

  [OwnerEmail("nrobot-list@gna.org")]
  [TeamInfo(Name="RandomTeam")]
  [RobotInfo(1, Name="RandomBot", // Make each color explicitly random rather
             BodyColor=-1,        // than using the defaults.
             TurretColor=-1,
             GunColor=-1,
             WheelColor=-1,
             CameraColor=-1,
             LensColor=-1,
             BulletColor=-1
             )]
  public class RandomBot : IRobot {
    public void Start(StartState state) {
      random = new Random();
      forwards = ((random.Next() % 2) == 0);
      clockwise = ((random.Next() % 2) == 0);
      turretClockwise = ((random.Next() % 2) == 0);
    }

    Random random;
    bool clockwise;
    bool turretClockwise;
    bool forwards;

    private int abs(int n) {
      return n > 0 ? n : -n;
    }

    // The randombot algorithm is to switch at random intervals between
    // forwards and backwards, and left and right, firing when a bot is close
    // to in the target area.
    // Dizzy and Random are pretty similar - the only difference is in the
    // fact that Dizzy keeps spinning in the same direction, where Random
    // switches back and forth.
    public void Tick(TickState state) {

      // If we've stopped turning, start again, alternating clockwise and
      // anti, for a random duration.
      if (state.TurnDuration == 0) {
        clockwise = !clockwise;
        state.TurnSpeed = state.MaxTurnSpeed;
        if (!clockwise) state.TurnSpeed = -state.TurnSpeed;
        state.TurnDuration = random.Next(10, 60);
      }

      // Do the same thing for our turret.
      if (state.TurretTurnDuration == 0) {
        turretClockwise = !turretClockwise;
        state.TurretTurnSpeed = state.MaxTurretTurnSpeed;
        if (!turretClockwise) state.TurretTurnSpeed = -state.TurretTurnSpeed;
        state.TurretTurnDuration = random.Next(10, 60);
      }

      // If we've stopped moving, start again, alternating forwards and
      // backwards, for a random duration.
      if (state.MoveDuration == 0) {
        forwards = !forwards;
        state.MoveSpeed = state.MaxMoveSpeed;
        if (!forwards) state.MoveSpeed = -state.MoveSpeed;
        state.MoveDuration = random.Next(10, 60);
      }

      // If an enemy bot is in visible range, shoot it
      if (state.VisibleBots.Count > 0) {
        VisibleBot vb = (VisibleBot) state.VisibleBots[0];
        if (vb.Team != state.Team &&
            abs(vb.AngleFromSelf) * vb.Distance < 250 * vb.Radius) {
          state.Fire();
        }
      }
    }
  }
}
