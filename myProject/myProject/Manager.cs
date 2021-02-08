
using myProject.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myProject
{

    public partial class Manager : Form
    {
        //userInfo存放从数据库表读取的用户信息
        public Dictionary<int, Dictionary<string, string>> userInfo = new Dictionary<int, Dictionary<string, string>>();
        DBService dbIris = DBService.getInstance("myProject");
        public Manager()
        {
            InitializeComponent();
        }
        //时间
        private void frmMain_Load(object sender, EventArgs e)
        {
            //labDateTime为显示时间的定时器，显示间隔时间为1秒
            this.label25.Text = DateTime.Now.ToString();
            this.timerDate.Interval = 1000;
            this.timerDate.Enabled = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //学生信息注册
        private void button3_Click(object sender, EventArgs e)
        {
            string fingerid = this.textBox3.Text;
            string number = this.textBox1.Text;
            string name = this.textBox2.Text;
            string sex = "未知";
            if (radioButton1.Checked)
                sex = "男";
            else if(radioButton2.Checked)
                sex = "女";
            string password = this.textBox4.Text;
            if (this.textBox3.Text != "" && this.textBox1.Text != "" && this.textBox2.Text != "" && this.textBox4.Text != "")
            {
                DialogResult result = MessageBox.Show("请确认是否添加该学生？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string sqlString = @"SELECT StuName FROM mp_stu WHERE StuID = '" + number + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if(dt.Rows.Count == 0)
                    {
                        sqlString = @"insert into [myProject].[dbo].[mp_stu] (StuID, StuName, Sex, Password, FingerPrint) 
                                   values('" + number + "','" + name + "','" + sex + "','" + password + "','" + fingerid + "')";
                        dbIris.ExecuteSql(sqlString);
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("已经注册过该学生", "提示", MessageBoxButtons.OK);
                    }
                    
                }
            }
            else
                MessageBox.Show("请先填写好信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void label22_Click(object sender, EventArgs e)
        {

        }
        //指纹识别调用
        public int identify(int Dev_ID)
        {
            //指纹识别函数，返回识别的用户ID号
            int res = 0;
            int pLongRun = new int();
            int pRecCount = new int();
            int pRetCount = new int();
            int ppPersons = new int();
            int pTemp = 0;
            int tempPtr = 0;
            //int Dev_ID = Convert.ToInt32(DevID.Text);
            //1.启动读取新考勤记录的线程
            res = FPService.CKT_GetClockingNewRecordEx(Dev_ID, ref pLongRun);
            if (res == 1)
            {
                //2. 循环读取缓存中的记录
                while (true)
                {
                    res = FPService.CKT_GetClockingRecordProgress(pLongRun, ref pRecCount, ref pRetCount, ref ppPersons);
                    if (res != 0)
                    {
                        pTemp = Marshal.SizeOf(FPService.clocking);
                        tempPtr = ppPersons;
                        for (int i = 0; i < pRetCount; i++)
                        {
                            FPService.RtlMoveMemory(ref FPService.clocking, ppPersons, pTemp);
                            ppPersons += ppPersons + pTemp;
                            if (FPService.clocking.PersonID < 0)
                            {
                                continue;
                            }
                            else
                            {
                                if (tempPtr != 0)
                                {
                                    FPService.CKT_FreeMemory(tempPtr);
                                }
                                FPService.CKT_ClearClockingRecord(Dev_ID, 0, 0);
                                //FPID.Text = Service.FPService.clocking.PersonID.ToString();
                                //IdentifyCallBack(clocking.PersonID);
                                return FPService.clocking.PersonID;
                            }
                        }
                    }
                    if (tempPtr != 0)
                    {
                        FPService.CKT_FreeMemory(tempPtr);
                        FPService.CKT_ClearClockingRecord(1, 0, 0);
                    }
                    if (res == 1)
                    {
                        break;
                    }
                }
            }
            return -1;
        }
        //打开指纹开始识别
        private void timer1_Tick(object sender, EventArgs e)
        {
            int Dev_ID = Convert.ToInt32(textBox5.Text);
            int FP_ID = identify(Dev_ID);
            if(FP_ID > 0)
            {
                textBox3.Text = FP_ID.ToString();
                for(int i = 0; i < userInfo.Count; i++)
                {
                    if(userInfo[i]["ID"] == textBox3.Text)
                    {
                        textBox2.Text = userInfo[i]["Name"];
                        break;
                    }
                }
                timer1.Enabled = false;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            common.common.getMachineForm().Show();
            this.Hide();
        }
        //显示当前时间
        private void timerDate_Tick(object sender, EventArgs e)
        {
            this.label25.Text = DateTime.Now.ToString();
        }
        //学生信息注册连接指纹
        private void button1_Click(object sender, EventArgs e)
        {
            //获取识别设备ID，连接指纹设备

            int Dev_ID = Convert.ToInt32(textBox5.Text);
            int res = FPService.CKT_RegisterUSB(Dev_ID, 0);
            if (res == 1)
                MessageBox.Show("连接成功");
            else
                MessageBox.Show("连接失败");


        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 3000;
            timer1.Enabled = true;
        }
        //管理员管理添加管理员（需要最高管理员权限）
        private void button9_Click(object sender, EventArgs e)
        {
            string name = this.textBox16.Text;
            string password = this.textBox10.Text;
            if (common.common.manageid == "00")
            {
                if (this.textBox16.Text != "" && this.textBox10.Text != "")
                {
                    DialogResult result = MessageBox.Show("请确认是否添加该管理员？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        string sqlString = @"SELECT ID FROM mp_admin WHERE AdminID = '" + name + "'";
                        sqlString = string.Format(sqlString);
                        DataTable dt = dbIris.GetDataTableBySql(sqlString);
                        if (dt.Rows.Count == 0)
                        {
                            sqlString = @"insert into [myProject].[dbo].[mp_admin] (AdminID, AdminPassword) 
                                   values('" + name + "','" + password + "')";
                            dbIris.ExecuteSql(sqlString);
                            MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("已经有这个管理员了", "提示", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                    MessageBox.Show("请先填写好信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("您不是最高管理员", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }
        //管理员管理删除管理员（需要最高管理员权限）
        private void button8_Click(object sender, EventArgs e)
        {
            string name = this.textBox16.Text;
            string password = this.textBox10.Text;
            if (common.common.manageid == "00")
            {
                if (this.textBox16.Text != "")
                {
                    DialogResult result = MessageBox.Show("请确认是否删除该管理员？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        string sqlString = @"delete from [myProject].[dbo].[mp_admin]
                                   where AdminID = '" + name + "'";
                        dbIris.ExecuteSql(sqlString);
                        MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK);
                    }
                }
                else
                    MessageBox.Show("请先填写好信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("您不是最高管理员", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //学生信息管理删除学生信息（需要最高管理员权限）
        private void button6_Click(object sender, EventArgs e)
        {
            string number = this.textBox12.Text;
            if (common.common.manageid == "00")
            {
                if (this.textBox12.Text != "")
                {
                    DialogResult result = MessageBox.Show("请确认是否删除该学生信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        string sqlString = @"delete from [myProject].[dbo].[mp_stu]
                                   where StuID = '" + number + "'";
                        dbIris.ExecuteSql(sqlString);
                        MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK);
                    }
                }
                else
                    MessageBox.Show("请先填写好信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("您不是最高管理员", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //学生信息管理添加课程
        private void button10_Click(object sender, EventArgs e)
        {
            string s_number = this.textBox12.Text;
            string c_number = this.textBox13.Text;
            string c_name = this.textBox9.Text;
            if (this.textBox12.Text != "")
            {
                if (this.textBox13.Text != "")
                {
                    string sqlString = @"SELECT Course FROM mp_course WHERE CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有此课程");
                    }
                    else 
                    {
                        DialogResult result = MessageBox.Show("请确认是否添加该课程？(" + c_number + ":" + dt.Rows[0][0].ToString() +")", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        dt.Clear();
                        if (result == DialogResult.Yes)
                        {
                            sqlString = @"SELECT Section FROM mp_course WHERE CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            for (int i = 1; i <= int.Parse(dt.Rows[0][0].ToString()); i++)
                            {
                                sqlString = @"insert into [myProject].[dbo].[mp_sign] (StuID, CourseSection, CourseID, Attendance) 
                                                values('" + s_number + "','" + i + "','" + c_number + "','')";
                                dbIris.ExecuteSql(sqlString);
                            }
                            dt.Clear();
                            MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK);
                        }
                    }
                }
                else if (this.textBox9.Text != "")
                {
                    string sqlString = @"SELECT CourseID FROM mp_course WHERE Course = '" + c_name + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有此课程");
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("请确认是否添加该课程？(" + dt.Rows[0][0].ToString() + ":" + c_name + ")", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        c_number = dt.Rows[0][0].ToString();
                        dt.Clear();
                        if (result == DialogResult.Yes)
                        {
                            sqlString = @"SELECT Section FROM mp_course WHERE CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            for (int i = 1; i <= int.Parse(dt.Rows[0][0].ToString()); i++)
                            {
                                sqlString = @"insert into [myProject].[dbo].[mp_sign] (StuID, CourseSection, CourseID, SignInTime, SignOutTime, Attendance) 
                                                values('" + s_number + "','" + i + "','" + c_number + "','','','')";
                                dbIris.ExecuteSql(sqlString);
                            }
                            dt.Clear();
                            MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                    MessageBox.Show("请先填写课程名或课程ID", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
                MessageBox.Show("请先填写学生ID或学生姓名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //课程信息导入
        private void button4_Click(object sender, EventArgs e)
        {
            string section = this.textBox11.Text;
            string name = this.textBox14.Text;
            string number = this.textBox15.Text;
            string c_name = this.textBox8.Text;
            string t_name = this.textBox7.Text;
            string l_in = this.comboBox1.Text + ":" + this.comboBox2.Text;
            string l_out = this.comboBox4.Text + ":" +  this.comboBox3.Text;
            string massage = this.textBox6.Text;
            if (this.textBox11.Text != "" && this.textBox14.Text != "" && this.textBox15.Text != "")
            {
                DialogResult result = MessageBox.Show("请确认是否添加该课程？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string sqlString = @"SELECT Course FROM mp_course WHERE CourseID = '" + number + "'";
                    dbIris.ExecuteSql(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if(dt.Rows.Count == 0)
                    {
                        sqlString = @"insert into [myProject].[dbo].[mp_course](Course, CourseID, Section, CheckInTime, CheckOutTime, ClassroomInformation, TeacherInformation, CourseInformation)
                                            values ('" + name + "','" + number + "','" + section + "','" + l_in + "','" + l_out + "','" + c_name + "','" + t_name + "','" + massage + "')";
                        dbIris.ExecuteSql(sqlString);
                        MessageBox.Show("已经添加", "提示", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("已经添加过课程", "提示", MessageBoxButtons.OK);
                    }
                    
                }
            }
            else
                MessageBox.Show("请先填写好信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //学生信息管理删除课程
        private void button5_Click(object sender, EventArgs e)
        {
            string s_number = this.textBox12.Text;
            string c_number = this.textBox13.Text;
            string c_name = this.textBox9.Text;
            if (this.textBox12.Text != "")
            {
                if (this.textBox13.Text != "")
                {
                    string sqlString = @"SELECT Course FROM mp_course WHERE CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有此课程");
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("请确认是否删除该课程？(" + c_number + ":" + dt.Rows[0][0].ToString() + ")", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        dt.Clear();
                        if (result == DialogResult.Yes)
                        {
                            sqlString = @"SELECT Section FROM mp_course WHERE CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            for (int i = 1; i <= int.Parse(dt.Rows[0][0].ToString()); i++)
                            {
                                sqlString = @"delete from [myProject].[dbo].[mp_sign] 
                                                   where CourseID = '" + c_number + "' and StuID = '" + s_number + "'";
                                dbIris.ExecuteSql(sqlString);
                            }
                            dt.Clear();
                            MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK);
                        }
                    }
                }
                else if (this.textBox9.Text != "")
                {
                    string sqlString = @"SELECT CourseID FROM mp_course WHERE Course = '" + c_name + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有此课程");
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("请确认是否删除该课程？(" + dt.Rows[0][0].ToString() + ":" + c_name + ")", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        c_number = dt.Rows[0][0].ToString();
                        dt.Clear();
                        if (result == DialogResult.Yes)
                        {
                            sqlString = @"SELECT Section FROM mp_course WHERE CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            for (int i = 1; i <= int.Parse(dt.Rows[0][0].ToString()); i++)
                            {
                                sqlString = @"delete from [myProject].[dbo].[mp_sign] 
                                                   where CourseID = '" + c_number + "' and StuID = '" + s_number + "')";
                                dbIris.ExecuteSql(sqlString);
                            }
                            dt.Clear();
                            MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                    MessageBox.Show("请先填写课程名或课程ID", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
                MessageBox.Show("请先填写学生ID或学生姓名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
