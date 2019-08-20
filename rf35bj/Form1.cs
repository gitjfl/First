using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace rf35bj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            byte[] by = new byte[] { };
        }

        private string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += "%" + Convert.ToString(b[i], 16);
            }
            return result;
        }


        public int icdev;
        int st;
        byte[] snr = new byte[5];
        private void button1_Click(object sender, EventArgs e)
        {
            icdev = Program.rf_usbopen();
            if (icdev > 0)
            {
                listBox1.Items.Add("Com Connect success!");
                byte[] status = new byte[30];
                st = Program.rf_get_status(icdev, status);
                //lbHardVer.Text=System.Text.Encoding.ASCII.GetString(status);
                listBox1.Items.Add(System.Text.Encoding.Default.GetString(status));
                Program.rf_beep(icdev, 25);
            }
            else
                listBox1.Items.Add("Com Connect failed!");

            byte[] key = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            int mode = 0;
            int sector = 0;
            for (int i = 0; i < 16; i++)
            {
                st = Program.rf_load_key(icdev, mode, sector, key);
                if (st != 0)
                {
                    string s1 = Convert.ToString(sector);
                    listBox1.Items.Add(s1 + " sector rf_load_key error!");
                }
                sector++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int sector = 0;
            st = Program.rf_card(icdev, 1, snr);
            if (st != 0)
            {
                listBox1.Items.Add("rf_card error");
            }
            else
            {
                byte[] snr1 = new byte[8];
                string snvalue = string.Empty;
                listBox1.Items.Add("rf_card right!");
                Program.hex_a(snr, snr1, 4);
                snvalue = System.Text.Encoding.Default.GetString(snr1);
                listBox1.Items.Add(snvalue);
                listBox1.Items.Add(Convert.ToInt64(snvalue, 16));
            }
            st = Program.rf_authentication(icdev, 0, sector);
            if (st != 0)
            {
                listBox1.Items.Add("rf_authentication error!");
            }
            else
            {
                listBox1.Items.Add("rf_authentication right!");
            }

            byte[] data = new byte[16];
            string databuff = "123456";
            data = Encoding.Default.GetBytes(databuff);
            //st = Program.rf_write(icdev, sector * 4 + 1, data);
            //if (st != 0)
            //{
            //    listBox1.Items.Add("rf_write error!");
            //    listBox1.Items.Add(st.ToString());
            //}
            //else
            //{
            //    listBox1.Items.Add("rf_write right!");
            //}

            byte[] databuffer = new byte[16];

            st = Program.rf_read(icdev, sector * 4 + 0, databuffer);
            string s = string.Empty;
            for (int i = 0; i < 16; i++)
            {
                s += Convert.ToString(databuffer[i], 16);
            }
            listBox1.Items.Add(s);
            if (st != 0)
            {
                listBox1.Items.Add("rf_read error!");
                listBox1.Items.Add(st.ToString());
            }
            else
            {
                listBox1.Items.Add("rf_read right!");
                listBox1.Items.Add(System.Text.Encoding.Default.GetString(databuffer));
            }

            byte[] keya = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            byte[] keyb = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
            st = Program.rf_changeb3(icdev, sector, keya, 0, 0, 0, 1, 0, keyb);
            if (st != 0)
            {
                listBox1.Items.Add("rf_changeb3 error!");
                listBox1.Items.Add(st.ToString());
            }
            else
            {
                listBox1.Items.Add("rf_changeb3 right!");
            }
            Program.rf_beep(icdev, 50);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            st = Program.rf_usbclose(icdev);
            if (st == 0)
            {
                listBox1.Items.Add("断开连接！");
            }
        }
    }
}
