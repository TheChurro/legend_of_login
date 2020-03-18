using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    [System.Serializable]
    public enum FlagInstruction {
        Add,
        Remove,
        Require,
        Disables
    }
    [System.Serializable]
    public struct Flag {
        public FlagInstruction Instruction;
        public string Domain;
        public string Value;
    }
}
