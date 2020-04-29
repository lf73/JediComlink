using System;
using System.ComponentModel;
using System.Dynamic;

namespace JediCodeplug
{
    public class Block10 : Block
    {
        public override byte Id { get => 0x10; }
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
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] FeatureBlock { get; set; } = new byte[0x19];

        [DisplayName("Flashcode Length")]
        [Description("Seems to always be 6")]
        public byte FlashcodeLength { get; } = 0x06;

        [DisplayName("Flashcode")]
        [TypeConverter(typeof(HexByteArrayTypeConverter))]
        public byte[] Flashcode { get; set; } = new byte[0x06];





        #region FDB BYTE 0x00
        [DisplayName("Radio Type Part 1")]
        [Description("1/2  Invalid=T/T Conv Only=F/T  Trunk & Conv = F/F  ???=T/F (Disables Menu Option Selection for Conv)")]
        public bool RadioType1 {
            get => (FeatureBlock[0x00] & (1 << 7)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 7)) : (byte)(FeatureBlock[0x00] & ~(1 << 7));
        }

        [DisplayName("Radio Type Part 2")]
        [Description("1/2  Invalid=T/T Conv Only=F/T  Trunk & Conv = F/F  ???=T/F (Disables Menu Option Selection for Conv)")]
        public bool RadioType2
        {
            get => (FeatureBlock[0x00] & (1 << 6)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 6)) : (byte)(FeatureBlock[0x00] & ~(1 << 6));
        }

        [DisplayName("Radio Lock")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Display & Menu --> Radio Lock")]
        public bool RadioLock
        {
            get => (FeatureBlock[0x00] & (1 << 5)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 5)) : (byte)(FeatureBlock[0x00] & ~(1 << 5));
        }

        [DisplayName("Selectable Keypad Mute")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Radio Wide --> General --> Selectable Keypad Mute")]
        public bool SelectableKeypadMute
        {
            get => (FeatureBlock[0x00] & (1 << 4)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 4)) : (byte)(FeatureBlock[0x00] & ~(1 << 4));
        }

        [DisplayName("Battery Saver")]
        [Description("No references in CPS.")]
        public bool BatterySaver
        {
            get => (FeatureBlock[0x00] & (1 << 3)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 3)) : (byte)(FeatureBlock[0x00] & ~(1 << 3));
        }

        [DisplayName("Out of Range Indicator")]
        [Description("Safe to Enable. Applies to Trunking Only. Toogles ability to set in CPS, Radio Wide --> General --> Out of Range Indicator")]
        public bool OutOfRangeIndicator
        {
            get => (FeatureBlock[0x00] & (1 << 2)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 2)) : (byte)(FeatureBlock[0x00] & ~(1 << 2));
        }

        [DisplayName("Talk-around")]
        [Description("Safe to Enable. Applies to Trunking Only. Toogles ability to set in CPS, Conventional Personality --> Direct/Talkaround.  Does not seem to change the ability to add to button or switch, its option is always present there.")]
        public bool TalkAround
        {
            get => (FeatureBlock[0x00] & (1 << 1)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 1)) : (byte)(FeatureBlock[0x00] & ~(1 << 1));
        }

        [DisplayName("Transmit Inhibit Switch")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Controls --> Switches --> Concentric --> Tx Inhbit")]
        public bool TransmitInhibitSwitch
        {
            get => (FeatureBlock[0x00] & (1 << 0)) > 0;
            set => FeatureBlock[0x00] = value ? (byte)(FeatureBlock[0x00] | (1 << 0)) : (byte)(FeatureBlock[0x00] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x01
        [DisplayName("Adaptive Splatter")]
        [Description("Unknown Feature. No references in CPS.")]
        public bool AdaptiveSplatter
        {
            get => (FeatureBlock[0x01] & (1 << 7)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 7)) : (byte)(FeatureBlock[0x01] & ~(1 << 7));
        }

        [DisplayName("Audio Comparator")]
        [Description("Unknown Feature. No references in CPS.")]
        public bool AudioComparator
        {
            get => (FeatureBlock[0x01] & (1 << 6)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 6)) : (byte)(FeatureBlock[0x01] & ~(1 << 6));
        }

        [DisplayName("Variable Power")]
        [Description("Unknown Feature. No references in CPS. Could be related to trunking and sets power based on control channel strength?")]
        public bool VariablePower
        {
            get => (FeatureBlock[0x01] & (1 << 5)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 5)) : (byte)(FeatureBlock[0x01] & ~(1 << 5));
        }

        [DisplayName("Uppercase-only Display")]
        [Description("Only seen set to True. Unknown Feature. Seems to have no affect in CPS. Zone text still converted to uppercase.")]
        public bool UpperCaseOnlyDisplay
        {
            get => (FeatureBlock[0x01] & (1 << 4)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 4)) : (byte)(FeatureBlock[0x01] & ~(1 << 4));
        }

        [DisplayName("Radio-to-radio Cloning")]
        [Description("Likely Safe to Enable. Probably a waste of time without proper cable. Toogles ability to set in CPS, Radio Wide --> General --> Radio Cloning.")]
        public bool RadioToRadioCloning
        {
            get => (FeatureBlock[0x01] & (1 << 3)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 3)) : (byte)(FeatureBlock[0x01] & ~(1 << 3));
        }

        [DisplayName("Whisper Mode")]
        [Description("Unknown Feature. No references in CPS. To do: determine if any change in mic / speaker")]
        public bool WhisperMode
        {
            get => (FeatureBlock[0x01] & (1 << 2)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 2)) : (byte)(FeatureBlock[0x01] & ~(1 << 2));
        }

        [DisplayName("Fdb in External")]
        [Description("Leave Alone! 800mhz, False. VHF, True. At least with VHF, allows internal codeplug to span past 0x0200 to 0x0280, since it has extra test frequencies.")]
        public bool FdbInExternal
        {
            get => (FeatureBlock[0x01] & (1 << 1)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 1)) : (byte)(FeatureBlock[0x01] & ~(1 << 1));
        }

        [DisplayName("Continuous Rotary")]
        [Description("Set based on hardware. Likely allows channel selection beyond 16. Toogles ability to set in CPS, Radio Wide --> Alert Tones --> Rotary Alert")]
        public bool ContinuousRotary
        {
            get => (FeatureBlock[0x01] & (1 << 0)) > 0;
            set => FeatureBlock[0x01] = value ? (byte)(FeatureBlock[0x01] | (1 << 0)) : (byte)(FeatureBlock[0x01] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x02
        [DisplayName("Vox")]
        [Description("Unknown Feature. No references in CPS.")]
        public bool Vox
        {
            get => (FeatureBlock[0x02] & (1 << 7)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 7)) : (byte)(FeatureBlock[0x02] & ~(1 << 7));
        }

        [DisplayName("Keypad Lock")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Controls --> Switches --> Either Concentric or Three Position --> Keypad Lock")]
        public bool KeypadLock
        {
            get => (FeatureBlock[0x02] & (1 << 6)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 6)) : (byte)(FeatureBlock[0x02] & ~(1 << 6));
        }

        [DisplayName("Closed featureset Only")]
        [Description("Unknown Feature. No references in CPS. Maybe used in flash feature upgrade process?")]
        public bool ClosedFeaturesetOnly
        {
            get => (FeatureBlock[0x02] & (1 << 5)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 5)) : (byte)(FeatureBlock[0x02] & ~(1 << 5));
        }

        [DisplayName("Emergency RX")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Display & Menu --> Advanced --> Emergency Receive")]
        public bool EmergencyRx
        {
            get => (FeatureBlock[0x02] & (1 << 4)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 4)) : (byte)(FeatureBlock[0x02] & ~(1 << 4));
        }

        [DisplayName("VOC (Voice-on-control)")]
        [Description("Safe to Enable. Toogles ability to set in CPS, Trunking Configuration --> VOC.  (Voice on Control Channel allows)")]
        public bool VVoiceOnControl
        {
            get => (FeatureBlock[0x02] & (1 << 3)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 3)) : (byte)(FeatureBlock[0x02] & ~(1 << 3));
        }

        [DisplayName("821-824 MHz Disable")]
        [Description("Safe to Enable/Disable. Toogles ability to set transmit frequncy in NPSPAC range. The NPSPAC receive and TA (Direct) transmit frequncies in 866 to 869 are unaffected.")]
        public bool Freq821To824Disable
        {
            get => (FeatureBlock[0x02] & (1 << 2)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 2)) : (byte)(FeatureBlock[0x02] & ~(1 << 2));
        }

        [DisplayName("ASTRO-ready")]
        [Description("Unknown Feature. No references in CPS. Likely XTS3000 feature?")]
        public bool ASTROready
        {
            get => (FeatureBlock[0x02] & (1 << 1)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 1)) : (byte)(FeatureBlock[0x02] & ~(1 << 1));
        }

        [DisplayName("Hot Mic Emergency")]
        [Description("Safe to Enable. Toogles ability to see tab page in CPS, Trunking Personality --> Hot Mic.")]
        public bool HotMicEmergency
        {
            get => (FeatureBlock[0x02] & (1 << 0)) > 0;
            set => FeatureBlock[0x02] = value ? (byte)(FeatureBlock[0x02] | (1 << 0)) : (byte)(FeatureBlock[0x02] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x03
        [DisplayName("Privacy Plus Trunking")]
        [Description("Safe to Enable. Should allow older Type I System use. No change detected in CPS.")]
        public bool PrivacyPlusTrunking
        {
            get => (FeatureBlock[0x03] & (1 << 7)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 7)) : (byte)(FeatureBlock[0x03] & ~(1 << 7));
        }

        [DisplayName("SmartNet Trunking")]
        [Description("Safe to Enable. Should allow APCO16 features on Type II Systems. No change detected in CPS.")]
        public bool SmartNetTrunking
        {
            get => (FeatureBlock[0x03] & (1 << 6)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 6)) : (byte)(FeatureBlock[0x03] & ~(1 << 6));
        }

        [DisplayName("Failsoft per-mode")]
        [Description("Safe to Enable. Toogles ability to see tab page in CPS, Trunking Personality --> Failsoft.")]
        public bool FailsoftPeMode
        {
            get => (FeatureBlock[0x03] & (1 << 5)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 5)) : (byte)(FeatureBlock[0x03] & ~(1 << 5));
        }

        [DisplayName("Auto Affiliation")]
        [Description("Probably good to set to false for trunk recieve only. Although, no effect in CPS as you can still select affiliation option on Trunking System --> Type II/IIi --> Affilialtion Type")]
        public bool AutoAffiliation
        {
            get => (FeatureBlock[0x03] & (1 << 4)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 4)) : (byte)(FeatureBlock[0x03] & ~(1 << 4));
        }

        [DisplayName("AMSS")]
        [Description("Safe to Enable. Toogles ability to see tab page in CPS, Trunking System --> General --> Coverage Type and Trunking Personality --> WAC_AMSS.  (Wide Area Coverage Automatic Multiple Site Select)")]
        public bool AMSS
        {
            get => (FeatureBlock[0x03] & (1 << 3)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 3)) : (byte)(FeatureBlock[0x03] & ~(1 << 3));
        }

        [DisplayName("Trunked Status")]
        [Description("Safe to Enable. Toogles ability to set Radio Confiuration --> Display & Menu --> Trunk --> Status. Also toggles Alias Check Box in Trunking System -- > Aliasing ")]
        public bool TrunkedStatus
        {
            get => (FeatureBlock[0x03] & (1 << 2)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 2)) : (byte)(FeatureBlock[0x03] & ~(1 << 2));
        }

        [DisplayName("Trunked Message")]
        [Description("Safe to Enable. Toogles ability to set Radio Confiuration --> Display & Menu --> Trunk --> Message. Also toggles Alias Check Box in Trunking System -- > Aliasing ")]
        public bool TrunkedMessage
        {
            get => (FeatureBlock[0x03] & (1 << 1)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 1)) : (byte)(FeatureBlock[0x03] & ~(1 << 1));
        }

        [DisplayName("OBT (Other Band Trunking)")]
        [Description("Unknown Feature. No references in CPS.")]
        public bool OBT
        {
            get => (FeatureBlock[0x03] & (1 << 0)) > 0;
            set => FeatureBlock[0x03] = value ? (byte)(FeatureBlock[0x03] | (1 << 0)) : (byte)(FeatureBlock[0x03] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x04
        [DisplayName("Dynamic Regrouping")]
        [Description("Safe to Enable. Toogles ability to set CPS, Trunking System --> Dynamic Regrouping --> Enable Dynamic Regrouping")]
        public bool DynamicRegrouping
        {
            get => (FeatureBlock[0x04] & (1 << 7)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 7)) : (byte)(FeatureBlock[0x04] & ~(1 << 7));
        }

        [DisplayName("Reprogram Request")]
        [Description("Safe to Enable, seems to be default. No detectable change in CPS")]
        public bool ReprogramRequest
        {
            get => (FeatureBlock[0x04] & (1 << 6)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 6)) : (byte)(FeatureBlock[0x04] & ~(1 << 6));
        }

        [DisplayName("Trunking Emergency")]
        public bool TrunkingEmergency
        {
            get => (FeatureBlock[0x04] & (1 << 5)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 5)) : (byte)(FeatureBlock[0x04] & ~(1 << 5));
        }

        [DisplayName("Trunking Emergency Type")]
        public bool TrunkingEmergencyType1
        {
            get => (FeatureBlock[0x04] & (1 << 4)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 4)) : (byte)(FeatureBlock[0x04] & ~(1 << 4));
        }

        [DisplayName("Trunking Emergency Type (related to above)")]
        public bool TrunkingEmergencyType2
        {
            get => (FeatureBlock[0x04] & (1 << 3)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 3)) : (byte)(FeatureBlock[0x04] & ~(1 << 3));
        }

        [DisplayName("Trunk Unusable 1")]
        public bool TrunkUnusable1
        {
            get => (FeatureBlock[0x04] & (1 << 2)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 2)) : (byte)(FeatureBlock[0x04] & ~(1 << 2));
        }

        [DisplayName("Phone Select")]
        public bool PhoneSelect1
        {
            get => (FeatureBlock[0x04] & (1 << 1)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 1)) : (byte)(FeatureBlock[0x04] & ~(1 << 1));
        }

        [DisplayName("Phone Select (related to above)")]
        public bool PhoneSelect2
        {
            get => (FeatureBlock[0x04] & (1 << 0)) > 0;
            set => FeatureBlock[0x04] = value ? (byte)(FeatureBlock[0x04] | (1 << 0)) : (byte)(FeatureBlock[0x04] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x05
        [DisplayName("Trunking Private Call")]
        public bool TrunkingPrivateCall
        {
            get => (FeatureBlock[0x05] & (1 << 7)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 7)) : (byte)(FeatureBlock[0x05] & ~(1 << 7));
        }

        [DisplayName("Trunking Private Call Select")]
        public bool TrunkingPrivateCallSelect1
        {
            get => (FeatureBlock[0x05] & (1 << 6)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 6)) : (byte)(FeatureBlock[0x05] & ~(1 << 6));
        }

        [DisplayName("Trunking Private Call Select (related to above)")]
        public bool TrunkingPrivateCallSelect2
        {
            get => (FeatureBlock[0x05] & (1 << 5)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 5)) : (byte)(FeatureBlock[0x05] & ~(1 << 5));
        }

        [DisplayName("Trunking Call Alert")]
        public bool TrunkingCallAlert
        {
            get => (FeatureBlock[0x05] & (1 << 4)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 4)) : (byte)(FeatureBlock[0x05] & ~(1 << 4));
        }

        [DisplayName("Trunking Call Alert Select")]
        public bool TrunkingCallAlertSelect1
        {
            get => (FeatureBlock[0x05] & (1 << 3)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 3)) : (byte)(FeatureBlock[0x05] & ~(1 << 3));
        }

        [DisplayName("Trunking Call Alert Select (related to above)")]
        public bool TrunkingCallAlertSelect2
        {
            get => (FeatureBlock[0x05] & (1 << 2)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 2)) : (byte)(FeatureBlock[0x05] & ~(1 << 2));
        }

        [DisplayName("Trunking PC Call ID Aliasing")]
        public bool TrunkingPCCallIdAliasing
        {
            get => (FeatureBlock[0x05] & (1 << 1)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 1)) : (byte)(FeatureBlock[0x05] & ~(1 << 1));
        }

        [DisplayName("Trunking Radio Type")]
        public bool TrunkingRadioType1
        {
            get => (FeatureBlock[0x05] & (1 << 0)) > 0;
            set => FeatureBlock[0x05] = value ? (byte)(FeatureBlock[0x05] | (1 << 0)) : (byte)(FeatureBlock[0x05] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x06
        [DisplayName("Trunking Radio Type (related to above")]
        public bool TrunkingRadioType2
        {
            get => (FeatureBlock[0x06] & (1 << 7)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 7)) : (byte)(FeatureBlock[0x06] & ~(1 << 7));
        }

        [DisplayName("Channel Change ID")]
        public bool ChannelChangeId
        {
            get => (FeatureBlock[0x06] & (1 << 6)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 6)) : (byte)(FeatureBlock[0x06] & ~(1 << 6));
        }

        [DisplayName("Expandable Tuning Blocks")]
        public bool ExpandableTuningBlocks
        {
            get => (FeatureBlock[0x06] & (1 << 5)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 5)) : (byte)(FeatureBlock[0x06] & ~(1 << 5));
        }

        [DisplayName("Dispatcher Interrupt")]
        public bool DispatcherInterrupt
        {
            get => (FeatureBlock[0x06] & (1 << 4)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 4)) : (byte)(FeatureBlock[0x06] & ~(1 << 4));
        }

        [DisplayName("One Touch Operation")]
        public bool OneTouchOperation
        {
            get => (FeatureBlock[0x06] & (1 << 3)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 3)) : (byte)(FeatureBlock[0x06] & ~(1 << 3));
        }

        [DisplayName("SmartZone")]
        public bool SmartZone
        {
            get => (FeatureBlock[0x06] & (1 << 2)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 2)) : (byte)(FeatureBlock[0x06] & ~(1 << 2));
        }

        [DisplayName("CWE Features")]
        public bool CWEFeatures
        {
            get => (FeatureBlock[0x06] & (1 << 1)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 1)) : (byte)(FeatureBlock[0x06] & ~(1 << 1));
        }

        [DisplayName("Trunk I Features")]
        public bool TrunkIFeatures
        {
            get => (FeatureBlock[0x06] & (1 << 0)) > 0;
            set => FeatureBlock[0x06] = value ? (byte)(FeatureBlock[0x06] | (1 << 0)) : (byte)(FeatureBlock[0x06] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x07
        [DisplayName("Mode Slaved Scan")]
        public bool ModeSlavedScan
        {
            get => (FeatureBlock[0x07] & (1 << 7)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 7)) : (byte)(FeatureBlock[0x07] & ~(1 << 7));
        }

        [DisplayName("Operator Selectable Scan")]
        public bool OperatorSelectableScan
        {
            get => (FeatureBlock[0x07] & (1 << 6)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 6)) : (byte)(FeatureBlock[0x07] & ~(1 << 6));
        }

        [DisplayName("Trunking Priority Monitor Scan")]
        public bool TrunkingPriorityMonitorScan
        {
            get => (FeatureBlock[0x07] & (1 << 5)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 5)) : (byte)(FeatureBlock[0x07] & ~(1 << 5));
        }

        [DisplayName("Talkgroup Scan")]
        public bool TalkgroupScan
        {
            get => (FeatureBlock[0x07] & (1 << 4)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 4)) : (byte)(FeatureBlock[0x07] & ~(1 << 4));
        }

        [DisplayName("Conventional Priority Scan")]
        public bool ConventionalPriorityScan
        {
            get => (FeatureBlock[0x07] & (1 << 3)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 3)) : (byte)(FeatureBlock[0x07] & ~(1 << 3));
        }

        [DisplayName("Home Mode Revert Scan")]
        public bool HomeModeRevertScan
        {
            get => (FeatureBlock[0x07] & (1 << 2)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 2)) : (byte)(FeatureBlock[0x07] & ~(1 << 2));
        }

        [DisplayName("Off-hook Suspend Scan")]
        public bool OffHookSuspendScan
        {
            get => (FeatureBlock[0x07] & (1 << 1)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 1)) : (byte)(FeatureBlock[0x07] & ~(1 << 1));
        }

        [DisplayName("Smartzone Full-spectrum control channel scan")]
        public bool SmartzoneFullSpectrumControlChannelScan
        {
            get => (FeatureBlock[0x07] & (1 << 0)) > 0;
            set => FeatureBlock[0x07] = value ? (byte)(FeatureBlock[0x07] | (1 << 0)) : (byte)(FeatureBlock[0x07] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x08
        [DisplayName("MDC")]
        public bool MDC
        {
            get => (FeatureBlock[0x08] & (1 << 7)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 7)) : (byte)(FeatureBlock[0x08] & ~(1 << 7));
        }

        [DisplayName("MDC Status")]
        public bool MDCStatus
        {
            get => (FeatureBlock[0x08] & (1 << 6)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 6)) : (byte)(FeatureBlock[0x08] & ~(1 << 6));
        }

        [DisplayName("MDC Message")]
        public bool MDCMessage
        {
            get => (FeatureBlock[0x08] & (1 << 5)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 5)) : (byte)(FeatureBlock[0x08] & ~(1 << 5));
        }

        [DisplayName("MDC RTT (Request-to-talk)")]
        public bool MDCRTT
        {
            get => (FeatureBlock[0x08] & (1 << 4)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 4)) : (byte)(FeatureBlock[0x08] & ~(1 << 4));
        }

        [DisplayName("MDC Radio Inhibit")]
        public bool MDCRadioInhibit
        {
            get => (FeatureBlock[0x08] & (1 << 3)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 3)) : (byte)(FeatureBlock[0x08] & ~(1 << 3));
        }

        [DisplayName("MDC Enhanced Selective Call (SelCall)")]
        public bool MDCEnhancedSelectiveCall
        {
            get => (FeatureBlock[0x08] & (1 << 2)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 2)) : (byte)(FeatureBlock[0x08] & ~(1 << 2));
        }

        [DisplayName("MDC Selective Call Type")]
        public bool MDCSelectiveCallType1
        {
            get => (FeatureBlock[0x08] & (1 << 1)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 1)) : (byte)(FeatureBlock[0x08] & ~(1 << 1));
        }

        [DisplayName("MDC Selective Call Type (related to above)")]
        public bool MDCSelectiveCallType2
        {
            get => (FeatureBlock[0x08] & (1 << 0)) > 0;
            set => FeatureBlock[0x08] = value ? (byte)(FeatureBlock[0x08] | (1 << 0)) : (byte)(FeatureBlock[0x08] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x09
        [DisplayName("MDC Multiple Systems")]
        public bool MDCMultipleSystems
        {
            get => (FeatureBlock[0x09] & (1 << 7)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 7)) : (byte)(FeatureBlock[0x09] & ~(1 << 7));
        }

        [DisplayName("MDC Selective Call Encode Select")]
        public bool MDCSelectiveCallEncodeSelect1
        {
            get => (FeatureBlock[0x09] & (1 << 6)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 6)) : (byte)(FeatureBlock[0x09] & ~(1 << 6));
        }

        [DisplayName("MDC Selective Call Encode Select (related to above)")]
        public bool MDCSelectiveCallEncodeSelect2
        {
            get => (FeatureBlock[0x09] & (1 << 5)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 5)) : (byte)(FeatureBlock[0x09] & ~(1 << 5));
        }

        [DisplayName("MDC Call Alert Type")]
        public bool MDCCallAlertType1
        {
            get => (FeatureBlock[0x09] & (1 << 4)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 4)) : (byte)(FeatureBlock[0x09] & ~(1 << 4));
        }

        [DisplayName("MDC Call Alert Type (related to above)")]
        public bool MDCCallAlertType2
        {
            get => (FeatureBlock[0x09] & (1 << 3)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 3)) : (byte)(FeatureBlock[0x09] & ~(1 << 3));
        }

        [DisplayName("MDC Future Feature 2")]
        public bool MDCFutureFeature2
        {
            get => (FeatureBlock[0x09] & (1 << 2)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 2)) : (byte)(FeatureBlock[0x09] & ~(1 << 2));
        }

        [DisplayName("MDC Call Alert Encode Select")]
        public bool MDCCallAlertEncodeSelect1
        {
            get => (FeatureBlock[0x09] & (1 << 1)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 1)) : (byte)(FeatureBlock[0x09] & ~(1 << 1));
        }

        [DisplayName("MDC Call Alert Encode Select (related to above)")]
        public bool MDCCallAlertEncodeSelect2
        {
            get => (FeatureBlock[0x09] & (1 << 0)) > 0;
            set => FeatureBlock[0x09] = value ? (byte)(FeatureBlock[0x09] | (1 << 0)) : (byte)(FeatureBlock[0x09] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0A
        [DisplayName("MDC Emergency")]
        public bool MDCEmergency
        {
            get => (FeatureBlock[0x0A] & (1 << 7)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 7)) : (byte)(FeatureBlock[0x0A] & ~(1 << 7));
        }

        [DisplayName("MDC Emergency Type")]
        public bool MDCEmergencyType1
        {
            get => (FeatureBlock[0x0A] & (1 << 6)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 6)) : (byte)(FeatureBlock[0x0A] & ~(1 << 6));
        }

        [DisplayName("MDC Emergency Type (related to above)")]
        public bool MDCEmergencyType2
        {
            get => (FeatureBlock[0x0A] & (1 << 5)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 5)) : (byte)(FeatureBlock[0x0A] & ~(1 << 5));
        }

        [DisplayName("MDC RAC")]
        public bool MDCRAC
        {
            get => (FeatureBlock[0x0A] & (1 << 4)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 4)) : (byte)(FeatureBlock[0x0A] & ~(1 << 4));
        }

        [DisplayName("MDC Future Feature 5")]
        public bool MDCFutureFeature5
        {
            get => (FeatureBlock[0x0A] & (1 << 3)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 3)) : (byte)(FeatureBlock[0x0A] & ~(1 << 3));
        }

        [DisplayName("RAC Select")]
        public bool RACSelect1
        {
            get => (FeatureBlock[0x0A] & (1 << 2)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 2)) : (byte)(FeatureBlock[0x0A] & ~(1 << 2));
        }

        [DisplayName("RAC Select (related to above)")]
        public bool RACSelect2
        {
            get => (FeatureBlock[0x0A] & (1 << 1)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 1)) : (byte)(FeatureBlock[0x0A] & ~(1 << 1));
        }

        [DisplayName("ATIS")]
        public bool ATIS
        {
            get => (FeatureBlock[0x0A] & (1 << 0)) > 0;
            set => FeatureBlock[0x0A] = value ? (byte)(FeatureBlock[0x0A] | (1 << 0)) : (byte)(FeatureBlock[0x0A] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0B
        [DisplayName("Unknown")]
        public bool _0BBit7
        {
            get => (FeatureBlock[0x0B] & (1 << 7)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 7)) : (byte)(FeatureBlock[0x0B] & ~(1 << 7));
        }

        [DisplayName("Unknown")]
        public bool _0BBit6
        {
            get => (FeatureBlock[0x0B] & (1 << 6)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 6)) : (byte)(FeatureBlock[0x0B] & ~(1 << 6));
        }

        [DisplayName("Quik - Call II System")]
        public bool QuikCallIISystem
        {
            get => (FeatureBlock[0x0B] & (1 << 5)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 5)) : (byte)(FeatureBlock[0x0B] & ~(1 << 5));
        }

        [DisplayName("Byte 0x0E Bit 4 Future Feature")]
        public bool _0BBit4
        {
            get => (FeatureBlock[0x0B] & (1 << 4)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 4)) : (byte)(FeatureBlock[0x0B] & ~(1 << 4));
        }

        [DisplayName("Byte 0x0E Bit 3 Future Feature")]
        public bool _0BBit3
        {
            get => (FeatureBlock[0x0B] & (1 << 3)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 3)) : (byte)(FeatureBlock[0x0B] & ~(1 << 3));
        }

        [DisplayName("Byte 0x0E Bit 2 Future Feature")]
        public bool _0BBit2
        {
            get => (FeatureBlock[0x0B] & (1 << 2)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 2)) : (byte)(FeatureBlock[0x0B] & ~(1 << 2));
        }

        [DisplayName("MDC Future Feature 3")]
        public bool MDCFutureFeature3
        {
            get => (FeatureBlock[0x0B] & (1 << 1)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 1)) : (byte)(FeatureBlock[0x0B] & ~(1 << 1));
        }

        [DisplayName("MDC Future Feature 4")]
        public bool MDCFutureFeature4
        {
            get => (FeatureBlock[0x0B] & (1 << 0)) > 0;
            set => FeatureBlock[0x0B] = value ? (byte)(FeatureBlock[0x0B] | (1 << 0)) : (byte)(FeatureBlock[0x0B] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0C
        [DisplayName("Unknown")]
        public bool _0CBit7
        {
            get => (FeatureBlock[0x0C] & (1 << 7)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 7)) : (byte)(FeatureBlock[0x0C] & ~(1 << 7));
        }

        [DisplayName("Unknown")]
        public bool _0CBit6
        {
            get => (FeatureBlock[0x0C] & (1 << 6)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 6)) : (byte)(FeatureBlock[0x0C] & ~(1 << 6));
        }

        [DisplayName("Unknown")]
        public bool _0CBit5
        {
            get => (FeatureBlock[0x0C] & (1 << 5)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 5)) : (byte)(FeatureBlock[0x0C] & ~(1 << 5));
        }

        [DisplayName("Unknown")]
        public bool _0CBit4
        {
            get => (FeatureBlock[0x0C] & (1 << 4)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 4)) : (byte)(FeatureBlock[0x0C] & ~(1 << 4));
        }

        [DisplayName("Unknown")]
        public bool _0CBit3
        {
            get => (FeatureBlock[0x0C] & (1 << 3)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 3)) : (byte)(FeatureBlock[0x0C] & ~(1 << 3));
        }

        [DisplayName("Unknown")]
        public bool _0CBit2
        {
            get => (FeatureBlock[0x0C] & (1 << 2)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 2)) : (byte)(FeatureBlock[0x0C] & ~(1 << 2));
        }

        [DisplayName("Unknown")]
        public bool _0CBit1
        {
            get => (FeatureBlock[0x0C] & (1 << 1)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 1)) : (byte)(FeatureBlock[0x0C] & ~(1 << 1));
        }

        [DisplayName("Unknown")]
        public bool _0CBit0
        {
            get => (FeatureBlock[0x0C] & (1 << 0)) > 0;
            set => FeatureBlock[0x0C] = value ? (byte)(FeatureBlock[0x0C] | (1 << 0)) : (byte)(FeatureBlock[0x0C] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0D
        [DisplayName("Telephone Interconnect Type")]
        public bool TelephoneInterconnectType1
        {
            get => (FeatureBlock[0x0D] & (1 << 7)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 7)) : (byte)(FeatureBlock[0x0D] & ~(1 << 7));
        }

        [DisplayName("Telephone Interconnect Type (related to above)")]
        public bool TelephoneInterconnectType2
        {
            get => (FeatureBlock[0x0D] & (1 << 6)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 6)) : (byte)(FeatureBlock[0x0D] & ~(1 << 6));
        }

        [DisplayName("DTMF Selective Call Type")]
        public bool DTMFSelectiveCallType1
        {
            get => (FeatureBlock[0x0D] & (1 << 5)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 5)) : (byte)(FeatureBlock[0x0D] & ~(1 << 5));
        }

        [DisplayName("DTMF Selective Cal Type (related to above)")]
        public bool DTMFSelectiveCallType2
        {
            get => (FeatureBlock[0x0D] & (1 << 4)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 4)) : (byte)(FeatureBlock[0x0D] & ~(1 << 4));
        }

        [DisplayName("DTMF Selective Call Encode Select")]
        public bool DTMFSelectiveCallEncodeSelect1
        {
            get => (FeatureBlock[0x0D] & (1 << 3)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 3)) : (byte)(FeatureBlock[0x0D] & ~(1 << 3));
        }

        [DisplayName("DTMF Selective Call Encode Select (related to above)")]
        public bool DTMFSelectiveCallEncodeSelect2
        {
            get => (FeatureBlock[0x0D] & (1 << 2)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 2)) : (byte)(FeatureBlock[0x0D] & ~(1 << 2));
        }

        [DisplayName("DTMF RAC")]
        public bool DTMFRAC
        {
            get => (FeatureBlock[0x0D] & (1 << 1)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 1)) : (byte)(FeatureBlock[0x0D] & ~(1 << 1));
        }

        [DisplayName("DTMF Future Feature 2")]
        public bool DTMFFutureFeature2
        {
            get => (FeatureBlock[0x0D] & (1 << 0)) > 0;
            set => FeatureBlock[0x0D] = value ? (byte)(FeatureBlock[0x0D] | (1 << 0)) : (byte)(FeatureBlock[0x0D] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0E
        [DisplayName("DTMF Call Alert Encode")]
        public bool DTMFCallAlertEncode
        {
            get => (FeatureBlock[0x0E] & (1 << 7)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 7)) : (byte)(FeatureBlock[0x0E] & ~(1 << 7));
        }

        [DisplayName("DTMF Call Alert Encode Select")]
        public bool DTMFCallAlertEncodeSelect1
        {
            get => (FeatureBlock[0x0E] & (1 << 6)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 6)) : (byte)(FeatureBlock[0x0E] & ~(1 << 6));
        }

        [DisplayName("DTMF Call Alert Encode Select (related to above)")]
        public bool DTMFCallAlertEncodeSelect2
        {
            get => (FeatureBlock[0x0E] & (1 << 5)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 5)) : (byte)(FeatureBlock[0x0E] & ~(1 << 5));
        }

        [DisplayName("DTMF Future Feature 3")]
        public bool DTMFFutureFeature3
        {
            get => (FeatureBlock[0x0E] & (1 << 4)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 4)) : (byte)(FeatureBlock[0x0E] & ~(1 << 4));
        }

        [DisplayName("DTMF Future Feature 4")]
        public bool DTMFFutureFeature4
        {
            get => (FeatureBlock[0x0E] & (1 << 3)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 3)) : (byte)(FeatureBlock[0x0E] & ~(1 << 3));
        }

        [DisplayName("DTMF Future Feature 5")]
        public bool DTMFFutureFeature5
        {
            get => (FeatureBlock[0x0E] & (1 << 2)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 2)) : (byte)(FeatureBlock[0x0E] & ~(1 << 2));
        }

        [DisplayName("DTMF Future Feature 6")]
        public bool DTMFFutureFeature6
        {
            get => (FeatureBlock[0x0E] & (1 << 1)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 1)) : (byte)(FeatureBlock[0x0E] & ~(1 << 1));
        }

        [DisplayName("DTMF Future Feature 7")]
        public bool DTMFFutureFeature7
        {
            get => (FeatureBlock[0x0E] & (1 << 0)) > 0;
            set => FeatureBlock[0x0E] = value ? (byte)(FeatureBlock[0x0E] | (1 << 0)) : (byte)(FeatureBlock[0x0E] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x0F
        [DisplayName("Operator Selectable PL Encode")]
        public bool OperatorSelectablePLEncode
        {
            get => (FeatureBlock[0x0F] & (1 << 7)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 7)) : (byte)(FeatureBlock[0x0F] & ~(1 << 7));
        }

        [DisplayName("PAC RT")]
        public bool PACRT
        {
            get => (FeatureBlock[0x0F] & (1 << 6)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 6)) : (byte)(FeatureBlock[0x0F] & ~(1 << 6));
        }

        [DisplayName("Single Tone")]
        public bool SingleTone
        {
            get => (FeatureBlock[0x0F] & (1 << 5)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 5)) : (byte)(FeatureBlock[0x0F] & ~(1 << 5));
        }

        [DisplayName("VRM")]
        public bool VRM
        {
            get => (FeatureBlock[0x0F] & (1 << 4)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 4)) : (byte)(FeatureBlock[0x0F] & ~(1 << 4));
        }

        [DisplayName("Clone and Destroy")]
        public bool CloneAndDestroy
        {
            get => (FeatureBlock[0x0F] & (1 << 3)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 3)) : (byte)(FeatureBlock[0x0F] & ~(1 << 3));
        }

        [DisplayName("Conventional Voting Scan")]
        public bool _0FBit2
        {
            get => (FeatureBlock[0x0F] & (1 << 2)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 2)) : (byte)(FeatureBlock[0x0F] & ~(1 << 2));
        }

        [DisplayName("Conventional Future Feature 4")]
        public bool ConventionalFutureFeature4
        {
            get => (FeatureBlock[0x0F] & (1 << 1)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 1)) : (byte)(FeatureBlock[0x0F] & ~(1 << 1));
        }

        [DisplayName("Conventional Future Feature 5")]
        public bool ConventionalFutureFeature5
        {
            get => (FeatureBlock[0x0F] & (1 << 0)) > 0;
            set => FeatureBlock[0x0F] = value ? (byte)(FeatureBlock[0x0F] | (1 << 0)) : (byte)(FeatureBlock[0x0F] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x10
        [DisplayName("HHCH")]
        public bool _10Bit7
        {
            get => (FeatureBlock[0x10] & (1 << 7)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 7)) : (byte)(FeatureBlock[0x10] & ~(1 << 7));
        }

        [DisplayName("HHCH Future Feature 1")]
        public bool HHCHFutureFeature1
        {
            get => (FeatureBlock[0x10] & (1 << 6)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 6)) : (byte)(FeatureBlock[0x10] & ~(1 << 6));
        }

        [DisplayName("HHCH Future Feature 2")]
        public bool HHCHFutureFeature2
        {
            get => (FeatureBlock[0x10] & (1 << 5)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 5)) : (byte)(FeatureBlock[0x10] & ~(1 << 5));
        }

        [DisplayName("HHCH Future Feature 3")]
        public bool HHCHFutureFeature3
        {
            get => (FeatureBlock[0x10] & (1 << 4)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 4)) : (byte)(FeatureBlock[0x10] & ~(1 << 4));
        }

        [DisplayName("HHCH Future Feature 4")]
        public bool HHCHFutureFeature4
        {
            get => (FeatureBlock[0x10] & (1 << 3)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 3)) : (byte)(FeatureBlock[0x10] & ~(1 << 3));
        }

        [DisplayName("HHCH Future Feature 5")]
        public bool HHCHFutureFeature5
        {
            get => (FeatureBlock[0x10] & (1 << 2)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 2)) : (byte)(FeatureBlock[0x10] & ~(1 << 2));
        }

        [DisplayName("HHCH Future Feature 6")]
        public bool HHCHFutureFeature6
        {
            get => (FeatureBlock[0x10] & (1 << 1)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 1)) : (byte)(FeatureBlock[0x10] & ~(1 << 1));
        }

        [DisplayName("HHCH Future Feature 7")]
        public bool HHCHFutureFeature7
        {
            get => (FeatureBlock[0x10] & (1 << 0)) > 0;
            set => FeatureBlock[0x10] = value ? (byte)(FeatureBlock[0x10] | (1 << 0)) : (byte)(FeatureBlock[0x10] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x11
        [DisplayName("HHCH Future Feature 8")]
        public bool HHCHFutureFeature8
        {
            get => (FeatureBlock[0x11] & (1 << 7)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 7)) : (byte)(FeatureBlock[0x11] & ~(1 << 7));
        }

        [DisplayName("HHCH Future Feature 9")]
        public bool HHCHFutureFeature9
        {
            get => (FeatureBlock[0x11] & (1 << 6)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 6)) : (byte)(FeatureBlock[0x11] & ~(1 << 6));
        }

        [DisplayName("HHCH Future Feature 10")]
        public bool HHCHFutureFeature10
        {
            get => (FeatureBlock[0x11] & (1 << 5)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 5)) : (byte)(FeatureBlock[0x11] & ~(1 << 5));
        }

        [DisplayName("HHCH Future Feature 11")]
        public bool HHCHFutureFeature11
        {
            get => (FeatureBlock[0x11] & (1 << 4)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 4)) : (byte)(FeatureBlock[0x11] & ~(1 << 4));
        }

        [DisplayName("HHCH Future Feature 12")]
        public bool HHCHFutureFeature12
        {
            get => (FeatureBlock[0x11] & (1 << 3)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 3)) : (byte)(FeatureBlock[0x11] & ~(1 << 3));
        }

        [DisplayName("HHCH Future Feature 13")]
        public bool HHCHFutureFeature13
        {
            get => (FeatureBlock[0x11] & (1 << 2)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 2)) : (byte)(FeatureBlock[0x11] & ~(1 << 2));
        }

        [DisplayName("HHCH Future Feature 14")]
        public bool HHCHFutureFeature14
        {
            get => (FeatureBlock[0x11] & (1 << 1)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 1)) : (byte)(FeatureBlock[0x11] & ~(1 << 1));
        }

        [DisplayName("HHCH Future Feature 15")]
        public bool HHCHFutureFeature15
        {
            get => (FeatureBlock[0x11] & (1 << 0)) > 0;
            set => FeatureBlock[0x11] = value ? (byte)(FeatureBlock[0x11] | (1 << 0)) : (byte)(FeatureBlock[0x11] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x12
        [DisplayName("Secure")]
        public bool Secure
        {
            get => (FeatureBlock[0x12] & (1 << 7)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 7)) : (byte)(FeatureBlock[0x12] & ~(1 << 7));
        }

        [DisplayName("Single Key Software Encryption")]
        public bool SingleKeySoftwareEncryption
        {
            get => (FeatureBlock[0x12] & (1 << 6)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 6)) : (byte)(FeatureBlock[0x12] & ~(1 << 6));
        }

        [DisplayName("Multikey Software Encryption")]
        public bool MultikeySoftwareEncryption
        {
            get => (FeatureBlock[0x12] & (1 << 5)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 5)) : (byte)(FeatureBlock[0x12] & ~(1 << 5));
        }

        [DisplayName("FIPS II")]
        public bool FIPSII
        {
            get => (FeatureBlock[0x12] & (1 << 4)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 4)) : (byte)(FeatureBlock[0x12] & ~(1 << 4));
        }

        [DisplayName("Infinite Key Retention")]
        public bool InfiniteKeyRetention
        {
            get => (FeatureBlock[0x12] & (1 << 3)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 3)) : (byte)(FeatureBlock[0x12] & ~(1 << 3));
        }

        [DisplayName("Conventional Multikey")]
        public bool ConventionalMultikey
        {
            get => (FeatureBlock[0x12] & (1 << 2)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 2)) : (byte)(FeatureBlock[0x12] & ~(1 << 2));
        }

        [DisplayName("Conventional OTAR")]
        public bool ConventionalOTAR
        {
            get => (FeatureBlock[0x12] & (1 << 1)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 1)) : (byte)(FeatureBlock[0x12] & ~(1 << 1));
        }

        [DisplayName("Future Feature Byte 12 Bit 0")]
        public bool _12Bit0
        {
            get => (FeatureBlock[0x12] & (1 << 0)) > 0;
            set => FeatureBlock[0x12] = value ? (byte)(FeatureBlock[0x12] | (1 << 0)) : (byte)(FeatureBlock[0x12] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x13
        [DisplayName("Trunked Multikey")]
        public bool TrunkedMultikey
        {
            get => (FeatureBlock[0x13] & (1 << 7)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 7)) : (byte)(FeatureBlock[0x13] & ~(1 << 7));
        }

        [DisplayName("Trunked OTAR")]
        public bool TrunkedOTAR
        {
            get => (FeatureBlock[0x13] & (1 << 6)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 6)) : (byte)(FeatureBlock[0x13] & ~(1 << 6));
        }

        [DisplayName("Remote Monitor Radio Trace")]
        public bool RemoteMonitorRadioTrace
        {
            get => (FeatureBlock[0x13] & (1 << 5)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 5)) : (byte)(FeatureBlock[0x13] & ~(1 << 5));
        }

        [DisplayName("Secure Future Feature 2")]
        public bool SecureFutureFeature2
        {
            get => (FeatureBlock[0x13] & (1 << 4)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 4)) : (byte)(FeatureBlock[0x13] & ~(1 << 4));
        }

        [DisplayName("Secure Future Feature 3")]
        public bool SecureFutureFeature3
        {
            get => (FeatureBlock[0x13] & (1 << 3)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 3)) : (byte)(FeatureBlock[0x13] & ~(1 << 3));
        }

        [DisplayName("Secure Future Feature 4")]
        public bool SecureFutureFeature4
        {
            get => (FeatureBlock[0x13] & (1 << 2)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 2)) : (byte)(FeatureBlock[0x13] & ~(1 << 2));
        }

        [DisplayName("Secure Future Feature 5")]
        public bool SecureFutureFeature5
        {
            get => (FeatureBlock[0x13] & (1 << 1)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 1)) : (byte)(FeatureBlock[0x13] & ~(1 << 1));
        }

        [DisplayName("Secure Future Feature 6")]
        public bool SecureFutureFeature6
        {
            get => (FeatureBlock[0x13] & (1 << 0)) > 0;
            set => FeatureBlock[0x13] = value ? (byte)(FeatureBlock[0x13] | (1 << 0)) : (byte)(FeatureBlock[0x13] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x14
        [DisplayName("TrunkingHorn&Lights")]
        public bool TrunkingHornAndLights
        {
            get => (FeatureBlock[0x14] & (1 << 7)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 7)) : (byte)(FeatureBlock[0x14] & ~(1 << 7));
        }

        [DisplayName("MDC Horn & Lights")]
        public bool MDCHornAndLights
        {
            get => (FeatureBlock[0x14] & (1 << 6)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 6)) : (byte)(FeatureBlock[0x14] & ~(1 << 6));
        }

        [DisplayName("Motorcycle Radio")]
        public bool MotorcycleRadio
        {
            get => (FeatureBlock[0x14] & (1 << 5)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 5)) : (byte)(FeatureBlock[0x14] & ~(1 << 5));
        }

        [DisplayName("Smart Status VIP")]
        public bool SmartStatusVIP
        {
            get => (FeatureBlock[0x14] & (1 << 4)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 4)) : (byte)(FeatureBlock[0x14] & ~(1 << 4));
        }

        [DisplayName("Metrocom Trunking Radio")]
        public bool MetrocomTrunkingRadio
        {
            get => (FeatureBlock[0x14] & (1 << 3)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 3)) : (byte)(FeatureBlock[0x14] & ~(1 << 3));
        }

        [DisplayName("Metrocom Conventional Radio")]
        public bool MetrocomConventionalRadio
        {
            get => (FeatureBlock[0x14] & (1 << 2)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 2)) : (byte)(FeatureBlock[0x14] & ~(1 << 2));
        }

        [DisplayName("Auxiliary Receiver")]
        public bool AuxiliaryReceiver
        {
            get => (FeatureBlock[0x14] & (1 << 1)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 1)) : (byte)(FeatureBlock[0x14] & ~(1 << 1));
        }

        [DisplayName("Multi-radio System")]
        public bool MultiRadioSystem
        {
            get => (FeatureBlock[0x14] & (1 << 0)) > 0;
            set => FeatureBlock[0x14] = value ? (byte)(FeatureBlock[0x14] | (1 << 0)) : (byte)(FeatureBlock[0x14] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x15
        [DisplayName("Speaker Absent")]
        public bool SpeakerAbsent
        {
            get => (FeatureBlock[0x15] & (1 << 7)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 7)) : (byte)(FeatureBlock[0x15] & ~(1 << 7));
        }

        [DisplayName("Internal PA")]
        public bool InternalPA
        {
            get => (FeatureBlock[0x15] & (1 << 6)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 6)) : (byte)(FeatureBlock[0x15] & ~(1 << 6));
        }

        [DisplayName("Vehicular Repeater")]
        public bool VehicularRepeater
        {
            get => (FeatureBlock[0x15] & (1 << 5)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 5)) : (byte)(FeatureBlock[0x15] & ~(1 << 5));
        }

        [DisplayName("Siren PA Option Capable")]
        public bool SirenPAOptionCapable
        {
            get => (FeatureBlock[0x15] & (1 << 4)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 4)) : (byte)(FeatureBlock[0x15] & ~(1 << 4));
        }

        [DisplayName("DEK Option Capable")]
        public bool DEKOptionCapable
        {
            get => (FeatureBlock[0x15] & (1 << 3)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 3)) : (byte)(FeatureBlock[0x15] & ~(1 << 3));
        }

        [DisplayName("APCO Packet Data")]
        public bool APCOPacketData
        {
            get => (FeatureBlock[0x15] & (1 << 2)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 2)) : (byte)(FeatureBlock[0x15] & ~(1 << 2));
        }

        [DisplayName("Enhanced Radio Control Protocol (RCP)")]
        public bool EnhancedRadioControlProtocol
        {
            get => (FeatureBlock[0x15] & (1 << 1)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 1)) : (byte)(FeatureBlock[0x15] & ~(1 << 1));
        }

        [DisplayName("IMBE Digital Operation (CAI)")]
        public bool IMBEDigitalOperation
        {
            get => (FeatureBlock[0x15] & (1 << 0)) > 0;
            set => FeatureBlock[0x15] = value ? (byte)(FeatureBlock[0x15] | (1 << 0)) : (byte)(FeatureBlock[0x15] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x16
        [DisplayName("Omnilink")]
        public bool Omnilink
        {
            get => (FeatureBlock[0x16] & (1 << 7)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 7)) : (byte)(FeatureBlock[0x16] & ~(1 << 7));
        }

        [DisplayName("Reserved for REDI Smart Messaging")]
        [Description("Leave to False. CPS will refuse to open if set.")]
        public bool ReservedForREDISmartMessaging
        {
            get => (FeatureBlock[0x16] & (1 << 6)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 6)) : (byte)(FeatureBlock[0x16] & ~(1 << 6));
        }

        [DisplayName("APCO Trunking (ASTRO25 9600bps operation)")]
        public bool APCOTrunking
        {
            get => (FeatureBlock[0x16] & (1 << 5)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 5)) : (byte)(FeatureBlock[0x16] & ~(1 << 5));
        }

        [DisplayName("Scan with VRS")]
        public bool ScanWithVRS
        {
            get => (FeatureBlock[0x16] & (1 << 4)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 4)) : (byte)(FeatureBlock[0x16] & ~(1 << 4));
        }

        [DisplayName("Zone Mode NJSP")]
        public bool ZoneModeNJSP
        {
            get => (FeatureBlock[0x16] & (1 << 3)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 3)) : (byte)(FeatureBlock[0x16] & ~(1 << 3));
        }

        [DisplayName("Mobile Future Feature 10")]
        public bool MobileFutureFeature10
        {
            get => (FeatureBlock[0x16] & (1 << 2)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 2)) : (byte)(FeatureBlock[0x16] & ~(1 << 2));
        }

        [DisplayName("Mobile Future Feature 11")]
        public bool MobileFutureFeature11
        {
            get => (FeatureBlock[0x16] & (1 << 1)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 1)) : (byte)(FeatureBlock[0x16] & ~(1 << 1));
        }

        [DisplayName("Mobile Future Feature 12")]
        public bool MobileFutureFeature12
        {
            get => (FeatureBlock[0x16] & (1 << 0)) > 0;
            set => FeatureBlock[0x16] = value ? (byte)(FeatureBlock[0x16] | (1 << 0)) : (byte)(FeatureBlock[0x16] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x17
        [DisplayName("256K Flash Memory")]
        public bool FlashMemory256KB
        {
            get => (FeatureBlock[0x17] & (1 << 7)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 7)) : (byte)(FeatureBlock[0x17] & ~(1 << 7));
        }

        [DisplayName("512K Flash Memory")]
        public bool FlashMemory512KB
        {
            get => (FeatureBlock[0x17] & (1 << 6)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 6)) : (byte)(FeatureBlock[0x17] & ~(1 << 6));
        }

        [DisplayName("1 Meg Flash Memory")]
        public bool FlashMemory1MB
        {
            get => (FeatureBlock[0x17] & (1 << 5)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 5)) : (byte)(FeatureBlock[0x17] & ~(1 << 5));
        }

        [DisplayName("Flash Portable Radio")]
        public bool FlashPortableRadio
        {
            get => (FeatureBlock[0x17] & (1 << 4)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 4)) : (byte)(FeatureBlock[0x17] & ~(1 << 4));
        }

        [DisplayName("DSP 256K Flash Memory")]
        public bool DSP256KFlashMemory
        {
            get => (FeatureBlock[0x17] & (1 << 3)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 3)) : (byte)(FeatureBlock[0x17] & ~(1 << 3));
        }

        [DisplayName("384K Flash Memory")]
        public bool FlashMemory384KB
        {
            get => (FeatureBlock[0x17] & (1 << 2)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 2)) : (byte)(FeatureBlock[0x17] & ~(1 << 2));
        }

        [DisplayName("DSP 512K Flash Memory")]
        public bool DSP512KFlashMemory
        {
            get => (FeatureBlock[0x17] & (1 << 1)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 1)) : (byte)(FeatureBlock[0x17] & ~(1 << 1));
        }

        [DisplayName("DSP 1 Meg Flash Memory")]
        public bool DSP1MegFlashMemory
        {
            get => (FeatureBlock[0x17] & (1 << 0)) > 0;
            set => FeatureBlock[0x17] = value ? (byte)(FeatureBlock[0x17] | (1 << 0)) : (byte)(FeatureBlock[0x17] & ~(1 << 0));
        }
        #endregion

        #region FDB BYTE 0x18
        [DisplayName("Unknown")]
        public bool _18Bit7
        {
            get => (FeatureBlock[0x18] & (1 << 7)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 7)) : (byte)(FeatureBlock[0x18] & ~(1 << 7));
        }

        [DisplayName("Unknown")]
        public bool _18Bit6
        {
            get => (FeatureBlock[0x18] & (1 << 6)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 6)) : (byte)(FeatureBlock[0x18] & ~(1 << 6));
        }

        [DisplayName("Unknown")]
        public bool _18Bit5
        {
            get => (FeatureBlock[0x18] & (1 << 5)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 5)) : (byte)(FeatureBlock[0x18] & ~(1 << 5));
        }

        [DisplayName("Unknown")]
        public bool _18Bit4
        {
            get => (FeatureBlock[0x18] & (1 << 4)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 4)) : (byte)(FeatureBlock[0x18] & ~(1 << 4));
        }

        [DisplayName("Unknown")]
        public bool _18Bit3
        {
            get => (FeatureBlock[0x18] & (1 << 3)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 3)) : (byte)(FeatureBlock[0x18] & ~(1 << 3));
        }

        [DisplayName("Unknown")]
        public bool _18Bit2
        {
            get => (FeatureBlock[0x18] & (1 << 2)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 2)) : (byte)(FeatureBlock[0x18] & ~(1 << 2));
        }

        [DisplayName("Unknown")]
        public bool _18Bit1
        {
            get => (FeatureBlock[0x18] & (1 << 1)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 1)) : (byte)(FeatureBlock[0x18] & ~(1 << 1));
        }

        [DisplayName("Unknown")]
        public bool _18Bit0
        {
            get => (FeatureBlock[0x18] & (1 << 0)) > 0;
            set => FeatureBlock[0x18] = value ? (byte)(FeatureBlock[0x18] | (1 << 0)) : (byte)(FeatureBlock[0x18] & ~(1 << 0));
        }
        #endregion



        #region Flash Code BYTE 0x00
        [DisplayName("XXXX")]
        public bool _FC00Bit7
        {
            get => (Flashcode[0x00] & (1 << 7)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 7)) : (byte)(Flashcode[0x00] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit6
        {
            get => (Flashcode[0x00] & (1 << 6)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 6)) : (byte)(Flashcode[0x00] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit5
        {
            get => (Flashcode[0x00] & (1 << 5)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 5)) : (byte)(Flashcode[0x00] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit4
        {
            get => (Flashcode[0x00] & (1 << 4)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 4)) : (byte)(Flashcode[0x00] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit3
        {
            get => (Flashcode[0x00] & (1 << 3)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 3)) : (byte)(Flashcode[0x00] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit2
        {
            get => (Flashcode[0x00] & (1 << 2)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 2)) : (byte)(Flashcode[0x00] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit1
        {
            get => (Flashcode[0x00] & (1 << 1)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 1)) : (byte)(Flashcode[0x00] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC00Bit0
        {
            get => (Flashcode[0x00] & (1 << 0)) > 0;
            set => Flashcode[0x00] = value ? (byte)(Flashcode[0x00] | (1 << 0)) : (byte)(Flashcode[0x00] & ~(1 << 0));
        }
        #endregion

        #region Flash Code BYTE 0x01
        [DisplayName("XXXX")]
        public bool _FC01Bit7
        {
            get => (Flashcode[0x01] & (1 << 7)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 7)) : (byte)(Flashcode[0x01] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit6
        {
            get => (Flashcode[0x01] & (1 << 6)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 6)) : (byte)(Flashcode[0x01] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit5
        {
            get => (Flashcode[0x01] & (1 << 5)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 5)) : (byte)(Flashcode[0x01] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit4
        {
            get => (Flashcode[0x01] & (1 << 4)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 4)) : (byte)(Flashcode[0x01] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit3
        {
            get => (Flashcode[0x01] & (1 << 3)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 3)) : (byte)(Flashcode[0x01] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit2
        {
            get => (Flashcode[0x01] & (1 << 2)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 2)) : (byte)(Flashcode[0x01] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit1
        {
            get => (Flashcode[0x01] & (1 << 1)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 1)) : (byte)(Flashcode[0x01] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC01Bit0
        {
            get => (Flashcode[0x01] & (1 << 0)) > 0;
            set => Flashcode[0x01] = value ? (byte)(Flashcode[0x01] | (1 << 0)) : (byte)(Flashcode[0x01] & ~(1 << 0));
        }
        #endregion

        #region Flash Code BYTE 0x02
        [DisplayName("XXXX")]
        public bool _FC02Bit7
        {
            get => (Flashcode[0x02] & (1 << 7)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 7)) : (byte)(Flashcode[0x02] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit6
        {
            get => (Flashcode[0x02] & (1 << 6)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 6)) : (byte)(Flashcode[0x02] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit5
        {
            get => (Flashcode[0x02] & (1 << 5)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 5)) : (byte)(Flashcode[0x02] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit4
        {
            get => (Flashcode[0x02] & (1 << 4)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 4)) : (byte)(Flashcode[0x02] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit3
        {
            get => (Flashcode[0x02] & (1 << 3)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 3)) : (byte)(Flashcode[0x02] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit2
        {
            get => (Flashcode[0x02] & (1 << 2)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 2)) : (byte)(Flashcode[0x02] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit1
        {
            get => (Flashcode[0x02] & (1 << 1)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 1)) : (byte)(Flashcode[0x02] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC02Bit0
        {
            get => (Flashcode[0x02] & (1 << 0)) > 0;
            set => Flashcode[0x02] = value ? (byte)(Flashcode[0x02] | (1 << 0)) : (byte)(Flashcode[0x02] & ~(1 << 0));
        }
        #endregion

        #region Flash Code BYTE 0x03
        [DisplayName("XXXX")]
        public bool _FC03Bit7
        {
            get => (Flashcode[0x03] & (1 << 7)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 7)) : (byte)(Flashcode[0x03] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit6
        {
            get => (Flashcode[0x03] & (1 << 6)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 6)) : (byte)(Flashcode[0x03] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit5
        {
            get => (Flashcode[0x03] & (1 << 5)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 5)) : (byte)(Flashcode[0x03] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit4
        {
            get => (Flashcode[0x03] & (1 << 4)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 4)) : (byte)(Flashcode[0x03] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit3
        {
            get => (Flashcode[0x03] & (1 << 3)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 3)) : (byte)(Flashcode[0x03] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit2
        {
            get => (Flashcode[0x03] & (1 << 2)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 2)) : (byte)(Flashcode[0x03] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit1
        {
            get => (Flashcode[0x03] & (1 << 1)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 1)) : (byte)(Flashcode[0x03] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC03Bit0
        {
            get => (Flashcode[0x03] & (1 << 0)) > 0;
            set => Flashcode[0x03] = value ? (byte)(Flashcode[0x03] | (1 << 0)) : (byte)(Flashcode[0x03] & ~(1 << 0));
        }
        #endregion

        #region Flash Code BYTE 0x04
        [DisplayName("XXXX")]
        public bool _FC04Bit7
        {
            get => (Flashcode[0x04] & (1 << 7)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 7)) : (byte)(Flashcode[0x04] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit6
        {
            get => (Flashcode[0x04] & (1 << 6)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 6)) : (byte)(Flashcode[0x04] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit5
        {
            get => (Flashcode[0x04] & (1 << 5)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 5)) : (byte)(Flashcode[0x04] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit4
        {
            get => (Flashcode[0x04] & (1 << 4)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 4)) : (byte)(Flashcode[0x04] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit3
        {
            get => (Flashcode[0x04] & (1 << 3)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 3)) : (byte)(Flashcode[0x04] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit2
        {
            get => (Flashcode[0x04] & (1 << 2)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 2)) : (byte)(Flashcode[0x04] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit1
        {
            get => (Flashcode[0x04] & (1 << 1)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 1)) : (byte)(Flashcode[0x04] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC04Bit0
        {
            get => (Flashcode[0x04] & (1 << 0)) > 0;
            set => Flashcode[0x04] = value ? (byte)(Flashcode[0x04] | (1 << 0)) : (byte)(Flashcode[0x04] & ~(1 << 0));
        }
        #endregion

        #region Flash Code BYTE 0x05
        [DisplayName("XXXX")]
        public bool _FC05Bit7
        {
            get => (Flashcode[0x05] & (1 << 7)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 7)) : (byte)(Flashcode[0x05] & ~(1 << 7));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit6
        {
            get => (Flashcode[0x05] & (1 << 6)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 6)) : (byte)(Flashcode[0x05] & ~(1 << 6));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit5
        {
            get => (Flashcode[0x05] & (1 << 5)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 5)) : (byte)(Flashcode[0x05] & ~(1 << 5));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit4
        {
            get => (Flashcode[0x05] & (1 << 4)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 4)) : (byte)(Flashcode[0x05] & ~(1 << 4));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit3
        {
            get => (Flashcode[0x05] & (1 << 3)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 3)) : (byte)(Flashcode[0x05] & ~(1 << 3));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit2
        {
            get => (Flashcode[0x05] & (1 << 2)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 2)) : (byte)(Flashcode[0x05] & ~(1 << 2));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit1
        {
            get => (Flashcode[0x05] & (1 << 1)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 1)) : (byte)(Flashcode[0x05] & ~(1 << 1));
        }

        [DisplayName("XXXX")]
        public bool _FC05Bit0
        {
            get => (Flashcode[0x05] & (1 << 0)) > 0;
            set => Flashcode[0x05] = value ? (byte)(Flashcode[0x05] | (1 << 0)) : (byte)(Flashcode[0x05] & ~(1 << 0));
        }
        #endregion

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
