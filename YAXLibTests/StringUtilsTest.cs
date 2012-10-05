﻿// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using NUnit.Framework;

using YAXLib;

namespace YAXLibTests
{
    /// <summary>
    /// Summary description for StringUtilsTest
    /// </summary>
    [TestFixture]
    public class StringUtilsTest
    {
        [Test]
        public void RefineElementNameTest()
        {
            Assert.That(StringUtils.RefineLocationString(".."), Is.EqualTo(".."));
            Assert.That(StringUtils.RefineLocationString("."), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString("      "), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString(" /      \\ "), Is.EqualTo("."));
            Assert.That(StringUtils.RefineLocationString("ans"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("/ans"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("/ans/"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans/"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans/////"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("ans\\\\\\"), Is.EqualTo("ans"));
            Assert.That(StringUtils.RefineLocationString("..."), Is.EqualTo("___"));
            Assert.That(StringUtils.RefineLocationString("one / two / three / four "), Is.EqualTo("one/two/three/four"));
            Assert.That(StringUtils.RefineLocationString("one / two \\ three / four "), Is.EqualTo("one/two/three/four"));
            Assert.That(StringUtils.RefineLocationString("one / two / three and else / four "), Is.EqualTo("one/two/three_and_else/four"));
            Assert.That(StringUtils.RefineLocationString("one / two / .. / four "), Is.EqualTo("one/two/../four"));
            Assert.That(StringUtils.RefineLocationString("one / two / .. / four / "), Is.EqualTo("one/two/../four"));
            Assert.That(StringUtils.RefineLocationString("one / two / . . / four / "), Is.EqualTo("one/two/___/four"));
            Assert.That(StringUtils.RefineLocationString("one / two / two:words.are / four "), Is.EqualTo("one/two/two_words_are/four"));
            Assert.That(StringUtils.RefineLocationString("one-two-three-four"), Is.EqualTo("one-two-three-four"));
        }

        [Test]
        public void ExtractPathAndAliasTest()
        {
            TestPathAndAlias("one/two#name", "one/two", "name");
            TestPathAndAlias("one / two # name", "one / two", "name");
            TestPathAndAlias("one / two # name1 name2", "one / two", "name1 name2");
            TestPathAndAlias(" one / two # name1 name2", "one / two", "name1 name2");
            TestPathAndAlias(" one / two name1 name2 ", " one / two name1 name2 ", "");
            TestPathAndAlias(" one / two # name1 # name2 ", "one / two", "name1 # name2");
            TestPathAndAlias(" one / two # ", "one / two", "");
            TestPathAndAlias(" one / two #", "one / two", "");
            TestPathAndAlias("# one / two ", "", "one / two");
        }

        private static void TestPathAndAlias(string locationString, string expPath, string expAlias)
        {
            string path, alias;
            StringUtils.ExttractPathAndAliasFromLocationString(locationString, out path, out alias);
            Assert.That(path, Is.EqualTo(expPath));
            Assert.That(alias, Is.EqualTo(expAlias));
        }

        [Test]
        public void IsLocationAllGenericTest()
        {
            Assert.That(StringUtils.IsLocationAllGeneric(".."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("./.."), Is.True);
            Assert.That(StringUtils.IsLocationAllGeneric("../.."), Is.True);

            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/../two"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/../two"));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("../one/../two/.."));
            Assert.IsFalse(StringUtils.IsLocationAllGeneric("one/../two/.."));
        }

        [Test]
        public void DivideLocationOneStepTest()
        {
            string newLocation;
            string newElement;

            string location = "..";
            bool returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo(".."));
            Assert.That(newElement, Is.Null);
            Assert.That(returnValue, Is.False);

            location = ".";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo("."));
            Assert.That(newElement, Is.Null);
            Assert.That(returnValue, Is.False);

            location = "../..";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That("../..", Is.EqualTo(newLocation));
            Assert.That(newElement, Is.Null);
            Assert.That(returnValue, Is.False);

            location = "../../folder";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo("../.."));
            Assert.That(newElement, Is.EqualTo("folder"));
            Assert.That(returnValue, Is.True);

            location = "../../folder/..";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo("../../folder/.."));
            Assert.That(newElement, Is.Null);
            Assert.That(returnValue, Is.False);

            location = "one/two/three/four";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo("one/two/three"));
            Assert.That(newElement, Is.EqualTo("four"));
            Assert.That(returnValue, Is.True);

            location = "one";
            returnValue = StringUtils.DivideLocationOneStep(location, out newLocation, out newElement);
            Assert.That(newLocation, Is.EqualTo("."));
            Assert.That(newElement, Is.EqualTo("one"));
            Assert.That(returnValue, Is.True);
        }

        [Test]
        public void LooksLikeExpandedNameTest()
        {
            var falseCases = new[] {"", "    ", "{}", "{a", "{} ", " {}", " {} ", " {a} ", "{a}", "{a}    ", "something"};
            var trueCases = new[] {"{a}b", " {a}b ", " {a}b"};

            foreach (var falseCase in falseCases)
            {
                Assert.IsFalse(StringUtils.LooksLikeExpandedXName(falseCase));
            }

            foreach (var trueCase in trueCases)
            {
                Assert.That(StringUtils.LooksLikeExpandedXName(trueCase), Is.True);
            }
        } 
    }
}
