using System;
using System.ComponentModel;

namespace JediCodeplug
{
    public class Block10 : Block
    {
        public override int Id { get => 0x10; }
        public override string Description { get => "Feature Descriptor Block"; }

        #region Definition
        /*  0  1  2  3   4  5  6  7    8  9  A  B   C  D  E  F
        0: 19 07 10 19  7E F0 90 20   FB 00 80 40  00 00 40 00
        1: 00 00 00 80  00 00 00 00   50 30 06 00  00 04 00 00
        2: 00
        */

        private const int CONTENTS_LENGTH = 0x21;
        private const int FEATURE_LENGTH = 0x00;
        private const int FEATURE_BLOCK = 0x01;
        private const int FLASHCODE_LENGTH = 0x1A;
        private const int FLASHCODE = 0x1B;
        #endregion

        #region Propeties
        [DisplayName("Feature Block Length")]
        [Description("Seems to always be 0x19")]
        public byte FeatureBlockLength { get; } = 0x19;

        [DisplayName("Feature Block")]
        public byte[] FeatureBlock { get; } = new byte[0x19];

        [DisplayName("Flashcode Length")]
        [Description("Seems to always be 6")]
        public byte FlashcodeLength { get; } = 0x06;

        [DisplayName("Flashcode")] 
        public byte[] Flashcode { get; } = new byte[0x06];
        #endregion


        public Block10() { }

        public override void Deserialize(byte[] codeplugContents, int address)
        {
            var contents = Deserializer(codeplugContents, address);
            contents.Slice(FEATURE_BLOCK, FeatureBlockLength).CopyTo(FeatureBlock.AsSpan());
            contents.Slice(FLASHCODE, FlashcodeLength).CopyTo(Flashcode.AsSpan());
        }

        public override int Serialize(byte[] codeplugContents, int address)
        {
            var contents = new byte[CONTENTS_LENGTH].AsSpan();
            contents[FEATURE_LENGTH] = FeatureBlockLength;
            FeatureBlock.AsSpan().CopyTo(contents.Slice(FEATURE_BLOCK, FeatureBlockLength));
            contents[FLASHCODE_LENGTH] = FlashcodeLength;
            Flashcode.AsSpan().CopyTo(contents.Slice(FLASHCODE, FlashcodeLength));

            return Serializer(codeplugContents, address, contents) + address;
        }
    }
}
