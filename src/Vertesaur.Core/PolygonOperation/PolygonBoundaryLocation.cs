﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Vertesaur.PolygonOperation
{
    /// <summary>
    /// A location on a polygon boundary.
    /// </summary>
    public sealed class PolygonBoundaryLocation :
        IEquatable<PolygonBoundaryLocation>,
        IComparable<PolygonBoundaryLocation>
    {

        [Pure] private static int Compare(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            if (ReferenceEquals(a, b))
                return 0;
            if (ReferenceEquals(a, null))
                return -1;
            if (ReferenceEquals(b, null))
                return 1;
            return a.CompareTo(b);
        }

        // TODO: can it be made so that null is an invalid parameter for a or b? May help with performance?

        /// <inheritdoc/>
        public static bool operator ==(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return ReferenceEquals(null, a) ? ReferenceEquals(null, b) : a.Equals(b);
        }
        /// <inheritdoc/>
        public static bool operator !=(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return !(ReferenceEquals(null, a) ? ReferenceEquals(null, b) : a.Equals(b));
        }
        /// <inheritdoc/>
        public static bool operator >(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return Compare(a, b) > 0;
        }
        /// <inheritdoc/>
        public static bool operator >=(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return Compare(a, b) >= 0;
        }
        /// <inheritdoc/>
        public static bool operator <(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return Compare(a, b) < 0;
        }
        /// <inheritdoc/>
        public static bool operator <=(PolygonBoundaryLocation a, PolygonBoundaryLocation b) {
            return Compare(a, b) <= 0;
        }

        /// <summary>
        /// The ring index that the location is on.
        /// </summary>
        public int RingIndex { get; private set; }

        /// <summary>
        /// The segment index that the location is on.
        /// </summary>
        public int SegmentIndex { get; private set; }

        /// <summary>
        /// The approximate ratio along the segment that the location refers to.
        /// This property is not recommended to be used for calculations but is
        /// to be used for sorting.
        /// </summary>
        /// <remarks>
        /// A segment ratio of exactly 0.0 indicates that the location is the start point of the segment.
        /// A segment ratio of exactly 1.0 indicates that the location is the end point of the segment.
        /// If an intersection is created between the two points that has a ratio of 0 or 1,
        /// you may need to adjust it as these values have special meaning. Offset the value using
        /// a significantly small value if you must.
        /// Modification of this value may be OK as the location may need to be recalculated
        /// to account for the insertion of newly calculated intersection points.
        /// </remarks>
        public readonly double SegmentRatio;

        /// <summary>
        /// Constructs a new ring location.
        /// </summary>
        /// <param name="ringIndex">The index of the specified ring.</param>
        /// <param name="segmentIndex">The index of the specified segment.</param>
        /// <param name="segmentRatio">The approximate ratio of the location on the segment</param>
        public PolygonBoundaryLocation(int ringIndex, int segmentIndex, double segmentRatio) {
            Contract.Requires(ringIndex >= 0);
            Contract.Requires(segmentIndex >= 0);
            SegmentIndex = segmentIndex;
            SegmentRatio = segmentRatio;
            RingIndex = ringIndex;
        }

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() {
            return RingIndex ^ -SegmentIndex ^ SegmentRatio.GetHashCode();
        }

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) {
            return Equals(obj as PolygonBoundaryLocation);
        }

        /// <inheritdoc/>
        [Pure]
        public bool Equals(PolygonBoundaryLocation other) {
            return !ReferenceEquals(null, other)
                && SegmentIndex == other.SegmentIndex
                && SegmentRatio == other.SegmentRatio
                && RingIndex == other.RingIndex;
        }

        /// <inheritdoc/>
        [Pure]
        public override string ToString() {
            return String.Format("{0}:{1}:{2}", RingIndex, SegmentIndex, SegmentRatio);
        }

        /// <inheritdoc/>
        [Pure]
        public int CompareTo(PolygonBoundaryLocation other) {
            if (other == null)
                return 1;
            int compareResult;
            return (compareResult = RingIndex.CompareTo(other.RingIndex)) == 0
                ? (
                    (compareResult = SegmentIndex.CompareTo(other.SegmentIndex)) == 0
                    ? SegmentRatio.CompareTo(other.SegmentRatio)
                    : compareResult
                )
                : compareResult;
        }

        [ContractInvariantMethod]
        [Conditional("CONTRACTS_FULL")]
        private void CodeContractInvariant() {
            Contract.Invariant(RingIndex >= 0);
            Contract.Invariant(SegmentIndex >= 0);
        }

    }
}
