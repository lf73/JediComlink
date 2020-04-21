using System.Text;

namespace JediCodeplug
{
    public static class AuthCode
    {
        private static readonly byte[] KEY = { 0x00, 0x99, 0xAC, 0x35, 0xC7, 0x5E, 0x6B, 0xF2 };
        private static readonly byte[] ORDER = {
            0x16, 0x3D, 0x03, 0x2D, 0x35, 0x07, 0x21, 0x41, 0x4B, 0x32, 0x2E, 0x17, 0x28, 0x05, 0x08, 0x4E,
            0x45, 0x40, 0x43, 0x01, 0x13, 0x46, 0x09, 0x0D, 0x36, 0x39, 0x2B, 0x33, 0x31, 0x1A, 0x3B, 0x12,
            0x3E, 0x14, 0x25, 0x38, 0x0F, 0x1C, 0x4A, 0x1B, 0x27, 0x42, 0x0A, 0x47, 0x48, 0x1E, 0x02, 0x4D,
            0x44, 0x11, 0x4F, 0x04, 0x2A, 0x3F, 0x2C, 0x15, 0x24, 0x2F, 0x0E, 0x1D, 0x0C, 0x06, 0x3A, 0x29,
            0x22, 0x26, 0x1F, 0x20, 0x19, 0x30, 0x49, 0x10, 0x18, 0x37, 0x3C, 0x23, 0x0B, 0x34, 0x4C, 0x00
        };

        private const int MODEL = 0x00;
        private const int FACTORY_CODE = 0x10;
        private const int FLASH_SIGNATURE = 0x20;
        private const int FDB_PART_A = 0x2B;
        private const int FDB_PART_B = 0x44;

        public static byte[] Calculate(Codeplug codeplug)
        {
            byte[] buffer = new byte[0x50];

            Encoding.ASCII.GetBytes(codeplug.InternalCodeplug.Model).CopyTo(buffer, MODEL);

            //Calculate Factory Code Bytes 7 and 8
            //These bytes are calculated by the official process and already stored in flash.
            //Codeplug has the same series of bytes, except for bytes 7 and 8 are 0x00;
            //Seems to just really be a checksum
            var fc = codeplug.FactoryCode;
            var check = (fc[0x0] * 0x100 + fc[0x1]) +
                        (fc[0x2] * 0x100 + fc[0x3]) +
                        (fc[0x4] * 0x100 + fc[0x5]) +
                        (fc[0x8] * 0x100 + fc[0x9]) +
                        (fc[0xA] * 0x100 + fc[0xB]) +
                        (fc[0xC] * 0x100 + fc[0xD]) +
                        (fc[0xE] * 0x100 + fc[0xF]);
            check = ~((check & 0xFFFF) - 0xFFF8) + 1;
            fc[0x6] = (byte)(check / 0x100);
            fc[0x7] = (byte)(check % 0x100);

            fc.CopyTo(buffer, FACTORY_CODE);

            //0x03000, 0x0B000, 0x13000, 0x1B000, 0x23000, 0x2B000, 0x33000, 0x3B000
           
            byte[] flashSignature = { 0xFF, 0xCC, 0x05, 0xCC, 0x01, 0xFF, 0xFF, 0x41 }; //6.73
            //byte[] flashSignature = { 0xFF, 0xCC, 0x05, 0xCE, 0x51, 0xFF, 0xFF, 0x41 }; //8.73?
            //byte[] flashSignature = { 0xFF, 0xBD, 0x43, 0xCE, 0x41, 0xFF, 0xFF, 0x06 };
            flashSignature.CopyTo(buffer, FLASH_SIGNATURE);

            codeplug.InternalCodeplug.Block10.FeatureBlock.CopyTo(buffer, FDB_PART_A);
            codeplug.InternalCodeplug.Block10.Flashcode.CopyTo(buffer, FDB_PART_B);

            byte[] authCode = new byte[10];

            var i = 0;
            var j = 0;
            byte z = 0;
            foreach (byte o in ORDER)
            {
                byte a = buffer[o];

                a ^= z; //eora byte_70
                z = a; //staa byte_70

                a = (byte)((sbyte)(a) >> 1); //asra    ;Arithmetic Shift Right
                a = (byte)(a >> 1); //lsra    ;Logical Shift Right 
                a ^= z; //eora byte_70

                byte b = a; //tab

                a = (byte)(a << 1); //asla (or lsla)  ;Logical Shift Left

                a &= 0xF0; //anda #$F0

                b = (byte)((sbyte)(b) >> 1); //asrb    ;Arithmetic Shift Right

                if ((b & 0x80) == 0x80)  //bmi  ;Is MSB (aka Negative Bit) Set?
                {
                    b = (byte)(~b); //comb
                }

                b &= 0x0f; //andb #$F

                a = (byte)(a + b); //aba

                b = z; //ldab byte_70
                b &= 0x7; //andb #7

                a ^= KEY[b]; //ldx #$D8B9  -- abx  -- eora 0,x

                z = a; //staa byte_70

                if (++i % 8 == 0)
                {
                    authCode[j++] = z;
                    z = 0;
                }
            }

            byte[] serial = Encoding.ASCII.GetBytes(codeplug.InternalCodeplug.Serial);
            for (int x = 0; x < 10; x++)
            {
                authCode[x] = (byte)(authCode[x] ^ serial[x]);
            }
            return authCode;
        }
    }
}
