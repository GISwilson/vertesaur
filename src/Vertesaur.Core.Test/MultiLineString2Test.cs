﻿// ===============================================================================
//
// Copyright (c) 2011 Aaron Dandy 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// ===============================================================================

using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

#pragma warning disable 1591

namespace Vertesaur.Core.Test {

	[TestFixture]
	public class MultilineString2Test {


		// ReSharper disable InconsistentNaming
		private List<LineString2> _lineStrings;
		// ReSharper restore InconsistentNaming

		[SetUp]
		public void SetUp() {
			_lineStrings = new[] {
			    new[]{new Point2(0,2),new Point2(0,1),new Point2(1,0),new Point2(6,0)},
			    new[]{new Point2(2,5),new Point2(2,2),new Point2(4,2),new Point2(4,-1)},
			    new[]{new Point2(4,2),new Point2(6,2),new Point2(3,-1)},
		    }
			.Select(points => new LineString2(points))
			.ToList();
		}

		[Test]
		public void MagnitudeTest() {
			var target = new MultiLineString2(_lineStrings);
			var expected = target.Sum(ls => ls.GetMagnitude());
			var actual = target.GetMagnitude();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void MagnitudeSquaredTest() {
			var target = new MultiLineString2(_lineStrings);
			var expected = target.Sum(ls => ls.GetMagnitude());
			expected = expected * expected;
			var actual = target.GetMagnitudeSquared();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void IntersectsPointTest() {
			var target = new MultiLineString2(_lineStrings);
			Assert.IsFalse(target.Intersects(new Point2(1, 2)));
			Assert.IsFalse(target.Intersects(new Point2(3, 3)));
			Assert.IsTrue(target.Intersects(new Point2(1, 0)));
			Assert.IsTrue(target.Intersects(new Point2(5, 1)));
		}

		[Test]
		public void GetMinimumBoundingRectangleTest() {
			var target = new MultiLineString2(_lineStrings);
			var expected = target[0].GetMbr();
			for (int i = 1; i < target.Count; i++) {
				expected = expected.Encompass(target[i].GetMbr());
			}
			var actual = target.GetMbr();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetCentroidTest() {
			var target = new MultiLineString2(_lineStrings);
			var expected = new Point2();
			var magSum = 0.0;
			foreach (var ls in target) {
				var mag = ls.GetMagnitude();
				var ct = ls.GetCentroid();
				expected = new Point2(expected.X + (ct.X * mag), expected.Y + (ct.Y * mag));
				magSum += mag;
			}
			expected = new Point2(expected.X / magSum, expected.Y / magSum);

			var actual = target.GetCentroid();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DistanceTest() {
			var target = new MultiLineString2(_lineStrings);
			Assert.AreEqual(1, target.Distance(new Point2(1, 2)));
			Assert.AreEqual(1, target.Distance(new Point2(3, 3)));
			Assert.AreEqual(0, target.Distance(new Point2(1, 0)));
			Assert.AreEqual(0, target.Distance(new Point2(5, 1)));
		}

		[Test]
		public void DistanceSquaredTest() {
			var target = new MultiLineString2(_lineStrings);
			Assert.AreEqual(1, target.DistanceSquared(new Point2(1, 2)));
			Assert.AreEqual(1, target.DistanceSquared(new Point2(3, 3)));
			Assert.AreEqual(0, target.DistanceSquared(new Point2(1, 0)));
			Assert.AreEqual(0, target.DistanceSquared(new Point2(5, 1)));
		}

		[Test]
		public void MultilineStringDefaultConstructorTest() {
			var target = new MultiLineString2();
			Assert.AreEqual(0, target.Count);
		}

		[Test]
		public void MultilineStringEstimatedCapacityConstructorTest() {
			var capacity = 10;
			var target = new MultiLineString2(capacity);
			Assert.AreEqual(0, target.Count);
		}

		[Test]
		public void MultilineStringPointCollectionsConstructorTest() {
			var target = new MultiLineString2(_lineStrings);
			Assert.AreEqual(3, target.Count);
			for (int i = 0; i < _lineStrings.Count; i++) {
				Assert.AreEqual(_lineStrings[i].Count, target[i].Count);
			}
		}

	}
}

#pragma warning restore 1591