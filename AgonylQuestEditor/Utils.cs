using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgonylQuestEditor
{
    public static class Utils
    {
        public static Dictionary<uint, Npc> NpcList;

        public static string GetMyDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static string GetNpcName(uint id)
        {
            if (NpcList.ContainsKey(id))
            {
                return NpcList[id].NpcName.Trim();
            }

            return string.Empty;
        }

        public static void LoadNpcData()
        {
            var npcDataFile = File.ReadAllBytes(GetMyDirectory() + Path.DirectorySeparatorChar + "NPC.bin");
            NpcList = new Dictionary<uint, Npc>();
            for (var i = 4; i < npcDataFile.Length; i += 44)
            {
                if (NpcList.ContainsKey(BitConverter.ToUInt32(npcDataFile.Skip(i).Take(4).ToArray(), 0)))
                {
                    continue;
                }

                var item = new Npc()
                {
                    NpcId = BitConverter.ToUInt32(npcDataFile.Skip(i).Take(4).ToArray(), 0),
                    NpcName = System.Text.Encoding.Default.GetString(npcDataFile.Skip(i + 4).Take(32).ToArray()),
                };

                NpcList.Add(item.NpcId, item);
            }
        }

        public static bool IsEmptyData(ushort data)
        {
            return data == 0 || data == 0xffff;
        }

        public static bool IsEmptyData(uint data)
        {
            return data == 0 || data == 0xffffffff;
        }

        public static bool IsEmptyData(string data)
        {
            return string.IsNullOrEmpty(data);
        }

        public static void ReplaceBytesAt(ref byte[] source, int startIndex, byte[] toReplace)
        {
            if (startIndex >= source.Length)
            {
                return;
            }

            for (var i = 0; i < toReplace.Length; i++)
            {
                if (startIndex >= source.Length)
                {
                    break;
                }

                source[startIndex] = toReplace[i];
                startIndex++;
            }
        }
    }
}
