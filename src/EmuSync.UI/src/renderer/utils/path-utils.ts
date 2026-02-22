export function normalisePathDelims(path: string, isWindows: boolean) {

    return isWindows
        ? path.replace(/\//g, "\\") //normalise → Windows
        : path.replace(/\\/g, "/"); //normalise → mac + linux
}