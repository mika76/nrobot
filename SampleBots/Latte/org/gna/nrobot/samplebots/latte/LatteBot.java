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

package org.gna.nrobot.samplebots.latte;
import cli.NRobot.Robot.*;
import cli.System.*;
import cli.System.Collections.*;

public class LatteBot implements IRobot {
  public void Start(StartState state) {
  }

  private int abs(int n) {
    return n > 0 ? n : -n;
  }

  private int moveJitter;
  private int turnJitter;

  private int getMoveJitter(TickState state) {
    return state.get_MaxMoveSpeed() * (abs(moveJitter - 1) - 1);
  }
  private int getTurnJitter(TickState state) {
    return state.get_MaxTurnSpeed() * (abs(turnJitter - 2) - 2);
  }

  // LatteBot is essentially "FollowerBot with the jitters" (as befits
  // a bot written in Java). It spins counterclockwise until at least one
  // bot is visible, and then points back and forth firing a spread of
  // bullets to the left and right of the target while also maintaining a
  // distance that oscillates around 3 * RobotRadius.
  public void Tick(TickState state) {

    moveJitter = (moveJitter + 1) % 4;
    turnJitter = (turnJitter + 1) % 7;

    // If an enemy bot is in firing range, shoot it
    if (state.get_VisibleBots().get_Count() > 0) {
      VisibleBot vb = (VisibleBot) state.get_VisibleBots().get_Item(0);
      if (vb.get_Team() != state.get_Team() &&
          abs(vb.get_AngleFromSelf()) * vb.get_Distance() < 250 * vb.get_Radius()) {
        state.Fire();
      }
    }

    // See if any enemy bots at all are visible; if so find the first one.
    // Notice the use of CLR-style enumeration - IKVM does not (yet)
    // transparently map this to the Java style iterator()/hasNext()/next()
    // pattern. Even if we were using 1.5 syntax, we can't use Java's
    // foreach equivalent either.
    VisibleBot firstEnemy = null;
    int dist_times_angle = Integer.MAX_VALUE;
    for (IEnumerator e = state.get_VisibleBots().GetEnumerator(); e.MoveNext(); ) {
      VisibleBot vb = (VisibleBot) e.get_Current();
      if (vb.get_Team() != state.get_Team()) {
        int dta = vb.get_Distance() * abs(vb.get_AngleFromSelf() + 5);
        if (dta < dist_times_angle) {
          firstEnemy = vb;
          dist_times_angle = dta;
        }
        break;
      }
    }

    // If there is an enemy bot visible, try to aim exactly towards it and
    // maintain a distance of exactly 6*RobotRadius.
    if (firstEnemy != null) {

      // These will be auto-capped at MaxTurnSpeed and MaxMoveSpeed if
      // they're out of range.
      state.set_TurnSpeed(firstEnemy.get_AngleFromSelf() + getTurnJitter(state));
      state.set_MoveSpeed(firstEnemy.get_Distance() - 3 * firstEnemy.get_Radius() + getMoveJitter(state));

    // If no enemies are in range, spin counterclockwise at top speed to try to
    // find some.
    } else {
      state.set_TurnSpeed(-state.get_MaxTurnSpeed());
      state.set_MoveSpeed(getMoveJitter(state));
    }
  }
}
