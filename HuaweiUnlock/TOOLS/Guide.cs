using System.Collections.Generic;

namespace HuaweiUnlocker
{
    class Guide
    {
        ///<summary>
        ///Справочник типов программного обеспечения из сертификата
        ///</summary>
        internal readonly Dictionary<string, string> SW_ID_type = new Dictionary<string, string>(32)
        {
            { "0", "SBL1 image" },
            { "1", "MBA image" },
            { "2", "Modem image hashtable" },
            { "3", "Firehose programmer" },
            { "4", "lpass etc running on ADSP" },
            { "5", "Device configuration image" },
            { "6", "Reserved" },
            { "7", "TZBSP image" },
            { "8", "Reserved" },
            { "9", "APPSBL" },
            { "A", "RPM firmware" },
            { "B", "Reserved" },
            { "C", "TrustZone applications - Playready/TrustZone" },
            { "D", "Pronto/WCN image" },
            { "E", "Venus image" },
            { "F", "Reserved" },
            { "11", "STI image" },
            { "12", "System debug image" },
            { "13", "ACPI image" },
            { "14", "GPU Microcode" },
            { "15", "Hypervisor image" },
            { "16", "PMIC" },
            { "17", "Reserved" },
            { "18", "Sensor Low Power Island (SLPI)" },
            { "19", "EOS firmware image" },
            { "1A", "Validated image programmer (VIP)" },
            { "1B", "EFS TARfile" },
            { "1C", "ABL image" },
            { "1D", "IPA firmware" },
            { "1E", "TrustZone TEE" },
            { "1F", "TrustZone LIB" },
            { "200", "Debug Policy" }
        };

        /// <summary>
        /// Справочник типов памяти
        /// </summary>
        internal enum MEM_TYPE
        {
            eMMC,
            UFS
        }

        /// <summary>
        /// Дубликаты CPU
        /// </summary>
        internal readonly Dictionary<string, string> Double_CPU = new Dictionary<string, string>(48)
        {
            { "009F00E1", "APQ8056" },
            { "009710E1", "APQ8056" },
            { "007190E1", "APQ8056" },
            { "009830E1", "APQ8076" },
            { "009D00E1", "APQ8076" },
            { "009000E1", "APQ8084" },
            { "009010E1", "APQ8084" },
            { "009300E1", "APQ8092" },
            { "009630E1", "APQ8092" },
            { "008000E1", "MSM8226" },
            { "009150E1", "MSM8226" },
            { "007210E1", "MSM8930" },
            { "0072C0E1", "MSM8930" },
            { "0090B0E1", "MSM8939" },
            { "050E10E1", "MSM8939" },
            { "000460E1", "MSM8953" },
            { "F00460E1", "MSM8953" },
            { "009700E1", "MSM8956" },
            { "009B00E1", "MSM8956" },
            { "007B00E1", "MSM8974" },
            { "007B20E1", "MSM8974" },
            { "007B40E1", "MSM8974AB" },
            { "007BA0E1", "MSM8974AB" },
            { "006B10E1", "MSM8974AC" },
            { "007B60E1", "MSM8974AC" },
            { "009640E1", "MSM8992" },
            { "009690E1", "MSM8992" },
            { "000630E1", "MSM8996AU" },
            { "0006F0E1", "MSM8996AU" },
            { "1006F0E1", "MSM8996AU" },
            { "4006F0E1", "MSM8996AU" },
            { "30020000", "MSM8998" },
            { "0005E0E1", "MSM8998" },
            { "00FFF0E1", "MSM8998" },
            { "000020E1", "MSM8998" },
            { "30070000", "SDM630" },
            { "0007E0E1", "SDM630" },
            { "000AC0E1", "SDM636" },
            { "000CC0E1", "SDM636" },
            { "F00CC0E1", "SDM636" },
            { "30060000", "SDM660" },
            { "0008C0E1", "SDM660" },
            { "0009C0E1", "SDM660" },
            { "0007D0E1", "SDM660" },
            { "001080E1", "SDM712" },
            { "60040000", "SDM712" },
            { "60000000", "SDM845" },
            { "0008B0E1", "SDM845" }
        };

        /// <summary>
        /// Список признаков отношения файла к шлангу
        /// </summary>
        internal enum FH_magic_numbers : uint
        {
            ELF = 2135247942, //7F454C46 - ELF
            ELE = 2135247941, //7F454C45 - ELE
            OLD = 3520875396, //D1DC4B84 - Старый программер
            PATCHEDOLD = 1442835967, //55FFEDFF - Паченый старый программер
            ZTEEncode = 3973917226, //ECDD2A2A - ZTE программер зашифрованный
            ARMPRG = 117440512, //07000000 - самый старый программер
            ARM9 = 218103808, //0D000000 - ARMPRG 9
            ARM412 = 2484117477, //94109FE5 - ARMPRG 0412
            ARM12 = 4094730213, //F4109FE5 - ARMPRG 120001
            ARM14 = 83886080, //05000000 - ARMPRG 140000
            UFSEncoding = 1684108385, //64617461 - Закодированный UFS программер
            XiUFSEnc = 2358991973, //8C9B5C65 - Xiaomi закодированный UFS программер
            OLDasus = 930790575, //377ABCAF - Старый Асус программер
            OLDESTasus = 1347093252 //504B0304 - Ещё один старый Асус программер
        }

        /// <summary>
        /// Список признаков типа архитектуры эльф-файла
        /// </summary>
        internal enum ELF_AArch : byte
        {
            Invalid,
            bit32,
            bit64
        }

        /// <summary>
        /// Признак алгоритма подписи сертификата в коде программера
        /// </summary>
        internal readonly Dictionary<string, int> SHA_magic_numbers = new Dictionary<string, int>(5)
        {
            { "06092A864886F70D010101", 0 }, //SHA384 - старые серты
            { "06092A864886F70D010105", 1 }, //SHA256 - старые серты
            { "06092A864886F70D01010B", 2 }, //SHA256 - нормальные серты
            { "06092A864886F70D01010A", 3 }, //SHA384 - новые серты
            { "06072A8648CE3D020106052B81040022", 4 }  //SHA384 - паченый старый программер
        };

        /// <summary>
        /// Список файлов, которые нужно удалить после закрытия программы
        /// </summary>
        internal readonly List<string> FilesToClean = new List<string>(19)
        {
            {"commandop01.bin" },
            {"commandop02.bin" },
            {"commandop03.bin" },
            {"commandop07.bin" },
            {"work.xml" },
            {"p_r.xml" },
            {"port_trace.txt" },
            {"gpt_backup0.bin" },
            {"gpt_backup1.bin" },
            {"gpt_backup2.bin" },
            {"gpt_backup3.bin" },
            {"gpt_backup4.bin" },
            {"gpt_backup5.bin" },
            {"gpt_main0.bin" },
            {"gpt_main1.bin" },
            {"gpt_main2.bin" },
            {"gpt_main3.bin" },
            {"gpt_main4.bin" },
            {"gpt_main5.bin" }
        };

        /// <summary>
        /// Список процессов, которые необходимо подчистить
        /// </summary>
        internal readonly List<string> ProcessToClean = new List<string>(4)
        {
            {"qsaharaserver" },
            {"fh_loader" },
            {"adb" },
            {"fastboot" }
        };
    }

    /// <summary>
    /// Параметры выбранного диска, обязательные для использования.
    /// </summary>
    class Flash_Disk
    {
        public int Total_Sectors;
        public int Sector_Size;
        public int Count_Lun;
        public Flash_Disk(int total_sectors, int sector_size, int count_lun)
        {
            Total_Sectors = total_sectors;
            Sector_Size = sector_size;
            Count_Lun = count_lun;
        }
    }

    class Search_Hex
    {
        public List<string> FullFileNames;
        public string SearchString;
        public Search_Hex(List<string> fullfilenames, string searchstring)
        {
            FullFileNames = fullfilenames;
            SearchString = searchstring;
        }
    }

    class Search_Result
    {
        public string Adress_hex;
        public string Result_String;
        public string File_Name;
        public Search_Result(string adress_hex, string result_string, string file_name)
        {
            Adress_hex = adress_hex;
            Result_String = result_string;
            File_Name = file_name;
        }
    }
}
