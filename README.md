PrivateReflectionSample
===============
- How to search a private field.
- How to add method of reflection event

プライベートフィールドを探索する方法と、リフレクションを使ってイベントを登録する方法のサンプルコードです。

リフレクションを使った View と Logic の完全分離（Logicは Portable Class Library で作成）するための実験コードから切り出し。Windows Store 8.1 と Windows Phone 8.1 で共通 PCL を使う。似た感じで、Xamarin.Forms も作れるので、Windows.UI.Xaml と Xamarin.Forms.Xaml が共通コードで扱えるようになる。

# プライベートフィールドの探索

GetTypeInfo().GetDeclaredField(name) で、目的のプライベートフィールを取得できる。

```C#
text1 = FindPropNoPublic("text1");

public object FindPropNoPublic(string name)
{
    Type t = Target.GetType();
    var fi = t.GetTypeInfo().GetDeclaredField(name);
    var obj = fi.GetValue(Target);
    return obj;
}
```

# イベントの登録

```C#
Action<object, object> action = OnClickButton;
AddEventHandler(this.btn1, "Click", action);

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
```

# Author 

Tomoaki Masuda @moonmile

