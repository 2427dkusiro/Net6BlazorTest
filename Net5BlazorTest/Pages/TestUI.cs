﻿using System.Threading.Tasks;
using System;

using TestImplement;

namespace Net5BlazorTest.Pages
{
    public class TestUI
    {
        public TestUI(string name, Func<Tester, Task> onClick)
        {
            Name = name;
            Status = "";
            OnClick = onClick;
        }

        public string Name { get; }

        public string Status { get; private set; }

        public Tester Tester { get; private set; }

        public Func<Tester, Task> OnClick { get; }

        public async Task Invoke()
        {
            var tester = new Tester();

            Status = "テスト実行中。お待ち下さい...";
            await Task.Delay(10);
            await OnClick(tester);
            Tester = tester;
            Status = "";
        }
    }
}
