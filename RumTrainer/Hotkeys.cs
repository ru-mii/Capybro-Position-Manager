using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RumTrainer.Saves;

namespace RumTrainer
{
    public partial class Hotkeys : Form
    {
        public Hotkeys() { InitializeComponent(); }

        bool selected_SavePosiiton = false;
        bool selected_LoadPlayerPosition = false;
        bool selected_ChangePlayerSpeed = false;
        bool selected_ResetPlayerSpeed = false;

        // holds all buttons, 0 = not pressed, 1 = pressed
        public int[] gamepadTable = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // gamepad one time click forcers
        public int[] gamepadTableReady = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private void Hotkeys_Load(object sender, EventArgs e)
        {
            selector.Select();

            textBox_SavePlayerPosition.Text = Read("settings", "KeyCodeSavePlayerPosition");
            textBox_LoadPlayerPosition.Text = Read("settings", "KeyCodeLoadPlayerPosition");
            //textBox_ChangePlayerSpeed.Text = Read("settings", "KeyCodeChangePlayerSpeed");
            //textBox_ResetPlayerSpeed.Text = Read("settings", "KeyCodeResetPlayerSpeed");

            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // gamepad object
            X.Gamepad gamepad = X.Gamepad_1;

            while (true)
            {
                if (gamepadTable.Sum() > 0)
                {
                    int padCode = -1;
                    for (int i = 0; i < gamepadTable.Length; i++)
                    {
                        if (gamepadTable[i] != 0)
                        {
                            padCode = i;
                            break;
                        }
                    }
                    
                    if (padCode != -1 && gamepadTableReady[padCode] == 0)
                    {
                        if (selected_SavePosiiton)
                        {
                            Main.hotkey_SavePlayerPosition = padCode + 1000;
                            Save("settings", "KeyCodeSavePlayerPosition", ((buttons)padCode).ToString());
                            Save("settings", "KeyValueSavePlayerPosition", (padCode + 1000).ToString());
                            textBox_SavePlayerPosition.Text = ((buttons)padCode).ToString();
                        }

                        else if (selected_LoadPlayerPosition)
                        {
                            Main.hotkey_LoadPlayerPosition = padCode + 1000;
                            Save("settings", "KeyCodeLoadPlayerPosition", ((buttons)padCode).ToString());
                            Save("settings", "KeyValueLoadPlayerPosition", (padCode + 1000).ToString());
                            textBox_LoadPlayerPosition.Text = ((buttons)padCode).ToString();
                        }

                        else if (selected_ChangePlayerSpeed)
                        {
                            Main.hotkey_ChangePlayerSpeed = padCode + 1000;
                            Save("settings", "KeyCodeChangePlayerSpeed", ((buttons)padCode).ToString());
                            Save("settings", "KeyValueChangePlayerSpeed", (padCode + 1000).ToString());
                            //textBox_ChangePlayerSpeed.Text = ((buttons)padCode).ToString();
                        }

                        else if (selected_ResetPlayerSpeed)
                        {
                            Main.hotkey_ResetPlayerSpeed = padCode + 1000;
                            Save("settings", "KeyCodeResetPlayerSpeed", ((buttons)padCode).ToString());
                            Save("settings", "KeyValueResetPlayerSpeed", (padCode + 1000).ToString());
                            //textBox_ResetPlayerSpeed.Text = ((buttons)padCode).ToString();
                        }

                        gamepadTableReady[padCode] = 1;
                    }
                }

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

                Thread.Sleep(1);
            }
        }

        private void textBox_SavePlayerPosition_KeyDown(object sender, KeyEventArgs e)
        {
            Label tempLabel = Application.OpenForms["Main"].Controls["label_HotkeySavePlayerPosition"] as Label;
            tempLabel.Text = e.KeyCode.ToString();

            Main.hotkey_SavePlayerPosition = e.KeyValue;
            Save("settings", "KeyCodeSavePlayerPosition", e.KeyCode.ToString());
            Save("settings", "KeyValueSavePlayerPosition", e.KeyValue.ToString());
            textBox_SavePlayerPosition.Text = e.KeyCode.ToString();
        }

        private void textBox_LoadPlayerPosition_KeyDown(object sender, KeyEventArgs e)
        {
            Label tempLabel = Application.OpenForms["Main"].Controls["label_HotkeyLoadPlayerPosition"] as Label;
            tempLabel.Text = e.KeyCode.ToString();

            Main.hotkey_LoadPlayerPosition = e.KeyValue;
            Save("settings", "KeyCodeLoadPlayerPosition", e.KeyCode.ToString());
            Save("settings", "KeyValueLoadPlayerPosition", e.KeyValue.ToString());
            textBox_LoadPlayerPosition.Text = e.KeyCode.ToString();
        }

        private void textBox_ChangePlayerSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            Main.hotkey_ChangePlayerSpeed= e.KeyValue;
            Save("settings", "KeyCodeChangePlayerSpeed", e.KeyCode.ToString());
            Save("settings", "KeyValueChangePlayerSpeed", e.KeyValue.ToString());
            //textBox_ChangePlayerSpeed.Text = e.KeyCode.ToString();
        }

        private void textBox_ResetPlayerSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            Main.hotkey_ResetPlayerSpeed = e.KeyValue;
            Save("settings", "KeyCodeResetPlayerSpeed", e.KeyCode.ToString());
            Save("settings", "KeyValueResetPlayerSpeed", e.KeyValue.ToString());
            //textBox_ResetPlayerSpeed.Text = e.KeyCode.ToString();
        }

        // selected flag
        private void textBox_SavePlayerPosition_Enter(object sender, EventArgs e) { selected_SavePosiiton = true; }
        private void textBox_SavePlayerPosition_Leave(object sender, EventArgs e) { selected_SavePosiiton = false; }
        private void textBox_LoadPlayerPosition_Enter(object sender, EventArgs e) { selected_LoadPlayerPosition = true; }
        private void textBox_LoadPlayerPosition_Leave(object sender, EventArgs e) { selected_LoadPlayerPosition = false; }
        private void textBox_ChangePlayerSpeed_Enter(object sender, EventArgs e) { selected_ChangePlayerSpeed = true; }
        private void textBox_ChangePlayerSpeed_Leave(object sender, EventArgs e) { selected_ChangePlayerSpeed = false; }
        private void textBox_ResetPlayerSpeed_Enter(object sender, EventArgs e) { selected_ResetPlayerSpeed = true; }
        private void textBox_ResetPlayerSpeed_Leave(object sender, EventArgs e) { selected_ResetPlayerSpeed = false; }

        // all gamepad buttons and their index
        enum buttons
        {
            p_ltrigger = 0,
            p_rtrigger = 1,
            p_lbumper = 2,
            p_rbumper = 3,
            p_lstick = 4,
            p_rstick = 5,
            p_back = 6,
            p_start = 7,
            p_ldpad = 8,
            p_udpad = 9,
            p_rdpad = 10,
            p_ddpad = 11,
            p_x = 12,
            p_y = 13,
            p_b = 14,
            p_a = 15
        }

        private void Hotkeys_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.formHotkeysOpen = false;
        }
    }
}
