using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Rhino.Mocks.Constraints;

namespace AmazingCloudSearch.Test
{
    public delegate void MethodThatThrows();

    public static class TestHelper
    {
        const string TestdrivenNetAddin = "ProcessInvocation";
        private static bool? _runningInteractively;
        public static bool IsRunningInteractively()
        {
            if (!_runningInteractively.HasValue)
            {
                _runningInteractively = Process.GetCurrentProcess().ProcessName.Equals(TestdrivenNetAddin, StringComparison.OrdinalIgnoreCase);
            }
            return _runningInteractively.Value;
        }
    }

    public static class SpecificationExtensions
    {
        public static void ShouldHave<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            values.FirstOrDefault(func).ShouldNotBeNull();
        }

        public static void ShouldBeFalse(this bool condition, string errorMessage)
        {
            Assert.IsFalse(condition, errorMessage);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeTrue(this bool condition, string errorMessage)
        {
            Assert.IsTrue(condition, errorMessage);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void ShouldBeTrueBecause(this bool condition, string reason, params object[] args)
        {
            Assert.IsTrue(condition, reason, args);
        }

        public static object ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
            return expected;
        }

        public static object ShouldEqual(this string actual, object expected)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return null;
            }
            Assert.AreEqual(expected.ToString(), actual);
            return expected;
        }

        public static void ShouldMatch(this string actual, string pattern, bool ignoreCase = false)
        {
            Assert.That(actual, new RegexConstraint(pattern, ignoreCase));
        }

        public static XmlElement AttributeShouldEqual(this XmlElement element, string attributeName, object expected)
        {
            Assert.IsNotNull(element, "The Element is null");

            string actual = element.GetAttribute(attributeName);
            Assert.AreEqual(expected, actual);
            return element;
        }

        public static XmlElement AttributeShouldEqual(this XmlNode node, string attributeName, object expected)
        {
            var element = node as XmlElement;

            Assert.IsNotNull(element, "The Element is null");

            string actual = element.GetAttribute(attributeName);
            Assert.AreEqual(expected, actual);
            return element;
        }

        public static XmlElement ShouldHaveChild(this XmlElement element, string xpath)
        {
            var child = element.SelectSingleNode(xpath) as XmlElement;
            Assert.IsNotNull(child, "Should have a child element matching " + xpath);

            return child;
        }

        public static XmlElement DoesNotHaveAttribute(this XmlElement element, string attributeName)
        {
            Assert.IsNotNull(element, "The Element is null");
            Assert.IsFalse(element.HasAttribute(attributeName),
                           "Element should not have an attribute named " + attributeName);

            return element;
        }

        public static object ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
            return actual;
        }

        public static void ShouldBeNull(this object anObject)
        {
            Assert.IsNull(anObject);
        }

        public static T ShouldNotBeNull<T>(this T anObject)
        {
            Assert.IsNotNull(anObject);
            return anObject;
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
            return actual;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
            return actual;
        }

        public static T ShouldBeOfType<T>(this object actual)
        {
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType(typeof(T));
            return (T)actual;
        }

        public static T ShouldBeAssignableTo<T>(this object actual)
        {
            actual.ShouldNotBeNull();
            typeof(T).IsAssignableFrom(actual.GetType()).ShouldBeTrue();
            return (T)actual;
        }

        public static object ShouldBeOfType(this object actual, Type expected)
        {
            Assert.AreEqual(expected, actual.GetType());
            return actual;
        }

        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            Assert.IsNotInstanceOf(expected, actual);
        }

        public static void ShouldNotBeOfType<T>(this object actual)
        {
            ShouldNotBeOfType(actual, typeof(T));
        }

        public static void ShouldContain(this IList actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {            
            if (!actual.Any(t => t.Equals(expected)))
            {
                Assert.Fail(string.Format("The item '{0}' was not found in the sequence.", expected));
            }
        }

        public static void ShouldBeOrderedBy<T>(this IEnumerable<T> actual, Func<T, object> comparer)
        {
            var orderedList = actual.ToList().OrderBy(comparer);
            actual.ShouldHaveTheSameElementsAs(orderedList);
        }

        public static void ShouldBeOrderedByDesc<T>(this IEnumerable<T> actual, Func<T, object> comparer)
        {
            var orderedList = new List<T>(actual).OrderByDescending(comparer);
            actual.ShouldHaveTheSameElementsAs(orderedList);
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> actual)
        {
            Assert.Greater(actual.Count(), 0, "The list should have at least one element");
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected)
        {
            if (actual.Any(t => t.Equals(expected)))
            {
                Assert.Fail(string.Format("The item '{0}' was found in the sequence it should not be in.", expected));
            }
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected)
        {
            ShouldHaveTheSameElementsAs(actual, expected, (a, b) => a.ShouldEqual(b));
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected, Action<object, object> comparer)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            if (actual.Count != expected.Count)
            {
                Debug.WriteLine("Actual elements:");
                foreach (var list in actual)
                {
                    Debug.WriteLine("\t" + list);
                }
                Assert.Fail("Counts do not match. Expected {0} But Was {1}", expected.Count, actual.Count);
            }
            actual.Count.ShouldEqual(expected.Count);

            for (int i = 0; i < actual.Count; i++)
            {
                comparer(actual[i], expected[i]);
            }
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            ShouldHaveTheSameElementsAs(actual, expected, (a, b) => a.ShouldEqual(b));
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, params T[] expected)
        {
            ShouldHaveTheSameElementsAs(actual, expected, (a, b) => a.ShouldEqual(b));
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Action<T, T> comparer)
        {
            IList actualList = (actual is IList) ? (IList)actual : actual.ToList();
            IList expectedList = (expected is IList) ? (IList)expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList, (a, b) => comparer((T)a, (T)b));
        }

        public static void ShouldHaveTheSameElementKeysAs<TElement, TKey>(this IEnumerable<TElement> actual,
                                                                        IEnumerable expected,
                                                                        Func<TElement, TKey> keySelector)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            var actualArray = actual.ToArray();
            var expectedArray = expected.Cast<object>().ToArray();

            actualArray.Length.ShouldEqual(expectedArray.Length);

            for (var i = 0; i < actual.Count(); i++)
            {
                keySelector(actualArray[i]).ShouldEqual(expectedArray[i]);
            }
        }

        public static DateTime ShouldBeAboutTheSameTimeAs(this DateTime origDate, DateTime otherDate)
        {
            Math.Abs((origDate - otherDate).TotalSeconds).ShouldBeLessThan(2);

            return origDate;
        }

        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Greater(arg1, arg2);
            return arg2;
        }

        public static IComparable ShouldBeLessThanOrEqual(this IComparable arg1, IComparable arg2)
        {
            Assert.LessOrEqual(arg1, arg2);
            return arg2;
        }

        public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Less(arg1, arg2);
            return arg2;
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldBeEmpty(this string aString)
        {
            Assert.IsEmpty(aString);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.IsNotEmpty(collection);
        }

        public static void ShouldNotBeEmpty(this string aString)
        {
            Assert.IsNotNull(aString);
            Assert.IsNotEmpty(aString);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            StringAssert.Contains(expected, actual);
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            foreach (T obj in values)
                eachAction(obj);
            return values;
        }

        [DebuggerStepThrough]
        public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
        {
            foreach (object obj in values)
                eachAction(obj);
            return values;
        }

        public static void ShouldContainAllOf(this string actual, params string[] expectedItems)
        {
            
            expectedItems.Each(actual.ShouldContain);
        }

        public static void ShouldContainKey<T, U>(this IDictionary<T, U> dict, T key)
        {
            if (!dict.ContainsKey(key))
            {
                Assert.Fail("Expected dictionary to contain key '{0}', but it did not", key);
            }
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, Func<T, bool> expected)
        {
            actual.Count().ShouldBeGreaterThan(0);
            var result = actual.FirstOrDefault(expected);
            Assert.AreNotEqual(default(T), result, "Expected item was not found in the actual sequence");
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            Assert.That(actual, new NotConstraint(new SubstringConstraint(expected)));
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, Func<T, bool> expected)
        {
            if (!actual.Any()) return;

            actual.FirstOrDefault(expected).ShouldEqual(default(T));
        }

        public static string ShouldBeEqualIgnoringCase(this string actual, string expected)
        {
            StringAssert.AreEqualIgnoringCase(expected, actual);
            return expected;
        }

        public static void ShouldEndWith(this string actual, string expected)
        {
            StringAssert.EndsWith(expected, actual);
        }

        public static void ShouldNotEndWith(this string actual, string expected)
        {
            Assert.That(actual, new NotConstraint(new EndsWithConstraint(expected)));
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            StringAssert.StartsWith(expected, actual);
        }

        public static void ShouldContainErrorMessage(this Exception exception, string expected)
        {
            StringAssert.Contains(expected, exception.Message);
        }

        public static Exception ShouldNotBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;
            try
            {
                method();
            }

            catch (Exception e)
            {
                Assert.AreNotEqual(exceptionType, e.GetType(), e.ToString());
                exception = e;
            }

            if (exception != null)
            {
                Assert.Fail("Expected {0} to not be thrown.", exceptionType.FullName);
            }

            return exception;
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType(), e.ToString());
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail("Expected {0} to be thrown.", exceptionType.FullName);
            }

            return exception;
        }

        public static DateTime ToDateTime(this string value)
        {
            return DateTime.Parse(value);
        }

        public static void ShouldEqualSqlDate(this DateTime actual, DateTime expected)
        {
            var timeSpan = actual - expected;
            Assert.Less(Math.Abs(timeSpan.TotalMilliseconds), 3);
        }
        
        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected)
        {
            actual.Count().ShouldEqual(expected);
            return actual;
        }
        
        public class CapturingConstraint : AbstractConstraint
        {
            private readonly ArrayList _argList = new ArrayList();

            public override string Message
            {
                get { return ""; }
            }

            public override bool Eval(object obj)
            {
                _argList.Add(obj);
                return true;
            }

            public T First<T>()
            {
                return ArgumentAt<T>(0);
            }

            public T ArgumentAt<T>(int pos)
            {
                ensureWasCalled();
                return (T)_argList[pos];
            }

            private void ensureWasCalled()
            {
                if (_argList.Count == 0) Assert.Fail("Call was not made, so no arguments were captured.");
            }

            public T Second<T>()
            {
                return ArgumentAt<T>(1);
            }
        }

        public static void ShouldBeTrueEventually(this Func<bool> predicate, TimeSpan timeout)
        {
            ShouldBeTrueEventually(predicate, timeout, TimeSpan.FromSeconds(1));
        }

        public static void ShouldBeTrueEventually(this Func<bool> predicate, TimeSpan timeout, TimeSpan retryInterval)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                if (predicate())
                    return;

                Thread.Sleep(retryInterval);
            }
            while (stopwatch.Elapsed < timeout);

            stopwatch.Stop();

            Assert.Fail("After {0} seconds (retrying every {1} seconds) the predicate was always false.", timeout.TotalSeconds, retryInterval.TotalSeconds);
        } 
    }

    public class RegexConstraint : Constraint
    {
        readonly string _pattern;
        readonly bool _ignoreCase;

        public RegexConstraint(string pattern, bool ignoreCase = false)
        {
            _pattern = pattern;
            _ignoreCase = ignoreCase;
        }

        public override bool Matches(object actualValue)
        {
            actual = actualValue;
            return actual is string && Regex.IsMatch((string)actual, _pattern, _ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String regex matching");
            writer.WriteExpectedValue(_pattern);

            if (_ignoreCase)
            {
                writer.WriteModifier("ignoring case");
            }
        }
    }

    public static class Exception<T> where T : Exception
    {
        public static T ShouldBeThrownBy(Action action)
        {
            T exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e.ShouldBeOfType<T>();
            }

            if (exception == null) Assert.Fail("An exception was expected, but not thrown by the given action.");

            return exception;
        }
    }
}