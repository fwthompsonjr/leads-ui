using legallead.installer.Models;

namespace legallead.installer.Interfaces
{
    public interface ILeadFileOperation
    {
        /// <summary>
        /// Tests if the given path refers to an existing DirectoryInfo on disk.
        /// </summary>
        /// <param name="path"></param>
        bool DirectoryExists(string path);

        /// <summary>
        /// Creates all directories and subdirectories in the specified path with the specified permissions unless they already exist.
        /// </summary>
        /// <param name="path">The directory to create.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="path" /> is a zero-length string, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The caller attempts to use an invalid file mode.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path exceeds the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="path" /> is a file.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">A component of the <paramref name="path" /> is not a directory.</exception>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes directory and optionally deletes the subdirectories in the specified path
        /// </summary>
        /// <param name="path">The directory to create.</param>
        /// <param name="recursive">When true deletes sub directorys and files recursively.</param>
        void DeleteDirectory(string path, bool recursive);
        /// <summary>
        /// Extracts byte array to target path
        /// </summary>
        /// <param name="path">Path to extract files to</param>
        /// <param name="content">Binary content as byte-array</param>
        bool Extract(string path, byte[] content);
        /// <summary>
        /// Searches the specified path and finds top-level child directory information
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        DirectoryInfoModel[] GetDirectories(string path);
        bool FileExists(string path);
        string? FindExecutable(string path);
        IEnumerable<string> FindFiles(string path, string pattern);
        bool LaunchExecutable(string path);

        /// <summary>
        /// Deletes file in the specified path
        /// </summary>
        /// <param name="path">The file location.</param>
        void DeleteFile(string path);
    }
}
