using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DisplayResolutionLibrary
{
    public sealed class Resolution
    {
        /// <summary>
        /// Get the designation (name) of the resolution
        /// </summary>
        public string Designation { get; private set; }

        /// <summary>
        /// Get the width of the frame
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Get the height of the frame
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Get the total amount of pixels in the frame
        /// </summary>
        public int Pixels { get { return Width * Height; } }

        private string _aspectRatio = "";
        /// <summary>
        /// Get the aspect ratio of the frame. Some ratios are defined rather than calculated. If no ratio was defined, the true aspect ratio is calculated.
        /// </summary>
        public string aspectRatio
        {
            get
            {
                if (!string.IsNullOrEmpty(_aspectRatio))
                    return _aspectRatio;

                // If no aspect ratio was defined, calculate the true aspect ratio of the frame using extended euclidean.
                int r, a = Width, b = Height;
                while (b != 0)
                {
                    r = a % b;
                    a = b;
                    b = r;
                }
                // Simplify the resolution and store the aspect ratio.
                _aspectRatio = string.Format("{0}:{1}", Width / a, Height / a);

                return _aspectRatio;
            }
        }

        /// <summary>
        /// Represents a predefined resolution with a (common) name and aspect ratio.
        /// </summary>
        /// <param name="Width">Sets the width of the frame</param>
        /// <param name="Height">Sets the height of the frame</param>
        /// <param name="Designation">Sets the name of the resolution</param>
        /// <param name="AspectRatio">Sets a custom aspect ratio for the resolution. Format should be "INT:INT". If left empty, the aspect ratio will be calculated.</param>
        public Resolution(int width, int height, string designation = "", string aspectRatio = "")
        {
            Width = Clamp(width, 1, int.MaxValue);
            Height = Clamp(height, 1, int.MaxValue); ;
            Designation = designation;

            if (Regex.IsMatch(aspectRatio, @"^\d*:\d*$")) // Only accept valid formats
                _aspectRatio = aspectRatio;
        }

        internal T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        #region Operators

        public override bool Equals(object obj)
        {
            return this == (Resolution)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static int Compare(Resolution first, Resolution second)
        {
            if (first == null && second == null)
                return (0);     // Equal
            if(first == null)
                return (-1);    // null < non-null
            if (second == null)
                return (1);     // non-null > null

            if (first < second)
                return (-1);
            if (first == second)
                return (0);
            return (1); // first > second
        }

        /// <summary>
        /// Checks if the width and height of the resolutions are equal.
        /// </summary>
        public static bool operator ==(Resolution first, Resolution second)
        {
            return first?.Width == second?.Width && first?.Height == second?.Height;
        }

        /// <summary>
        /// Checks if the width and height of the resolutions are unequal.
        /// </summary>
        public static bool operator !=(Resolution first, Resolution second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Checks if the first resolution has less pixels than the second resolution.
        /// </summary>
        public static bool operator <(Resolution first, Resolution second)
        {
            return first?.Pixels < second?.Pixels;
        }

        /// <summary>
        /// Checks if the first resolution has more pixels than the second resolution.
        /// </summary>
        public static bool operator >(Resolution first, Resolution second)
        {
            return first?.Pixels > second?.Pixels;
        }

        /// <summary>
        /// Checks if the first resolution has less or equally as many pixels as the second resolution.
        /// </summary>
        public static bool operator <=(Resolution first, Resolution second)
        {
            return first?.Pixels <= second?.Pixels;
        }

        /// <summary>
        /// Checks if the first resolution has more or equally as many pixels as the second resolution.
        /// </summary>
        public static bool operator >=(Resolution first, Resolution second)
        {
            return first?.Pixels >= second?.Pixels;
        }

        #endregion
    }

    /// <summary>
    /// Represents the total list of predefined resolutions.
    /// </summary>
    public static class Resolutions
    {
        /// Sources:
        /// https://en.wikipedia.org/wiki/Common_Intermediate_Format
        /// https://en.wikipedia.org/wiki/List_of_common_resolutions
        /// https://en.wikipedia.org/wiki/Graphics_display_resolution
        /// https://en.wikipedia.org/wiki/Computer_display_standard
        /// https://en.wikipedia.org/wiki/Image_resolution
        /// https://en.wikipedia.org/wiki/Ultrawide_formats

        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            List<Resolution> l = new List<Resolution>();

            l.AddRange(CommonIntermediateFormat.GetList());
            l.AddRange(HighDefinition.GetList());
            l.AddRange(VideoGraphicsArray.GetList());
            l.AddRange(ExtendedGraphicsArray.GetList());
            l.AddRange(QuadExtendedGraphicsArray.GetList());
            l.AddRange(HyperExtendedGraphicsArray.GetList());
            l.AddRange(UltraWideCinema.GetList());
            l.AddRange(UltraWideScreen.GetList());

            l.Add(WPAL);
            l.Add(Infinity);

            return l;
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution SQCIF  { get { return new Resolution( 128,   96, nameof(SQCIF),    "16:9" ); } }
        public static Resolution QCIF   { get { return new Resolution( 176,  144, nameof(QCIF),     "16:9" ); } }
        public static Resolution SIF    { get { return new Resolution( 352,  240, nameof(SIF),      "16:9" ); } }
        public static Resolution CIF    { get { return new Resolution( 352,  288, nameof(CIF),      "16:9" ); } }
        public static Resolution SIF4   { get { return new Resolution( 704,  480, nameof(SIF4),     "16:9" ); } }
        public static Resolution CIF4   { get { return new Resolution( 704,  576, nameof(CIF4),     "16:9" ); } }
        public static Resolution CIF16  { get { return new Resolution(1408, 1152, nameof(CIF16),    "16:9" ); } }

        public static Resolution nHD    { get { return new Resolution( 640,  360, nameof(nHD),      "16:9" ); } }
        public static Resolution qHD    { get { return new Resolution( 960,  540, nameof(qHD),      "16:9" ); } }
        public static Resolution HD     { get { return new Resolution(1280,  720, nameof(HD),       "16:9" ); } }
        public static Resolution HDp    { get { return new Resolution(1600,  900, nameof(HDp),      "16:9" ); } }
        public static Resolution FHD    { get { return new Resolution(1920, 1080, nameof(FHD),      "16:9" ); } }
        public static Resolution FHDp   { get { return new Resolution(2160, 1440, nameof(FHDp),     "3:2"  ); } }
        public static Resolution DCI2K  { get { return new Resolution(2048, 1080, nameof(DCI2K),    "19:10"); } }
        public static Resolution QHD    { get { return new Resolution(2560, 1440, nameof(QHD),      "16:9" ); } }
        public static Resolution QHDp   { get { return new Resolution(3200, 1800, nameof(QHDp),     "16:9" ); } }
        public static Resolution UWQHD  { get { return new Resolution(3440, 1440, nameof(UWQHD),    "43:18"); } }
        public static Resolution UW4K   { get { return new Resolution(3840, 1600, nameof(UW4K),     "12:5" ); } }
        public static Resolution UHD4K  { get { return new Resolution(3840, 2160, nameof(UHD4K),    "16:9" ); } }
        public static Resolution DCI4K  { get { return new Resolution(4096, 2160, nameof(DCI4K),    "19:10"); } }
        public static Resolution UW5K   { get { return new Resolution(5120, 2160, nameof(UW5K),     "21:9" ); } }
        public static Resolution UHDp5K { get { return new Resolution(5120, 2880, nameof(UHDp5K),   "16:9" ); } }
        public static Resolution UHD8K  { get { return new Resolution(7680, 4320, nameof(UHD8K),    "16:9" ); } }
        public static Resolution UW10K  { get { return new Resolution(10240,4320, nameof(UW10K),    "21:9" ); } }
        public static Resolution UHD16K { get { return new Resolution(15360,8640, nameof(UHD16K),   "16:9" ); } }
        public static Resolution UHD64K { get { return new Resolution(61440,34560,nameof(UHD64K),   "16:9" ); } }

        public static Resolution QQVGA  { get { return new Resolution( 160,  120, nameof(QQVGA),    "4:3"  ); } }
        public static Resolution HQVGA  { get { return new Resolution( 240,  160, nameof(HQVGA),    "3:2"  ); } }
        public static Resolution QVGA   { get { return new Resolution( 320,  240, nameof(QVGA),     "4:3"  ); } }
        public static Resolution WQVGA  { get { return new Resolution( 400,  240, nameof(WQVGA),    "5:3"  ); } }
        public static Resolution qSVGA  { get { return new Resolution( 400,  300, nameof(qSVGA),    "4:3"  ); } }
        public static Resolution HVGA   { get { return new Resolution( 480,  320, nameof(HVGA),     "3:2"  ); } }
        public static Resolution VGA    { get { return new Resolution( 640,  480, nameof(VGA),      "4:3"  ); } }
        public static Resolution WVGA   { get { return new Resolution( 768,  480, nameof(WVGA),     "16:10"); } }
        public static Resolution WGA    { get { return new Resolution( 800,  480, nameof(WGA),      "5:3"  ); } }
        public static Resolution FWVGA  { get { return new Resolution( 854,  480, nameof(FWVGA),    "16:9" ); } }
        public static Resolution SVGA   { get { return new Resolution( 800,  600, nameof(SVGA),     "4:3"  ); } }
        public static Resolution DVGA   { get { return new Resolution( 960,  640, nameof(DVGA),     "3:2"  ); } }
        public static Resolution WSVGA  { get { return new Resolution(1024,  576, nameof(WSVGA),    "16:9" ); } }
        
        public static Resolution XGA    { get { return new Resolution(1024,  768, nameof(XGA),      "4:3"  ); } }
        public static Resolution WXGA   { get { return new Resolution(1280,  768, nameof(WXGA),     "5:3"  ); } }
        public static Resolution FWXGA  { get { return new Resolution(1366,  768, nameof(FWXGA),    "16:9" ); } }
        public static Resolution XGAp   { get { return new Resolution(1152,  864, nameof(XGAp),     "4:3"  ); } }
        public static Resolution WXGAp  { get { return new Resolution(1440,  900, nameof(WXGAp),    "16:10"); } }
        public static Resolution WSXGA  { get { return new Resolution(1440,  960, nameof(WSXGA),    "3:2"  ); } }
        public static Resolution SXGAm  { get { return new Resolution(1280,  960, nameof(SXGAm),    "4:3"  ); } }
        public static Resolution SXGA   { get { return new Resolution(1280, 1024, nameof(SXGA),     "5:4"  ); } }
        public static Resolution SXGAp  { get { return new Resolution(1400, 1050, nameof(SXGAp),    "4:3"  ); } }
        public static Resolution WSXGAp { get { return new Resolution(1680, 1050, nameof(WSXGAp),   "16:10"); } }
        public static Resolution UXGA   { get { return new Resolution(1600, 1200, nameof(UXGA),     "4:3"  ); } }
        public static Resolution WUXGA  { get { return new Resolution(1920, 1200, nameof(WUXGA),    "16:10"); } }
        public static Resolution TXGA   { get { return new Resolution(1920, 1400, nameof(TXGA),     "7:5"  ); } }
        public static Resolution CWSXGA { get { return new Resolution(2880,  900, nameof(CWSXGA),   "16:5" ); } }

        public static Resolution QWXGA  { get { return new Resolution(2048, 1152, nameof(QWXGA),    "16:9" ); } }
        public static Resolution QXGA   { get { return new Resolution(2048, 1536, nameof(QXGA),     "4:3"  ); } }
        public static Resolution WQXGA  { get { return new Resolution(2560, 1600, nameof(WQXGA),    "16:10"); } }
        public static Resolution QSXGA  { get { return new Resolution(2560, 2048, nameof(QSXGA),    "5:4"  ); } }
        public static Resolution WQSXGA { get { return new Resolution(3200, 2048, nameof(WQSXGA),   "25:16"); } }
        public static Resolution QUXGA  { get { return new Resolution(3200, 2400, nameof(QUXGA),    "4:3"  ); } }
        public static Resolution WQUXGA { get { return new Resolution(3840, 2400, nameof(WQUXGA),   "16:10"); } }
        
        public static Resolution HXGA   { get { return new Resolution(4096, 3072, nameof(HXGA),     "4:3"  ); } }
        public static Resolution WHXGA  { get { return new Resolution(5120, 3200, nameof(WHXGA),    "16:10"); } }
        public static Resolution HSXGA  { get { return new Resolution(5120, 4096, nameof(HSXGA),    "5:4"  ); } }
        public static Resolution WHSXGA { get { return new Resolution(6400, 4096, nameof(WHSXGA),   "25:16"); } }
        public static Resolution HUXGA  { get { return new Resolution(6400, 4800, nameof(HUXGA),    "4:3"  ); } }
        public static Resolution WHUXGA { get { return new Resolution(7680, 4800, nameof(WHUXGA),   "16:10"); } }

        public static Resolution WHD    { get { return new Resolution( 1720,  720, nameof(WHD),     "43:18"); } }
        public static Resolution WHDp   { get { return new Resolution( 2160,  900, nameof(WHDp),    "12:5" ); } }
        public static Resolution WFHD   { get { return new Resolution( 2560, 1080, nameof(WFHD),    "64:27"); } }
        public static Resolution WFHDp  { get { return new Resolution( 2880, 1200, nameof(WFHDp),   "12:5" ); } }
        public static Resolution WQHD   { get { return new Resolution( 3440, 1440, nameof(WQHD),    "43:18"); } }
        public static Resolution UW1600 { get { return new Resolution( 3840, 1600, nameof(UW1600),  "12:5" ); } }
        public static Resolution WQHDp  { get { return new Resolution( 4320, 1800, nameof(WQHDp),   "12:5" ); } }
        public static Resolution WUHD   { get { return new Resolution( 5120, 2160, nameof(WUHD),    "64:27"); } }
        public static Resolution UW2400 { get { return new Resolution( 5760, 2400, nameof(UW2400),  "12:5" ); } }
        public static Resolution UW2880 { get { return new Resolution( 6880, 2880, nameof(UW2880),  "43:18"); } }
        public static Resolution UW4320 { get { return new Resolution(10240, 4320, nameof(UW4320),  "64:27"); } }

        public static Resolution DFHD   { get { return new Resolution(3840, 1080, nameof(DFHD),     "32:9" ); } }
        public static Resolution DFHDp  { get { return new Resolution(3840, 1200, nameof(DFHDp),    "16:5" ); } }
        public static Resolution SWFHDp { get { return new Resolution(4320, 1200, nameof(SWFHDp),   "18:5" ); } }
        public static Resolution DQHD   { get { return new Resolution(5120, 1440, nameof(DQHD),     "32:9" ); } }
        public static Resolution DQHDp  { get { return new Resolution(5120, 1600, nameof(DQHDp),    "16:5" ); } }
        public static Resolution SWQHDp { get { return new Resolution(5760, 1600, nameof(SWQHDp),   "18:5" ); } }
        public static Resolution SW6K   { get { return new Resolution(6480, 1800, nameof(SW6K),     "18:5" ); } }
        public static Resolution SW8K   { get { return new Resolution(8640, 2400, nameof(SW8K),     "18:5" ); } }

        public static Resolution WPAL   { get { return new Resolution( 848,  480, nameof(WPAL),     "53:30"); } }
        public static Resolution Infinity{get { return new Resolution(2960, 1440, nameof(Infinity), "18.5:9");} }
    }

    /// <summary>
    /// Represents only predefined Common Intermediate Format resolutions.
    /// </summary>
    public static class CommonIntermediateFormat
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { SQCIF, QCIF, SIF, CIF, SIF4, CIF4, CIF16 };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution SQCIF  { get { return Resolutions.SQCIF;  } }
        public static Resolution QCIF   { get { return Resolutions.QCIF;   } }
        public static Resolution SIF    { get { return Resolutions.SIF;    } }
        public static Resolution CIF    { get { return Resolutions.CIF;    } }
        public static Resolution SIF4   { get { return Resolutions.SIF4;   } }
        public static Resolution CIF4   { get { return Resolutions.CIF4;   } }
        public static Resolution CIF16  { get { return Resolutions.CIF16;  } }
    }

    /// <summary>
    /// Represents only predefined HD resolutions.
    /// </summary>
    public static class HighDefinition
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { nHD, qHD, HD, HDp, FHD, FHDp, DCI2K, QHD, QHDp, UWQHD, UW4K, UHD4K, DCI4K, UW5K, UHDp5K, UHD8K, UW10K, UHD16K, UHD64K };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution nHD    { get { return Resolutions.nHD;    } }
        public static Resolution qHD    { get { return Resolutions.qHD;    } }
        public static Resolution HD     { get { return Resolutions.HD;     } }
        public static Resolution HDp    { get { return Resolutions.HDp;    } }
        public static Resolution FHD    { get { return Resolutions.FHD;    } }
        public static Resolution FHDp   { get { return Resolutions.FHDp;   } }
        public static Resolution DCI2K  { get { return Resolutions.DCI2K;  } }
        public static Resolution QHD    { get { return Resolutions.QHD;    } }
        public static Resolution QHDp   { get { return Resolutions.QHDp;   } }
        public static Resolution UWQHD  { get { return Resolutions.UWQHD;  } }
        public static Resolution UW4K   { get { return Resolutions.UW4K;   } }
        public static Resolution UHD4K  { get { return Resolutions.UHD4K;  } }
        public static Resolution DCI4K  { get { return Resolutions.DCI4K;  } }
        public static Resolution UW5K   { get { return Resolutions.UW5K;   } }
        public static Resolution UHDp5K { get { return Resolutions.UHDp5K; } }
        public static Resolution UHD8K  { get { return Resolutions.UHD8K;  } }
        public static Resolution UW10K  { get { return Resolutions.UW10K;  } }
        public static Resolution UHD16K { get { return Resolutions.UHD16K; } }
        public static Resolution UHD64K { get { return Resolutions.UHD64K; } }
    }

    /// <summary>
    /// Represents only predefined VGA resolutions.
    /// </summary>
    public static class VideoGraphicsArray
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { QQVGA, HQVGA, QVGA, WQVGA, qSVGA, HVGA, VGA, WVGA, WGA, FWVGA, SVGA, DVGA, WSVGA };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution QQVGA  { get { return Resolutions.QQVGA;  } }
        public static Resolution HQVGA  { get { return Resolutions.HQVGA;  } }
        public static Resolution QVGA   { get { return Resolutions.QVGA;   } }
        public static Resolution WQVGA  { get { return Resolutions.WQVGA;  } }
        public static Resolution qSVGA  { get { return Resolutions.qSVGA;  } }
        public static Resolution HVGA   { get { return Resolutions.HVGA;   } }
        public static Resolution VGA    { get { return Resolutions.VGA;    } }
        public static Resolution WVGA   { get { return Resolutions.WVGA;   } }
        public static Resolution WGA    { get { return Resolutions.WGA;    } }
        public static Resolution FWVGA  { get { return Resolutions.FWVGA;  } }
        public static Resolution SVGA   { get { return Resolutions.SVGA;   } }
        public static Resolution DVGA   { get { return Resolutions.DVGA;   } }
        public static Resolution WSVGA  { get { return Resolutions.WSVGA;  } }
    }

    /// <summary>
    /// Represents only predefined XGA resolutions.
    /// </summary>
    public static class ExtendedGraphicsArray
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { XGA, WXGA, FWXGA, XGAp, WXGAp, WSXGA, SXGAm, SXGA, SXGAp, WSXGAp, UXGA, WUXGA, TXGA, CWSXGA };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution XGA    { get { return Resolutions.XGA;    } }
        public static Resolution WXGA   { get { return Resolutions.WXGA;   } }
        public static Resolution FWXGA  { get { return Resolutions.FWXGA;  } }
        public static Resolution XGAp   { get { return Resolutions.XGAp;   } }
        public static Resolution WXGAp  { get { return Resolutions.WXGAp;  } }
        public static Resolution WSXGA  { get { return Resolutions.WSXGA;  } }
        public static Resolution SXGAm  { get { return Resolutions.SXGAm;  } }
        public static Resolution SXGA   { get { return Resolutions.SXGA;   } }
        public static Resolution SXGAp  { get { return Resolutions.SXGAp;  } }
        public static Resolution WSXGAp { get { return Resolutions.WSXGAp; } }
        public static Resolution UXGA   { get { return Resolutions.UXGA;   } }
        public static Resolution WUXGA  { get { return Resolutions.WUXGA;  } }
        public static Resolution TXGA   { get { return Resolutions.TXGA;   } }
        public static Resolution CWSXGA { get { return Resolutions.CWSXGA; } }
    }

    /// <summary>
    /// Represents only predefined QXGA resolutions.
    /// </summary>
    public static class QuadExtendedGraphicsArray
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { QWXGA, QXGA, WQXGA, QSXGA, WQSXGA, QUXGA, WQUXGA };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution QWXGA  { get { return Resolutions.QWXGA;  } }
        public static Resolution QXGA   { get { return Resolutions.QXGA;   } }
        public static Resolution WQXGA  { get { return Resolutions.WQXGA;  } }
        public static Resolution QSXGA  { get { return Resolutions.QSXGA;  } }
        public static Resolution WQSXGA { get { return Resolutions.WQSXGA; } }
        public static Resolution QUXGA  { get { return Resolutions.QUXGA;  } }
        public static Resolution WQUXGA { get { return Resolutions.WQUXGA; } }
    }

    /// <summary>
    /// Represents only predefined HXGA resolutions.
    /// </summary>
    public static class HyperExtendedGraphicsArray
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { HXGA, WHXGA, HSXGA, WHSXGA, HUXGA, WHUXGA };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution HXGA   { get { return Resolutions.HXGA; } }
        public static Resolution WHXGA  { get { return Resolutions.WHXGA; } }
        public static Resolution HSXGA  { get { return Resolutions.HSXGA; } }
        public static Resolution WHSXGA { get { return Resolutions.WHSXGA; } }
        public static Resolution HUXGA  { get { return Resolutions.HUXGA; } }
        public static Resolution WHUXGA { get { return Resolutions.WHUXGA; } }
    }

    /// <summary>
    /// Represents only predefined Ultrawide-Cinema resolutions.
    /// </summary>
    public static class UltraWideCinema
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { WHD, WHDp, WFHD, WQHD, WFHDp, UW1600, WQHDp, WUHD, UW2400, UW2880, UW4320 };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution WHD    { get { return Resolutions.WHD; } }
        public static Resolution WHDp   { get { return Resolutions.WHDp; } }
        public static Resolution WFHD   { get { return Resolutions.WFHD; } }
        public static Resolution WFHDp  { get { return Resolutions.WFHDp; } }
        public static Resolution WQHD   { get { return Resolutions.WQHD; } }
        public static Resolution UW1600 { get { return Resolutions.UW1600; } }
        public static Resolution WQHDp  { get { return Resolutions.WQHDp; } }
        public static Resolution WUHD   { get { return Resolutions.WUHD; } }
        public static Resolution UW2400 { get { return Resolutions.UW2400; } }
        public static Resolution UW2880 { get { return Resolutions.UW2880; } }
        public static Resolution UW4320 { get { return Resolutions.UW4320; } }
    }
    
    /// <summary>
    /// Represents only predefined Ultra-Widescreen resolutions.
    /// </summary>
    public static class UltraWideScreen
    {
        public static Resolution[] GetArray()
        {
            return GetList().ToArray();
        }
        public static List<Resolution> GetList()
        {
            return new List<Resolution> { DFHD, DFHDp, SWFHDp, DQHD, DQHDp, SWQHDp, SW6K, SW8K };
        }
        /// <summary>
        /// Returns a dictionary of the resolutions, indexed by designation.
        /// </summary>
        public static Dictionary<string, Resolution> GetDictionary()
        {
            Dictionary<string, Resolution> d = new Dictionary<string, Resolution>();

            foreach (Resolution r in GetList())
                d[r.Designation] = r;

            return d;
        }

        public static Resolution DFHD   { get { return Resolutions.DFHD; } }
        public static Resolution DFHDp  { get { return Resolutions.DFHDp; } }
        public static Resolution SWFHDp { get { return Resolutions.SWFHDp; } }
        public static Resolution DQHD   { get { return Resolutions.DQHD; } }
        public static Resolution DQHDp  { get { return Resolutions.DQHDp; } }
        public static Resolution SWQHDp { get { return Resolutions.SWQHDp; } }
        public static Resolution SW6K   { get { return Resolutions.SW6K; } }
        public static Resolution SW8K   { get { return Resolutions.SW8K; } }
    }
}