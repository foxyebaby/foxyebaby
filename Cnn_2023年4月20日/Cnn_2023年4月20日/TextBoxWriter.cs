/*
 * 2023年4月3日-15点26分-
 * 
 * 这是一个继承自 TextWriter 类的自定义写入器，
 * 它将输出内容写入到一个 TextBox 控件中。具体来说，
 * 它重写了 WriteLine 方法和 Encoding 属性。
 * 其中，TextBox 对象和委托对象 writeAction 是在构造函数中传入的。
 * WriteLine 方法会将输出内容以字符串的形式传递给 UpdateTextBox 方法，
 * 然后再通过 Invoke 方法在 UI 线程中更新 TextBox 控件的文本内容。
 * 如果当前线程不是 UI 线程，需要使用 Invoke 方法将更新操作委托给 UI 线程执行。
 * 如果当前线程是 UI 线程，可以直接更新 TextBox 控件的文本内容。
 * Encoding 属性返回输出文本的编码方式，这里是 UTF-8 编码。
 * 这个属性重写是必须的，因为 Encoding 属性在 TextWriter 类中是抽象的，
 * 必须在派生类中实现
 * ---------需要在窗体载入的时候加入下面这个代码
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox1)); //将控制台的输出重定向到一个名为textBox1的文本框中
        }
 * 
 */

using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Cnn_2023年4月20日
{
    public class TextBoxWriter : TextWriter
    {
        private TextBox textBox;
        private Action<string> writeAction; // 委托

        #region MyRegion


        public TextBoxWriter(TextBox textBox, Action<string> writeAction = null)
        {
            this.textBox = textBox;
            this.writeAction = writeAction;
        }
        public override void WriteLine(string value)
        {
            // 在 TextBox 中显示输出
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action<string>(UpdateTextBox), value);
            }
            else
            {
                textBox.AppendText(Environment.NewLine + value);
            }
            // 调用委托
            if (writeAction != null)
            {
                writeAction(value);
            }
        }
        private void UpdateTextBox(string value)
        {
            textBox.AppendText(Environment.NewLine + value);
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        #endregion
    }
}