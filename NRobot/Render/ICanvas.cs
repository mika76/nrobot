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

  public interface ICanvas {
    int Width {get;}
    int Height {get;}
    void DrawRectangle(object color, int x, int y, int width, int height);
    void DrawCircle(object color, int startx, int starty, int diameter);
    void DrawLine(object color, int x1, int y1, int x2, int y2, int thickness);
    object GetColor(NRColor color);
  }
}
