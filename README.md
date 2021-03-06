# WHAT IS THIS?
C#で平行実行の方法を確認したが、見様見真似で作った方法だと結果がおかしくなったので、何とか正しい結果を得られるように試行錯誤してみた。  
その結果を記す。  

##テストプログラムの概要
引数に渡された数値を＋１して戻り値で返す関数に、並行実行で0から9までの数値を渡してみる。  
（渡した値がおかしくなっているのをわかりやすくするために）その戻り値を合計した。  
並行実行は、以下のように実行している。  

- OKケース(クラスのコンストラクタに値を渡してインスタンスを生成し、メソッドを実行)
```
task = new testTask(i);
task.wait = r.Next(100, 300);
tasks.Add(Task.Run<int>(() => {return task.inc(); }));
```
- NGケース（メソッドの引数に値を渡す）
```
tasks.Add(Task.Run(() => { return inc(i, r.Next(100, 300)); }));
```

##結果
以下の結果を見ていただきたい。  
　OK 1/NG 1：x が並行実行の関数に渡す前の値。  
　OK 3/NG 3：x wait=y が並行実行の関数のなかに入った値とウエイト数。  
　OK 2/NG 2：x が並行実行の関数から帰ってきた値。  
　★OK/★NG xがOK、NGの戻り値の合計。  

★OK 55、★NG 66となっているのがおわかりだろうか。  
```
C:\test\testThread1\bin\debug>testThread1.exe
OK 1:0
OK 1:1
OK 1:2
OK 1:3
OK 1:4
OK 1:5
OK 1:6
OK 1:7
OK 1:8
OK 1:9
OK 3:1 wait=104
OK 3:3 wait=181
OK 3:5 wait=188
OK 3:6 wait=203
OK 3:8 wait=104
OK 3:7 wait=211
OK 3:2 wait=217
OK 3:4 wait=280
OK 3:0 wait=295
OK 3:9 wait=166
OK 2:1
OK 2:2
OK 2:3
OK 2:4
OK 2:5
OK 2:6
OK 2:7
OK 2:8
OK 2:9
OK 2:10
★OK55
NG 1:0
NG 1:1
NG 1:2
NG 1:3
NG 1:4
NG 1:5
NG 1:6
NG 1:7
NG 1:8
NG 1:9
NG 3:3 wait=114
NG 3:6 wait=123
NG 3:4 wait=155
NG 3:2 wait=188
NG 3:1 wait=221
NG 3:5 wait=231
NG 3:10 wait=174
NG 3:8 wait=249
NG 3:7 wait=275
NG 3:10 wait=182
NG 2:2
NG 2:3
NG 2:4
NG 2:5
NG 2:6
NG 2:7
NG 2:8
NG 2:9
NG 2:11
NG 2:11
★NG66
```

- 値を渡す場合は、並行実行を行う用のクラスを作って、プロパティなりで値を渡してメンバ変数に保存した値を見る形にする。
- 自クラスのメソッドを呼び出すときは、値（引数）を渡さない用途でないとだめくさい。NGケースでメソッドに渡す引数はプリミティブなint型だし、参照渡しならわかるが値渡しなのになんでメソッドに入ると違う値（たぶん次に渡される値）になるのか理解できない。

理由がわからないのが口惜しい。  
→もしかして、スレッドプールが関係しているのかしらん。NGケースはfor文で使っている変数をメソッドに渡しているが、スレッドプールを使い切ったときに次のループに入ると変数値が書き換わって、スレッドプールに空いたスレッドが戻ってきて、そしたら書き換わった変数でメソッドが実行されるという・・・。検証するとしたらどうすれば？
