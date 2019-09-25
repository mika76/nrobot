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

// Partially based on Scribble.cs (c) 2002 Rachel Hestilow

namespace NRobot.GTK {

  using GLib;
  using Gtk;
  using Gdk;
  using GtkSharp;
  using System;
  using System.Collections;
  using NRobot.Engine;
  using NRobot.Render;
  using GdkGC = Gdk.GC;

  public class NRobotGTK {
    private static Gtk.DrawingArea darea;
    private static Gdk.Pixmap pixmap = null;
    private static Game game = null;
    private static bool started = false;
    private static GTKCanvas canvas = null;
    private static Renderer renderer = null;

    public class GTKCanvas : ICanvas {
      Rectangle allocation;
      public GTKCanvas() {
        allocation = darea.Allocation;
      }
      public int Width {get {return allocation.Width;}}
      public int Height {get {return allocation.Height;}}
      public void DrawRectangle(object color, int x, int y,
                                int width, int height) {
        GdkGC gc = (GdkGC) color;
        pixmap.DrawRectangle(gc, true, x, y, width, height);
      }
      public void DrawCircle(object color, int startx, int starty,
                             int diameter) {
        GdkGC gc = (GdkGC) color;
        pixmap.DrawArc(gc, true, startx, starty, diameter, diameter, 0, 360*64);
      }
      public void DrawLine(object color, int x1, int y1, int x2, int y2,
                           int thickness) {
        GdkGC gc = (GdkGC) color;
        gc.SetLineAttributes(thickness, LineStyle.Solid, CapStyle.Round,
                             JoinStyle.Round);
        pixmap.DrawLine(gc, x1, y1, x2, y2);
      }
      public object GetColor(NRColor color) {
        GdkGC gc = (GdkGC) colors[color];
        if (gc == null) {
          gc = new GdkGC(pixmap);
          Color col = new Color(color.Red, color.Green, color.Blue);
          Colormap.System.AllocColor(ref col, true, true);
          gc.Foreground = col;
          colors[color] = gc;
        }
        return gc;
      }
      private Hashtable colors = new Hashtable();
    }

    private static void sayHandler(Robot robot, string message) {
      Console.Write("                                                                               \r");
      Console.WriteLine((robot == null ? "GAME" : robot.Name) + ": " + message);
    }

    public static int Main (string[] args) {
      game = Game.Create();
      game.Say += new Sayer(sayHandler);
      game.InitFromCommandLineArgs(args);
      game.Start();

      Application.Init();
      Gtk.Window win = new Gtk.Window("NRobot GTK Client");
      win.DeleteEvent += new DeleteEventHandler(Window_Delete);

      darea = new Gtk.DrawingArea();
      darea.SetSizeRequest(game.ArenaWidth / 50, game.ArenaHeight / 50);
      win.Add(darea);
      
      darea.ExposeEvent += new ExposeEventHandler(ExposeEvent);
      darea.ConfigureEvent += new ConfigureEventHandler(ConfigureEvent);
      darea.ButtonPressEvent += new ButtonPressEventHandler(ButtonPressEvent);
      darea.Events = EventMask.ExposureMask |
                     EventMask.ButtonPressMask;

      win.ShowAll();
      DrawArena();
      Application.Run();
      return 0;
    }

    static void Window_Delete (object obj, DeleteEventArgs args) {
      Application.Quit();
      args.RetVal = true;
    }

    static void ExposeEvent (object obj, ExposeEventArgs args) {
      Gdk.EventExpose ev = args.Event;
      Gdk.Window window = ev.Window;
      Gdk.Rectangle area = ev.Area;

      window.DrawDrawable(darea.Style.BlackGC, pixmap,
                          area.X, area.Y, area.X, area.Y,
                          area.Width, area.Height);

      args.RetVal = false;
    }
    
    static void ConfigureEvent (object obj, ConfigureEventArgs args) {
      Gdk.EventConfigure ev = args.Event;
      Gdk.Window window = ev.Window;
      Gdk.Rectangle allocation = darea.Allocation;

      pixmap = new Gdk.Pixmap (window, allocation.Width, allocation.Height, -1);
      canvas = new GTKCanvas();
      renderer = new Renderer(game, canvas);
      renderer.Render();
      args.RetVal = true;
    }

    static void DrawArena() {
      if (renderer != null) {
        renderer.Render();
        darea.QueueDrawArea(0, 0, canvas.Width, canvas.Height);
      }
      string s = "";
      foreach (Robot robot in game.AliveBots) {
        if (s != "") s += " ";
        s += robot.Name + ":" + robot.Health;
      }
      s += "                                                                               ";
      s = s.Substring(0, 79);
      Console.Write(s + "\r");
      if (game.Over) {
        Console.WriteLine();
        Console.WriteLine(game.Winner == null ? "Tied game" : "Winner: " + game.Winner.Name);
      }
    }
  
    static void ButtonPressEvent(object obj, ButtonPressEventArgs args) {
      if (!started && pixmap != null) {
        GLib.Timeout.Add (40, new TimeoutHandler(Tick));
        started = true;
      }

      args.RetVal = true;
    }

    static bool Tick() {
      game.Tick();
      DrawArena();
      return !game.Over;
    }
  }
}

