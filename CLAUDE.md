# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A fork of an FPS-unlock tool for Genshin Impact, maintained for compatibility with [Genshin Stella Mod](https://stella.sefinek.net) (a separate, larger repo: `Genshin-Impact-ReShade`). It has two components in one solution:

- **`unlockfps_nc/`** - C# .NET 10 WinForms app (`Genshin FPS Unlocker.csproj`). The launcher UI: finds/starts the game, manages `unlocker.config.json`, and drives the actual unlock via IPC.
- **`UnlockerStub/`** - native C++ DLL (`UnlockerStub.vcxproj`), built as `x64`. Gets injected into the running game process and does the actual memory patching. Its compiled output is checked into `unlockfps_nc/Resources/UnlockerStub.dll` and embedded as a resource in the C# app (`unlockfps_nc.Resources.UnlockerStub.dll`) - **if you change `UnlockerStub`, you must rebuild it and drop the new `UnlockerStub.dll` into `unlockfps_nc/Resources/` before the C# app will pick it up.**

Both projects live in `unlockfps_nc.slnx` (open via `OPEN THE SOLUTION.cmd` or Visual Studio 2026).

## Build & run

- Open the whole solution: `unlockfps_nc.slnx` in Visual Studio 2026.
- Build/check the C# app only: `dotnet build "unlockfps_nc/Genshin FPS Unlocker.csproj" -c Debug` (or `Release`) from the repo root.
- **Never launch the built app yourself** (`dotnet run`, running the exe, etc.) to test changes - it requires administrator elevation and will spawn/attach to a live Genshin Impact process. Ask the user to run it if manual verification is needed.
- Full release build (both projects + packaging): `Build.cmd`, Windows-only, requires `7z` on PATH. It publishes self-contained and framework-dependent `win-x64` builds, zips them into `Upload/`, and regenerates `Upload/CHECKSUMS.md`. It expects `UnlockerStub.dll` to already be built and present under `unlockfps_nc\Resources\`.
- `UnlockerStub` normally builds via MSBuild/Visual Studio (`UnlockerStub.vcxproj`, x64). A `GNUmakefile` also exists for a mingw-w64 cross-build (`x86_64-w64-mingw32-g++`), but it only compiles `dllmain.cpp` - it doesn't include `Utils.cpp`/`Zydis.c`, so treat it as a secondary/incomplete path, not the source of truth.
- **No test suite and no CI build pipeline** exist in this repo (`.github/workflows/auto-assign.yml` only auto-assigns issues/PRs). Verify changes with `dotnet build` and manual reasoning; there's nothing to "run the tests" with.
- Release builds are strong-name signed via `<AssemblyOriginatorKeyFile>D:\Projects\stella\SignAssembly\FPS-Unlock.snk</AssemblyOriginatorKeyFile>` (absolute path outside this repo, `unlockfps_nc.csproj`). A machine without that key file/path will fail a `Release` build (Debug builds are unaffected in practice since signing still applies, so watch for this if a `Release` build fails somewhere unusual).

## Architecture: how the unlock actually works

This is the part that isn't obvious from any single file - it spans `unlockfps_nc/Service/IpcService.cs` and `UnlockerStub/dllmain.cpp`:

1. **Injection is a classic global hook, not `CreateRemoteThread`/manual mapping.** `IpcService.StartAsync` calls `LoadLibrary` on `UnlockerStub.dll` **in the launcher's own process** (just to get a valid `HMODULE` + the address of its exported `WndProc`), finds the game's window/thread, then calls `SetWindowsHookEx(WH_GETMESSAGE, ..., hMod: thatModule, dwThreadId: gameThreadId)` and immediately fires a `PostThreadMessage` at that thread. Windows itself injects the DLL into the *game* process as a side effect of installing a hook with a module handle targeting another process's thread; the dummy posted message is what makes the hook (and thus `DllMain`) actually run inside the game.
2. **Communication is a fixed-name shared memory block**, not sockets/pipes: `Global\2DE95FDC-6AB7-4593-BFE6-760DD4AB422B`, a small `IpcData`/`IPCData` struct (status, target framerate, power-save flag, mobile-UI flag). The layout is duplicated by hand on both sides (`IpcService.cs` and `dllmain.cpp`) - **if you change one side's struct, you must mirror the change on the other side and keep field order/size/`Pack`/`align` identical**, or IPC silently desyncs.
3. **Inside the game process** (`dllmain.cpp::ThreadProc`), the stub locates the game's `il2cpp` PE section and byte-pattern-scans it (via the vendored Zydis disassembler, `Zydis.c`/`Zydis.h`) to find the live frame-rate variable's address. This is why unlock can break on game updates - the byte patterns are Genshin-version-specific and surface as an "outdated pattern" error box when they no longer match.
4. **The FPS unlock is a live poll loop**, not a one-shot patch: every 62ms the stub reads the desired FPS from shared memory and writes it directly to the discovered game-memory address, clamped to `[10, 1000]`. Power-save mode force-clamps to 10 FPS while the game window isn't foreground. The C# side polls/writes to the same shared memory on the same 62ms cadence (`ProcessService.UnlockerPoll`), and pushes config changes into it via `IpcService.Update()`.
5. Mobile-UI toggling (`UseMobileUI`) is a separate, self-removing x64 hook: `InstallHook` builds a trampoline over a pattern-scanned function, `HookProc` runs once (using Zydis to walk instructions and find a register offset), flips the target, then uninstalls itself and restores the original bytes.

## Config & monitor handling

- Config is a flat JSON file next to the exe (`unlocker.config.json`, model in `Model/Config.cs`), owned by `Service/ConfigService.cs` (load → `Sanitize()` clamps → registry game-path fallback → first-run init or version migration → `Save()`).
- Monitors are identified by a **stable device ID** (`MonitorUtils.GetDeviceId`, derived from `EnumDisplayDevices`/registry EDID), not by list position - list order can change when displays are unplugged/reconnected or a docking station is used. `MonitorUtils.ResolveMonitorIndex` matches by ID first, then falls back to the stored `MonitorNum` position, then to index 0. `Screen[]` arrays are always ordered primary-first via `MonitorUtils.GetOrderedScreens()`.
- An empty `MonitorId` means "let the game pick" (shown in Settings as `"-- Not selected --"`); `ProcessService.BuildCommandLine` only appends `-monitor <n>` when `MonitorId` is set.
- When the previously-bound monitor is no longer connected, `MainForm.BtnStartGame_Click` intentionally re-syncs `MonitorId` **and** `FPSTarget`/`CustomResX`/`CustomResY` to the fallback monitor (not just the ID) - this is a deliberate, documented product behavior (see `stella.sefinek.net` changelog, v8.11.1.0), not a bug, even though it looks like scope creep in a diff.

## Localization

- 18 languages via per-form/per-language `.resx` files (e.g. `Forms/SettingsForm.pl.resx`). Managed externally through the [Stella-Mod-Translations](https://github.com/sefinek/Stella-Mod-Translations) repo, edited locally with the ResXManager VS extension (`ResXManager.config.xml`).
- Startup language resolution lives in `Program.cs` (`SupportedLangs` + `ResolveFallbackLanguage`), persisted to `%AppData%/Genshin Stella Mod/settings.ini` via the hand-rolled `Service/IniFile.cs` (P/Invoke `Get/WritePrivateProfileString`, not `System.Configuration`).
- The sibling `Genshin-Impact-ReShade` repo has its own, near-identical language-resolution logic centralized in `Stella.Utils/LanguageResolver.cs` (shared across its Launcher/Configuration/Welcome/Prepare apps). The two implementations are **not** shared code - a bug fixed in one (e.g. Traditional-vs-Simplified Chinese fallback) needs to be checked/ported in the other by hand.

## Style conventions (from `.editorconfig`)

- Tabs for indentation; Allman brace style (opening brace on its own line).
- Private fields: `_camelCase`. `var` when the type is apparent from the right-hand side.
- User-facing strings go through `Properties/Resources.resx` (`Resources.SomeKey`), never inline literals, so they can be localized.
