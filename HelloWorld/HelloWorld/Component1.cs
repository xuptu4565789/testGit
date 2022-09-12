using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorld
{
    public partial class Component1 : Component
    {
        private int id = 5;
        private int group = 1;
        private int numOfChannel = 4;
        public decimal[] CurrentArray = { 0, 0, 0, 0 };
        public decimal[] VoltageArray = { 0, 0, 0, 0 };
        public decimal[] SettedVoltageArray = { 0, 0, 0, 0 };
        public decimal[] Capabilties = { 5, 9, 12, 15, 20};

        private enum functionCode {
            write = 0x06,
            read = 0x03
        }
        public Component1()
        {
            InitializeComponent();
        }
        public bool IsComOpen
        {
            get
            {
                return serialPort1.IsOpen;
            }
        }

        public Component1(IContainer container)
        {            
            //addParameter();
            //openSerialPort();
            container.Add(this);
            InitializeComponent();
        }

        public void SwitchSerialPort()
        {
            if(!serialPort1.IsOpen)
                serialPort1.Open();
            else
                serialPort1.Close();
        }


        public void AddParameter(object[] info)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.BaudRate = (int)info[1];
                serialPort1.PortName = (string)info[0];
                serialPort1.Parity = (System.IO.Ports.Parity)info[2];
                serialPort1.DataBits = 8;
                serialPort1.StopBits = (System.IO.Ports.StopBits)info[3];
                serialPort1.WriteTimeout = 500;
                serialPort1.ReadTimeout = 500;
            }
        }
        public void ReadAllSenseVoltage()
        {

        }

        public void SetLoad(bool isOn)
        {
            var writeCommand = ((int)functionCode.write).ToString("X2");
            var setLoad = "0E";
            var commandData = (isOn)?"01010101": "00000000";
            string command = id.ToString("X2") + writeCommand + group.ToString("X2") + setLoad + commandData;
            var commandAddCRC = AddCRC(command);

            byte[] byteCommand = StringToByteArray(commandAddCRC);

            try
            {
                serialPort1.Write(byteCommand, 0, byteCommand.Length);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }

        public void SetIOut(int channel, float current)
        {
            var writeCommand = ((int)functionCode.write).ToString("X2");
            var setIOut = channel.ToString("X2");
            byte[] currentByte = BitConverter.GetBytes(current);
            var commandData = ToHexString(current);
            
            //var commandData = "40400000";//3A
            string command = id.ToString("X2") + writeCommand + group.ToString("X2") + setIOut + commandData;
            var commandAddCRC = AddCRC(command);

            byte[] byteCommand = StringToByteArray(commandAddCRC);
            try
            {
                serialPort1.Write(byteCommand, 0, byteCommand.Length);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        internal void AddParameter()
        {
            throw new NotImplementedException();
        }

        public void SetVoltageIndexForTypeC(int channel, int index)
        {
            var writeCommand = ((int)functionCode.write).ToString("X2");
            var setVoltage = (channel+111).ToString("X2");
            var commandData = "000000" + index.ToString("X2");
            string command = id.ToString("X2") + writeCommand + group.ToString("X2") + setVoltage + commandData;
            var commandAddCRC = AddCRC(command);

            byte[] byteCommand = StringToByteArray(commandAddCRC);
            try
            {
                serialPort1.Write(byteCommand, 0, byteCommand.Length);

                //TODO: 如果沒有設定電壓需要重新設定
                //Thread.Sleep(1000);
                //ReadAllSettedTypeCVoltage();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        public void ReadAllSettedTypeCVoltage()
        {
            var readCommand = ((int)functionCode.read).ToString("X2");
            var readAllSettedTypeCVoltage = "0A";
            var commandData = "00000000";
            string command = id.ToString("X2") + readCommand + group.ToString("X2") + readAllSettedTypeCVoltage + commandData;
            var commandAddCRC = AddCRC(command);
            byte[] byteCommand = StringToByteArray(commandAddCRC);
            try
            {
                serialPort1.Write(byteCommand, 0, byteCommand.Length);
                Thread.Sleep(20);
                int length = serialPort1.BytesToRead;
                byte[] byteRecieveData = new byte[length];
                serialPort1.Read(byteRecieveData, 0, length);

                int dataIndex = 4;
                //int numOfChannel = 4;
                int dataSize = 4;//4 Byte byteRecieveData[0]為一個byte
                string recieve;

                for (int channel = 0; channel < numOfChannel; channel++)
                {
                    recieve = "";
                    for (int position = 0; position < dataSize; position++)
                    {
                        recieve += byteRecieveData[dataIndex + channel * numOfChannel + position].ToString("X2");
                    }
                    float value = FromHexString(recieve);
                    SettedVoltageArray[channel] = decimal.Round((decimal)value, 4);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void ReadAllVoltageAndCurrent()
        {
            var readCommand = ((int)functionCode.read).ToString("X2");
            var readVoltageAndCurrent = "04";
            var commandData = "00000000";
            string command = id.ToString("X2") + readCommand + group.ToString("X2") + readVoltageAndCurrent + commandData;
            var commandAddCRC = AddCRC(command);
            byte[] byteCommand = StringToByteArray(commandAddCRC);

            try
            {
                serialPort1.Write(byteCommand, 0, byteCommand.Length);
                Thread.Sleep(20);
                int length = serialPort1.BytesToRead;
                byte[] byteRecieveData = new byte[length];
                serialPort1.Read(byteRecieveData, 0, length);
                //data 39->byte index 4
                //data 38->byte index 5
                int currentIndex = 12;//data31
                int dataSize = 4;
                //int numOfChannel = 4;
                string recieve;

                for (int channel = 0; channel < numOfChannel; channel++)
                {
                    recieve = "";
                    for (int position = 0; position < dataSize; position++)
                    {
                        recieve += byteRecieveData[currentIndex + channel * numOfChannel + position].ToString("X2");
                    }
                    float value = FromHexString(recieve);
                    CurrentArray[channel] = decimal.Round((decimal)value, 4);
                }

                int voltageIndex = currentIndex + numOfChannel * dataSize;

                for (int channel = 0; channel < numOfChannel; channel++)
                {
                    recieve = "";
                    for (int position = 0; position < dataSize; position++)
                    {
                        recieve += byteRecieveData[voltageIndex + channel * numOfChannel + position].ToString("X2");
                    }
                    var value = FromHexString(recieve);
                    if (value >= 0)
                        VoltageArray[channel] = decimal.Round((decimal)value, 4);
                }

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
        public string ToHexString(float f)
        {
            f = 2;
            var bytes = BitConverter.GetBytes(f);
            var i = BitConverter.ToInt32(bytes, 0);
            return i.ToString("X8");
        }


        public float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            var bytes = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public string AddCRC(string command)
        {
            byte[] bytes = StringToByteArray(command);
            var CRC_register = bytes[0] ^ 0xFFFF;//最原始的CRC碼為0xFFFF
            int tmp;
            for (int cout = 0; cout < bytes.Length; cout++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    tmp = CRC_register;
                    tmp = tmp >> 1;//將目前的CRC碼右移一個bit
                    if (CRC_register % 2 == 1)//若前一次的CRC碼的LSB是1，則將目前的CRC碼與0xA001做XOR，反之不與0xA001做XOR
                        CRC_register = tmp ^ 0xA001;
                    else
                        CRC_register = tmp;
                }
                if (cout + 1 < bytes.Length)
                    CRC_register = bytes[cout + 1] ^ CRC_register;
            }
            return command + CRC_register.ToString("X4");
        }
    }
}
