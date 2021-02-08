using myProject.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myProject
{
    
    public partial class Machine : Form
    {
        //userInfo存放从数据库表读取的用户信息
        public Dictionary<int, Dictionary<string, string>> userInfo = new Dictionary<int, Dictionary<string, string>>();
        DBService dbIris = DBService.getInstance("myProject");
        public Machine()
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
        //指纹识别函数
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
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
        //管理员登入
        private void button7_Click_1(object sender, EventArgs e)
        {
            if (this.textBox11.Text != "" && this.textBox14.Text != "")
            {
                common.common.manageid = textBox11.Text;
                string managepassword = textBox14.Text;
                string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '" + common.common.manageid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                DataTable dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == managepassword)
                {
                    textBox11.Text = "";
                    textBox14.Text = "";
                    common.common.getManagerForm().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
            }
            else
            {
                MessageBox.Show("需要填写所有信息");
            }
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
        //显示时钟
        private void timerDate_Tick_1(object sender, EventArgs e)
        {
            this.label25.Text = DateTime.Now.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int Dev_ID = Convert.ToInt32(textBox5.Text);
            int FP_ID = identify(Dev_ID);
            if (FP_ID > 0)
            {
                textBox3.Text = FP_ID.ToString();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    if (userInfo[i]["ID"] == textBox3.Text)
                    {
                        textBox2.Text = userInfo[i]["Name"];
                        break;
                    }
                }
                timer1.Enabled = false;
            }
        }
        //指纹设备启动
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
        //指纹设备识别
        private void button9_Click(object sender, EventArgs e)
        {
            timer1.Interval = 3000;
            timer1.Enabled = true;
            string number = this.textBox3.Text;
            string id;
            string name;
            string sqlString = @"SELECT StuID FROM mp_stu WHERE FingerPrint = " + number;
            sqlString = string.Format(sqlString);
            DataTable dt = dbIris.GetDataTableBySql(sqlString);
            if (dt != null && dt.Rows.Count == 0)
            {
                id = "没有找到此学生";
            }
            else
            {
                id = dt.Rows[0][0].ToString();
            }
                
            dt.Clear();
            this.textBox1.Text = id.ToString();
            sqlString = @"SELECT StuName FROM mp_stu WHERE FingerPrint = " + number;
            sqlString = string.Format(sqlString);
            dt = dbIris.GetDataTableBySql(sqlString);
            if (dt != null && dt.Rows.Count == 0)
            {
                name = "没有找到此学生";
            }
            else
            {
                name = dt.Rows[0][0].ToString();
            }
            this.textBox2.Text = name.ToString();
            dt.Clear();
        }
        //学生签到
        private void button2_Click(object sender, EventArgs e)
        {
            string s_number = this.textBox1.Text;
            string c_s = this.textBox10.Text;
            string c_number = this.textBox7.Text;
            string sqlString = "";
            DataTable dt;
            if (this.textBox1.Text != "" && this.textBox10.Text != "" && this.textBox7.Text != "")
            {
                sqlString = @"SELECT ID FROM mp_sign WHERE CourseID = '" + c_number + "'";
                sqlString = string.Format(sqlString);
                dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到此课程");
                    sqlString = "";
                }
                else
                {
                    dt.Clear();
                    sqlString = @"SELECT ID FROM mp_sign WHERE StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有找到此学生");
                        sqlString = "";
                    }
                    else
                    {
                        dt.Clear();
                        sqlString = @"SELECT ID FROM mp_sign WHERE CourseSection = '" + c_s + "' AND StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                        sqlString = string.Format(sqlString);
                        dt = dbIris.GetDataTableBySql(sqlString);
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            MessageBox.Show("课程节数错误，请重新修改课程节数");
                            sqlString = "";
                        }
                        else
                        {
                            dt.Clear();
                            sqlString = @"UPDATE mp_sign
                                            SET SignInTime = '" + DateTime.Now.ToString() +"' WHERE StuID = '" + s_number +"' and CourseID = '" + c_number +"' and CourseSection = '" + c_s +"';";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            textBox1.Text = "签到成功";
                        }
                    }
                }
                dt.Clear();
            }
            else
            {
                MessageBox.Show("请确认信息没有问题");
            }
        }
        //学生签退
        private void button3_Click(object sender, EventArgs e)
        {
            string s_number = this.textBox1.Text;
            string c_s = this.textBox10.Text;
            string c_number = this.textBox7.Text;
            string sqlString = "";
            DataTable dt;
            if (this.textBox1.Text != "" && this.textBox10.Text != "" && this.textBox7.Text != "")
            {
                sqlString = @"SELECT ID FROM mp_sign WHERE CourseID = '" + c_number + "'";
                sqlString = string.Format(sqlString);
                dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("没有找到此课程");
                    sqlString = "";
                }
                else
                {
                    dt.Clear();
                    sqlString = @"SELECT ID FROM mp_sign WHERE StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有找到此学生");
                        sqlString = "";
                    }
                    else
                    {
                        dt.Clear();
                        sqlString = @"SELECT ID FROM mp_sign WHERE CourseSection = '" + c_s + "' AND StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                        sqlString = string.Format(sqlString);
                        dt = dbIris.GetDataTableBySql(sqlString);
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            MessageBox.Show("课程节数错误，请重新修改课程节数");
                            sqlString = "";
                        }
                        else
                        {
                            dt.Clear();
                            sqlString = @"UPDATE mp_sign
                                            SET SignOutTime = '" + DateTime.Now.ToString() + "' WHERE StuID = '" + s_number + "' and CourseID = '" + c_number + "' and CourseSection = '" + c_s + "';";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            textBox1.Text = "签退成功";
                        }
                    }
                }
                dt.Clear();
            }
            else
            {
                MessageBox.Show("请确认信息没有问题");
            }
        }
        //管理员签到
        private void button5_Click_1(object sender, EventArgs e)
        {
            string s_number = this.textBox4.Text;
            string c_s = this.textBox10.Text;
            string c_number = this.textBox7.Text;
            string sqlString = "";
            DataTable dt;
            if (this.textBox11.Text != "" && this.textBox14.Text != "")
            {
                common.common.manageid = textBox11.Text;
                string managepassword = textBox14.Text;
                sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '" + common.common.manageid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == managepassword)
                {
                    if (this.textBox4.Text != "" && this.textBox10.Text != "" && this.textBox7.Text != "")
                    {
                        sqlString = @"SELECT ID FROM mp_sign WHERE CourseID = '" + c_number + "'";
                        sqlString = string.Format(sqlString);
                        dt = dbIris.GetDataTableBySql(sqlString);
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            MessageBox.Show("没有找到此课程");
                            sqlString = "";
                        }
                        else
                        {
                            dt.Clear();
                            sqlString = @"SELECT ID FROM mp_sign WHERE StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            if (dt != null && dt.Rows.Count == 0)
                            {
                                MessageBox.Show("没有找到此学生");
                                sqlString = "";
                            }
                            else
                            {
                                dt.Clear();
                                sqlString = @"SELECT ID FROM mp_sign WHERE CourseSection = '" + c_s + "' AND StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                                sqlString = string.Format(sqlString);
                                dt = dbIris.GetDataTableBySql(sqlString);
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    MessageBox.Show("课程节数错误，请重新修改课程节数");
                                    sqlString = "";
                                }
                                else
                                {
                                    dt.Clear();
                                    sqlString = @"SELECT CheckInTime FROM mp_course WHERE CourseID = '" + c_number + "'";
                                    sqlString = string.Format(sqlString);
                                    dt = dbIris.GetDataTableBySql(sqlString);
                                    string sit = dt.Rows[0][0].ToString();
                                    dt.Clear();
                                    sqlString = @"UPDATE mp_sign
                                            SET SignInTime = '" + sit + "' WHERE StuID = '" + s_number + "' and CourseID = '" + c_number + "' and CourseSection = '" + c_s + "';";
                                    sqlString = string.Format(sqlString);
                                    dt = dbIris.GetDataTableBySql(sqlString);
                                    MessageBox.Show("签到成功");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("需要填写所有信息");
                    }
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
                dt.Clear();
            }
            else
            {
                MessageBox.Show("需要填写所有信息");
            }
        }
        //管理员签退
        private void button4_Click_1(object sender, EventArgs e)
        {
            string s_number = this.textBox4.Text;
            string c_s = this.textBox10.Text;
            string c_number = this.textBox7.Text;
            string sqlString = "";
            DataTable dt;
            if (this.textBox11.Text != "" && this.textBox14.Text != "")
            {
                common.common.manageid = textBox11.Text;
                string managepassword = textBox14.Text;
                sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '" + common.common.manageid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == managepassword)
                {
                    if (this.textBox4.Text != "" && this.textBox10.Text != "" && this.textBox7.Text != "")
                    {
                        sqlString = @"SELECT ID FROM mp_sign WHERE CourseID = '" + c_number + "'";
                        sqlString = string.Format(sqlString);
                        dt = dbIris.GetDataTableBySql(sqlString);
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            MessageBox.Show("没有找到此课程");
                            sqlString = "";
                        }
                        else
                        {
                            dt.Clear();
                            sqlString = @"SELECT ID FROM mp_sign WHERE StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                            sqlString = string.Format(sqlString);
                            dt = dbIris.GetDataTableBySql(sqlString);
                            if (dt != null && dt.Rows.Count == 0)
                            {
                                MessageBox.Show("没有找到此学生");
                                sqlString = "";
                            }
                            else
                            {
                                dt.Clear();
                                sqlString = @"SELECT ID FROM mp_sign WHERE CourseSection = '" + c_s + "' AND StuID = '" + s_number + "' AND CourseID = '" + c_number + "'";
                                sqlString = string.Format(sqlString);
                                dt = dbIris.GetDataTableBySql(sqlString);
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    MessageBox.Show("课程节数错误，请重新修改课程节数");
                                    sqlString = "";
                                }
                                else
                                {
                                    dt.Clear();
                                    sqlString = @"SELECT CheckOutTime FROM mp_course WHERE CourseID = '" + c_number + "'";
                                    sqlString = string.Format(sqlString);
                                    dt = dbIris.GetDataTableBySql(sqlString);
                                    string sot = dt.Rows[0][0].ToString();
                                    dt.Clear();
                                    sqlString = @"UPDATE mp_sign
                                            SET SignOutTime = '" + sot + "' WHERE StuID = '" + s_number + "' and CourseID = '" + c_number + "' and CourseSection = '" + c_s + "';";
                                    sqlString = string.Format(sqlString);
                                    dt = dbIris.GetDataTableBySql(sqlString);
                                    MessageBox.Show("签退成功");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("需要填写所有信息");
                    }
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
                dt.Clear();
            }
            else
            {
                MessageBox.Show("需要填写所有信息");
            }
        }
        //当前课程查询
        private void button6_Click(object sender, EventArgs e)
        {
            string c_number = this.textBox7.Text;
            string c_name = this.textBox6.Text;
            string cl_name = "";
            string t_name = "";
            string l_in = "";
            string l_out = "";
            string sqlString = "";
            DataTable dt;
            if (this.textBox7.Text != "" || this.textBox6.Text != "")
            {
                if (this.textBox6.Text != "" && this.textBox7.Text == "")
                {
                    sqlString = @"SELECT CourseID FROM mp_course WHERE Course = '" + c_name + "'";
                    sqlString = string.Format(sqlString);
                    dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有找到此课程");
                        sqlString = "";
                    }
                    else
                    {
                        c_number = dt.Rows[0][0].ToString();
                    }
                    dt.Clear();
                    this.textBox7.Text = c_number.ToString();
                }
                else if(this.textBox7.Text != "" && this.textBox6.Text == "")
                {
                    sqlString = @"SELECT Course FROM mp_course WHERE CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    dt = dbIris.GetDataTableBySql(sqlString);
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        MessageBox.Show("没有找到此课程");
                        sqlString = "";
                    }
                    else
                    {
                        c_name = dt.Rows[0][0].ToString();
                    }
                    dt.Clear();
                    this.textBox6.Text = c_name.ToString();
                }
                if (sqlString != "")
                {
                    sqlString = @"SELECT CheckInTime, CheckOutTime, ClassroomInformation, TeacherInformation, CourseInformation FROM mp_course WHERE CourseID = '" + c_number + "'";
                    sqlString = string.Format(sqlString);
                    dt = dbIris.GetDataTableBySql(sqlString);
                    l_in = dt.Rows[0][0].ToString();
                    l_out = dt.Rows[0][1].ToString();
                    cl_name = dt.Rows[0][2].ToString();
                    t_name = dt.Rows[0][3].ToString();
                    dt.Clear();
                    this.textBox12.Text = l_in.ToString();
                    this.textBox13.Text = l_out.ToString();
                    this.textBox10.Text = "1";
                    this.textBox9.Text = cl_name.ToString();
                    this.textBox8.Text = t_name.ToString();
                }
            }
            else
            {
                MessageBox.Show("需要填写所有信息");
            }
        }
        //当前课程确定
        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox6.ReadOnly = true;
            this.textBox7.ReadOnly = true;
            this.textBox10.ReadOnly = true;
        }
        //当前课程清除
        private void button10_Click(object sender, EventArgs e)
        {
            if (this.textBox11.Text != "" && this.textBox14.Text != "")
            {
                common.common.manageid = textBox11.Text;
                string managepassword = textBox14.Text;
                string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '" + common.common.manageid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                DataTable dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == managepassword)
                {
                    this.textBox6.ReadOnly = false;
                    this.textBox7.ReadOnly = false;
                    this.textBox10.ReadOnly = false;
                    this.textBox6.Text = "";
                    this.textBox7.Text = "";
                    this.textBox8.Text = "";
                    this.textBox9.Text = "";
                    this.textBox10.Text = "";
                    this.textBox12.Text = "";
                    this.textBox13.Text = "";
                    this.textBox14.Text = "";
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
            }
            else
            {
                MessageBox.Show("需要管理员权限才能清空");
            }
        }
        //当前课程下课
        private void button11_Click(object sender, EventArgs e)
        {
            if (this.textBox11.Text != "" && this.textBox14.Text != "")
            {
                common.common.manageid = textBox11.Text;
                string managepassword = textBox14.Text;
                string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '" + common.common.manageid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                DataTable dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == managepassword)
                {
                    string CheckInTime = textBox12.Text.ToString();
                    string CheckOutTime = textBox13.Text.ToString();
                    string Section = textBox10.Text.ToString();
                    string CourseID = textBox7.Text.ToString();
                    sqlString = @"SELECT ID,SignInTime,SignOutTime,Attendance FROM mp_sign WHERE CourseID = '" + CourseID + "' and CourseSection = '" + Section + "'";
                    sqlString = string.Format(sqlString);
                    DataTable dt1 = dbIris.GetDataTableBySql(sqlString);
                    int a = dt1.Rows.Count ; 
                    for(int i = 0;i < a;i++)
                    {
                        string[] CheckInTimeArray = CheckInTime.Split(':');
                        string[] CheckOutTimeArray = CheckOutTime.Split(':');
                        string[] SignInTimeArray = null;
                        string[] SignOutTimeArray = null;
                        if (dt1.Rows[i][1].ToString()!="")
                        {
                            SignInTimeArray = dt1.Rows[i][1].ToString().Split(':');
                            
                        }
                        else
                        {
                            SignInTimeArray = CheckInTime.Split(':');
                            SignInTimeArray[1] = (int.Parse(SignInTimeArray[1]) + 10).ToString();
                        }
                        if (dt1.Rows[i][2].ToString() != "")
                        {
                            SignOutTimeArray = dt1.Rows[i][2].ToString().Split(':');
                        }
                        else
                        {
                            SignOutTimeArray = CheckOutTime.Split(':');
                            SignOutTimeArray[1] = (int.Parse(SignOutTimeArray[1]) - 10).ToString();
                        }
                        string Attendance = "1";
                        int InTime = (int.Parse(CheckInTimeArray[0])- int.Parse(SignInTimeArray[0])) * 60 + (int.Parse(CheckInTimeArray[1]) - int.Parse(SignInTimeArray[1]));//应到时间-实到时间
                        int OutTime = (int.Parse(CheckOutTimeArray[0]) - int.Parse(SignOutTimeArray[0])) * 60 + (int.Parse(CheckOutTimeArray[1]) - int.Parse(SignOutTimeArray[1]));//应退时间-实退时间
                        //MessageBox.Show(dt1.Rows[i][0] + ",,,," + dt1.Rows[i][1] + ",,,," + dt1.Rows[i][2] + ",,,," + InTime + ",,,," + OutTime);
                        if (InTime <= -5)
                        {
                            Attendance = "2";//迟到
                        }
                        if(OutTime >= 5 && Attendance == "2")
                        {
                            Attendance = "4";//缺勤
                        }
                        else if(OutTime >= 5)
                        {
                            Attendance = "3";//早退
                        }
                        sqlString = @"UPDATE mp_sign
                                            SET Attendance = " + Attendance + " WHERE ID = " + dt1.Rows[i][0].ToString() + ";";
                        sqlString = string.Format(sqlString);
                        dt = dbIris.GetDataTableBySql(sqlString);
                        dt.Clear();
                        Attendance = "1";
                    }

                    this.textBox6.ReadOnly = false;
                    this.textBox7.ReadOnly = false;
                    this.textBox10.ReadOnly = false;
                    this.textBox6.Text = "";
                    this.textBox7.Text = "";
                    this.textBox8.Text = "";
                    this.textBox9.Text = "";
                    this.textBox10.Text = "";
                    this.textBox12.Text = "";
                    this.textBox13.Text = "";
                    this.textBox14.Text = "";
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
            }
            else
            {
                MessageBox.Show("需要管理员权限才能结课");
            }
        }


    }
}
