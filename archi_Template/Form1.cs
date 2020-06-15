using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace archi_Template
{
    public partial class Form1 : Form
    {


        

        
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Intialize all Data needed through runtime
        /// </summary>
        Dictionary<int, string> userCode = new Dictionary<int, string>();
        Dictionary<int, int> MipsRegisters = new Dictionary<int, int>();
        Dictionary<string, string> DataMemory = new Dictionary<string, string>();
        int CycleCount = 1;
        int instructionNumber;
       

        void LoadUserCode() {

            string[] instructions = UserCodetxt.Text.ToString().Split('\n');
           // MessageBox.Show(instructions.Length.ToString());
            for (int i = 0; i < instructions.Length; i++) {
               // MessageBox.Show(instructions[i].Length.ToString());
                if (instructions[i].Length != 0)
                {
                    string[] eachInstruction = instructions[i].Split(':');
                    if(eachInstruction[1].Length == 36)
                        userCode.Add(int.Parse(eachInstruction[0]), eachInstruction[1].Substring(0, 35));

                    else
                        userCode.Add(int.Parse(eachInstruction[0]), eachInstruction[1].Substring(0,37));
                }
            }
      
            
          //  MessageBox.Show(userCode.Keys.ElementAt(0).ToString()+'\n'+ userCode.Keys.ElementAt(1).ToString() +'\n'+ userCode.Keys.ElementAt(2).ToString()+'\n' + userCode.Keys.ElementAt(3).ToString() +'\n'+ userCode.Keys.ElementAt(4).ToString() );
          // MessageBox.Show(userCode.Values.ElementAt(0).ToString() + '\n' + userCode.Values.ElementAt(1).ToString() + '\n' + userCode.Values.ElementAt(2).ToString() + '\n' + userCode.Values.ElementAt(3).ToString() + '\n' + userCode.Values.ElementAt(4).ToString());
         

        }
        
        void LoadMipsRegistersTableView() {
            MipsRegisterGrid.Rows.Clear();

            MipsRegisterGrid.Rows.Add("$0", "0");

            int count = 1;
            while (count <= 31)
            {
                int acutalValue = count + 100;

                MipsRegisterGrid.Rows.Add("$" + MipsRegisters.Keys.ElementAt(count).ToString(), MipsRegisters.Values.ElementAt(count).ToString());

                count++;
            }
        }

        void LoadMipsRegisters() {

            MipsRegisterGrid.Rows.Add("$0", "0");

            MipsRegisters.Add(0, 0);
            int count = 1 ;
            while( count <= 31)
            {
                int actualVlaue = count + 100;

                MipsRegisters.Add(count, actualVlaue);

                count++;

            }
        }

        void loadDataMemoryTableView() {

            int count = 0;
            int actualValur = 99;

            while (count<=1024)
            {

                string dataMemoryLocation = Convert.ToString(count, 16);

                DataMemory.Add(dataMemoryLocation, actualValur.ToString());

                MemoryGrid.Rows.Add("0x" + dataMemoryLocation, actualValur.ToString());
                
                count += 4;


            }
        }
        private void inializeBtn_Click(object sender, EventArgs e)
        {
            LoadUserCode();
            LoadMipsRegisters();
            LoadMipsRegistersTableView();
            loadDataMemoryTableView();
            UserCodetxt.Enabled = false;
            StartPCTxt.Enabled = false;
            inializeBtn.Enabled = false;
            instructionNumber = int.Parse(StartPCTxt.Text);
            MessageBox.Show("      UserCode are set \n      MIPSRegister are set\n      DataMemory are set");


        }


        /// <summary>
        /// The acutal cycle Run 
        /// </summary>
        /// <param name="position"></param>
        /// 

        struct DataSaved {
            public static Dictionary<int, string> DecodeStages = new Dictionary<int, string>();
            public static Dictionary<int, string> MemoryStages = new Dictionary<int, string>();
            public static Dictionary<int, string> ExcuteStages = new Dictionary<int, string>();
        }
        struct DataReadWrite
        {
            public static int FirstRegister;
            public static int SecondRegister;
            public static int DistinationRegister;
            public static int FirstRegisterActualValue;
            public static int SecondRegisterActualValue;


        }
        struct Helpers
        {
            public static string FunctionCode;
            public static int SignExtention;


        }
        struct Pipeline
        {
            public static string IF_ID;
            public static string ID_EX;
            public static string EX_MEM;
            public static string MEM_WB;

        }
        struct Controllers
        {


            public static string Branch;
            public static string RegisterWrite;
            public static string MemoryRegister;
            public static string RegisterDistination;
            public static string AluOpp;
            public static string ALUSource;
            public static string MemeryRead;
            public static string MemryWrite;
            public static string WE;
            public static string M;
            public static string EX;

        }

       

      
       
       
       
        void MemoryAccessStage(int Position) {
            string[] DataSpilted = DataSaved.ExcuteStages[Position].Split(' ');
            Pipeline.MEM_WB = DataSpilted[1] + " " + DataSpilted[0] + " " + DataSpilted[2] + " " + "99";
            DataSaved.MemoryStages.Add(Position, Pipeline.MEM_WB);



        }
       
     
        int ResultOfAlU(int Position, string code, string AluOpp)
        {

            if (AluOpp != "10"){
                string[] DataSpilted = userCode[Position].Split(' ');
                int Pos = Convert.ToInt32(DataSpilted[1], 2);
                int actualValue = MipsRegisters[Pos];
                return actualValue + LoadSignExtention(Position);
            }
            else{
                string AndOR = "";
                string FirstHex;
                string SecondHex;
                switch (code) {
                    //////////////////////

                    
                   case "100000":
                      return DataReadWrite.FirstRegisterActualValue + DataReadWrite.SecondRegisterActualValue;
                   case "100010":
                      return DataReadWrite.FirstRegisterActualValue - DataReadWrite.SecondRegisterActualValue;
                   case "100100":
                       FirstHex = Convert.ToString(DataReadWrite.FirstRegisterActualValue, 2);
                       SecondHex = Convert.ToString(DataReadWrite.SecondRegisterActualValue, 2);
                       int count = 0;
                       while (count < FirstHex.Length){
                          if (FirstHex[count] == '0' || SecondHex[count] == '0')
                             AndOR += '0';
                          else
                              AndOR += '1';

                            count++;
                        }
                       return Convert.ToInt32(AndOR, 2);

                    case "100101":
                        FirstHex = Convert.ToString(DataReadWrite.FirstRegisterActualValue, 2);
                        SecondHex = Convert.ToString(DataReadWrite.SecondRegisterActualValue, 2);
                        int count1 = 0;
                        while (count1 < FirstHex.Length)
                        {
                            if (FirstHex[count1] == '1' || SecondHex[count1] == '1')
                                AndOR += '1';
                            else
                                AndOR += '0';

                            count1++;
                        }
                        return Convert.ToInt32(AndOR, 2);




                        ////////////////////////////////////
                }


            }

            return -1;

   }

        void DecodeStage(int Position)
        {
            string[] DataSpilted = userCode[Position].Split(' ');
            string PinsSet = PinsControlFills(Position, DataSpilted[0]);
            Pipeline.ID_EX = DataReadWrite.FirstRegisterActualValue.ToString() + " " + DataReadWrite.SecondRegisterActualValue.ToString() + " " + Helpers.SignExtention + " " + PinsSet + " " + DataReadWrite.DistinationRegister + " " + DataReadWrite.SecondRegister;
            String DataPass = DataReadWrite.DistinationRegister + " " + DataReadWrite.SecondRegister + " " + DataReadWrite.FirstRegisterActualValue.ToString() + " " + DataReadWrite.SecondRegisterActualValue.ToString();
            DataSaved.DecodeStages.Add(Position, Pipeline.ID_EX);
            UpdatePipelineRegisterTableView(1, "ID_EX", DataPass);
        }
        void registerRead(int Position) {

            string[] DataSpilted = userCode[Position].Split(' ');
            DataReadWrite.FirstRegister = Convert.ToInt32(DataSpilted[1], 2);
            DataReadWrite.SecondRegister = Convert.ToInt32(DataSpilted[2], 2);
            DataReadWrite.DistinationRegister= Convert.ToInt32(DataSpilted[3], 2);
            DataReadWrite.FirstRegisterActualValue = MipsRegisters[DataReadWrite.FirstRegister];
            DataReadWrite.SecondRegisterActualValue = MipsRegisters[DataReadWrite.SecondRegister];

        }

        int LoadSignExtention(int Position) {
            string[] DataSpilted = userCode[Position].Split(' ');
            if (DataSpilted.Length != 6) {

                if (DataSpilted[3].ElementAt(0) == '0')
                {
                    for (int i = 0; i < 16; i++)
                    {
                        DataSpilted[3] = "0" + DataSpilted[3];
                    }
                }
                else {
                    for (int i = 0; i < 16; i++)
                    {
                        DataSpilted[3] = "0" + DataSpilted[3];
                    }
                }

                return Convert.ToInt32(DataSpilted[3], 2);

            }

          return -1;
        }
        void WriteBackStage(int Position)
        {
            string[] DataSpilted = DataSaved.MemoryStages[Position].Split(' ');
            string[] DataSpiltedAgain = userCode[Position].Split(' ');

            if (DataSpilted[0].ElementAt(10).ToString() == "0")
            {
                DataReadWrite.DistinationRegister = Convert.ToInt32(DataSpiltedAgain[3], 2);
                MipsRegisters[DataReadWrite.DistinationRegister] = int.Parse(DataSpilted[1]);
                LoadMipsRegistersTableView();
            }
            else
            {
                DataReadWrite.SecondRegister = Convert.ToInt32(DataSpiltedAgain[2], 2);
                MipsRegisters[DataReadWrite.SecondRegister] = 99;
                LoadMipsRegistersTableView();
            }
            if (DataSpilted[0].ElementAt(1).ToString() == "0" && DataSpilted[0].ElementAt(2).ToString() == "0")
                UpdatePipelineRegisterTableView(3, "EX_MEM", DataSpilted[3]);
            else
                UpdatePipelineRegisterTableView(3, "EX_MEM", DataSpilted[1]);
        }

        string PinsControlFills(int Position, string OppCode)
        {

            switch (OppCode) {
                case "100011":
                    
                    Controllers.Branch = "0";
                    Controllers.RegisterWrite = "1";
                    Controllers.MemoryRegister = "1";
                    Controllers.RegisterDistination = "0";
                    Controllers.AluOpp = "00";
                    Controllers.ALUSource = "1";
                    Controllers.MemeryRead = "1";
                    Controllers.MemryWrite = "0";
                    break;
                case "000000":
                    Controllers.Branch = "0";
                    Controllers.RegisterWrite = "1";
                    Controllers.MemoryRegister = "0";
                    Controllers.RegisterDistination = "1";
                    Controllers.AluOpp = "10";
                    Controllers.ALUSource = "0";
                    Controllers.MemeryRead = "0";
                    Controllers.MemryWrite = "0";
                    break;

            }
            registerRead(Position);
            Helpers.SignExtention = LoadSignExtention(Position);
            Controllers.EX = Controllers.RegisterDistination + Controllers.AluOpp + Controllers.ALUSource;
            Controllers.M = Controllers.MemeryRead + Controllers.MemryWrite + Controllers.Branch;
            Controllers.WE = Controllers.RegisterWrite + Controllers.MemoryRegister;
        return Controllers.EX + ":" + Controllers.M + ":" + Controllers.WE;
      }
        void FetchStage(int Postion)
        {
            Pipeline.IF_ID = Postion.ToString();
            UpdatePipelineRegisterTableView(0, "IF_ID", Pipeline.IF_ID);
        }

        void UpdatePipelineRegisterTableView(int Position, string Pipeline, string Data)
        {

            if (PiplineGrid.Rows.Count == Position + 1)
            {
                PiplineGrid.Rows.Insert(Position, Pipeline, Data);
            }
            else
            {
                PiplineGrid.Rows.RemoveAt(Position);
                PiplineGrid.Rows.Insert(Position, Pipeline, Data);
            }
        }


        int MuxRegisterSelector(string Selector)
        {
            if (Selector == "0")
                return DataReadWrite.SecondRegister;
            else
                return DataReadWrite.DistinationRegister;
        }

        void excuteStage(int Position)
        {
            string[] code = userCode[Position].Split(' ');
            string[] DataSpilted = DataSaved.DecodeStages[Position].Split(' ');
            string pass;
            if (code.Length == 6)
                pass = code[5];
            else
                pass = "0";
            int MuxResult = MuxRegisterSelector(DataSpilted[3].ElementAt(0).ToString());
            int result = ResultOfAlU(Position, pass, DataSpilted[3].ElementAt(1).ToString() + DataSpilted[3].ElementAt(2).ToString());
            Pipeline.EX_MEM = result.ToString() + " " + DataSpilted[3] + " " + MuxResult;
            string DataPass = result.ToString() + "  " + MuxResult;
            DataSaved.ExcuteStages.Add(Position, Pipeline.EX_MEM);
            UpdatePipelineRegisterTableView(2, "EX_MEM", DataPass);
        }



        void FirstCycle() {
    
            FetchStage(instructionNumber);
            StartPCTxt.Text = (instructionNumber+4).ToString();
        }
        void SecondCycle()
        {
            DecodeStage(instructionNumber);
            FetchStage(instructionNumber+4);
            StartPCTxt.Text = (instructionNumber+8).ToString();
        }
        void ThirdCycle()
        {
            excuteStage(instructionNumber);
            DecodeStage(instructionNumber+4);
            FetchStage(instructionNumber+8);
            StartPCTxt.Text = (instructionNumber + 12).ToString();
        }
        void FourthCycle()
        {
            MemoryAccessStage(instructionNumber);
            excuteStage(instructionNumber+4);
            DecodeStage(instructionNumber+8);
            FetchStage(instructionNumber+12);
            StartPCTxt.Text = (instructionNumber + 16).ToString();

        }
        void FifthCycle()
        {
            WriteBackStage(instructionNumber);
            MemoryAccessStage(instructionNumber+4);
            excuteStage(instructionNumber+8);
            DecodeStage(instructionNumber+12);
            FetchStage(instructionNumber+16);
            StartPCTxt.Text = (instructionNumber + 20).ToString();


        }
        void sixthCycle()
        {
            WriteBackStage(instructionNumber+4);
            MemoryAccessStage(instructionNumber+8);
            excuteStage(instructionNumber+12);
            DecodeStage(instructionNumber+16);
            UpdatePipelineRegisterTableView(0, "IF_ID", "0 No Instruction");

        }
        void seventhCycle()
        {
            WriteBackStage(instructionNumber+8);
            MemoryAccessStage(instructionNumber+12);
            excuteStage(instructionNumber+16);
            UpdatePipelineRegisterTableView(1, "ID_EX", "0 0 0 0");

        }
        void eigthCycle()
        {
            WriteBackStage(instructionNumber+12);
            MemoryAccessStage(instructionNumber+16);
            UpdatePipelineRegisterTableView(2, "EX_MEM", "0 0");
        }
        void ninthCycle()
        {
            WriteBackStage(instructionNumber+16);

        }

        void run() {

            switch (CycleCount) {

                case 1 :
                  FirstCycle();
                   break;
                case 2:
                    SecondCycle();
                    break;
                case 3:
                    ThirdCycle();
                    break;
                case 4:
                    FourthCycle();
                    break;
                case 5:
                    FifthCycle();
                    break;
                case 6:
                    sixthCycle();
                    break;
                case 7:
                    seventhCycle();
                    break;
                case 8:
                    eigthCycle();
                    break;
                case 9:
                    ninthCycle();
                    break;
                default:
                    MessageBox.Show("      Our CPU Run correctly \n");
                    break;



            }
            CycleCount++;


        }

        private void runCycleBtn_Click(object sender, EventArgs e)
        {
            run();
        }
    }
}
