using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using utils.Service;

namespace myProjectUser
{
    public partial class Login : Form
    {
        public Dictionary<int, Dictionary<string, string>> userInfo = new Dictionary<int, Dictionary<string, string>>();
        DBService dbIris = DBService.getInstance("myProjectuser");
        public Login()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "" && this.textBox2.Text != "")
            {
                common.common.stuid = textBox1.Text;
                string password = textBox2.Text;
                string sqlString = @"SELECT Password FROM mp_stu WHERE StuID = '" + common.common.stuid + "'";
                //string sqlString = @"SELECT AdminPassword FROM mp_admin WHERE AdminID = '00'";
                sqlString = string.Format(sqlString);
                DataTable dt = dbIris.GetDataTableBySql(sqlString);
                if (dt != null && dt.Rows.Count == 0)
                {
                    MessageBox.Show("用户不存在!");

                }
                else if (dt != null && dt.Rows[0][0].ToString() == password)
                {
                    common.common.getUserForm().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
            }
        }
    }
}
