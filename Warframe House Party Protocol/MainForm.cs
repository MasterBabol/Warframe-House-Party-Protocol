//////////////////////////////////////////////////////////////////////////////////
// MainForm for Warframe House Party Protocol
// Copyright (c) 2016 Babol.
// Contributed by Babol <babol@live.co.kr>
//
// This file is part of Warframe House Party Protocol.
//
// Warframe House Party Protocol is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3, or (at your option)
// any later version.
//
// Warframe House Party Protocol is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Warframe House Party Protocol; see the file COPYING.
// If not, see <http://www.gnu.org/licenses/>.
//
//////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Xml;

namespace Warframe_House_Party_Protocol
{
    public partial class MainForm : Form
    {
        private Thread BackgroundInputThread;
        private Thread BackgroundActivationThread;
        private bool activated = false;
        private DictionarySaver config = null;

        private String TitleString = "";

        private Int32 FireImmediateKey = 'Q';
        private Int32 FireContinuousKey = 'Y';

        public MainForm()
        {
            InitializeComponent();
            TitleString = Text;

            LoadConfig();
        }

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetForegroundWindow();

        private void MainForm_Load(object sender, EventArgs e)
        {
            BackgroundInputThread = new Thread(new ThreadStart(BackgroundInputDispatch));
            BackgroundActivationThread = new Thread(new ThreadStart (BackgroundActivationDispatch));

            BackgroundInputThread.Start();
            BackgroundActivationThread.Start();
        }

        private void LoadConfig()
        {
            config = new DictionarySaver("WHPP.config");
            String fistr = config.GetItem("FireImmediate");
            if (fistr != null && fistr.Length > 0)
                FireImmediateKey = fistr[0];
            String fcstr = config.GetItem("FireContinuous");
            if (fcstr != null && fcstr.Length > 0)
                FireContinuousKey = fcstr[0];

            String rest = config.GetItem("LastRestTime");
            if (rest != null)
                tbInterval.Value = Int32.Parse(rest);
            String charge = config.GetItem("LastChargeTime");
            if (charge != null)
                tbChargeTime.Value = Int32.Parse(charge);
            String usecharge = config.GetItem("LastChargeUse");
            if (usecharge != null)
                cbCharge.Checked = Convert.ToBoolean(usecharge);

            Point loc = Location;
            String lastleft = config.GetItem("LastWindowLeft");
            if (lastleft != null)
                loc.X = Int32.Parse(lastleft);
            String lasttop = config.GetItem("LastWindowTop");
            if (lasttop != null)
                loc.Y = Int32.Parse(lasttop);
            Location = loc;

            UpdateUI();
        }

        private void StoreConfig()
        {
            config.SetItem("FireImmediate", ((Char)FireImmediateKey).ToString());
            config.SetItem("FireContinuous", ((Char)FireContinuousKey).ToString());

            config.SetItem("LastRestTime", tbInterval.Value.ToString());
            config.SetItem("LastChargeTime", tbChargeTime.Value.ToString());

            config.SetItem("LastChargeUse", cbCharge.Checked.ToString());

            config.SetItem("LastWindowLeft", Location.X.ToString());
            config.SetItem("LastWindowTop", Location.Y.ToString());
        }

        private void UpdateUI()
        {
            lbFireImmediate.Text = "Immediate Fire: " + ((Char)FireImmediateKey).ToString();
            lbFireContinuous.Text = "Continuous Fire: " + ((Char)FireContinuousKey).ToString();
            lbInterval.Text = tbInterval.Value.ToString() + " ms";
            lbChargeTime.Text = tbChargeTime.Value.ToString() + " ms";
        }

        private void BackgroundInputDispatch()
        {
            for (;;)
            {
                StringBuilder sb = new StringBuilder(256);
                IntPtr window = GetForegroundWindow();
                GetWindowText(window, sb, sb.Capacity);

                if (sb.ToString().Equals("WARFRAME", StringComparison.CurrentCultureIgnoreCase))
                {
                    Text = TitleString + " [WARFRAME is foreground present]";

                    if (((VirtualInput.GetAsyncKeyState(FireImmediateKey) & 0x8000) == 0x8000))
                        btImmediate.BackColor = Color.Green;
                    else
                        btImmediate.BackColor = Color.Red;

                    int selected = 0;
                    if (rbAb1.Checked)
                        selected = 1;
                    else if (rbAb2.Checked)
                        selected = 2;
                    else if (rbAb3.Checked)
                        selected = 3;
                    else if (rbAb4.Checked)
                        selected = 4;
                    else if (rbMbL.Checked)
                        selected = 5;
                    else if (rbMelee.Checked)
                        selected = 6;

                    if (selected > 0 && (activated || ((VirtualInput.GetAsyncKeyState(FireImmediateKey) & 0x8000) == 0x8000)))
                    {
                        switch (selected)
                        {
                            case 1:
                                VirtualInput.KeyDown('1', false);
                                break;
                            case 2:
                                VirtualInput.KeyDown('2', false);
                                break;
                            case 3:
                                VirtualInput.KeyDown('3', false);
                                break;
                            case 4:
                                VirtualInput.KeyDown('4', false);
                                break;
                            case 5:
                                VirtualInput.MouseDown(VirtualInput.MouseButton.Left);
                                break;
                            case 6:
                                VirtualInput.KeyDown('E', false);
                                break;
                        }

                        if (cbCharge.Checked)
                            Thread.Sleep(tbChargeTime.Value);
                        else
                            Thread.Sleep(25);

                        switch (selected)
                        {
                            case 1:
                                VirtualInput.KeyUp('1', false);
                                break;
                            case 2:
                                VirtualInput.KeyUp('2', false);
                                break;
                            case 3:
                                VirtualInput.KeyUp('3', false);
                                break;
                            case 4:
                                VirtualInput.KeyUp('4', false);
                                break;
                            case 5:
                                VirtualInput.MouseUp(VirtualInput.MouseButton.Left);
                                break;
                            case 6:
                                VirtualInput.KeyUp('E', false);
                                break;
                        }
                        Thread.Sleep(tbInterval.Value - 50);
                    }
                }
                else
                    Text = TitleString + " [WARFRAME is not foreground present]";

                Thread.Sleep(25);
            }
        }

        private void BackgroundActivationDispatch()
        {
            Boolean previouslyPressed = false;
            for (;;)
            {
                if ((VirtualInput.GetAsyncKeyState(FireContinuousKey) & 0x8000) == 0x8000)
                {
                    if (previouslyPressed == false)
                    {
                        StringBuilder sb = new StringBuilder(256);
                        IntPtr window = GetForegroundWindow();
                        GetWindowText(window, sb, sb.Capacity);
                        if (sb.ToString().Equals("WARFRAME", StringComparison.CurrentCultureIgnoreCase))
                        {
                            previouslyPressed = true;
                            ToggleActivation();
                        }
                    }
                }
                else
                {
                    if (previouslyPressed == true)
                    {
                        previouslyPressed = false;
                    }
                }
                Thread.Sleep(25);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BackgroundActivationThread.Abort();
            BackgroundInputThread.Abort();

            StoreConfig();
            if (config != null)
                config.Dispose();
        }

        private void tbInterval_Scroll(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void ToggleActivation()
        {
            if (activated)
            {
                activated = false;
                btActivation.BackColor = Color.Red;
            }
            else
            {
                activated = true;
                btActivation.BackColor = Color.Green;
            }
        }

        private void btActivation_Click(object sender, EventArgs e)
        {
            ToggleActivation();
        }

        private void tbChargeTime_Scroll(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void rbNone_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
