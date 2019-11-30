using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Communication
{
    ///  
    /// 音量控制 
    ///  
    public class AudioMixerHelper : object
    {
        #region API及常量声明
        [DllImport("winmm.dll")]
        private static extern uint sndPlaySound(string lpszSoundName, Int64 uSndMode);

        private const long SND_SYNC = 0;
        private const long SND_ASYNC = 1;
        private const long SND_LOOP = 8;
        private const long SND_STOP = 4;

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern uint mixerGetNumDevs();


        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerGetDevCaps(uint uMxId, ref MIXERCAPS pmxcaps, uint cbmxcaps);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerOpen(ref uint phmx, uint uMxId, uint dwCallback, uint dwInstance, ObjectFlag fdwOpen);
        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerClose(uint hmx);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerGetLineInfo(uint hmxobj, ref MIXERLINE pmxl, GetLineInfoFlag fdwInfo);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerGetLineControls(uint hmxobj, ref MIXERLINECONTROLS pmxlc, GetLineControlsFlag fdwControls);

        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerGetControlDetails(uint hmxobj, ref MIXERCONTROLDETAILS pmxcd, GetControlDetailsFlag fdwDetails);
        [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
        private static extern MMSYSERR mixerSetControlDetails(uint hmxobj, ref MIXERCONTROLDETAILS pmxcd, SetControlDetailsFlag fdwDetails);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(ref byte Struct, uint ptr, uint cb);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(ref MIXERCONTROL Struct, uint ptr, uint cb);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(ref MIXERCONTROLDETAILS_LISTTEXT Struct, uint ptr, uint cb);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(ref MIXERCONTROLDETAILS_UNSIGNED Struct, uint ptr, uint cb);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(uint ptr, ref MIXERCONTROLDETAILS_LISTTEXT Struct, uint cb);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern void RtlMoveMemory(uint ptr, ref MIXERCONTROLDETAILS_UNSIGNED Struct, uint cb);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern uint GlobalAlloc(uint wFlags, uint dwBytes);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern uint GlobalLock(uint hmem);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern uint GlobalFree(uint hmem);


        private const int MAXPNAMELEN = 32;
        private const int MIXER_LONG_NAME_CHARS = 64;
        private const int MIXER_SHORT_NAME_CHARS = 16;
        private enum MMSYSERR : uint
        {
            NOERROR = 0x0,
            ERROR = (NOERROR + 1),
            BADDEVICEID = (NOERROR + 2),
            NOTENABLED = (NOERROR + 3),
            ALLOCATED = (NOERROR + 4),
            INVALHANDLE = (NOERROR + 5),
            NODRIVER = (NOERROR + 6),
            NOMEM = (NOERROR + 7),
            NOTSUPPORTED = (NOERROR + 8),
            BADERRNUM = (NOERROR + 9),
            INVALFLAG = (NOERROR + 10),
            INVALPARAM = (NOERROR + 11),
            HANDLEBUSY = (NOERROR + 12),
            INVALIDALIAS = (NOERROR + 13),
            BADDB = (NOERROR + 14),
            KEYNOTFOUND = (NOERROR + 15),
            READERROR = (NOERROR + 16),
            WRITEERROR = (NOERROR + 17),
            DELETEERROR = (NOERROR + 18),
            VALNOTFOUND = (NOERROR + 19),
            NODRIVERCB = (NOERROR + 20)

        };
        public enum Mid : short
        {
            MM_MICROSOFT = 1,
            MM_CREATIVE = 2,
            MM_MEDIAVISION = 3,
            MM_FUJITSU = 4,
            MM_ARTISOFT = 20,
            MM_TURTLE_BEACH = 21,
            MM_IBM = 22,
            MM_VOCALTEC = 23,
            MM_ROLAND = 24,
            MM_DSP_SOLUTIONS = 25,
            MM_NEC = 26,
            MM_ATI = 27,
            MM_WANGLABS = 28,
            MM_TANDY = 29,
            MM_VOYETRA = 30,
            MM_ANTEX = 31,
            MM_ICL_PS = 32,
            MM_INTEL = 33,
            MM_GRAVIS = 34,
            MM_VAL = 35,
            MM_INTERACTIVE = 36,
            MM_YAMAHA = 37,
            MM_EVEREX = 38,
            MM_ECHO = 39,
            MM_SIERRA = 40,
            MM_CAT = 41,
            MM_APPS = 42,
            MM_DSP_GROUP = 43,
            MM_MELABS = 44,
            MM_COMPUTER_FRIENDS = 45,
            MM_ESS = 46,
            MM_AUDIOFILE = 47,
            MM_MOTOROLA = 48,
            MM_CANOPUS = 49,
            MM_EPSON = 50,
            MM_TRUEVISION = 51,
            MM_AZTECH = 52,
            MM_VIDEOLOGIC = 53,
            MM_SCALACS = 54,
            MM_KORG = 55,
            MM_APT = 56,
            MM_ICS = 57,
            MM_ITERATEDSYS = 58,
            MM_METHEUS = 59,
            MM_LOGITECH = 60,
            MM_WINNOV = 61,
            MM_NCR = 62,
            MM_EXAN = 63,
            MM_AST = 64,
            MM_WILLOWPOND = 65,
            MM_SONICFOUNDRY = 66,
            MM_VITEC = 67,
            MM_MOSCOM = 68,
            MM_SILICONSOFT = 69,
            MM_SUPERMAC = 73,
            MM_AUDIOPT = 74,
            MM_SPEECHCOMP = 76,
            MM_DOLBY = 78,
            MM_OKI = 79,
            MM_AURAVISION = 80,
            MM_OLIVETTI = 81,
            MM_IOMAGIC = 82,
            MM_MATSUSHITA = 83,
            MM_CONTROLRES = 84,
            MM_XEBEC = 85,
            MM_NEWMEDIA = 86,
            MM_NMS = 87,
            MM_LYRRUS = 88,
            MM_COMPUSIC = 89,
            MM_OPTI = 90,
            MM_DIALOGIC = 93
        }
        public enum GetLineInfoFlag : uint
        {
            DESTINATION = 0x0,
            SOURCE = 0x1,
            LINEID = 0x2,
            COMPONENTTYPE = 0x3,
            TARGETTYPE = 0x4,
            QUERYMASK = 0xF,
        }
        public enum ObjectFlag : uint
        {
            MIXER = 0x00000000,
            WAVEOUT = 0x10000000,
            WAVEIN = 0x20000000,
            MIDIOUT = 0x30000000,
            MIDIIN = 0x40000000,
            AUX = 0x50000000,
            HANDLE = 0x80000000,
            HMIDIIN = (HANDLE | MIDIIN),
            HMIDIOUT = (HANDLE | MIDIOUT),
            HMIXER = (HANDLE | MIXER),
            HWAVEIN = (HANDLE | WAVEIN),
            HWAVEOUT = (HANDLE | WAVEOUT)
        }
        public enum LineFlag : uint
        {
            ACTIVE = 0x00000001,
            DISCONNECTED = 0x00008000,
            SOURCE = 0x80000000,
        }
        public enum ComponentType : uint
        {
            DST_UNDEFINED = 0x0000,
            DST_DIGITAL = (DST_UNDEFINED + 1),
            DST_LINE = (DST_UNDEFINED + 2),
            DST_MONITOR = (DST_UNDEFINED + 3),
            DST_SPEAKERS = (DST_UNDEFINED + 4),
            DST_HEADPHONES = (DST_UNDEFINED + 5),
            DST_TELEPHONE = (DST_UNDEFINED + 6),
            DST_WAVEIN = (DST_UNDEFINED + 7),
            DST_VOICEIN = (DST_UNDEFINED + 8),

            SRC_UNDEFINED = 0x1000,
            SRC_DIGITAL = (SRC_UNDEFINED + 1),
            SRC_LINE = (SRC_UNDEFINED + 2),
            SRC_MICROPHONE = (SRC_UNDEFINED + 3),
            SRC_SYNTHESIZER = (SRC_UNDEFINED + 4),
            SRC_COMPACTDISC = (SRC_UNDEFINED + 5),
            SRC_TELEPHONE = (SRC_UNDEFINED + 6),
            SRC_PCSPEAKER = (SRC_UNDEFINED + 7),
            SRC_WAVEOUT = (SRC_UNDEFINED + 8),
            SRC_AUXILIARY = (SRC_UNDEFINED + 9),
            SRC_ANALOG = (SRC_UNDEFINED + 10),

        }
        public enum TargetType : uint
        {
            UNDEFINED = 0x0,
            WAVEOUT = 0x1,
            WAVEIN = 0x2,
            MIDIOUT = 0x3,
            MIDIIN = 0x4,
            AUX = 0x5,
        }
        public enum ControlType_Units : uint
        {
            CUSTOM = 0x000000,
            BOOLEAN = 0x010000,
            SIGNED = 0x020000,
            UNSIGNED = 0x030000,
            DECIBELS = 0x040000,
            PERCENT = 0x050000,
            MASK = 0xFF0000,
        }
        public enum ControlType_SubClass : uint
        {
            LIST_SINGLE = 0x0000000,
            METER_POLLED = 0x0000000,
            SWITCH_BOOLEAN = 0x0000000,
            TIME_MICROSECS = 0x0000000,
            LIST_MULTIPLE = 0x1000000,
            SWITCH_BUTTON = 0x1000000,
            TIME_MILLISECS = 0x1000000,
            MASK = 0xF000000,
        }
        public enum ControlType_Class : uint
        {
            CUSTOM = 0x00000000,
            METER = 0x10000000,
            SWITCH = 0x20000000,
            NUMBER = 0x30000000,
            SLIDER = 0x40000000,
            FADER = 0x50000000,
            TIME = 0x60000000,
            LIST = 0x70000000,
            MASK = 0xF0000000,
        }
        public enum ControlType : uint
        {
            CUSTOM = (ControlType_Class.CUSTOM | ControlType_Units.CUSTOM),
            BOOLEANMETER = (ControlType_Class.METER | ControlType_SubClass.METER_POLLED | ControlType_Units.BOOLEAN),
            SIGNEDMETER = (ControlType_Class.METER | ControlType_SubClass.METER_POLLED | ControlType_Units.SIGNED),
            PEAKMETER = (SIGNEDMETER + 1),
            UNSIGNEDMETER = (ControlType_Class.METER | ControlType_SubClass.METER_POLLED | ControlType_Units.UNSIGNED),
            BUTTON = (ControlType_Class.SWITCH | ControlType_SubClass.SWITCH_BUTTON | ControlType_Units.BOOLEAN),
            BOOLEAN = (ControlType_Class.SWITCH | ControlType_SubClass.SWITCH_BOOLEAN | ControlType_Units.BOOLEAN),
            ONOFF = (BOOLEAN + 1),
            MUTE = (BOOLEAN + 2),
            MONO = (BOOLEAN + 3),
            LOUDNESS = (BOOLEAN + 4),
            STEREOENH = (BOOLEAN + 5),
            PERCENT = (ControlType_Class.NUMBER | ControlType_Units.PERCENT),
            SIGNED = (ControlType_Class.NUMBER | ControlType_Units.SIGNED),
            UNSIGNED = (ControlType_Class.NUMBER | ControlType_Units.UNSIGNED),
            DECIBELS = (ControlType_Class.NUMBER | ControlType_Units.DECIBELS),
            SLIDER = (ControlType_Class.SLIDER | ControlType_Units.SIGNED),
            PAN = (SLIDER + 1),
            QSOUNDPAN = (SLIDER + 2),
            FADER = (ControlType_Class.FADER | ControlType_Units.UNSIGNED),
            VOLUME = (FADER + 1),
            BASS = (FADER + 2),
            TREBLE = (FADER + 3),
            EQUALIZER = (FADER + 4),
            MICROTIME = (ControlType_Class.TIME | ControlType_SubClass.TIME_MICROSECS | ControlType_Units.UNSIGNED),
            MILLITIME = (ControlType_Class.TIME | ControlType_SubClass.TIME_MILLISECS | ControlType_Units.UNSIGNED),
            SINGLESELECT = (ControlType_Class.LIST | ControlType_SubClass.LIST_SINGLE | ControlType_Units.BOOLEAN),
            MUX = (SINGLESELECT + 1),
            MULTIPLESELECT = (ControlType_Class.LIST | ControlType_SubClass.LIST_MULTIPLE | ControlType_Units.BOOLEAN),
            MIXER = (MULTIPLESELECT + 1),

        }
        public enum GetLineControlsFlag : uint
        {
            ALL = 0x0,
            ONEBYID = 0x1,
            ONEBYTYPE = 0x2,
            QUERYMASK = 0xF,
        }
        public enum GetControlDetailsFlag : uint
        {
            VALUE = 0x0,
            LISTTEXT = 0x1,
            QUERYMASK = 0xF,
        }
        public enum SetControlDetailsFlag : uint
        {
            VALUE = 0x0,
            CUSTOM = 0x1,
            QUERYMASK = 0xF,

        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERCAPS
        {
            public Mid wMid;
            public short wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
            public uint fdwSupport;
            public uint cDestinations;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERLINE
        {
            public uint cbStruct;
            public uint dwDestination;
            public uint dwSource;
            public uint dwLineID;
            public uint fdwLine;
            public uint dwUser;
            public ComponentType dwComponentType;
            public uint cChannels;
            public uint cConnections;
            public uint cControls;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
            public TargetType dwType;
            public uint dwDeviceID;
            public Mid wMid;
            public short wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERLINECONTROLS
        {
            public uint cbStruct;
            public uint dwLineID;

            public uint dwControl;
            public uint cControls;
            public uint cbmxctrl;
            public uint pamxctrl;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROL
        {
            public uint cbStruct;
            public uint dwControlID;
            public ControlType dwControlType;
            public uint fdwControl;
            public uint cMultipleItems;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
            public uint dwMinimum;
            public uint dwMaximum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROLDETAILS
        {
            public uint cbStruct;
            public uint dwControlID;
            public uint cChannels;
            public uint item;
            public uint cbDetails;
            public uint paDetails;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROLDETAILS_LISTTEXT
        {
            public uint dwParam1;
            public uint dwParam2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MIXERCONTROLDETAILS_UNSIGNED
        {
            public uint dwValue;
        }


        private uint StructLen(object Structure)
        {
            return Convert.ToUInt32(Marshal.SizeOf(Structure));
        }


        private string MMSYSERR_ToString(MMSYSERR ERR)
        {
            switch (ERR)
            {
                case MMSYSERR.NOERROR:
                    return "No error.";
                case MMSYSERR.BADDEVICEID:
                    return "Invalid device identifier.";
                case MMSYSERR.NOTENABLED:
                    return "The driver is not enabled.";
                case MMSYSERR.ALLOCATED:
                    return "Device is opened by the maximum number of clients.";
                case MMSYSERR.INVALHANDLE:
                    return "Invalid handle.";
                case MMSYSERR.NODRIVER:
                    return "No mixer device is available.";
                case MMSYSERR.NOMEM:
                    return "The system is unable to allocate resources.";
                case MMSYSERR.NOTSUPPORTED:
                    return "The ACM driver did not process the message..";
                case MMSYSERR.BADERRNUM:
                    return "The specified error number is out of range.";
                case MMSYSERR.INVALFLAG:
                    return "One or more flags are invalid.";
                case MMSYSERR.INVALPARAM:
                    return "One or more parameters are invalid.";
                case MMSYSERR.HANDLEBUSY:
                    return "Handle busy.";
                case MMSYSERR.INVALIDALIAS:
                    return "Invalid Alias.";
                case MMSYSERR.READERROR:
                    return "Read error.";
                case MMSYSERR.WRITEERROR:
                    return "Write error.";
                case MMSYSERR.DELETEERROR:
                    return "Delete error.";
                default:
                    return "Undifined error code.";
            }
        }


        public string Mid_ToString(Mid wMid)
        {
            switch (wMid)
            {
                case Mid.MM_MICROSOFT:
                    return "Microsoft Corporation";
                case Mid.MM_CREATIVE:
                    return "Creative Labs, Inc.";
                case Mid.MM_MEDIAVISION:
                    return "Media Vision, Inc.";
                case Mid.MM_FUJITSU:
                    return "Fujitsu, Ltd.";
                case Mid.MM_ARTISOFT:
                    return "Artisoft, Inc.";
                case Mid.MM_TURTLE_BEACH:
                    return "Turtle Beach Systems";
                case Mid.MM_IBM:
                    return "International Business Machines";
                case Mid.MM_VOCALTEC:
                    return "VocalTec, Inc.";
                case Mid.MM_ROLAND:
                    return "Roland Corporation";
                case Mid.MM_DSP_SOLUTIONS:
                    return "DSP Solutions, Inc.";
                case Mid.MM_NEC:
                    return "NEC Corporation";
                case Mid.MM_ATI:
                    return "ATI Technologies, Inc.";
                case Mid.MM_WANGLABS:
                    return "Wang Laboratories";
                case Mid.MM_TANDY:
                    return "Tandy Corporation";
                case Mid.MM_VOYETRA:
                    return "Voyetra Technologies";
                case Mid.MM_ANTEX:
                    return "Antex Electronics Corporation";
                case Mid.MM_ICL_PS:
                    return "ICL Personal Systems";
                case Mid.MM_INTEL:
                    return "Intel Corporation";
                case Mid.MM_GRAVIS:
                    return "Advanced Gravis Computer Technology, Ltd.";
                case Mid.MM_VAL:
                    return "Video Associates Labs, Inc.";
                case Mid.MM_INTERACTIVE:
                    return "InterActive, Inc.";
                case Mid.MM_YAMAHA:
                    return "Yamaha Corporation of America";
                case Mid.MM_EVEREX:
                    return "Everex Systems, Inc.";
                case Mid.MM_ECHO:
                    return "Echo Speech Corporation";
                case Mid.MM_SIERRA:
                    return "Sierra Semiconductor Corporation";
                case Mid.MM_CAT:
                    return "Computer Aided Technology, Inc.";
                case Mid.MM_APPS:
                    return "APPS Software";
                case Mid.MM_DSP_GROUP:
                    return "DSP Group, Inc.";
                case Mid.MM_MELABS:
                    return "microEngineering Labs";
                case Mid.MM_COMPUTER_FRIENDS:
                    return "Computer Friends, Inc.";
                case Mid.MM_ESS:
                    return "ESS Technology, Inc.";
                case Mid.MM_AUDIOFILE:
                    return "Audio, Inc.";
                case Mid.MM_MOTOROLA:
                    return "Motorola, Inc.";
                case Mid.MM_CANOPUS:
                    return "Canopus, Co., Ltd.";
                case Mid.MM_EPSON:
                    return "Seiko Epson Corporation, Inc.";
                case Mid.MM_TRUEVISION:
                    return "Truevision, Inc.";
                case Mid.MM_AZTECH:
                    return "Aztech Labs, Inc.";
                case Mid.MM_VIDEOLOGIC:
                    return "VideoLogic, Inc.";
                case Mid.MM_SCALACS:
                    return "SCALACS";
                case Mid.MM_KORG:
                    return "Toshihiko Okuhura, Korg, Inc.";
                case Mid.MM_APT:
                    return "Audio Processing Technology";
                case Mid.MM_ICS:
                    return "Integrated Circuit Systems, Inc.";
                case Mid.MM_ITERATEDSYS:
                    return "Iterated Systems, Inc.";
                case Mid.MM_METHEUS:
                    return "Metheus Corporation";
                case Mid.MM_LOGITECH:
                    return "Logitech, Inc.";
                case Mid.MM_WINNOV:
                    return "Winnov, LP";
                case Mid.MM_NCR:
                    return "NCR Corporation";
                case Mid.MM_EXAN:
                    return "EXAN, Ltd.";
                case Mid.MM_AST:
                    return "AST Research, Inc.";
                case Mid.MM_WILLOWPOND:
                    return "Willow Pond Corporation";
                case Mid.MM_SONICFOUNDRY:
                    return "Sonic Foundry";
                case Mid.MM_VITEC:
                    return "Visual Information Technologies, Inc.";
                case Mid.MM_MOSCOM:
                    return "MOSCOM Corporation";
                case Mid.MM_SILICONSOFT:
                    return "Silicon Software, Inc.";
                case Mid.MM_SUPERMAC:
                    return "Supermac Technology, Inc.";
                case Mid.MM_AUDIOPT:
                    return "Audio Processing Technology";
                case Mid.MM_SPEECHCOMP:
                    return "Speech Compression";
                case Mid.MM_DOLBY:
                    return "Dolby Laboratories, Inc.";
                case Mid.MM_OKI:
                    return "OKI";
                case Mid.MM_AURAVISION:
                    return "Auravision Corporation";
                case Mid.MM_OLIVETTI:
                    return "Ing. C. Olivetti & C., S.p.A.";
                case Mid.MM_IOMAGIC:
                    return "I/O Magic Corporation";
                case Mid.MM_MATSUSHITA:
                    return "Matsushita Electric Corporation of America";
                case Mid.MM_CONTROLRES:
                    return "Control Resources Corporation";
                case Mid.MM_XEBEC:
                    return "Xebec Multimedia Solutions Limited";
                case Mid.MM_NEWMEDIA:
                    return "New Media Corporation";
                case Mid.MM_NMS:
                    return "Natural MicroSystems Corporation";
                case Mid.MM_LYRRUS:
                    return "Lyrrus, Inc.";
                case Mid.MM_COMPUSIC:
                    return "Compusic";
                case Mid.MM_OPTI:
                    return "OPTi, Inc.";
                case Mid.MM_DIALOGIC:
                    return "Dialogic Corporation";
                default:
                    return "Unknow";
            }
        }

        #endregion

        private byte Prev_Speaker_Vol = 0;
        private bool Prev_Speaker_Mute = false;
        private byte Prev_WaveOut_Vol = 0;
        private bool Prev_WaveOut_Mute = false;

        public void Play(string FileName, byte Vol)
        {
            try
            {
                GetVolume(ComponentType.DST_SPEAKERS, ref Prev_Speaker_Vol, ref Prev_Speaker_Mute);
                GetVolume(ComponentType.SRC_WAVEOUT, ref Prev_WaveOut_Vol, ref Prev_WaveOut_Mute);
                SetVolume(ComponentType.DST_SPEAKERS, Vol, false);
                SetVolume(ComponentType.SRC_WAVEOUT, Vol, false);

                long Result = sndPlaySound(FileName, SND_ASYNC);
            }
            catch (Exception exp)
            {

            }
        }

        ///  
        /// 设置静音 
        ///  
        ///  
        public void SetMute(bool bMute)
        {
            byte vol = 0;
            bool bPrev = false;
            try
            {
                GetVolume(ComponentType.SRC_MICROPHONE, ref vol, ref bPrev);

                SetVolume(ComponentType.SRC_MICROPHONE, vol, bMute);
            }
            catch (Exception exp)
            {

            }

        }
        ///  
        /// 判断是否静音 
        ///  
        ///  
        public bool GetMute()
        {
            byte vol = 0;
            bool bPrev = false;
            try
            {
                GetVolume(ComponentType.DST_SPEAKERS, ref vol, ref bPrev);
            }
            catch (Exception exp)
            {

            }
            if (bPrev)
                return true;
            else
                return false;
        }
        ///  
        /// 提取当前音量 
        ///  
        ///  
        public int GetSound()
        {
            byte vol = 0;
            bool bPrev = false;
            try
            {
                GetVolume(ComponentType.DST_SPEAKERS, ref vol, ref bPrev);
            }
            catch (Exception exp)
            {

            }
            return vol;
        }
        ///  
        /// 设置音量 
        ///  
        ///  
        public void SetSound(int volume)
        {
            try
            {
                byte vol = 0;
                bool bPrev = false;
                GetVolume(ComponentType.DST_SPEAKERS, ref vol, ref bPrev);
                SetVolume(ComponentType.DST_SPEAKERS, (byte)(vol + volume), bPrev);
            }
            catch (Exception exp)
            {

            }
        }

        public void PlayLoop(string FileName, byte Vol)
        {

            GetVolume(ComponentType.DST_SPEAKERS, ref Prev_Speaker_Vol, ref Prev_Speaker_Mute);
            GetVolume(ComponentType.SRC_WAVEOUT, ref Prev_WaveOut_Vol, ref Prev_WaveOut_Mute);
            SetVolume(ComponentType.DST_SPEAKERS, Vol, false);
            SetVolume(ComponentType.SRC_WAVEOUT, Vol, false);

            long Result = sndPlaySound(FileName, SND_ASYNC | SND_LOOP);
        }

        public void StopPlay()
        {
            long Result = sndPlaySound("", SND_STOP);
            SetVolume(ComponentType.DST_SPEAKERS, Prev_Speaker_Vol, Prev_Speaker_Mute);
            SetVolume(ComponentType.SRC_WAVEOUT, Prev_WaveOut_Vol, Prev_WaveOut_Mute);
        }

        private bool GetVolume(ComponentType Source, ref byte Volume, ref bool Mute)
        {
            uint hmixer = 0;

            bool bGetVolume = false;
            MMSYSERR Rc;

            if (mixerGetNumDevs() < 1)
            {
                MessageBox.Show("No Sound card!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                bGetVolume = false;
            }
            else
            {
                Rc = mixerOpen(ref hmixer, 0, 0, 0, 0);
                if (MMSYSERR.NOERROR != Rc)
                {
                    MessageBox.Show("MixerOpen Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    bGetVolume = false;
                }
                else
                {
                    bGetVolume = true;
                    long Value;

                    Value = GetControlDetails(hmixer, Source, ControlType.VOLUME);
                    if (Value > -1)
                        Volume = Convert.ToByte(Value);
                    else
                        bGetVolume = false;

                    Value = GetControlDetails(hmixer, Source, ControlType.MUTE);
                    if (Value > -1)
                        Mute = Value != 0;
                    else
                        bGetVolume = false;

                }
                Rc = mixerClose(hmixer);
            }
            return bGetVolume;

        }

        private long GetControlDetails(uint hmixer, ComponentType componentType, ControlType ctrlType)
        {
            uint hmem;
            MMSYSERR Rc;
            MIXERCONTROL mxc = new MIXERCONTROL();
            MIXERLINECONTROLS mxlc = new MIXERLINECONTROLS();
            MIXERLINE mxl = new MIXERLINE();
            MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
            MIXERCONTROLDETAILS_UNSIGNED mxcdu = new MIXERCONTROLDETAILS_UNSIGNED();

            mxl.cbStruct = StructLen(mxl);
            mxl.dwComponentType = componentType;

            Rc = mixerGetLineInfo(hmixer, ref mxl, GetLineInfoFlag.COMPONENTTYPE);
            if (MMSYSERR.NOERROR == Rc)
            {
                mxlc.cbStruct = StructLen(mxlc);
                mxlc.dwLineID = mxl.dwLineID;
                mxlc.dwControl = Convert.ToUInt32(ctrlType);
                mxlc.cControls = mxl.cControls;
                mxlc.cbmxctrl = StructLen(mxc);

                hmem = GlobalAlloc(64, StructLen(mxc));
                mxlc.pamxctrl = GlobalLock(hmem);
                mxc.cbStruct = StructLen(mxc);

                Rc = mixerGetLineControls(hmixer, ref mxlc, GetLineControlsFlag.ONEBYTYPE);
                if (MMSYSERR.NOERROR == Rc)
                {
                    RtlMoveMemory(ref mxc, mxlc.pamxctrl, StructLen(mxc));
                    GlobalFree(hmem);

                    mxcd.item = 0;
                    mxcd.dwControlID = mxc.dwControlID;
                    mxcd.cbStruct = StructLen(mxcd);
                    mxcd.cChannels = 1;

                    hmem = GlobalAlloc(64, StructLen(mxcdu));
                    mxcd.cbDetails = StructLen(mxcdu);
                    mxcd.paDetails = GlobalLock(hmem);
                    Rc = mixerGetControlDetails(hmixer, ref mxcd, GetControlDetailsFlag.VALUE);
                    if (Rc == MMSYSERR.NOERROR)
                    {
                        RtlMoveMemory(ref mxcdu, mxcd.paDetails, StructLen(mxcdu));
                        GlobalFree(hmem);
                        return 100 * (mxcdu.dwValue - mxc.dwMinimum) / (mxc.dwMaximum - mxc.dwMinimum);
                    }
                    else
                    {
                        GlobalFree(hmem);
                        MessageBox.Show("mixerGetControlDetails Failed!!", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                        return -1;
                    }
                }
                else
                {
                    MessageBox.Show("mixerGetLineControls Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    GlobalFree(hmem);
                    return -1;
                }
            }
            else
            {
                MessageBox.Show("mixerGetLineInfo Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                return -1;
            }
        }

        private bool SetVolume(ComponentType Source, byte Volume, bool Mute)
        {
            uint hmixer = 0;

            bool bSetVolume = false;
            MIXERCONTROL mxc = new MIXERCONTROL();
            MMSYSERR Rc;

            if (mixerGetNumDevs() < 1)
            {
                MessageBox.Show("No Sound card!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                bSetVolume = false;
            }
            else
            {
                Rc = mixerOpen(ref hmixer, 0, 0, 0, 0);
                if (MMSYSERR.NOERROR != Rc)
                {
                    MessageBox.Show("MixerOpen Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    bSetVolume = false;
                }
                else
                {
                    bSetVolume = true;
                    bSetVolume = bSetVolume | SetControlDetails(hmixer, Source, ControlType.VOLUME, Volume);

                    if (Mute == true)
                        bSetVolume = bSetVolume | SetControlDetails(hmixer, Source, ControlType.MUTE, 100);
                    else
                        bSetVolume = bSetVolume | SetControlDetails(hmixer, Source, ControlType.MUTE, 0);

                }
                Rc = mixerClose(hmixer);
            }
            return bSetVolume;

        }


        private bool SetControlDetails(uint hmixer, ComponentType componentType, ControlType ctrlType, byte Value)
        {
            uint hmem;
            MMSYSERR Rc;
            MIXERCONTROL mxc = new MIXERCONTROL();
            MIXERLINECONTROLS mxlc = new MIXERLINECONTROLS();
            MIXERLINE mxl = new MIXERLINE();
            MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
            MIXERCONTROLDETAILS_UNSIGNED mxcdu = new MIXERCONTROLDETAILS_UNSIGNED();

            mxl.cbStruct = StructLen(mxl);
            mxl.dwComponentType = componentType;

            Rc = mixerGetLineInfo(hmixer, ref mxl, GetLineInfoFlag.COMPONENTTYPE);
            if (MMSYSERR.NOERROR == Rc)
            {
                mxlc.cbStruct = StructLen(mxlc);
                mxlc.dwLineID = mxl.dwLineID;
                mxlc.dwControl = Convert.ToUInt32(ctrlType);
                mxlc.cControls = mxl.cControls;
                mxlc.cbmxctrl = StructLen(mxc);

                hmem = GlobalAlloc(64, StructLen(mxc));
                mxlc.pamxctrl = GlobalLock(hmem);
                mxc.cbStruct = StructLen(mxc);

                Rc = mixerGetLineControls(hmixer, ref mxlc, GetLineControlsFlag.ONEBYTYPE);
                if (MMSYSERR.NOERROR == Rc)
                {
                    RtlMoveMemory(ref mxc, mxlc.pamxctrl, StructLen(mxc));
                    GlobalFree(hmem);

                    mxcd.item = 0;
                    mxcd.dwControlID = mxc.dwControlID;
                    mxcd.cbStruct = StructLen(mxcd);
                    mxcd.cbDetails = StructLen(mxcdu);

                    hmem = GlobalAlloc(64, StructLen(mxcdu));
                    mxcd.paDetails = GlobalLock(hmem);
                    mxcd.cChannels = 1;
                    mxcdu.dwValue = mxc.dwMinimum + Convert.ToUInt32(Value * (mxc.dwMaximum - mxc.dwMinimum) / 100);

                    RtlMoveMemory(mxcd.paDetails, ref mxcdu, StructLen(mxcdu));

                    Rc = mixerSetControlDetails(hmixer, ref mxcd, SetControlDetailsFlag.VALUE);
                    GlobalFree(hmem);
                    if (MMSYSERR.NOERROR == Rc)
                        return true;
                    else
                    {
                        MessageBox.Show("mixerSetControlDetails Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("mixerGetLineControls Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("mixerGetLineInfo Failed!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                return false;
            }
        }
    }

}
