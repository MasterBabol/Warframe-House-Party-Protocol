//////////////////////////////////////////////////////////////////////////////////
// VirtualInput for Warframe House Party Protocol
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

class VirtualInput
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    private struct INPUT
    {
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [DllImport("User32.dll")]
    private static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern int MapVirtualKey(int wCode, int wMapType);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    public static void KeyDown(int vk, bool extended)
    {
        INPUT input = new INPUT();
        input.Type = 1;
        input.Data.Keyboard.Scan = (ushort)MapVirtualKey(vk, 0);
        input.Data.Keyboard.Time = 0;
        input.Data.Keyboard.Flags = 8;
        if (extended)
            input.Data.Keyboard.Flags |= 1;
        SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
    }

    public static void KeyUp(int vk, bool extended)
    {
        INPUT input = new INPUT();
        input.Type = 1;
        input.Data.Keyboard.Scan = (ushort)MapVirtualKey(vk, 0);
        input.Data.Keyboard.Time = 0;
        input.Data.Keyboard.Flags = 10;
        if (extended)
            input.Data.Keyboard.Flags |= 1;
        SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
    }

    public static void MouseDown(MouseButton mb)
    {
        INPUT input = new INPUT();
        input.Type = 0;

        if (mb == MouseButton.Left)
            input.Data.Mouse.Flags = 0x02;
        else if (mb == MouseButton.Middle)
            input.Data.Mouse.Flags = 0x20;
        else
            input.Data.Mouse.Flags = 0x08;

        SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
    }

    public static void MouseUp(MouseButton mb)
    {
        INPUT input = new INPUT();
        input.Type = 0;

        if (mb == MouseButton.Left)
            input.Data.Mouse.Flags = 0x04;
        else if (mb == MouseButton.Middle)
            input.Data.Mouse.Flags = 0x40;
        else
            input.Data.Mouse.Flags = 0x10;

        SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
    }
}

