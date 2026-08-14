// Downfall/fmod/Scripts/export_to_audio.js
//
// Adds a "Downfall > Export to audio" menu item (Ctrl+Shift+E) that:
//   1. builds every bank for the current platform,
//   2. writes GUIDs.txt (same as File > Export GUIDs),
//   3. copies *.bank AND GUIDs.txt into Downfall/audio.
//
// Layout assumed:
//   Downfall/
//   |- fmod/
//   |  |- fmod.fspro
//   |  |- Build/
//   |  |  |- GUIDs.txt      <- exportGUIDs() writes it HERE (build root)
//   |  |  \- Desktop/       <- build() writes the .bank files HERE
//   |  \- Scripts/          <- this file goes here
//   \- audio/               <- everything lands here
//
// Put this file in  Downfall/fmod/Scripts/  then use Scripts > Reload in FMOD Studio.
// Windows only (uses ROBOCOPY). studio.project.filePath uses forward slashes.

studio.menu.addMenuItem({
    name: "Downfall\\Export to audio",
    keySequence: "Ctrl+Shift+E",
    execute: function () {
        // ---- config -------------------------------------------------------
        // Platform subfolder that build() writes the banks into. Build once and
        // look inside Downfall/fmod/Build/ to confirm (often "Desktop", sometimes "-").
        var PLATFORM = "Desktop";
        // -------------------------------------------------------------------

        // Directory containing fmod.fspro  ->  Downfall/fmod  (no trailing slash).
        var projectDir = studio.project.filePath.substr(
            0, studio.project.filePath.lastIndexOf("/"));

        // One level up  ->  Downfall
        var downfallDir = projectDir.substr(0, projectDir.lastIndexOf("/"));

        var buildRoot = projectDir + "/Build";              // GUIDs.txt lives here
        var bankDir   = buildRoot + "/" + PLATFORM;         // .bank files live here
        var audioDir  = downfallDir + "/Downfall/audio";             // destination

        // 1. Build all banks for the current platform.
        studio.project.build();

        // 2. Export GUIDs.txt (into buildRoot).
        studio.project.exportGUIDs();

        // 3a. Copy the banks (flat, from the platform folder).
        studio.system.start("ROBOCOPY", {
            workingDir: projectDir,
            args: [bankDir, audioDir, "*.bank", "/NJH", "/NJS"],
            timeout: 120000
        });

        // 3b. Copy GUIDs.txt (from the build root, one level up from the banks).
        //     No /S, so only the GUIDs.txt in buildRoot is taken, dropped flat into audio/.
        studio.system.start("ROBOCOPY", {
            workingDir: projectDir,
            args: [buildRoot, audioDir, "GUIDs.txt", "/NJH", "/NJS"],
            timeout: 60000
        });

        console.log("Exported banks (" + bankDir + ") + GUIDs.txt (" + buildRoot + ") to " + audioDir);
    }
});