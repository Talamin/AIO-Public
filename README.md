### AIO-WOTLK

## Setting up the project

The external dependencies, such as wRobot assemblies, are stored in the `Dependencies` folder. However, this folder is intentionally left empty in order to allow each developer to setup the references using his own environment.

The wRobot assemblies used are the following:

- `MahApps.Metro.dll`
- `MemoryRobot.dll`
- `robotManager.dll`
- `UpdateManager.dll`
- `wManager.dll`

You may choose to copy these into the `Dependencies` folder, but this would require you to manually update them each time wRobot updates are released. Instead, we recommend [symbolic links](https://en.wikipedia.org/wiki/Symbolic_link).

You can setup symbolic links using the following PowerShell commands in the `Dependencies` folder:

```powershell
$wRobotBin = "<path to wRobot bin folder>"
Get-ChildItem $wRobotBin -Filter *.dll |
Foreach-Object {
    New-Item -ItemType SymbolicLink -Path $_.Name -Target $_.FullName
}
```

Don't forget to replace the path with the one on your system, i.e. `C:\Users\username\Desktop\WRobot\Bin`.

You can grab a copy of `MarsSettingsGUI.dll` from [here](https://github.com/Marsbars/SettingsGUI/raw/master/Compiled/MarsSettingsGUI.dll).

> Hint: You may symlink your output file to your wRobot `FightClass` folder in order to avoid extra build steps.
