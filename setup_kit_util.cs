using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace AppinstallerWorkaround
{
    /* Usage:
     
       // App.xaml
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (setup_kit_util.has_update_downloaded_locally() && await setup_kit_util.launch_update_kit()) {
                Application.Current.Exit();
                return;
            }
            // note: you can place this anywhere, as long as it's AFTER the above code
            Task.Run(async () => await setup_kit_util.download_update_task());

            ...
        }

     */
    public static class setup_kit_util
    {
        // the idea - this is a folder where you'll keep your downloaded setup kits
        public static string updates_dir { get; } = create_and_return("updates");

        // example: https://cinematicstudio.app/cinematic/CinematicStudio.appinstaller 
        public static string appinstaller_url = "https://your_server/path/to/your/file.appinstaller";
        // example: https://cinematicstudio.app/cinematic/CinematicStudio.msixbundle
        public static string msix_url = "https://your_server/path/to/your/file.msixbundle";

        private static string get_root_directory() {
            var folder = ApplicationData.Current.LocalFolder.Path + "\\";
            return folder;
        }
        private static string create_and_return(string root, string sub) {
            var sub_dir = root + sub + "\\";
            try {
                if ( !Directory.Exists(sub_dir))
                    Directory.CreateDirectory(sub_dir);
            } catch (Exception e) {
            }
            return sub_dir;
        }
        private static string create_and_return(string sub) {
            return create_and_return(get_root_directory(), sub);
        }

        private static (ushort, string) last_number_from_string(string s) {
            var idx = s.LastIndexOf('.');
            if (idx < 0)
                return (0, s);
            var number_str = s.Substring(idx + 1);
            ushort.TryParse(number_str, out var n);
            return (n, s.Substring(0, idx));
        }

        private static version_wrapper version_from_file(string f) {
            try {
                f = f.Substring(0, f.Length - ".msixbundle".Length);
                ushort major = 0, minor = 0, build = 0, revision = 0;
                (revision, f) = last_number_from_string(f);
                (build, f) = last_number_from_string(f);
                (minor, f) = last_number_from_string(f);
                (major, f) = last_number_from_string(f);
                return new version_wrapper(major, minor, build, revision);
            } catch {
            }
            return new version_wrapper(0, 0, 0, 0);
        }

        public static bool has_update_downloaded_locally() {
            var cur_ver = new version_wrapper( workaround_util.version());
            try {
                var downloaded = Directory.EnumerateFiles(updates_dir, "*.msixbundle").Select(f => (version_from_file(f), Path.GetFileName(f), new FileInfo(f).LastWriteTime )).ToList();
                foreach ( var kit in downloaded.Where(k => k.Item1 < cur_ver))
                    File.Delete(updates_dir + kit.Item2);

                var last_kit = downloaded.Where(k => k.Item1 > cur_ver).OrderByDescending(k => k.LastWriteTime).FirstOrDefault();
                return last_kit.Item1 != null;
            } catch(Exception e) {
            }

            return false;
        }

        public static async Task<bool> launch_update_kit() {
            var cur_ver = new version_wrapper( workaround_util.version());
            try {
                var downloaded = Directory.EnumerateFiles(updates_dir, "*.msixbundle").Select(f => (version_from_file(f), Path.GetFileName(f), new FileInfo(f).LastWriteTime )).ToList();
                var last_kit = downloaded.Where(k => k.Item1 > cur_ver).OrderByDescending(k => k.LastWriteTime).FirstOrDefault();
                var file = await (await StorageFolder.GetFolderFromPathAsync(updates_dir)).GetFileAsync(Path.GetFileName(last_kit.Item2));
                await Launcher.LaunchFileAsync(file);
                return true;
            } catch(Exception e) {
            }

            return false;
        }

        public static async Task download_update_task() {
            try {
                var appinstaller = workaround_util.read_html_page(appinstaller_url);
                var same_str = "Version=\"" + workaround_util.version_str();
                var same = appinstaller.Contains(same_str);
                if (same)
                    return; // no update yet

                var idx_version = appinstaller.IndexOf("Version=\"");
                Debug.Assert(idx_version > 0);
                idx_version += "Version=\"".Length;
                var idx_end = appinstaller.IndexOf('"', idx_version);

                var version = appinstaller.Substring(idx_version, idx_end - idx_version);

                // download it with ".temp" extension first
                var bytes = workaround_util.read_html_page_bytes(msix_url);

                var kit_name = updates_dir + "CinematicStudio." + version + ".msixbundle";
                File.WriteAllBytes(kit_name, bytes);
            } catch (Exception e) {
            }
        }
    }
}
