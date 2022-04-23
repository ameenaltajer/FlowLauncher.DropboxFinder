$PluginFolder = $env:APPDATA + "\FlowLauncher\Plugins\DropboxFinder"
$AllPluginFiles = $PluginFolder + "\*"
$FlowLauncherExecutable = "C:\Users\" + $env:USERNAME + "\AppData\Local\FlowLauncher\Flow.Launcher.exe"

echo $FlowLauncherExecutable

echo "Killing Flow Launcher.."
taskkill /im Flow.Launcher.exe /f

Start-Sleep -s 1

echo "Removing old plugin folder.."

Remove-Item  $AllPluginFiles -Recurse

Start-Sleep -s 1

echo "Copy new plugin folder.."
Copy-Item -Path "Flow.Launcher.Plugin.DropboxFinder\bin\Debug\*" -Destination $PluginFolder -Recurse

Start-Sleep -s 1

echo "Rebooting Flow Launcher.."
Start-Process -FilePath $FlowLauncherExecutable
