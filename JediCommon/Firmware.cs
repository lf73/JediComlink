using System;
using System.Linq;

namespace JediCommon
{
    public class Firmware
    {
        public decimal Version { get; set; }
        
        /// <summary>
        /// Selected bytes from the firmware that are feed into the Auth Code calculation
        /// </summary>
        public string Signature { get; set; }
        
        public byte[] SignatureBytes
        {
            get => String.IsNullOrEmpty(Signature) ? null : Enumerable.Range(0, Signature.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(Signature.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// Hash of the firmware bytes, with the Factory Code at 3FFF0 thru 3FFFF erased.
        /// </summary>
        public string MD5Hash { get; set; }
        
        /// <summary>
        /// The copyright year as found in the raw firmware.
        /// </summary>
        public string Year { get; set; }
        
        /// <summary>
        /// Actual Size accounts for 512KB minus all of the unsused bytes at the end of each 16KB page.
        /// </summary>
        public string ActualSize { get; set; }

        /// <summary>
        /// Any notes about the firmware, such as being the final released version.
        /// </summary>
        public string Comments { get; set; }

        public override string ToString()
        {
            return $"{Version:0.00}";
        }

    }
}
