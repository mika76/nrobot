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
using NRobot.Robot;

namespace NRobot.Engine 
{

	[Serializable]
	public struct NRColor 
	{

		private static Random random = new Random();

		private const int white = 0xffffff;
		private const int red   = 0xff0000;
		private const int green = 0x00ff00;
		private const int blue  = 0x0000ff;
		private int color;
		public NRColor(int color) 
		{
			if ((color & white) != color) color = random.Next(white + 1);
			this.color = color;
		}
		public byte Red   {get {return (byte) ((color & red) >> 16);}}
		public byte Green {get {return (byte) ((color & green) >> 8);}}
		public byte Blue  {get {return (byte)  (color & blue);}}

		public static explicit operator NRColor(int color) 
		{
			return new NRColor(color);
		}
		public static explicit operator int(NRColor color) 
		{
			return color.color;
		}
		public override bool Equals(object o) 
		{
			if (!(o is NRColor)) return false;
			return ((NRColor) o).color == color;
		}
		public override int GetHashCode() 
		{
			return color.GetHashCode();
		}
	}
}

