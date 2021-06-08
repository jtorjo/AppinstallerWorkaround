using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace AppinstallerWorkaround
{
    public static class workaround_util
    {
        public static PackageVersion version() {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return version;
        }
        public static string version_str() {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        public static byte[] read_html_page_bytes(string url, string username = null, string passw = null) {
            try {
                WebClient client = new WebClient();
                // this is optional
                //client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add ("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");
                if ( username != null)
                    client.Credentials = new NetworkCredential(username, passw);
                return client.DownloadData(url);
            } catch (Exception e) {
                return null;
            }
        }

        public static string read_html_page(string url, string username = null, string passw = null) {
            try {
                WebClient client = new WebClient();
                // this is optional
                //client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add ("user-agent", "Mozilla/5.0 (Windows NT 6.1; rv:15.0) Gecko/20120716 Firefox/15.0a2");
                if ( username != null)
                    client.Credentials = new NetworkCredential(username, passw);
                Stream data = client.OpenRead (url);
                StreamReader reader = new StreamReader (data);
                string s = reader.ReadToEnd ();
                return s;
            } catch (Exception e) {
                return "";
            }
        }
    }
}
