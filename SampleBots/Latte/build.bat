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

if exist "%JAVA_HOME%\lib\rt.jar" goto jok
echo Must set JAVA_HOME to a valid jre installation.
exit /b 1
:jok

if exist "%IKVM_HOME%\bin\ikvmc.exe" goto iok
echo Must set IKVM_HOME to a valid IKVM installation.
exit /b 1
:iok

if exist org\gna\nrobot\samplebots\latte\LatteBot.java goto dok
echo This build script must be run from the Latte folder.
exit /b 1
:dok

copy ..\..\NRobot\bin\debug\NRobot.dll .
"%IKVM_HOME%\bin\ikvmstub" NRobot.dll
"%IKVM_HOME%\bin\ikvmstub" mscorlib
copy "%IKVM_HOME%\bin\IKVM.GNU.Classpath.dll" .
jikes -classpath "%JAVA_HOME%\lib\rt.jar;mscorlib.jar;NRobot.jar" org\gna\nrobot\samplebots\latte\LatteBot.java

copy NRobot.dll "%IKVM_HOME%\bin"
"%IKVM_HOME%\bin\ikvmc" -remap:lattebot-map.xml -target:library -out:..\..\Bots\Win\Latte.dll -r:NRobot.dll -r:IKVM.GNU.Classpath.dll -recurse:org
del "%IKVM_HOME%\bin\NRobot.dll"
copy "%IKVM_HOME%\bin\IKVM.GNU.Classpath.dll" ..\..\NRobotWin\bin\Debug
copy "%IKVM_HOME%\bin\IKVM.Runtime.dll" ..\..\NRobotWin\bin\Debug
echo WARNING: Currently robots written in Java, such as LatteBot, require the
echo -insecure option to be passed to NRobotWin.
echo DO NOT set this option if there are any robots whose authors you do not trust!