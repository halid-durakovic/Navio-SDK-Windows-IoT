using System;
using System.Collections.Generic;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Provides a mapping between class id / message id and message mnemonic.
    /// </summary>
    class MnemonicProvider
    {
        /// <summary>
        /// The cross reference table of mnemonic name, class id and message id.
        /// </summary>
        public MultiKeyDictionary<byte, byte, string> Mappings { get; private set; }

        /// <summary>
        /// Creates a new provider with a set of default mappings.
        /// </summary>
        public MnemonicProvider()
            : this (new MultiKeyDictionary<byte, byte, string>
            {
                { 0x05 }, { 0x01 }, { "ACK-ACK" }
            });

        /// <summary>
        /// Creates a lookup engine using the provided mapping.
        /// </summary>
        /// <param name="mapping"></param>
        public MnemonicProvider(MultiKeyDictionary<byte, byte, string> mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }
            Mappings = mapping;
        }

        /// <summary>
        /// Given a class id and message id determin the message mnemonic
        /// </summary>
        /// <param name="classId">Message class ID</param>
        /// <param name="messageID">Message ID</param>
        /// <param name="mnemonic">Message Mnemonic</param>
        /// <returns></returns>
        public bool TryGetMnemonic(byte classId, byte messageID, out string mnemonic)
        {
            return Mappings.TryGetValue(classId, messageID, out mnemonic);
        }

        public class MultiKeyDictionary<T1, T2, T3> : Dictionary<T1, Dictionary<T2, T3>>
        {
            new public Dictionary<T2, T3> this[T1 key]
            {
                get
                {
                    if (!ContainsKey(key))
                        Add(key, new Dictionary<T2, T3>());

                    Dictionary<T2, T3> returnObj;
                    TryGetValue(key, out returnObj);

                    return returnObj;
                }
            }
        }
    }
}
