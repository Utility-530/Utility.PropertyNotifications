using System;
using System.Runtime.InteropServices;


namespace Leepfrog.WpfFramework
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1OLD
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;

        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        [MarshalAs(UnmanagedType.I4)]
        public int x;
        [MarshalAs(UnmanagedType.I4)]
        public int y;
    }

    [StructLayout(LayoutKind.Sequential,
    CharSet = CharSet.Ansi)]
    public struct DEVMODE1OLD2
    {
        // You can define the following constant
        // but OUTSIDE the structure because you know
        // that size and layout of the structure
        // is very important
        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        // In addition you can define the last character array
        // as following:
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        //public Char[] dmDeviceName;

        // After the 32-bytes array
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSpecVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSize;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverExtra;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmFields;

        public POINTL dmPosition;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayOrientation;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFixedOutput;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmColor;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmDuplex;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmYResolution;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmTTOption;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmCollate;

        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        // Also can be defined as
        //[MarshalAs(UnmanagedType.ByValArray,
        //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
        //public Byte[] dmFormName;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmLogPixels;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmBitsPerPel;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsHeight;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFlags;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFrequency;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMMethod;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMIntent;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmMediaType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDitherType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved1;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved2;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningHeight;
    }

    public enum DisplayOrientation
    {
        Landscape = 0,
        Portrait = 1,
        LandscapeInverted = 2,
        PortraitInverted = 3
    }

    [Flags]
    public enum DmFields : uint
    {
        DM_ORIENTATION = 0x00000001,
        DM_PAPERSIZE = 0x00000002,
        DM_PAPERLENGTH = 0x00000004,
        DM_PAPERWIDTH = 0x00000008,
        DM_SCALE = 0x00000010,
        DM_POSITION = 0x00000020,
        DM_NUP = 0x00000040,
        DM_DISPLAYORIENTATION = 0x00000080,
        DM_COPIES = 0x00000100,
        DM_DEFAULTSOURCE = 0x00000200,
        DM_PRINTQUALITY = 0x00000400,
        DM_COLOR = 0x00000800,
        DM_DUPLEX = 0x00001000,
        DM_YRESOLUTION = 0x00002000,
        DM_TTOPTION = 0x00004000,
        DM_COLLATE = 0x00008000,
        DM_FORMNAME = 0x00010000,
        DM_LOGPIXELS = 0x00020000,
        DM_BITSPERPEL = 0x00040000,
        DM_PELSWIDTH = 0x00080000,
        DM_PELSHEIGHT = 0x00100000,
        DM_DISPLAYFLAGS = 0x00200000,
        DM_DISPLAYFREQUENCY = 0x00400000,
        DM_ICMMETHOD = 0x00800000,
        DM_ICMINTENT = 0x01000000,
        DM_MEDIATYPE = 0x02000000,
        DM_DITHERTYPE = 0x04000000,
        DM_PANNINGWIDTH = 0x08000000,
        DM_PANNINGHEIGHT = 0x10000000,
        DM_DISPLAYFIXEDOUTPUT = 0x20000000
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DEVMODE_UnionA
    {
        // union a
        [FieldOffsetAttribute(0)]
        public short dmOrientation;
        [FieldOffsetAttribute(2)]
        public short dmPaperSize;
        [FieldOffsetAttribute(4)]
        public short dmPaperLength;
        [FieldOffsetAttribute(6)]
        public short dmPaperWidth;
        [FieldOffsetAttribute(8)]
        public short dmScale;
        [FieldOffsetAttribute(10)]
        public short dmCopies;
        [FieldOffsetAttribute(12)]
        public short dmDefaultSource;
        [FieldOffsetAttribute(14)]
        public short dmPrintQuality;

        // union b
        [FieldOffsetAttribute(0)]
        public int dmPositionX;
        [FieldOffsetAttribute(4)]
        public int dmPositionY;
        [FieldOffsetAttribute(8)]
        public uint dmDisplayOrientation;
        [FieldOffsetAttribute(12)]
        public uint dmDisplayFixedOutput;
    }

    public enum DmDisplayFlags : uint
    {
        DM_COLOR_NONINTERLACED = 0,
        DM_GRAYSCALE = 0x00000001, /* This flag is no longer valid */
        DM_INTERLACED = 0x00000002
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DEVMODE_UnionB
    {
        [FieldOffsetAttribute(0)]
        public DmDisplayFlags dmDisplayFlags;
        [FieldOffsetAttribute(0)]
        public uint dmNup;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE1
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public DmFields dmFields;

        //// union a
        //public short dmOrientation;
        //public short dmPaperSize;
        //public short dmPaperLength;
        //public short dmPaperWidth;
        //public short dmScale;
        //public short dmCopies;
        //public short dmDefaultSource;
        //public short dmPrintQuality;
        //// union b
        //public int dmPositionX;
        //public int dmPositionY;
        //public uint dmDisplayOrientation;
        //public uint dmDisplayFixedOutput;
        public DEVMODE_UnionA UnionA;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public ushort dmLogPixels;
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;

        //public DmDisplayFlags dmDisplayFlags;   // union a
        //public uint dmNup;                      // union b
        public DEVMODE_UnionB UnionB;

        public uint dmDisplayFrequency;

        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }
    class User_32
    {

        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName, ref DEVMODE1 lpDevMode, IntPtr hwnd,
            int dwflags, IntPtr lParam);
        public const int ENUM_CURRENT_SETTINGS = -1;

        [Flags()]
        public enum ChangeDisplaySettingsFlags : uint
        {
            CDS_NONE = 0,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_TEST = 0x00000002,
            CDS_FULLSCREEN = 0x00000004,
            CDS_GLOBAL = 0x00000008,
            CDS_SET_PRIMARY = 0x00000010,
            CDS_VIDEOPARAMETERS = 0x00000020,
            CDS_ENABLE_UNSAFE_MODES = 0x00000100,
            CDS_DISABLE_UNSAFE_MODES = 0x00000200,
            CDS_RESET = 0x40000000,
            CDS_RESET_EX = 0x20000000,
            CDS_NORESET = 0x10000000
        }

        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_FAILED = -1;
    }



    public static class ResolutionHelper
    {
        public static bool SetResolution(int display, int newWidth, int newHeight, DisplayOrientation orientation )
        {
            var logger = new object();
            int iRet = 0;

            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (ushort)Marshal.SizeOf(dm);

            var devName = $@"\\.\Display{display}";
            if (0 != User_32.EnumDisplaySettings(devName, User_32.ENUM_CURRENT_SETTINGS, ref dm))
            {
                
                if (
                    (dm.dmPelsWidth == newWidth)
                 && (dm.dmPelsHeight == newHeight)
                 && (dm.UnionA.dmDisplayOrientation == (ushort) orientation)
                   )
                {
                    logger.AddLog("RESOLUTION", $"Display {display} already set to {newWidth}x{newHeight} {orientation}");
                    return false;
                }

                logger.AddLog("RESOLUTION", $"Display {display} currently set to {dm.dmPelsWidth}x{dm.dmPelsHeight} {(DisplayOrientation) dm.UnionA.dmDisplayOrientation}");

                dm.dmPelsWidth = (uint)newWidth;
                dm.dmPelsHeight = (uint)newHeight;
                dm.UnionA.dmDisplayOrientation = (ushort)orientation;
                dm.dmFields = (DmFields.DM_PELSWIDTH | DmFields.DM_PELSHEIGHT | DmFields.DM_DISPLAYORIENTATION );

                iRet = User_32.ChangeDisplaySettingsEx(devName, ref dm, IntPtr.Zero, (int)(User_32.ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | User_32.ChangeDisplaySettingsFlags.CDS_GLOBAL),
                    IntPtr.Zero);

                switch (iRet)
                {
                    case User_32.DISP_CHANGE_SUCCESSFUL:
                        {
                            //successfull change
                            logger.AddLog("RESOLUTION", $"Changed display {display} to {newWidth}x{newHeight} {orientation}");
                            return true;
                        }
                    case User_32.DISP_CHANGE_RESTART:
                        {
                            //windows 9x series you have to restart
                            return true;
                        }
                }
            }
            //failed to change
            logger.AddLog("RESOLUTION", $"Failed to change display {display} to {newWidth}x{newHeight} {orientation}, return code {iRet}");
            return false;
        }
    }
}
