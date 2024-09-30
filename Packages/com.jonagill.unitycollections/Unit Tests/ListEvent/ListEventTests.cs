using NUnit.Framework;
using System;
using System.Text;
using UnityEngine.TestTools;

namespace UnityCollections.Tests
{
    public class ListEventTests
    {
        StringBuilder sb;
        ListEvent listEvent;
        ListEvent autoClearListEvent;

        [SetUp]
        public void SetUp() {
            sb = new StringBuilder();
            listEvent = new ListEvent();
            autoClearListEvent = new ListEvent(clearAfterInvoke: true);
        }

        private void AppendA()
        {
            sb.Append("A");
        }

        private void AppendB()
        {
            sb.Append("B");
        }

        private void AddAppendB()
        {
            listEvent += AppendB;
        }

        private void RemoveAppendB()
        {
            listEvent -= AppendB;
        }

        private void Clear()
        {
            listEvent.Clear();
        }

        [Test(ExpectedResult = "AB")]
        public string AddingActionsWorks()
        {
            listEvent += AppendA;
            listEvent += AppendB;

            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "ABAA")]
        public string AddingMultipleTimesWorks()
        {
            listEvent += AppendA;
            listEvent += AppendB;
            listEvent += AppendA;
            listEvent += AppendA;

            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "B")]
        public string RemovingActionsWorks()
        {
            listEvent += AppendA;
            listEvent += AppendB;
            listEvent -= AppendA;

            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "")]
        public string ClearingActionsWorks()
        {
            listEvent += AppendA;
            listEvent += AppendB;
            listEvent.Clear();

            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "")]
        public string RemovingNonExistentActionsIsFine()
        {
            listEvent -= AppendA;

            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "A AB")]
        public string AddingActionsDuringInvocationWorks()
        {
            listEvent += AddAppendB;
            listEvent += AppendA;

            listEvent.Invoke();
            sb.Append(" ");
            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "AB A")]
        public string RemovingActionsDuringInvocationWorks()
        {
            listEvent += RemoveAppendB;
            listEvent += AppendA;
            listEvent += AppendB;

            listEvent.Invoke();
            sb.Append(" ");
            listEvent.Invoke();
            return sb.ToString();
        }

        [Test(ExpectedResult = "AB ")]
        public string ClearingActionsDuringInvocationWorks()
        {
            listEvent += Clear;
            listEvent += AppendA;
            listEvent += AppendB;

            listEvent.Invoke();
            sb.Append(" ");
            listEvent.Invoke();
            return sb.ToString();
        }
        
        [Test(ExpectedResult = "A AB B")]
        public string ClearingActionsAfterInvocationWorks()
        {
            autoClearListEvent += AppendA;
            autoClearListEvent.Invoke();
            
            sb.Append(" ");

            // A should fire again, then get cleared
            autoClearListEvent += AppendA;
            autoClearListEvent.Invoke();
            autoClearListEvent += AppendB;

            autoClearListEvent.Invoke();
            sb.Append(" ");

            autoClearListEvent += AppendB;
            autoClearListEvent.Invoke();

            // Second Invoke should not result in changes
            autoClearListEvent.Invoke();
            return sb.ToString();
        }
        
        [Test(ExpectedResult = "ABAB A")]
        public string ReentrancyOnInvokeWorks()
        {
            int reentrancyCount = 1;
            Action reentrancy = () =>
            {
                reentrancyCount--;
                if (reentrancyCount >= 0)
                {
                    // Clearing shouldn't take effect until after the outer Invoke returns, nor should
                    // adding a new handler
                    listEvent.Clear();
                    listEvent += AppendA;

                    listEvent.Invoke();
                }
            };

            listEvent += reentrancy;
            listEvent += AppendA;
            listEvent += AppendB;
            listEvent.Invoke();

            sb.Append(" ");

            listEvent.Invoke();

            return sb.ToString();
        }
        
        [TestCase(true, ExpectedResult = "[A][B] [B]")]
        [TestCase(false, ExpectedResult = "[A] [B]")]
        public string ZeroArgumentEventWorks(bool useTryCatch)
        {
            Action actionA = () => sb.AppendFormat("[A]");
            Action actionB = () => sb.AppendFormat("[B]");
            Action actionThrows = () => { throw new Exception("Foo"); };
            ListEvent ev = new ListEvent(useTryCatch: useTryCatch);

            ev += actionA;
            ev += actionThrows;
            ev += actionB;
            InvokeExpectingException(useTryCatch, () => ev.Invoke());

            sb.Append(" ");

            ev -= actionA;
            ev -= actionThrows;
            ev.Invoke();

            return sb.ToString();
        }

        [TestCase(true, ExpectedResult = "[A1][B1] [B2]")]
        [TestCase(false, ExpectedResult = "[A1] [B2]")]
        public string OneArgumentEventWorks(bool useTryCatch)
        {
            Action<int> actionA = (a) => sb.AppendFormat("[A{0}]", a);
            Action<int> actionB = (a) => sb.AppendFormat("[B{0}]", a);
            Action<int> actionThrows = (a) => { throw new Exception("Foo"); };
            ListEvent<int> ev = new ListEvent<int>(useTryCatch: useTryCatch);

            ev += actionA;
            ev += actionThrows;
            ev += actionB;
            InvokeExpectingException(useTryCatch, () => ev.Invoke(1));

            sb.Append(" ");

            ev -= actionA;
            ev -= actionThrows;
            ev.Invoke(2);

            return sb.ToString();
        }

        [TestCase(true, ExpectedResult = "[A1X][B1X] [B2Y]")]
        [TestCase(false, ExpectedResult = "[A1X] [B2Y]")]
        public string TwoArgumentEventWorks(bool useTryCatch)
        {
            Action<int, string> actionA = (a, b) => sb.AppendFormat("[A{0}{1}]", a, b);
            Action<int, string> actionB = (a, b) => sb.AppendFormat("[B{0}{1}]", a, b);
            Action<int, string> actionThrows = (a, b) => { throw new Exception("Foo"); };
            ListEvent<int, string> ev = new ListEvent<int, string>(useTryCatch: useTryCatch);

            ev += actionA;
            ev += actionThrows;
            ev += actionB;
            InvokeExpectingException(useTryCatch, () => ev.Invoke(1, "X"));

            sb.Append(" ");

            ev -= actionA;
            ev -= actionThrows;
            ev.Invoke(2, "Y");

            return sb.ToString();
        }

        [TestCase(true, ExpectedResult = "[A1X5][B1X5] [B2Y6]")]
        [TestCase(false, ExpectedResult = "[A1X5] [B2Y6]")]
        public string ThreeArgumentEventWorks(bool useTryCatch)
        {
            Action<int, string, int> actionA = (a, b, c) => sb.AppendFormat("[A{0}{1}{2}]", a, b, c);
            Action<int, string, int> actionB = (a, b, c) => sb.AppendFormat("[B{0}{1}{2}]", a, b, c);
            Action<int, string, int> actionThrows = (a, b, c) => { throw new Exception("Foo"); };
            ListEvent<int, string, int> ev = new ListEvent<int, string, int>(useTryCatch: useTryCatch);

            ev += actionA;
            ev += actionThrows;
            ev += actionB;
            InvokeExpectingException(useTryCatch, () => ev.Invoke(1, "X", 5));

            sb.Append(" ");

            ev -= actionA;
            ev -= actionThrows;
            ev.Invoke(2, "Y", 6);

            return sb.ToString();
        }

        private void InvokeExpectingException(bool useTryCatch, Action action)
        {
            if (useTryCatch)
            {
                LogAssert.Expect(UnityEngine.LogType.Exception, "Exception: Foo");
                action.Invoke();
            }
            else
            {
                Assert.Throws<Exception>(() => action.Invoke());
            }
        }
    }
}