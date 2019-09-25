@echo off
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: NRobot - Autonomous robot fighting game
:: Copyright (c) 2004,2005 Stuart Ballard
::
::  This program is free software; you can redistribute it and/or modify
::  it under the terms of the GNU General Public License as published by
::  the Free Software Foundation; either version 2 of the License, or
::  (at your option) any later version.
::
::  This program is distributed in the hope that it will be useful,
::  but WITHOUT ANY WARRANTY; without even the implied warranty of
::  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
::  GNU General Public License for more details.
::
::  You should have received a copy of the GNU General Public License
::  along with this program; if not, write to the Free Software
::  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
::
:: The GNU General Public License should be located in the file COPYING.
::
:: For more information about NRobot, please contact nrobot-list@gna.org or
:: write to Stuart Ballard at NetReach Inc, 124 S Maple Street, Ambler,
:: PA  19002  USA.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

csc /target:library /r:..\NRobot\bin\Debug\NRobot.dll /out:..\Bots\Win\Dizzy.dll Dizzy\Dizzy.cs
csc /target:library /r:..\NRobot\bin\Debug\NRobot.dll /out:..\Bots\Win\Dizzy2.dll Dizzy\Dizzy.cs
csc /target:library /r:..\NRobot\bin\Debug\NRobot.dll /out:..\Bots\Win\Follower.dll Follower\Follower.cs
csc /target:library /r:..\NRobot\bin\Debug\NRobot.dll /out:..\Bots\Win\Random.dll Random\Random.cs
csc /target:library /r:..\NRobot\bin\Debug\NRobot.dll /out:..\Bots\Win\Random2.dll Random\Random.cs

echo This script does not compile the Latte bot which is written in Java. Use the
echo separate build script in the Latte folder to compile that.