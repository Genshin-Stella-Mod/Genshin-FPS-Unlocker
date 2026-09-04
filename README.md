# Genshin FPS Unlocker
This is a fork providing better compatibility with [Genshin Stella Mod](https://stella.sefinek.net). If you want to use ReShade + FPS Unlocker at the same time, use Stella Mod instead.

> [!IMPORTANT]
> Windows Security may show the following warning when you run the program: *"Part of this app has been blocked. Some features of Genshin FPS Unlock may not work because we can't confirm who published Genshin FPS Unlock.dll that the app tried to load."*
> This is expected - the compiled exe and DLL aren't digitally signed (no certificate), so **Smart App Control** can't verify their publisher and blocks part of the app's functionality.
> To fix this, disable the feature: `Windows Security` → `App & browser control` → `Smart App Control` → `Off`.

> [!IMPORTANT]
> This version defaults fullscreen to `Exclusive` instead of `Borderless` for better compatibility with ReShade scaling and for monitors with a resolution higher than FHD. If you have an FHD monitor, you can safely use `Borderless` - there shouldn't be any issues.
> It's also recommended to disable the `Optimizations for windowed games` option in Windows graphics settings, as in rare cases it can cause various issues.

## Information
- This tool helps you to unlock the 60 FPS limit in the game.
- This is an external program which uses **WriteProcessMemory** to write the desired fps to the game.
- Handle protection bypass is already included.
- Does not require a driver for R/W access.
- Supports OS and CN version.
- Should work for future updates.
- You can download the compiled binary over at [Release](https://github.com/Genshin-Stella-Mod/Genshin-FPS-Unlocker/releases) if you don't want to compile it yourself.

## Usage
- Make sure you have the [.NET Desktop Runtime 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Usually it should come installed.
- Run the exe and click `Start game`.
- If it is your first time running, unlocker will attempt to find your game through the registry. If it fails, then it will ask you to either browse or run the game.
- Place the compiled exe anywhere you want (except for the game folder).
- Make sure your game is closed - the unlocker will automatically start the game for you.
- Run the exe as administrator, and leave the exe running.
> It requires administrator because the game needs to be started by the unlocker and the game requires such permission.

## Notes
- HoYoverse (miHoYo) is well aware of this tool, and you will not get banned for using FPS unlock.
- If you are using other third-party plugins, you are doing it at your own risk.
- Any artifacts from unlocking fps (e.g. stuttering) is NOT a bug of the unlocker.

## Compiling
Use `Visual Studio 2026 Community Edition` to compile.
