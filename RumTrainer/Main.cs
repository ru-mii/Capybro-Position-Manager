using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RumTrainer.INF;
using static RumTrainer.Toolkit;
using static RumTrainer.Saves;

namespace RumTrainer
{
    public partial class Main : Form
    {
        public Main() { InitializeComponent(); }
        public static Process gameProcess = null;
        long processSession = 0;

        IntPtr module_UnityPlayer = IntPtr.Zero;
        IntPtr moduleSize_UnityPlayer = IntPtr.Zero;

        IntPtr pointer_Position = IntPtr.Zero;

        public static bool formHotkeysOpen = false;

        public static int hotkey_SavePlayerPosition = 0;
        public static int hotkey_LoadPlayerPosition = 0;
        public static int hotkey_ChangePlayerSpeed = 0;
        public static int hotkey_ResetPlayerSpeed = 0;

        // gamepad object
        X.Gamepad gamepad = X.Gamepad_1;

        // holds all buttons, 0 = not pressed, 1 = pressed
        public int[] gamepadTable = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // one time click forcers
        public int[] gamepadTableReady = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] keyboardTableReady = new int[999];

        private void Main_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            selector.Select();

            // fill out keyboard table
            for (int i = 0; i < keyboardTableReady.Length; i++)
                keyboardTableReady[i] = 0;

            // load hotkeys
            int.TryParse(Read("settings", "KeyValueSavePlayerPosition"), out hotkey_SavePlayerPosition);
            int.TryParse(Read("settings", "KeyValueLoadPlayerPosition"), out hotkey_LoadPlayerPosition);
            int.TryParse(Read("settings", "KeyValueChangePlayerSpeed"), out hotkey_ChangePlayerSpeed);
            int.TryParse(Read("settings", "KeyValueResetPlayerSpeed"), out hotkey_ResetPlayerSpeed);

            label_HotkeySavePlayerPosition.Text = Read("settings", "KeyCodeSavePlayerPosition");
            label_HotkeyLoadPlayerPosition.Text = Read("settings", "KeyCodeLoadPlayerPosition");

            backgroundWorker_CheckProcess.RunWorkerAsync();
            backgroundWorker.RunWorkerAsync();
            backgroundWorker_ControllerState.RunWorkerAsync();
        }

        public Vector3 GetPlayerPosition()
        {
            byte[] rawInsidePointer = ReadMemoryBytes(pointer_Position, 8);
            IntPtr aPointer = (IntPtr)BitConverter.ToInt64(rawInsidePointer, 0);

            return new Vector3(
                ReadMemoryFloat(aPointer + 0x4),
                ReadMemoryFloat(aPointer + 0xC),
                ReadMemoryFloat(aPointer + 0x14));
        }

        public bool SetPlayerPosition(Vector3 vector)
        {
            byte[] rawInsidePointer = ReadMemoryBytes(pointer_Position, 8);
            IntPtr aPointer = (IntPtr)BitConverter.ToInt64(rawInsidePointer, 0);

            WriteMemory(aPointer + 0x4, BitConverter.GetBytes(vector.X));
            WriteMemory(aPointer + 0xC, BitConverter.GetBytes(vector.Y));
            WriteMemory(aPointer + 0x14, BitConverter.GetBytes(vector.Z));

            return true;
        }

        private void button_SavePlayerPosition_Click(object sender, EventArgs e)
        {
            Vector3 temp = GetPlayerPosition();
            textBox_PlayerPositionX.Text = temp.X.ToString(CultureInfo.InvariantCulture);
            textBox_PlayerPositionY.Text = temp.Y.ToString(CultureInfo.InvariantCulture);
            textBox_PlayerPositionZ.Text = temp.Z.ToString(CultureInfo.InvariantCulture);
            selector.Select();
        }

        private void button_LoadPlayerPosition_Click(object sender, EventArgs e)
        {
            Vector3 vector = new Vector3();

            if (float.TryParse(textBox_PlayerPositionX.Text, out vector.X) && float.TryParse(textBox_PlayerPositionY.Text, out vector.Y)
            && float.TryParse(textBox_PlayerPositionZ.Text, out vector.Z))
            {
                byte[] rawInsidePointer = ReadMemoryBytes(pointer_Position, 8);
                IntPtr aPointer = (IntPtr)BitConverter.ToInt64(rawInsidePointer, 0);

                if (vector.X != 0 && vector.Y != 0 && vector.Z != 0)
                {
                    WriteMemory(aPointer + 0x4, BitConverter.GetBytes(vector.X));
                    WriteMemory(aPointer + 0xC, BitConverter.GetBytes(vector.Y));
                    WriteMemory(aPointer + 0x14, BitConverter.GetBytes(vector.Z));
                }
                else ShowError("did not load position as one of the values was 0");
            }
            else ShowError("one of the position values is incorrect");
            selector.Select();
        }

        private void button_Hotkeys_Click(object sender, EventArgs e)
        {
            formHotkeysOpen = true;
            Hotkeys hotkeysForm = new Hotkeys();
            hotkeysForm.ShowDialog();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (!formHotkeysOpen)
                {
                    if (hotkey_SavePlayerPosition < 1000)
                    {
                        if (IsKeyPressed(hotkey_SavePlayerPosition))
                        {
                            if (keyboardTableReady[hotkey_SavePlayerPosition] != 1)
                            {
                                button_SavePlayerPosition.PerformClick();
                                keyboardTableReady[hotkey_SavePlayerPosition] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_SavePlayerPosition] = 0;
                    }
                    else
                    {
                        if (gamepadTable[hotkey_SavePlayerPosition - 1000] == 1 && gamepadTableReady[hotkey_SavePlayerPosition - 1000] == 0)
                        {
                            button_SavePlayerPosition.PerformClick();
                            gamepadTableReady[hotkey_SavePlayerPosition - 1000] = 1;
                        }
                    }

                    if (hotkey_LoadPlayerPosition < 1000)
                    {
                        if (IsKeyPressed(hotkey_LoadPlayerPosition))
                        {
                            if (keyboardTableReady[hotkey_LoadPlayerPosition] != 1)
                            {
                                button_LoadPlayerPosition.PerformClick();
                                keyboardTableReady[hotkey_LoadPlayerPosition] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_LoadPlayerPosition] = 0;
                    }
                    else
                    {
                        if (gamepadTable[hotkey_LoadPlayerPosition - 1000] == 1 && gamepadTableReady[hotkey_LoadPlayerPosition - 1000] == 0)
                        {
                            button_LoadPlayerPosition.PerformClick();
                            gamepadTableReady[hotkey_LoadPlayerPosition - 1000] = 1;
                        }
                    }

                    if (hotkey_ChangePlayerSpeed < 1000)
                    {
                        if (IsKeyPressed(hotkey_ChangePlayerSpeed))
                        {
                            if (keyboardTableReady[hotkey_ChangePlayerSpeed] != 1)
                            {
                                // button_ChangePlayerSpeed.PerformClick();
                                keyboardTableReady[hotkey_ChangePlayerSpeed] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_ChangePlayerSpeed] = 0;
                    }
                    else
                    {
                        if (gamepadTable[hotkey_ChangePlayerSpeed - 1000] == 1 && gamepadTableReady[hotkey_ChangePlayerSpeed - 1000] == 0)
                        {
                            // button_ChangePlayerSpeed.PerformClick();
                            gamepadTableReady[hotkey_ChangePlayerSpeed - 1000] = 1;
                        }
                    }

                    if (hotkey_ResetPlayerSpeed < 1000)
                    {
                        if (IsKeyPressed(hotkey_ResetPlayerSpeed))
                        {
                            if (keyboardTableReady[hotkey_ResetPlayerSpeed] != 1)
                            {
                                // button_ResetPlayerSpeed.PerformClick();
                                keyboardTableReady[hotkey_ResetPlayerSpeed] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_ResetPlayerSpeed] = 0;
                    }
                    else
                    {
                        if (gamepadTable[hotkey_ResetPlayerSpeed - 1000] == 1 && gamepadTableReady[hotkey_ResetPlayerSpeed - 1000] == 0)
                        {
                            // button_ResetPlayerSpeed.PerformClick();
                            gamepadTableReady[hotkey_ResetPlayerSpeed - 1000] = 1;
                        }
                    }

                    /////////////

                    if (hotkey_SavePlayerPosition < 1000)
                    {
                        if (IsKeyPressed(hotkey_SavePlayerPosition))
                        {
                            if (keyboardTableReady[hotkey_SavePlayerPosition] != 1)
                            {
                                button_SavePlayerPosition.PerformClick();
                                keyboardTableReady[hotkey_SavePlayerPosition] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_SavePlayerPosition] = 0;

                        if (IsKeyPressed(hotkey_LoadPlayerPosition))
                        {
                            if (keyboardTableReady[hotkey_LoadPlayerPosition] != 1)
                            {
                                button_LoadPlayerPosition.PerformClick();
                                keyboardTableReady[hotkey_LoadPlayerPosition] = 1;
                            }
                        }
                        else keyboardTableReady[hotkey_LoadPlayerPosition] = 0;

                        if (IsKeyPressed(hotkey_ChangePlayerSpeed))
                        {
                            if (keyboardTableReady[hotkey_ChangePlayerSpeed] != 1)
                            {

                            }
                        }
                        else keyboardTableReady[hotkey_ChangePlayerSpeed] = 0;

                        if (IsKeyPressed(hotkey_ResetPlayerSpeed))
                        {
                            if (keyboardTableReady[hotkey_ResetPlayerSpeed] != 1)
                            {

                            }
                        }
                        else keyboardTableReady[hotkey_ResetPlayerSpeed] = 0;
                    }
                    else
                    {
                        if (gamepadTable[hotkey_SavePlayerPosition - 1000] == 1 && gamepadTableReady[hotkey_SavePlayerPosition - 1000] == 0)
                        {
                            button_SavePlayerPosition.PerformClick();
                            gamepadTableReady[hotkey_SavePlayerPosition - 1000] = 1;
                        }

                        if (gamepadTable[hotkey_LoadPlayerPosition - 1000] == 1 && gamepadTableReady[hotkey_LoadPlayerPosition - 1000] == 0)
                        {
                            button_SavePlayerPosition.PerformClick();
                            gamepadTableReady[hotkey_LoadPlayerPosition - 1000] = 1;
                        }

                        if (gamepadTable[hotkey_ChangePlayerSpeed - 1000] == 1 && gamepadTableReady[hotkey_ChangePlayerSpeed - 1000] == 0)
                        {
                            // button_ChangePlayerSpeed.PerformClick();
                            gamepadTableReady[hotkey_ChangePlayerSpeed - 1000] = 1;
                        }

                        if (gamepadTable[hotkey_ResetPlayerSpeed - 1000] == 1 && gamepadTableReady[hotkey_ResetPlayerSpeed - 1000] == 0)
                        {
                            // button_ResetPlayerSpeed.PerformClick();
                            gamepadTableReady[hotkey_ResetPlayerSpeed - 1000] = 1;
                        }
                    }
                }

                Thread.Sleep(5);
            }
        }

        private void backgroundWorker_ControllerState_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // syncs with actions on the gamepad
                // for all signs check https://github.com/ru-mii/kangur/blob/main/gamepadSigns.png?raw=true
                if (gamepad.Update())
                {
                    if (gamepad.LTrigger_N != 0) gamepadTable[0] = 1;
                    else { gamepadTable[0] = 0; gamepadTableReady[0] = 0; }
                    if (gamepad.RTrigger_N != 0) gamepadTable[1] = 1;
                    else { gamepadTable[1] = 0; gamepadTableReady[1] = 0; }
                    if (gamepad.LBumper_down) gamepadTable[2] = 1;
                    else { gamepadTable[2] = 0; gamepadTableReady[2] = 0; }
                    if (gamepad.RBumper_down) gamepadTable[3] = 1;
                    else { gamepadTable[3] = 0; gamepadTableReady[3] = 0; }
                    if (gamepad.LStick_down) gamepadTable[4] = 1;
                    else { gamepadTable[4] = 0; gamepadTableReady[4] = 0; }
                    if (gamepad.RStick_down) gamepadTable[5] = 1;
                    else { gamepadTable[5] = 0; gamepadTableReady[5] = 0; }
                    if (gamepad.Back_down) gamepadTable[6] = 1;
                    else { gamepadTable[6] = 0; gamepadTableReady[6] = 0; }
                    if (gamepad.Start_down) gamepadTable[7] = 1;
                    else { gamepadTable[7] = 0; gamepadTableReady[7] = 0; }
                    if (gamepad.Dpad_Left_down) gamepadTable[8] = 1;
                    else { gamepadTable[8] = 0; gamepadTableReady[8] = 0; }
                    if (gamepad.Dpad_Up_down) gamepadTable[9] = 1;
                    else { gamepadTable[9] = 0; gamepadTableReady[9] = 0; }
                    if (gamepad.Dpad_Right_down) gamepadTable[10] = 1;
                    else { gamepadTable[10] = 0; gamepadTableReady[10] = 0; }
                    if (gamepad.Dpad_Down_down) gamepadTable[11] = 1;
                    else { gamepadTable[11] = 0; gamepadTableReady[11] = 0; }
                    if (gamepad.X_down) gamepadTable[12] = 1;
                    else { gamepadTable[12] = 0; gamepadTableReady[12] = 0; }
                    if (gamepad.Y_down) gamepadTable[13] = 1;
                    else { gamepadTable[13] = 0; gamepadTableReady[13] = 0; }
                    if (gamepad.B_down) gamepadTable[14] = 1;
                    else { gamepadTable[14] = 0; gamepadTableReady[14] = 0; }
                    if (gamepad.A_down) gamepadTable[15] = 1;
                    else { gamepadTable[15] = 0; gamepadTableReady[15] = 0; }
                }

                Thread.Sleep(5);
            }
        }

        private void backgroundWorker_CheckProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                long tempProcessSession = 0;
                bool foundProcess = false;

                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "Capybro")
                    {
                        gameProcess = process;
                        tempProcessSession = ((DateTimeOffset)gameProcess.StartTime).ToUnixTimeMilliseconds();
                        foundProcess = true;
                    }
                }

                if (foundProcess && processSession != tempProcessSession)
                {
                    foreach (ProcessModule module in gameProcess.Modules)
                    {
                        if (module.ModuleName.Equals("UnityPlayer.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            module_UnityPlayer = module.BaseAddress;
                            moduleSize_UnityPlayer = (IntPtr)module.ModuleMemorySize;
                            break;
                        }
                    }

                    IntPtr positionRegisterAddress = module_UnityPlayer + 0x571235;
                    byte[] oldCode1 = { 0xF2, 0x0F, 0x10, 0x30, 0xF2, 0x0F, 0x10, 0x78, 0x08, 0xF2, 0x44, 0x0F, 0x10, 0x40, 0x10, 0x48, 0x8B, 0x01, 0x66, 0x0F };
                    WriteMemory(positionRegisterAddress, oldCode1);
                    pointer_Position = SaveRegister(positionRegisterAddress, 15, "eax", true, IntPtr.Zero);

                    processSession = tempProcessSession;
                }

                Thread.Sleep(100);
            }
        }
    }
}
