using System;
using Windows.ApplicationModel;

namespace AppinstallerWorkaround
{
    // si I can do comparisons
    public class version_wrapper : IComparable<version_wrapper> {
        public int CompareTo(version_wrapper other) {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var major_comparison = major.CompareTo(other.major);
            if (major_comparison != 0) return major_comparison;
            var minor_comparison = minor.CompareTo(other.minor);
            if (minor_comparison != 0) return minor_comparison;
            var build_comparison = build.CompareTo(other.build);
            if (build_comparison != 0) return build_comparison;
            return revision.CompareTo(other.revision);
        }

        protected bool Equals(version_wrapper other) {
            return major == other.major && minor == other.minor && build == other.build && revision == other.revision;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((version_wrapper) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hash_code = major.GetHashCode();
                hash_code = (hash_code * 397) ^ minor.GetHashCode();
                hash_code = (hash_code * 397) ^ build.GetHashCode();
                hash_code = (hash_code * 397) ^ revision.GetHashCode();
                return hash_code;
            }
        }

        public static bool operator <(version_wrapper left, version_wrapper right) {
            return left.CompareTo(right) < 0;
        }
        public static bool operator >(version_wrapper left, version_wrapper right) {
            return left.CompareTo(right) > 0;
        }

        public static bool operator ==(version_wrapper left, version_wrapper right) {
            return Equals(left, right);
        }

        public static bool operator !=(version_wrapper left, version_wrapper right) {
            return !Equals(left, right);
        }

        public readonly ushort major;
        public readonly ushort minor;
        public readonly ushort build;
        public readonly ushort revision;

        public version_wrapper(ushort major, ushort minor, ushort build, ushort revision) {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }

        public version_wrapper(PackageVersion v) : this(v.Major, v.Minor, v.Build, v.Revision) {

        }
    }
}
