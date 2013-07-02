using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.IO;

namespace Common
{
    public static partial class PInvoke
    {
        private enum PrimaryLanguage : ushort
        {
            LANG_NEUTRAL = 0x00
        }

        private enum SecondaryLanguage : ushort
        {
            SUBLANG_NEUTRAL = 0x00
        }

        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd318691(v=vs.85).aspx
        private static ushort GetLanguage(PrimaryLanguage primary, SecondaryLanguage secondary)
        {
            var p = (ushort)((ushort)primary & 0x3ff);
            var s = (ushort)((ushort)secondary >> 10) << 10;
            return (ushort)(p | s);
        }


        private static readonly ushort neutralLanguage = GetLanguage(PrimaryLanguage.LANG_NEUTRAL,
            SecondaryLanguage.SUBLANG_NEUTRAL);


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648030(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr BeginUpdateResource(string filename, bool deleteExisting);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648049(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UpdateResource(IntPtr updateHandle, ushort type,
            ushort name, ushort language, byte[] data, uint size);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648032(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndUpdateResource(IntPtr updateHandle, bool discard);


        // converts provided strings into name / bytes[] pairs for string table resource representation
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa381050(v=vs.85).aspx
        public static Dictionary<ushort, byte[]> GetStringTable(IEnumerable<string> texts)
        {
            if (texts == null)
                throw new ArgumentNullException("texts");

            const int stringsPerGroup = 16;

            var result = new Dictionary<ushort, byte[]>();
            var strings = texts.Select(t => t ?? string.Empty).ToArray();

            var groupCount = strings.Length / stringsPerGroup + 1;

            for (var g = 0; g < groupCount; g++)
            {
                var textsGroup = strings.Skip(stringsPerGroup * g).Take(stringsPerGroup).ToList();
                textsGroup.AddRange(Enumerable.Repeat(string.Empty, stringsPerGroup - textsGroup.Count));

                var groupBytesCount = stringsPerGroup * sizeof(ushort) + 
                    textsGroup.Sum(s => s.Length) * sizeof(char);

                using (var groupStream = new MemoryStream(groupBytesCount))
                {
                    using (var groupWriter = new BinaryWriter(groupStream, Encoding.Unicode))
                    {
                        for (int s = 0; s < stringsPerGroup; s++)
                        {
                            var text = textsGroup[s];
                            groupWriter.Write((ushort)text.Length);
                            groupWriter.Write(Encoding.Unicode.GetBytes(text));
                        }

                        result.Add((ushort)(g + 1), groupStream.ToArray());
                    }
                }               
            }

            return result;
        }

        // gets free icon resource names using provided busy names
        public static IEnumerable<ushort> GetFreeIconNames(IEnumerable<ushort> busyNames)
        {
            const ushort firstName = 2;
            const ushort lastName = ushort.MaxValue;

            return Enumerable.Range(firstName, lastName - firstName + 1).
                Select(n => (ushort)n).Except(busyNames);
        }

        public static IntPtr BeginUpdateResource(string filename)
        {
            return BeginUpdateResource(filename, false);
        }

        public static bool UpdateResource(IntPtr updateHandle, ushort type, 
            ushort name, byte[] data)
        {
            return UpdateResource(updateHandle, type, name, neutralLanguage, 
                data, data != null ? (uint)data.Length : 0);
        }

        public static bool DeleteResource(IntPtr updateHandle, ushort type, ushort name)
        {
            return UpdateResource(updateHandle, type, name, neutralLanguage, null, 0);
        }

        public static bool CompleteUpdateResource(IntPtr updateHandle)
        {
            return EndUpdateResource(updateHandle, false);
        }

        public static bool DiscardUpdateResource(IntPtr updateHandle)
        {
            return EndUpdateResource(updateHandle, true);
        }
    }
}
