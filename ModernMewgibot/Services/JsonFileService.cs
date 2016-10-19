using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ModernMewgibot.Services
{
    static class JsonFileService
    {
        private static string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MewgiBot\\";

        /// <summary>
        /// Ensure all files required for the bot are created
        /// </summary>
        public static void InitializeAllFiles()
        {
            if (!Directory.Exists(baseDirectory + @".\Logs"))
                Directory.CreateDirectory(baseDirectory + @".\Logs");
            if (!Directory.Exists(baseDirectory + @".\Data"))
                Directory.CreateDirectory(baseDirectory + @".\Data");

            string date = DateTimeOffset.Now.ToString("MMddyyyy");
            string logname = baseDirectory + @".\Logs\log-" + date + ".txt";

            Task.Factory.StartNew(() => InitializeFile(baseDirectory + @".\Data\quotes.json"));
            Task.Factory.StartNew(() => InitializeFile(baseDirectory + @".\Data\commands.json"));
            Task.Factory.StartNew(() => CreateFile(baseDirectory + @".\Logs\thanks.json"));
            Task.Factory.StartNew(() => InitializeFile(logname));
        }

        /// <summary>
        /// Create specified file if it doesn't already exist
        /// </summary>
        /// <param name="fileName">File to create</param>
        private static void InitializeFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                CreateFile(fileName);
            }
        }

        /// <summary>
        /// Create specified file.
        /// </summary>
        /// <param name="fileName">File to create</param>
        private static void CreateFile(string fileName)
        {
            FileStream stream = File.Create(fileName);
            stream.Close();
        }

        /// <summary>
        /// Convert object to a JSON string, then save to a .json file
        /// </summary>
        /// <typeparam name="T">Type of object to save, typically a list of something</typeparam>
        /// <param name="toSave">Object to convert to json file</param>
        /// <param name="fileNameAndPath">Name and path of file</param>
        public static async Task SaveToFile<T>(T toSave, string fileNameAndPath)
        {
            await WriteTextAsync(baseDirectory + fileNameAndPath, JsonConvert.SerializeObject(toSave, Formatting.Indented));
        }

        /// <summary>
        /// Writes text to a file asynchronously
        /// </summary>
        /// <param name="filePath">Name and path of the file written to</param>
        /// <param name="text">Text to be written</param>
        /// <returns></returns>
        private static async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await stream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        /// <summary>
        /// Load data from a .json file
        /// </summary>
        /// <typeparam name="T">Type of object after deserialization of json</typeparam>
        /// <param name="fileNameAndPath">Name and path of .json file</param>
        /// <returns>Deserialized object</returns>
        public static async Task<T> LoadFromFile<T>(string fileNameAndPath)
        {
            string json = await ReadTextAsync(baseDirectory + fileNameAndPath);
            T toReturn = default(T);

            if (!(json == "File not found.") && !(String.IsNullOrEmpty(json)) && !(String.IsNullOrWhiteSpace(json)))
                toReturn = JsonConvert.DeserializeObject<T>(json);

            return toReturn;
        }

        /// <summary>
        /// Loads text from a file asynchronously
        /// </summary>
        /// <param name="filePath">Name and path of file</param>
        /// <returns>File's text as a string</returns>
        private static async Task<string> ReadTextAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] buffer = new byte[0x1000];
                    int numRead;
                    while ((numRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string text = Encoding.Unicode.GetString(buffer, 0, numRead);
                        sb.Append(text);
                    }

                    return sb.ToString();
                }
            }
            else
                return "File not found.";
        }
    }
}
