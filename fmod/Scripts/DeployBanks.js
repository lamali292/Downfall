studio.menu.addMenuItem({
    name: "Scripts\\Deploy Banks to Mods",
    execute: function () {

        studio.project.build();

        var projectPath = studio.project.filePath;
        var projectDir = projectPath.substring(0, projectPath.lastIndexOf("/"));
        var buildDir = projectDir + "/Build/Desktop/";

        var banks = studio.project.model.Bank.findInstances();

        for (var i = 0; i < banks.length; i++) {
            var bankName = banks[i].name;
            if (bankName === "Master") continue; // skip Master bank

            var destDir = projectDir + "/../" + bankName + "/" + bankName + "/banks/";
            var files = [bankName + ".bank", bankName + ".strings.bank"];

            for (var j = 0; j < files.length; j++) {
                var src  = buildDir + files[j];
                var dest = destDir  + files[j];

                var f = studio.system.getFile(src);
                f.copy(dest);
                console.log("Copied: " + files[j]);
            }
        }

        console.log("Deploy complete.");
    }
});