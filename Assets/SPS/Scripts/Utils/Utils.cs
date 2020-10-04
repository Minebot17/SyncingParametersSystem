using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public static class Utils {
        public static readonly System.Random rnd = new System.Random();
    
        [Serializable]
        public class MessagesList : List<MessageBase> { }

        [Serializable]
        public class StringList : List<string> { }

        [Serializable]
        public class MultyStringList : List<List<string>> { }
	
        [Serializable]
        public class Vector3List : List<Vector3> { }
	
        [Serializable]
        public class IntegerList : List<int> {  }

        public static IEnumerable<Type> FindChildesOfType(Type parent) {
            return typeof(Utils).Assembly.GetTypes().Where(t => t.IsSubclassOf(parent));
        }
    }
}