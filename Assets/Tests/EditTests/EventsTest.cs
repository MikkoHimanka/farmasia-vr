﻿using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {

    // Only no data callbacks are tested. Callback with data code is almost identical though.
    public class EventsTest {

        private CallbackContainer callbacks;

        public int testValue;

        public static int a, b, c;
        public static string dataString;

        [SetUp]
        public void SetUp() {
            a = 0;
            b = 0;
            c = 0;
            callbacks = new CallbackContainer();
            testValue = 0;
            Events.Reset();
            dataString = null;
        }

        #region No data
        [Test]
        public void NoData_ResetWorks() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.Reset();

            Events.FireEvent(EventType.A);

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
        }

        [Test]
        public void NoData_CallbacksAreCalled() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.B, EventType.B);
            Events.SubscribeToEvent(callbacks.C, EventType.C);

            Events.FireEvent(EventType.A);
            Events.FireEvent(EventType.B);
            Events.FireEvent(EventType.C);

            Assert.AreEqual(1, callbacks.a, "Event callback was not called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");
            Assert.AreEqual(1, callbacks.c, "Event callback was not called");

            Assert.AreEqual(1, a, "Event callback was not called");
            Assert.AreEqual(1, b, "Event callback was not called");
            Assert.AreEqual(1, c, "Event callback was not called");
        }

        [Test]
        public void NoData_CallbackCanBeRemoved() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.B, EventType.B);
            Events.SubscribeToEvent(callbacks.C, EventType.C);

            Events.UnsubscribeFromEvent(callbacks.A, EventType.A);

            Events.FireEvent(EventType.A);
            Events.FireEvent(EventType.B);
            Events.FireEvent(EventType.C);

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");
            Assert.AreEqual(1, callbacks.c, "Event callback was not called");
        }
        [Test]
        public void NoData_OnlyMatchingCallbacksAreRemoved() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.B, EventType.A);

            Events.UnsubscribeFromEvent(callbacks.A, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");
        }
        [Test]
        public void NoData_AllMatchingCallbacksAreRemoved() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.B, EventType.A);

            Events.UnsubscribeFromEvent(callbacks.A, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
        }

        [Test]
        public void NoData_SameCallbackCanBeAddedTwice() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.A, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(2, callbacks.a, "Event callback was not called 2 times");
        }

        [Test]
        public void NoData_SubscriptionsCanBeOverwritten() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.A, EventType.A);

            Events.OverrideSubscription(callbacks.B, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(0, callbacks.a, "Event callback was not overridden");
            Assert.AreEqual(1, callbacks.b, "Overriding callback was not called");
        }

        [Test]
        public void NoData_CallbackSupportMultipleCallbacks() {

            Events.SubscribeToEvent(callbacks.A, EventType.A);
            Events.SubscribeToEvent(callbacks.B, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(1, callbacks.a, "Event callback A was not called");
            Assert.AreEqual(1, callbacks.b, "Event callback B was not called");
        }
        #endregion

        #region With data

        [Test]
        public void Data_ResetWorks() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.Reset();

            Events.FireEvent(EventType.A, CallbackData.String("a"));

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
            Assert.AreEqual(null, dataString, "Was called");
        }

        [Test]
        public void Data_CallbacksAreCalled() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.BData, EventType.B);
            Events.SubscribeToEvent(callbacks.CData, EventType.C);

            Events.FireEvent(EventType.A, CallbackData.String("a"));
            Events.FireEvent(EventType.B, CallbackData.String("b"));
            Events.FireEvent(EventType.C, CallbackData.String("c"));

            Assert.AreEqual(1, callbacks.a, "Event callback was not called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");
            Assert.AreEqual(1, callbacks.c, "Event callback was not called");

            Assert.AreEqual(1, a, "Event callback was not called");
            Assert.AreEqual(1, b, "Event callback was not called");
            Assert.AreEqual(1, c, "Event callback was not called");

            Assert.AreEqual("c", dataString, "Data was not transmitted succesfully");
        }

        [Test]
        public void Data_CallbackCanBeRemoved() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.BData, EventType.B);
            Events.SubscribeToEvent(callbacks.CData, EventType.C);

            Events.UnsubscribeFromEvent(callbacks.AData, EventType.A);

            Events.FireEvent(EventType.A, CallbackData.String("a"));
            Events.FireEvent(EventType.B, CallbackData.String("b"));
            Events.FireEvent(EventType.C, CallbackData.String("c"));

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");
            Assert.AreEqual(1, callbacks.c, "Event callback was not called");

            Assert.AreEqual("c", dataString, "Data was not transmitted succesfully");
        }
        [Test]
        public void Data_OnlyMatchingCallbacksAreRemoved() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.BData, EventType.A);

            Events.UnsubscribeFromEvent(callbacks.AData, EventType.A);

            Events.FireEvent(EventType.A, CallbackData.String("a"));

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
            Assert.AreEqual(1, callbacks.b, "Event callback was not called");

            Assert.AreEqual("a", dataString, "Data was not transmitted succesfully");
        }
        [Test]
        public void Data_AllMatchingCallbacksAreRemoved() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.BData, EventType.A);

            Events.UnsubscribeFromEvent(callbacks.AData, EventType.A);

            Events.FireEvent(EventType.A);

            Assert.AreEqual(0, callbacks.a, "Event callback was called");
        }

        [Test]
        public void Data_SameCallbackCanBeAddedTwice() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.AData, EventType.A);

            Events.FireEvent(EventType.A, CallbackData.String("a"));

            Assert.AreEqual(2, callbacks.a, "Event callback was not called 2 times");
        }

        [Test]
        public void Data_SubscriptionsCanBeOverwritten() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.AData, EventType.A);

            Events.OverrideSubscription(callbacks.BData, EventType.A);

            Events.FireEvent(EventType.A, CallbackData.String("a"));

            Assert.AreEqual(0, callbacks.a, "Event callback was not overridden");
            Assert.AreEqual(1, callbacks.b, "Overriding callback was not called");
        }

        [Test]
        public void Data_CallbackSupportMultipleCallbacks() {

            Events.SubscribeToEvent(callbacks.AData, EventType.A);
            Events.SubscribeToEvent(callbacks.BData, EventType.A);

            Events.FireEvent(EventType.A, CallbackData.String("a"));

            Assert.AreEqual(1, callbacks.a, "Event callback A was not called");
            Assert.AreEqual(1, callbacks.b, "Event callback B was not called");

            Assert.AreEqual("a", dataString);
        }
        #endregion
    }

    public class CallbackContainer {

        public int a, b, c;

        public CallbackContainer() {
            int a = 0;
            int b = 0;
            int c = 0;
        }

        public Action TestFunction;

        public void A() {
            EventsTest.a++;
            a++;
        }
        public void B() {
            EventsTest.b++;
            b++;
        }
        public void C() {
            EventsTest.c++;
            c++;
        }

        public void AData(CallbackData data) {
            EventsTest.a++;
            a++;
            EventsTest.dataString = data.DataString;
        }
        public void BData(CallbackData data) {
            EventsTest.b++;
            b++;
            EventsTest.dataString = data.DataString;
        }
        public void CData(CallbackData data) {
            EventsTest.c++;
            c++;
            EventsTest.dataString = data.DataString;
        }
    }
}