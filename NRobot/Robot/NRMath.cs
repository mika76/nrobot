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

namespace NRobot.Robot {

  public class NRMath {

    public const int FullCircle = 1000;
    public const int Right = FullCircle / 4;
    public const int HalfCircle = FullCircle / 2;
    public const int Right3 = Right * 3;

    private static readonly decimal[] sin;

    static NRMath() {
      sin = new decimal[Right + 1];
      for (int i = 0; i <= Right; i++) {
        sin[i] = (decimal) Math.Sin(2 * i * Math.PI / FullCircle);
      }
    }

    private NRMath() {} // Can't be constructed.

    // Cos and Sin take advantage of symmetry to only keep 90 degrees worth of
    // Sin values calculated. All the other segments can be figured out quickly
    // from these.
    public static decimal Cos(int i) {
      i = Angle(i);
      if (i < Right) return sin[Right - i];
      else if (i < HalfCircle) return -sin[i - Right];
      else if (i < Right3) return -sin[Right3 - i];
      else return sin[i - Right3];
    }
    public static decimal Sin(int i) {
      i = Angle(i);
      if (i < Right) return sin[i];
      else if (i < HalfCircle) return sin[HalfCircle - i];
      else if (i < Right3) return -sin[i - HalfCircle];
      else return -sin[FullCircle - i];
    }
    public static int Atan2(decimal a, decimal b) {
      return (int) (Math.Atan2((double)a, (double)b) * (FullCircle / 2) / Math.PI);
    }

    /// <summary>Normalizes a value into the range 0 - FullCircle-1</summary>
    public static int Angle(int a) {
      while (a < 0) a += FullCircle;
      return a % FullCircle;
    }
    /// <summary>Normalizes a value into the range -HalfCircle - HalfCircle-1</summary>
    public static int AngOff(int a) {
      return Angle(a + HalfCircle) - HalfCircle;
    }
  }
}
