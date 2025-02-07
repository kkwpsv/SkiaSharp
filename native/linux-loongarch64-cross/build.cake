DirectoryPath ROOT_PATH = MakeAbsolute(Directory("../.."));

#load "../../scripts/cake/shared.cake"

if (BUILD_ARCH.Length == 0)
    BUILD_ARCH = new [] { "loongarch64" };

string sysroot = Argument("sysroot", EnvironmentVariable("SYSROOT"));

string GetGnArgs(string arch)
{
    var init = $"'--sysroot={sysroot}', '--target=loongarch64-linux-gnu'";
    var bin = $"'-B/opt/loongarch64/loongson-gnu-toolchain-8.3-x86_64-loongarch64-linux-gnu-rc1.5/loongarch64-linux-gnu/bin/' ";
    return
        $"extra_asmflags+=[ {init}, {bin} ] " +
        $"extra_cflags+=[ {init}, {bin} ] " +
        $"extra_ldflags+=[ {init}, {bin} ] " +
        ADDITIONAL_GN_ARGS;
}

Task("libSkiaSharp")
    .WithCriteria(IsRunningOnLinux())
    .Does(() =>
{
    foreach (var arch in BUILD_ARCH) {
        RunCake("../linux/build.cake", "libSkiaSharp", new Dictionary<string, string> {
            { "arch", arch },
            { "gnArgs", GetGnArgs(arch) },
        });
    }
});

Task("libHarfBuzzSharp")
    .WithCriteria(IsRunningOnLinux())
    .Does(() =>
{
    foreach (var arch in BUILD_ARCH) {
        RunCake("../linux/build.cake", "libHarfBuzzSharp", new Dictionary<string, string> {
            { "arch", arch },
            { "gnArgs", GetGnArgs(arch) },
        });
    }
});

Task("Default")
    .IsDependentOn("libSkiaSharp")
    .IsDependentOn("libHarfBuzzSharp");

RunTarget(TARGET);
