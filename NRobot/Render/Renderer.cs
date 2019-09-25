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

namespace NRobot.Render 
{

  using System;
  using System.Collections;
  using NRobot.Engine;
  using NRMath = NRobot.Robot.NRMath;

  public class Renderer {
    private Game game;
    private ICanvas canvas;
    private object grey;
    private object black;
    private int xorigin = 0;
    private int yorigin = 0;
    private int scalefrom;
    private int scaleto;

    public Renderer(Game game, ICanvas canvas) {
      this.game = game;
      this.canvas = canvas;
      grey = canvas.GetColor(new NRColor(0xaaaaaa));
      black = canvas.GetColor(new NRColor(0x000000));
      if (game.ArenaWidth / canvas.Width > game.ArenaHeight / canvas.Height) {
        scalefrom = game.ArenaWidth;
        scaleto = canvas.Width;
        yorigin = (canvas.Height - mapDist(game.ArenaHeight)) / 2;
      } else {
        scalefrom = game.ArenaHeight;
        scaleto = canvas.Height;
        xorigin = (canvas.Width - mapDist(game.ArenaWidth)) / 2;
      }
    }

    int mapX(int x) {
      return xorigin + mapDist(x);
    }
    int mapX(decimal x) {
      return xorigin + mapDist(x);
    }
    int mapY(int y) {
      return yorigin + mapDist(y);
    }
    int mapY(decimal y) {
      return yorigin + mapDist(y);
    }
    int mapDist(int dist) {
      return dist * scaleto / scalefrom;
    }
    int mapDist(decimal dist) {
      return (int) (dist * scaleto / scalefrom);
    }

    void DrawCircle(object color, int x, int y, int radius) {
      canvas.DrawCircle(color, mapX(x - radius), mapY(y - radius),
                        mapDist(2 * radius));
    }
    void DrawCircle(object color, int x, int y, int radius, int dir, int dist) {
      DrawCircle(color, x, y, radius, dir, dist, 0);
    }
    void DrawCircle(object color, int x, int y, int radius, int dir, int dist,
                           int perp) {
      int startX = mapX(x + NRMath.Sin(dir) * dist + NRMath.Sin(dir + NRMath.Right) * perp - radius);
      int startY = mapY(y + NRMath.Cos(dir) * dist + NRMath.Cos(dir + NRMath.Right) * perp - radius);
      canvas.DrawCircle(color, startX, startY, mapDist(2 * radius));
    }
    void DrawLinePolar(object color, int x, int y, int dir, int len, int thickness) {
      int offX = mapX(x + NRMath.Sin(dir) * len);
      int offY = mapY(y + NRMath.Cos(dir) * len);
      canvas.DrawLine(color, mapX(x), mapY(y), offX, offY, mapDist(thickness));
    }
    void DrawLinePolar(object color, int x, int y, int dir, int len, int dist,
                       int perp, int thickness) {
      int startX = mapX(x + NRMath.Sin(dir) * dist + NRMath.Sin(dir + NRMath.Right) * perp);
      int startY = mapY(y + NRMath.Cos(dir) * dist + NRMath.Cos(dir + NRMath.Right) * perp);
      int endX = mapX(x + NRMath.Sin(dir) * (dist + len) + NRMath.Sin(dir + NRMath.Right) * perp);
      int endY = mapY(y + NRMath.Cos(dir) * (dist + len) + NRMath.Cos(dir + NRMath.Right) * perp);
      canvas.DrawLine(color, startX, startY, endX, endY, mapDist(thickness));
    }

    private object col(NRColor color) {return canvas.GetColor(color);}

    void DrawRobot(Robot robot) {
      // Draw body
      DrawCircle(col(robot.BodyColor), robot.X, robot.Y, robot.Radius);

      // Draw wheels - four of them
      object wcol = col(robot.WheelColor);
      DrawLinePolar(wcol, robot.X, robot.Y, robot.Direction,
                    robot.Radius / 5, robot.Radius * 2 / 5,
                    robot.Radius * 2 / 5, 150);
      DrawLinePolar(wcol, robot.X, robot.Y, robot.Direction,
                    robot.Radius / 5, robot.Radius * 2 / 5,
                    -robot.Radius * 2 / 5, 150);
      DrawLinePolar(wcol, robot.X, robot.Y, robot.Direction,
                    robot.Radius / 5, -robot.Radius * 2 / 5,
                    robot.Radius * 3 / 5, 150);
      DrawLinePolar(wcol, robot.X, robot.Y, robot.Direction,
                    robot.Radius / 5, -robot.Radius * 2 / 5,
                    -robot.Radius * 3 / 5, 150);

      // Draw cameras - two of them
      object ccol = col(robot.CameraColor);
      DrawCircle(ccol, robot.X, robot.Y, 250, robot.CameraDirection,
                 robot.Radius, 250);
      DrawCircle(ccol, robot.X, robot.Y, 250, robot.CameraDirection,
                 robot.Radius, -250);

      // Draw lenses - two of them
      object lcol = col(robot.LensColor);
      DrawCircle(lcol, robot.X, robot.Y, 75, robot.CameraDirection,
                 robot.Radius + 175, 250);
      DrawCircle(lcol, robot.X, robot.Y, 75, robot.CameraDirection,
                 robot.Radius + 175, -250);

      // Draw gun
      DrawLinePolar(col(robot.GunColor), robot.X, robot.Y,
                    robot.GunDirection, robot.Radius * 4 / 5, 250);

      // Draw turret
      DrawCircle(col(robot.TurretColor), robot.X, robot.Y,
                 robot.Radius * 2 / 5);
    }
    void DrawBullet(Bullet bullet) {
      DrawCircle(col(bullet.Robot.BulletColor), bullet.X, bullet.Y, 100);
    }
    public void Render() {
      canvas.DrawRectangle(black, 0, 0, canvas.Width, canvas.Height);
      canvas.DrawRectangle(grey, xorigin, yorigin,
                           mapDist(game.ArenaWidth), mapDist(game.ArenaHeight));
      foreach (Robot robot in game.AliveBots) {
        DrawRobot(robot);
      }
      foreach (Bullet bullet in game.Bullets) {
        DrawBullet(bullet);
      }
    }
  }
}

