using static System.Environment;

namespace Utility.Constants
{
    public class Paths
    {
        public const string DefaultDataPath = "O:\\Data";
        public const string DefaultModelsFileName = "models.sqlite";
        public const string DefaultAPIFileName = "api.sqlite";
        public static readonly string DefaultModelsFilePath = Path.Combine(DefaultDataPath, DefaultModelsFileName);
        public static readonly string DefaultAPIFilePath = Path.Combine(DefaultDataPath, DefaultAPIFileName);

        /// <summary>
        /// <a href="https://gist.github.com/DamianSuess/c143ed869e02e002d252056656aeb9bf"></a>
        /// </summary>

        public static readonly string ProgramData = GetFolderPath(SpecialFolder.CommonApplicationData);

        // C:\Users\USERNAME\AppData\Roaming
        public static readonly string ApplicationData = GetFolderPath(SpecialFolder.ApplicationData);

        // C:\Users\USERNAME\Documents\
        public static readonly string Personal = GetFolderPath(SpecialFolder.Personal);

        /// <summary>
        /// Generate List
        /// N.B, MyDocuments and Personal are both = 5, so it will appear twice as, MyDocuments
        ///SpecialFolder Enum  Windows Path    Mac Path
        ///AdminTools C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        ///ApplicationData C:\Users\USERNAME\AppData\Roaming	/Users/USERNAME/.config
        ///CDBurning   C:\Users\USERNAME\AppData\Local\Microsoft\Windows\Burn\Burn
        ///CommonAdminTools    C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        ///CommonApplicationData C:\ProgramData	/usr/share
        ///CommonDesktopDirectory  C:\Users\Public\Desktop
        ///CommonDocuments C:\Users\Public\Documents
        ///CommonMusic C:\Users\Public\Music
        ///CommonOemLinks
        ///CommonPictures C:\Users\Public\Pictures
        ///CommonProgramFiles  C:\Program Files\Common Files
        ///CommonProgramFilesX86 C:\Program Files(x86)\Common Files
        ///CommonPrograms C:\ProgramData\Microsoft\Windows\Start Menu\Programs
        ///CommonStartMenu C:\ProgramData\Microsoft\Windows\Start Menu
        ///CommonStartup C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup
        ///CommonTemplates C:\ProgramData\Microsoft\Windows\Templates
        ///CommonVideos    C:\Users\Public\Videos
        ///Cookies C:\Users\USERNAME\AppData\Local\Microsoft\Windows\INetCookies
        ///Desktop C:\Users\USERNAME\Desktop	/Users/USERNAME/Desktop
        ///DesktopDirectory    C:\Users\USERNAME\Desktop	/Users/USERNAME/Desktop
        ///Favorites   C:\Users\USERNAME\Favorites	/Users/USERNAME/Library/Favorites
        ///Fonts   C:\WINDOWS\Fonts	/Users/USERNAME/Library/Fonts
        ///History C:\Users\USERNAME\AppData\Local\Microsoft\Windows\History
        ///InternetCache   C:\Users\USERNAME\AppData\Local\Microsoft\Windows\INetCache	/Users/USERNAME/Library/Caches
        ///LocalApplicationData    C:\Users\USERNAME\AppData\Local	/Users/USERNAME/.local/share
        ///LocalizedResources
        ///MyComputer
        ///MyDocuments C:\Users\USERNAME\Documents	/Users/USERNAME
        ///MyMusic C:\Users\USERNAME\Music	/Users/USERNAME/Music
        ///MyPictures  C:\Users\USERNAME\Pictures	/Users/USERNAME/Pictures
        ///MyVideos    C:\Users\USERNAME\Videos
        ///NetworkShortcuts    C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Network Shortcuts
        ///Personal C:\Users\USERNAME\Documents	/Users/USERNAME
        ///PrinterShortcuts
        ///ProgramFiles C:\Program Files	/Applications
        ///ProgramFilesX86 C:\Program Files(x86)
        ///Programs C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
        ///Recent  C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Recent
        ///Resources   C:\WINDOWS\resources
        ///SendTo  C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\SendTo
        ///StartMenu   C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu
        ///Startup C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
        ///System  C:\WINDOWS\system32	/System
        ///SystemX86   C:\WINDOWS\SysWOW64
        ///Templates   C:\Users\USERNAME\AppData\Roaming\Microsoft\Windows\Templates
        ///UserProfile C:\Users\USERNAME	/Users/USERNAME
        ///Windows C:\WINDOWS
        /// </summary>
        public void OutputPathsTest()
        {
            var values = GetValues<System.Environment.SpecialFolder>();
            foreach (var e in values)
            {
                System.Diagnostics.Debug.Print("| " + e.ToString() + " | " + GetFolderPath(e) + " |");
            }

            static System.Collections.Generic.IEnumerable<T> GetValues<T>()
            {
                return System.Enum.GetValues(typeof(T)).Cast<T>();
            }
        }
    }
}