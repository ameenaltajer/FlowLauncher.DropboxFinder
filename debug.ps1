$PluginFolder = $env:APPDATA + "\FlowLauncher\Plugins\DropboxFinder"
$AllPluginFiles = $PluginFolder + "\*"
$FlowLauncherExecutable = "C:\Users\" + $env:USERNAME + "\AppData\Local\FlowLauncher\Flow.Launcher.exe"

echo $FlowLauncherExecutable

echo "Building the debug package.."
dotnet publish Flow.Launcher.Plugin.DropboxFinder -c Debug -r win-x64

echo "Killing Flow Launcher.."
taskkill /im Flow.Launcher.exe /f

Start-Sleep -s 1

echo "Removing old plugin folder.."

Remove-Item  $AllPluginFiles -Recurse

Start-Sleep -s 1

echo "Copy new plugin folder.."
Copy-Item -Path "Flow.Launcher.Plugin.DropboxFinder\bin\Debug\win-x64\publish\*" -Destination $PluginFolder -Recurse

echo "Copy images to the new plugin folder.."
Copy-Item -Path "Flow.Launcher.Plugin.DropboxFinder\*.png" -Destination $PluginFolder -Recurse

Start-Sleep -s 1

echo "Rebooting Flow Launcher.."
Start-Process -FilePath $FlowLauncherExecutable
