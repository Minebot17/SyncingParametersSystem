using System.Collections.Generic;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public static class Extensions {
        public static U Get<T, U>(this Dictionary<T, U> dict, T key) where U : class {
            U val;
            dict.TryGetValue(key, out val);
            return val;
        }

        public static void Write(this NetworkWriter writer, Player player) {
            writer.Write(player.Id);
        }
    
        public static Player ReadPlayer(this NetworkReader reader) {
            return SPS.GetPlayer(reader.ReadInt32());
        }
    }
}