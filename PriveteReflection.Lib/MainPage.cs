using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;


namespace PriveteReflection.Lib
{
    public class MainPage
    {
        /// <summary>
        /// 元ポインタは、dynamic で保存しておく。
        /// こうすると、動的にプロパティ/メソッドを呼び出しができるようになる。
        /// </summary>
        public object Target { get; set; }
        public dynamic text1 { get; set; }
        public dynamic btn1 { get; set; }

        public MainPage(object target)
        {
            // 元の MainPage をバインド
            this.Target = target;

            // private フィールドを探索する
            text1 = FindPropNoPublic("text1");
            btn1 = FindPropNoPublic("btn1");
            // クリックイベントを設定する
            Action<object, object> action = OnClickButton;
            AddEventHandler(this.btn1, "Click", action);
        }

        private int count = 0;

        private void OnClickButton(object sender, object e)
        {
            ++count;
            text1.Text = string.Format("reflection : {0} clicked.", count);
        }

        /// <summary>
        /// プライベートフィールドを探索する
        /// XAMLで生成される button1 などのフィールドが privete で作られているので
        /// これを見つけ出して割り当てるため
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object FindPropNoPublic(string name)
        {
            Type t = Target.GetType();
            var fi = t.GetTypeInfo().GetDeclaredField(name);
            var obj = fi.GetValue(Target);
            return obj;
        }

        /// <summary>
        /// イベントを設定する
        /// 特に Click イベントは RoutedEventArgs 設定する必要があるので、
        /// この方式を使う
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void AddEventHandler(object target, string eventName, Action<object, EventArgs> action)
        {
            var ei = target.GetType().GetRuntimeEvent(eventName);
            Type tt = ei.AddMethod.GetParameters()[0].ParameterType;
            var mi = ei.AddMethod;
            // イベントの型を変えるために一度ラムダ式でくるむ
            Action<object, object> handler = ((s, e) => { action(s, new EventArgs()); });
            var handlerInvoke = action.GetType().GetRuntimeMethod(
                "Invoke", new Type[] { typeof(object), typeof(Type[]) });
            var dele = handlerInvoke.CreateDelegate(tt, handler);
            mi.Invoke((object)target, new object[] { dele });
        }
    }
}
