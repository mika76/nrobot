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

namespace NRobot.CLI {
  using Robot = NRobot.Engine.Robot;
  public class NRobotCLI {
    public static void Main(string[] args) {
      Game game = Game.Create();
      game.Say += new Sayer(sayHandler);
      game.InitFromCommandLineArgs(args);
      game.Start();
      while (!game.Over) {
        game.Tick();
        string s = "";
        foreach (Robot robot in game.AliveBots) {
          if (s != "") s += " ";
          s += robot.Name + ":" + robot.Health;
        }
        s += "                                                                               ";
        s = s.Substring(0, 79);
        Console.Write(s + "\r");
        Console.Write("\r");
      }
      Console.WriteLine("");
      if (game.Winner == null) {
        Console.WriteLine("Tied game");
      } else {
        Console.WriteLine("Winning team is: " + game.Winner.Name);
      }
    }
    private static void sayHandler(Robot robot, string message) {
      Console.Write("                                                                               \r");
      Console.WriteLine((robot == null ? "GAME" : robot.Name) + ": " + message);
    }
  }
}
